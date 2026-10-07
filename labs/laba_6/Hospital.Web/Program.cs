var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<Hospital.Data.IHospitalRepository>(_ =>
    new Hospital.Data.HospitalRepository(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "hospital.json")));
builder.Services.AddScoped<Hospital.Services.HospitalService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
