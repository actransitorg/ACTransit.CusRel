using System.ComponentModel;
using log4net;

namespace ACTransit.CusRel.ServiceHost
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(ProjectInstaller));
        public ProjectInstaller()
        {
            log.Info("Initializing");
            InitializeComponent();
        }

        public static void Install()
        {
            System.Configuration.Install.TransactedInstaller ti = null;
            ti = new System.Configuration.Install.TransactedInstaller();
            ti.Installers.Add(new ProjectInstaller());
            ti.Context = new System.Configuration.Install.InstallContext("", null);
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ti.Context.Parameters["assemblypath"] = path;
            ti.Install(new System.Collections.Hashtable());
        }

        public static void Uninstall()
        {
            System.Configuration.Install.TransactedInstaller ti = null;
            ti = new System.Configuration.Install.TransactedInstaller();
            ti.Installers.Add(new ProjectInstaller());
            ti.Context = new System.Configuration.Install.InstallContext("", null);
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ti.Context.Parameters["assemblypath"] = path;
            ti.Uninstall(null);
        }
    }
}
