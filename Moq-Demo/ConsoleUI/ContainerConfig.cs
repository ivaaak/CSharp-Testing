using Autofac;
using DemoLibrary.Logic;
using DemoLibrary.Utilities;

namespace ConsoleUI
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<PersonProcessor>().As<IPersonProcessor>();
            builder.RegisterType<SqliteDataAccess>().As<ISqliteDataAccess>();

            return builder.Build();
        }
    }
}
