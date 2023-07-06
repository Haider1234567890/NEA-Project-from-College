using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pandemic
{
    public partial class StartMenu : Form
    {
        public StartMenu()
        {
            InitializeComponent();
        }

        private void LoadGame(object sender, EventArgs e)
        {
            Form1 gameWindow = new Form1();

            gameWindow.ShowDialog();
        }

        private void LoadControls(object sender, EventArgs e)
        {
            HelpScreen gameWindow2 = new HelpScreen();

            gameWindow2.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Prologue gameWindow3 = new Prologue();

            gameWindow3.ShowDialog();
        }
    }
}