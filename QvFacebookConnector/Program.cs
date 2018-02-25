using log4net;
using System;

namespace QvFacebookConnector
{
    internal static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            //Load config
            log4net.Config.XmlConfigurator.Configure();

            if (args != null && args.Length >= 2)
            {
                log.Info("Running non standalone server...");
                log.Debug($"parent name: {args[0]}, pipeLine: {args[1]}");
                new QvFacebookServer().Run(args[0], args[1]);
            }
        }
    }
}
