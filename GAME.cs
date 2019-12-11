using System.Drawing;
using System.Windows.Forms;

namespace YINSH
{
    public partial class GAME : Form
    {
        public GAME()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            panel = panel1;//그려지는 판
            width = panel.Width;
            height = panel.Height;

            //Game_Point, Ring, Marker 맵 크기만큼 배열[x, y] 정의
            int length = (Map_Size * 2) + 1;
            Game_Point = new PointF[length, length];
            Ring.Set = new bool[length, length];
            Marker.Set = new bool[length, length];

            //Map 그리기 및 좌표 정의
            Map();

            //Component Ring, Marker 정의
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Board_Image();
            //Ring_Image();
            //Marker_Image();
            Component_Drawing(Ring, Item.Ring, Color.Black);//test color
            Component_Drawing(Marker, Item.Marker, Color.White);//test color
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor_Point = new Point(e.X, e.Y);
            ComponentPos(Ring);//설치할 때 적용
            ComponentPos(Marker);//설치할 때 적용
        }
    }
}
