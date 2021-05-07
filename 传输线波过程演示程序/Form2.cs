using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace 传输线波过程演示程序
{
    public partial class Form2 : Form
    {
        double [,] LP;
        Load ZL;
        Sourse SV;
        public Form2(double [,] LinePara,Load ZLoad,Sourse SourVolt)
        {
            InitializeComponent();
            LP = LinePara;
            ZL = ZLoad;
            SV = SourVolt;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Image img = new Bitmap(600, 200);
            pictureBox1.Image = img;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
            Pen p1 = new Pen(System.Drawing.Color.Blue,4);
            SolidBrush sb1 = new SolidBrush(Color.Black);
            Font ft1 = new Font("宋体", 11);

            int NC=LP.GetLength(0);
            double Lsum=0;
            for (int i = 0; i < NC; i++) Lsum+=LP[i,0]; 

            int kst=50;

            for (int i = 0; i < NC; i++)
            {
                switch (i) //选择颜色
                {
                    case 1:
                        p1.Color = System.Drawing.Color.Red;
                        break;
                    case 2:
                        p1.Color = System.Drawing.Color.Black;
                        break;
                    case 3:
                        p1.Color = System.Drawing.Color.Yellow;
                        break;
                    case 4:
                        p1.Color = System.Drawing.Color.Green;
                        break;
                    default:
                        break;
                }
                //画线
                g.DrawLine(p1, kst, 60, kst + System.Convert.ToInt16(LP[i, 0] / Lsum * 500), 60);
                g.DrawString("Zc="+Convert.ToString(Convert.ToInt16(Math.Sqrt(LP[i, 1]/LP[i,2])))+"Ω",ft1,sb1,new PointF(kst + System.Convert.ToInt16(LP[i, 0] / Lsum * 170),35));               
                kst += System.Convert.ToInt16(LP[i, 0] / Lsum* 500) ;

                //g.DrawString(Convert.ToString(i + 1), f, q1, new Rectangle(310 + (this.Width - 340) / (int)(Para[NS, 3]) * i, 60, 310 + (this.Width - 340) / (int)(Para[NS, 3]) * i, 50));
            }
            switch (ZL.Type) //负载端状况文字描述
            {
                case 0:
                    g.DrawString("负载端接电阻", ft1, sb1, new PointF(417, 100));
                    g.DrawString(Convert.ToString(ZL.Value)+" Ω", ft1, sb1, new PointF(417, 120));
                    break;
                case 1:
                    g.DrawString("负载端接电容", ft1, sb1, new PointF(417, 100));
                    g.DrawString(Convert.ToString(ZL.Value) + " F", ft1, sb1, new PointF(417, 120));
                    break;
                case 2:
                    g.DrawString("负载端接电感", ft1, sb1, new PointF(417, 100));
                    g.DrawString(Convert.ToString(ZL.Value) + " H", ft1, sb1, new PointF(417, 120));
                    break;
                case 3:
                    g.DrawString("负载端开路", ft1, sb1, new PointF(417, 100));
                    break;
                default:
                    g.DrawString("负载端短路", ft1, sb1, new PointF(417, 100));
                    break;
            }

            switch (SV.Type) //源端状况文字描述
            {
                case 0:
                    g.DrawString("直流电压源", ft1, sb1, new PointF(90, 100));
                    break;
                case 1:
                    g.DrawString("高斯脉冲源", ft1, sb1, new PointF(90, 100));
                    break;
                case 2:
                    g.DrawString("双指数脉冲源", ft1, sb1, new PointF(90, 100));
                    break;
                default:
                    g.DrawString("正弦周期源", ft1, sb1, new PointF(90, 100));
                    break;
            }
            
        }
    }
}