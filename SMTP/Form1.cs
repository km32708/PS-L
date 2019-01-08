using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Timers;

namespace SMTP
{
    public partial class Form1 : Form
    {
        SmtpSendMessage client;
        string user, password, url;
        int port, interval;
        System.Timers.Timer checkMailTimer;

        private void doWorkButton_Click(object sender, EventArgs e)
        {
            SmtpSendMessage newMessage = new SmtpSendMessage(loginTextBox.Text, passwordTextBox.Text, serverTextBox.Text, int.Parse(portTextBox.Text), fromTextBox.Text, toTextBox.Text, subjectTextBox.Text, messageTextBox.Text);

        }

        public Form1()
        {
            InitializeComponent();
            ReadConfig();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
       
        void ReadConfig()
        {
            string name = "config.ini";
            int i = 0;
            using (FileStream fileStream = File.OpenRead(Path.GetFullPath(name)))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                string line;
                while((line = streamReader.ReadLine()) != null)
                {
                    switch (i)
                    {
                        case 0:
                            user = line;
                            break;
                        case 1:
                            password = line;
                            break;
                        case 2:
                            url = line;
                            break;
                        case 3:
                            port = int.Parse(line);
                            break;
                        default:
                            break;
                    }
                    i++;
                }
            }
            loginTextBox.Text = user;
            passwordTextBox.Text = password;
            serverTextBox.Text = url;
            portTextBox.Text = port.ToString();
            fromTextBox.Text = user;

        }
    }

    
}
