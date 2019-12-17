using System;
using System.Drawing;

namespace YINSH
{
    public class Map
    {
        private static Map Instance = null;

        public static Map GetInstance
        {
            get
            {
                if(Instance == null)
                {
                    Instance = new Map();
                }
                return Instance;
            }
        }

        #region 변수
        public Bitmap Board;

        public const int Hex = 6;//정육각형 맵 중심으로부터 6방향
        public const int Size = 5;//맵 크기 5칸
        public const int Length = 240;//길이

        int angle;//각도

        public PointF[,] Point = new PointF[0, 0];//실제 좌표를 담은 보드게임 좌표
        #endregion

        #region 함수
        /// <summary>
        /// Board에 Map을 그리고 저장한 뒤
        /// Map 좌표를 정의한다
        /// </summary>
        public void System()
        {
            Drawing();
            Line(GAME.width, GAME.height);
        }

        /// <summary>
        /// Board에 맵 그림을 그린다
        /// </summary>
        void Drawing()
        {
            using (Graphics g = Graphics.FromImage(Board))//그래픽 담당
            {
                using (Pen pen = new Pen(Color.Black, 1))//그리는 펜, 색깔, 두께
                {

                    int Size = Length / Map.Size;//한칸 길이
                    int tail = Map.Size * 2;//맵 끝에 튀어나오는 꼬리 꾸미기
                    angle = 0;//각도
                    int x = 0, y = Length;//좌표, 선분XY, 선분의 길이

                    PointF[,] Map_Point = new PointF[Map.Size, Map.Hex];//맵 좌표

                    #region 꼭짓점 그리기
                    for (int j = 0; j < Map.Size; j++)//맵의 사이즈만큼 등분한다
                    {
                        for (int i = 0; i < Map.Hex; i++)//정육각형이므로 6방향
                        {
                            angle += 360 / Map.Hex;//60도
                            Map_Point[j, i] = new PointF(Center_Move(GAME.width / 2, Seg_X(angle, x, y)),
                                                        Center_Move(GAME.height / 2, Seg_Y(angle, x, y)));//맵 좌표
                            PointF center = new PointF(Center_Move(GAME.width / 2, 0),
                                                      Center_Move(GAME.height / 2, 0));//중심 좌표
                            PointF Draw_Point = new PointF(Center_Move(GAME.width / 2, Seg_X(angle, x, (y - Size) + tail)),
                                                          Center_Move(GAME.height / 2, Seg_Y(angle, x, (y - Size) + tail)));//그리는 좌표
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
                    angle = (180 - (180 - (360 / Map.Hex)) / 2) + (360 / Map.Hex);//선분 길이 늘리기
                    for (int j = 0; j < Map.Size; j++)//맵의 사이즈만큼 반복한다
                    {
                        for (int i = 0; i < Map.Hex; i++)//정육각형이므로 6번 반복한다
                        {
                            int n = (i + 1 == Map.Hex) ? 0 : i + 1;//마지막 꼭짓점 안그려지는 만큼 일부러 잘랐다
                            PointF seg_1 = new PointF(Map_Point[j, i].X - Seg_X(angle, x, y + tail),
                                                      Map_Point[j, i].Y - Seg_Y(angle, x, y + tail));//180도 꺽은 선분을 추가하여 
                            PointF seg_2 = new PointF(Map_Point[j, n].X + Seg_X(angle, x, y + tail),
                                                      Map_Point[j, n].Y + Seg_Y(angle, x, y + tail));//선분 길이를 늘린다
                            g.DrawLine(pen, seg_1, seg_2);//첫번째 선분에서 두번째 선분까지 선긋기
                            angle += 360 / Map.Hex;
                        }
                        y += (j == 0) ? Size * 2 : Size;
                        /*
                        점점 작게 그릴수록 선분 길이 한칸 크기씩 늘려주기, 
                        Size * 2는 처음에 자른 크기 복구
                        */
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Board좌표에 실제 좌표를 지정해준다
        /// </summary>
        void Line(double width, double height)
        {
            int Size = Length / Map.Size;
            //첫째줄
            Position(180, 10, width / 2, height / 2, Size, 1, 5, Map.Size);
            //둘째줄~다섯째줄
            for (int i = 4; i > 0; i--)
            {
                angle = 180;
                PointF Center_Point = new PointF(Center_Move(width / 2, Seg_X(angle, 0, (Size * Map.Size) - (Size * (5 - i)))),
                                                Center_Move(height / 2, Seg_Y(angle, 0, (Size * Map.Size) - (Size * (5 - i)))));//이동 가능한 그림좌표 범위

                Position(120, i + Map.Size, Center_Point.X, Center_Point.Y, Size, i - 5, 6, 6 - i);
            }
            //여섯째줄(X축)
            Position(120, 5, width / 2, height / 2, Size, -4, 5, Map.Size);
            //일곱째줄~열째줄
            for (int i = -1; i > -5; i--)
            {
                angle = 180;
                PointF Center_Point = new PointF(Center_Move(width / 2, Seg_X(angle, 0, (Size * Map.Size) - (Size * (5 - i)))),
                                                Center_Move(height / 2, Seg_Y(angle, 0, (Size * Map.Size) - (Size * (5 - i)))));//이동 가능한 그림좌표 범위

                Position(120, i + Map.Size, Center_Point.X, Center_Point.Y, Size, -5, i + 6, 6);
            }
            //열한째줄(마지막줄)
            Position(60, 0, width / 2, height / 2, Size, -4, 0, Map.Size);
        }

        /// <summary>
        /// 방향, 몇번째 줄, 중심x, 중심y, 이동, 시작, 끝, 반복
        /// </summary>
        void Position(int angle, int line, double width, double height, int move, int start, int end, int count)
        {
            int Size = Length / Map.Size;
            PointF Center_Point = new PointF(Center_Move(width, Seg_X(angle, 0, (move * count))),
                                            Center_Move(height, Seg_Y(angle, 0, (move * count))));//좌표밖 중심 정의
            angle = 300;
            for (int i = start; i < end; i++)
            {
                Point[i + Map.Size, line] = new PointF(Center_Move(Center_Point.X, Seg_X(angle, 0, move)),
                                                            Center_Move(Center_Point.Y, Seg_Y(angle, 0, move)));
                move += Size;
            }
        }
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
        #endregion
    }
}
