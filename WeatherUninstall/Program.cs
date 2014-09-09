using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherUninstall
{
    class Program
    {
        static void Main(string[] args)
        {
            string sysroot = System.Environment.SystemDirectory;
            System.Diagnostics.Process.Start(sysroot + "\\msiexec.exe", "/x {B464E326-2C89-4AC5-A358-1D0ABED0F2AE} /qr");
        }
    }
}
