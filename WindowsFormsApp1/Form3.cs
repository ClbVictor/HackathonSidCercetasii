using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        private string connectionString = @"";

        private string originalNume;
        private string originalAn;
        private string originalLuna;
        private string originalDataUpdate;
        private string originalPath;

        public Form3()
        {
            InitializeComponent();
            LoadYearsFromDatabase();
            LoadData("All");

            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Creare comandă SQL (exemplu: selectare toate coloanele dintr-un tabel)
                    string sql = "SELECT * FROM fisiere3";
                    using (OdbcCommand command = new OdbcCommand(sql, connection))
                    {
                        // Executare comandă și obținere rezultat
                        OdbcDataReader reader = command.ExecuteReader();

                        // Creare DataGridView și configurare coloane
                        dataGridView1.AutoGenerateColumns = true; // Ensure columns are auto-generated

                        // Adăugare rânduri în DataGridView
                        DataTable table = new DataTable();
                        table.Load(reader);
                        //Console.WriteLine("Rows count: " + table.Rows.Count); 
                        dataGridView1.DataSource = table;

                        reader.Close();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

            dataGridView1.Visible = true;

            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
        }


        private void LoadData(string selectedYear)
        {
            string sql = "SELECT * FROM fisiere3";

            if (selectedYear != "All")
            {
                sql += " WHERE an = ?";
            }

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(sql, connection))
                {
                    if (selectedYear != "All")
                    {
                        command.Parameters.AddWithValue("@an", selectedYear);
                    }

                    using (OdbcDataReader reader = command.ExecuteReader())
                    {
                        // Create a DataTable and populate it directly using the reader
                        DataTable table = new DataTable();
                        table.Load(reader);

                        // Set the DataGridView's DataSource to the populated DataTable
                        dataGridView1.DataSource = table;
                    }
                }
            }
        }

        private void LoadYearsFromDatabase()
        {
            string sql = "SELECT DISTINCT an FROM fisiere3";

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();
                using (OdbcCommand command = new OdbcCommand(sql, connection))
                {
                    using (OdbcDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader["an"].ToString());
                        }
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedYear = comboBox1.SelectedItem.ToString();
            LoadData(selectedYear);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Assign values to textboxes
                textBox1.Text = row.Cells["nume"].Value.ToString();
                textBox4.Text = row.Cells["an"].Value.ToString();
                textBox3.Text = row.Cells["luna"].Value.ToString();
                textBox2.Text = row.Cells["dataupdate"].Value.ToString();
                textBox5.Text = row.Cells["path"].Value.ToString();

                // Store original values for comparison
                originalNume = row.Cells["nume"].Value.ToString();
                originalAn = row.Cells["an"].Value.ToString();
                originalLuna = row.Cells["luna"].Value.ToString();
                originalDataUpdate = row.Cells["dataupdate"].Value.ToString();
                originalPath = row.Cells["path"].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    // Update command (using the original values for comparison)
                    string sqlupdate = "UPDATE fisiere3 SET an = "+textBox4.Text+ ", luna = "+textBox3.Text+ ", dataupdate = now(), path = '"+textBox5.Text+"' WHERE nume = '" + textBox1.Text+"'";
                    using (OdbcCommand command = new OdbcCommand(sqlupdate, connection))
                    {
                        
                        // Execute update
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Rows were updated.");
                        }
                        else
                        {
                            MessageBox.Show("Nothing was done."); 
                        }

                        // refresh automat
                        /*string stringcmd = "Select * from fisiere3";
                        OdbcCommand cmdrefresh = new OdbcCommand(stringcmd, connection);
                        OdbcDataAdapter da = new OdbcDataAdapter(cmdrefresh);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;*/
                        LoadData("All");

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }

}
