using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;


namespace bme_system_design_viewer2
{
    public partial class Form1 : Form
    {
        SerialPort sPort;
        int[] data_buff = new int[200];
        static int buffsize = 50*120, moving_n = 5;
        double[] input_Data_1 = new double[buffsize];
        double[] input_Data_2 = new double[buffsize];
        double[] moving_avg_data1 = new double[buffsize - moving_n + 1];
        double[] moving_avg_data2 = new double[buffsize - moving_n + 1];
        public double[] input_Draw_1 = new double[buffsize];
        public double[] input_Draw_2 = new double[buffsize];

        int start_byte = 0;
        int start_flag = 0;
        int data_count = 0;
        int buff_count = 1;
        int flexon_count,flexon = 0;
        double pre_ref_sum1, pre_ref_sum2, ref_mean1, ref_mean2 = 0;
        int Data_1, Data_2, flex_signal;
        double threshold_ref_mag = 1.5;
        int version = 1;//1.BenchPress 2.SideLateralRaise
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void scope2_Click(object sender, EventArgs e)
        {

        }

        private void SPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            while (sPort.BytesToRead > 0)
            {
                if (sPort.IsOpen)
                {
                    if (start_flag == 0)
                    {
                        start_byte = sPort.ReadByte();
                        buff_count++;
                    }
                }

                if (start_byte == 0x81)
                {

                    start_flag = 1;

                    data_buff[data_count] = sPort.ReadByte();

                    data_count++;

                    if (data_count == 4)
                    {
                        Data_1 = ((data_buff[0] & 0x7f) << 7) + (data_buff[1] & 0x7f) - 7000 + 4500;
                        Data_2 = ((data_buff[2] & 0x7f) << 7) + (data_buff[3] & 0x7f) - 7000 + 4500;
                        flex_signal = data_buff[4];
                        start_flag = 2;
                        data_count = 0;
                    }

                    if (start_flag == 2)
                    {
                        for (int i = buffsize - 1; i > buffsize - buff_count - 1; --i)
                        {
                            input_Data_1[i] = input_Data_1[i + 1];
                            input_Data_2[i] = input_Data_2[i + 1];
                        }
                        input_Data_1[buffsize - 1] = Data_1;
                        input_Data_2[buffsize - 1] = Data_2;
                        //big noise rejecting
                        /*if (input_Data_1[buffsize - 1] >= 10*input_Data_1[buffsize - 2])
                        {
                            input_Data_1[buffsize - 1] = input_Data_1[buffsize - 2];
                        }
                        if (input_Data_2[buffsize - 1] >= 10 * input_Data_2[buffsize - 2])
                        {
                            input_Data_2[buffsize - 1] = input_Data_2[buffsize - 2];
                        }*/
                        input_Draw_1 = input_Data_1;
                        input_Draw_2 = input_Data_2;

                        start_flag = 0;
                    }
                }
                if ((flex_signal != 0x00) && (flexon == 0))
                {
                    flexon_count++;
                    flexon = 1;
                }
                else if(flex_signal == 0x00)
                {
                    flexon = 0;
                }
                //signal processing
                //make reference using data of first break time
                if (flexon_count == 0)
                {
                    pre_ref_sum1 += input_Data_1[buffsize - 1];
                    pre_ref_sum2 += input_Data_2[buffsize - 1];
                }

                if ((flexon_count == 1)&&(ref_mean1 == 0))
                {
                    ref_mean1 = pre_ref_sum1 / buff_count;
                    ref_mean2 = pre_ref_sum2 / buff_count;
                }

                //moving average filter
                if (buff_count >=moving_n)
                {
                    for (int i = 0;i < moving_n;i++)
                    {
                        moving_avg_data1[buffsize - buff_count + 1] += input_Data_1[buffsize - buff_count + i + 1];
                        moving_avg_data2[buffsize - buff_count + 1] += input_Data_2[buffsize - buff_count + i + 1];
                    }
                    moving_avg_data1[buffsize - buff_count + 1] = moving_avg_data1[buffsize - buff_count + 1] / moving_n;
                    moving_avg_data2[buffsize - buff_count + 1] = moving_avg_data2[buffsize - buff_count + 1] / moving_n;
                }


                if (flex_signal == 0x00)
                {
                    Image none = Image.FromFile(@"C:\Users\Admin\Desktop\bme_system_design_viewer2\none.png");
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Image = none.GetThumbnailImage(150, 150, null, IntPtr.Zero);
                }else 
                {
                    if (moving_avg_data1[buffsize - buff_count + 1] >= threshold_ref_mag * ref_mean1)
                    {
                        if (moving_avg_data2[buffsize - buff_count + 1] >= threshold_ref_mag * ref_mean2)
                        {
                            if (version == 1)
                            {
                                Image PecMaj_UpTra = Image.FromFile(@"C:\muscles\Pectoralis Major Upper Trapezius.png");
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                                pictureBox1.Image = PecMaj_UpTra.GetThumbnailImage(150, 150, null, IntPtr.Zero);
                            }else
                            {
                                Image AntDel_UpTra = Image.FromFile(@"C:\Users\Admin\Desktop\bme_system_design_viewer2\Anterior Deltoid Upper Trapezius.png");
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                                pictureBox1.Image = AntDel_UpTra.GetThumbnailImage(150, 150, null, IntPtr.Zero);
                            }
                        }else
                        {
                            if (version == 1)
                            {
                                Image PecMaj = Image.FromFile(@"C:\Users\Admin\Desktop\bme_system_design_viewer2\Pectoralis Major.png");
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                                pictureBox1.Image = PecMaj.GetThumbnailImage(150, 150, null, IntPtr.Zero);
                            }else
                            {
                                Image AntDel = Image.FromFile(@"C:\Users\Admin\Desktop\bme_system_design_viewer2\Anterior Deltoid.png");
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                                pictureBox1.Image = AntDel.GetThumbnailImage(150, 150, null, IntPtr.Zero);
                            }
                        }
                    }else if (moving_avg_data2[buffsize - buff_count + 1] >= threshold_ref_mag * ref_mean2)
                    {
                        Image UpTra = Image.FromFile(@"C:\Users\Admin\Desktop\bme_system_design_viewer2\Upper Trapezius.png");
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox1.Image = UpTra.GetThumbnailImage(150, 150, null, IntPtr.Zero);
                    }else
                    {
                        Image none = Image.FromFile(@"C:\Users\Admin\Desktop\bme_system_design_viewer2\none.png");
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox1.Image = none.GetThumbnailImage(150, 150, null, IntPtr.Zero);
                    } 
                }
            }
        }

        private void On_timer1(object sender, EventArgs e)
        {
            scope1.Channels[0].Data.SetYData(input_Draw_1);
            scope2.Channels[0].Data.SetYData(input_Draw_2);
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (null == sPort)
                {
                    sPort = new SerialPort();
                    sPort.DataReceived += new SerialDataReceivedEventHandler(SPort_DataReceived);

                    sPort.PortName = cboPortName.SelectedItem.ToString();
                    sPort.Parity = Parity.None;
                    sPort.StopBits = StopBits.One;
                    sPort.Open();
                }

                if (sPort.IsOpen)
                {
                    btnOpen.Enabled = false;
                    btnClose.Enabled = true;
                }
                else
                {
                    btnOpen.Enabled = true;
                    btnClose.Enabled = false;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            if (null != sPort)
            {
                if (sPort.IsOpen)
                {
                    sPort.Close();
                    sPort.Dispose();
                    sPort = null;
                }
            }

            btnOpen.Enabled = true;
            btnClose.Enabled = false;

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnOpen.Enabled = true;
            btnClose.Enabled = false;

            cboPortName.BeginUpdate();
            foreach (string comport in SerialPort.GetPortNames())
            {
                cboPortName.Items.Add(comport);
            }
            cboPortName.EndUpdate();

            cboPortName.SelectedItem = "COM14";

            CheckForIllegalCrossThreadCalls = false;

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != sPort)
            {
                if (sPort.IsOpen)
                {
                    sPort.Close();
                    sPort.Dispose();
                    sPort = null;
                }
            }
        }
    }
}
