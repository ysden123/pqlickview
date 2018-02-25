using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using log4net;
using QlikView.Qvx.QvxLibrary;

namespace StulSoft.PQlickView.EventLogConnectorElaborate
{
    class QvEventLogConnection : QvxConnection
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(QvEventLogConnection));
        // Has been hardcoded, should preferably be done programatically.
        public override void Init()
        {
            QvxLog.SetLogLevels(false, false);
            QvxLog.Log(QvxLogFacility.Application, QvxLogSeverity.Notice, "Init()");
            log.Debug("Init()");

            VerifyCredentials();

            var applicationsEventLogFields = new QvxField[]
                {
                    new QvxField("Category", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("EntryType", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("Message", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("CategoryNumber", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("Index", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("MachineName", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("Source", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("TimeGenerated", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII)
                };

            var systemEventLogFields = new QvxField[]
                {
                    new QvxField("Category", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("EntryType", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("Message", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("CategoryNumber", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("Index", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("MachineName", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("Source", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII),
                    new QvxField("TimeGenerated", QvxFieldType.QVX_TEXT, QvxNullRepresentation.QVX_NULL_FLAG_SUPPRESS_DATA, FieldAttrType.ASCII)
                };

            MTables = new List<QvxTable> {
                new QvxTable {
                    TableName = "ApplicationsEventLog",
                    GetRows = GetApplicationEvents,
                    Fields = applicationsEventLogFields
                },
                new QvxTable {
                    TableName = "SystemEventLog",
                    GetRows = GetSystemEvents,
                    Fields = systemEventLogFields
                }
            };
        }

        private IEnumerable<QvxDataRow> GetApplicationEvents()
        {
            QvxLog.Log(QvxLogFacility.Application, QvxLogSeverity.Notice, "GetApplicationEvents()");
            log.Debug("GetApplicationEvents()");

            return GetEvents("Application", "ApplicationsEventLog");
        }

        private IEnumerable<QvxDataRow> GetSystemEvents()
        {
            QvxLog.Log(QvxLogFacility.Application, QvxLogSeverity.Notice, "GetSystemEvents()");
            log.Debug("GetSystemEvents()");

            return GetEvents("System", "SystemEventLog");
        }

        private IEnumerable<QvxDataRow> GetEvents(string log, string tableName)
        {
            QvxLog.Log(QvxLogFacility.Application, QvxLogSeverity.Debug, String.Format("GetEvents(log: {0}, tableName: {1})", log, tableName));

            QvEventLogConnection.log.Debug($"GetSystemEvents(log: {log}, tableName: {tableName})");
            VerifyCredentials();

            if (!EventLog.Exists(log))
            {
                throw new QvxPleaseSendReplyException(QvxResult.QVX_TABLE_NOT_FOUND,
                    String.Format("There is no EventLog with name: {0}", tableName));
            }

            var ev = new EventLog(log);

            foreach (var evl in ev.Entries)
            {
                yield return MakeEntry(evl as EventLogEntry, FindTable(tableName, MTables));
            }
        }

        private QvxDataRow MakeEntry(EventLogEntry evl, QvxTable table)
        {
            var row = new QvxDataRow();
            row[table.Fields[0]] = evl.Category;
            row[table.Fields[1]] = evl.EntryType.ToString();
            row[table.Fields[2]] = evl.Message;
            row[table.Fields[3]] = evl.CategoryNumber.ToString();
            row[table.Fields[4]] = evl.Index.ToString();
            row[table.Fields[5]] = evl.MachineName;
            row[table.Fields[6]] = evl.Source;
            row[table.Fields[7]] = evl.TimeGenerated.ToString();
            return row;
        }

        private void VerifyCredentials()
        {
            QvxLog.Log(QvxLogFacility.Application, QvxLogSeverity.Notice, "VerifyCredentials()");
            log.Debug("VerifyCredentials()");

            string username, password;
            this.MParameters.TryGetValue("UserId", out username);
            this.MParameters.TryGetValue("Password", out password);

            // Hardcoded username and password
            if (username != "InterBlag" || password != "BlagBlag")
            {
                var error = "Username and/or passowrd is incorrect";
                QvxLog.Log(QvxLogFacility.Application, QvxLogSeverity.Notice, String.Format("VerifyCredentials() - {0}", error));
                log.Error($"VerifyCredentials() - {error}");
                throw new AuthenticationException(error);
            }
        }

        public override QvxDataTable ExtractQuery(string query, List<QvxTable> qvxTables)
        {
            return base.ExtractQuery(query, qvxTables);
        }
    }
}
