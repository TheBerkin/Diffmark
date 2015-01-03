using System;
using System.Drawing;
using System.Windows.Forms;

namespace Diffmark.Demo
{
    public partial class DemoForm : Form
    {
        public DemoForm()
        {
            InitializeComponent();
            txtBaseString.Text = "world";
            txtPattern.Text = @"Hello,\s+ ; !";
            ResetTimer();
        }

        private static readonly Color GoodColor = SystemColors.Control;
        private static readonly Color BadColor = Color.OrangeRed;

        private void delayTimer_Tick(object sender, EventArgs e)
        {
            try
            {   
                txtOutput.Text = Diff.Mark(txtBaseString.Text, txtPattern.Text);
                txtOutput.ForeColor = GoodColor;
            }
            catch
            {
                txtOutput.ForeColor = BadColor;
            }
        }

        private void txtBaseString_TextChanged(object sender, EventArgs e)
        {
            ResetTimer();
        }

        private void txtPattern_TextChanged(object sender, EventArgs e)
        {
            ResetTimer();
        }

        private void ResetTimer()
        {
            delayTimer.Stop();
            delayTimer.Start();
        }
    }
}
