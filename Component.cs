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
                if (Instance == null)
                {
                    Instance = new Component();
                }
                return Instance;
            }
        }

        #region 인스턴스
        readonly Map map = Map.GetInstance;
        readonly Turn turn = Turn.GetInstance;
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
            Marker,
            Cursor
        }
        // Ring, Marker 설치 좌표
        public Item[,] Ring = new Item[0, 0];
        public Item[,] Marker = new Item[0, 0];
        public Item[,] Cursor = new Item[0, 0];
        // 색 저장
        public Color?[,] R_Color = new Color?[0, 0];
        public Color?[,] M_Color = new Color?[0, 0];

        public bool Cursor_Out;

        // Cursor 좌표
        public Point Cursor_Point = new Point();
        public Point Point = new Point();
        #endregion

        #region 함수
        public void Preview_Drawing(Graphics g)
        {
            var Size = Map.Length / Map.Size;
            var length = (Map.Size * 2) + 1;

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    if (map.Point[i, j] != new PointF(0, 0))
                    {
                        // Game_Point 위에 마우스를 올렸을 때
                        if (Cursor[i, j] == Item.Cursor && Collision_Circle(map.Point[i, j], Size / 2, Cursor_Point))
                        {
                            using (g)
                            {
                                using (Pen pen = new Pen(Color.Black, 1))
                                {
                                    g.DrawEllipse(pen, map.Point[i, j].X - (Size / 2),
                                                       map.Point[i, j].Y - (Size / 2),
                                                       Size,
                                                       Size);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Layer_Drawing()
        {
            using (Graphics g = Graphics.FromImage(Layer))
            {
                g.Clear(Color.Transparent);
                Drawing(Ring, Item.Ring);
                Drawing(Marker, Item.Marker);
            }
        }

        /// <summary>
        /// 보드 좌표 위에 컴포넌트가 존재하면 그리기
        /// </summary>
        void Drawing(Item[,] component, Item set_item)
        {
            var Size = Map.Length / Map.Size;
            var length = (Map.Size * 2) + 1;

            const int Ring_thickness = 6;
            const int Border_thikness = 3;
            var Ring_Size = Size - (Ring_thickness * 2);
            var Marker_Size = Size - ((Ring_thickness + Border_thikness) * 2);

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    if (map.Point[i, j] != new PointF(0, 0))
                    {
                        // 그래픽 담당
                        using (Graphics g = Graphics.FromImage(Layer))
                        {
                            using (Pen Border_pen = new Pen(Color.Blue, Border_thikness))
                            {
                                // component자리가 비어있고 Game_Point 위에 마우스를 올렸을 때
                                if (component[i, j] == set_item)
                                {
                                    if(set_item == Item.Ring)
                                    {
                                        // Ring Color, Thickness
                                        using (Pen Ring_pen = new Pen((Color)R_Color[i, j], Ring_thickness))
                                        {
                                            // Ring In Border
                                            g.DrawEllipse(Border_pen, map.Point[i, j].X - ((Ring_Size + Ring_thickness) / 2),
                                                                      map.Point[i, j].Y - ((Ring_Size + Ring_thickness) / 2),
                                                                      Ring_Size + Ring_thickness,
                                                                      Ring_Size + Ring_thickness);
                                            // Ring In Border
                                            g.DrawEllipse(Border_pen, map.Point[i, j].X - (Marker_Size / 2),
                                                                      map.Point[i, j].Y - (Marker_Size / 2),
                                                                      Marker_Size,
                                                                      Marker_Size);
                                            // Ring Draw
                                            g.DrawEllipse(Ring_pen, map.Point[i, j].X - ((Ring_Size + 0.6f) / 2),
                                                                    map.Point[i, j].Y - ((Ring_Size + 0.4f) / 2),
                                                                    Ring_Size,
                                                                    Ring_Size);
                                            turn.Next = true;
                                        }
                                    }
                                    if(set_item == Item.Marker)
                                    {
                                        // Marker 색깔
                                        using (Brush Marker_brush = new SolidBrush((Color)M_Color[i, j]))
                                        {
                                            // Marker Border
                                            g.DrawEllipse(Border_pen, map.Point[i, j].X - (Marker_Size / 2),
                                                                      map.Point[i, j].Y - (Marker_Size / 2),
                                                                      Marker_Size,
                                                                      Marker_Size);
                                            // Marker Draw
                                            g.FillEllipse(Marker_brush, map.Point[i, j].X - (Marker_Size / 2),
                                                                        map.Point[i, j].Y - (Marker_Size / 2),
                                                                        Marker_Size,
                                                                        Marker_Size);
                                        }
                                    }
                                }
                            }
                            //label1.Text = "(" + (i - 5) + ", " + (j - 5) + ")";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 보드 좌표 위에 컴포넌트 올리기
        /// </summary>
        public void System(Item[,] component, Item set_item)
        {
            // 한칸 길이
            // 배열 길이
            // 현재 턴 색깔
            var Size = Map.Length / Map.Size;
            var length = (Map.Size * 2) + 1;
            Color color = turn.Player[turn.User];

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    // Game_Point 위에 마우스를 올렸을 때
                    if (map.Point[i, j] != new PointF(0, 0) && Collision_Circle(map.Point[i, j], Size / 2, Cursor_Point))
                    {
                        // component자리가 비어있으면
                        if (component[i, j] == Item.None)
                        {
                            // 같은 자리에서 다시 그리지 않기
                            component[i, j] = set_item;
                            // 색 지정하기
                            if(set_item == Item.Ring)
                            {
                                R_Color[i, j] = color;
                            }
                            if(set_item == Item.Marker)
                            {
                                M_Color[i, j] = color;
                            }
                            //label1.Text = "(" + (i - 5) + ", " + (j - 5) + ")";
                            turn.Check = true;
                            return;
                        }
                    }
                }
            }
        }

        public void Setting(Size size, int length)
        {
            Layer = new Bitmap(size.Width, size.Height);

            Ring = new Item[length, length];
            Marker = new Item[length, length];
            Cursor = new Item[length, length];

            R_Color = new Color?[length, length];
            M_Color = new Color?[length, length];

            Cursor_Out = true;
        }
        #endregion

        #region 수학 식
        /// <summary>
        /// 원 충돌범위
        /// </summary>
        public bool Collision_Circle(PointF point, float r, Point Cursor_Point)//x축 y축 원 반지름, 마우스x 마우스y
        {
            double x = point.X - Cursor_Point.X;
            double y = point.Y - Cursor_Point.Y;

            var length = Math.Sqrt((x * x) + (y * y));
            return (length <= r) ? true : false;
        }
        #endregion

    }
}
