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

        public static void LogTime(string s, TimeSpan time) {
            Log(s + " took: " + ((int)(time.TotalSeconds * 10)) / 10.0 + "s");
        }
    }
}
