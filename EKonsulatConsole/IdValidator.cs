using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKonsulatConsole
{
    public class IdValidator
    {
        private const string LvivCityId = "92";
        private const string VinicaCityId = "0";

        private const string TestCityId = "149";
        private const string TestServiceId = "8";

        private const string ShengenVisaTypeId = "1";

        public bool ValidateArgsService(string args)
        {
            if (args == ShengenVisaTypeId || args == TestServiceId)
            {
                return true;
            }
            return false;
        }

        public bool ValidateArgsCity(string args)
        {
            if (args == LvivCityId || args == VinicaCityId || args == TestCityId || args == "83")
            {
                return true;
            }
            return false;
        }
    }
}
