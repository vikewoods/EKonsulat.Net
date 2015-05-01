using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKonsulatConsole
{
    public class Helper
    {
        public void Log(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write("[" + DateTime.Now.ToLocalTime() + "] " + text + Environment.NewLine);
            Console.ForegroundColor = originalColor;
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
