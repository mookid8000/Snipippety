using System;
using System.IO;
using NUnit.Framework;
using Testy;

namespace Snipippety.Tests
{
    [TestFixture]
    public class TestReplacer_Includes : FixtureBase
    {
        Replacer _replacer;

        public TestReplacer_Includes()
        {
            _replacer = new Replacer();
        }

        [Test]
        public void CanIncludeTextFromFile()
        {
            var input = @"Her er noget tekst.

//// include1.md

Her er noget mere tekst.";
            var context = new ReplacerContext(Path.Combine(AppContext.BaseDirectory, "Testdata", "Includes"));

            var output = _replacer.Replace(input, context);

            Assert.That(output, Is.EqualTo(@"Her er noget tekst.

Her er noget inkluderet tekst.

Her er noget mere tekst."));
        }

        [Test]
        public void CanIncludeTextFromFile_Recursive()
        {
            var input = @"Her er noget tekst.

//// include3.md

Her er noget mere tekst.";
            var context = new ReplacerContext(Path.Combine(AppContext.BaseDirectory, "Testdata", "Includes"));

            var output = _replacer.Replace(input, context);

            Assert.That(output, Is.EqualTo(@"Her er noget tekst.

Fil3 start

Fil2 start

Her er noget inkluderet tekst.

Fil2 slut

Fil3 slut

Her er noget mere tekst."));
        }
    }
}