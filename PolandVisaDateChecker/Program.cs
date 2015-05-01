using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolandVisaDateChecker
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Title = "Poland visa checker";

            Worker worker = new Worker();

            Console.Write("Please enter city number: ");
            var city = Console.ReadLine();
            Console.Write("Please enter visa: ");
            var vizatype = Console.ReadLine();

            while (!worker.FoundedNewDates)
            {
                worker.DoWork(city, vizatype);
            }
        }
    }
}
