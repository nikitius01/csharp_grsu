using System.Text.Json;
using Hospital.Business;

namespace Hospital.Data;

public interface IHospitalRepository
{
    IReadOnlyList<Patient> GetPatients();
    Patient? GetPatient(Guid id);
    void SavePatient(Patient patient);
    IReadOnlyList<string> Departments { get; }
    IReadOnlyList<string> Doctors { get; }
    IReadOnlyList<string> Nurses { get; }
}

public sealed class HospitalRepository : IHospitalRepository
{
    private readonly string _filePath;
    private readonly object _sync = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private HospitalData _data;

    public HospitalRepository(string filePath)
    {
        _filePath = filePath;
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        _data = File.Exists(filePath)
            ? JsonSerializer.Deserialize<HospitalData>(File.ReadAllText(filePath), _jsonOptions) ?? HospitalData.CreateDefault()
            : HospitalData.CreateDefault();
        if (!File.Exists(filePath)) Persist();
    }

    public IReadOnlyList<string> Departments => _data.Departments;
    public IReadOnlyList<string> Doctors => _data.Doctors;
    public IReadOnlyList<string> Nurses => _data.Nurses;

    public IReadOnlyList<Patient> GetPatients()
    {
        lock (_sync) return _data.Patients.OrderByDescending(p => p.AdmittedAt).ToList();
    }

    public Patient? GetPatient(Guid id)
    {
        lock (_sync) return _data.Patients.FirstOrDefault(p => p.Id == id);
    }

    public void SavePatient(Patient patient)
    {
        lock (_sync)
        {
            var index = _data.Patients.FindIndex(p => p.Id == patient.Id);
            if (index < 0) _data.Patients.Add(patient);
            else _data.Patients[index] = patient;
            Persist();
        }
    }

    private void Persist()
    {
        var temporaryPath = _filePath + ".tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(_data, _jsonOptions));
        File.Move(temporaryPath, _filePath, overwrite: true);
    }

    private sealed class HospitalData
    {
        public List<Patient> Patients { get; set; } = [];
        public List<string> Departments { get; set; } = [];
        public List<string> Doctors { get; set; } = [];
        public List<string> Nurses { get; set; } = [];

        public static HospitalData CreateDefault() => new()
        {
            Departments = ["Кардиологическое", "Хирургическое", "Неврологическое", "Психоневрологический диспансер"],
            Doctors = ["Анна Смирнова", "Иван Петров", "Мария Козлова", "Федор Боярин"],
            Nurses = ["Елена Волкова", "Ольга Соколова", "Наталья Морозова", "Николай Курьян"]
        };
    }
}
