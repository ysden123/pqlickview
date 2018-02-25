using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StulSoft.PQlickView.EventLogConnectorElaborate
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        [STAThread]
        static void Main(string[] args)
        {
            //Load config
            log4net.Config.XmlConfigurator.Configure();
            if (args != null && args.Length >= 2)
            {
                log.Info("Running non standalone server...");
                new QvEventLogServer().Run(args[0], args[1]);
            }
            else
            {
                log.Info("Running standalone server...");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Standalone());
            }
        }
    }
}
