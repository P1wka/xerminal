using System;
using System.Drawing;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

namespace Xerminal
{
    public class Fetch
    {
        static ConsoleTab tab;
        public static void Run(ConsoleTab console)
        {
            tab = console;

            console.SetTitle("xfetch");
            console.SetForeColor(Color.Cyan);
            console.AppendOutput(Design.title + "\n");
            console.SetForeColor(Color.White);
            GetUserInformation();
            GetOSInformation();
            GetRamUsage();
            PrintAscii();
        }

        public static void GetUserInformation()
        {

            Design.startline(tab);
            tab.SetForeColor(Color.LightGoldenrodYellow);
            tab.AppendOutput(Environment.UserName + "'s PC" + "\n");
            Design.startline(tab);
            tab.AppendOutput(Design.line + "\n");

        }

        public static void GetOSInformation()
        {
            Design.printfunc("OS", $"{RuntimeInformation.OSDescription}" + "\n", tab);
            Design.printfunc("Architecture", $"{RuntimeInformation.OSArchitecture}" + "\n", tab);
            Design.printfunc("Process Architecture", $"{RuntimeInformation.ProcessArchitecture}" + "\n", tab);
            Version version = Environment.OSVersion.Version;
            Design.printfunc("Major", $"{version.Major}" + "\n", tab);
            Design.printfunc("Minor", $"{version.Minor}" + "\n", tab);
        }

        public static void GetRamUsage()
        {
            var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (var ram in searcher.Get())
            {
                ulong total = Convert.ToUInt64(ram["TotalVisibleMemorySize"]);
                ulong free = Convert.ToUInt64(ram["FreePhysicalMemory"]);
                Design.printfunc("RAM", $"{(total / 1024) - (free / 1024)} MiB / {total / 1024} MiB" + "\n", tab);
                PrintRamBar(total, free);
            }

        }

        public static void PrintRamBar(double t, double f)
        {
            int free = (int)((f / t) * 10);
            Design.startline(tab);
            tab.AppendOutput("[");
            for (int i = 0; i < (10 - free) * 2; i++)
            {
                tab.AppendOutput("█");
            }
            for (int i = 0; i < free * 2; i++)
            {
                tab.AppendOutput("-");
            }
            tab.AppendOutput("]");
            tab.AppendOutput("\n");
        }

        public static void PrintAscii()
        {
            Design.startline(tab);
            tab.AppendOutput("\n");
            string path = @"ascii\win.txt";
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() >= 0)
                {
                    Design.startline(tab);
                    tab.SetForeColor(Color.DarkBlue);
                    tab.AppendOutput(sr.ReadLine() + "\n");
                }
            }
            tab.SetForeColor(Color.White);
            tab.AppendOutput("\n");
        }
    }
}
