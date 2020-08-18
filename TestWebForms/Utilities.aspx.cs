using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Horseshoe.NET.Extensions;
using Horseshoe.NET.Application;
using Horseshoe.NET.Text;

namespace TestWebForms
{
    public partial class Utilities : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var tr = new TableRow();
            tr.Cells.Add
            (
                new TableHeaderCell { ColumnSpan = 2, CssClass = "section-header", Text = "Bootstrap Tests" }
            );
            Table1.Rows.Add(tr);

            tr = new TableRow();
            tr.Cells.Add
            (
                new TableHeaderCell { Text = "Test Links" }
            );
            var bootstrapTD = new TableCell();
            bootstrapTD.Controls.Add(new LinkButton { Text = "Info" });
            bootstrapTD.Controls.Add(new LiteralControl("<br/>"));
            tr.Cells.Add(bootstrapTD);
            Table1.Rows.Add(tr);

            tr = new TableRow();
            tr.Cells.Add
            (
                new TableHeaderCell { ColumnSpan = 2, CssClass = "section-header", Text = "Utility Tests" }
            );
            Table1.Rows.Add(tr);

            tr = new TableRow();
            tr.Cells.Add
            (
                new TableHeaderCell { Text = "App Type" }
            );
            var messageTracker = new StringBuilder();
            tr.Cells.Add
            (
                new TableCell { Text = ClientApp.DetectAppType(messageTracker: messageTracker)?.ToString() ?? "[null]" }
            );
            Table1.Rows.Add(tr);

            tr = new TableRow();
            tr.Cells.Add
            (
                new TableHeaderCell { Text = "Message Tracker" }
            );
            tr.Cells.Add
            (
                new TableCell { Text = messageTracker.ToString().Replace(Environment.NewLine, "<br/>") }
            );
            Table1.Rows.Add(tr);

            tr = new TableRow();
            tr.Cells.Add
            (
                new TableHeaderCell { ColumnSpan = 2, CssClass = "section-header", Text = "Console Properties" }
            );
            Table1.Rows.Add(tr);

            var consoleProperties = typeof(Console).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var prop in consoleProperties)
            {
                tr = new TableRow();
                tr.Cells.Add
                (
                    new TableHeaderCell { Text = prop.Name }
                );
                try
                {
                    var cellValue = TextUtil.RevealNullOrBlank(prop.GetValue(null));
                    tr.Cells.Add
                    (
                        new TableCell { Text = cellValue }
                    );
                }
                catch (Exception ex)
                {
                    tr.Cells.Add
                    (
                        new TableCell { Text = HttpUtility.HtmlEncode(ex.Render()).Replace("\n", "<br />"), ForeColor = Color.Red }
                    );
                }
                Table1.Rows.Add(tr);
            }

            tr = new TableRow();
            tr.Cells.Add
            (
                new TableHeaderCell { ColumnSpan = 2, CssClass = "section-header", Text = "Environment Properties" }
            );
            Table1.Rows.Add(tr);

            var environmentProperties = typeof(Environment).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var prop in environmentProperties)
            {
                tr = new TableRow();
                tr.Cells.Add
                (
                    new TableHeaderCell { Text = prop.Name }
                );
                try
                {
                    var cellValue = TextUtil.RevealWhiteSpace(prop.GetValue(null));
                    tr.Cells.Add
                    (
                        new TableCell { Text = cellValue }
                    );
                }
                catch (Exception ex)
                {
                    tr.Cells.Add
                    (
                        new TableCell { Text = HttpUtility.HtmlEncode(ex.Render()).Replace("\n", "<br />"), ForeColor = Color.Red }
                    );
                }
                Table1.Rows.Add(tr);
            }

            tr = new TableRow();
            tr.Cells.Add
            (
                new TableHeaderCell { ColumnSpan = 2, CssClass = "section-header", Text = "App Domain Properties" }
            );
            Table1.Rows.Add(tr);

            var appDomainProperties = typeof(AppDomain).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in appDomainProperties)
            {
                tr = new TableRow();
                tr.Cells.Add
                (
                    new TableHeaderCell { Text = prop.Name }
                );
                try
                {
                    var cellValue = TextUtil.RevealWhiteSpace(prop.GetValue(AppDomain.CurrentDomain));
                    tr.Cells.Add
                    (
                        new TableCell { Text = cellValue }
                    );
                }
                catch (Exception ex)
                {
                    tr.Cells.Add
                    (
                        new TableCell { Text = HttpUtility.HtmlEncode(ex.Render()).Replace("\n", "<br />"), ForeColor = Color.Red }
                    );
                }
                Table1.Rows.Add(tr);
            }
        }
    }
}