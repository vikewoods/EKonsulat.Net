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

        static void Main(string[] args)
        {

            Helper.Log(ConsoleColor.Cyan, "Please enter city number: ");
            var idc = Console.ReadLine();
            Helper.Log(ConsoleColor.Cyan, "Please enter service or visa type id: ");
            var ids = Console.ReadLine();

            if (IdValidator.ValidateArgsCity(idc) && IdValidator.ValidateArgsService(ids))
            {
                LvivWorker driveWorker = new LvivWorker(ids, idc);
                while (driveWorker.IsDone == false)
                {
                    driveWorker.DoJob();
                }
            }
            else
            {
                Helper.Log(ConsoleColor.Cyan, "[ERROR] Entered city or visa type is wrong!");
                Console.ReadLine();
            }
            Console.ReadLine();
        }
    }
}
