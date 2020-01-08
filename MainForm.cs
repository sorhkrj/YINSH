using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Diagnostics;
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
        int newversion = 0;
        string LastSavePath;
        readonly string folder = @"Notes";
        bool saveFile;
        bool changeFile;

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

        #region 메뉴
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

        private void OpenFolderToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var path = $@"{Application.StartupPath}\{folder}";
            Process.Start(path);
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
            ChangeMode();
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
            Process.Start("http://www.gipf.com/yinsh/index.html");
        }
        #endregion

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
            if (e.KeyCode == Keys.F && e.Modifiers == Keys.Alt)
            {
                SaveAs();
            }
            if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                Save();
            }
            if (e.KeyCode == Keys.S && e.Modifiers == (Keys.Control | Keys.Shift))
            {
                SaveAs();
            }
            if (e.KeyCode == Keys.M && e.Modifiers == Keys.Alt)
            {
                ChangeMode();
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
                changeFile = false;
                this.Text = this.Text.Substring(0, this.Text.Length - 1);
            }
            else
            {
                SaveAs();
            }
        }

        void SaveAs()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            if (!dirInfo.Exists) dirInfo.Create();

            SaveFileDialog saveNote = new SaveFileDialog
            {
                InitialDirectory = $@"{Application.StartupPath}\{folder}",
                Filter = "Note File (*.txt)|*.txt|All File (*.*)|*.*",
                FileName = "*.txt"
            };
            if (saveNote.ShowDialog() == DialogResult.OK)
            {
                var path = saveNote.FileName;
                File.WriteAllText(path, WriteNote(), Encoding.UTF8);
                LastSavePath = path;
                this.Text += $" {saveNote.FileName}";
                saveFile = true;
            }
        }

        void ChangeMode()
        {
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
            game.Board.Invalidate();
        }

        #region Text
        void NewTextEvent()
        {
            game.ChangeFile += new Game.CheckChangeFile(ChangeFile);
            component.RecodeText += new Component.SendRecodeText(RecodeText);
            game.RingText += new Game.SendRingText(RingText);
            game.MarkerText += new Game.SendMarkerText(MarkerText);
            game.ResultText += new Game.SendResultText(ResultText);
        }

        void ResetTextEvent()
        {
            saveFile = false;
            changeFile = false;
            textBox1.Text = string.Empty;
            RecodeText($"{version[newversion]} Note");
            RingText($"Turn {game.player[turn.User]}");
            MarkerText($"Marker\r\n{component.Marker_Quantity.ToString()}");
            ResultText(string.Empty);
        }

        void ChangeFile()
        {
            if (saveFile)
            {
                if (!changeFile)
                {
                    this.Text += "*";
                    changeFile = true;
                }
            }
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
                RecodeText($"\r\n{text}");
            }
            label3.Text = text;
        }
        #endregion
        #endregion
    }
}
