using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YasnaSoftwareGroup;

namespace LCTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                var serial = new SerialPort(textBox1.Text, Convert.ToInt32(textBox2.Text));
                serial.Open();
                var arr = LC415Wrapper.WrapTotalPrice("12345678");
                serial.Write(arr, 0, arr.Length);
                serial.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
