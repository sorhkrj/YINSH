using System;
using System.Drawing;

namespace YINSH
{
    public class Map
    {
        private static Map Instance = null;

        public static Map GetInstance()
        {
            if (Instance == null)
            {
                Instance = new Map();
            }
            return Instance;
        }

        #region 변수
        public Bitmap Board;

        // 정육각형 맵 중심으로부터 6방향
        // 맵 크기 5칸
        // 길이
        const int Hex = 6;
        public const int Size = 5;
        public const int Length = 240;

        // 각도
        int angle;

        // 실제 좌표를 담은 보드게임 좌표
        public PointF[,] Point = new PointF[0, 0];

        // Hex 방향만큼 추가
        public Point[] Direction = new Point[]
            { 
                // X+
                new Point(1, 0), 
                // -X
                new Point(-1, 0), 
                // Y+
                new Point(0, 1), 
                // -Y
                new Point(0, -1), 
                // X+, Y+
                new Point(1, 1), 
                // -X, -Y
                new Point(-1, -1)
            };

        // 보드게임 문자 좌표 글자
        readonly public string[] Coord_Number = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };
        readonly public string[] Coord_Alphabet = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k" };

        public string Coord(Point point)
        {
            string Alphabet, Number;
            Alphabet = Coord_Alphabet[point.X];
            Number = Coord_Number[point.Y];
            return $"{Alphabet}{Number}";
        }
        #endregion

        #region 함수
        /// <summary>
        /// Board와 게임 Point 초기화
        /// </summary>
        void Setting(Size size, int length)
        {
            // Board, Layer, Point 맵 크기만큼 배열[x, y] 정의
            Board = new Bitmap(size.Width, size.Width);
            Point = new PointF[length, length];
        }

        /// <summary>
        /// Board에 맵 그림을 그린다
        /// </summary>
        void Draw(Size board)
        {
            var side = Length / Size;
            var tail = Size;
            angle = 0;

            // 보드 좌표
            PointF[,] Map_Point = new PointF[Size, Hex];
            // 좌표, 선분XY, 선분의 길이
            int x = 0, y = Length;
            // 그래픽 담당
            using (Graphics g = Graphics.FromImage(Board))
            {
                // 그리는 펜, 색깔, 두께
                using (Pen pen = new Pen(Color.Black, 1))
                {
                    #region 꼭짓점 그리기
                    // 등분
                    for (var j = 0; j < Size; j++)
                    {
                        // 방향
                        for (var i = 0; i < Hex; i++)
                        {
                            angle += 360 / Hex;
                            Map_Point[j, i] = new PointF(Center_Move(board.Width / 2, Seg_X(angle, x, y)),
                                                        Center_Move(board.Height / 2, Seg_Y(angle, x, y)));
                            PointF center = new PointF(Center_Move(board.Width / 2, 0),
                                                      Center_Move(board.Height / 2, 0));
                            PointF Draw_Point = new PointF(Center_Move(board.Width / 2, Seg_X(angle, x, (y - side) + tail)),
                                                          Center_Move(board.Height / 2, Seg_Y(angle, x, (y - side) + tail)));
                            if (j == 0)
                            {
                                g.DrawLine(pen, center, Draw_Point);
                            }
                        }
                        y -= side;
                    }
                    #endregion

                    #region 선분 그리기
                    // 마지막 꼭짓점 안그려지는 만큼 일부러 잘랐다
                    y = 0 - (side);
                    // 선분 길이 늘리기
                    angle = (180 - (180 - (360 / Hex)) / 2) + (360 / Hex);
                    for (var j = 0; j < Size; j++)
                    {
                        for (var i = 0; i < Hex; i++)
                        {
                            // 마지막 꼭짓점 안그려지는 만큼 일부러 잘랐다
                            var n = (i + 1 == Hex) ? 0 : i + 1;
                            // 180도 꺽은 선분을 추가하여 선분 길이를 늘린다
                            PointF seg_1 = new PointF(Map_Point[j, i].X - Seg_X(angle, x, y + tail),
                                                      Map_Point[j, i].Y - Seg_Y(angle, x, y + tail));
                            PointF seg_2 = new PointF(Map_Point[j, n].X + Seg_X(angle, x, y + tail),
                                                      Map_Point[j, n].Y + Seg_Y(angle, x, y + tail));
                            g.DrawLine(pen, seg_1, seg_2);
                            angle += 360 / Hex;
                        }
                        // 점점 작게 그릴수록 선분 길이 한칸 크기씩 늘려주기, 
                        // size * 2는 처음에 자른 크기 복구
                        y += (j == 0) ? side * 2 : side;
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Board좌표에 실제 좌표를 지정해준다
        /// </summary>
        void Line(Size board)
        {
            var side = Length / Size;
            // 첫째줄
            Position(180, 10, board.Width / 2, board.Height / 2, side, 1, 5, Size);
            // 둘째줄~다섯째줄
            for (var i = 4; i > 0; i--)
            {
                angle = 180;
                // 이동 가능한 그림좌표 범위
                PointF Center_Point = new PointF(Center_Move(board.Width / 2, Seg_X(angle, 0, (side * Size) - (side * (Size - i)))),
                                                Center_Move(board.Height / 2, Seg_Y(angle, 0, (side * Size) - (side * (Size - i)))));

                Position(120, i + Size, Center_Point.X, Center_Point.Y, side, i - 5, 6, 6 - i);
            }
            // 여섯째줄(X축)
            Position(120, 5, board.Width / 2, board.Height / 2, side, -4, 5, Size);
            // 일곱째줄~열째줄
            for (var i = -1; i > -5; i--)
            {
                angle = 180;
                // 이동 가능한 그림좌표 범위
                PointF Center_Point = new PointF(Center_Move(board.Width / 2, Seg_X(angle, 0, (side * Size) - (side * (Size - i)))),
                                                Center_Move(board.Height / 2, Seg_Y(angle, 0, (side * Size) - (side * (Size - i)))));

                Position(120, i + Size, Center_Point.X, Center_Point.Y, side, -5, i + 6, 6);
            }
            // 열한째줄(마지막줄)
            Position(60, 0, board.Width / 2, board.Height / 2, side, -4, 0, Size);
        }

        /// <summary>
        /// 방향, 몇번째 줄, 중심x, 중심y, 이동, 시작, 끝, 반복
        /// </summary>
        void Position(int angle, int line, double width, double height, int move, int start, int end, int count)
        {
            var side = Length / Size;
            // 좌표밖 중심 정의
            PointF Center_Point = new PointF(Center_Move(width, Seg_X(angle, 0, (move * count))),
                                            Center_Move(height, Seg_Y(angle, 0, (move * count))));
            angle = 300;
            for (var i = start; i < end; i++)
            {
                Point[i + Size, line] = new PointF(Center_Move(Center_Point.X, Seg_X(angle, 0, move)),
                                                            Center_Move(Center_Point.Y, Seg_Y(angle, 0, move)));
                move += side;
            }
        }

        void Coordinate()
        {
            var side = Size * 4;
            Font font = new Font("Consolas", 12f, FontStyle.Regular, GraphicsUnit.Point);
            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };

            using (Graphics g = Graphics.FromImage(Board))
            {
                using (Brush brush = Brushes.Black)
                {
                    #region 영문자 좌표
                    Draw_Text(Coord_Alphabet, g, 0, font, brush, Point[0, 1], 0, -side, stringFormat);
                    for (var x = 1; x <= 4; x++)
                    {
                        Draw_Text(Coord_Alphabet, g, x, font, brush, Point[x, 0], 0, -side, stringFormat);
                    }
                    Draw_Text(Coord_Alphabet, g, 5, font, brush, Point[5, 1], 0, -side, stringFormat);
                    for (int x = 6, y = 1; x <= 9; x++, y++)
                    {
                        Draw_Text(Coord_Alphabet, g, x, font, brush, Point[x, y], 0, -side, stringFormat);
                    }
                    Draw_Text(Coord_Alphabet, g, 10, font, brush, Point[10, 6], 0, -side, stringFormat);
                    #endregion

                    #region 숫자 좌표
                    Draw_Text(Coord_Number, g, 0, font, brush, Point[1, 0], side, side, stringFormat);
                    for (var y = 1; y <= 4; y++)
                    {
                        Draw_Text(Coord_Number, g, y, font, brush, Point[0, y], side, side, stringFormat);
                    }
                    Draw_Text(Coord_Number, g, 5, font, brush, Point[1, 5], side, side, stringFormat);
                    for (int x = 1, y = 6; y <= 9; x++, y++)
                    {
                        Draw_Text(Coord_Number, g, y, font, brush, Point[x, y], side, side, stringFormat);
                    }
                    Draw_Text(Coord_Number, g, 10, font, brush, Point[6, 10], side, side, stringFormat);
                    #endregion
                }
            }
        }

        void Draw_Text(string[] coordinate, Graphics g, int index, Font font, Brush brush, PointF point, float sideX, float sideY, StringFormat stringFormat)
        {
            g.DrawString(coordinate[index], font, brush, point.X - sideX,
                                                         point.Y - sideY, 
                                                         stringFormat);
        }

        public void System(Size size, int length)
        {
            Setting(size, length);
            Draw(size);
            Line(size);
            Coordinate();
        }
        #endregion

        #region 수학 식
        ///참고자료
        ///https://ko.wikipedia.org/wiki/%ED%9A%8C%EC%A0%84%EB%B3%80%ED%99%98%ED%96%89%EB%A0%AC

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
        #endregion
    }
}
