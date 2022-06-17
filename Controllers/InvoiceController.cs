using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using physioCard.Domain;
using physioCard.Services;
using physioCard.wwwroot.lib.Mail;
using System.Collections.Generic;
using System.Net.Mail;

namespace physioCard.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly DashBoardController _dashBoardController;
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IClinicService _clinicService;
        private readonly IDoctorService _doctorService;

        public InvoiceController(IInvoiceService invoiceService, DashBoardController dashBoardController, IAppointmentService appointmentService, IPatientService patientService, IClinicService clinicService, IDoctorService doctorService)
        {
            _invoiceService = invoiceService;
            _dashBoardController = dashBoardController;
            _appointmentService = appointmentService;
            _patientService = patientService;
            _clinicService = clinicService;
            _doctorService = doctorService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> generateInvoice()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            List<Patient> patients = await _patientService.getallpatient(did);
            List<Patient> ipatients = new List<Patient>();
            foreach(var patient in patients)
            {
                Clinic clinic = await _clinicService.GetByClinicID(patient.clinicid);
                patient.clinicname = clinic.name;
                List<Appointment> appointments = await _invoiceService.getAppointmentbyPatientID(patient.patientID);
                if(appointments.Count > 0)
                {
                    ipatients.Add(patient);
                } 
            }
            ViewBag.patients = ipatients;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> getAppointmentbyPatientID(int patientID)
        {
            List<Appointment> appointments = await _invoiceService.getAppointmentbyPatientID(patientID);
            return Json(appointments);
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> createInvoice(int[] appointment_ids, int patientID, double discount)
        {
            try 
            {
                if(appointment_ids.Count() > 0)
                {
                    int did = (int)HttpContext.Session.GetInt32("docID");
                    ViewBag.data = await _dashBoardController.getProfileAsync(did);
                    List<Appointment> appointments = new List<Appointment>();
                    foreach (int id in appointment_ids)
                    {
                        Appointment appointment = await _appointmentService.getAppointmentByID(id);
                        appointments.Add(appointment);
                    }
                    Invoice invoice = new Invoice();
                    Random random = new Random();
                    invoice.invoiceNo = DateTime.Now.ToString("yyyyMMdd") + patientID.ToString() + random.Next(00,99).ToString();
                    invoice.patientID = patientID;
                    invoice.doctorID = did;
                    invoice.invoice_date = DateTime.Now;
                    invoice.total_appointment = appointments.Count;
                    invoice.discount = discount;
                    invoice.istart_date = appointments[0].date_start;
                    invoice.iend_date = appointments[appointments.Count - 1].date_start;

                    foreach (var item in appointments)
                    {
                        invoice.total_amount = invoice.total_amount + item.cost;
                    }
                    double discountamount = (invoice.discount * invoice.total_amount) / 100;
                    invoice.gross_amount = invoice.total_amount - discountamount;

                    bool status = await _invoiceService.createInvoice(invoice);

                    if (status)
                    {
                        await _invoiceService.updateAppointmentInvoiceStatus(appointments);
                        return RedirectToAction("yourInvoice", new { ino = invoice.invoiceNo });
                    }
                    else
                    {
                        List<Patient> patients = await _patientService.getallpatient(did);
                        foreach (var patient in patients)
                        {
                            Clinic clinic = await _clinicService.GetByClinicID(patient.clinicid);
                            patient.clinicname = clinic.name;
                        }
                        ViewBag.patients = patients;
                        ViewBag.invoice = false;
                        return View("generateInvoice");
                    }
                }
                return RedirectToAction("generateInvoice");
                
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Unable to create invoice. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
                return View();
            }
        }

        public async Task<IActionResult> yourInvoice(String ino)
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            Doctor doctor = await _doctorService.GetByIDAsync(did);
            Invoice invoice = await _invoiceService.getInvoiceByID(ino);
            Patient patient = await _patientService.getpatient(invoice.patientID);
            Clinic clinic = await _clinicService.GetByClinicID(patient.clinicid);
            List<Appointment> appointments = await _invoiceService.getAppointmentsinInvoice(invoice);
            foreach (var item in appointments)
            {
                item.endtime = item.starttime.Add(item.sessiontime.TimeOfDay);
            }

            string to = patient.email;
            string subject = "Your Invoice Dated On " + DateTime.Now.ToString("dd-MM-yyyy");
            string body = sendmail.CreateBody(invoice, patient, clinic, doctor);
            AlternateView alternateViewLogo = _invoiceService.CreateAlternateViewofLogo(body, clinic);
            AlternateView alternateViewSign = _invoiceService.CreateAlternateViewofSign(body, clinic);
            bool mailStatus = sendmail.sendingEmailWithImage(to, subject, body, alternateViewLogo, alternateViewSign);

            ViewBag.appointments = appointments;    
            ViewBag.invoice = invoice;
            ViewBag.patient = patient;
            ViewBag.clinic = clinic;
            ViewBag.doctor = doctor;
            return View();
        }

        public async Task<IActionResult> paymentStatus()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            List<Invoice> invoice = await _invoiceService.getAllInvoiceMonthly(did);
            ViewBag.invoice = invoice;
            return View();
        }

        [HttpPost]
        public async Task<bool> payStatusEdit(bool status, int invoiceID)
        {
            bool mystatus = await _invoiceService.payStatusEdit(status, invoiceID);
            return status;
        }

        public async Task<IActionResult> seeAllPendingInvoice()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            List<Invoice> invoice = await _invoiceService.seeAllPendingInvoice(did);
            ViewBag.allinvoice = invoice;
            return View();
        }
    }
}
