using System;
using System.Drawing;

namespace YINSH
{
    public class Component
    {
        private static Component Instance = null;

        public static Component GetInstance
        {
            get
            {
                if(Instance == null)
                {
                    Instance = new Component();
                }
                return Instance;
            }
        }

        #region 인스턴스
        Map map = Map.GetInstance;
        Turn turn = Turn.GetInstance;
        #endregion

        #region 변수
        /// <summary>
        /// None, Ring, Marker
        /// </summary>
        public Bitmap Layer;

        public enum Item
        {
            None,
            Ring,
            Marker
        }
        //Ring, Marker 설치 좌표
        public Item[,] Ring_Point = new Item[0, 0];
        public Item[,] Marker_Point = new Item[0, 0];
        Point Point = new Point();
        #endregion

        #region 함수
        public void System(Item[,] component, Item set_item)
        {
            Position(component, set_item);
            Drawing(component, set_item, turn.Player[turn.User]);
        }

        /// <summary>
        /// 노드를 좌표 위에 그린다
        /// </summary>
        void Drawing(Item[,] component, Item set_item, Color color)
        {
            using (Graphics g = Graphics.FromImage(Layer))//그래픽 담당
            {
                const int Ring_thickness = 6;//Ring 두께
                const int Border_thikness = 3;


                int Size = Map.Length / Map.Size;//한칸 길이
                int length = (Map.Size * 2) + 1;//배열 길이
                int Ring_Size = Size - (Ring_thickness * 2);//Ring 크기
                int Marker_Size = Size - ((Ring_thickness + Border_thikness) * 2);//Marker 크기

                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        //Game_Point 위에 마우스를 올렸을 때
                        if (map.Point[i, j] != new PointF(0, 0) && component[i, j] != Item.None && Collision_Circle(map.Point[i, j], Size / 2, GAME.Cursor_Point))
                        {
                            using (Pen Border_pen = new Pen(Color.Blue, Border_thikness))//Marker's Border Color,Thickness
                            {
                                if (set_item == Item.Ring)
                                {
                                    using (Pen Ring_pen = new Pen(color, Ring_thickness))//Ring Color, Thickness
                                    {
                                        g.DrawEllipse(Border_pen, map.Point[i, j].X - ((Ring_Size + Ring_thickness) / 2),
                                                                  map.Point[i, j].Y - ((Ring_Size + Ring_thickness) / 2),
                                                                  Ring_Size + Ring_thickness,
                                                                  Ring_Size + Ring_thickness);//Ring In Border
                                        g.DrawEllipse(Border_pen, map.Point[i, j].X - (Marker_Size / 2),
                                                                  map.Point[i, j].Y - (Marker_Size / 2),
                                                                  Marker_Size,
                                                                  Marker_Size);//Ring In Border
                                        g.DrawEllipse(Ring_pen, map.Point[i, j].X - ((Ring_Size + 0.6f) / 2),
                                                                map.Point[i, j].Y - ((Ring_Size + 0.4f) / 2),
                                                                Ring_Size,
                                                                Ring_Size);//Ring Draw
                                        turn.Next = true;
                                    }
                                }
                                if (set_item == Item.Marker)
                                {
                                    using (Brush Marker_brush = new SolidBrush(color))//Marker 색깔
                                    {
                                        g.DrawEllipse(Border_pen, map.Point[i, j].X - (Marker_Size / 2),
                                                                  map.Point[i, j].Y - (Marker_Size / 2),
                                                                  Marker_Size,
                                                                  Marker_Size);//Marker Border
                                        g.FillEllipse(Marker_brush, map.Point[i, j].X - (Marker_Size / 2),
                                                                    map.Point[i, j].Y - (Marker_Size / 2),
                                                                    Marker_Size,
                                                                    Marker_Size);//Marker Draw
                                    }
                                }
                            }
                            turn.Check = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 보드 좌표 위에 노드가 있는지 확인
        /// </summary>
        void Position(Item[,] component, Item set_item)
        {
            int Size = Map.Length / Map.Size;//한칸 길이
            int length = (Map.Size * 2) + 1;//배열 길이

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (map.Point[i, j] != new PointF(0, 0))
                    {
                        //Game_Point 위에 마우스를 올렸을 때
                        if (component[i, j] == Item.None && Collision_Circle(map.Point[i, j], Size / 2, GAME.Cursor_Point))
                        {
                            component[Point.X, Point.Y] = Item.None;//다시 그릴 수 있도록 true
                            Point = new Point(i, j);
                            component[i, j] = set_item;//같은 자리에서 다시 그리지 않기 위해 false
                            //label1.Text = "(" + (i - 5) + ", " + (j - 5) + ")";
                            break;
                        }
                        //마우스가 Game_Point밖으로 벗어났을 때
                        else if (component[i, j] != Item.None && !Collision_Circle(map.Point[i, j], Size / 2, GAME.Cursor_Point))
                        {
                            component[Point.X, Point.Y] = Item.None;//다시 그릴 수 있도록 true
                        }
                    }
                }
            }
        }
        #endregion

        #region 수학 식
        /// <summary>
        /// 원 충돌범위
        /// </summary>
        bool Collision_Circle(PointF point, float r, Point Cursor_Point)//x축 y축 원 반지름, 마우스x 마우스y
        {
            double x = point.X - Cursor_Point.X;
            double y = point.Y - Cursor_Point.Y;

            double length = Math.Sqrt((x * x) + (y * y));
            return (length <= r) ? true : false;
        }
        #endregion

    }
}
