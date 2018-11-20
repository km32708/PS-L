using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.Security;
using System.Net;
using System.IO;

namespace POP3
{
    class Message
    {
        string _uid;
        string _subject;
        string _retrieveableId;
        bool _isNew;
        public string Subject { get => _subject; set => _subject = value; }
        public string Uid { get => _uid; set => _uid = value; }
        public string RetrieveableId { get => _retrieveableId; set => _retrieveableId = value; }
        public bool IsNew { get => _isNew; set => _isNew = value; }

        public Message(string uid, string subject, string retrieveableId)
        {
            this._uid = uid;
            this._subject = subject;
            this._retrieveableId = retrieveableId;
            _isNew = true;
        }
    }

    class Pop3Client
    {
        string _user, _pass, _url;
        int _port;
        TcpClient client;
        SslStream stream;
        StreamReader reader;
        StreamWriter writer;
        string tempstring = " ";
        //mapa uid -> wiadomość
        public Dictionary<string, Message> messages = new Dictionary<string, Message>();
        public Pop3Client(string user, string pass, string url, int port)
        {
            _user = user;
            _pass = pass;
            _url = url;
            _port = port;

            client = new TcpClient();
            client.Connect(url, port);
            stream = new SslStream(client.GetStream());
            stream.AuthenticateAsClient(url);
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            Console.WriteLine("POP3 INIT:\n\n");
            writer.WriteLine("USER " + user);
            writer.Flush();

            writer.WriteLine("PASS " + pass);
            writer.Flush();

            writer.WriteLine("LIST");
            writer.Flush();

            while ((tempstring = reader.ReadLine()) != null)
            {
                Console.WriteLine(tempstring);
                if (tempstring == "." || tempstring.IndexOf("-ERR") != -1)
                {
                    break;
                }
            }

        }
        ~Pop3Client()
        {
            writer.WriteLine("QUIT");
            //writer.Flush();
        }

        public void updateMessagesList()
        {
            Console.WriteLine("GETTING MESSAGES LIST:\n\n");
            string[] tempstringarray;

            writer.WriteLine("UIDL");
            writer.Flush();
            while ((tempstring = reader.ReadLine()) != null)
            {
                Console.WriteLine(tempstring);
                if (tempstring == "." || tempstring.IndexOf("-ERR")!=-1)
                {
                    break;
                }
                if (tempstring.IndexOf("+")<0)
                {
                    tempstringarray = tempstring.Split(' ');
                    if (!messages.ContainsKey(tempstringarray[1]))
                    {
                        messages.Add(tempstringarray[1], new Message(tempstringarray[1], "", tempstringarray[0]));
                    }
                    else
                    {
                        messages[tempstringarray[1]].IsNew = false;
                    }
                }
            }
        }
        public void updateMessagesSubjects()
        {
            Console.WriteLine("GETTING MESSAGES SUBJECTS:\n\n");
            foreach (string key in messages.Keys)
            {
                writer.WriteLine("RETR " + messages[key].RetrieveableId.ToString());
                writer.Flush();

                while ((tempstring = reader.ReadLine()) != null)
                {
                    //Console.WriteLine(tempstring);
                    if (tempstring == "." || tempstring.IndexOf("-ERR") != -1)
                    {
                        break;
                    }
                    if (tempstring.IndexOf("Subject:")==0)
                    {
                        tempstring = tempstring.Remove(0, 9);

                        if (tempstring[0]=='=' && tempstring[8]=='Q') //wypadek quoted printable
                        {
                            tempstring = tempstring.Remove(0, 10);
                            tempstring = tempstring.Remove(tempstring.Length - 2, 2);
                            tempstring = tempstring.Replace('_', ' ');
                        }
                        messages[key].Subject = tempstring;
                        Console.WriteLine(tempstring);
                    }
                }
            }
        }
        public bool ShouldNotify()
        {
            bool result = false;
            foreach (string key in messages.Keys)
            {
                if (messages[key].IsNew == true)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void Reconnect()
        {
            client.GetStream().Close();
            client.Close();
            writer.WriteLine("QUIT");

            //writer.Flush();

            client = new TcpClient();
            client.Connect(_url, _port);
            stream = new SslStream(client.GetStream());
            stream.AuthenticateAsClient(_url);
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            writer.WriteLine("USER " + _user);
            writer.Flush();

            writer.WriteLine("PASS " + _pass);
            writer.Flush();

            writer.WriteLine("LIST");
            writer.Flush();
        }

        public void RefreshAll()
        {
            Reconnect();
            updateMessagesList();
            updateMessagesSubjects();
        }
    }
}