using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKonsulatConsole
{
    class Program
    {
        private static readonly IdValidator IdValidator = new IdValidator();
        private static readonly Helper Helper = new Helper();
        private static int tries = 0;
        static void Main(string[] args)
        {

            Helper.Log(ConsoleColor.Cyan, "Please enter city number: ");
            var idc = Console.ReadLine();
            Helper.Log(ConsoleColor.Cyan, "Please enter service or visa type id: ");
            var ids = Console.ReadLine();
            Helper.Log(ConsoleColor.Green, "Please enter applicant id: ");
            var appId = Console.ReadLine();

            if (idc == "83")
            {
                Helper.Log(ConsoleColor.Green, "Please enter visa for id: ");
                var visaFor = Console.ReadLine();
                LvivWorker.visaForLuck = visaFor;
            }

            if (IdValidator.ValidateArgsCity(idc) && IdValidator.ValidateArgsService(ids))
            {
                //Console.Title = "[RUN] E-Konsulat Visa Search with params!";
                LvivWorker driveWorker = new LvivWorker(ids, idc, appId);
                while (driveWorker.IsDone == false)
                {
                    tries++;
                    Console.Title = $"[{tries}][RUN] E-Konsulat Visa Search with params!";
                    driveWorker.DoJob();
                }
            }
            else
            {
                Helper.Log(ConsoleColor.Cyan, "[ERROR] Entered city or visa type is wrong!");
                Console.ReadLine();
            }
            //Console.ReadLine();
        }
    }
}
