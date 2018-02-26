using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StulSoft.PQlickView.ListNamedPipes
{
    class Program
    {
        private static readonly string PREFIX = @"\\.\pipe\";
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            //Load config
            log4net.Config.XmlConfigurator.Configure();
            log.Info("Started ListNamedPipes");
            System.IO.Directory.GetFiles(@"\\.\pipe\")
                            .Select(n => n.Substring(PREFIX.Length))
                            .OrderBy(n => n)
                            .ToList()
                            .ForEach(p => log.Info(p));
            log.Info("Finished ListNamedPipes");
        }
    }
}
