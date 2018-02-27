using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginsManager
{
    public partial class AboutWnd : Form
    {

        public AboutWnd()
        {
            InitializeComponent();
        }

        private void NameLabel_Click(object sender, EventArgs e)
        {

            MessageBox.Show(
                this,
                "Hello from Sirkadirov! ;)",
                "There is nothing to see here...",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly
            );

        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
