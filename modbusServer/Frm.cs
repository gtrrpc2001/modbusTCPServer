using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modbus.Device;
using System.IO.Ports;
using System.Net.Sockets;
using ModbusSlaveLibrary;
using Modbus.Message;
using lunar;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.AxHost;
using System.Threading;
using Timer = System.Windows.Forms.Timer;
using Modbus.Extensions.Enron;
using System.Xml.Linq;
using System.Net.Http;
using System.Net;
using System.Diagnostics;

namespace deviceTest
{
    public partial class Frm : Form
    {
        TcpClient tcpClient;
        ModbusIpMaster master;
        Timer T;
        string ipAddr = "";
        string tcpPort = "";
        List<Dictionary<string, string>> Tag_dictList;
        clsQuery mysql;

        public Frm()
        {
            InitializeComponent();
            mysql = new clsQuery();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (MessageBox.Show("바로 시작 하시겠습니까?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                ProcessStart();
        }

        private void ProcessStart(bool begin = true)
        {
            if (begin | Tag_dictList is null) SetTagList();

            tcpClient = new TcpClient();

            var dict = Tag_dictList[0];
            dict.TryGetValue("Host", out ipAddr);
            dict.TryGetValue("Port", out tcpPort);
            int.TryParse(tcpPort, out int port);
            SetMasterBegginer(port);
        }

        private void SetTagList()
        {
            DataTable dT = mysql.GetDataTable();
            Tag_dictList = clsDict.GetDictList(dT);
        }

        private void SetMasterBegginer(int port)
        {
            tcpClient.BeginConnect(ipAddr, port, null, null);
            master = ModbusIpMaster.CreateIp(tcpClient);
            var tagAddrArr = clsDict.GetTagAddress(Tag_dictList);
            SetCmbItems(tagAddrArr);
            txtCheck.Text = "Timer working";
            T = new Timer();
            T.Tick += (x, y) => { SetModbusValue(tagAddrArr); };
            T.Interval = 1000;
            T.Start();
        }

        private void SetCmbItems(List<TagWithAddrModel> tagAddrArr) {
            var groupName = "";
            foreach (TagWithAddrModel item in tagAddrArr) {                
                if (groupName != item.group) {
                    groupName = item.group;
                    cmb.Items.Add(groupName);
                }
            }
            if (cmb.Items.Count > 0)
                cmb.SelectedIndex = 0;            
        }
        
        private async void SetModbusValue(List<TagWithAddrModel> tagAddrArr)
        {
            try
            {

                //for (var id = 1; id < 256; id++)
                //{
                //    int i = -1;
                foreach (var tagAddr in tagAddrArr)
                {
                    //i += 1;
                    //if (i == 10) break;                    
                    if (cmb.Text == tagAddr.group) { 

                    ushort addr = (ushort)tagAddr.Address;
                    ushort[] values = master.ReadHoldingRegisters(addr, 1);
                 

                    //var txt = "-- id : " + id + "--   address : " + addr + "---" + values[0];

                    var txt = "address : " + addr + "-- - " + values[0];
                    Console.WriteLine(txt);

                    var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    var prevValue = (double)values[0];

                    double value = (tagAddr.scale == 0 || tagAddr.scale == 1) ? prevValue + tagAddr.offset : (prevValue / tagAddr.scale) + tagAddr.offset;


                    mysql.SetTag_ListUpdate(tagAddr.group, tagAddr.Tag, value, time);
                    //SetAddSecond();
                    mysql.SetDataInsert(tagAddr.eq, tagAddr.Tag, time, value);
                    
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                txtCheck.Text = $"Timer Stop :: error";
                MessageBox.Show(ex.Message);
                T.Stop();
            }
        }

        private async void SetAddSecond()
        {
            //Stopwatch sw = Stopwatch.StartNew();
            //while (sw.IsRunning)
            //{
            //    if (sw.Elapsed.TotalMilliseconds >= 500) sw.Stop();

            //}


        }

        bool btnChecked = false;
      

        private void btnOpenServer_Click(object sender, EventArgs e)
        {
            if (btnChecked == false)
            {
                if (tcpClient != null) tcpClient.Close();
                ProcessStart(false);
                txtCheck.Text = "Timer working";
            }
            else
            {
                txtCheck.Text = "Timer Stop";
                T.Stop();
            }
        }

        private void txtCheck_TextChanged(object sender, EventArgs e)
        {
            if (txtCheck.Text == "Timer working")
            {
                btnChecked = true;
            }
            else
            {
                btnChecked = false;
            }
        }

        #region closedForm        

        private void Frm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        #endregion

        private void cmb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
