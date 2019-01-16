using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace FTP
{
    internal class FTPmanager
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;

        private TcpClient clientSecondary;
        private NetworkStream streamSecondary;
        private StreamReader readerSecondary;
        private StreamWriter writerSecondary;

        private string _address;

        public bool IsEverythingOk = true;
        public string pwd = " ";
        public List<string> pwl; //pwd list
        public List<string> pwnl; //pwd nlst
        private string lastMessage = " ";
        private string lastMessageSecondary = " ";

        ~FTPmanager()
        {
            if (writer != null)
            {
                writer.WriteLine("QUIT");
                Wbtc();
            }
        }

        public FTPmanager(string user, string password, string address, int port)
        {
            _address = address; //saved, we'll need that later

            try
            {
                //opening first stream, no ssl this time
                client = new TcpClient();
                client.Connect(address, port);
                stream = client.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);
            }
            catch
            {
                IsEverythingOk = false;
                return;
            }

            //cleanup buffer
            Wbtc();
            Wbtc();

            if (!IsOk(ReplyCode(lastMessage)))
            {
                IsEverythingOk = false;
                return;
            }

            //login
            Wlf("USER " + user);
            Wbtc();

            Wlf("PASS " + password);
            Wbtc();

            //initial refresh
            RefreshPwd();
            RefreshList();
        }

        private void Wlf(string line) //write line and flush
        {
            Console.WriteLine("--> " + line);
            writer.WriteLine(line);
            writer.Flush();
        }

        private void Wbtc() //write buffer to console
        {
            try
            {
                lastMessage = reader.ReadLine();
            }
            catch
            {
                return;
            }
            while (reader.Peek() >= 0)
            {
                Console.WriteLine("<-- " + lastMessage);
                if (reader.EndOfStream)
                {
                    break;
                }
                lastMessage = reader.ReadLine();
            }
            Console.WriteLine("<-- " + lastMessage);
        }

        private void Wsbtc() //write secondary buffer to console
        {
            if (readerSecondary == null)
            {
                return;
            }
            try
            {
                lastMessageSecondary = readerSecondary.ReadLine();
            }
            catch
            {
                return;
            }
            while (readerSecondary.Peek() >= 0)
            {
                Console.WriteLine("<2- " + lastMessageSecondary);
                if (readerSecondary.EndOfStream)
                {
                    break;
                }
                lastMessageSecondary = readerSecondary.ReadLine();
            }
            Console.WriteLine("<2- " + lastMessageSecondary);
        }

        private int ReplyCode(string responseLine) //self-explainatory
        {
            return int.Parse(responseLine.Substring(0, 3));
        }

        private bool IsOk(int Code)
        {
            return ((Code / 100) == 2);
        }

        public void RefreshPwd() //runs pwd and updates class variable
        {
            Wlf("PWD");
            Wbtc();
            if (!IsOk(ReplyCode(lastMessage)))
            {
                IsEverythingOk = false;
                return;
            }
            MatchCollection Matches = Regex.Matches(lastMessage, "\\\"(.*?)\\\"");
            pwd = Matches[0].ToString().Trim('"');
        }

        private int CalculatePortByResponse(string Response) //for listing with list and nlst
        {
            string NumberString = Response.Split('(', ')')[1];
            var Numbers = NumberString.Split(',');
            return ((int.Parse(Numbers[4]) * 256) + int.Parse(Numbers[5]));
        }

        private List<string> ParseSecondaryBufferAsList() //splits secondary buffer line by line, for listing with list and nlst
        {
            List<string> result = new List<string>();
            lastMessageSecondary = readerSecondary.ReadLine();
            while (readerSecondary.Peek() >= 0)
            {
                result.Add(lastMessageSecondary);
                if (readerSecondary.EndOfStream)
                {
                    break;
                }
                lastMessageSecondary = readerSecondary.ReadLine();
            }
            result.Add(lastMessageSecondary);
            return result;
        }

        public void RefreshList() //refreshes current list and nlst lists in class' variables
        {
            Wlf("PASV");
            Wbtc();

            int portSecondary = CalculatePortByResponse(lastMessage);

            try
            {
                clientSecondary = new TcpClient();
                clientSecondary.Connect(_address, portSecondary);
                streamSecondary = clientSecondary.GetStream();
                writerSecondary = new StreamWriter(streamSecondary);
                readerSecondary = new StreamReader(streamSecondary);
            }
            catch
            {
                IsEverythingOk = false;
                return;
            }

            Wlf("LIST");
            Wbtc();

            pwl = ParseSecondaryBufferAsList();

            Wlf("PASV");
            Wbtc();
            Wbtc(); //transfer ok

            //now same thing with just items' names
            portSecondary = CalculatePortByResponse(lastMessage);

            try
            {
                clientSecondary = new TcpClient();
                clientSecondary.Connect(_address, portSecondary);
                streamSecondary = clientSecondary.GetStream();
                writerSecondary = new StreamWriter(streamSecondary);
                readerSecondary = new StreamReader(streamSecondary);
            }
            catch
            {
                IsEverythingOk = false;
                return;
            }

            Wlf("NLST");
            Wbtc();
            Wbtc();

            pwnl = ParseSecondaryBufferAsList();
        }

        public void ChangePwd(string newDir) //changes pwd, needs full path
        {
            Wlf("CWD " + newDir);
            Wbtc();
            RefreshPwd();
            RefreshList();
        }

        public void ChangePwdUp()
        {
            Wlf("CDUP");
            Wbtc();

            RefreshPwd();
            RefreshList();
        }
    }
}