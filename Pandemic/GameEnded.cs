using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pandemic
{
    public partial class GameEnded : Form
    {
        public GameEnded()
        {
            InitializeComponent();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\OneDrive\Documents\A level Computer Science\Pandemic NEA\Pandemic v2 (2)\Pandemic v2\Pandemic\Pandemic\LeaderBoard.mdf;Integrated Security=True");
            SqlCommand cmd = new SqlCommand("INSERT INTO Players(Firstname, LastName) OUTPUT INSERTED.PlayerID VALUES(@Firstname, @LastName)", con);
            cmd.Parameters.AddWithValue("@Firstname", textBox1.Text);
            cmd.Parameters.AddWithValue("@LastName", textBox2.Text);
            con.Open();
            int i = (int)(cmd.ExecuteScalar());
            SqlCommand cmd2 = new SqlCommand("INSERT INTO playerScores(Score, PlayerID) VALUES(@Score, @PlayerID)", con);
            cmd2.Parameters.AddWithValue("@Score", textBox3.Text);
            cmd2.Parameters.AddWithValue("@PlayerID", i);
            int j = cmd2.ExecuteNonQuery();
            con.Close();

            if (i != 0 && j != 0)
            {
                MessageBox.Show("Data Saved.");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
