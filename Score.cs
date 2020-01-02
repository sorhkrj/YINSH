using System.Drawing;

namespace YINSH
{
    public class Score
    {
        private static Score Instance = null;

        public static Score GetInstance()
        {
            if (Instance == null)
            {
                Instance = new Score();
            }
            return Instance;
        }

        #region 인스턴스
        readonly Turn turn = Turn.GetInstance();
        #endregion

        #region 변수
        public Bitmap Layer;
        PointF[,] Draw_Point;
        public int[] Player;
        public int winscore;
        public bool check;
        #endregion

        #region 함수
        public void Setting(Size size)
        {
            Layer = new Bitmap(size.Width, size.Height);
            Draw_Point = new PointF[turn.Player.Count, winscore];
            Player = new int[turn.Player.Count];
            Position(size);
            Draw_Map();
        }

        void Position(Size size)
        {
            var side = Map.Length / Map.Size;
            var height = size.Height;
            var width = size.Width;

            var line = 0;
            for (var i = 0; i < winscore; i++)
            {
                line += side;
                Draw_Point[0, i] = new PointF(line + (side / 5), height - side);
            }
            line = 0;
            for (var i = 0; i < winscore; i++)
            {
                line += side;
                Draw_Point[1, i] = new PointF(width - line - (side / 5), side);
            }
        }

        void Draw_Map()
        {
            var side = Map.Length / Map.Size;

            const int Ring_thickness = 6;
            var Ring_Size = side - (Ring_thickness * 2);
            // 그래픽 담당
            using (Graphics g = Graphics.FromImage(Layer))
            {
                // Ring Color, Thickness
                using (Pen Ring_pen = new Pen(Color.LightSkyBlue, Ring_thickness))
                {
                    for (var player = 0; player < turn.Player.Count; player++)
                    {
                        for (int i = 0; i < winscore; i++)
                        {
                            // Ring Draw
                            g.DrawEllipse(Ring_pen, Draw_Point[player, i].X - ((Ring_Size + 0.6f) / 2),
                                                    Draw_Point[player, i].Y - ((Ring_Size + 0.4f) / 2),
                                                    Ring_Size,
                                                    Ring_Size);
                        }
                    }
                }
            }
        }

        void Draw()
        {
            var side = Map.Length / Map.Size;

            const int Ring_thickness = 6;
            const int Border_thikness = 3;
            var Ring_Size = side - (Ring_thickness * 2);
            var Marker_Size = side - ((Ring_thickness + Border_thikness) * 2);
            // 그래픽 담당
            using (Graphics g = Graphics.FromImage(Layer))
            {
                using (Pen Border_pen = new Pen(Color.Black, Border_thikness))
                {
                    for (var player = 0; player < turn.Player.Count; player++)
                    {
                        for (int i = 0; i < Player[player]; i++)
                        {
                            // Ring Color, Thickness
                            using (Pen Ring_pen = new Pen(turn.Player[player], Ring_thickness))
                            {
                                // Ring In Border
                                g.DrawEllipse(Border_pen, Draw_Point[player, i].X - ((Ring_Size + Ring_thickness) / 2),
                                                          Draw_Point[player, i].Y - ((Ring_Size + Ring_thickness) / 2),
                                                          Ring_Size + Ring_thickness,
                                                          Ring_Size + Ring_thickness);
                                // Ring In Border
                                g.DrawEllipse(Border_pen, Draw_Point[player, i].X - (Marker_Size / 2),
                                                          Draw_Point[player, i].Y - (Marker_Size / 2),
                                                          Marker_Size,
                                                          Marker_Size);
                                // Ring Draw
                                g.DrawEllipse(Ring_pen, Draw_Point[player, i].X - ((Ring_Size + 0.6f) / 2),
                                                        Draw_Point[player, i].Y - ((Ring_Size + 0.4f) / 2),
                                                        Ring_Size,
                                                        Ring_Size);
                            }
                        }
                    }
                }
            }
        }

        public void System()
        {
            if (check)
            {
                Draw();
            }
        }
        #endregion
    }
}
