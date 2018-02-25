using log4net;
using QlikView.Qvx.QvxLibrary;
using System;
using System.Windows.Interop;

namespace StulSoft.PQlickView.EventLogConnectorElaborate
{
    internal class QvEventLogServer : QvxServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(QvEventLogServer));
        public override QvxConnection CreateConnection()
        {
            log.Debug("CreateConnection()");
            return new QvEventLogConnection();
        }

        public override string CreateConnectionString()
        {
            QvxLog.Log(QvxLogFacility.Application, QvxLogSeverity.Debug, "CreateConnectionString()");
            log.Debug("CreateConnectionString()");

            var login = CreateLoginWindowHelper();
            login.ShowDialog();

            string connectionString = null;
            if (login.DialogResult.Equals(true))
            {
                connectionString = String.Format("Server={0};UserId={1};Password={2}",
                    login.GetServer(), login.GetUsername(), login.GetPassword());
                log.Debug($"connectionString: {connectionString}");
            }

            return connectionString;
        }

        private Login CreateLoginWindowHelper()
        {
            log.Debug("CreateLoginWindowHelper()");
            // Since the owner of the loginWindow is a Win32 process we need to
            // use WindowInteropHelper to make it modal to its owner.
            var login = new Login();
            var wih = new WindowInteropHelper(login);
            wih.Owner = MParentWindow;

            return login;
        }
    }
}
