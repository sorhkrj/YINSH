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

        void Image()
        {
            using (Graphics g = panel.CreateGraphics())
            {
                g.DrawImage(map.Board, new Point(0, 0));
                g.DrawImage(component.Layer, new Point(0, 0));
            }
        }

        void Rule()
        {
            component.System();
            turn.System();
        }
        #endregion
    }
}
