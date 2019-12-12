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
            //그려지는 판
            panel = panel1;
            width = panel.Width;
            height = panel.Height;

            Board = new Bitmap(panel.Width, panel.Height);
            Component_Layer = new Bitmap(panel.Width, panel.Height);

            //Game_Point, Ring_Point, Marker_Point 맵 크기만큼 배열[x, y] 정의
            int length = (Map_Size * 2) + 1;
            Game_Point = new PointF[length, length];
            Ring_Point = new Item[length, length];
            Marker_Point = new Item[length, length];

            //Map 그리기 및 좌표 정의
            Map();

            //Turn 초기화
            Turn_Clear();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Board_Image();
            Component_Image();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor_Point = new Point(e.X, e.Y);
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Rule();
        }
    }
}
