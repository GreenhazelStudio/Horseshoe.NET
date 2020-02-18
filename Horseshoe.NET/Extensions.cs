using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Cryptography;
using Horseshoe.NET.Text;

namespace Horseshoe.NET
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

        public static int AgeInYearsFrom(this DateTime from)
        {
            return DateUtil.AgeInYears(from);
        }

        public static double TotalAgeInYearsFrom(this DateTime from, int decimals = -1)
        {
            return DateUtil.TotalAgeInYears(from, decimals: decimals);
        }

        public static int AgeInMonthsFrom(this DateTime from)
        {
            return DateUtil.AgeInMonths(from);
        }

        public static double TotalAgeInMonthsFrom(this DateTime from, int decimals = -1)
        {
            return DateUtil.TotalAgeInMonths(from, decimals: decimals);
        }

        public static int AgeInDaysFrom(this DateTime from)
        {
            return DateUtil.AgeInDays(from);
        }

        public static double TotalAgeInDaysFrom(this DateTime from, int decimals = -1)
        {
            return DateUtil.TotalAgeInDays(from, decimals: decimals);
        }

        public static bool IsLeapYear(this DateTime date)
        {
            return DateUtil.IsLeapYear(date);
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
