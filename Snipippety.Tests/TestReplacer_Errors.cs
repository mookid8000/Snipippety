using System;
using System.IO;
using NUnit.Framework;
using Testy;

namespace Snipippety.Tests
{
    [TestFixture]
    public class TestReplacer_Errors : FixtureBase
    {
        Replacer _replacer;

        public TestReplacer_Errors()
        {
            _replacer = new Replacer();
        }

        [Test]
        public void WhackException()
        {
            var input = @"Før

//// findes-ikke.md / crazy format : helt mystisk

Efter.";

            var output = _replacer.Replace(input, GetContext());

            Console.WriteLine($@"{input}

=>

{output}");


        }

        [Test]
        public void DoesNotDieWhenIncludeFileCannotBeFound()
        {
            var input = @"Før

//// findes-ikke.md

Efter.";

            var output = _replacer.Replace(input, GetContext());

            Console.WriteLine($@"{input}

=>

{output}");

            Assert.That(output, Is.EqualTo(@"Før

Could not find file C:\projects-rebusfm\Snipippety\Snipippety.Tests\bin\Debug\net6.0\findes-ikke.md

Efter."));
        }

        [Test]
        public void DoesNotDieWhenSnippetFileCannotBeFound()
        {
            var input = @"Før

//// findes-ikke.cs : whatever

Efter.";

            var output = _replacer.Replace(input, GetContext());

            Console.WriteLine($@"{input}

=>

{output}");

            Assert.That(output, Is.EqualTo(@"Før

```csharp
Could not find snippet 'whatever' in C:\projects-rebusfm\Snipippety\Snipippety.Tests\bin\Debug\net6.0\findes-ikke.cs
```

Efter."));
        }

        static ReplacerContext GetContext()
        {
            return new ReplacerContext(AppContext.BaseDirectory);
        }
    }
}