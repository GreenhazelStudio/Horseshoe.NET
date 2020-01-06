using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Horseshoe.NET.IO;

namespace Horseshoe.NET.IO.ReportingServices
{
    public static class ReportServer
    {
        public static string Render(string reportPath, string reportServer = null, IDictionary<string, object> userParameters = null, ReportFormat reportFormat = ReportFormat.PDF, string targetFileName = null, string targetDirectory = null, Credential? credentials = null)
        {
            var targetExt = ReportUtil.ConvertOutputTypeToFileType(reportFormat);
            var bytes = RenderBytes(reportPath, reportServer: reportServer, userParameters: userParameters, reportFormat: reportFormat, credentials: credentials);

            return FileUtil.Create
            (
                bytes,
                targetDirectory: targetDirectory,
                targetFileName: FileUtil.AppendExtensionIf(targetFileName ?? ReportUtil.ParseReportName(reportPath), targetExt, true),
                fileType: targetExt
            );
        }

        public static byte[] RenderBytes(string url, IDictionary<string, object> userParameters = null, Credential? credentials = null)
        {
            try
            {
                return FileUtil.BytesFromWeb(url, credentials: credentials);
            }
            catch (WebException wex)
            {
                throw new ReportException("Web Error (" + wex.Status + "): Check if the report server is operational and double check your report parameters (if applicable), dynamic or user-supplied URL (including report path), report options, credentials, etc.", wex) { Parameters = userParameters };
            }
        }

        public static byte[] RenderBytes(string reportPath, string reportServer = null, IDictionary<string, object> userParameters = null, ReportFormat reportFormat = ReportFormat.PDF, Credential? credentials = null)
        {
            var url = ReportUtil.BuildURL(reportPath, userParameters: userParameters, reportServer: reportServer, reportFormat: reportFormat);

            return RenderBytes(url, credentials: credentials, userParameters: userParameters);
        }
    }
}
