namespace Hospital.Business;

public sealed class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public string Phone { get; set; } = "";
    public DateTime AdmittedAt { get; set; } = DateTime.Now;
    public string AdmissionDiagnosis { get; set; } = "";
    public string Department { get; set; } = "";
    public string AttendingDoctor { get; set; } = "";
    public bool IsDischarged { get; set; }
    public DateTime? DischargedAt { get; set; }
    public List<MedicalOrder> Orders { get; set; } = [];
    public List<MedicalRecordEntry> Record { get; set; } = [];
    public DischargeSummary? DischargeSummary { get; set; }

    public void ValidateForAdmission()
    {
        if (string.IsNullOrWhiteSpace(FullName)) throw new ValidationException("Укажите ФИО пациента.");
        if (BirthDate == default || BirthDate > DateOnly.FromDateTime(DateTime.Today)) throw new ValidationException("Укажите корректную дату рождения.");
        if (string.IsNullOrWhiteSpace(AdmissionDiagnosis)) throw new ValidationException("Укажите диагноз при поступлении.");
        if (string.IsNullOrWhiteSpace(Department)) throw new ValidationException("Выберите отделение.");
        if (string.IsNullOrWhiteSpace(AttendingDoctor)) throw new ValidationException("Укажите лечащего врача.");
    }
}

public sealed class MedicalOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public OrderType Type { get; set; }
    public string Name { get; set; } = "";
    public MedicationForm? MedicationForm { get; set; }
    public int? Quantity { get; set; }
    public string? Dosage { get; set; }
    public int? Days { get; set; }
    public string ResponsibleStaff { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; } = OrderStatus.Planned;
    public DateTime? CompletedAt { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new ValidationException("Введите название назначения.");
        if (string.IsNullOrWhiteSpace(ResponsibleStaff)) throw new ValidationException("Укажите ответственного сотрудника.");
        if (Type == OrderType.Medication)
        {
            if (MedicationForm is null) throw new ValidationException("Выберите форму медикаментозного назначения.");
            if (MedicationForm == Hospital.Business.MedicationForm.Injection && (Quantity is null or <= 0))
                throw new ValidationException("Для инъекции укажите количество больше нуля.");
            if (MedicationForm == Hospital.Business.MedicationForm.Tablet && (string.IsNullOrWhiteSpace(Dosage) || Days is null or <= 0))
                throw new ValidationException("Для таблеток укажите дозировку и количество дней приема.");
        }
    }
}

public sealed class MedicalRecordEntry
{
    public DateTime OccurredAt { get; set; } = DateTime.Now;
    public string Text { get; set; } = "";
    public string Author { get; set; } = "";
}

public sealed class DischargeSummary
{
    public string PatientName { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public string Department { get; set; } = "";
    public DateTime AdmittedAt { get; set; }
    public DateTime DischargedAt { get; set; }
    public string AdmissionDiagnosis { get; set; } = "";
    public List<string> CompletedOrders { get; set; } = [];
}
