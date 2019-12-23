using System.Drawing;
using System.Windows.Forms;

namespace YINSH
{
    public partial class GAME : Form
    {
        #region 인스턴스
        readonly Map map = Map.GetInstance;
        readonly Component component = Component.GetInstance;
        readonly Turn turn = Turn.GetInstance;
        #endregion

        #region 함수
        void Setting()
        {
            Size size = panel.Size;
            int length = (Map.Size * 2) + 1;

            // Map: Board, Point Set & Draw Map;
            // Turn: Clear
            // Component: Layer, Ring, Marker, Cursor;
            map.System(size, length);
            turn.Setting();
            component.Setting(size, length);
        }

        /// <summary>
        /// Test Label
        /// </summary>
        void TextTest()
        {
            string ready = (turn.Count == 0) ? "Ready" : "Turn: " + turn.Count;
            string Ring = string.Empty;
            string Marker = "Marker: " + component.Marker_Quantity;
            if (ready == "Ready")
            {
                Ring = "White Ring: " + component.Ring_Quantity[0] + ", Black Ring: " + component.Ring_Quantity[1];
            }
            else
            {
                if (turn.Player[turn.User] == Color.White)
                {
                    Ring = "White Ring";
                }
                if (turn.Player[turn.User] == Color.Black)
                {
                    Ring = "Black Ring";
                }
            }
            label2.Text = Ring + ", " + Marker + ", " + ready;
        }

        void Image()
        {
            using (Graphics g = panel.CreateGraphics())
            {
                g.DrawImage(map.Board, new Point(0, 0));
                g.DrawImage(component.Layer, new Point(0, 0));
            }
        }

        /// <summary>
        /// 규칙 컴포넌트 설치 후 그림 띄어주고 턴
        /// </summary>
        void Rule()
        {
            component.System();
            if (component.Show)
            {
                this.Refresh();
                component.Show = false;
            }
            turn.System();
        }
        #endregion
    }
}
