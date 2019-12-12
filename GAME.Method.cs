using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace YINSH
{
    /// <summary>
    /// 분할 클래스 변수, 함수 모아두는 Method Class
    /// </summary>
    public partial class GAME : Form
    {
        #region 변수
        #region Draw Image
        Panel panel;
        Bitmap Board;
        Bitmap Component_Layer;
        #endregion

        #region Map Data
        double width;
        double height;

        const int Map_Hex = 6;//정육각형 맵 중심으로부터 6방향
        const int Map_Size = 5;//맵 크기 5칸
        const int LineSize = 240;//길이

        int angle;//각도

        PointF[,] Game_Point = new PointF[0, 0];//실제 좌표를 담은 보드게임 좌표
        #endregion

        #region Component Data
        /// <summary>
        /// None, Ring, Marker
        /// </summary>
        enum Item
        {
            None,
            Ring,
            Marker
        }
        //Ring, Marker 설치 좌표
        Item[,] Ring_Point = new Item[0, 0];
        Item[,] Marker_Point = new Item[0, 0];
        Point Component_Point = new Point();
        #endregion

        #region Mouse Data
        Point Cursor_Point;//마우스 위치
        #endregion

        #region Turn Data
        int Turn;//반복
        int Turn_Count;//Turn 횟수
        /// <summary>
        /// YINSH Player White & Black
        /// </summary>
        List<Color> Turn_Color = new List<Color>();//Turn 색깔
        #endregion
        #endregion

        #region 함수
        #region 규칙
        void Rule()
        {
            if (Turn_Count <= 10)
                Component(Ring_Point, Item.Ring);
            else
                Component(Marker_Point, Item.Marker);
            //Marker Set -> Ring Move
        }

        void Turn_System()
        {
            Turn_Count++;
            Turn++;
            Turn = (Turn_Count % Turn_Color.Count == 0) ? 0 : Turn;
        }

        void Turn_Clear()
        {
            Turn_Color.Clear();
            Turn_Color.Add(Color.White);
            Turn_Color.Add(Color.Black);
            Turn = -1;
            Turn_Count = -1;
        }
        #endregion

        #region 컴포넌트

        void Component(Item[,] component, Item set_item)
        {
            ComponentPos(component, set_item);
            Component_Drawing(component, set_item, Turn_Color[Turn]);
        }

        /// <summary>
        /// Component_Layer를 panel에 보여준다
        /// </summary>
        void Component_Image()
        {
            Graphics g = panel.CreateGraphics();
            g.DrawImage(Component_Layer, new Point(0, 0));
            g.Dispose();
        }

        /// <summary>
        /// 노드를 좌표 위에 그린다
        /// </summary>
        void Component_Drawing(Item[,] component, Item set_item, Color color)
        {
            Graphics g = Graphics.FromImage(Component_Layer);//그래픽 담당
            int Ring_thickness = 6;//Ring 두께
            int Border_thikness = 3;

            Pen Ring_pen = new Pen(color, Ring_thickness);//Ring Color, Thickness
            Pen Border_pen = new Pen(Color.Blue, Border_thikness);//Marker's Border Color,Thickness
            Brush Marker_brush = new SolidBrush(color);//Marker 색깔

            int Size = LineSize / Map_Size;//한칸 길이
            int length = (Map_Size * 2) + 1;//배열 길이
            int Ring_Size = Size - (Ring_thickness * 2);//Ring 크기
            int Marker_Size = Size - ((Ring_thickness + Border_thikness) * 2);//Marker 크기

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    //Game_Point 위에 마우스를 올렸을 때
                    if (Game_Point[i, j] != new PointF(0, 0) && component[i, j] != Item.None && Collision_Circle(Game_Point[i, j], Size / 2, Cursor_Point))
                    {
                        if (set_item == Item.Ring)
                        {
                            g.DrawEllipse(Border_pen, Game_Point[i, j].X - ((Ring_Size + Ring_thickness) / 2),
                                                      Game_Point[i, j].Y - ((Ring_Size + Ring_thickness) / 2),
                                                      Ring_Size + Ring_thickness,
                                                      Ring_Size + Ring_thickness);//Ring In Border
                            g.DrawEllipse(Border_pen, Game_Point[i, j].X - (Marker_Size / 2),
                                                      Game_Point[i, j].Y - (Marker_Size / 2),
                                                      Marker_Size,
                                                      Marker_Size);//Ring In Border
                            g.DrawEllipse(Ring_pen, Game_Point[i, j].X - ((Ring_Size + 0.6f) / 2),
                                                    Game_Point[i, j].Y - ((Ring_Size + 0.4f) / 2),
                                                    Ring_Size,
                                                    Ring_Size);//Ring Draw
                        }
                        if(set_item == Item.Marker)
                        {
                            g.DrawEllipse(Border_pen, Game_Point[i, j].X - (Marker_Size / 2), 
                                                      Game_Point[i, j].Y - (Marker_Size / 2),
                                                      Marker_Size, 
                                                      Marker_Size);//Marker Border
                            g.FillEllipse(Marker_brush, Game_Point[i, j].X - (Marker_Size / 2), 
                                                        Game_Point[i, j].Y - (Marker_Size / 2), 
                                                        Marker_Size, 
                                                        Marker_Size);//Marker Draw
                        }
                        this.Refresh();
                        break;
                    }
                }
            }
            g.Dispose();
            Ring_pen.Dispose();
            Border_pen.Dispose();
            Marker_brush.Dispose();
        }

        /// <summary>
        /// 보드 좌표 위에 노드가 있는지 확인
        /// </summary>
        void ComponentPos(Item[,] component, Item set_item)
        {
            int Size = LineSize / Map_Size;//한칸 길이
            int length = (Map_Size * 2) + 1;//배열 길이

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (Game_Point[i, j] != new PointF(0, 0))
                    {
                        //Game_Point 위에 마우스를 올렸을 때
                        if (component[i, j] == Item.None && Collision_Circle(Game_Point[i, j], Size / 2, Cursor_Point))
                        {
                            Turn_System();
                            component[Component_Point.X, Component_Point.Y] = Item.None;//다시 그릴 수 있도록 true
                            Component_Point = new Point(i, j);
                            component[i, j] = set_item;//같은 자리에서 다시 그리지 않기 위해 false
                            label1.Text = "Turn : " + Turn_Count + ", (" + (i - 5) + ", " + (j - 5) + ")";
                            break;
                        }
                        //마우스가 Game_Point밖으로 벗어났을 때
                        else if (component[i, j] != Item.None && !Collision_Circle(Game_Point[i, j], Size / 2, Cursor_Point))
                        {
                            component[Component_Point.X, Component_Point.Y] = Item.None;//다시 그릴 수 있도록 true
                        }
                    }
                }
            }
        }
        #endregion

        #region 맵

        /// <summary>
        /// Board에 Map을 그리고 저장한 뒤
        /// Map 좌표를 정의한다
        /// </summary>
        void Map()
        {
            Map_Drawing();
            LinePos(width, height);
        }

        /// <summary>
        /// Board를 panel에 보여준다
        /// </summary>
        void Board_Image()
        {
            Graphics g = panel.CreateGraphics();
            g.DrawImage(Board, new Point(0, 0));
            g.Dispose();
        }

        /// <summary>
        /// Board에 맵 그림을 그린다
        /// </summary>
        void Map_Drawing()
        {
            Graphics g = Graphics.FromImage(Board);//그래픽 담당
            Pen pen = new Pen(Color.Black, 1);//그리는 펜, 색깔, 두께

            int Size = LineSize / Map_Size;//한칸 길이
            int tail = Map_Size * 2;//맵 끝에 튀어나오는 꼬리 꾸미기
            angle = 0;//각도
            int x = 0, y = LineSize;//좌표, 선분XY, 선분의 길이

            PointF[,] Map_Point = new PointF[Map_Size, Map_Hex];//맵 좌표

            #region 꼭짓점 그리기
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
            #endregion

            #region 선분 그리기
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
            #endregion
            g.Dispose();
            pen.Dispose();
        }

        /// <summary>
        /// Board좌표에 실제 좌표를 지정해준다
        /// </summary>
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

                Game_Pos(120, i + Map_Size, Center_Point.X, Center_Point.Y, Size, -5, i + 6, 6);
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
                move += Size;
            }
        }
        #endregion
        #endregion

        #region 수학 식
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
