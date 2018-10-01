using System;
using System.IO;
using NUnit.Framework;

namespace Snipippety.Tests
{
    [TestFixture]
    public class TestReplacer_Errors
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

Could not find file C:\projects\Snipippety\Snipippety.Tests\bin\Debug\netcoreapp2.1\findes-ikke.md

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
Could not find snippet 'whatever' in C:\projects\Snipippety\Snipippety.Tests\bin\Debug\netcoreapp2.1\findes-ikke.cs
```

Efter."));
        }

        static ReplacerContext GetContext()
        {
            return new ReplacerContext(AppContext.BaseDirectory);
        }
    }
}