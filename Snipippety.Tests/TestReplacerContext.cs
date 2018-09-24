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
        public void CanParseFile()
        {
            var context = new ReplacerContext(Path.Combine(AppContext.BaseDirectory, "Testdata", "Simple"));

            var snippet = context.GetSnippet("DoesNotCompile.cs", "relistic examples").ToArray();

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