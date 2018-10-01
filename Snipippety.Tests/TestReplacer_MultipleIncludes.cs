using System;
using System.IO;
using NUnit.Framework;

namespace Snipippety.Tests
{
    [TestFixture]
    public class TestReplacer_MultipleIncludes
    {
        Replacer _replacer;

        public TestReplacer_MultipleIncludes()
        {
            _replacer = new Replacer();
        }

        [Test]
        public void CanDoIt()
        {
            const string input = @"//// 01-introduction.md

//// 02-configuration.md

//// 03-scenarios.md

//// 04-unit-testing.md



# Advanced topics

## Custom routing

Implement routing of transport messages.
";

            var output = _replacer.Replace(input, new ReplacerContext(Path.Combine(AppContext.BaseDirectory, "Testdata", "Markdowns")));

            Console.WriteLine($@"{input}

=>

{output}");

            Assert.That(output, Is.EqualTo(@"This is 01-introduction.md

This is 02-configuration.md

This is 03-scenarios.md

This is 04-unit-testing.md



# Advanced topics

## Custom routing

Implement routing of transport messages.
"));
        }
    }
}