using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using physioCard.Domain;
using physioCard.Services;
using physioCard.wwwroot.lib.Mail;
using physioCard.wwwroot.lib.Securities;
using System.Security.Claims;

namespace physioCard.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IClinicService _clinicService;
        private readonly IDoctorClinicService _doctorClinicService;
        private readonly ILoginService _loginService;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly DashBoardController _dashBoardController;
        public DoctorController(IDoctorService doctorService, IClinicService clinicService, IDoctorClinicService doctorClinicService, ILoginService loginService, IWebHostEnvironment webHostEnvironment, DashBoardController dashBoardController)
        {
            _clinicService = clinicService;
            _doctorService = doctorService;
            _doctorClinicService = doctorClinicService;
            _loginService = loginService;
            _WebHostEnvironment = webHostEnvironment;
            _dashBoardController = dashBoardController;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> SignUp()
        {
            //ViewBag.clinicList = new MultiSelectList(await _clinicService.GetAllAsync(), "clinicID", "name");

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> SignUp(Doctor doctor)
        {
            try
            {
                //ViewBag.cliniclist = new MultiSelectList(await _clinicService.GetAllAsync());
                // checks whether all the built-in and custom validation is applied on all the data or not
                bool emailcheck = await _doctorService.checkemail(doctor.email);
                if (emailcheck)
                {
                    if (ModelState.IsValid)
                    {
                        // gets the file from post request
                        var theFile = HttpContext.Request.Form.Files.GetFile("photo_img");
                        // server location to store the files
                        string folder = "Doctor/photo/";
                        // will add some garbage data in file name
                        folder += Guid.NewGuid().ToString().Substring(0, 8) + "_" + theFile.FileName;
                        // buiding the server path to the uploads directory
                        string serverFolder = Path.Combine(_WebHostEnvironment.WebRootPath, folder);
                        // saving the file
                        await theFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                        // storing file path string
                        doctor.photo = folder;


                        // encrypting the password
                        doctor.password = CustomSecurity.ConvertToEncrypt(doctor.password);


                        // storing data in database
                        await _doctorService.SignUpAsync(doctor);
                        // redirecting page to login page
                        return View(nameof(Login));
                    }
                }
                else
                {
                    ViewBag.error = "YOU WON'T BE ABLE TO REGISTER USING THIS E-MAIL AS IT IS ALREADY REGISTERD. KINDLY USE ANOTHER E-MAIL ID.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Sign Up. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
            }


            return View(doctor);
        }
        [Authorize]
        public async Task<ActionResult> EditDoctor()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            Doctor doctor = await _doctorService.GetByIDAsync(did);
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            return View(doctor);
        }
        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> EditDoctor(Doctor doctor)
        {
            try
            {
                if (doctor.photo_img != null)
                {
                    // gets the file from post request
                    var theFile = HttpContext.Request.Form.Files.GetFile("photo_img");
                    // server location to store the files
                    string folder = "Doctor/photo/";
                    // will add some garbage data in file name
                    folder += Guid.NewGuid().ToString().Substring(0, 8) + "_" + theFile.FileName;
                    // buiding the server path to the uploads directory
                    string serverFolder = Path.Combine(_WebHostEnvironment.WebRootPath, folder);
                    // saving the file
                    await theFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                    // storing file path string
                    doctor.photo = folder;
                }
                int did = (int)HttpContext.Session.GetInt32("docID");
                doctor.docID = did;
                //ViewBag.data = await _dashBoardController.getProfileAsync(did);
                // checks whether all the built-in and custom validation is applied on all the data or not
                if (ModelState.IsValid)
                {
                    // updating data in database
                    await _doctorService.UpdateDoctorAsync(doctor);
                    // redirecting page to login page
                    return RedirectToAction(nameof(ViewProfile));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Update data. " +
                                             "Try Again, and if the problem persists" +
                                             "See you system administrator.");
            }
            return View(doctor);
        }
        [Authorize]
        public async Task<ActionResult> ViewProfile()
        {
            try
            {
                int did = (int)HttpContext.Session.GetInt32("docID");
                Doctor doctor = await _doctorService.GetByIDAsync(did);
                List<Clinic> clinics = await _clinicService.GetByDoctorIDAsync(did);
                //Tuple<Doctor, List<Clinic>> tuple = new Tuple<Doctor, List<Clinic>>(doctor, clinics);
                string[] name = new string[clinics.Count];
                for (int i = 0; i < clinics.Count; i++)
                {
                    name[i] = clinics[i].name;
                }
                int[] id = new int[clinics.Count];
                for (int i = 0; i < clinics.Count; i++)
                {
                    id[i] = clinics[i].clinicID;
                }
                ViewBag.cID = id;
                ViewBag.cName = name;
                ViewBag.data = await _dashBoardController.getProfileAsync(did);
                ViewBag.Doctor = doctor;
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to show Profile. " +
                                             "Try Again, and if the problem persists" +
                                             "See your system administrator.");
            }
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login l)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    l.password = CustomSecurity.ConvertToEncrypt(l.password);
                    int did = await _loginService.LoginAsync(l);
                    ViewBag.did = did;
                    if (did > 0)
                    {
                        HttpContext.Session.SetInt32("docID", did);
                        //FormsAuthentication.SetAuthCookie(did, l.email);
                        //Craeting the Security Context
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, did.ToString()),
                            new Claim(ClaimTypes.Email, l.email),
                        };
                        var identity = new ClaimsIdentity(claims, "MyAuthenticatedCookie");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = l.rememberMe
                        };

                        var options = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(30)
                        };
                        if (l.rememberMe)
                        {
                            Response.Cookies.Append("uID", did.ToString(), options);
                        }
                        else
                        {
                            HttpContext.Response.Cookies.Append("uID", "0", options);
                        }

                        await HttpContext.SignInAsync("MyAuthenticatedCookie", claimsPrincipal, authProperties);
                        return RedirectToAction("DashBoard", "DashBoard");
                    }
                    else
                    {
                        return View();
                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to Login. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyAuthenticatedCookie");
            HttpContext.Session.Remove("docID");
            Response.Cookies.Delete("uID");
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Changepassword()
        {
            int did = (int)HttpContext.Session.GetInt32("docID");
            ViewBag.data = await _dashBoardController.getProfileAsync(did);
            return View();
        }



        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> Changepassword(OTPModel f)
        {
            try
            {
                int did = (int)HttpContext.Session.GetInt32("docID");
                ViewBag.data = await _dashBoardController.getProfileAsync(did);
                if (f.oldpassword != null)
                {
                    if (f.password == f.confirmpassword)
                    {
                        Doctor doctor = await _doctorService.GetByIDAsync(did);
                        f.oldpassword = CustomSecurity.ConvertToEncrypt(f.oldpassword);
                        if (f.oldpassword == doctor.password)
                        {
                            f.password = CustomSecurity.ConvertToEncrypt(f.password);
                            await _doctorService.changepassword(did, f.password);
                            ViewBag.Status = true;
                        }
                        else
                        {
                            ViewBag.Status = false;
                            ViewBag.error = "OLD PASSWORD DOES NOT MATCH";
                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.Status = false;
                        ViewBag.error = "NEW PASSWORD AND CONFIRM PASSWORD DOES NOT MATCH";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Status = false;
                    ViewBag.error = "ENTER OLD PASSWORD";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to change password. " +
                "Try Again, and if the problem persists" +
                "See your system administrator.");
                ViewBag.Status = false;
                ViewBag.error = "SOMETHING WENT WRONG. RETRY AGAIN";
                return View();
            }
        }

        public IActionResult sendOTP()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> sendOTP(OTPModel f)
        {
            if (ModelState.IsValid)
            {
                bool status, check;
                check = await _loginService.checkemail(f.email);
                HttpContext.Session.SetString("emailtoresetpassword", f.email);
                if (check)
                {
                    String to, subject, msgbody;
                    Random random = new Random();
                    String randomCode = (random.Next(100000, 999999).ToString());
                    HttpContext.Session.SetString("OTP", randomCode); //ViewBag.Message = HttpContext.Session.GetString("OTP");
                    to = f.email;
                    subject = "Reset Password";
                    msgbody = "Your OTP to reset password is " + randomCode;
                    try
                    {
                        status = sendmail.sendingEmail(to, subject, msgbody);
                    }
                    catch (Exception ex)
                    {
                        status = false;
                    }
                    if (status)
                    {
                        ViewBag.Status = true;
                        return View("checkOTP");
                    }
                    else
                    {
                        ViewBag.Status = false;
                        return View("sendOTP");
                    }
                }
                else
                {
                    ViewBag.msg = false;
                    return View();
                }
            }
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult checkOTP(OTPModel f)
        {
            if (f.OTP != null && f.OTP.Length == 6)
            {
                string mainotp = HttpContext.Session.GetString("OTP");
                if (mainotp == f.OTP.ToString())
                {
                    return View("resetpassword");
                }
                else
                {
                    ViewBag.msgforincorrectOTP = false;
                    //Response.WriteAsync(@"<script>alert('OTP DOESN'T MATCH');</script>");
                    return View("sendOTP");
                }
            }
            ViewBag.msgofsomethingwentwrong = false;
            return View("sendOTP");
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> resetpassword(OTPModel f)
        {
            if (f.password == f.confirmpassword && f.password != null && f.confirmpassword != null)
            {
                f.email = HttpContext.Session.GetString("emailtoresetpassword");
                f.password = CustomSecurity.ConvertToEncrypt(f.password);
                bool s = await _loginService.resetpassword(f);
                if (s)
                {
                    ViewBag.Status = true;
                }
                else { ViewBag.Status = false; }
            }
            else { ViewBag.Status = false; }
            return View();
        }
    }
}