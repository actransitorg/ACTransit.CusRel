using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using ACTransit.CusRel.ServiceHost.Common;
using log4net.Config;

namespace ACTransit.CusRel.ServiceHost
{
    public static class Program
    {
        public static bool IsRunning { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Interactive Mode.");
                if (args != null)
                {
                    if (args.Any(a => a == "--start"))
                    {
                        var service = new CusRelService();
                        service.Start();
                        IsRunning = true;
                        if (args.Any(a => a == "--wait"))
                            while (true)
                                System.Threading.Thread.Sleep(60000);
                    }
                    else if (args.Any(a => a == "--install"))
                        ProjectInstaller.Install();
                    else if (args.Any(a => a == "--uninstall"))
                        ProjectInstaller.Uninstall();
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine("usage: ACTransit.CusRel.ServiceHost.exe [--install|--uninstall|--start] [--wait]");
                        Console.WriteLine("");
                    }
                }
            }            
            else
            {
                var servicesToRun = new ServiceBase[]
                {
                    new CusRelService()
                };
                ServiceBase.Run(servicesToRun);
                IsRunning = true;
            }
        }
    }
}
