using System.Text.RegularExpressions;
using laba_3.Models;
using TextModel = laba_3.Models.Text;

namespace laba_3.Services;

public sealed partial class TextParser
{
    private const string SentenceEndings = ".!?";
    private const string OpeningPunctuation = "([{«";

    public TextModel ParseFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Parse(File.ReadAllText(path));
    }

    public TextModel Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var text = new TextModel();
        var sentence = new Sentence();
        var sentenceEnded = false;

        foreach (Match match in TokenPattern().Matches(source))
        {
            var value = match.Value;
            var isWord = char.IsLetterOrDigit(value[0]);

            if (sentence.Tokens.Count > 0 && sentenceEnded &&
                (isWord || IsOpeningPunctuation(value)))
            {
                text.Sentences.Add(sentence);
                sentence = new Sentence();
                sentenceEnded = false;
            }

            Token token = isWord
                ? new Word(value)
                : new Punctuation(value);

            sentence.Tokens.Add(token);

            if (token is Punctuation punctuation && IsSentenceEnding(punctuation.Value))
                sentenceEnded = true;
        }

        if (sentence.Tokens.Count > 0)
            text.Sentences.Add(sentence);

        return text;
    }

    private static bool IsSentenceEnding(string value) =>
        value.Length == 1 && SentenceEndings.Contains(value[0]);

    private static bool IsOpeningPunctuation(string value) =>
        value.Length == 1 && OpeningPunctuation.Contains(value[0]);

    [GeneratedRegex(@"[\p{L}\p{M}\p{N}]+(?:['’\-][\p{L}\p{M}\p{N}]+)*|[^\p{L}\p{M}\p{N}\s]",
        RegexOptions.CultureInvariant)]
    private static partial Regex TokenPattern();
}
