using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TextModel = laba_3.Models.Text;

namespace laba_3.Services;

public static class TextXmlExporter
{
    private static readonly XmlSerializer Serializer = new(typeof(TextModel));

    public static void Export(TextModel text, string path)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var fullPath = Path.GetFullPath(path);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            Indent = true
        };

        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);

        using var writer = XmlWriter.Create(fullPath, settings);
        Serializer.Serialize(writer, text, namespaces);
    }
}
