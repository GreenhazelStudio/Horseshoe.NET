using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Horseshoe.NET.Text;

namespace TestWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // console properties
            var consoleProperties = typeof(Console).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var prop in consoleProperties)
            {
                textBox1.AppendText(prop.Name.PadRight(25) + " ");
                try
                {
                    textBox1.AppendLine(TextUtil.RevealNullOrBlank(prop.GetValue(null)));
                }
                catch (Exception ex)
                {
                    textBox1.AppendLine(ex.GetType().Name);
                }
            }

            // environment properties
            var environmentProperties = typeof(Environment).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var prop in environmentProperties)
            {
                textBox2.AppendText(prop.Name.PadRight(25) + " ");
                try
                {
                    if (prop.Name.Equals("NewLine"))
                    {
                        textBox2.AppendLine(TextUtil.RevealWhiteSpace(prop.GetValue(null)));
                    }
                    else
                    {
                        textBox2.AppendLine(TextUtil.RevealNullOrBlank(prop.GetValue(null)));
                    }
                }
                catch (Exception ex)
                {
                    textBox2.AppendLine(ex.GetType().Name);
                }
            }

            // app domain properties
            var appDomainProperties = typeof(AppDomain).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in appDomainProperties)
            {
                textBox3.AppendText(prop.Name.PadRight(25) + " ");
                try
                {
                    textBox3.AppendLine(TextUtil.RevealNullOrBlank(prop.GetValue(AppDomain.CurrentDomain)));
                }
                catch (Exception ex)
                {
                    textBox3.AppendLine(ex.GetType().Name);
                }
            }
        }
    }
}
