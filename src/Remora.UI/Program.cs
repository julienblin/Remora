using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Remora.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var container = new WindsorContainer();
            container.Register(
                Component.For<MainWindow>(),
                AllTypes
                    .FromThisAssembly()
                    .BasedOn<ICommand>()
                    .Configure(c => c.LifeStyle.Transient)
            );

            Application.Run(container.Resolve<MainWindow>());
        }
    }
}
