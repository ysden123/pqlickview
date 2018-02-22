using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StulSoft.PQlickView.EventLogConnectorElaborate
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        [STAThread]
        static void Main(string[] args)
        {
            if (args != null && args.Length >= 2)
            {
                log.Info("Running non standalone server...");
            }
            else
            {
                log.Info("Running standalone server...");
            }
        }
    }
}
