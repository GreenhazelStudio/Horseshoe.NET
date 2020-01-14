using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Horseshoe.NET;
using Horseshoe.NET.Application;
using Horseshoe.NET.Text;
using Horseshoe.NET.WebForms;
using Horseshoe.NET.WebForms.Bootstrap3;

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
                    var cellValue = TextUtil.Reveal(prop.GetValue(null), nullOrBlank: true);
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
                    var cellValue = TextUtil.Reveal(prop.GetValue(null), nullOrBlank: true, crlf: true);
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
                    var cellValue = TextUtil.Reveal(prop.GetValue(AppDomain.CurrentDomain), nullOrBlank: true, crlf: true);
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

        protected void infoAlertTest_Click(object sender, EventArgs e)
        {
            var alert = Alert.CreateInfoAlert("Polly want a cracker! <em>Squawk!</em>");
            var alertControl = alert.ToControl();
            BootstrapAlertArea.Controls.Add(alertControl);
        }

        protected void closeableWarningAlert_Click(object sender, EventArgs e)
        {
            var alert = Alert.CreateWarningAlert("What's shakin' bacon?", closeable: true);
            var alertControl = alert.ToControl();
            BootstrapAlertArea.Controls.Add(alertControl);
        }

        protected void errorAlertInAlert_Click(object sender, EventArgs e)
        {
            Exception ex = null;
            try
            {
                throw new Exception("It is what it is homey!");
            }
            catch(Exception _ex)
            {
                ex = _ex;
            }
            var alert = Alert.CreateErrorAlert(ex, displayFullClassName: true, displayStackTrace: true, errorRendering: ExceptionRenderingPolicy.InAlert);
            var alertControl = alert.ToControl();
            BootstrapAlertArea.Controls.Add(alertControl);
        }
    }
}