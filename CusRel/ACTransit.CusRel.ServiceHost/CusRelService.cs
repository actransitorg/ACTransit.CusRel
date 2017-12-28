using System.ServiceProcess;
using ACTransit.CusRel.ServiceHost.Tasks;
using log4net;

namespace ACTransit.CusRel.ServiceHost
{
    public partial class CusRelService : ServiceBase
    {
        private static TaskManager taskManager;
        private static readonly ILog log = LogManager.GetLogger(nameof(CusRelService));

        public CusRelService()
        {
            log.Info("Initializing");
            InitializeComponent();
            taskManager = new TaskManager();
        }

        public void Start()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            log.Debug("Begin TaskManager.OnStart");
            taskManager.OnStart();
            log.Debug("End TaskManager.OnStart");
        }

        public void Stop()
        {
            OnStop();
        }

        protected override void OnStop()
        {
            log.Debug("Begin TaskManager.OnStop");
            taskManager.OnStop();
            log.Debug("End TaskManager.OnStop");
        }
    }
}