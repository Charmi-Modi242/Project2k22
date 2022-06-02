using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using physioCard.Domain;
using physioCard.Services;

namespace physioCard.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IClinicService _clinicService;
        private readonly DashBoardController _dashboardController;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public  PatientController(IPatientService patientService, IClinicService clinicService, DashBoardController dashBoardController, IWebHostEnvironment webHostEnvironment)
        {
            _patientService = patientService;
            _clinicService = clinicService;
            _dashboardController = dashBoardController;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> addPatient()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashboardController.getProfileAsync(did);
            string dname = await _patientService.getdocname(did);
            ViewBag.doc = "Dr. " + dname;
            ViewBag.docClinic = new SelectList(await _clinicService.GetByDoctorIDAsync(did), "clinicID", "name");
            return View("addPatient");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> addPatient(Patient patient)
        {
            try
            {
                if (ModelState.IsValid && patient.photo_img != null)
                {
                    patient.docid = (int)HttpContext.Session.GetInt32("docID");
                    ViewBag.docClinic = new SelectList(await _clinicService.GetByDoctorIDAsync(patient.docid), "clinicID", "name");
                    ViewBag.data = await _dashboardController.getProfileAsync(patient.docid);
                    // gets the file from post request
                    var theFile = HttpContext.Request.Form.Files.GetFile("photo_img");
                    // server location to store the files
                    string folder = "Patient/photo/";
                    // will add some garbage data in file name
                    folder += Guid.NewGuid().ToString().Substring(0, 8) + "_" + theFile.FileName;
                    // buiding the server path to the uploads directory
                    string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                    // saving the file
                    await theFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                    // storing file path string
                    patient.photo = folder;

                    await _patientService.addpatient(patient);

                    return RedirectToAction("showPatient");
                }
                else
                {
                    if (patient.photo_img == null)
                    {
                        ViewBag.error = "SELECT PROFILE PHOTO OF PATIENT";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Add Patient. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
                return View();
            }
        }

        public async Task<IActionResult> showPatient()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashboardController.getProfileAsync(did);
            List<Patient> patient = await _patientService.getallpatient(did);
            foreach (Patient item in patient)
            {
                if (item.medical_History == null)
                {
                    item.medical_History = "None";
                }
                TimeSpan ts = DateTime.Now - item.dob;
                int age = (int)ts.TotalDays / 365;
                item.age = age;
            }
            ViewBag.docClinic = new SelectList(await _clinicService.GetByDoctorIDAsync(did), "clinicID", "name");
            ViewBag.patients = patient;
            return View();
        }

        public async Task<IActionResult> editPatient(int id)
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashboardController.getProfileAsync(did);
            string dname = await _patientService.getdocname(did);
            ViewBag.doc = "Dr. " + dname;
            ViewBag.docClinic = new SelectList(await _clinicService.GetByDoctorIDAsync(did), "clinicID", "name");
            return View(await _patientService.getpatient(id));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> editPatient(Patient patient)
        {
            try
            {
                patient.docid = (int)HttpContext.Session.GetInt32("docID");
                ViewBag.data = await _dashboardController.getProfileAsync(patient.docid);
                if (ModelState.IsValid)
                {
                    

                    if (patient.photo_img == null)
                    {
                        patient.photo = patient.photo;
                    }
                    else
                    {
                        // gets the file from post request
                        var theFile = HttpContext.Request.Form.Files.GetFile("photo_img");
                        // server location to store the files
                        string folder = "Patient/photo/";
                        // will add some garbage data in file name
                        folder += Guid.NewGuid().ToString().Substring(0, 8) + "_" + theFile.FileName;
                        // buiding the server path to the uploads directory
                        string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                        // saving the file
                        await theFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                        // storing file path string
                        patient.photo = folder;
                    }

                    await _patientService.editpatient(patient);
                    return RedirectToAction("showPatientdetails", new { id = patient.patientID });
                }
                else
                {
                    if (patient.photo_img == null)
                    {
                        ViewBag.error = "SELECT PROFILE PHOTO OF PATIENT";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Edit Patient. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
                return View();
            }
        }

        public async Task<IActionResult> showPatientdetails(int id)
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashboardController.getProfileAsync(did);
            Patient patient = await _patientService.getpatient(id);
            Clinic clinic = await _clinicService.GetByClinicID(patient.clinicid);
            if (patient.medical_History == null)
            {
                patient.medical_History = "None";
            }
            patient.clinicname = clinic.name;
            TimeSpan ts = DateTime.Now - patient.dob;
            int age = (int)ts.TotalDays / 365;
            patient.age = age;
            return View(patient);
        }

        public async Task<IActionResult> GetPatientByID(int id)
        {
            //int did = (int)HttpContext.Session.GetInt32("docID");
            //ViewBag.data = await _dashboardController.getProfileAsync(did);
            Patient patient = await _patientService.getpatient(id);
            Clinic clinic = await _clinicService.GetByClinicID(patient.clinicid);
            patient.clinicname = clinic.name;
            //TimeSpan ts = DateTime.Now - patient.dob;
            //int age = (int)ts.TotalDays / 365;
            //patient.age = age;
            return Json(patient);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> searchByPatient(string txtname)
        {
            try
            {
                int did = (int)HttpContext.Session.GetInt32("docID");
                ViewBag.data = await _dashboardController.getProfileAsync(did);
                if (txtname != null)
                {
                    string fname = null, lname = null;
                    if (txtname.Contains(" "))
                    {
                        String[] strList = txtname.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        fname = strList[0];
                        if (strList.Length == 2)
                        {
                            lname = strList[1];
                        }
                        else
                        {
                            lname = "None";
                        }
                    }
                    else
                    {
                        fname = txtname;
                        lname = "None";
                    }
                    List<Patient> patient = await _patientService.getpatientbyname(did, fname, lname);
                    if (patient.Count > 0)
                    {
                        foreach (Patient item in patient)
                        {
                            if (item.medical_History == null)
                            {
                                item.medical_History = "None";
                            }

                            TimeSpan ts = DateTime.Now - item.dob;
                            int age = (int)ts.TotalDays / 365;
                            item.age = age;
                        }
                        return View(patient);
                    }
                    else
                    {
                        ViewBag.Error = "NO DATA FOUND";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "ENTER NAME TO SEARCH FIRST";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Search Patient. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
                return View();
            }
        }
    }
}