using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        //private string FileFolder = @"Z:\Programare\c#\fisiere";
        private string connectionString = @"";

        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*Form2 form2 = new Form2();
            form2.FolderSelected += Form2_FolderSelected;
            form2.ShowDialog();*/

            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a folder";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                folderBrowserDialog.ShowNewFolderButton = false;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog.SelectedPath;
                    label1.Text = folderPath;
                    listBox1.Items.Clear();

                    string[] files = Directory.GetFiles(folderPath);
                    foreach (string file in files)
                    {
                        listBox1.Items.Add(Path.GetFileName(file));
                    }
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //label1.Text = string.Empty;
            listBox1.Items.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                ArrangeFilesByCreateDate();
            }
            else if (radioButton2.Checked)
            {
                ArrangeFilesByDate();
            }
        }

        private void ArrangeFilesByCreateDate()
        {
            string selectedPath = label1.Text;

            if (Directory.Exists(selectedPath))
            {
                var files = Directory.GetFiles(selectedPath)
                                     .OrderBy(f => File.GetLastWriteTime(f))
                                     .ToList();

                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {
                    connection.Open();

                    foreach (var file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        DateTime lastWriteTime = fileInfo.LastWriteTime;
                        string year = lastWriteTime.Year.ToString();
                        string month = lastWriteTime.Month.ToString("D2");

                        string destinationFolder = Path.Combine(selectedPath, year, month);
                        if (!Directory.Exists(destinationFolder))
                        {
                            Directory.CreateDirectory(destinationFolder);
                        }

                        string destinationPath = Path.Combine(destinationFolder, fileInfo.Name);
                        if (!File.Exists(destinationPath))
                        {
                            File.Move(file, destinationPath);
                        }

                        if (!FileExistsInDatabase(connection, fileInfo.Name))
                        {
                            InsertFileIntoDatabase(connection, fileInfo.Name, year, month, DateTime.Now, destinationPath);
                        }
                    }

                    MessageBox.Show("fisiere repartizate + update.");
                }
            }
            else
            {
                MessageBox.Show("nu merge.");
            }

        }

        private bool FileExistsInDatabase(OdbcConnection connection, string fileName)
        {
            string query = "SELECT COUNT(*) FROM fisiere3 WHERE nume = ?";
            using (OdbcCommand command = new OdbcCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nume", fileName);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private void InsertFileIntoDatabase(OdbcConnection connection, string fileName, string year, string month, DateTime dateMoved, string filePath)
        {
            string query = "INSERT INTO fisiere3 (nume, an, luna, dataupdate, path) VALUES (?, ?, ?, ?, ?)";
            using (OdbcCommand command = new OdbcCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nume", fileName);
                command.Parameters.AddWithValue("@an", year);
                command.Parameters.AddWithValue("@luna", month);
                command.Parameters.AddWithValue("@dataupdate", dateMoved);
                command.Parameters.AddWithValue("@path", filePath);
                command.ExecuteNonQuery();
            }
        }

        private void ArrangeFilesByDate()
        {
            string selectedPath = label1.Text;
            if (Directory.Exists(selectedPath))
            {
                var files = Directory.GetFiles(selectedPath)
                                     .OrderBy(f => File.GetLastWriteTime(f))
                                     .ToList();

                using (OdbcConnection connection = new OdbcConnection(connectionString))
                {

                    connection.Open();

                    foreach (var file in files)
                    {
                        try
                        {
                            XDocument xmlDoc;
                            using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
                            {
                                xmlDoc = XDocument.Load(reader);
                            }

                            var issueDateElement = xmlDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "IssueDate");
                            if (issueDateElement != null)
                            {
                                DateTime issueDate = DateTime.Parse(issueDateElement.Value);
                                string year = issueDate.Year.ToString();
                                string month = issueDate.Month.ToString("D2");

                                UpdateFileInDatabase(connection, Path.GetFileName(file), year, month);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error processing file {file}: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("not working, ups.");
            }

        }

        private void UpdateFileInDatabase(OdbcConnection connection, string fileName, string year, string month)
        {
            string query = "UPDATE fisiere3 SET an = ?, luna = ? WHERE nume = ?";
            using (OdbcCommand command = new OdbcCommand(query, connection))
            {
                command.Parameters.AddWithValue("@an", year);
                command.Parameters.AddWithValue("@luna", month);
                command.Parameters.AddWithValue("@nume", fileName);
                command.ExecuteNonQuery();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }

        
    }
}
