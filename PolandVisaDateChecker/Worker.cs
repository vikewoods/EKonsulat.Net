using System;
using System.IO;
using System.Net.Mail;
using System.Threading;
using SimpleBrowser;

namespace PolandVisaDateChecker
{
    public class Worker
    {
        private const string Url = "https://www.vfsvisaonline.com/poland-ukraine-appointment/(S(4mjecdqtsat0ww55ddgeaf45))/AppScheduling/AppWelcome.aspx?P=s2x6znRcBRv7WQQK7h4MTjZiPRbOsXKqJzddYBh3qCA=";
        public bool FoundedNewDates { get; set; }

        public Worker()
        {
            //Console.SetBufferSize(Console.WindowWidth, 120);
        }

        public bool DoWork(string city, string visatype)
        {
            var browser = new Browser();
            FoundedNewDates = false;
            Random rnd = new Random();
            //StreamWriter streamWriter = new StreamWriter(@"Logs.txt");

            // All types of visa
            // Ivano Frankivsk, Vinica, Xmelnuckij, Zhutomur, Chernivci (except Donck, Lugansk)
            // Except dates with given arguments


            try
            {
                ColoredConsoleWrite(ConsoleColor.Cyan, "[INFO]: Connecting to https://www.vfsvisaonline.com/poland-ukraine-appointment/");
                Console.Title = "[RUN] Poland visa checker";

                browser.RequestLogged += OnBrowserRequestLogged;
                browser.MessageLogged += new Action<Browser, string>(OnBrowserMessageLogged);
                browser.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.10 (KHTML, like Gecko) Chrome/8.0.552.224 Safari/534.10";
                
                Random proxyRandom = new Random();
                var lines = File.ReadAllLines(@"Proxy.txt");
                var selectRandomProxy = proxyRandom.Next(0, lines.Length);
                browser.SetProxy(lines[selectRandomProxy]);
                ColoredConsoleWrite(ConsoleColor.Cyan, "[INFO]: Setting proxy to: " + lines[selectRandomProxy]);
                //streamWriter.WriteLine("{ " + DateTime.Now.ToLocalTime() + " } [INFO]: Setting proxy to: " + lines[selectRandomProxy]);

                browser.Navigate(Url);

                if (LastRequestFailed(browser)) { ColoredConsoleWrite(ConsoleColor.Red, "[ERROR]: Cannot connect to proxy server! Trying to get new connection!"); return FoundedNewDates = false; }

                var firstBtnClick = browser.Find("ctl00_plhMain_lnkChkAppmntAvailability");

                if (firstBtnClick.Exists)
                {
                    ColoredConsoleWrite(ConsoleColor.Cyan, "[INFO]: Founded first link to check!");
                    //streamWriter.WriteLine("{ " + DateTime.Now.ToLocalTime() + " } [INFO]: Founded first link to check!");
                    firstBtnClick.Click();

                    if (LastRequestFailed(browser)) { ColoredConsoleWrite(ConsoleColor.Red, "[ERROR]: Cannot connect to STEP 1"); return FoundedNewDates = false; }

                    //ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Lviv");

                    if (city.Equals("7"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Lviv");
                    }else if (city.Equals("5"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Ivano Frankovsk");
                    }else if (city.Equals("8"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Ternopil");
                    }else if (city.Equals("9"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Rivne");
                    }else if (city.Equals("10"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Luck");
                    }else if (city.Equals("11"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Dnipropetrovsk");
                    }else if (city.Equals("12"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Harkiv");
                    }else if (city.Equals("13"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Kyiv");
                    }else if (city.Equals("14"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Odesa");
                    }else if (city.Equals("15"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Xmelnuck");
                    }else if (city.Equals("16"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Zhutomur");
                    }
                    else if (city.Equals("17"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Vinica");
                    }
                    else if (city.Equals("20"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Uzhorod");
                    }
                    else if (city.Equals("21"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Poland - Chernivci");
                    }
                    

                    //streamWriter.WriteLine("{ " + DateTime.Now.ToLocalTime() + " } [SEARCH]: Poland - Lviv");
                    browser.Find("ctl00_plhMain_cboVAC").Value = city;
                    browser.Find("ctl00_plhMain_btnSubmit").Click();
                    
                    Thread.Sleep(5000);
                    if (LastRequestFailed(browser)) { ColoredConsoleWrite(ConsoleColor.Red, "[ERROR]: Cannot connect to STEP 2"); return FoundedNewDates = false; }


                    //ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Shengen");
                        //streamWriter.WriteLine("{ " + DateTime.Now.ToLocalTime() + " } [SEARCH]: Shengen");

                    if (visatype.Equals("235"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: National visa");
                    }
                    else if (visatype.Equals("229"))
                    {
                        ColoredConsoleWrite(ConsoleColor.Magenta, "[SEARCH]: Shengen visa");
                    }

                        browser.Find("ctl00_plhMain_cboVisaCategory").Value = visatype;
                        browser.Find("ctl00_plhMain_btnSubmit").Click();


                        if (LastRequestFailed(browser)) { ColoredConsoleWrite(ConsoleColor.Red, "[ERROR]: Cannot connect to STEP 3"); return FoundedNewDates = false; }

                        var noDatesInput = browser.Find("span", FindBy.Class, "Success");

                    //Console.WriteLine(browser.CurrentHtml);

                    if (!noDatesInput.Exists)
                    {
                        //streamWriter.WriteLine("{ " + DateTime.Now.ToLocalTime() + " } [ATENTION]: No avaliable dates for now! Trying again!");
                        ColoredConsoleWrite(ConsoleColor.Red, "[ATENTION]: No avaliable dates for now! Trying again!");
                        FoundedNewDates = false;
                    }
                    else
                    {
                        //streamWriter.WriteLine("{ " + DateTime.Now.ToLocalTime() + " } [ATENTION]: New dates are AVALIABLE!!! Press ENTER to continue!");
                        ColoredConsoleWrite(ConsoleColor.Green, "[ATENTION]: New dates are AVALIABLE!!! Press ENTER to continue!");
                        DoSendReportToEmail(city, visatype);
                        FoundedNewDates = true;
                        Console.Title = "[ATENTION] Poland visa checker";
                        Console.Beep();
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception exception)
            {
                //streamWriter.WriteLine("{ " + DateTime.Now.ToLocalTime() + " } [ERROR]: Unknown error " + exception.Message);
                ColoredConsoleWrite(ConsoleColor.Red, "[ERROR]: Unknown error " + exception.Message);
                throw;
            }

            
            var timeout = rnd.Next(500, 5000);
            Thread.Sleep(timeout);
            //streamWriter.WriteLine("{ " + DateTime.Now.ToLocalTime() + " } [INFO]: Waiting time " + timeout / 1000);
            ColoredConsoleWrite(ConsoleColor.Cyan, "[INFO]: Waiting time " + timeout/1000);
            //streamWriter.Close();
            return true;
        }

        private void DoSendReportToEmail(string city, string visatype)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("vik.ewoods@gmail.com");
            mail.To.Add("tandemin@mail.ua");
            mail.Subject = "[Визовый Центр] Доступны новый даты";
            mail.Body = "На сайте визового центра доступны новые даты. \n";
            mail.Body += String.Format("Номер город: {0} и тип визы {1}", city, visatype);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("vik.ewoods@gmail.com", "Lirika159357*0");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

        }

        static bool LastRequestFailed(Browser browser)
        {
            if (browser.LastWebException != null)
            {
                browser.Log("There was an error loading the page: " + browser.LastWebException.Message);
                return true;
            }
            return false;
        }

        static void OnBrowserMessageLogged(Browser browser, string log)
        {
            //Console.WriteLine(log);
        }

        static void OnBrowserRequestLogged(Browser req, HttpRequestLog log)
        {
            ColoredConsoleWrite(ConsoleColor.Yellow, "[REQUEST]: " + log.Method + " request to https://www.vfsvisaonline.com/poland-ukraine-appointment/");
            ColoredConsoleWrite(ConsoleColor.Yellow, "[REQUEST]: Response status code: " + log.ResponseCode);
        }

        static string WriteFile(string filename, string text)
        {
            var dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"));
            if (!dir.Exists) dir.Create();
            var path = Path.Combine(dir.FullName, filename);
            File.WriteAllText(path, text);
            return path;
        }

        public static void ColoredConsoleWrite(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write("{ " + DateTime.Now.ToLocalTime() + " } " + text + Environment.NewLine);
            Console.ForegroundColor = originalColor;
        }

        

    }
}
