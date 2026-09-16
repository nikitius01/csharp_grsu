namespace laba_3.Services;

public static class StopWordLoader
{
    public static IReadOnlyCollection<string> Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return File.ReadLines(path)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0 && !line.StartsWith('#'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
