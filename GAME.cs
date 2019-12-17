using System.Drawing;
using System.Windows.Forms;

namespace YINSH
{
    /// <summary>
     /// 분할 클래스 진행을 맡은 Main Class
     /// </summary>
    public partial class GAME : Form
    {
        public GAME()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            #region 인스턴스
            Map map = Map.GetInstance;
            Component component = Component.GetInstance;
            #endregion

            //그려지는 판
            panel = panel1;
            width = panel.Width;
            height = panel.Height;

            map.Board = new Bitmap(panel.Width, panel.Height);
            component.Layer = new Bitmap(panel.Width, panel.Height);

            //Game_Point, Ring_Point, Marker_Point 맵 크기만큼 배열[x, y] 정의
            int length = (Map.Size * 2) + 1;
            map.Point = new PointF[length, length];
            component.Ring_Point = new Component.Item[length, length];
            component.Marker_Point = new Component.Item[length, length];

            //Map 그리기 및 좌표 정의
            map.System();

            //Turn 초기화
            turn.Setting();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Image();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor_Point = new Point(e.X, e.Y);
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Rule();
            this.Refresh();
        }
    }
}
