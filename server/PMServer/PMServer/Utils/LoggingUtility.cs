using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Tracing;
using NLog;
using System.Net.Http;
using System.Text;

namespace PMServer.Utils
{
    public class LoggingUtility : ITraceWriter
    {
        private static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<Dictionary<TraceLevel, Action<string>>> LoggingMap = new Lazy<Dictionary<TraceLevel, Action<string>>>(() => new Dictionary<TraceLevel, Action<string>> { { TraceLevel.Info, ClassLogger.Info }, { TraceLevel.Debug, ClassLogger.Debug }, { TraceLevel.Error, ClassLogger.Error }, { TraceLevel.Fatal, ClassLogger.Fatal }, { TraceLevel.Warn, ClassLogger.Warn } });
        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if (level != TraceLevel.Off)
            {
                if (traceAction != null && traceAction.Target != null)
                {
                    category = category + Environment.NewLine + "Action Parameters : " + traceAction.Target.ToString();
                }
                var record = new TraceRecord(request, category, level);
                if (traceAction != null) traceAction(record);
                Log(record);
            }
        }

        private void Log(TraceRecord record)
        {
            var log = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(record.Message))
                log.AppendFormat("Operation Result Format: {0}", record.Message);

            if (record.RequestId != null)
                log.AppendFormat("Request ID: {0}", record.RequestId.ToString());

            if (record.Request != null)
            {
                if (record.Request.Method != null)
                    log.AppendFormat("Method: {0}", record.Request.Method);

                if (record.Request.RequestUri != null)
                    log.AppendFormat("URL: {0}", record.Request.RequestUri);

                if (record.Request.Headers != null && record.Request.Headers.Contains("Token") && record.Request.Headers.GetValues("Token") != null && record.Request.Headers.GetValues("Token").FirstOrDefault() != null)
                    log.AppendFormat("Token: {0}", record.Request.Headers.GetValues("Token").FirstOrDefault());
            }

            if (!string.IsNullOrWhiteSpace(record.Category))
                log.Append(record.Category);

            if (!string.IsNullOrWhiteSpace(record.Operator))
                log.AppendFormat(" {0} {1}", record.Operator, record.Operation);

            if (record.Exception != null && !string.IsNullOrWhiteSpace(record.Exception.GetBaseException().Message))
            {
                var exceptionType = record.Exception.GetType();
                log.Append(Environment.NewLine);
                log.AppendFormat("Error: {0}", record.Exception.GetBaseException().Message);
            }
            var logger = NLog.LogManager.GetCurrentClassLogger();

            logger.Info(log.ToString() + Environment.NewLine);
        }
    }
}