using System.Drawing;
using System.Windows.Forms;

namespace YINSH
{
    public partial class GAME : Form
    {
        #region 인스턴스
        Map map = Map.GetInstance;
        Component component = Component.GetInstance;
        Turn turn = Turn.GetInstance;
        #endregion

        #region 변수
        Panel panel;

        public static double width;
        public static double height;

        public static Point Cursor_Point;//마우스 위치
        #endregion

        #region 함수
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
            if (turn.Each[turn.User] < 5)
                component.System(component.Ring_Point, Component.Item.Ring);
            else
                component.System(component.Marker_Point, Component.Item.Marker);
            //Marker Set -> Ring Move
            turn.System();
        }
        #endregion
    }
}
