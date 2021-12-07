using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace LoadScreenGen {
    public class Logger {
        public static void Log(string s) {
            Debug.WriteLine(s);
            Console.WriteLine(s);
        }
    }
}
