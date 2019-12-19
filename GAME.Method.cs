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

        #region 변수
        Panel panel;
        #endregion

        #region 함수
        void Init()
        {
            panel = panel1;
            Size size = panel.Size;
            int length = (Map.Size * 2) + 1;

            // Map: Board, Point Set & Draw Map;
            // Component: Layer, Ring, Marker, Cursor;
            // Turn: Clear
            map.Setting(size, length);
            component.Setting(size, length);
            turn.Setting();
        }

        void Image()
        {
            using (Graphics g = panel.CreateGraphics())
            {
                g.DrawImage(map.Board, new Point(0, 0));
                g.DrawImage(component.Layer, new Point(0, 0));
                component.Preview_Drawing(g);
            }
        }

        void Preview()
        {
            var Size = Map.Length / Map.Size;
            var length = (Map.Size * 2) + 1;

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    if (map.Point[i, j] != new PointF(0, 0))
                    {
                        // 마우스를 Game_Point에 올렸을 때
                        if (component.Cursor[i, j] == Component.Item.None && component.Collision_Circle(map.Point[i, j], Size / 2, component.Cursor_Point))
                        {
                            component.Cursor_Out = true;
                            // 다시 그릴 수 있도록 true
                            component.Cursor[component.Point.X, component.Point.Y] = Component.Item.None;
                            component.Point = new Point(i, j);
                            // 같은 자리에서 다시 그리지 않기 위해 false
                            component.Cursor[i, j] = Component.Item.Cursor;
                            this.Refresh();
                            return;
                        }
                        // 마우스가 Game_Point를 벗어났을 때
                        else if (component.Cursor[i, j] == Component.Item.Cursor && !component.Collision_Circle(map.Point[i, j], Size / 2, component.Cursor_Point))
                        {
                            if (component.Cursor_Out)
                            {
                                this.Refresh();
                                component.Cursor_Out = false;
                                // 다시 그릴 수 있도록 true
                                component.Cursor[component.Point.X, component.Point.Y] = Component.Item.None;
                            }
                        }
                    }
                }
            }
        }

        void Rule()
        {
            if (turn.Each[turn.User] < 5)
            {
                component.System(component.Ring, Component.Item.Ring);
            }
            else
            {
                component.System(component.Marker, Component.Item.Marker);
            }
            component.Layer_Drawing();
            this.Refresh();
            turn.System();
        }
        #endregion
    }
}
