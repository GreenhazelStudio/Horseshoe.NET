using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Horseshoe.NET.IO.ReportingServices
{
    public static class ReportUtil
    {
        public static event Action<string> ReportUrlGenerated;

        public static string BuildFileUrl(string reportPath, string reportServer = null, IDictionary<string, object> userParameters = null, ReportFormat reportFormat = ReportFormat.PDF, bool announce = false)
        {
            if (reportPath == null) throw new ArgumentNullException(nameof(reportPath));
            reportServer = reportServer ?? Settings.DefaultReportServer;
            if (reportServer == null) throw new ArgumentNullException(nameof(reportServer));
            if (reportServer.EndsWith("/")) reportServer = reportServer.Substring(0, reportServer.Length - 1);  // remove trailing slash, if applicable
            var parameterPortionOfQueryString = BuildReportParametersPortionOfQueryString(userParameters);
            var sb = new StringBuilder(reportServer)      // e.g. http://reports.mycompany.com
                .Append("/ReportServer?")
                .Append(reportPath.Replace(" ", "%20"))   // e.g. /Acct/Expense Report => /Acct/Expense%20Report
                .Append(parameterPortionOfQueryString)    // e.g. &user=Bob Cratchit   => &user=Bob%20Cratchit
                .Append("&rs:Command=Render")
                .Append("&rs:Format=")
                .Append(reportFormat);                    // e.g. PDF, EXCEL
            if (announce)
            {
                ReportUrlGenerated?.Invoke(sb.ToString());
            }
            return sb.ToString();                         // http://reports.mycompany.com/ReportServer?/Acct/Expense%20Report&user=Bob%20Cratchit&rs:Command=Render&rs:Format=PDF
        }

        public static string BuildHyperlinkUrl(string reportPath, string reportServer = null, IDictionary<string, object> userParameters = null, bool announce = false)
        {
            if (reportPath == null) throw new ArgumentNullException(nameof(reportPath));
            reportServer = reportServer ?? Settings.DefaultReportServer;
            if (reportServer == null) throw new ArgumentNullException(nameof(reportServer));
            if (reportServer.EndsWith("/")) reportServer = reportServer.Substring(0, reportServer.Length - 1);  // remove trailing slash, if applicable
            var parameterPortionOfQueryString = BuildReportParametersPortionOfQueryString(userParameters);
            var sb = new StringBuilder(reportServer)      // e.g. http://reports.mycompany.com
                .Append("/reports/report")
                .Append(reportPath.Replace(" ", "%20"))   // e.g. /Acct/Expense Report => /Acct/Expense%20Report
                .Append(parameterPortionOfQueryString);   // e.g. &user=Bob Cratchit   => &user=Bob%20Cratchit
            if (announce)
            {
                ReportUrlGenerated?.Invoke(sb.ToString());
            }
            return sb.ToString();                         // http://reports.mycompany.com/reports/report/Acct/Expense%20Report&user=Bob%20Cratchit
        }

        public static string BuildReportParametersPortionOfQueryString(IDictionary<string, object> userParameters)
        {
            if (userParameters == null || !userParameters.Any())
            {
                return "";
            }
            return "&" + string.Join("&", userParameters.Select(pkvp => pkvp.Key + "=" + string.Join(",", ParseParamValues(pkvp.Value)).Replace(" ", "%20")));
        }

        public static string ParseReportName(string reportPath)
        {
            var reportName = reportPath.Any(c => c == '/')
                ? reportPath.Substring(reportPath.LastIndexOf("/") + 1)
                : reportPath;
            return reportName;
        }

        internal static string[] ParseParamValues(object o)
        {
            if (o == null)
            {
                return new string[] { null };
            }
            else if (o is DateTime)
            {
                return new string[] { ((DateTime)o).ToShortDateString() };
            }
            else if (o is IEnumerable<string>)
            {
                return ((IEnumerable<string>)o).ToArray();
            }
            else if (o is IEnumerable<int>)
            {
                return ((IEnumerable<int>)o).Select(n => n.ToString()).ToArray();
            }
            else if (o is IEnumerable<int?>)
            {
                return ((IEnumerable<int?>)o).Select(n => n?.ToString()).ToArray();
            }
            else if (o is IEnumerable<double>)
            {
                return ((IEnumerable<double>)o).Select(n => n.ToString()).ToArray();
            }
            else if (o is IEnumerable<double?>)
            {
                return ((IEnumerable<double?>)o).Select(n => n?.ToString()).ToArray();
            }
            else if (o is IEnumerable<long>)
            {
                return ((IEnumerable<long>)o).Select(n => n.ToString()).ToArray();
            }
            else if (o is IEnumerable<long?>)
            {
                return ((IEnumerable<long?>)o).Select(n => n?.ToString()).ToArray();
            }
            else if (o is IEnumerable<decimal>)
            {
                return ((IEnumerable<decimal>)o).Select(n => n.ToString()).ToArray();
            }
            else if (o is IEnumerable<decimal?>)
            {
                return ((IEnumerable<decimal?>)o).Select(n => n?.ToString()).ToArray();
            }
            else if (o is IEnumerable<DateTime>)
            {
                return ((IEnumerable<DateTime>)o).Select(n => n.ToShortDateString()).ToArray();
            }
            else if (o is IEnumerable<DateTime?>)
            {
                return ((IEnumerable<DateTime?>)o).Select(n => n?.ToShortDateString()).ToArray();
            }
            return new string[] { o.ToString() };
        }

        internal static FileType ConvertOutputTypeToFileType(ReportFormat reportOutputType)
        {
            switch (reportOutputType)
            {
                case ReportFormat.EXCEL:
                    return FileType.XLS;
                case ReportFormat.PDF:
                default:
                    return FileType.PDF;
            }
        }
    }
}
