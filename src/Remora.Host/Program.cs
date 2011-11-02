using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;

namespace Remora.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                //x.Service<TownCrier>(s => 
                //{
                //    s.SetServiceName("tc");
                //    s.ConstructUsing(name => new TownCrier());
                //    s.WhenStarted(tc => tc.Start());
                //    s.WhenStopped(tc => tc.Stop());
                //});
                //x.RunAsLocalSystem();

                //x.SetDescription("Sample Topshelf Host");
                //x.SetDisplayName("Stuff");
                //x.SetServiceName("stuff");
            });
        }
    }
}
