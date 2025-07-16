using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Xerminal
{
    public partial class ConsoleTab : UserControl
    {
        private int promptIndex = 0;

        private bool isMaximized = false;
        private Rectangle previousBounds;

        public ConsoleTab()
        {
            InitializeComponent();
            txtConsole.AppendText("Hello! Welcome to Xerminal." + "\n");
            this.Text = Path.GetDirectoryName(Application.ExecutablePath);
            AppendPrompt();
        }

        private void txtConsole_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                string fullText = txtConsole.Text;
                string command = fullText.Substring(promptIndex).Trim();

                AppendOutput("\n");
                AppendOutput(RunCommand(command));
                AppendPrompt();
            }

            else if (e.KeyCode == Keys.Back && txtConsole.SelectionStart <= promptIndex)
            {
                e.SuppressKeyPress = true;
            }

            else if (e.KeyCode == Keys.Left && txtConsole.SelectionStart <= promptIndex)
            {
                e.SuppressKeyPress = true;
            }
        }


        private void AppendPrompt()
        {
            SetForeColor(Color.Red);
            AppendOutput($"\n{Environment.UserName.ToLowerInvariant()}@{Environment.UserDomainName.ToLowerInvariant()} ~ ");
            SetForeColor(Color.White);
            promptIndex = txtConsole.TextLength;
            txtConsole.SelectionStart = txtConsole.TextLength;
            txtConsole.ScrollToCaret();
        }

        public void AppendOutput(string text)
        {
            txtConsole.AppendText(text);
            txtConsole.SelectionStart = txtConsole.TextLength;
            txtConsole.ScrollToCaret();
        }


        private string RunCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return "";

            if (command == "cls")
            {
                txtConsole.Clear();
                return "";
            }
            else if (command == "xfetch")
            {
                Fetch.Run(this);
                return "";
            }
            else
            {
                if (command.StartsWith("wsl"))
                {
                    if (command == "wsl")
                    {
                        Process.Start(new ProcessStartInfo("wsl.exe")
                        {
                            UseShellExecute = true
                        });
                        return "";
                    }
                    else
                    {
                        string wslCommand = command.Substring(4);
                        return RunWslCommand(wslCommand);
                    }
                }
                else
                {
                    return RunCmdCommand(command);
                }
            }

        }

        private string RunCmdCommand(string command)
        {
            try
            {
                var psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(psi);
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return string.IsNullOrWhiteSpace(error) ? output : error;
            }
            catch (Exception ex)
            {
                return $"COMMAND EXCEPTION: {ex.Message}";
            }
        }

        private string RunWslCommand(string command)
        {
            try
            {
                var psi = new ProcessStartInfo("wsl.exe", $"-- {command}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new Process() { StartInfo = psi, EnableRaisingEvents = true };

                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                process.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        outputBuilder.AppendLine(e.Data);
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        errorBuilder.AppendLine(e.Data);
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                var error = errorBuilder.ToString();
                var output = outputBuilder.ToString();

                return string.IsNullOrWhiteSpace(error) ? output : error;
            }
            catch (Exception ex)
            {
                return $"WSL EXCEPTION: {ex.Message}";
            }
        }



        private void txtConsole_SelectionChanged(object sender, EventArgs e)
        {
            if (txtConsole.SelectionStart < promptIndex)
            {
                txtConsole.SelectionStart = txtConsole.TextLength;
            }
        }

        public void SetTitle(string title)
        {
            this.Text = title;
        }

        private void titlePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public void SetForeColor(Color color)
        {
            txtConsole.SelectionColor = color;
        }

        public void SetBackColor(Color color)
        {
            txtConsole.SelectionBackColor = color;
        }
    }
}
