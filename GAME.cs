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

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            if (Start)
            {
                Image();
            }
        }

        private void Panel_MouseClick(object sender, MouseEventArgs e)
        {
            if (Start)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (!End())
                    {
                        Rule();
                    }
                    TextTest();
                }
            }
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (Start)
            {
                component.Preview(e.Location);
            }

            // Test Label
            label1.Text = component.Preview_Text;
        }

        private void Button1_Click(object sender, System.EventArgs e)
        {
            Start = !Start;
            panel.Refresh();
            if (Start == true)
            {
                comboBox1.Enabled = false;
                button1.Text = "Give Up";
            }
            if (Start == false)
            {
                comboBox1.Enabled = true;
                button1.Text = "Start";
            }
        }
    }
}
