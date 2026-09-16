using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace laba_3.Models;

[XmlRoot("text")]
public sealed partial class Text
{
    private const string Vowels = "aeiouаеёиоуыэюя";
    private const string SentenceEndings = ".!?";
    private const string OpeningPunctuation = "([{«";

    [XmlElement("sentence")]
    public List<Sentence> Sentences { get; set; } = [];

    public static Text ParseFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Parse(File.ReadAllText(path));
    }

    public static Text Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var text = new Text();
        var sentence = new Sentence();
        var sentenceEnded = false;
        var straightQuoteIsOpen = false;

        foreach (Match match in TokenPattern().Matches(source))
        {
            var value = match.Value;
            var isWord = char.IsLetterOrDigit(value[0]);
            var startsQuotedFragment = value == "\"" && !straightQuoteIsOpen;

            if (sentence.Tokens.Count > 0 && sentenceEnded &&
                (isWord || IsOpeningPunctuation(value) || startsQuotedFragment))
            {
                text.Sentences.Add(sentence);
                sentence = new Sentence();
                sentenceEnded = false;
                straightQuoteIsOpen = false;
            }

            object token = isWord
                ? new Word(value)
                : new Punctuation(value);

            sentence.Tokens.Add(token);

            if (value == "\"")
                straightQuoteIsOpen = !straightQuoteIsOpen;

            if (token is Punctuation punctuation && IsSentenceEnding(punctuation.Value))
                sentenceEnded = true;
        }

        if (sentence.Tokens.Count > 0)
            text.Sentences.Add(sentence);

        return text;
    }

    public IEnumerable<Sentence> OrderByWordCount() =>
        Sentences.OrderBy(sentence => sentence.WordCount);

    public IEnumerable<Sentence> OrderByLength() =>
        Sentences.OrderBy(sentence => sentence.Length);

    public IReadOnlyList<Word> FindWordsInQuestions(int length)
    {
        EnsurePositiveLength(length);

        return Sentences
            .Where(sentence => sentence.IsQuestion)
            .SelectMany(sentence => sentence.Words)
            .Where(word => word.Value.Length == length)
            .DistinctBy(word => word.Value, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public int RemoveWordsStartingWithConsonant(int length)
    {
        EnsurePositiveLength(length);

        var removedCount = 0;
        foreach (var sentence in Sentences)
        {
            removedCount += sentence.Tokens.RemoveAll(token =>
                token is Word word &&
                word.Value.Length == length &&
                StartsWithConsonant(word.Value));
        }

        return removedCount;
    }

    public int ReplaceWordsInSentence(int sentenceNumber, int length, string replacement)
    {
        EnsurePositiveLength(length);
        ArgumentNullException.ThrowIfNull(replacement);

        if (sentenceNumber < 1 || sentenceNumber > Sentences.Count)
            throw new ArgumentOutOfRangeException(nameof(sentenceNumber),
                $"Номер предложения должен быть от 1 до {Sentences.Count}.");

        var replacementCount = 0;
        foreach (var word in Sentences[sentenceNumber - 1].Words.Where(word => word.Value.Length == length))
        {
            word.Value = replacement;
            replacementCount++;
        }

        return replacementCount;
    }

    public int RemoveStopWords(IEnumerable<string> stopWords)
    {
        ArgumentNullException.ThrowIfNull(stopWords);

        var stopWordSet = stopWords
            .Select(word => word.Trim())
            .Where(word => word.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var removedCount = 0;
        foreach (var sentence in Sentences)
        {
            removedCount += sentence.Tokens.RemoveAll(token =>
                token is Word word && stopWordSet.Contains(word.Value));
        }

        return removedCount;
    }

    public void ExportToXml(string path)
    {
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
        new XmlSerializer(typeof(Text)).Serialize(writer, this, namespaces);
    }

    public override string ToString() => string.Join(' ', Sentences);

    private static bool StartsWithConsonant(string value)
    {
        if (value.Length == 0 || !char.IsLetter(value[0]))
            return false;

        return !Vowels.Contains(char.ToLowerInvariant(value[0]));
    }

    private static void EnsurePositiveLength(int length)
    {
        if (length <= 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Длина слова должна быть положительной.");
    }

    private static bool IsSentenceEnding(string value) =>
        value.Length == 1 && SentenceEndings.Contains(value[0]);

    private static bool IsOpeningPunctuation(string value) =>
        value.Length == 1 && OpeningPunctuation.Contains(value[0]);

    [GeneratedRegex(@"[\p{L}\p{M}\p{N}]+(?:['’\-][\p{L}\p{M}\p{N}]+)*|[^\p{L}\p{M}\p{N}\s]",
        RegexOptions.CultureInvariant)]
    private static partial Regex TokenPattern();
}
