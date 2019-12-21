using System.Drawing;
using System.Windows.Forms;

namespace YINSH
{
    /// <summary>
     /// 분할 클래스 진행을 맡은 Main Class
     /// </summary>
    public partial class GAME : Form
    {
        public GAME()
        {
            InitializeComponent();
            Setting();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            Image();
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            component.Cursor_Point = new Point(e.X, e.Y);
            component.Preview();
            label1.Text = component.Preview_Text;
        }

        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Rule();
        }
    }
}
