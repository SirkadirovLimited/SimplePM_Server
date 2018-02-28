using System;
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
                MessageBoxIcon.Error
            );

        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

    }

}
