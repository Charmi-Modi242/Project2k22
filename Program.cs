using physioCard.Controllers;
using physioCard.Repositories;
using physioCard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
builder.Services.AddScoped<IClinicService, ClinicService>();
builder.Services.AddScoped<IDoctorClinicRepository, DoctorClinicRepository>();
builder.Services.AddScoped<IDoctorClinicService, DoctorClinicService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<DashBoardController, DashBoardController>();

builder.Services.AddAuthentication("MyAuthenticatedCookie").AddCookie("MyAuthenticatedCookie", options =>
{
    options.Cookie.Name = "MyAuthenticatedCookie";
    options.LoginPath = "/Doctor/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

/*for session*/
builder.Services.AddSession(options => { 
    options.IdleTimeout = TimeSpan.FromDays(30);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession(); /*for session*/

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DashBoard}/{action=DashBoard}/{id?}");

app.Run();
