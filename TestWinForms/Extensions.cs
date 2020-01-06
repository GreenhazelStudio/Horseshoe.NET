using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWinForms
{
    public static class Extensions
    {
        public static void AppendLine(this TextBox textBox, string text)
        {
            textBox.AppendText(text + Environment.NewLine);
        }
    }
}
