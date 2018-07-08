using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoteriaEngine
{
    public static class Debug
    {
        public static void Log(string message) {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + ": " + message);
        }
    }
}
