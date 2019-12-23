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

        enum Item
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

        // 마커를 설치하고 들어올린 링 좌표
        Point Pick = new Point();

        // Player
        Color?[,] Ring_Color = new Color?[0, 0];
        Color?[,] Marker_Color = new Color?[0, 0];

        // 컴포넌트를 설치하면 true
        public bool Show;

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

            // 컴포넌트를 설치하면 true Layer를 그리면 false
            Show = false;

            // Cursor
            Cursor = new Item[length, length];
            Cursor_Out = true;
        }

        /// <summary>
        /// Mouse Cursor 좌표
        /// </summary>
        public void Preview(Point point)
        {
            Cursor_Point = point;

            var side = Map.Length / Map.Size;
            var length = (Map.Size * 2) + 1;

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    if (map.Point[i, j] != Point.Empty)
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
        void Draw_Layer()
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
                    if (map.Point[i, j] != Point.Empty)
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

        /// <summary>
        /// 턴을 넘길 준비가 되었는지 확인
        /// </summary>
        bool Ready()
        {
            var sum = 0;
            for (var i = 0; i < turn.Player.Count; i++)
            {
                sum += Ring_Quantity[i];
            }
            return (sum == 0) ? true : false;
        }

        /// <summary>
        /// 마커를 설치할 수 있는 좌표
        /// </summary>
        void CanSet()
        {
            if (Ready())
            {
                var length = (Map.Size * 2) + 1;
                Color color = turn.Player[turn.User];

                #region 컴포넌트 설치 위치 초기화
                for (var i = 0; i < length; i++)
                {
                    for (var j = 0; j < length; j++)
                    {
                        if (Ring[i, j] == Item.Set)
                        {
                            Ring[i, j] = Item.None;
                        }
                        if (Marker[i, j] == Item.Set)
                        {
                            Marker[i, j] = Item.None;
                        }
                    }
                }
                #endregion

                #region 마커를 링이 설치된 자신의 턴 색 위치에 Item.Set
                for (var i = 0; i < length; i++)
                {
                    for (var j = 0; j < length; j++)
                    {
                        if (Ring[i, j] == Item.Ring)
                        {
                            if (Ring_Color[i, j] == color)
                            {
                                Marker[i, j] = Item.Set;
                            }
                        }
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 컴포넌트가 움직인 위치에 컴포넌트가 있는지 확인
        /// </summary>
        bool OtherComponent(Item[,] component, Point point, Item other_item)
        {
            if (component[point.X, point.Y] == other_item)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 이동할 좌표가 배열안에 있는지 확인
        /// </summary>
        bool IndexIn(Point Move, int i, int length)
        {
            if (Move.X + map.Direction[i].X < length && Move.Y + map.Direction[i].Y < length &&
               Move.X + map.Direction[i].X >= 0 && Move.Y + map.Direction[i].Y >= 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 링이 움직일 수 있는 범위
        /// </summary>
        void CanMove(Point Set, int length)
        {
            // 정의된 모든 방향 확인
            for (var i = 0; i < map.Direction.Length; i++)
            {
                // 설치한 위치부터 움직일 Move
                Point Move = Set;

                // 마커를 뛰어넘었는지 확인
                bool JumpMarker = false;

                // 다음 방향으로 움직였을 때 배열 안에 있고 맵 밖이 아니고 마커를 뛰어넘지 않았다면
                while (IndexIn(Move, i, length) && map.Point[Move.X, Move.Y] != PointF.Empty && !JumpMarker)
                {
                    // 다음 방향으로 움직인다
                    Move.X += map.Direction[i].X;
                    Move.Y += map.Direction[i].Y;

                    // 다음 방향에 링이 있다면
                    if (OtherComponent(Ring, Move, Item.Ring)) { break; }
                    // 다음 방향에 마커가 있고 그 다음 방향이 배열안에 있으면
                    if (Marker[Move.X, Move.Y] == Item.Marker && IndexIn(Move, i, length))
                    {
                        // 그 다음 방향에 마커가 없다면
                        if (Marker[Move.X + map.Direction[i].X, Move.Y + map.Direction[i].Y] == Item.None)
                        {
                            Move = new Point(Move.X + map.Direction[i].X, Move.Y + map.Direction[i].Y);
                            if (OtherComponent(Ring, Move, Item.Ring)) { break; }
                            JumpMarker = true;
                        }
                    }
                    // 링을 설치할 수 있다
                    Ring[Move.X, Move.Y] = Item.Set;
                }
            }
        }

        /// <summary>
        /// 마커 뒤집기
        /// </summary>
        void Reverse(Point Pick, Point set, int length)
        {
            Point direction = new Point();
            #region 이동한 방향값 얻어오기
            // 정의된 모든 방향을 확인
            for (var i = 0; i < map.Direction.Length; i++)
            {
                Point Move = Pick;
                // 설치한 링을 찾았는지 확인
                bool Find = false;
                // 다음 방향으로 움직였을 때 맵 밖이 아니고(맵 배열 안에 있고)
                // 비어있는 좌표가 아니고
                // 설치한 링 좌표를 찾지 못했다면
                while (Move.X + map.Direction[i].X < length && Move.Y + map.Direction[i].Y < length &&
                       Move.X + map.Direction[i].X >= 0 && Move.Y + map.Direction[i].Y >= 0 &&
                       map.Point[Move.X, Move.Y] != PointF.Empty && !Find)
                {
                    // 다음 방향으로 움직인다
                    Move.X += map.Direction[i].X;
                    Move.Y += map.Direction[i].Y;
                    // 이동한 좌표에서 설치한 링 좌표를 찾으면 그 방향으로 반환한다
                    if (new Point(Move.X, Move.Y) == set)
                    {
                        direction = map.Direction[i];
                        Find = true;
                        break;
                    }
                }
                if (Find) { break; }
            }
            #endregion

            #region 이동한 거리에 마커가 있으면 뒤집기
            // 다음 방향이 링을 설치한 좌표일 때까지 반복
            Point change = Pick;
            while (new Point(change.X + direction.X, change.Y + direction.Y) != set)
            {
                change.X += direction.X;
                change.Y += direction.Y;
                if (Marker[change.X, change.Y] == Item.Marker)
                {
                    for (var i = 0; i < turn.Player.Count; i++)
                    {
                        if (Marker_Color[change.X, change.Y] == turn.Player[i])
                        {
                            Marker_Color[change.X, change.Y] = turn.Player[(++i < turn.Player.Count) ? i : 0];
                            break;
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 보드 좌표 위에 컴포넌트 내려놓기
        /// </summary>
        void Position(Item[,] component, Color?[,] component_color, Item set_item)
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
                    if (map.Point[i, j] != Point.Empty && Collision_Circle(map.Point[i, j], side / 2, Cursor_Point))
                    {
                        // component자리에 설치가 가능하다면
                        if (component[i, j] == Item.Set)
                        {
                            // 같은 자리에서 다시 그리지 않기위해 component 설치
                            component[i, j] = set_item;
                            component_color[i, j] = color;
                            // 좌표에 component 설치 및 기능
                            if (set_item == Item.Ring)
                            {
                                Ring_Quantity[turn.User]--;

                                // 준비가 완료된 후
                                if (Ready())
                                {
                                    if (Pick != Point.Empty)
                                    {
                                        Reverse(Pick, new Point(i, j), length);
                                    }
                                    // 턴 넘기기
                                    turn.Next = true;
                                }
                                // 컴포넌트 넘기기
                                turn.Check = true;
                            }
                            if (set_item == Item.Marker)
                            {
                                Marker_Quantity--;
                                Ring_Quantity[turn.User]++;
                                Ring[i, j] = Item.None;
                                Pick = new Point(i, j);
                                CanMove(new Point(i, j), length);
                            }
                            Show = true;
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 컴포넌트 설치
        /// </summary>
        void SetPos()
        {
            if (Ring_Quantity[turn.User] > 0)
            {
                Position(Ring, Ring_Color, Item.Ring);
            }
            else if (Marker_Quantity > 0)
            {
                Position(Marker, Marker_Color, Item.Marker);
            }
        }

        public void System()
        {
            CanSet();
            SetPos();
            Draw_Layer();
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
