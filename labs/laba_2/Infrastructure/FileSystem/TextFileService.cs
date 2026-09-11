using System.Text;

namespace laba_2.Infrastructure.FileSystem;

public sealed class TextFileService
{
    public string[] ReadAllLines(string path) => File.ReadAllLines(path);

    public void WriteAllText(string path, string content) =>
        File.WriteAllText(path, content, new UTF8Encoding(false));
}
