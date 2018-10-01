using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snipippety
{
    public class Replacer
    {
        static readonly string[] CodeFileExtensions = { ".cs" };
        static readonly string[] LiteralIncludeFileExtension = {".md"};
        static readonly string LineSeparator = Environment.NewLine;

        public string Replace(string input, ReplacerContext context)
        {
            var lines = input.Split(new[] { LineSeparator }, StringSplitOptions.None);

            return string.Join(LineSeparator, ProcessLines(context, lines));
        }

        static IEnumerable<string> ProcessLines(ReplacerContext context, string[] lines)
        {
            return lines.SelectMany(line => Process(line, context));
        }

        static IEnumerable<string> Process(string line, ReplacerContext context)
        {
            const string snippetIntro = "////";

            if (!line.StartsWith(snippetIntro))
            {
                yield return line;
                yield break;
            }

            var snippetLines = ReadSnippetLines(line.Substring(snippetIntro.Length), context);

            foreach (var snippetLine in snippetLines)
            {
                yield return snippetLine;
            }
        }

        static IEnumerable<string> ReadSnippetLines(string line, ReplacerContext context)
        {
            var tokens = line.Split('/').Select(token => token.Trim()).ToArray();
            var fileName = tokens.First();
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (LiteralIncludeFileExtension.Contains(extension))
            {
                var expandedSnippetLines = context.ReadLines(fileName).ToArray();

                foreach (var expandedSnippetLine in ProcessLines(context, expandedSnippetLines))
                {
                    yield return expandedSnippetLine;
                }

                yield break;
            }

            if (CodeFileExtensions.Contains(extension)) yield return "```csharp";

            var snippetLines = context.GetSnippet(fileName, tokens.Skip(1));

            foreach (var snippetLine in snippetLines)
            {
                yield return snippetLine;
            }

            if (CodeFileExtensions.Contains(extension)) yield return "```";
        }
    }
}