using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace 传输线波过程演示程序
{
    public partial class Form1 : Form
    {
        public int NC;
        Sourse SV = new Sourse();
        double[,] LP;
        Load ZL = new Load();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //数据输入表格初始化
            dataGridView1.Rows.Add(4);
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            comboBox2.SelectedIndex = 0;
            comboBox4.SelectedIndex = 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //清空
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 6; j++)
                    dataGridView1.Rows[i].Cells[j].Value = " ";

            //填写默认值
            NC = Convert.ToInt16(comboBox1.SelectedIndex) + 1;
            for (int i = 0; i < NC; i++)
                dataGridView1.Rows[i].Cells[0].Value = Convert.ToInt16(i + 1);
            for (int i = 0; i < NC; i++)
            {
                dataGridView1.Rows[i].Cells[1].Value = "10";
                dataGridView1.Rows[i].Cells[2].Value = "1.1e-6";
                dataGridView1.Rows[i].Cells[3].Value = "10e-12";
                dataGridView1.Rows[i].Cells[4].Value = "0";
                dataGridView1.Rows[i].Cells[5].Value = "0";
            }
            for (int i = NC; i < 5; i++)
            {
                for (int j = 0; j <= 5; j++)
                    dataGridView1.Rows[i].Cells[j].Value = "/";
            }

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            //激励源参数输入
            textBox1.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
            textBox7.Visible = false;
            label2.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label14.Visible = false;
            label16.Visible = false;

            //高斯脉冲
            if (comboBox4.SelectedIndex == 1)
            {
                textBox4.Visible = true;
                textBox1.Visible = true;
                label2.Visible = true;
                label10.Visible = true;
            }

            //双指数脉冲
            if (comboBox4.SelectedIndex == 2)
            {
                textBox6.Visible = true;
                textBox7.Visible = true;
                label14.Visible = true;
                label16.Visible = true;
            }

            //正弦周期
            if (comboBox4.SelectedIndex == 3)
            {
                textBox5.Visible = true;
                label11.Visible = true;
                label12.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //读激励源参数           
            SV.Type = comboBox4.SelectedIndex;
            SV.A = System.Convert.ToDouble(textBox3.Text);
            if (SV.Type == 1)
            {
                SV.b = System.Convert.ToDouble(textBox4.Text);
                SV.c = System.Convert.ToDouble(textBox1.Text);
            }
            if (SV.Type == 2)
            {
                SV.alpha = System.Convert.ToDouble(textBox7.Text);
                SV.beta = System.Convert.ToDouble(textBox6.Text);
            }
            if (SV.Type == 3)
            {
                SV.f = System.Convert.ToDouble(textBox5.Text);
            }

            //读仿真参数
            double dt = System.Convert.ToDouble(textBox8.Text);
            SV.dt = dt;
            double Tsim = System.Convert.ToDouble(textBox9.Text);
            int NDZ = System.Convert.ToInt16(textBox10.Text);

            //读传输线参数
            LP = new double[NC, 5];
            for (int i = 0; i < NC; i++)
                for (int j = 0; j < 5; j++)
                    LP[i, j] = System.Convert.ToDouble(dataGridView1.Rows[i].Cells[j + 1].Value);

            //读负载端参数          
            ZL.Type = comboBox2.SelectedIndex;
            if (ZL.Type == 0)
                ZL.Value = System.Convert.ToDouble(textBox2.Text);



            //计算波过程
            double Lsum = 0; //线路总长
            for (int i = 0; i < NC; i++)
                Lsum += LP[i, 0];
            double dz = Lsum / NDZ; //空间步长
            double[] Vpri = new double[NDZ + 1]; //priori 前
            double[] Vpos = new double[NDZ + 1]; //posteri 后
            double[] Ipri = new double[NDZ];
            double[] Ipos = new double[NDZ];

            //电压电流数组初始化
            for (int i = 0; i < NDZ; i++)
            {
                Vpri[i] = 0;
                Ipri[i] = 0;
                Vpos[i] = 0;
                Ipos[i] = 0;
            }
            Vpri[NDZ] = 0;
            Vpos[NDZ] = 0;
            int NDT = Convert.ToInt16(Tsim / dt);
            double[,] Vrec = new double[NDT, NDZ + 1];
            double[,] Irec = new double[NDT, NDZ];

            //时间循环
            for (int i = 1; i <= NDT; i++)
            {
                double t1, t2;
                //源端处理
                Vpos[0] = SV.Volt(i * dt, dt);     //源内阻为0

                //负载端接电阻时处理
                if (ZL.Type == 1)
                    Vpos[NDZ] = 0;
                else
                {
                    if (ZL.Type == 2)
                        ZL.Value = 1e80;
                    t1 = dt / (dz * LP[NC - 1, 2] * ZL.Value);
                    Vpos[NDZ] = ((1 - t1) * Vpri[NDZ] + 2 * dt / dz / LP[NC - 1, 2] * Ipri[NDZ - 1]) / (1 + t1);
                }



                //沿线电压迭代
                for (int j = 1; j < NDZ; j++)
                {
                    double t3 = 0;
                    int t4 = NC - 1;
                    //判定当前点所处的位置
                    for (int k = 0; k < NC; k++)
                    {
                        t3 += LP[k, 0];
                        if (j * dz <= t3)
                        {
                            t4 = k;
                            break;
                        }
                    }

                    t1 = LP[t4, 2] * dz / dt + LP[t4, 4] * dz / 2;
                    t2 = LP[t4, 2] * dz / dt - LP[t4, 4] * dz / 2;
                    Vpos[j] = (t2 * Vpri[j] - (Ipri[j] - Ipri[j - 1])) / t1;
                }
                //前后步数组间的替换
                for (int j = 0; j <= NDZ; j++)
                    Vpri[j] = Vpos[j];

                //沿线电流迭代
                for (int j = 0; j < NDZ; j++)
                {
                    double t3 = 0;
                    int t4 = NC - 1;
                    //判定当前点所处的位置
                    for (int k = 0; k < NC; k++)
                    {
                        t3 += LP[k, 0];
                        if (j * dz + dz / 2 <= t3)
                        {
                            t4 = k;
                            break;
                        }
                    }

                    t1 = LP[t4, 1] * dz / dt + LP[t4, 3] * dz / 2;
                    t2 = LP[t4, 1] * dz / dt - LP[t4, 3] * dz / 2;
                    Ipos[j] = (t2 * Ipri[j] - (Vpri[j + 1] - Vpri[j])) / t1;
                }

                //前后步数组间的替换
                for (int j = 0; j < NDZ; j++)
                    Ipri[j] = Ipos[j];

                //存储计算值
                for (int j = 0; j <= NDZ; j++)
                    Vrec[i - 1, j] = Vpri[j];
                for (int j = 0; j < NDZ; j++)
                    Irec[i - 1, j] = Ipri[j];
            }
            /*StreamWriter MyFile = new StreamWriter("G:\\test.txt", false);
            for (int i = 0; i < NDT; i++)
            {
                for (int j = 0; j < NDZ + 1; j++)
                {
                    MyFile.Write(Vrec[i, j]);
                    MyFile.Write("  ");
                }
                MyFile.Write("\n");
            }
            MyFile.Close();*/
            Form3 f3 = new Form3(Vrec, Irec, dt, dz, SV.A, SV.A / Math.Sqrt(LP[0, 1] / LP[0, 2]), LP, ZL, SV);
            f3.ShowDialog();
            f3.Dispose();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex > 0)
            {
                textBox2.Visible = false;
                label4.Visible = false;
            }
            else
            {
                textBox2.Visible = true;
                label4.Visible = true;
            }
        }

       /* private void button2_Click(object sender, EventArgs e)
        {
            //读激励源参数           
            SV.Type = comboBox4.SelectedIndex;
            SV.A = System.Convert.ToDouble(textBox3.Text);
            if (SV.Type == 1)
            {
                SV.b = System.Convert.ToDouble(textBox4.Text);
                SV.c = System.Convert.ToDouble(textBox1.Text);
            }
            if (SV.Type == 2)
            {
                SV.alpha = System.Convert.ToDouble(textBox7.Text);
                SV.beta = System.Convert.ToDouble(textBox6.Text);
            }
            if (SV.Type == 3)
            {
                SV.f = System.Convert.ToDouble(textBox5.Text);
            }

            //读仿真参数
            double dt = System.Convert.ToDouble(textBox8.Text);
            SV.dt = dt;
            double Tsim = System.Convert.ToDouble(textBox9.Text);
            int NDZ = System.Convert.ToInt16(textBox10.Text);

            //读负载端参数          
            ZL.Type = comboBox2.SelectedIndex;
            if (ZL.Type == 0)
                ZL.Value = System.Convert.ToDouble(textBox2.Text);

            //读传输线参数
            LP = new double[NC, 5];
            for (int i = 0; i < NC; i++)
                for (int j = 0; j < 5; j++)
                    LP[i, j] = System.Convert.ToDouble(dataGridView1.Rows[i].Cells[j + 1].Value);

            //画传输线示意图
            Form2 f2 = new Form2(LP, ZL, SV);
            f2.ShowDialog();
            f2.Dispose();
        }*/


    }

    //激励源信息存储类
    public class Sourse
    {
        public int Type = 0; //类型
        public double A = 0; //幅值
        public double b = 0,c=0; //
        public double f = 0; //频率
        public double alpha = 0, beta = 0; //双指数波形参数
        public double dt = 0;

        public double Volt(double t, double dt)
        {
            double V = 0;

            //直流
            if (this.Type == 0)
            {
                if (t <= (dt * 10))
                    V = this.A * t / dt / 10;
                else
                    V = this.A;
            }

            //高斯脉冲
            if (this.Type == 1)
            {
                V = this.A * Math.Exp(-Math.Pow((t - b) / c, 2));
            }

            //双指数脉冲
            if (this.Type == 2)
            {
                V = A * (Math.Exp(this.alpha * t) - Math.Exp(this.beta * t));
            }

            //正弦周期
            if (this.Type == 3)
            {
                V = A * Math.Sin(2 * Math.PI * this.f * t);
            }

            return V;
        }
    }

    //端接阻抗存储
    public class Load
    {
        public int Type = 0;
        public double Value = 0;
    }

}