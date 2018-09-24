using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snipippety
{
    public class Replacer
    {
        public string Replace(string input, ReplacerContext context)
        {
            var lineSeparator = Environment.NewLine;

            var lines = input.Split(new[] {lineSeparator}, StringSplitOptions.None);

            return string.Join(lineSeparator, lines.SelectMany(line => Process(line, context)));
        }

        static IEnumerable<string> Process(string line, ReplacerContext context)
        {
            const string snippetIntro = "////";

            if (!line.StartsWith(snippetIntro))
            {
                yield return line;
                yield break;
            }

            yield return "```csharp";

            var snippetLines = ReadSnippetLines(line.Substring(snippetIntro.Length), context);

            foreach (var snippetLine in snippetLines)
            {
                yield return snippetLine;
            }

            yield return "```";
        }

        static IEnumerable<string> ReadSnippetLines(string line, ReplacerContext context)
        {
            var tokens = line.Split('/').Select(token => token.Trim()).ToArray();

            if (tokens.Length != 2)
            {
                throw new FormatException($"Could not extract <filename>/<snippet-name> pair from '{line}'");
            }

            return context.GetSnippet(tokens[0], tokens[1]);
        }
    }
}