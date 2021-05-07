using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 传输线波过程演示程序
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {           
            Form1 f1 = new Form1();
            this.Visible = false;
            f1.ShowDialog();           
            f1.Dispose();
            this.Dispose();
        }

    }
}