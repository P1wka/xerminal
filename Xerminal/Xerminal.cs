using System;
using System.Drawing;
using System.Windows.Forms;

namespace Xerminal
{
    public partial class Xerminal : Form
    {
        public Xerminal()
        {
            InitializeComponent();
        }

        private void Xerminal_Load(object sender, EventArgs e)
        {
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.Padding = new Point(22, 5);

            tabControl.Paint += TabControl_Paint;

            TabPage plusTab = new TabPage();
            plusTab.Text = string.Empty;
            tabControl.TabPages.Add(plusTab);

            AddNewTab();
        }

        private void TabControl_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = tabControl.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            using (Pen pen = new Pen(Color.Black, 2))
            {
                e.Graphics.DrawRectangle(pen, rect);
            }
        }

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                TabPage tabPage = tabControl.TabPages[e.Index];
                Rectangle tabRect = tabControl.GetTabRect(e.Index);
                bool isLast = (e.Index == tabControl.TabCount - 1);

                string title = isLast ? "   +" : "   " + tabPage.Text;

                using (SolidBrush brush = new SolidBrush(Color.BlanchedAlmond))
                {
                    e.Graphics.FillRectangle(brush, tabRect);
                }

                TextRenderer.DrawText(e.Graphics, title, this.Font, tabRect, Color.Black);

                if (!isLast)
                {
                    Rectangle closeRect = new Rectangle(tabRect.Right - 15, tabRect.Top + 4, 10, 10);
                    e.Graphics.DrawString("x", this.Font, Brushes.Red, closeRect);
                }
            }
            catch { }
        }

        private void tabControl_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                Rectangle tabRect = tabControl.GetTabRect(i);
                bool isLast = (i == tabControl.TabCount - 1);
                Rectangle closeRect = new Rectangle(tabRect.Right - 15, tabRect.Top + 4, 10, 10);

                if (isLast && tabRect.Contains(e.Location))
                {
                    AddNewTab();
                    return;
                }

                if (!isLast && closeRect.Contains(e.Location))
                {
                    tabControl.TabPages.RemoveAt(i);

                    if (tabControl.TabPages.Count == 1)
                    {
                        Application.Exit();
                    }

                    return;
                }
            }
        }

        private void AddNewTab()
        {
            var control = new ConsoleTab();
            control.Dock = DockStyle.Fill;

            string title = "Tab " + (tabControl.TabCount);

            TabPage tab = new TabPage(title);
            tab.Controls.Add(control);

            int insertIndex = Math.Max(0, tabControl.TabCount - 1);
            tabControl.TabPages.Insert(insertIndex, tab);
            tabControl.SelectedTab = tab;
        }

    }
}
