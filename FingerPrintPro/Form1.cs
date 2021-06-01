using System;
using System.Windows.Forms;
using Riss.Devices;
using ZDC2911Demo.IConvert;
using ZDC2911Demo.Entity;
using System.Collections.Generic;
using ZDC2911Demo;
using Newtonsoft.Json;
using System.Net.Http;

namespace FingerPrintPro
{
    public partial class Form1 : Form
    {
        private Device device;
        private DeviceConnection deviceConnection;
        private DeviceCommEty deviceEty;
        private static readonly HttpClient client = new HttpClient();

        private List<DateTime> GetDateTimeList()
        {


            DateTime d;
            DateTime d2;

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string yesterday = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
           // yesterday = "2019-03-05"; 
            //today = "2019-03-05";
            if (DateTime.TryParseExact(yesterday, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
            {
                // use d
            }

            if (DateTime.TryParseExact(today, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d2))
            {
                // use d
            }

            List<DateTime> dtList = new List<DateTime>();
            dtList.Add(d);
            dtList.Add(d2);
            return dtList;
        }



        public Form1()
        {
            InitializeComponent();
            Shown += Form1_Shown;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

            getData();
            var timer = new System.Windows.Forms.Timer();
            timer.Tick += (s, ed) => {
                getData();
            };
            timer.Interval = 1000 * 60 * 1;
            timer.Start();
      
        }
 
        public async void getData()
        {

            try
            {
                device = new Device();
                device.DN = 100;
                // device.Password = "100";
                device.Password = "100";
                device.Model = "ZDC2911";
                device.ConnectionModel = 5;//等于5时才能正确加载ZD2911通讯模块
                //device.IpAddress = "192.168.1.225";
                // device.IpAddress = "192.168.1.250";
                device.IpAddress = "192.168.1.101";
                //device.IpPort = 5500;
                device.IpPort = 5500;
                device.CommunicationType = CommunicationType.Tcp;
                this.label1.Text = "Connecting .. \n" + this.label1.Text;
                deviceConnection = DeviceConnection.CreateConnection(ref device);
                if (deviceConnection.Open() > 0)
                {
                    this.label1.Text = "Connected  \n" + this.label1.Text;

                    deviceEty = new DeviceCommEty();
                    deviceEty.Device = device;
                    deviceEty.DeviceConnection = deviceConnection;

                    object extraProperty = new object();
                    object extraData = new object();
                    extraData = Global.DeviceBusy;

                    try
                    {
                        List<DateTime> dtList = GetDateTimeList();
                        bool result = deviceConnection.SetProperty(DeviceProperty.Enable, extraProperty, device, extraData);
                        extraProperty = false;
                        extraData = dtList;
                        result = deviceConnection.GetProperty(DeviceProperty.AttRecordsCount, extraProperty, ref device,
                            ref extraData);
                        if (false == result)
                        {
                            this.label1.Text = "Get All Glog Fail \n" + this.label1.Text;

                         //   MessageBox.Show("Get All Glog Fail", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int recordCount = (int)extraData;
                        /*  if (0 == recordCount)
                          {//为0时说明没有新日志记录
                              lvw_GLogList.Items.Clear();
                              return;
                          }*/

                        List<bool> boolList = new List<bool>();
                        boolList.Add(false);//所有日志
                        boolList.Add(false);//清除新日志标记，false则不清除
                        extraProperty = boolList;
                        extraData = dtList;
                        result = deviceConnection.GetProperty(DeviceProperty.AttRecords, extraProperty, ref device, ref extraData);
                        if (result)
                        {

                            Console.WriteLine(result);
                           
                            this.label1.Text = "Posting data \n" + this.label1.Text;

                            List<Record> recordList = (List<Record>)extraData;

                            var json = JsonConvert.SerializeObject(recordList);

                            var values = new Dictionary<string, string>
                                    {
                                       { "data", json } 
                                    };

                            var content = new FormUrlEncodedContent(values);

                            var response = await client.PostAsync("http://192.168.1.100/controller/Attendence/index", content);

                            var responseString = await response.Content.ReadAsStringAsync();

                            this.label1.Text = responseString+" \n" + this.label1.Text;

                            Console.WriteLine(responseString);
                            Console.WriteLine("--------------------- \n");
                            Console.WriteLine(json);
                            Console.WriteLine("--------------------- \n");
                            

                        }
                        else
                        {
                            this.label1.Text = "Get All log Fail \n" + this.label1.Text;
                           // MessageBox.Show("Get All Glog Fail", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.label1.Text = ex.Message + " \n" + this.label1.Text;

                    }
                    finally
                    {
                        extraData = Global.DeviceIdle;
                        deviceConnection.SetProperty(DeviceProperty.Enable, extraProperty, device, extraData);
                        closeConnection();
                    }

                }
                else
                {
                    this.label1.Text = "Connect Device Fail \n" + this.label1.Text;
                    closeConnection();
                    getData();
                }


            }
            catch (Exception ex)
            {
                this.label1.Text = ex.Message + " \n" + this.label1.Text;
                closeConnection();
                getData();
            }

            this.label1.Text = "sleeping... \n" + this.label1.Text;
        }

        public void closeConnection()
        {
            deviceConnection.Close();
            device = null;
            deviceConnection = null;
            deviceEty = null;
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

     

    }
}
