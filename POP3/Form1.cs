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

namespace POP3
{
    public partial class Form1 : Form
    {
        Pop3Client client;
        string user, password, url;
        int port, interval;
        System.Timers.Timer checkMailTimer;

        private void refreshMailButton_Click(object sender, EventArgs e)
        {
            RefreshMail();
        }
        void RefreshMail()
        {
            client.RefreshAll();
            RebuildList();
            if (client.ShouldNotify())
            {
                this.Invoke(new Action(() => newMailLabel.Text = "NEW MAIL"));

            }
            else
            {
                this.Invoke(new Action(() => newMailLabel.Text = "No new mail :("));

            }
        }
        public Form1()
        {
            InitializeComponent();
            ReadConfig();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            client = new Pop3Client(user,password, url, port); //albo 110
            refreshMailButton.Enabled = true;
            connectButton.Enabled = false;
            RefreshMail();
            SetUpTimer(); //na razie nie działa
        }

        void RebuildList()
        {

            this.Invoke(new Action(() => mailListBox.Items.Clear()));
            foreach (string key in client.messages.Keys)
            {
                this.Invoke(new Action(() => mailListBox.Items.Add(client.messages[key].Subject)));
                
            }
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
                        case 4:
                            interval = int.Parse(line);
                            break;
                        default:
                            break;
                    }
                    i++;
                }
            }
        }

        void SetUpTimer()
        {
            checkMailTimer = new System.Timers.Timer();
            checkMailTimer.Elapsed += new ElapsedEventHandler(TimerCallback);
            checkMailTimer.Interval = 1000 * interval;
            checkMailTimer.Start();
        }
        private  void TimerCallback(object source, ElapsedEventArgs e)
        {
            RefreshMail();
        }
    }

    
}
