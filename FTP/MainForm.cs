using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FTP
{
    public partial class MainForm : Form
    {
        private string user, password, address;
        private int port;
        private FTPmanager FTPclient;

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

        private void RefreshView() //refreshes current view using FTPClient's current data
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
            if (currentDirListView.SelectedItems.Count == 1)
            {
                ListViewItem selectedItem = currentDirListView.SelectedItems[0];
                string selectedItemString = selectedItem.SubItems[0].Text;
                if (selectedItemString[0] != 'd')
                {
                    MessageBox.Show("Can't move to non-directory!");
                    return;
                }

                int selectedItemInd = currentDirListView.SelectedIndices[0];
                //switching folders by full list's item index but using only item name
                if (FTPclient.pwd.Last() == '/')
                {
                    FTPclient.ChangePwd(FTPclient.pwd + FTPclient.pwnl[selectedItemInd] + "/");
                }
                else
                {
                    FTPclient.ChangePwd(FTPclient.pwd + "/" + FTPclient.pwnl[selectedItemInd] + "/");
                }

                RefreshView();
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            FTPclient.ChangePwd(currentPathTextBox.Text);
            RefreshView();
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            FTPclient.ChangePwdUp();
            RefreshView();
        }

        private void LoadConfig() //config is (line by line): user, pass, server address, main server port
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