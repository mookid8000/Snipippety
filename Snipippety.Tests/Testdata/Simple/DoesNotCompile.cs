
//// basic outline
{
    Configure.With(...)
        .Transport(t => t.Use(...))
        .Start();
}

//// simplest possible configuration
{
    using(var activator = new BuiltinHandlerActivator())
    {
        Configure.With(activator)
            .Transport(t => t.UseMsmq("your-queue"))
            .Start();

        Console.WriteLine("Press ENTER to quit");
        Console.ReadLine();
    }
}

//// relistic examples
{
    // somewhere at the entry point of the app:
    static readonly IWindsorContainer = new WindsorContainer()
        .Install(...) // your other Windsor installers
        .Install(new RebusInstaller());

    // and then you have this Windsor installer somewhere:
    public class RebusInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var rabbitConn = ConfigurationManager.ConnectionStrings["rabbit"];
            var sqlConn = ConfigurationManager.ConnectionStrings["rebus"];

            Configure.With(new CastleWindsorContainerAdapter(container))
                .Logging(l => l.Serilog(Log.Logger))
                .Transport(t => t.UseRabbitMq(rabbitConn, "your-queue"))
                .Subscriptions(s => s.StoreInSqlServer(sqlConn, "Subscriptions", isCentralized: true))
                .Sagas(s => s.StoreInSqlServer(sqlConn, "Sagas", "SagaIndex"))
                .Timeouts(t => t.StoreInSqlServer(sqlConn, "Timeouts"))
                .Options(o => {
                    o.EnableCompression(bodySizeThresholdBytes: 32768);
                    o.SetNumberOfWorkers(numberOfWorkers: 4);
                    o.SetMaxParallelism(maxParallelism: 25);	
                })
                .Start();
        }
    }	
}