﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Events;

using Excel = Microsoft.Office.Interop.Excel;

namespace Horseshoe.NET.ExcelHelper.Interop
{
    public static class ConvertExcelFile
    {
        public static EasyNotifier<string> ConversionCanceled;

        public static ConverionSource From(string filePath)
        {
            return new ConverionSource(filePath);
        }

        public class ConverionSource
        {
            public string FilePath { get; }
            internal ConverionSource(string filePath) { FilePath = filePath; }

            public ConversionDest To(ExcelFileTypeByExtension fileType)
            {
                return new ConversionDest(this, null, fileType);
            }

            public ConversionDest To(string filePath, ExcelFileTypeByExtension fileType)
            {
                return new ConversionDest(this, filePath, fileType);
            }
        }

        public class ConversionDest
        {
            public ConverionSource Source { get; }
            public string DestFilePath { get; private set; }
            public ExcelFileTypeByExtension DestFileType { get; }

            internal ConversionDest(ConverionSource source, string filePath, ExcelFileTypeByExtension fileType) 
            {
                Source = source;
                DestFilePath = filePath;
                DestFileType = fileType;
            }

            public string Execute(bool overwriteExisting = false, bool allowAnyExtension = false, bool ignoreSourceFormat = false)
            {
                var sourceFile = new FileInfo(Source.FilePath);
                if (!sourceFile.Exists)
                {
                    throw new ValidationException("Source file does not exist or access is denied: " + sourceFile.FullName);
                }

                if (sourceFile.Extension.Length == 0)
                {
                    if (!allowAnyExtension)
                    {
                        throw new ValidationException("Source file is missing extension (to bypass this set allowAnyExtension = true and try again).");
                    }
                }
                else
                {
                    var acceptedExcelFileTypes = Enum.GetNames(typeof(ExcelFileTypeByExtension));
                    if (!sourceFile.Extension.Substring(1).InIgnoreCase(acceptedExcelFileTypes) && !allowAnyExtension)
                    {
                        throw new ValidationException("\"" + sourceFile.Extension + "\" is not an accepted file extension, try one of [" + string.Join(", ", acceptedExcelFileTypes) + "] (to bypass this set allowAnyExtension = true and try again).");
                    }

                    if (string.Equals(sourceFile.Extension.Substring(1), DestFileType.ToString(), StringComparison.OrdinalIgnoreCase) && !ignoreSourceFormat)
                    {
                        throw new ValidationException("Source file appears to already be in the requested format (to bypass this set ignoreSourceFormat = true and try again).");
                    }
                }

                DestFilePath = DestFilePath 
                    ?? Path.Combine(sourceFile.DirectoryName, sourceFile.Name.Replace(sourceFile.Extension, "") + "." + DestFileType.ToString().ToLower());

                if (File.Exists(this.DestFilePath) && !overwriteExisting)
                {
                    throw new ValidationException("Destination file already exists (to bypass this set overwriteExisting = true and try again).");
                }

                var app = new Excel.Application
                {
                    Visible = false,
                    DisplayAlerts = false
                };
                var workbook = app.Workbooks.Open(Source.FilePath, Excel.XlUpdateLinks.xlUpdateLinksNever, true);
                workbook.SaveAs(DestFilePath, DestFileType);
                workbook.Close(false);
                app.Quit();
                return DestFilePath;
            }
        }
    }
}
