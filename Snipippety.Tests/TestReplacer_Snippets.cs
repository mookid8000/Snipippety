using System;
using System.IO;
using NUnit.Framework;

namespace Snipippety.Tests
{
    [TestFixture]
    public class TestReplacer_Snippets 
    {
        Replacer _replacer;

        public TestReplacer_Snippets()
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

//// snippet1.cs : bl1

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

        [Test]
        public void CanPerformMultipleReplacements_SameKey()
        {
            const string input = @"Her er første linje.

//// snippet1.cs : bl1

Her er tekst.

//// snippet1.cs : bl1

Her er mere tekst.
";

            var context = new ReplacerContext(Path.Combine(AppContext.BaseDirectory, "Testdata", "Simple"));
            
            var output = _replacer.Replace(input, context);

            Console.WriteLine($@"{input}

=>

{output}");

            Assert.That(output, Is.EqualTo(@"Her er første linje.

```csharp
int a = 2;

    var b = 3;

Console.WriteLine(""Hej med dig min ven"");
```

Her er tekst.

```csharp
int a = 2;

    var b = 3;

Console.WriteLine(""Hej med dig min ven"");
```

Her er mere tekst.
"));
        }

        [Test]
        public void CanPerformMultipleReplacements_DifferentKeys()
        {
            const string input = @"Her er første linje.

//// snippet2.cs : bl1

Her er tekst.

//// snippet2.cs : bl2

Her er mere tekst.
";

            var context = new ReplacerContext(Path.Combine(AppContext.BaseDirectory, "Testdata", "Simple"));
            
            var output = _replacer.Replace(input, context);

            Console.WriteLine($@"{input}

=>

{output}");

            Assert.That(output, Is.EqualTo(@"Her er første linje.

```csharp
Console.WriteLine(""hej1"");
```

Her er tekst.

```csharp
Console.WriteLine(""hej2"");
```

Her er mere tekst.
"));
        }
    }
}
