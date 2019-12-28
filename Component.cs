using System;
using System.Collections.Generic;
using System.Drawing;

namespace YINSH
{
    public class Component
    {
        private static Component Instance = null;

        public static Component GetInstance()
        {
            if (Instance == null)
            {
                Instance = new Component();
            }
            return Instance;
        }

        #region 인스턴스
        readonly Map map = Map.GetInstance();
        readonly Turn turn = Turn.GetInstance();
        readonly Score score = Score.GetInstance();
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
            Pick,
            PickUp,
            Cursor
        }
        // Ring, Marker 설치 좌표
        Item[,] Ring;
        Item[,] Marker;

        // Player
        Color[,] Ring_Color;
        Color[,] Marker_Color;

        // Ring, Marker 개수
        public int[] Ring_Quantity;
        public int Marker_Quantity;

        // 들어올린 링, 마커 좌표
        Point Ring_PickUp = new Point();
        readonly List<Point> Marker_PickUp = new List<Point>();

        // 링이 지나간 거리의 마커를 검사하는 구간 Ring_PickUp ~ Ring_Set
        Point Ring_Set = new Point();

        // Marker를 가져갈 수 있는 최대치는 5
        const int PickMax = 5;

        int PickAxis;
        bool PickDirection;
        bool EndPickUp;
        bool[] PickCount;
        int PickUser;

        // Cursor 좌표
        Item[,] Cursor;
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

            Ring_Color = new Color[length, length];
            Marker_Color = new Color[length, length];

            Ring_Quantity = new int[turn.Player.Count];
            // Player 모두에게 Ring 5개 초기화
            for (var i = 0; i < turn.Player.Count; i++)
            {
                Ring_Quantity[i] = 5;
            }
            Marker_Quantity = 51;

            PickDirection = false;
            EndPickUp = true;
            PickCount = new bool[turn.Player.Count];
            for (var i = 0; i < turn.Player.Count; i++)
            {
                PickCount[i] = false;
            }

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
                            // Preview_Text = "(" + i + ", " + j + ")";
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
                Draw(Marker, Item.Pick);
                Draw(Marker, Item.PickUp);
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
                                        using (Pen Ring_pen = new Pen(Ring_Color[i, j], Ring_thickness))
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
                                        using (Brush Marker_brush = new SolidBrush(Marker_Color[i, j]))
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
                                    if (set_item == Item.Pick)
                                    {
                                        if (Marker_Color[i, j] == turn.Player[PickUser])
                                        {
                                            // Marker 색깔
                                            using (Brush Marker_brush = new SolidBrush(Color.FromArgb(200, Marker_Color[i, j])))
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
                                        else
                                        {
                                            // Marker 색깔
                                            using (Brush Marker_brush = new SolidBrush(Marker_Color[i, j]))
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
                                using (Pen Border_pen = new Pen(Color.Yellow, Border_thikness))
                                {
                                    if (set_item == Item.PickUp)
                                    {
                                        // Marker 색깔
                                        using (Brush Marker_brush = new SolidBrush(Color.FromArgb(200, Marker_Color[i, j])))
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
        bool Ready(int[] count)
        {
            var sum = 0;
            for (var i = 0; i < turn.Player.Count; i++)
            {
                sum += count[i];
            }
            return (sum == 0) ? true : false;
        }

        /// <summary>
        /// 마커를 설치할 수 있는 좌표
        /// </summary>
        void CanSet()
        {
            var length = (Map.Size * 2) + 1;
            int next = turn.User;
            Color color = turn.Player[(++next < turn.Player.Count) ? next : 0];

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

            #region 다음 턴의 링이 움직일 수 있으면 설치 가능
            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    if (Ring[i, j] == Item.Ring)
                    {
                        if (Ring_Color[i, j] == color)
                        {
                            // 현재 위치에서 움직일 수 있는지 확인
                            if (CanMove(new Point(i, j), length))
                            {
                                Marker[i, j] = Item.Set;
                            }
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 컴포넌트가 움직인 위치에 컴포넌트가 있는지 확인
        /// </summary>
        bool MoveItem(Item[,] component, Point point, Item item)
        {
            if (component[point.X, point.Y] == item)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 이동할 좌표가 배열안에 있고 맵 밖으로 나가지 않으면 true
        /// </summary>
        bool PosIn(Point Move, Point direction, int length)
        {
            if (Move.X + direction.X < length && Move.Y + direction.Y < length &&
                Move.X + direction.X >= 0 && Move.Y + direction.Y >= 0 &&
                map.Point[Move.X + direction.X, Move.Y + direction.Y] != PointF.Empty)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 링이 움직일 수 있는지 확인
        /// </summary>
        bool CanMove(Point Set, int length)
        {
            // 정의된 모든 방향 확인
            for (var i = 0; i < map.Direction.Length; i++)
            {
                // 설치한 위치부터 움직일 Move
                Point Move = Set;

                // 다음 방향의 좌표가 좌표안에 있으면
                while (PosIn(Move, map.Direction[i], length))
                {
                    // 다음 방향으로 움직인다
                    Move.X += map.Direction[i].X;
                    Move.Y += map.Direction[i].Y;

                    // 움직인 방향에 링이 있다면
                    if (MoveItem(Ring, Move, Item.Ring)) { break; }
                    // 움직인 방향에 마커가 있다면
                    if (MoveItem(Marker, Move, Item.None))
                    {
                        // 그 다음 방향이 배열안에 없으면
                        if (!PosIn(Move, map.Direction[i], length)) { break; }
                        // 그 다음 방향에 마커가 없다면
                        if (Marker[Move.X + map.Direction[i].X, Move.Y + map.Direction[i].Y] == Item.None)
                        {
                            Move = new Point(Move.X + map.Direction[i].X, Move.Y + map.Direction[i].Y);
                            if (MoveItem(Ring, Move, Item.Ring)) { break; }
                        }
                        // 그 다음 방향에도 마커가 있다면
                        else { break; }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 링이 움직일 수 있는 범위
        /// </summary>
        void RingMove(Point Set, int length)
        {
            // 정의된 모든 방향 확인
            for (var i = 0; i < map.Direction.Length; i++)
            {
                // 설치한 위치부터 움직일 Move
                Point Move = Set;

                // 마커를 뛰어넘었는지 확인
                bool JumpMarker = false;
                // 다음 방향의 좌표가 좌표안에 있고 마커를 뛰어넘지 않았다면
                while (PosIn(Move, map.Direction[i], length) && !JumpMarker)
                {
                    // 움직인 방향으로 움직인다
                    Move.X += map.Direction[i].X;
                    Move.Y += map.Direction[i].Y;

                    // 움직인 방향에 링이 있다면
                    if (MoveItem(Ring, Move, Item.Ring)) { break; }
                    // 움직인 방향에 마커가 있다면
                    if (MoveItem(Marker, Move, Item.Marker))
                    {
                        if(!PosIn(Move, map.Direction[i], length)) { break; }
                        // 그 다음 방향에 마커가 없다면
                        if (Marker[Move.X + map.Direction[i].X, Move.Y + map.Direction[i].Y] == Item.None)
                        {
                            // 그 다음 방향을 확인한다
                            Point overmove = new Point(Move.X + map.Direction[i].X, Move.Y + map.Direction[i].Y);
                            if (MoveItem(Ring, overmove, Item.Ring)) { break; }
                            // 그 다음 방향에 링을 설치할 수 있다
                            Ring[overmove.X, overmove.Y] = Item.Set;
                            JumpMarker = true;
                        }
                        // 그 다음 방향에 마커가 있다면
                        if (JumpMarker) { break; }
                    }

                    // 링을 설치할 수 있다
                    Ring[Move.X, Move.Y] = Item.Set;
                }
            }
        }

        /// <summary>
        /// 이동한 방향값
        /// </summary>
        Point MoveDirection(Point Pick, Point set, int length)
        {
            // 정의된 모든 방향을 확인
            for (var i = 0; i < map.Direction.Length; i++)
            {
                Point Move = Pick;
                // 설치한 링을 찾았는지 확인
                bool Find = false;

                // 좌표안에 있고 설치한 링 좌표를 찾지 못했다면
                while (PosIn(Move, map.Direction[i], length) && !Find)
                {
                    // 다음 방향으로 움직인다
                    Move.X += map.Direction[i].X;
                    Move.Y += map.Direction[i].Y;
                    // 이동한 좌표에서 설치한 링 좌표를 찾으면 그 방향으로 반환한다
                    if (new Point(Move.X, Move.Y) == set)
                    {
                        return map.Direction[i];
                    }
                }
            }
            return Point.Empty;
        }

        /// <summary>
        /// 마커 뒤집기
        /// </summary>
        void Reverse(Point Pick, Point set, int length)
        {
            Point direction = MoveDirection(Pick, set, length);
            Point change = Pick;
            // 설치한 다음 방향부터 뒤집으므로 다음 방향으로 이동
            change.X += direction.X;
            change.Y += direction.Y;
            // 링을 설치한 좌표일 때까지 반복
            while (change != set)
            {
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
                change.X += direction.X;
                change.Y += direction.Y;
            }
        }

        /// <summary>
        /// 보드 좌표 위에 컴포넌트 내려놓기
        /// </summary>
        void SetPosition(Item[,] component, Color[,] component_color, Item set_item)
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
                        if (component[i, j] == Item.Set)
                        {
                            // 같은 자리에서 다시 그리지 않기위해 component 설치
                            component[i, j] = set_item;
                            component_color[i, j] = color;
                            // 좌표에 component 설치 후 기능
                            if (set_item == Item.Ring)
                            {
                                Ring_Quantity[turn.User]--;

                                // 준비가 완료되고 가져갈 컴포넌트가 없으면
                                if (Ready(Ring_Quantity))
                                {
                                    if (Ring_PickUp != Point.Empty)
                                    {
                                        Ring_Set = new Point(i, j);
                                        Reverse(Ring_PickUp, Ring_Set, length);
                                        CanPick(Ring_PickUp, Ring_Set, length);
                                    }
                                    if (EndPickUp)
                                    {
                                        // 턴 넘기기
                                        turn.Next = true;
                                        // 어떤 링에 마커를 설치할 수 있는지 확인
                                        CanSet();
                                    }
                                }
                                if (EndPickUp)
                                {
                                    // 컴포넌트 넘기기
                                    turn.Check = true;
                                }
                            }
                            if (set_item == Item.Marker)
                            {
                                Marker_Quantity--;
                                Ring_Quantity[turn.User]++;
                                Ring[i, j] = Item.None;
                                Ring_Color[i, j] = Color.Empty;
                                Ring_PickUp = Point.Empty;
                                Ring_PickUp = new Point(i, j);
                                RingMove(new Point(i, j), length);
                            }
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 규칙에 알맞게 컴포넌트 내려놓기
        /// </summary>
        void Set()
        {
            if (Ring_Quantity[turn.User] > 0)
            {
                SetPosition(Ring, Ring_Color, Item.Ring);
            }
            else if (Ring_Quantity[turn.User] == 0 && Marker_Quantity > 0)
            {
                SetPosition(Marker, Marker_Color, Item.Marker);
            }
            else
            {
                score.check = true;
            }
        }

        /// <summary>
        /// 점수를 얻을 수 있는지 확인
        /// </summary>
        void CanPick(Point Pick, Point set, int length)
        {
            Point direction = MoveDirection(Pick, set, length);
            List<Point> Pick_Point = new List<Point>();
            // 모든 플레이어의 색 확인
            for (var user = 0; user < turn.Player.Count; user++)
            {
                Point check = Pick;
                // 현재 방향이 링을 설치한 좌표일 때까지 반복
                while (check != set)
                {
                    if ((MoveItem(Marker, check, Item.Marker) || MoveItem(Marker, check, Item.Pick)) && Marker_Color[check.X, check.Y] == turn.Player[user])
                    {
                        // 정의된 모든 방향 확인
                        for (var i = 0; i < map.Direction.Length; i += 2)
                        {
                            Pick_Point.Clear();
                            Pick_Point.Add(check);
                            for (var j = i; j < i + 2; j++)
                            {
                                // 설치한 위치부터 움직일 Move
                                Point Move = check;
                                // 현재 방향이 좌표안에 있으면
                                while (PosIn(Move, Point.Empty, length))
                                {
                                    // 움직인 방향에 링이 있거나 또는 아무것도 없다면
                                    if (MoveItem(Marker, Move, Item.None) || MoveItem(Ring, Move, Item.Ring)) { break; }
                                    // 움직인 방향에 마커가 있다면
                                    if (MoveItem(Marker, Move, Item.Marker) || MoveItem(Marker, Move, Item.Pick))
                                    {
                                        // 움직인 방향에 마커의 색이 검색 중인 색이라면
                                        if (Marker_Color[Move.X, Move.Y] == turn.Player[user])
                                        {
                                            // 반대줄 검사할 때 겹치는 중간은 Pass
                                            if (Move != check)
                                            {
                                                Pick_Point.Add(Move);
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    // 움직인 방향으로 움직인다
                                    Move.X += map.Direction[j].X;
                                    Move.Y += map.Direction[j].Y;
                                }
                            }
                            // 점수 획득
                            if (Pick_Point.Count >= 5)
                            {
                                EndPickUp = false;
                                PickCount[user] = true;
                                for(var count = 0; count < Pick_Point.Count; count++)
                                {
                                    Marker[Pick_Point[count].X, Pick_Point[count].Y] = Item.Pick;
                                }
                            }
                        }
                    }
                    // 다음 방향으로 이동
                    check.X += direction.X;
                    check.Y += direction.Y;
                }
            }
            if (!EndPickUp)
            {
                PickUser = turn.User;
                if (!PickCount[turn.User])
                {
                    PickUser++;
                    PickUser = (PickUser >= turn.Player.Count) ? 0 : PickUser;
                }
                for (var i = 0; i < turn.Player.Count; i++)
                {
                    PickCount[i] = false;
                }
            }
        }

        /// <summary>
        /// 가져갈 수 있는 연속된 마커인지 확인
        /// </summary>
        bool CanDrag(Point Pick, int length)
        {
            if(PickDirection) { return false; }
            // 정의된 모든 방향 확인
            for (var i = 0; i < map.Direction.Length; i += 2)
            {
                for (var j = 0; j < i + 2; j++)
                {
                    // 설치한 위치부터 움직일 Move
                    Point Move = Marker_PickUp[0];

                    // 다음 방향의 좌표가 좌표안에 있으면
                    if (PosIn(Move, map.Direction[j], length))
                    {
                        // 다음 방향으로 움직인다
                        Move.X += map.Direction[j].X;
                        Move.Y += map.Direction[j].Y;

                        // 움직인 방향에 선택한 마커가 있다면
                        if (Move == Pick)
                        {
                            PickDirection = true;
                            PickAxis = i;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 정해진 방향으로 가져갈 수 있는 연속된 마커인지 확인
        /// </summary>
        bool CanDrag(Point Pick, int axis, int length)
        {
            for (int pu = 0; pu < Marker_PickUp.Count; pu++)
            {
                for (int i = axis; i < axis + 2; i++)
                {
                    Point Move = Marker_PickUp[pu];
                    // 다음 방향의 좌표가 좌표안에 있으면
                    if (PosIn(Move, map.Direction[i], length))
                    {
                        Move.X += map.Direction[i].X;
                        Move.Y += map.Direction[i].Y;

                        // 움직인 방향이 선택할 마커, 선택한 마커가 아니면
                        if (!MoveItem(Marker, Move, Item.Pick) && !MoveItem(Marker, Move, Item.PickUp)) { break; }

                        // 움직인 방향에 선택한 마커가 있다면
                        if (Move == Pick)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 가져갈 마커를 선택
        /// </summary>
        void DragPick(Point Drag, int length)
        {
            if (Marker_PickUp.Count == 0)
            {
                Marker[Drag.X, Drag.Y] = Item.PickUp;
                Marker_PickUp.Add(Drag);
            }
            else if (CanDrag(Drag, length))
            {
                Marker[Drag.X, Drag.Y] = Item.PickUp;
                Marker_PickUp.Add(Drag);
            }
            else if (CanDrag(Drag, PickAxis, length))
            {
                Marker[Drag.X, Drag.Y] = Item.PickUp;
                Marker_PickUp.Add(Drag);
            }
            else
            {
                DragPickUp(Item.PickUp, Item.Pick, length);
            }
        }

        /// <summary>
        /// 선택한 아이템을 다른 아이템으로 변경할 때
        /// </summary>
        void DragPickUp(Item pick_item, Item change_item, int length)
        {
            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    if (Marker[i, j] == pick_item)
                    {
                        Marker[i, j] = change_item;
                        if(change_item == Item.None) { Marker_Color[i, j] = Color.Empty; }
                    }
                }
            }
            Marker_PickUp.Clear();
            if (change_item == Item.Pick)
            {
                PickDirection = false;
            }
            if (change_item == Item.None)
            {
                PickDirection = false;
                Marker_Quantity += PickMax;
            }
        }

        /// <summary>
        /// 보드 좌표 위에 컴포넌트 가져가기
        /// </summary>
        void PickPosition(Color[,] component_color, Item set_item)
        {
            // 한칸 길이
            // 배열 길이
            var side = Map.Length / Map.Size;
            var length = (Map.Size * 2) + 1;

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    // Game_Point 위에 마우스를 올렸을 때
                    if (map.Point[i, j] != Point.Empty && Collision_Circle(map.Point[i, j], side / 2, Cursor_Point))
                    {
                        if (component_color[i, j] == turn.Player[PickUser])
                        {
                            if (set_item == Item.Ring)
                            {
                                if (Ring[i, j] == Item.Ring)
                                {
                                    Ring[i, j] = Item.None;
                                    Ring_Color[i, j] = Color.Empty;
                                    PickDirection = false;
                                    DragPickUp(Item.PickUp, Item.None, length);
                                    DragPickUp(Item.Pick, Item.Marker, length);
                                    score.Player[PickUser]++;
                                    EndPickUp = true;
                                    CanPick(Ring_PickUp, Ring_Set, length);
                                    score.check = true;
                                    if (EndPickUp)
                                    {
                                        EndPickUp = true;
                                        turn.Check = true;
                                        turn.Next = true;
                                        CanSet();
                                    }
                                }
                            }
                            if (set_item == Item.Marker)
                            {
                                if (Marker[i, j] == Item.Pick)
                                {
                                    if ((Marker_PickUp.Count < PickMax))
                                    {
                                        DragPick(new Point(i, j), length);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DragPickUp(Item.PickUp, Item.Pick, length);
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 규칙에 알맞게 컴포넌트 가져가기
        /// </summary>
        void Pick()
        {
            if(Marker_PickUp.Count == PickMax)
            {
                PickPosition(Ring_Color, Item.Ring);
            }
            else
            {
                PickPosition(Marker_Color, Item.Marker);
            }
        }

        /// <summary>
        /// 컴포넌트 내려놓기 or 가져가기
        /// </summary>
        void Prograss()
        {
            if (EndPickUp) { Set(); }
            else { Pick(); }
        }

        public void System()
        {
            Prograss();
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
