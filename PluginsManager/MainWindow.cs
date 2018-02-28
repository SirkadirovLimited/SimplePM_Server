using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PluginsManager
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AboutBtn_Click(object sender, EventArgs e)
        {

            new AboutWnd().Show(this);

        }

        private void WebsiteBtn_Click(object sender, EventArgs e)
        {

            Process.Start("https://spm.sirkadirov.com/");

        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {

            Application.Exit();

        }
    }
}
