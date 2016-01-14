using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Security.Cryptography;
using System.IO;

namespace Hx.AutoCgs
{
    public partial class Form1 : Form
    {
        private SerialPort comm = new SerialPort();
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。  
        private long received_count = 0;//接收计数  
        private long send_count = 0;//发送计数  

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化下拉串口名称列表框  
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            cbxPorts.Items.AddRange(ports);
            cbxPorts.SelectedIndex = cbxPorts.Items.Count > 0 ? 0 : -1;
            cbxBaudrate.SelectedIndex = cbxBaudrate.Items.IndexOf("9600");
            //初始化SerialPort对象  
            comm.NewLine = "/r/n";
            comm.RtsEnable = true;//根据实际情况吧。  
            //添加事件注册  
            comm.DataReceived += comm_DataReceived;
            comm.Close();
        }
        private void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = comm.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致  
            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据  
            received_count += n;//增加接收计数  
            comm.Read(buf, 0, n);//读取缓冲数据  
            builder.Clear();//清除字符串构造器的内容  
            //因为要访问ui资源，所以需要使用invoke方式同步ui。  
            this.Invoke((EventHandler)(delegate
            {
                    //直接按ASCII规则转换成字符串  
                    builder.Append(Encoding.ASCII.GetString(buf));
                //追加的形式添加到文本框末端，并滚动到最后。  
                this.txtContent.AppendText(builder.ToString());
            }));
        }

        private void btnScan_Click(object sender, EventArgs e)
        {

            comm.PortName = cbxPorts.Text;
            comm.BaudRate = int.Parse(cbxBaudrate.Text);
            try
            {
                comm.Open();
            }
            catch (Exception ex)
            {
                //捕获到异常信息，创建一个新的comm对象，之前的不能用了。  
                comm = new SerialPort();
                //现实异常信息给客户。  
                MessageBox.Show(ex.Message);
            }
        }
    }
}
