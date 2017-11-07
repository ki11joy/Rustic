using Caliburn.Micro;
using Ninject;
using Rustic.DataService;
using Rustic.Models;
using Rustic.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Windows;

namespace Rustic
{
    public class AppBootstrapper : BootstrapperBase
    {
        private IKernel kernel;

        public AppBootstrapper()
        {
            Initialize();

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
            if (!InDesignMode())
                Directory.CreateDirectory(path);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            Database.SetInitializer(new StorageInitializer());
        }

        protected override void Configure()
        {
            base.Configure();

            kernel = new StandardKernel();

            kernel.Bind<IWindowManager>()
                .To<WindowManager>()
                .InSingletonScope();

            kernel.Bind<DbContext>().To<TaskContext>();

            kernel.Bind<IRusticClient>()
                .To<RusticClient>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            kernel.Dispose();

            base.OnExit(sender, e);
        }

        protected override object GetInstance(Type service, string key)
        {
            return kernel.Get(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            kernel.Inject(instance);
        }

        private bool InDesignMode()
        {
            return !(Application.Current is App);
        }
    }
}
