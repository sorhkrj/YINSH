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
            Set,
            Ring,
            Marker,
            Cursor
        }
        // Ring, Marker 설치 좌표
        Item[,] Ring = new Item[0, 0];
        Item[,] Marker = new Item[0, 0];

        // Player
        Color?[,] Ring_Color = new Color?[0, 0];
        Color?[,] Marker_Color = new Color?[0, 0];

        // Ring, Marker 개수
        public int[] Ring_Quantity;
        public int Marker_Quantity;

        // Cursor 좌표
        Item[,] Cursor = new Item[0, 0];
        public Point Cursor_Point = new Point();
        Point Preview_Point = new Point();
        bool Cursor_Out;

        //좌표 텍스트
        public string Preview_Text = string.Empty;
        #endregion

        #region 함수
        public void Setting(Size size, int length)
        {
            Layer = new Bitmap(size.Width, size.Height);

            Ring = new Item[length, length];
            Marker = new Item[length, length];
            // 준비할 때 링 설치를 위해 설치가능하도록 초기화
            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    Ring[i, j] = Item.Set;
                }
            }

            Ring_Color = new Color?[length, length];
            Marker_Color = new Color?[length, length];

            Ring_Quantity = new int[turn.Player.Count];
            // Player 모두에게 Ring 5개 초기화
            for (var i = 0; i < turn.Player.Count; i++)
            {
                Ring_Quantity[i] = 5;
            }
            Marker_Quantity = 51;

            // Cursor
            Cursor = new Item[length, length];
            Cursor_Out = true;
        }

        /// <summary>
        /// Mouse Cursor 좌표
        /// </summary>
        public void Preview()
        {
            var side = Map.Length / Map.Size;
            var length = (Map.Size * 2) + 1;

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    if (map.Point[i, j] != new PointF(0, 0))
                    {
                        // 마우스를 Game_Point에 올렸을 때
                        if (Cursor[i, j] == Item.None && Collision_Circle(map.Point[i, j], side / 2, Cursor_Point))
                        {
                            Cursor_Out = true;
                            // 다시 그릴 수 있도록 true
                            Cursor[Preview_Point.X, Preview_Point.Y] = Item.None;
                            Preview_Point = new Point(i, j);
                            // 같은 자리에서 다시 그리지 않기 위해 false
                            Cursor[i, j] = Item.Cursor;
                            Preview_Text = "(" + map.Coord_Alphabet[i] + ", " + map.Coord_Number[j] + ")";
                            return;
                        }
                        // 마우스가 Game_Point를 벗어났을 때
                        else if (Cursor[i, j] == Item.Cursor && !Collision_Circle(map.Point[i, j], side / 2, Cursor_Point))
                        {
                            if (Cursor_Out)
                            {
                                Cursor_Out = false;
                                // 다시 그릴 수 있도록 true
                                Cursor[Preview_Point.X, Preview_Point.Y] = Item.None;
                                Preview_Text = string.Empty;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Layer를 지우고 Ring과 Marker를 다시 그리기
        /// </summary>
        public void Draw_Layer()
        {
            using (Graphics g = Graphics.FromImage(Layer))
            {
                g.Clear(Color.Transparent);
                Draw(Ring, Item.Ring);
                Draw(Marker, Item.Marker);
            }
        }

        /// <summary>
        /// 보드 좌표 위에 컴포넌트가 존재하면 Layer에 그리기
        /// </summary>
        void Draw(Item[,] component, Item set_item)
        {
            var side = Map.Length / Map.Size;
            var length = (Map.Size * 2) + 1;

            const int Ring_thickness = 6;
            const int Border_thikness = 3;
            var Ring_Size = side - (Ring_thickness * 2);
            var Marker_Size = side - ((Ring_thickness + Border_thikness) * 2);

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    if (map.Point[i, j] != new PointF(0, 0))
                    {
                        // component자리가 채워지면 그리기
                        if (component[i, j] == set_item)
                        {
                            // 그래픽 담당
                            using (Graphics g = Graphics.FromImage(Layer))
                            {
                                using (Pen Border_pen = new Pen(Color.Blue, Border_thikness))
                                {
                                    if (set_item == Item.Ring)
                                    {
                                        // Ring Color, Thickness
                                        using (Pen Ring_pen = new Pen((Color)Ring_Color[i, j], Ring_thickness))
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
                                        }
                                    }
                                    if (set_item == Item.Marker)
                                    {
                                        // Marker 색깔
                                        using (Brush Marker_brush = new SolidBrush((Color)Marker_Color[i, j]))
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
                        }
                    }
                }
            }
        }

        bool Ready()
        {
            var sum = 0;
            for(var i = 0; i < turn.Player.Count; i++)
            {
                sum += Ring_Quantity[i];
            }
            return (sum == 0) ? true : false;
        }

        /// <summary>
        /// 보드 좌표 위에 컴포넌트 올리기
        /// </summary>
        void Position(Item[,] component, Item set_item)
        {
            // 한칸 길이
            // 배열 길이
            // 현재 턴 색깔
            var side = Map.Length / Map.Size;
            var length = (Map.Size * 2) + 1;
            Color color = turn.Player[turn.User];

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    // Game_Point 위에 마우스를 올렸을 때
                    if (map.Point[i, j] != new PointF(0, 0) && Collision_Circle(map.Point[i, j], side / 2, Cursor_Point))
                    {
                        // component자리에 설치가 가능하다면
                        if (component[i, j] == Item.Set)
                        {
                            // 같은 자리에서 다시 그리지 않기위해 component 설치
                            component[i, j] = set_item;
                            // 좌표에 component 설치 및 기능
                            if(set_item == Item.Ring)
                            {
                                Ring_Color[i, j] = color;
                                Ring_Quantity[turn.User]--;

                                Marker[i, j] = Item.Set;

                                // 준비가 완료된 후 턴 넘기기
                                if (Ready())
                                {
                                    // 턴 넘기기
                                    turn.Next = true;
                                }
                                // 컴포넌트 넘기기
                                turn.Check = true;
                            }
                            if(set_item == Item.Marker)
                            {
                                Marker_Color[i, j] = color;
                                Marker_Quantity--;
                                Ring_Quantity[turn.User]++;
                                Ring[i, j] = Item.None;
                            }
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 컴포넌트가 존재할 때 설치
        /// </summary>
        public void System()
        {
            if (Ring_Quantity[turn.User] > 0)
            {
                Position(Ring, Item.Ring);
            }
            else if (Marker_Quantity > 0)
            {
                Position(Marker, Item.Marker);
            }
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
