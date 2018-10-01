using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Snipippety.Tests
{
    [TestFixture]
    public class TestReplacerContext
    {
        [Test]
        public void GetsNiceErrorTextWhenSnippetCannotBeFound_FileNotFound()
        {
            var context = new ReplacerContext();
            var lines = context.GetSnippet("file-does-not.exist.cs", new []{"snippet-name"});

            var text = string.Join(Environment.NewLine, lines);

            Console.WriteLine(text);

            Assert.That(text, Is.EqualTo("Could not find snippet \'snippet-name\' in C:\\projects\\Snipippety\\Snipippety.Tests\\bin\\Debug\\netcoreapp2.1\\file-does-not.exist.cs"));
        }

        [Test]
        public void GetsNiceErrorTextWhenSnippetCannotBeFound_SnippetNameDoesNotExist()
        {
            var context = new ReplacerContext(Path.Combine(AppContext.BaseDirectory, "Testdata", "Simple"));
            var lines = context.GetSnippet(Path.Combine("Snippet1.cs"), new []{"nonexistent-snippet-name"});

            var text = string.Join(Environment.NewLine, lines);

            Console.WriteLine(text);

            Assert.That(text, Contains.Substring("Could not find snippet 'nonexistent-snippet-name' in"));
            Assert.That(text, Contains.Substring("Snippet1.cs"));
        }

        [Test]
        public void CanParseFile()
        {
            var context = new ReplacerContext(Path.Combine(AppContext.BaseDirectory, "Testdata", "Simple"));

            var snippet = context.GetSnippet("DoesNotCompile.cs", new[]{"relistic examples"}).ToArray();

            var fullText = string.Join(Environment.NewLine, snippet);
            
            Console.WriteLine();
            Console.WriteLine(fullText);
            Console.WriteLine();

            Assert.That(fullText, Is.EqualTo(
                @"// somewhere at the entry point of the app:
static readonly IWindsorContainer = new WindsorContainer()
    .Install(...) // your other Windsor installers
    .Install(new RebusInstaller());

// and then you have this Windsor installer somewhere:
public class RebusInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        var rabbitConn = ConfigurationManager.ConnectionStrings[""rabbit""];
        var sqlConn = ConfigurationManager.ConnectionStrings[""rebus""];

        Configure.With(new CastleWindsorContainerAdapter(container))
            .Logging(l => l.Serilog(Log.Logger))
            .Transport(t => t.UseRabbitMq(rabbitConn, ""your-queue""))
            .Subscriptions(s => s.StoreInSqlServer(sqlConn, ""Subscriptions"", isCentralized: true))
            .Sagas(s => s.StoreInSqlServer(sqlConn, ""Sagas"", ""SagaIndex""))
            .Timeouts(t => t.StoreInSqlServer(sqlConn, ""Timeouts""))
            .Options(o => {
                o.EnableCompression(bodySizeThresholdBytes: 32768);
                o.SetNumberOfWorkers(numberOfWorkers: 4);
                o.SetMaxParallelism(maxParallelism: 25);
            })
            .Start();
    }
}"));
        }
    }
}