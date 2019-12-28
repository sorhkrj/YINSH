using System.Drawing;
using System.Windows.Forms;

namespace YINSH
{
    public partial class GAME : Form
    {
        #region 인스턴스
        readonly Map map = Map.GetInstance();
        readonly Turn turn = Turn.GetInstance();
        readonly Component component = Component.GetInstance();
        readonly Score score = Score.GetInstance();
        #endregion

        #region 변수
        readonly string[] version = { "Original", "Blitz" };

        bool Version(string mode, int winscore)
        {
            if (comboBox1.Items[comboBox1.SelectedIndex].ToString() == mode)
            {
                for (var i = 0; i < score.Player.Length; i++)
                {
                    if (score.Player[i] == winscore)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool Start;

        bool End()
        {
            if (Version("Original", 3)) { return true; }
            if (Version("Blitz", 1)) { return true; }
            return false;
        }
        #endregion

        #region 함수
        void Setting()
        {
            comboBox1.Items.AddRange(version);
            comboBox1.SelectedIndex = 0;
            Start = false;

            Size size = panel.Size;
            int length = (Map.Size * 2) + 1;

            // Map: Board, Point Set & Draw Map;
            // Turn: Clear
            // Component: Layer, Ring, Marker, Cursor;
            // Score: Layer, End;
            map.System(size, length);
            turn.Setting();
            component.Setting(size, length);
            score.Setting(size);
        }

        /// <summary>
        /// Test Label
        /// </summary>
        void TextTest()
        {
            string ready = (turn.Count == 0) ? "[Ready]" : "[Play]";
            string Ring = string.Empty;
            string Marker = "Marker: " + component.Marker_Quantity;
            string Turn = (turn.Count == 0) ? string.Empty : " (Turn: " + turn.Count + ")";
            if (ready == "[Ready]")
            {
                Ring = "\r\nWhite Ring: " + component.Ring_Quantity[0] + "\r\nBlack Ring: " + component.Ring_Quantity[1];
            }
            else
            {
                if (turn.Player[turn.User] == Color.White)
                {
                    Ring = " White ";
                }
                if (turn.Player[turn.User] == Color.Black)
                {
                    Ring = " Black ";
                }
            }
            label2.Text = ready + Marker + Ring + Turn;
            if (End())
            {
                label2.Text = "End";
            }
        }

        void Image()
        {
            using (Graphics g = panel.CreateGraphics())
            {
                g.DrawImage(map.Board, Point.Empty);
                g.DrawImage(component.Layer, Point.Empty);
                g.DrawImage(score.Layer, Point.Empty);
            }
        }

        void Rule()
        {
            component.System();
            turn.System();
            score.System();

            this.Refresh();
        }
        #endregion
    }
}
