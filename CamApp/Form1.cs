using MjpegProcessor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamApp
{
    public partial class MediaForm : Form
    {
        private int _IpCamIndex;
        private int _WebIndex;
        private int WeatherIndex = 0;
        private List<string> _Mpglist = new List<string>();
        private MjpegDecoder _mjpeg;
        private int MpgIndex = -1;
        private bool isPlay;

         int Mov ; 
         int MouseDownX;
         int MouseDownY;

        public MediaForm()
        {
            InitializeComponent();
            CenterToScreen();
            this.TopMost = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          //  CenterToScreen();
            // this.liveMedia.WindowState = FormWindowState.Maximized;

            _mjpeg = new MjpegDecoder();
            _mjpeg.FrameReady += mjpeg_FrameReady;
            _mjpeg.Error += mjpeg_Error;

            this.LoadConfig();
        }

        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
            liveMedia.Image = e.Bitmap;
        }
        void mjpeg_Error(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void LoadConfig()
        {
            XmlDocument document = new XmlDocument();

            document.Load("config.xml");
            XmlNode node = document.SelectSingleNode("root");

            if (node != null)
            {
                //-----------------------------------LiveCam
                XmlNode node2 = node.SelectSingleNode("IPCam");
                if (node2 != null)
                {
                    this._IpCamIndex = int.Parse(node2.Attributes["Index"].Value);
                    this.SelectIpCamera(this._IpCamIndex);
                }
                //-----------------------------------PlayMp4
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (node.ChildNodes[i].Name == "MPG")
                    {
                        string item = node.ChildNodes[i].Attributes["Name"].Value;
                        this._Mpglist.Add(item);
                    }
                }
                this.PlayMp4();
                //----------------------------------------Web
                XmlNode node3 = node.SelectSingleNode("Web");
                if (node3 != null)
                {
                    this._WebIndex = int.Parse(node3.Attributes["Index"].Value);
                    this.PlayWeb(this._IpCamIndex);
                }
            }
        }

        //Live Camera
        private void SelectIpCamera(int Index)
        {
            switch (Index)
            {
                case 1:
                    this._mjpeg.ParseStream(new Uri("http://210.241.53.147:80/abs2mjpg/mjpg?camera=1&resolution=352x240"));
                break;
                case 2:
                    this._mjpeg.ParseStream(new Uri("http://210.241.53.147:80/abs2mjpg/mjpg?camera=2&resolution=352x240"));
                break;
                case 3:
                    this._mjpeg.ParseStream(new Uri("http://210.241.53.147:80/abs2mjpg/mjpg?camera=3&resolution=352x240"));
                break;
                case 4:
                    this._mjpeg.ParseStream(new Uri("http://210.241.53.147:80/abs2mjpg/mjpg?camera=4&resolution=352x240"));
                break;
                case 5:
                    this._mjpeg.ParseStream(new Uri("http://210.241.53.147:80/abs2mjpg/mjpg?camera=5&resolution=352x240"));
                break;
                case 6:
                    this._mjpeg.ParseStream(new Uri("http://210.241.53.147:80/abs2mjpg/mjpg?camera=8&resolution=352x240"));
                break;
            }
        }

        //Play Mp4
        private void PlayMp4()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int num = random.Next(0,this._Mpglist.Count);
            while (this.MpgIndex ==num)
            {
                num = random.Next(0, this._Mpglist.Count);
            }
            this.MpgIndex = num;
            string str = this._Mpglist[num];

            try
            {
                this.axWindowsMediaPlayer.URL = str;
                this.axWindowsMediaPlayer.uiMode = "none";
            }
            catch (NullReferenceException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void axWindowsMediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            try
            {
                switch (this.axWindowsMediaPlayer.playState)
                {
                    case  WMPLib.WMPPlayState.wmppsReady:
                        this.axWindowsMediaPlayer.Ctlcontrols.play();
                    break;
                    case WMPLib.WMPPlayState.wmppsPlaying:
                        this.isPlay = true;
                    break;
                    case WMPLib.WMPPlayState.wmppsMediaEnded :

                        if (this.isPlay)
                        {
                            this.isPlay = false;
                            this.PlayMp4();
                        }
                         
                   break;
                }
            }
            catch (Exception)
            {
            }
        }

        private void axWindowsMediaPlayer_Enter(object sender, EventArgs e)
        {

        }


        //Web
        private void PlayWeb(int Index)
        {
            switch (Index)
            {
                case 1:
                    this.webBrowser.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/E035.htm");
                break;
                case 2:
                        switch (WeatherIndex)
                        {
                             case 0:
                                 this.label.Text = "武陵";
                                 this.webBrowser.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/E035.htm");
                                 this.webBrowser2.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/D078.htm");
                                 this.webBrowser.Visible = true;
                                 this.webBrowser2.Visible = false;
                            break;
                             default:
                                 this.label.Text = "雪山";
                                this.webBrowser.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/E035.htm");
                                this.webBrowser2.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/D078.htm");
                                this.webBrowser.Visible = false;
                                this.webBrowser2.Visible = true;
                            break;
                        }
                break;
                case 3:
                   // this.label.Text = "汶水遊客中心";
                    this.webBrowser.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/E019.htm");
                    break;
                case 4:
                    //this.lb_Title.Text = "雪見遊客中心";
                    this.label.Text = "雪見遊客中心";
                    this.webBrowser.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/E021.htm");
                    break;
                case 5:
                    this.webBrowser.Visible = true;
                    this.webBrowser2.Visible = false;
                    this.webBrowser.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/E021.htm");
                    break;
                case 6:
                    switch (WeatherIndex)
                    {
                        case 0:
                            this.label.Text = "觀霧";
                            this.webBrowser.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/E020.htm");
                            this.webBrowser2.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/D072.htm");
                            this.webBrowser.Visible = true;
                            this.webBrowser2.Visible = false;
                            break;
                        default:
                            this.label.Text = "大壩尖山";
                            this.webBrowser.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/E020.htm");
                            this.webBrowser2.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/D072.htm");
                            this.webBrowser.Visible = false;
                            this.webBrowser2.Visible = true;
                            break;
                    }
                    break;
                case 7:
                    this.webBrowser.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/E072.htm");
                    break;
                case 8:
                    this.webBrowser.Navigate("https://www.cwb.gov.tw/V7/forecast/entertainment/7Day/E078.htm");
                    break;
            }

            WeatherIndex++;
            if (WeatherIndex >= 2) WeatherIndex = 0;
        }

        private void WebTimer_Tick(object sender, EventArgs e)
        {
            this.PlayWeb(this._IpCamIndex);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Mov  = 1;
            MouseDownX = e.X;
            MouseDownY = e.Y;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mov == 1)
            {
                this.SetDesktopLocation(MousePosition.X - MouseDownX, MousePosition.Y - MouseDownY);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Mov = 0;
        }

        private void liveMedia_Click(object sender, EventArgs e)
        {
            //aaa
        }
    }


}
