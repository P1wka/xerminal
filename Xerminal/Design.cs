using System.Drawing;

namespace Xerminal
{
    public class Design
    {

        public static string title = "┌────────────────   xfetch   ────────────────┐";

        public static string line = "───────────────────────────────────────────";


        public static void printfunc(string text, string value, ConsoleTab console)
        {
            startline(console);
            console.SetForeColor(Color.LightGoldenrodYellow);
            console.AppendOutput($"{text,-20}");
            console.SetForeColor(Color.White);
            console.AppendOutput($": {value}");
        }

        public static void startline(ConsoleTab console)
        {
            console.SetForeColor(Color.DarkCyan);
            console.AppendOutput("│   ");
            console.SetForeColor(Color.White);
        }
    }
}
