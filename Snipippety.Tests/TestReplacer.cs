using System;
using System.IO;
using NUnit.Framework;

namespace Snipippety.Tests
{
    [TestFixture]
    public class TestReplacer 
    {
        Replacer _replacer;

        public TestReplacer()
        {
            _replacer = new Replacer();
        }

        [Test]
        public void CanProcessEmptyString()
        {
            var output = _replacer.Replace("", new ReplacerContext());

            Assert.That(output, Is.EqualTo(""));
        }

        [Test]
        public void CanPerformSimpleReplacement()
        {
            const string input = @"Her er noget tekst

/// snippet1.cs / bl1

Her er noget mere tekst";
            var context = new ReplacerContext(Path.Combine(AppContext.BaseDirectory, "Testdata", "Simple"));
            
            var output = _replacer.Replace(input, context);

            Console.WriteLine($@"{input}

=>

{output}");

            Assert.That(output, Is.EqualTo(@"Her er noget tekst

```csharp
int a = 2;

    var b = 3;

Console.WriteLine(""Hej med dig min ven"");
```

Her er noget mere tekst"));
        }
    }
}
