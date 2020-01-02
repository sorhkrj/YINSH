using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace YINSH
{
    public partial class MainForm : Form
    {
        #region 인스턴스
        readonly Game game = Game.GetInstance();
        readonly Turn turn = Turn.GetInstance();
        readonly Component component = Component.GetInstance();
        readonly Score score = Score.GetInstance();
        #endregion

        #region 변수
        readonly string[] version = { "Original", "Blitz" };
        string LastSavePath;

        bool GameStart()
        {
            var num = 0;
            for (var i = 0; i < turn.Player.Count; i++)
            {
                num += component.Ring_Quantity[i];
            }
            if (num != 10)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 함수
        public MainForm()
        {
            InitializeComponent();
            LoadGame();
        }

        private void TableLayoutPanel2_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            var panel = sender as TableLayoutPanel;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            var rectangle = e.CellBounds;
            using (var pen = new Pen(Color.Black, 1))
            {
                pen.Alignment = PenAlignment.Center;
                pen.DashStyle = DashStyle.Solid;

                if (e.Row == (panel.RowCount - 1))
                {
                    rectangle.Height--;
                }
                if (e.Column == (panel.ColumnCount - 1))
                {
                    rectangle.Width--;
                }
                e.Graphics.DrawRectangle(pen, rectangle);
            }
        }

        private void NewToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var box = MessageBox.Show("Do you want to start a new game?", "New Game", MessageBoxButtons.YesNo);
            if (GameStart())
            {
                if (box == DialogResult.Yes)
                {
                    NewGame();
                }
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Save();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            SaveAs();
        }

        private void BlitzToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var newversion = 0;
            if (BlitzToolStripMenuItem.Checked == true)
            {
                score.winscore = 1;
                newversion = 1;
            }
            if (BlitzToolStripMenuItem.Checked == false)
            {
                score.winscore = 3;
                newversion = 0;
            }
            var box = MessageBox.Show("Do you want to start a " + version[newversion] + " version?", version[newversion] + " version", MessageBoxButtons.YesNo);
            if (box == DialogResult.Yes)
            {
                var gamename = "YINSH";
                if (newversion == 1)
                {
                    gamename += $" - {version[newversion]}";
                }
                this.Text = gamename;
                NewGame();
            }
            else
            {
                BlitzToolStripMenuItem.Checked = !BlitzToolStripMenuItem.Checked;
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var box = MessageBox.Show("Do you want to close this game? ", "Exit", MessageBoxButtons.YesNo);
            if (box == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void HelpToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gipf.com/yinsh/index.html");
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F2)
            {
                var box = MessageBox.Show("Do you want to start a new game?", "New Game", MessageBoxButtons.YesNo);
                if (GameStart())
                {
                    if (box == DialogResult.Yes)
                    {
                        NewGame();
                    }
                    return;
                }
            }
            if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                Save();
            }
            if (e.KeyCode == Keys.S && e.Modifiers == (Keys.Control | Keys.Shift))
            {
                SaveAs();
            }
            if (e.KeyCode == Keys.Escape)
            {
                var box = MessageBox.Show("Do you want to close this game? ", "Exit", MessageBoxButtons.YesNo);
                if (box == DialogResult.Yes)
                {
                    this.Close();
                }
                return;
            }
        }

        string WriteNote()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (var i = 0; i < component.DataImage.Count; i++)
            {
                stringBuilder.AppendLine(component.DataImage[i]);
            }
            for (var i = 0; i < textBox1.Lines.Length; i++)
            {
                stringBuilder.AppendLine(textBox1.Lines[i]);
            }
            return stringBuilder.ToString();
        }

        void Save()
        {
            if (!string.IsNullOrEmpty(LastSavePath))
            {
                File.WriteAllText(LastSavePath, WriteNote(), Encoding.UTF8);
            }
            else
            {
                SaveAs();
            }
        }

        void SaveAs()
        {
            string folder = @"Notes";

            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            if (!dirInfo.Exists) dirInfo.Create();

            SaveFileDialog saveNote = new SaveFileDialog();
            saveNote.InitialDirectory = Application.StartupPath + folder;
            saveNote.Filter = "Note File (*.yinsh)|*.yinsh";
            saveNote.FileName = "*.yinsh";
            if (saveNote.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveNote.FileName, WriteNote(), Encoding.UTF8);
                LastSavePath = saveNote.FileName;
            }
        }

        void LoadGame()
        {
            panel.Controls.Add(game);
            LastSavePath = string.Empty;
            NewTextEvent();
            ResetTextEvent();
            game.Show();
        }

        void NewGame()
        {
            LastSavePath = string.Empty;
            game.Setting();
            ResetTextEvent();
            game.Board.Refresh();
        }

        void NewTextEvent()
        {
            component.RecodeText += new Component.SendRecodeText(RecodeText);
            game.RingText += new Game.SendRingText(RingText);
            game.MarkerText += new Game.SendMarkerText(MarkerText);
            game.ResultText += new Game.SendResultText(ResultText);
        }

        void ResetTextEvent()
        {
            textBox1.Text = string.Empty;
            RecodeText($"Note");
            RingText($"White {component.Ring_Quantity[0]}  Black {component.Ring_Quantity[1]}");
            MarkerText($"Marker\r\n{component.Marker_Quantity.ToString()}");
            ResultText(string.Empty);
        }

        void RecodeText(string text)
        {
            textBox1.Text += text;
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        void RingText(string text)
        {
            label4.Text = text;
        }

        void MarkerText(string text)
        {
            label2.Text = text;
        }

        void ResultText(string text)
        {
            if (game.End())
            {
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }
            label3.Text = text;
        }
        #endregion
    }
}
