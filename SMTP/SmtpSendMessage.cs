using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.Security;
using System.Net;
using System.IO;

namespace SMTP
{
    class SmtpSendMessage
    {
        TcpClient client;
        SslStream stream;
        StreamReader reader;
        StreamWriter writer;
        string tempstring = " ";
        //mapa uid -> wiadomość
        public SmtpSendMessage(string login, string pass, string url, int port, string from, string to, string subject, string message)
        {
            string[] messageLines = message.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            client = new TcpClient();
            client.Connect(url, port);
            stream = new SslStream(client.GetStream());
            stream.AuthenticateAsClient(url);
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            
            Console.WriteLine("SMTP INIT:\n\n");

            Wlf("EHLO");
            Console.WriteLine(reader.ReadLine());
            Wbtc();

            Console.WriteLine("SMTP LOGIN:\n\n");
            Wlf("AUTH LOGIN");
            Wbtc();
            Wlf(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(login)));
            Wbtc();
            Wlf(System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(pass)));
            Wbtc();

            Wlf("MAIL FROM:<" + from + ">");
            Wbtc();
            Wlf("RCPT TO:<" + to + ">");
            Wbtc();

            Wlf("DATA");
            Wbtc();

            Wlf("Date: " + DateTime.Now.ToString("ddd, dd MMM yyy HH:mm:ss", new System.Globalization.CultureInfo("en-US")));
            Wlf("From: " + from);
            Wlf("Subject: " + subject);
            Wlf("To: " + to);
            Wlf("");
            
            foreach (string messageLine in messageLines)
            {
                Wlf(messageLine);
            }

            Wlf(".");
            Wbtc();
            Console.WriteLine("SMTP send message finished");
        }
        ~SmtpSendMessage()
        {
            writer.WriteLine("QUIT");
        }

        //write line and flush
        void Wlf(string line)
        {
            Console.WriteLine("--> " + line);
            writer.WriteLine(line);
            writer.Flush();
        }
        int ReplyCode(string responseLine)
        {
            return int.Parse(responseLine.Substring(0, 3));
        }

        //write buffer to console
        void Wbtc()
        {
            tempstring = reader.ReadLine();
            while (reader.Peek() >= 0)
            {
                Console.WriteLine("<-- " + tempstring);
                if (reader.EndOfStream)
                {
                    break;
                }
                tempstring = reader.ReadLine();
            }
            Console.WriteLine("<-- " + tempstring);
        }
    }
}