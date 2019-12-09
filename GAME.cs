using System;
using System.Drawing;
using System.Windows.Forms;

namespace YINSH
{
    public partial class GAME : Form
    {
        Panel panel;

        //맵
        double width;
        double height;

        const int Map_Hex = 6;//정육각형 맵 중심으로부터 6방향
        const int Map_Size = 5;//맵 크기 5칸
        const int LineSize = 240;//길이

        int angle;//각도

        //실제 좌표를 담은 보드게임 좌표
        PointF[,] Game_Point = new PointF[0, 0];

        //마우스 위치에 따른 포인트 위치
        Point Node_Point;//노드 위치
        bool[,] Node_Bool = new bool[0, 0];//노드 

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
            Node_Bool = new bool[length, length];
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Map();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Node_Drawing(panel, new Point(e.X, e.Y));
        }

        private void Map()
        {
            Map_Drawing(panel);
            LinePos(width, height);
        }

        #region 그리는 함수들 정의
        private void Node_Drawing(Panel Node, Point Cursor_Point)
        {
            Graphics g = Node.CreateGraphics();//그래픽 담당
            Pen pen = new Pen(Color.Black, 1);//그리는 펜, 색깔, 두께

            int Size = LineSize / Map_Size;//한칸 길이
            int length = (Map_Size * 2) + 1;//배열 길이

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (Game_Point[i, j] != new PointF(0, 0) && Node_Bool[i, j] && Collision_Circle(Game_Point[i, j], Size / 2, Cursor_Point))
                    {
                        this.Refresh();
                        Node_Bool[Node_Point.X, Node_Point.Y] = true;
                        Node_Point = new Point(i, j);
                        Node_Bool[i, j] = false;
                        g.DrawEllipse(pen, Game_Point[i, j].X - (Size / 2), Game_Point[i, j].Y - (Size / 2), Size, Size);//게임 좌표 확인
                        break;
                    }
                }
            }
            g.Dispose();
            pen.Dispose();
        }

        void Map_Drawing(Panel Map)
        {
            Graphics g = Map.CreateGraphics();//그래픽 담당
            Pen pen = new Pen(Color.Black, 1);//그리는 펜, 색깔, 두께

            int Size = LineSize / Map_Size;//한칸 길이
            int tail = Map_Size * 2;//맵 끝에 튀어나오는 꼬리 꾸미기
            angle = 0;//각도
            int x = 0, y = LineSize;//좌표, 선분XY, 선분의 길이

            PointF[,] Map_Point = new PointF[Map_Size, Map_Hex];//맵 좌표

            /*
             * angle 방향
             * 1번 방향 60도
             * 2번 방향 120도
             * 3번 방향 180도
             * 4번 방향 240도
             * 5번 방향 300도
             * 6번 방향 360도
             * 꼭짓점
             * 꼭짓점좌표는 Map_X, Map_Y에 기록 되어 있다
             * Map_X, Map_Y -> Map_Point로 교체
             * Map_Point는 맵을 그리기 위한 최소한의 좌표를 기록하는 변수
             * Draw_Point는 맵을 그리는 좌표
             */

            for (int j = 0; j < Map_Size; j++)//맵의 사이즈만큼 등분한다
            {
                for (int i = 0; i < Map_Hex; i++)//정육각형이므로 6방향
                {
                    angle += 360 / Map_Hex;//60도
                    Map_Point[j, i] = new PointF(Center_Move(width / 2, Seg_X(angle, x, y)),
                                                Center_Move(height / 2, Seg_Y(angle, x, y)));//맵 좌표
                    PointF center = new PointF(Center_Move(width / 2, 0),
                                              Center_Move(height / 2, 0));//중심 좌표
                    PointF Draw_Point = new PointF(Center_Move(width / 2, Seg_X(angle, x, (y - Size) + tail)),
                                                  Center_Move(height / 2, Seg_Y(angle, x, (y - Size) + tail)));//그리는 좌표
                    if (j == 0)
                    {
                        g.DrawLine(pen, center, Draw_Point);//그림
                    }
                }
                y -= Size;
            }

            /*
             * 선분
             * 기록된 좌표를 사용하여 긋는다
             */
            y = 0 - (Size);//마지막 꼭짓점 안그려지는 만큼 일부러 잘랐다
            angle = (180 - (180 - (360 / Map_Hex)) / 2) + (360 / Map_Hex);//선분 길이 늘리기
            for (int j = 0; j < Map_Size; j++)//맵의 사이즈만큼 반복한다
            {
                for (int i = 0; i < Map_Hex; i++)//정육각형이므로 6번 반복한다
                {
                    int n = (i + 1 == Map_Hex) ? 0 : i + 1;//마지막 꼭짓점 안그려지는 만큼 일부러 잘랐다
                    PointF seg_1 = new PointF(Map_Point[j, i].X - Seg_X(angle, x, y + tail),
                                              Map_Point[j, i].Y - Seg_Y(angle, x, y + tail));//180도 꺽은 선분을 추가하여 
                    PointF seg_2 = new PointF(Map_Point[j, n].X + Seg_X(angle, x, y + tail),
                                              Map_Point[j, n].Y + Seg_Y(angle, x, y + tail));//선분 길이를 늘린다
                    g.DrawLine(pen, seg_1, seg_2);//첫번째 선분에서 두번째 선분까지 선긋기
                    angle += 360 / Map_Hex;
                }
                y += (j == 0) ? Size * 2 : Size;
                /*
                점점 작게 그릴수록 선분 길이 한칸 크기씩 늘려주기, 
                Size * 2는 처음에 자른 크기 복구
                */
            }
            g.Dispose();
            pen.Dispose();
        }

        void LinePos(double width, double height)
        {
            int Size = LineSize / Map_Size;
            //첫째줄
            Game_Pos(180, 10, width / 2, height / 2, Size, 1, 5, Map_Size);
            //둘째줄~다섯째줄
            for (int i = 4; i > 0; i--)
            {
                angle = 180;
                PointF Center_Point = new PointF(Center_Move(width / 2, Seg_X(angle, 0, (Size * Map_Size) - (Size * (5 - i)))),
                                                Center_Move(height / 2, Seg_Y(angle, 0, (Size * Map_Size) - (Size * (5 - i)))));//이동 가능한 그림좌표 범위

                Game_Pos(120, i + Map_Size, Center_Point.X, Center_Point.Y, Size, i - 5, 6, 6 - i);
            }
            //여섯째줄(X축)
            Game_Pos(120, 5, width / 2, height / 2, Size, -4, 5, Map_Size);
            //일곱째줄~열째줄
            for (int i = -1; i > -5; i--)
            {
                angle = 180;
                PointF Center_Point = new PointF(Center_Move(width / 2, Seg_X(angle, 0, (Size * Map_Size) - (Size * (5 - i)))),
                                                Center_Move(height / 2, Seg_Y(angle, 0, (Size * Map_Size) - (Size * (5 - i)))));//이동 가능한 그림좌표 범위

                Game_Pos(120, i + Map_Size, Center_Point.X, Center_Point.Y, Size, -4, i + 7, 6);
            }
            //열한째줄(마지막줄)
            Game_Pos(60, 0, width / 2, height / 2, Size, -4, 0, Map_Size);
        }

        /// <summary>
        /// 방향, 몇번째 줄, 중심x, 중심y, 이동, 시작, 끝, 반복
        /// </summary>
        void Game_Pos(int angle, int line, double width, double height, int move, int start, int end, int count)
        {
            int Size = LineSize / Map_Size;
            PointF Center_Point = new PointF(Center_Move(width, Seg_X(angle, 0, (move * count))),
                                            Center_Move(height, Seg_Y(angle, 0, (move * count))));//좌표밖 중심 정의
            angle = 300;
            for (int i = start; i < end; i++)
            {
                Game_Point[i + Map_Size, line] = new PointF(Center_Move(Center_Point.X, Seg_X(angle, 0, move)),
                                                            Center_Move(Center_Point.Y, Seg_Y(angle, 0, move)));
                Node_Bool[i + Map_Size, line] = true;
                move += Size;
            }
        }

        /// <summary>
        /// 지정된 좌표로부터
        /// </summary>
        float Center_Move(double P, double M)
        {
            double center = P + M;
            return (float)center;
        }

        float Seg_X(int angle, int x, int y)
        {
            double segment_x = Cos(angle, x) - Sin(angle, y);
            return (float)segment_x;
        }

        float Seg_Y(int angle, int x, int y)
        {
            double segment_y = Sin(angle, x) + Cos(angle, y);
            return (float)segment_y;
        }

        double Sin(int angle, int xy)
        {
            double sin = Math.Sin(Radian(angle)) * xy;
            return sin;
        }

        double Cos(int angle, int xy)
        {
            double cos = Math.Cos(Radian(angle)) * xy;
            return cos;
        }

        double Radian(int angle)
        {
            double radian = (Math.PI / 180) * angle;
            return radian;
        }

        /// <summary>
        /// 원 충돌범위
        /// </summary>
        bool Collision_Circle(PointF point, float r, Point Cursor_Point)//x축 y축 원 반지름, 마우스x 마우스y
        {
            double x = point.X - Cursor_Point.X;
            double y = point.Y - Cursor_Point.Y;

            double length = Math.Sqrt((x * x) + (y * y));
            if (length <= r)
                return true;
            else
                return false;
        }
        #endregion
    }
}
