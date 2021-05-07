using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using System.Threading;

namespace 传输线波过程演示程序
{
    public partial class Form3 : Form
    {
        double[,] Vrec, Irec;
        double dt, dz, Volt,Curr;
        double[,] LP;
        Load ZL;
        Sourse SV;
        public Form3(double[,] Vrec1, double[,] Irec1, double dt1, double dz1, double Volt1, double Curr1, double[,] LP1, Load ZL1, Sourse SV1)
        {
            InitializeComponent();
            Vrec = Vrec1;
            Irec = Irec1;
            dt = dt1;
            dz = dz1;
            Volt = Volt1;
            Curr = Curr1;
            LP = LP1;
            ZL = ZL1;
            SV = SV1;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Image img = new Bitmap(670, 200);
            pictureBox9.Image = img;
            Graphics g = Graphics.FromImage(pictureBox9.Image);
            g.Clear(Color.White);
            Pen p1 = new Pen(System.Drawing.Color.Blue, 4);
            SolidBrush sb1 = new SolidBrush(Color.Black);
            Font ft1 = new Font("宋体", 11);

            int NC = LP.GetLength(0);
            double Lsum = 0;
            for (int i = 0; i < NC; i++) Lsum += LP[i, 0];

            int kst = 26;

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
                g.DrawLine(p1, kst, 60, kst + System.Convert.ToInt16(LP[i, 0] / Lsum * 600), 60);
                g.DrawString("Zc=" + Convert.ToString(Convert.ToInt16(Math.Sqrt(LP[i, 1] / LP[i, 2]))) + "Ω", ft1, sb1, new PointF(kst + System.Convert.ToInt16(LP[i, 0] / Lsum * 170), 35));
                kst += System.Convert.ToInt16(LP[i, 0] / Lsum * 600);

            }
            switch (ZL.Type) //负载端状况文字描述
            {
                case 0:
                    g.DrawString("负载端接电阻", ft1, sb1, new PointF(510, 100));
                    g.DrawString(Convert.ToString(ZL.Value) + " Ω", ft1, sb1, new PointF(517, 120));
                    break;
                case 1:
                    g.DrawString("负载端短路", ft1, sb1, new PointF(510, 100));
                    break;
                default:
                    g.DrawString("负载端开路", ft1, sb1, new PointF(510, 100));
                    break;
            }

            switch (SV.Type) //源端状况文字描述
            {
                case 0:
                    g.DrawString("直流电压源", ft1, sb1, new PointF(50, 100));
                    break;
                case 1:
                    g.DrawString("高斯脉冲源", ft1, sb1, new PointF(50, 100));
                    break;
                case 2:
                    g.DrawString("双指数脉冲源", ft1, sb1, new PointF(50, 100));
                    break;
                default:
                    g.DrawString("正弦周期源", ft1, sb1, new PointF(50, 100));
                    break;
            }
            
            
            //电压绘图
            Image img1 = new Bitmap(600, 200);
            pictureBox1.Image = img1;
            Graphics g1 = Graphics.FromImage(pictureBox1.Image);
            g1.Clear(Color.White);
            //电流绘图
            Image img2 = new Bitmap(600, 200);
            pictureBox2.Image = img2;
            Graphics g2 = Graphics.FromImage(pictureBox2.Image);
            g2.Clear(Color.White);

            //坐标绘图
            Image img3 = new Bitmap(23, 200);
            pictureBox3.Image = img3;
            Graphics g3 = Graphics.FromImage(pictureBox3.Image);
            g3.Clear(Color.White);
            Image img4 = new Bitmap(23, 200);
            pictureBox4.Image = img4;
            Graphics g4 = Graphics.FromImage(pictureBox4.Image);
            g4.Clear(Color.White);
            Image img5 = new Bitmap(23, 200);
            pictureBox5.Image = img5;
            Graphics g5 = Graphics.FromImage(pictureBox5.Image);
            g5.Clear(Color.White);
            Image img6 = new Bitmap(23, 200);
            pictureBox6.Image = img6;
            Graphics g6 = Graphics.FromImage(pictureBox6.Image);
            g6.Clear(Color.White);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            numericUpDown1.Enabled = false;
            //电压绘图
            Image img1 = new Bitmap(600, 200);
            pictureBox1.Image = img1;
            Graphics g1 = Graphics.FromImage(pictureBox1.Image);
            g1.Clear(Color.White);
            //电流绘图
            Image img2 = new Bitmap(600, 200);
            pictureBox2.Image = img2;
            Graphics g2 = Graphics.FromImage(pictureBox2.Image);
            g2.Clear(Color.White);

            //坐标
            Image img3 = new Bitmap(23, 200);
            pictureBox3.Image = img3;
            Graphics g3 = Graphics.FromImage(pictureBox3.Image);
            g3.Clear(Color.White);
            Image img4 = new Bitmap(23, 200);
            pictureBox4.Image = img4;
            Graphics g4 = Graphics.FromImage(pictureBox4.Image);
            g4.Clear(Color.White);
            Image img5 = new Bitmap(23, 200);
            pictureBox5.Image = img5;
            Graphics g5 = Graphics.FromImage(pictureBox5.Image);
            g5.Clear(Color.White);
            Image img6 = new Bitmap(23, 200);
            pictureBox6.Image = img6;
            Graphics g6 = Graphics.FromImage(pictureBox6.Image);
            g6.Clear(Color.White);
           

            Pen p1 = new Pen(System.Drawing.Color.Red, 2);
            Pen p2 = new Pen(System.Drawing.Color.Blue, 2);
            SolidBrush sb1 = new SolidBrush(Color.Black);
            Font ft1 = new Font("微软雅黑", 11);           

            int NDZ, NDT;
            NDZ = Irec.GetLength(1);
            NDT = Irec.GetLength(0);
            Point[] pt1 = new Point[NDZ + 1];
            Point[] pt2 = new Point[NDZ];
            double Vmax = 0,Vmin=0,Imax = 0,Imin=0;
            double Iscale,Vscale;
            //寻找最大最小值
            for (int i = 0; i < NDT; i++)
            {
                for (int j = 0; j < NDZ + 1; j++)
                {
                    if (Vrec[i, j] > Vmax)
                        Vmax = Vrec[i, j];
                    if (Vrec[i, j] < Vmin)
                        Vmin = Vrec[i, j];
                }
               for (int j=0;j<NDZ;j++)
                {
                    if (Irec[i,j]>Imax)
                        Imax=Irec[i,j];
                    if (Irec[i,j]<Imin)
                        Imin=Irec[i,j];
                }
            }
            Vscale=200/(Vmax-Vmin)/1.1;
            Iscale=200/(Imax-Imin)/1.1;

            //电压坐标
            for (int i = 0; i < 4; i++)
            {
                if (Vmax >= i * Volt)
                {
                    g3.DrawString(Convert.ToString(i), ft1, sb1, new PointF(7, (float)(182 - (Volt * i - Vmin) * Vscale)));
                    g4.DrawString(Convert.ToString(i), ft1, sb1, new PointF(5, (float)(182 - (Volt * i - Vmin) * Vscale)));
                }
            }
            for (int i = -1; i >-4; i--)
            {
                if (Vmin <= i * Volt)
                {
                    g3.DrawString(Convert.ToString(i), ft1, sb1, new PointF(7, (float)(182 - (Volt * i - Vmin) * Vscale)));
                    g4.DrawString(Convert.ToString(i), ft1, sb1, new PointF(5, (float)(182 - (Volt * i - Vmin) * Vscale)));
                }
            }
            

            //电流坐标
            for (int i = 0; i < 4; i++)
            {
                if (Imax >= i * Curr)
                {
                    g5.DrawString(Convert.ToString(i), ft1, sb1, new PointF(7, (float)(182 - (Curr * i - Imin) * Iscale)));
                    g6.DrawString(Convert.ToString(i), ft1, sb1, new PointF(5, (float)(182 - (Curr * i - Imin) * Iscale)));
                }
            }
            for (int i = -1; i > -4; i--)
            {
                if (Imin <= i * Curr)
                {
                    g5.DrawString(Convert.ToString(i), ft1, sb1, new PointF(7, (float)(182 - (Curr * i - Imin) * Iscale)));
                    g6.DrawString(Convert.ToString(i), ft1, sb1, new PointF(6, (float)(182 - (Curr * i - Imin) * Iscale)));
                }
            }

            //帧间隔
            int Tinterp = 0;
            Tinterp = 1000 / (Convert.ToInt16(numericUpDown1.Value) + 1);

            //绘图
                for (int j = 0; j < NDT; j = j + 30)
                {
                    g1.Clear(Color.White);
                    for (int i = 0; i <= NDZ; i++)
                        pt1[i] = new Point((int)(i * 600 / NDZ), (int)(192 - (Vrec[j, i]-Vmin)* Vscale));
                    g1.DrawCurve(p1, pt1);

                    g2.Clear(Color.White);
                    for (int i = 0; i < NDZ; i++)
                        pt2[i] = new Point((int)(i * 600 / NDZ), (int)(192 - (Irec[j, i]-Imin) * Iscale));
                    g2.DrawCurve(p2, pt2);

                    this.Refresh();
                    Thread.Sleep(Tinterp);
                }

            button1.Enabled = true;
            numericUpDown1.Enabled = true;
        }

    }
}