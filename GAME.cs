using System.Drawing;
using System.Windows.Forms;

namespace YINSH
{
    public partial class Game : Form
    {
        private static Game Instance = null;

        public static Game GetInstance()
        {
            if (Instance == null)
            {
                Instance = new Game();
            }
            return Instance;
        }

        #region 인스턴스
        readonly Map map = Map.GetInstance();
        readonly Turn turn = Turn.GetInstance();
        readonly Component component = Component.GetInstance();
        readonly Score score = Score.GetInstance();
        #endregion

        #region 이벤트
        public delegate void CheckChangeFile();
        public event CheckChangeFile ChangeFile;
        public delegate void SendRingText(string text);
        public event SendRingText RingText;
        public delegate void SendMarkerText(string text);
        public event SendMarkerText MarkerText;
        public delegate void SendResultText(string text);
        public event SendResultText ResultText;
        #endregion

        #region 변수
        public Panel Board;

        readonly public string[] player = { "White", "Black" };
        string result;

        public bool End()
        {
            for (var i = 0; i < score.Player.Length; i++)
            {
                if (score.Player[i] == score.winscore)
                {
                    result = player[i] + " win!";
                    return true;
                }
            }
            if(component.Marker_Shortage)
            {
                result = "Draw";
                return true;
            }
            return false;
        }
        #endregion

        #region 함수
        public Game()
        {
            InitializeComponent();
            Init();
            Setting();
        }

        private void Init()
        {
            TopLevel = false;
            TopMost = false;
            Board = panel;
            score.winscore = 3;

            Size size = Board.Size;
            int length = (Map.Size * 2) + 1;
            map.System(size, length);
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Border(e);
            Image();
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            component.Cursor_Point = e.Location;
        }

        private void Panel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!End())
                {
                    Rule();
                    ComponentText();
                    if (End())
                    {
                        ResultText(result);
                    }
                }
            }
        }

        public void ComponentText()
        {
            ChangeFile();
            RingText($"Turn {player[turn.User]}");
            MarkerText($"Marker\r\n{component.Marker_Quantity.ToString()}");
        }

        public void Setting()
        {
            Size size = Board.Size;
            int length = (Map.Size * 2) + 1;

            turn.Setting();
            component.Setting(size, length);
            score.Setting(size);
        }

        void Border(PaintEventArgs e)
        {
            Rectangle r = this.ClientRectangle;
            r.Width -= 1;
            r.Height -= 1;
            e.Graphics.DrawRectangle(Pens.Black, r);
        }

        void Image()
        {
            using (Graphics g = Board.CreateGraphics())
            {
                g.DrawImage(map.Board, Point.Empty);
                g.DrawImage(component.Layer, Point.Empty);
                g.DrawImage(score.Layer, Point.Empty);
            }
        }

        void Rule()
        {
            component.System();
            turn.System();
            score.System();

            Board.Invalidate();
        }
        #endregion
    }
}
