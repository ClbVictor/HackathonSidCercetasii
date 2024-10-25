using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace FluxNoteV2
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            bool isAuthenticated = await FirebaseHelper.AuthenticateUser(username, password);

            if (isAuthenticated)
            {
                this.DialogResult = DialogResult.OK; // Set the dialog result to OK
                this.Close(); // Close the login form
            }
            else
            {
                MessageBox.Show("Invalid username or password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
