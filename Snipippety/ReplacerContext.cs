using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Snipippety
{
    public class ReplacerContext
    {
        readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string[]>> _snippetCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, string[]>>();
        readonly string _directory;

        public ReplacerContext(string directory = null)
        {
            _directory = directory ?? AppContext.BaseDirectory;
        }

        public IEnumerable<string> GetSnippet(string fileName, string snippetName) => _snippetCache.GetOrAdd(fileName, ReadSnippets)
            .TryGetValue(snippetName, out var lines)
            ? lines
            : throw new ArgumentException($"Could not find snippet named {snippetName} in {fileName}");

        ConcurrentDictionary<string, string[]> ReadSnippets(string fileName)
        {
            var filePath = Path.Combine(_directory, fileName);

            if (!File.Exists(filePath))
            {
                throw new IOException($"Could not find snippet file {fileName} in {_directory}");
            }

            var snippets = new ConcurrentDictionary<string,string[]>();
            var readingSnippet = false;
            var snippetName = "";
            var currentSnippet = new List<string>();
            var snippetBlockFoundAtIndentationLevel = 0;

            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (!readingSnippet && trimmedLine.StartsWith("////"))
                {
                    snippetName = trimmedLine.Substring(4).Trim();

                    if (string.IsNullOrWhiteSpace(snippetName))
                    {
                        throw new FormatException(@"Please format the snippet lead line like this:

    //// <snippet-name>");
                    }

                    if (snippets.TryGetValue(snippetName, out var previouslyDefinedSnippetWithThatName))
                    {
                        throw  new ArgumentException($@"The line

{line}

defines snippet with name '{snippetName}', but that snippet was already defined as

{string.Join(Environment.NewLine, previouslyDefinedSnippetWithThatName)}");
                    }

                    readingSnippet = true;
                    continue;
                }

                if (readingSnippet)
                {
                    // is the block starting?
                    if (currentSnippet.Count == 0 && trimmedLine == "{")
                    {
                        snippetBlockFoundAtIndentationLevel = line.IndexOf('{');
                        continue;
                    }

                    // is the block ending?
                    if (trimmedLine == "}" && line.IndexOf('}') == snippetBlockFoundAtIndentationLevel)
                    {
                        snippets[snippetName] = GetSnippet(currentSnippet);
                        currentSnippet.Clear();
                        readingSnippet = false;
                    }

                    currentSnippet.Add(line);
                }
            }

            return snippets;
        }

        static string[] GetSnippet(List<string> currentSnippet)
        {
            if (currentSnippet.Count == 0) return new string[0];

            int GetWhitespaceCount(string str)
            {
                if (string.IsNullOrWhiteSpace(str)) return int.MaxValue;

                for (var count = 0; count < str.Length; count++)
                {
                    if (str[count] != ' ') return count;
                }

                return int.MaxValue;
            }

            var leadingWhitespaceCount = currentSnippet.Select(GetWhitespaceCount).Min();

            return currentSnippet
                .Select(line => leadingWhitespaceCount >= line.Length ? "" : line.Substring(leadingWhitespaceCount))
                .ToArray();
        }
    }
}