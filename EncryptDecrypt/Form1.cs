using Dashboard.Infrastructure.Security;
using WorkShop.Core.Constants;

namespace EncryptDecrypt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = Aes256.DefaultInstance.Encrypt(textBox1.Text, SecurityKey.Aes256Key);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = Aes256.DefaultInstance.Decrypt(textBox1.Text, SecurityKey.Aes256Key);
        }
    }
}
