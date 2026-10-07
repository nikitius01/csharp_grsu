using System.ComponentModel.DataAnnotations;
using Hospital.Business;
using Hospital.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Web.Controllers;

public sealed class HomeController(HospitalService hospital) : Controller
{
    public IActionResult Index()
    {
        var patients = hospital.GetPatients();
        return View(new PatientListViewModel
        {
            Patients = patients,
            ActiveCount = patients.Count(p => !p.IsDischarged),
            DischargedCount = patients.Count(p => p.IsDischarged)
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View("Error", new Hospital.Web.Models.ErrorViewModel { RequestId = HttpContext.TraceIdentifier });

    [HttpGet]
    public IActionResult Admit() => View(new AdmitPatientViewModel { Departments = hospital.Departments, Doctors = hospital.Doctors });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Admit(AdmitPatientViewModel model)
    {
        if (!ModelState.IsValid) return View(WithCatalogs(model));
        try
        {
            var patient = new Patient
            {
                FullName = model.FullName.Trim(), BirthDate = model.BirthDate, Phone = model.Phone?.Trim() ?? "",
                AdmissionDiagnosis = model.AdmissionDiagnosis.Trim(), Department = model.Department,
                AttendingDoctor = model.AttendingDoctor
            };
            hospital.Admit(patient);
            TempData["Notice"] = "Пациент госпитализирован, электронная медицинская карта создана.";
            return RedirectToAction(nameof(Details), new { id = patient.Id });
        }
        catch (Hospital.Business.ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(WithCatalogs(model));
        }
    }

    public IActionResult Details(Guid id)
    {
        var patient = hospital.GetPatient(id);
        if (patient is null) return NotFound();
        return View(new PatientDetailsViewModel { Patient = patient, Doctors = hospital.Doctors, Nurses = hospital.Nurses });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddOrder(Guid id, OrderEditViewModel model)
    {
        try
        {
            hospital.AddOrder(id, new MedicalOrder
            {
                Type = model.Type, Name = model.Name?.Trim() ?? "", MedicationForm = model.MedicationForm,
                Quantity = model.Quantity, Dosage = model.Dosage?.Trim(), Days = model.Days,
                ResponsibleStaff = model.ResponsibleStaff ?? ""
            });
            TempData["Notice"] = "Назначение добавлено в электронную медицинскую карту.";
        }
        catch (Hospital.Business.ValidationException exception) { TempData["Error"] = exception.Message; }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CompleteOrder(Guid id, Guid orderId)
    {
        try
        {
            hospital.CompleteOrder(id, orderId);
            TempData["Notice"] = "Назначение отмечено выполненным и записано в ЭМК.";
        }
        catch (Hospital.Business.ValidationException exception) { TempData["Error"] = exception.Message; }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Discharge(Guid id)
    {
        try
        {
            hospital.Discharge(id);
            TempData["Notice"] = "Пациент выписан. Выписной эпикриз сформирован.";
        }
        catch (Hospital.Business.ValidationException exception) { TempData["Error"] = exception.Message; }
        return RedirectToAction(nameof(Details), new { id });
    }

    private AdmitPatientViewModel WithCatalogs(AdmitPatientViewModel model) => model with
    {
        Departments = hospital.Departments, Doctors = hospital.Doctors
    };
}

public sealed record PatientListViewModel
{
    public IReadOnlyList<Patient> Patients { get; init; } = [];
    public int ActiveCount { get; init; }
    public int DischargedCount { get; init; }
}

public sealed record AdmitPatientViewModel
{
    [Required(ErrorMessage = "Укажите ФИО пациента.")]
    public string FullName { get; init; } = "";
    [Required(ErrorMessage = "Укажите дату рождения.")]
    public DateOnly BirthDate { get; init; }
    public string? Phone { get; init; }
    [Required(ErrorMessage = "Укажите диагноз при поступлении.")]
    public string AdmissionDiagnosis { get; init; } = "";
    [Required(ErrorMessage = "Выберите отделение.")]
    public string Department { get; init; } = "";
    [Required(ErrorMessage = "Выберите лечащего врача.")]
    public string AttendingDoctor { get; init; } = "";
    public IReadOnlyList<string> Departments { get; init; } = [];
    public IReadOnlyList<string> Doctors { get; init; } = [];
}

public sealed record PatientDetailsViewModel
{
    public Patient Patient { get; init; } = new();
    public IReadOnlyList<string> Doctors { get; init; } = [];
    public IReadOnlyList<string> Nurses { get; init; } = [];
}

public sealed record OrderEditViewModel
{
    public OrderType Type { get; init; }
    public string? Name { get; init; }
    public MedicationForm? MedicationForm { get; init; }
    public int? Quantity { get; init; }
    public string? Dosage { get; init; }
    public int? Days { get; init; }
    public string? ResponsibleStaff { get; init; }
}
