using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EKonsulatConsole.GUI
{
    public partial class MainWindowForm : Form
    {
        public MainWindowForm()
        {
            InitializeComponent();
        }

        private void MainWindowForm_Load(object sender, EventArgs e)
        {
            Random proxyRandom = new Random();
            var lines = File.ReadAllLines(@"Proxy.txt");

            foreach (var line in lines)
            {
                txtProxy.AppendText(line + Environment.NewLine);
            }

            
            var selectRandomProxy = proxyRandom.Next(0, lines.Length);

        }
    }
}
