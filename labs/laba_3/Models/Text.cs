using System.Xml.Serialization;

namespace laba_3.Models;

[XmlRoot("text")]
public sealed class Text
{
    private const string Vowels = "aeiouаеёиоуыэюя";

    [XmlElement("sentence")]
    public List<Sentence> Sentences { get; set; } = [];

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
}
