using System;
using System.Windows.Forms;

namespace SeewoPCEnhancedAssistant
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Show(); // 1p
            pictureBox2.Hide();// 2p
            pictureBox3.Hide();// 3p
            button1.Show();//down 2p
            button2.Hide();//down 3p
            button4.Hide();//up 2p
            button5.Hide();//up 1p
            label1.Show();//1p Text
            label2.Hide();//2p Text
            label3.Hide();//3p Text
            //MinimumSize = new Size(618, 724);
            //MaximumSize = new Size(618, 724);
        }
        private void PictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void Button1_Click(object sender, EventArgs e) // 展示第二张图片
        {
            pictureBox1.Hide();// 隐藏第一张图片
            pictureBox2.Show();// 展示第二张
            button1.Hide();// 隐藏按钮1-2
            button2.Show();//显示按钮2-3
            button4.Show();//显示按钮2-1
            label1.Hide();//隐藏文字1
            label2.Show();//展示文字2
        }
        private void Button2_Click(object sender, EventArgs e)//展示第三张图片
        {
            pictureBox2.Hide();//隐藏第二张
            pictureBox3.Show();//展示第三张
            button2.Hide();//隐藏2-3
            button4.Hide();//隐藏2-1
            button5.Show();//展示3-2
            label2.Hide();//隐藏文字2
            label3.Show();//展示文字3
        }

        private void Button5_Click(object sender, EventArgs e)//切换第二张 3>2
        {
            pictureBox1.Hide();
            pictureBox2.Show();//展示图片2
            pictureBox3.Hide();//隐藏图片3

            button2.Show();//展示2-3
            button5.Hide();//隐藏3-2
            button4.Show();//展示2-1

            label1.Hide();
            label2.Show();
            label3.Hide();

        }
        private void Button4_Click(object sender, EventArgs e)//切换第一张
        {
            pictureBox1.Show();
            pictureBox2.Hide();
            pictureBox3.Hide();
            button2.Hide();//2-3
            button5.Hide();//3-2
            button4.Hide();//2-1
            button1.Show();//1-2

            label3.Hide();
            label2.Hide();
            label1.Show();
        }

    }
}
