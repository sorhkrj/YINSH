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

            // Test Label
            label1.Text = string.Empty;
            TextTest();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            Image();
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            component.Preview(e.Location);

            // Test Label
            label1.Text = component.Preview_Text;
        }

        private void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Rule();
            TextTest();
        }
    }
}
