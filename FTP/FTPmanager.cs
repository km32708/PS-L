using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.Security;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace FTP
{
    class FTPmanager
    {
        TcpClient client;
        NetworkStream stream;
        StreamReader reader;
        StreamWriter writer;

        TcpClient clientSecondary;
        NetworkStream streamSecondary;
        StreamReader readerSecondary;
        StreamWriter writerSecondary;

        string _address;

        public bool IsEverythingOk = true;
        public string pwd = " ";
        public List<string> pwl; //pwd list
        string lastMessage = " ";
        string lastMessageSecondary = " ";


        ~FTPmanager()
        {
            if (writer!=null)
            {
                writer.WriteLine("QUIT");
                Wbtc();
            }
        }
        public FTPmanager(string user, string password, string address, int port)
        {
            _address = address;

            client = new TcpClient();
            client.Connect(address, port);
            stream = client.GetStream();
            //stream.AuthenticateAsClient(address);
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            Wbtc();
            Wbtc();
            if (!IsOk(ReplyCode(lastMessage)))
            {
                IsEverythingOk = false;
                return;
            }

            Wlf("USER " + user);
            Wbtc();

            Wlf("PASS " + password);
            Wbtc();

            RefreshPwd();

            

            RefreshList();

        }
        void Wlf(string line) //write line and flush
        {
            Console.WriteLine("--> " + line);
            writer.WriteLine(line);
            writer.Flush();
        }
       
        void Wbtc() //write buffer to console
        {
            lastMessage = reader.ReadLine();
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

        void Wsbtc() //write secondary buffer to console
        {
            if (readerSecondary==null)
            {
                return;
            }
            lastMessageSecondary = readerSecondary.ReadLine();
            while (readerSecondary.Peek() >= 0)
            {
                Console.WriteLine("<2-- " + lastMessageSecondary);
                if (readerSecondary.EndOfStream)
                {
                    break;
                }
                lastMessageSecondary = readerSecondary.ReadLine();
            }
            Console.WriteLine("<2- " + lastMessageSecondary);
        }

        int ReplyCode(string responseLine)
        {
            return int.Parse(responseLine.Substring(0, 3));
        }
        bool IsOk(int Code)
        {
            return ((Code / 100) == 2);
        }
        public void RefreshPwd()
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
        int CalculatePortByResponse(string Response)
        {
            string NumberString = Response.Split('(', ')')[1];
            var Numbers = NumberString.Split(',');
            return ((int.Parse(Numbers[4]) * 256) + int.Parse(Numbers[5]));
        }
        List<string> ParseSecondaryBufferAsList()
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
        public void RefreshList()
        {
            Wlf("PASV");
            Wbtc();
            int portSecondary = CalculatePortByResponse(lastMessage);

            

            clientSecondary = new TcpClient();
            clientSecondary.Connect(_address, portSecondary);
            streamSecondary = clientSecondary.GetStream();
            //stream.AuthenticateAsClient(address);
            writerSecondary = new StreamWriter(streamSecondary);
            readerSecondary = new StreamReader(streamSecondary);

            //Wsbtc();

            Wlf("LIST");
            Wbtc();

            pwl = ParseSecondaryBufferAsList();


        }
        public void ChangePwd(string newDir) //full path
        {
            Wlf("CWD " + newDir);
            Wbtc();
            RefreshPwd();
        }
        public void ChangePwdUp()
        {
            Wlf("CDUP");
            Wbtc();

            RefreshPwd();
        }
    }
    
}
