using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Video.VFW;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Indv
{
    public partial class INDForm : Form
    {
        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;
        public AVIWriter writer;

        public INDForm()
        {
            InitializeComponent();
        }

        private void getCamList()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                comboBox1.Items.Clear();
                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                DeviceExist = true;
                foreach (FilterInfo device in videoDevices)
                {
                    comboBox1.Items.Add(device.Name);
                }
                comboBox1.SelectedIndex = 0; 
            }
            catch (ApplicationException)
            {
                DeviceExist = false;
                comboBox1.Items.Add("No capture device on your system");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            getCamList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            writer = new AVIWriter("DIB ");
            writer.Open("video.avi", 160, 120);
            Bitmap image = new Bitmap(160, 120);
            videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
#pragma warning disable CS0612
            videoSource.DesiredFrameSize = new Size(160, 120);
#pragma warning restore CS0612 
            videoSource.Start();
        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs) 
        {
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            writer.AddFrame(img);
        }


        private void CloseVideoSource()  
        {
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CloseVideoSource();
            writer.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {

                if (MessageBox.Show("Припинити роботу застосунку?",
                "Припинити роботу", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    Application.Exit();
        }


    }
}
