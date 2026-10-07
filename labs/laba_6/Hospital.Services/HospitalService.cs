using Hospital.Business;
using Hospital.Data;

namespace Hospital.Services;

public sealed class HospitalService(IHospitalRepository repository)
{
    public IReadOnlyList<Patient> GetPatients() => repository.GetPatients();
    public Patient? GetPatient(Guid id) => repository.GetPatient(id);
    public IReadOnlyList<string> Departments => repository.Departments;
    public IReadOnlyList<string> Doctors => repository.Doctors;
    public IReadOnlyList<string> Nurses => repository.Nurses;

    public Patient Admit(Patient patient)
    {
        patient.ValidateForAdmission();
        if (!repository.Departments.Contains(patient.Department)) throw new ValidationException("Неизвестное отделение.");
        if (!repository.Doctors.Contains(patient.AttendingDoctor)) throw new ValidationException("Выбранный врач не найден.");
        patient.AdmittedAt = DateTime.Now;
        patient.Record.Add(new MedicalRecordEntry { Text = $"Госпитализация. Диагноз при поступлении: {patient.AdmissionDiagnosis}. Направлен в отделение «{patient.Department}».", Author = "Дежурный врач" });
        repository.SavePatient(patient);
        return patient;
    }

    public void AddOrder(Guid patientId, MedicalOrder order)
    {
        var patient = RequireActivePatient(patientId);
        order.Validate();
        var validStaff = order.Type == OrderType.Medication ? repository.Nurses : repository.Doctors;
        if (!validStaff.Contains(order.ResponsibleStaff))
            throw new ValidationException(order.Type == OrderType.Medication ? "Медикаментозное назначение выполняет медсестра." : "Это назначение выполняет врач.");
        patient.Orders.Add(order);
        patient.Record.Add(new MedicalRecordEntry { Text = $"Назначено: {Describe(order)}.", Author = patient.AttendingDoctor });
        repository.SavePatient(patient);
    }

    public void CompleteOrder(Guid patientId, Guid orderId)
    {
        var patient = RequireActivePatient(patientId);
        var order = patient.Orders.FirstOrDefault(o => o.Id == orderId) ?? throw new ValidationException("Назначение не найдено.");
        if (order.Status == OrderStatus.Completed) throw new ValidationException("Назначение уже выполнено.");
        order.Status = OrderStatus.Completed;
        order.CompletedAt = DateTime.Now;
        patient.Record.Add(new MedicalRecordEntry { Text = $"Выполнено назначение: {Describe(order)}.", Author = order.ResponsibleStaff });
        repository.SavePatient(patient);
    }

    public void Discharge(Guid patientId)
    {
        var patient = RequireActivePatient(patientId);
        if (patient.Orders.Any(o => o.Status != OrderStatus.Completed))
            throw new ValidationException("Нельзя выписать пациента, пока остаются невыполненные назначения.");
        var dischargedAt = DateTime.Now;
        patient.IsDischarged = true;
        patient.DischargedAt = dischargedAt;
        patient.DischargeSummary = new DischargeSummary
        {
            PatientName = patient.FullName,
            BirthDate = patient.BirthDate,
            Department = patient.Department,
            AdmittedAt = patient.AdmittedAt,
            DischargedAt = dischargedAt,
            AdmissionDiagnosis = patient.AdmissionDiagnosis,
            CompletedOrders = patient.Orders.Where(o => o.Status == OrderStatus.Completed).Select(Describe).ToList()
        };
        patient.Record.Add(new MedicalRecordEntry { Text = "Пациент выписан. Сформирован выписной эпикриз.", Author = patient.AttendingDoctor });
        repository.SavePatient(patient);
    }

    private Patient RequireActivePatient(Guid id)
    {
        var patient = repository.GetPatient(id) ?? throw new ValidationException("Пациент не найден.");
        if (patient.IsDischarged) throw new ValidationException("Пациент уже выписан.");
        return patient;
    }

    private static string Describe(MedicalOrder order)
    {
        var details = order.Type == OrderType.Medication
            ? order.MedicationForm == MedicationForm.Injection ? $"инъекция, количество: {order.Quantity}" : $"таблетки, дозировка: {order.Dosage}, дней: {order.Days}"
            : order.Type == OrderType.Diagnostic ? "диагностическое" : "профилактическое";
        return $"{order.Name} ({details})";
    }
}
