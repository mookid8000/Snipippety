using System.Collections.Generic;
using System.Linq;

namespace Snipippety
{
    static class StringExtensions
    {
        public static IEnumerable<string> TrimEmptyStartLines(this IEnumerable<string> lines) => lines.SkipWhile(string.IsNullOrWhiteSpace);
    }
}