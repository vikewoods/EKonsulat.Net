using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EKonsulatConsole
{
    public class Helper
    {

        public bool CanPing(string address)
        {
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send(address, 2500);
                return reply?.Status == IPStatus.Success;
            }
            catch (PingException e)
            {
                Debug.Write("Ping debug line:" + e.Message);
                return false;
            }
        }

        public void Log(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write("[" + DateTime.Now.ToLocalTime() + "] " + text + Environment.NewLine);
            Console.ForegroundColor = originalColor;
        }

        public CookieCollection GetAllCookies(CookieContainer cookieJar)
        {
            CookieCollection cookieCollection = new CookieCollection();

            Hashtable table = (Hashtable)cookieJar.GetType().InvokeMember("m_domainTable",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.GetField |
                                                                            BindingFlags.Instance,
                                                                            null,
                                                                            cookieJar,
                                                                            new object[] { });

            foreach (var tableKey in table.Keys)
            {
                String str_tableKey = (string)tableKey;

                if (str_tableKey[0] == '.')
                {
                    str_tableKey = str_tableKey.Substring(1);
                }

                SortedList list = (SortedList)table[tableKey].GetType().InvokeMember("m_list",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.GetField |
                                                                            BindingFlags.Instance,
                                                                            null,
                                                                            table[tableKey],
                                                                            new object[] { });

                foreach (var listKey in list.Keys)
                {
                    String url = "https://" + str_tableKey + (string)listKey;
                    cookieCollection.Add(cookieJar.GetCookies(new Uri(url)));
                }
            }

            return cookieCollection;
        }

        public void Beep()
        {
            Console.Beep(520, 215);
            Console.Beep(520, 265);
            Console.Beep(520, 265);
            Console.Beep(520, 215);
            Console.Beep(560, 215);
            Console.Beep(520, 215);

            Console.Beep(520, 215);
            Console.Beep(520, 265);
            Console.Beep(520, 265);
            Console.Beep(520, 215);
            Console.Beep(560, 215);
            Console.Beep(520, 215);

            Console.Beep(520, 215);
            Console.Beep(520, 265);
            Console.Beep(520, 265);
            Console.Beep(520, 215);
            Console.Beep(560, 215);
            Console.Beep(520, 215);

            Console.Beep(520, 215);
            Console.Beep(520, 265);
            Console.Beep(520, 265);
            Console.Beep(520, 215);
            Console.Beep(560, 215);
            Console.Beep(520, 215);
        }
    }
}
