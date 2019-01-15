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

namespace FTP
{
    public partial class MainForm : Form
    {
        string user, password, address;
        int port;
        FTPmanager FTPclient;
        public MainForm()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FTPclient = new FTPmanager(user, password, address, port);
            RefreshView();
        }

        private void RefreshView()
        {
            currentPathTextBox.Text = FTPclient.pwd;
            currentDirListView.Clear();
            foreach (var item in FTPclient.pwl)
            {
                currentDirListView.Items.Add(item);
            }
        }

        private void currentDirListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (currentDirListView.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = currentDirListView.SelectedItems[0];
                string selectedItemString = selectedItem.SubItems[0].Text;
                //FTPclient.ChangePwd()
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            FTPclient.ChangePwdUp();
            RefreshView();
        }

        private void LoadConfig()
        {
            string name = "config.ini";
            int i = 0;
            if (!File.Exists(Path.GetFullPath(name)))
            {
                return;
            }            
            using (FileStream fileStream = File.OpenRead(Path.GetFullPath(name)))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
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
                            address = line;
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
            userTextBox.Text = user;
            passwordTextBox.Text = password;
            addressTextBox.Text = address;
            portNumeric.Value = port;
        }
    }
}
