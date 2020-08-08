using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

using Horseshoe.NET.Collections.Extensions;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.Dates;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.Extensions;

namespace Horseshoe.NET.Extensions
{
    public static class Extensions
    {
        public static string GetDisplayName(this Assembly assembly, int minDepth = 1)
        {
            return GetDisplayName(assembly.GetName(), minDepth: minDepth);
        }

        public static string GetDisplayName(this AssemblyName assemblyName, int minDepth = 1)
        {
            return assemblyName.Name + " " + GetDisplayVersion(assemblyName, minDepth: minDepth);
        }

        public static string GetDisplayVersion(this Assembly assembly, int minDepth = 1)
        {
            return GetDisplayVersion(assembly.GetName(), minDepth: minDepth);
        }

        public static string GetDisplayVersion(this AssemblyName assemblyName, int minDepth = 1)
        {
            return Display(assemblyName.Version, minDepth: minDepth);
        }

        public static string Display(this Version version, int minDepth = 1)
        {
            if (minDepth < 1) throw new UtilityException("minDepth must be at least 1");

            var sb = new StringBuilder(version.Major.ToString());

            if (minDepth > 1 || version.Minor > 0 || version.Build > 0 || version.Revision > 0)
            {
                sb.Append("." + version.Minor);

                if (minDepth > 2 || version.Build > 0 || version.Revision > 0)
                {
                    sb.Append("." + version.Build);

                    if (minDepth > 3 || version.Revision > 0)
                    {
                        sb.Append("." + version.Revision);
                    }
                }
            }

            return sb.ToString();
        }

        public static bool In<E>(this E obj, params E[] collection)
        {
            return In(obj, collection as IEnumerable<E>);
        }

        public static bool In<E>(this E obj, IEnumerable<E> collection)
        {
            if (collection == null) return false;
            return collection.Contains(obj);
        }

        public static bool InIgnoreCase(this string text, params string[] collection)
        {
            return InIgnoreCase(text, collection as IEnumerable<string>);
        }

        public static bool InIgnoreCase(this string text, IEnumerable<string> collection)
        {
            if (collection == null) return false;
            foreach (var s in collection)
            {
                if (string.Equals(s, text, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        public static string Render(this Exception exception, bool displayShortName = false, bool displayMessage = true, bool displayStackTrace = true, int indent = 2, bool recursive = false)
        {
            var strb = new StringBuilder();
            RenderRecursive(exception, strb, displayShortName, displayMessage, displayStackTrace, indent, recursive);
            return strb.ToString();
        }

        public static string Render(this ExceptionInfo exceptionInfo, bool displayShortName = false, bool displayMessage = true, bool displayStackTrace = true, int indent = 2, bool recursive = false)
        {
            var strb = new StringBuilder();
            RenderRecursive(exceptionInfo, strb, displayShortName, displayMessage, displayStackTrace, indent, recursive);
            return strb.ToString();
        }

        public static string RenderHtml(this Exception exception, bool displayShortName = false, bool displayMessage = true, bool displayStackTrace = true, int indent = 2, bool recursive = false)
        {
            var text = Render(exception, displayShortName: displayShortName, displayMessage: displayMessage, displayStackTrace: displayStackTrace, indent: indent, recursive: recursive);
            var html = text.ToHtml();
            return html;
        }

        public static string RenderHtml(this ExceptionInfo exceptionInfo, bool displayShortName = false, bool displayMessage = true, bool displayStackTrace = true, int indent = 2, bool recursive = false)
        {
            var text = Render(exceptionInfo, displayShortName: displayShortName, displayMessage: displayMessage, displayStackTrace: displayStackTrace, indent: indent, recursive: recursive);
            var html = text.ToHtml();
            return html;
        }

        private static void RenderRecursive(Exception exception, StringBuilder strb, bool displayShortName, bool displayMessage, bool displayStackTrace, int indent, bool recursive)
        {
            strb.Append
            (
                exception is ReconstitutedException reconstitutedException
                    ? (displayShortName ? reconstitutedException.ClassName : reconstitutedException.FullClassName)
                    : (displayShortName ? exception.GetType().Name : exception.GetType().FullName)
            ).Append(":");
            if (displayMessage && exception.Message != null)
            {
                strb.Append
                (
                    Environment.NewLine + new string(' ', indent) + TextUtil.RevealNullOrBlank(exception.Message)
                );
            }
            if (displayStackTrace && exception.StackTrace != null)
            {
                strb.Append
                (
                    Environment.NewLine + "Stack Trace:" +
                    Environment.NewLine + IndentStackTrace(exception.StackTrace, indent)
                );
            }
            if (recursive && exception.InnerException != null)
            {
                strb.AppendLine();
                RenderRecursive(exception.InnerException, strb, displayShortName, displayMessage, displayStackTrace, indent, recursive);
            }
        }

        static string IndentStackTrace(string stackTrace, int indent)
        {
            if (stackTrace == null) return null;
            var lines = stackTrace.Split('\r', '\n')
                .ZapAndPrune()
                .Select(ln => new string(' ', indent) + ln);
            return string.Join(Environment.NewLine, lines);
        }

        public static Age GetAge(this DateTime from, DateTime? asOf = null, int decimals = -1)
        {
            return new Age(from, asOf ?? DateTime.Now, decimals: decimals);
        }

        public static int GetAgeInYears(this DateTime from, DateTime? asOf = null)
        {
            return new Age(from, asOf ?? DateTime.Now).Years;
        }

        public static double GetTotalAgeInYears(this DateTime from, DateTime? asOf = null, int decimals = -1)
        {
           return new Age(from, asOf ?? DateTime.Now, decimals: decimals).TotalYears;
        }

        public static int GetAgeInMonths(this DateTime from, DateTime? asOf = null)
        {
            var age = new Age(from, asOf ?? DateTime.Now);
            return age.Years * 12 + age.Months;
        }

        public static double GetTotalAgeInMonths(this DateTime from, DateTime? asOf = null, int decimals = -1)
        {
            return new Age(from, asOf ?? DateTime.Now, decimals: decimals).TotalMonths;
        }

        public static int GetAgeInDays(this DateTime from, DateTime? asOf = null)
        {
            return ((asOf ?? DateTime.Now) - from).Days;
        }

        public static double GetTotalAgeInDays(this DateTime from, DateTime? asOf = null, int decimals = -1)
        {
            var totalDays = ((asOf ?? DateTime.Now) - from).TotalDays;
            return decimals >= 0
                ? Math.Round(totalDays, decimals)
                : totalDays;
        }

        public static int GetNumberOfDaysInMonth(this DateTime date)
        {
            return DateUtil.GetNumberOfDaysInMonth(date.Year, date.Month);
        }

        public static bool IsLeapYear(this DateTime date)
        {
            return DateUtil.IsLeapYear(date);
        }

        public static bool IsSameDay(this DateTime date, DateTime other)
        {
            return DateUtil.SameDay(date, other);
        }

        public static bool IsSameMonth(this DateTime date, DateTime other)
        {
            return DateUtil.SameMonth(date, other);
        }

        public static NetworkCredential ToNetworkCredentials(this Credential credentials)
        {
            if (credentials.HasSecurePassword)
            {
                return credentials.Domain != null 
                    ? new NetworkCredential(credentials.UserName, credentials.SecurePassword, credentials.Domain)
                    : new NetworkCredential(credentials.UserName, credentials.SecurePassword);
            }
            else if (credentials.IsEncryptedPassword)
            {
                return credentials.Domain != null
                    ? new NetworkCredential(credentials.UserName, Decrypt.SecureString(credentials.Password), credentials.Domain)
                    : new NetworkCredential(credentials.UserName, Decrypt.SecureString(credentials.Password));
            }
            else
            {
                return credentials.Domain != null
                    ? new NetworkCredential(credentials.UserName, credentials.Password, credentials.Domain)
                    : new NetworkCredential(credentials.UserName, credentials.Password);
            }
        }
    }
}
