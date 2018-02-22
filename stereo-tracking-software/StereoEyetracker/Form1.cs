using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;
    using Emgu.CV.UI;
using System.Windows.Forms.DataVisualization.Charting;


namespace StereoEyetracker
{


 

    public partial class Form1 : Form
    {
        // General variables
        bool CapturingProcess = true;
        int iSize;

        // Variables Cam 1
        public static long Timestamp;

        private static readonly CascadeClassifier Classifier = new CascadeClassifier("haarcascade_eye.xml");

        public static PointF PupilLeft = new PointF();
        public static PointF PupilRight = new PointF();
        public static PointF PupilLeftellipse = new PointF();
        public static PointF PupilRightellipse = new PointF();
        public static PointF GlintLeft1 = new PointF();
        public static PointF GlintLeft2 = new PointF();
        public static PointF GlintRight1 = new PointF();
        public static PointF GlintRight2 = new PointF();
        public static PointF Offset = new PointF();

        public static int Sizepupleft = new int();
        public static int Sizepupright = new int();

        public static float Pupilleftwidth = new float();
        public static float Pupilleftheight = new float();

        public static float Pupilrightwidth = new float();
        public static float Pupilrightheight = new float();

        public static float Pupilrightangle = new float();
        public static float Pupilleftangle = new float();

        double treshpupil;
        double treshglint;

        double treshpupil2;
        double treshglint2;

        private double Tresh;
        private double Treshglint;

        private double Treshinc;
        private double Treshglintinc;

        private double Tresh2;
        private double Treshglint2;

        private double Treshinc2;
        private double Treshglintinc2;

        private static StreamWriter logWriter;
        private static string logFilePath;
        private static FileStream fs;

        // object that will be used to synchronize UI & capture thread
        private Object syncRoot = new Object();
        private Object syncRoot2 = new Object();

        // two Bitmap resources that will hold the most recent captured image
        private Image<Bgr, byte> lastBitmap2;
        private Image<Bgr, byte> lastBitmap3;

        private Image<Bgr, byte> lastBitmap5;
        private Image<Bgr, byte> lastBitmap6;

        private Image<Bgr, Byte> Mask;
        private Image<Bgr, Byte> Mask2;

        private Image<Bgr, Byte> Maskcam2;
        private Image<Bgr, Byte> Mask2cam2;

        // a timer that will run on the UI thread, to update the UI periodically
        private System.Windows.Forms.Timer uiUpdateTimer;
        private System.Windows.Forms.Timer uiUpdateTimer2;
        private System.Windows.Forms.Timer uiUpdateTimer3;
        private System.Windows.Forms.Timer uiUpdateTimer4;

       
           
        byte[] pFrame;
            
        byte[] pFrame1;


        Image<Gray, byte> img2;
        Image<Gray, byte> img1;

        Emgu.CV.Cvb.CvBlobDetector bDetect = new Emgu.CV.Cvb.CvBlobDetector();

        Emgu.CV.Cvb.CvBlobDetector bDetect2 = new Emgu.CV.Cvb.CvBlobDetector();



        // Left eye
        Rectangle leftROI = Rectangle.Empty;
        bool leftROIset = false;
        int missingpupleft = 0;
        int missingeye = 0;

        // Right eye
        Rectangle rightROI = Rectangle.Empty;
        bool rightROIset = false;
        int missingpupright = 0;

        // Image processing values
        Point minLoc = new Point(), maxLoc = new Point();
        double minVal = 0, maxVal = 0;

        Image<Gray, byte> imgsmall;
        Image<Gray, byte> bin;
        Image<Gray, byte> bin2;

        Image<Gray, byte> imgsmallr;
        Image<Gray, byte> binr;
        Image<Gray, byte> bin2r;
                
        Lumenera.USB.dll.LucamFrameFormat m_lffFormat;
               
        IntPtr m_hCamera;
        IntPtr m_hCamera1;

        BackgroundWorker backgroundWorker1;
        BackgroundWorker backgroundWorker2;



        // Variables Cam 2
        public static long Timestampcam2;

        private static readonly CascadeClassifier Classifiercam2 = new CascadeClassifier("haarcascade_eye.xml");


        public static PointF PupilLeftcam2 = new PointF();
        public static PointF PupilRightcam2 = new PointF();
        public static PointF PupilLeftellipsecam2 = new PointF();
        public static PointF PupilRightellipsecam2 = new PointF();
        public static PointF GlintLeft1cam2 = new PointF();
        public static PointF GlintLeft2cam2 = new PointF();
        public static PointF GlintRight1cam2 = new PointF();
        public static PointF GlintRight2cam2 = new PointF();

        public static int Sizepupleftcam2 = new int();
        public static int Sizepuprightcam2 = new int();

        public static float Pupilleftwidthcam2 = new float();
        public static float Pupilleftheightcam2 = new float();

        public static float Pupilrightwidthcam2 = new float();
        public static float Pupilrightheightcam2 = new float();

        public static float Pupilrightanglecam2 = new float();
        public static float Pupilleftanglecam2 = new float();

        // Left eye
        Rectangle leftROIcam2 = Rectangle.Empty;
        bool leftROIsetcam2 = false;
        int missingpupleftcam2 = 0;
        int missingeyecam2 = 0;

        // Right eye
        Rectangle rightROIcam2 = Rectangle.Empty;
        bool rightROIsetcam2 = false;
        int missingpuprightcam2 = 0;

        private static StreamWriter logWritercam2;
        private static string logFilePathcam2;
        private static FileStream fscam2;

        Lumenera.USB.dll.LucamFrameFormat m_lffFormatcam2;

        // Image processing values
        Point minLoccam2 = new Point(), maxLoccam2 = new Point();
        double minValcam2 = 0, maxValcam2 = 0;

        Image<Gray, byte> imgsmallcam2;
        Image<Gray, byte> bincam2;
        Image<Gray, byte> bin2cam2;

        Image<Gray, byte> imgsmallrcam2;
        Image<Gray, byte> binrcam2;
        Image<Gray, byte> bin2rcam2;

        double treshpupilcam2;
        double treshglintcam2;

        double treshpupil2cam2;
        double treshglint2cam2;

        private double Tresh2cam2;
        private double Treshglint2cam2;
               
        private double Treshinccam2;
        private double Treshglintinccam2;

        private double Treshcam2;
        private double Treshglintcam2;

        private double Treshinc2cam2;
        private double Treshglintinc2cam2;

        bool Eqhist;

        private double sizeglint1;
        private double sizeglint2;
        private double sizepup;

        private double sizeglint1r;
        private double sizeglint2r;
        private double sizepupr;

        private double sizeglint1cam2;
        private double sizeglint2cam2;
        private double sizepupcam2;

        private double sizeglint1rcam2;
        private double sizeglint2rcam2;
        private double sizepuprcam2;

        private int Minpup;
        private int Maxpup;

        private int Minglint;
        private int Maxglint;



        bool Gauss;
        private int Gausssize;

        bool Monoc;

        PointF pcrl;
        PointF pcrr;

        PointF pcrl2;
        PointF pcrr2;

        PointF pcrlcam2;
        PointF pcrrcam2;

        PointF pcrl2cam2;
        PointF pcrr2cam2;




        Emgu.CV.Cvb.CvBlobDetector bDetectcam2 = new Emgu.CV.Cvb.CvBlobDetector();

        Emgu.CV.Cvb.CvBlobDetector bDetect2cam2 = new Emgu.CV.Cvb.CvBlobDetector();


        private PointF pupilLeft = new PointF();
        private PointF pupilRight = new PointF();
        private PointF pupilLeftellipse = new PointF();
        private PointF pupilRightellipse = new PointF();
        private PointF glintLeft1 = new PointF();
        private PointF glintLeft2 = new PointF();
        private PointF glintRight1 = new PointF();
        private PointF glintRight2 = new PointF();

        private PointF pupilLeftcam2 = new PointF();
        private PointF pupilRightcam2 = new PointF();
        private PointF pupilLeftellipsecam2 = new PointF();
        private PointF pupilRightellipsecam2 = new PointF();
        private PointF glintLeft1cam2 = new PointF();
        private PointF glintLeft2cam2 = new PointF();
        private PointF glintRight1cam2 = new PointF();
        private PointF glintRight2cam2 = new PointF();

        private double xax;
        private double xaxcam2;
        private int minnb;
        private double scl;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CapturingProcess == true)
            {
                CapturingProcess = false;
                button1.Text = "Start";
            }
            else
            {
                CapturingProcess = true;
                button1.Text = "Pause";
            }

            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }

            if (backgroundWorker2.IsBusy != true)
            {
                backgroundWorker2.RunWorkerAsync();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Treshinc = (double)numericUpDown1.Value;
        }


        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Treshglintinc = (double)numericUpDown2.Value;

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Treshinc2 = (double)numericUpDown3.Value;

        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Treshglintinc2 = (double)numericUpDown4.Value;

        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Treshinccam2 = (double)numericUpDown5.Value;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            Treshglintinccam2 = (double)numericUpDown6.Value;

        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            Treshinc2cam2 = (double)numericUpDown7.Value;

        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            Treshglintinc2cam2 = (double)numericUpDown8.Value;

        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            Minpup = (int)numericUpDown9.Value;

        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            Maxpup = (int)numericUpDown10.Value;

        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            Minglint = (int)numericUpDown11.Value;

        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            Maxglint = (int)numericUpDown12.Value;

        }

        

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Eqhist = true;

                Treshinc = 8;
                Treshglintinc = 20;

                Treshinc2 = 8;
                Treshglintinc2 = 20;

                Treshinccam2 = 8;
                Treshglintinccam2 = 20;

                Treshinc2cam2 = 8;
                Treshglintinc2cam2 = 20;
            }
            else
            {
                Eqhist = false;

                Treshinc = 20;
                Treshglintinc = 20;

                Treshinc2 = 20;
                Treshglintinc2 = 20;

                Treshinccam2 = 20;
                Treshglintinccam2 = 20;

                Treshinc2cam2 = 20;
                Treshglintinc2cam2 = 20;
            }
            numericUpDown1.Value = (decimal)Treshinc;
            numericUpDown2.Value = (decimal)Treshglintinc;

            numericUpDown3.Value = (decimal)Treshinc2;
            numericUpDown4.Value = (decimal)Treshglintinc2;

            numericUpDown5.Value = (decimal)Treshinccam2;
            numericUpDown6.Value = (decimal)Treshglintinccam2;

            numericUpDown7.Value = (decimal)Treshinc2cam2;
            numericUpDown8.Value = (decimal)Treshglintinc2cam2;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                Gauss = true;
            }
            else
            {
                Gauss = false;
            }
            
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                Monoc = true;
                rightROIsetcam2 = false;
                rightROIset = false;

            }
            else
            {
                Monoc = false;
            }

        }


        private void numericUpDown15_ValueChanged(object sender, EventArgs e)
        {
            Gausssize = (int)numericUpDown15.Value;

        }

        private void numericUpDown13_ValueChanged(object sender, EventArgs e)
        {
            
            backgroundWorker1.CancelAsync();

            backgroundWorker2.CancelAsync();


            Lumenera.USB.dll.LucamStreamVideoControl(m_hCamera, Lumenera.USB.dll.LucamStreamMode.STOP_STREAMING, 0);
            Lumenera.USB.dll.LucamStreamVideoControl(m_hCamera1, Lumenera.USB.dll.LucamStreamMode.STOP_STREAMING, 0);



            m_lffFormat.X = (int)numericUpDown13.Value;
            m_lffFormatcam2.X = (int)numericUpDown13.Value;


            Lumenera.USB.dll.LucamSetFormat(m_hCamera, ref m_lffFormat, 381);

            Lumenera.USB.dll.LucamSetFormat(m_hCamera1, ref m_lffFormatcam2, 381);


            Offset.X = (int)numericUpDown13.Value;

            Lumenera.USB.dll.LucamStreamVideoControl(m_hCamera, Lumenera.USB.dll.LucamStreamMode.START_DISPLAY, 0);
            Lumenera.USB.dll.LucamStreamVideoControl(m_hCamera1, Lumenera.USB.dll.LucamStreamMode.START_DISPLAY, 0);


            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }

            if (backgroundWorker2.IsBusy != true)
            {
                backgroundWorker2.RunWorkerAsync();
            }



        }

        private void numericUpDown14_ValueChanged(object sender, EventArgs e)
        {
            

            backgroundWorker1.CancelAsync();

            backgroundWorker2.CancelAsync();


            Lumenera.USB.dll.LucamStreamVideoControl(m_hCamera, Lumenera.USB.dll.LucamStreamMode.STOP_STREAMING, 0);
            Lumenera.USB.dll.LucamStreamVideoControl(m_hCamera1, Lumenera.USB.dll.LucamStreamMode.STOP_STREAMING, 0);



            m_lffFormat.Y = (int)numericUpDown14.Value;
            m_lffFormatcam2.Y = (int)numericUpDown14.Value;


            Lumenera.USB.dll.LucamSetFormat(m_hCamera, ref m_lffFormat, 381);

            Lumenera.USB.dll.LucamSetFormat(m_hCamera1, ref m_lffFormatcam2, 381);


            Offset.Y = (int)numericUpDown14.Value;

            Lumenera.USB.dll.LucamStreamVideoControl(m_hCamera, Lumenera.USB.dll.LucamStreamMode.START_DISPLAY, 0);
            Lumenera.USB.dll.LucamStreamVideoControl(m_hCamera1, Lumenera.USB.dll.LucamStreamMode.START_DISPLAY, 0);


            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }

            if (backgroundWorker2.IsBusy != true)
            {
                backgroundWorker2.RunWorkerAsync();
            }
        }



       


        private void Form1_Load(object sender, EventArgs e)
        {

            // Lumenera camera 2
            float m_fFrameRate;

            int numcams = Lumenera.USB.dll.LucamNumCameras();
            Lumenera.USB.dll.LucamVersion[] versions = new Lumenera.USB.dll.LucamVersion[numcams];

            Lumenera.USB.dll.LucamEnumCameras(versions, numcams);


            int cam1;
            int cam2;

            if (versions[0].SerialNumber > versions[1].SerialNumber)
            {
                cam1 = 2;
                cam2 = 1;
            }
            else
            {
                cam2 = 2;
                cam1 = 1;
            }

            m_hCamera = Lumenera.USB.dll.LucamCameraOpen(cam1);



            Lumenera.USB.dll.LucamGetFormat(m_hCamera, out   m_lffFormat, out m_fFrameRate);

            

            m_lffFormat.Height = 480;
            m_lffFormat.Width = 1240;

            m_lffFormat.X = 404;
            m_lffFormat.Y = 304;

            Offset.X = m_lffFormat.X;
            Offset.Y = m_lffFormat.Y;

            

            Lumenera.USB.dll.LucamSetFormat(m_hCamera, ref m_lffFormat, 381);

            Lumenera.USB.dll.LucamSetProperty(m_hCamera, Lumenera.USB.dll.LucamProperty.EXPOSURE, 2, Lumenera.USB.dll.LucamPropertyFlag.USE);
            Lumenera.USB.dll.LucamSetProperty(m_hCamera, Lumenera.USB.dll.LucamProperty.GAIN, 4, Lumenera.USB.dll.LucamPropertyFlag.USE);

            Lumenera.USB.dll.LucamStreamVideoControl(m_hCamera, Lumenera.USB.dll.LucamStreamMode.START_DISPLAY, 0);

            Lumenera.USB.dll.LucamGetFormat(m_hCamera, out   m_lffFormat, out m_fFrameRate);
            

            //button1.Text = "Start";
            
            // Allocate memory to store frames
            iSize = (m_lffFormat.Width * m_lffFormat.Height);
             pFrame = new byte[iSize];
            img2 = new Image<Gray, byte>(m_lffFormat.Width, m_lffFormat.Height);
            Lumenera.USB.dll.LucamTakeVideo(m_hCamera, 1, pFrame);
            img2.Bytes = pFrame;

             backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            //backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            CapturingProcess = true;
            button1.Text = "Pause";

            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }

            // Camera 1
            // Lumenera camera 2
            m_hCamera1 = Lumenera.USB.dll.LucamCameraOpen(cam2);

            Lumenera.USB.dll.LucamGetFormat(m_hCamera1, out   m_lffFormatcam2, out m_fFrameRate);



            m_lffFormatcam2.Height = 480;
            m_lffFormatcam2.Width = 1240;

            m_lffFormatcam2.X = 404;
            m_lffFormatcam2.Y = 304;



            Lumenera.USB.dll.LucamSetFormat(m_hCamera1, ref m_lffFormatcam2, 381);

            Lumenera.USB.dll.LucamSetProperty(m_hCamera1, Lumenera.USB.dll.LucamProperty.EXPOSURE, 2, Lumenera.USB.dll.LucamPropertyFlag.USE);
            Lumenera.USB.dll.LucamSetProperty(m_hCamera1, Lumenera.USB.dll.LucamProperty.GAIN, 4, Lumenera.USB.dll.LucamPropertyFlag.USE);

            Lumenera.USB.dll.LucamStreamVideoControl(m_hCamera1, Lumenera.USB.dll.LucamStreamMode.START_DISPLAY, 0);

            Lumenera.USB.dll.LucamGetFormat(m_hCamera1, out   m_lffFormatcam2, out m_fFrameRate);

            
            // Allocate memory to store frames
            pFrame1 = new byte[iSize];
            img1 = new Image<Gray, byte>(m_lffFormatcam2.Width, m_lffFormatcam2.Height);
            Lumenera.USB.dll.LucamTakeVideo(m_hCamera1, 1, pFrame1);
            img1.Bytes = pFrame1;

            // Set initial Thresholds
            Treshinc = 20;
            Treshglintinc = 20;

            Treshinc2 = 20;
            Treshglintinc2 = 20;

            Treshinccam2 = 20;
            Treshglintinccam2 = 20;

            Treshinc2cam2 = 20;
            Treshglintinc2cam2 = 20;

            numericUpDown1.Value = (decimal)Treshinc;
            numericUpDown2.Value = (decimal)Treshglintinc;

            numericUpDown3.Value = (decimal)Treshinc2;
            numericUpDown4.Value = (decimal)Treshglintinc2;

            numericUpDown5.Value = (decimal)Treshinccam2;
            numericUpDown6.Value = (decimal)Treshglintinccam2;

            numericUpDown7.Value = (decimal)Treshinc2cam2;
            numericUpDown8.Value = (decimal)Treshglintinc2cam2;

            Minpup = 100;
            Maxpup = 2000;

            numericUpDown9.Value = (decimal)Minpup;
            numericUpDown10.Value = (decimal)Maxpup;

            Minglint = 2;
            Maxglint = 100;

            numericUpDown11.Value = (decimal)Minglint;
            numericUpDown12.Value = (decimal)Maxglint;

            minnb = 4;


            scl = 4;


            xax = 1;

            chart2.ChartAreas[0].AxisY.Minimum = -30;
            chart2.ChartAreas[0].AxisY.Maximum = 30;

            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Maximum = 100;

            xaxcam2 = 1;

            chart1.ChartAreas[0].AxisY.Minimum = -30;
            chart1.ChartAreas[0].AxisY.Maximum = 30;

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 100;


            Gausssize = 3;

            numericUpDown15.Value = (decimal)Gausssize;


            backgroundWorker2 = new BackgroundWorker();
            backgroundWorker2.DoWork += new DoWorkEventHandler(backgroundWorker2_DoWork);
            //backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
           

            if (backgroundWorker2.IsBusy != true)
            {
                backgroundWorker2.RunWorkerAsync();
            }

            InitUITimer();
            InitUITimer2();
            InitUITimer3();
            InitUITimer4();

            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerSupportsCancellation = true;

            numericUpDown13.Value = (decimal)Offset.X;
            numericUpDown14.Value = (decimal)Offset.Y;


        }



  

        // this method needs to be called on form load
        private void InitUITimer()
        {
            uiUpdateTimer = new System.Windows.Forms.Timer();
            // run every 40 ms (25 frames / second)
            uiUpdateTimer.Interval = 40;
            uiUpdateTimer.Tick += uiUpdateTimer_TickHandler;
            uiUpdateTimer.Start();
        }

        // this method needs to be called on form load
        private void InitUITimer2()
        {
            uiUpdateTimer2 = new System.Windows.Forms.Timer();
            // run every 40 ms (25 frames / second)
            uiUpdateTimer2.Interval = 40;
            uiUpdateTimer2.Tick += uiUpdateTimer2_TickHandler;
            uiUpdateTimer2.Start();
        }

        // this method needs to be called on form load
        private void InitUITimer3()
        {
            uiUpdateTimer3 = new System.Windows.Forms.Timer();
            // run every 40 ms (25 frames / second)
            uiUpdateTimer3.Interval = 40;
            uiUpdateTimer3.Tick += uiUpdateTimer3_TickHandler;
            uiUpdateTimer3.Start();
        }

        // this method needs to be called on form load
        private void InitUITimer4()
        {
            uiUpdateTimer4 = new System.Windows.Forms.Timer();
            // run every 40 ms (25 frames / second)
            uiUpdateTimer4.Interval = 40;
            uiUpdateTimer4.Tick += uiUpdateTimer4_TickHandler;
            uiUpdateTimer4.Start();
        }

        private void uiUpdateTimer_TickHandler(object sender, EventArgs e)
        {
            lock (syncRoot)
            {
            
                if (lastBitmap2 != null) imageBox2.Image = lastBitmap2;
                if (lastBitmap3 != null) imageBox3.Image = lastBitmap3;

                label1.Text = Tresh.ToString();
                label2.Text = Treshglint.ToString();
                
                label3.Text = Tresh2.ToString();
                label4.Text = Treshglint2.ToString();
                label9.Text = sizepup.ToString();
                label10.Text = sizeglint1.ToString();
                label11.Text = sizeglint2.ToString();
                label12.Text = sizepupr.ToString();
                label13.Text = sizeglint1r.ToString();
                label14.Text = sizeglint2r.ToString();

                           



            }

            
        }

        private void uiUpdateTimer2_TickHandler(object sender, EventArgs e)
        {
            

            lock (syncRoot2)
            {
                if (lastBitmap5 != null)
                {
                    imageBox5.Image = lastBitmap5;

                }

                if (lastBitmap6 != null)
                {
                    imageBox6.Image = lastBitmap6;

                }

                label5.Text = Treshcam2.ToString();
                label6.Text = Treshglintcam2.ToString();

                label7.Text = Tresh2cam2.ToString();
                label8.Text = Treshglint2cam2.ToString();

                label15.Text = sizepupcam2.ToString();
                label16.Text = sizeglint1cam2.ToString();
                label17.Text = sizeglint2cam2.ToString();
                label18.Text = sizepuprcam2.ToString();
                label19.Text = sizeglint1rcam2.ToString();
                label20.Text = sizeglint2rcam2.ToString();

               


            }
        }

        private void uiUpdateTimer3_TickHandler(object sender, EventArgs e)
        {


            xax += 1;
            if (xax > 100)
            {
                chart2.Series[1].Points.RemoveAt(0);
                chart2.Series[0].Points.RemoveAt(0);
                chart2.Series[2].Points.RemoveAt(0);
                chart2.Series[3].Points.RemoveAt(0);
                chart2.Series[4].Points.RemoveAt(0);
                chart2.Series[5].Points.RemoveAt(0);
                chart2.Series[6].Points.RemoveAt(0);
                chart2.Series[7].Points.RemoveAt(0);




                chart2.ChartAreas[0].AxisX.Minimum = xax - 80;
                chart2.ChartAreas[0].AxisX.Maximum = xax + 20;
            }


            chart2.Series[0].Points.AddXY(xax, pcrl.X);
            chart2.Series[1].Points.AddXY(xax, pcrl.Y);
            chart2.Series[2].Points.AddXY(xax, pcrr.X);
            chart2.Series[3].Points.AddXY(xax, pcrr.Y);
            chart2.Series[4].Points.AddXY(xax, pcrr2.X);
            chart2.Series[5].Points.AddXY(xax, pcrr2.Y);
            chart2.Series[6].Points.AddXY(xax, pcrl2.X);
            chart2.Series[7].Points.AddXY(xax, pcrl2.Y);


            chart2.Refresh();




        }

        private void uiUpdateTimer4_TickHandler(object sender, EventArgs e)
        {

            xaxcam2 += 1;
            if (xaxcam2 > 100)
            {
                chart1.Series[1].Points.RemoveAt(0);
                chart1.Series[0].Points.RemoveAt(0);
                chart1.Series[2].Points.RemoveAt(0);
                chart1.Series[3].Points.RemoveAt(0);
                chart1.Series[4].Points.RemoveAt(0);
                chart1.Series[5].Points.RemoveAt(0);
                chart1.Series[6].Points.RemoveAt(0);
                chart1.Series[7].Points.RemoveAt(0);




                chart1.ChartAreas[0].AxisX.Minimum = xaxcam2 - 80;
                chart1.ChartAreas[0].AxisX.Maximum = xaxcam2 + 20;
            }


            chart1.Series[0].Points.AddXY(xaxcam2, pcrlcam2.X);
            chart1.Series[1].Points.AddXY(xaxcam2, pcrlcam2.Y);
            chart1.Series[2].Points.AddXY(xaxcam2, pcrrcam2.X);
            chart1.Series[3].Points.AddXY(xaxcam2, pcrrcam2.Y);
            chart1.Series[4].Points.AddXY(xaxcam2, pcrr2cam2.X);
            chart1.Series[5].Points.AddXY(xaxcam2, pcrr2cam2.Y);
            chart1.Series[6].Points.AddXY(xaxcam2, pcrl2cam2.X);
            chart1.Series[7].Points.AddXY(xaxcam2, pcrl2cam2.Y);


            chart1.Refresh();


        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            while(CapturingProcess)
            {
                PupilRight = new PointF();
                GlintRight1 = new PointF();
                GlintRight2 = new PointF();
                PupilRightellipse = new PointF();

                PupilLeft = new PointF();
                GlintLeft1 = new PointF();
                GlintLeft2 = new PointF();
                PupilLeftellipse = new PointF();


                Sizepupleft = new int();
                Sizepupright = new int();

                Pupilleftwidth = new float();
                Pupilleftheight = new float();

                Pupilrightwidth = new float();
                Pupilrightheight = new float();

                Pupilrightangle = new float();
                Pupilleftangle = new float();


                 
                pupilLeft = new PointF();            
                pupilRight = new PointF();        
                pupilLeftellipse = new PointF();        
                pupilRightellipse = new PointF();        
                glintLeft1 = new PointF();        
                glintLeft2 = new PointF();        
                glintRight1 = new PointF();        
                glintRight2 = new PointF();

                Stopwatch stopwatch = Stopwatch.StartNew();

                Emgu.CV.Cvb.CvBlobs resultingImgBlobs = new Emgu.CV.Cvb.CvBlobs();
                Emgu.CV.Cvb.CvBlobs resultingImgBlobs2 = new Emgu.CV.Cvb.CvBlobs();

                img2 = new Image<Gray, byte>(m_lffFormat.Width, m_lffFormat.Height);


                Lumenera.USB.dll.LucamTakeVideo(m_hCamera, 1, pFrame);

                Timestamp = DateTime.UtcNow.Ticks;

                img2.Bytes = pFrame;

                if (leftROIset == false)
                {
                    //Rectangle[] rectangles = Classifier.DetectMultiScale(img2, scl, minnb, new Size(20, 20), new Size(500, 500));
                    Rectangle[] rectangles = Classifier.DetectMultiScale(img2, 4, 4, new Size(20, 20), new Size(200, 200));

                    Console.WriteLine(" {0} eyes classifier", rectangles.Length);

                    // 1 eye found
                    if (rectangles.Length == 1)
                    {
                        leftROI = rectangles[0];

                        leftROI.Width += 20;
                        leftROI.Height += 20;
                        leftROIset = true;

                        rightROIset = false;

                    }

                    if (Monoc == false)
                    {
                        // 2 eyes found
                        if (rectangles.Length == 2)
                        {
                            leftROI = rectangles[0];

                            leftROI.Width += 20;
                            leftROI.Height += 20;
                            leftROIset = true;

                            rightROI = rectangles[1];

                            rightROI.Width += 20;
                            rightROI.Height += 20;
                            rightROIset = true;
                        }
                    }

                }


                // If only one eye is found continue, but after 10 frames look for second eye again
                if (Monoc == false)
                {
                    if (rightROIset == false)
                    {
                        missingeye += 1;

                        PupilRight = new PointF();
                        GlintRight1 = new PointF();
                        GlintRight2 = new PointF();

                        if (missingeye > 30)
                        {
                            leftROIset = false;
                        }

                        if (leftROIset == false)
                        {
                            Console.WriteLine(" no eyes found");

                        }
                    }
                }
                

                // Check whether ROI fits within image
                if (rightROIset == true)
                {
                    if (rightROI.X + rightROI.Width > m_lffFormat.Width - 1)
                    {
                        rightROIset = false;
                    }

                    if (rightROI.X < 0)
                    {
                        rightROIset = false;
                    }

                    if (rightROI.Y + rightROI.Height > m_lffFormat.Height - 1)
                    {
                        rightROIset = false;
                    }

                    if (rightROI.Y < 0)
                    {
                        rightROIset = false;
                    }


                }

                if (leftROIset == true)
                {
                    if (leftROI.X + leftROI.Width > m_lffFormat.Width - 1)
                    {
                        leftROIset = false;
                    }

                    if (leftROI.X < 0)
                    {
                        leftROIset = false;
                    }

                    if (leftROI.Y + leftROI.Height > m_lffFormat.Height - 1)
                    {
                        leftROIset = false;
                    }

                    if (leftROI.Y < 0)
                    {
                        leftROIset = false;
                    }


                }



                // Do actual tracking
                if (leftROIset == true)
                {



                    imgsmall = img2.Copy(leftROI);

                    if (Eqhist)
                    {
                        imgsmall._EqualizeHist();
                    }

                   
                    
                    CvInvoke.MinMaxLoc(imgsmall, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                    minLoc.X += leftROI.X;
                    minLoc.Y += leftROI.Y;

                    if (maxVal < 220)
                    {
                        maxVal = 220;
                    }
                    if (minVal > 80)
                    {
                        minVal = 80;
                    }
                     
                    treshpupil = minVal + Treshinc;                    
                    treshglint = maxVal - Treshglintinc;


                    bin = imgsmall.ThresholdBinary(new Gray(treshglint), new Gray(255));

                    if (Gauss)
                    {
                        imgsmall._SmoothGaussian(Gausssize);
                    }

                    bin2 = imgsmall.ThresholdBinaryInv(new Gray(treshpupil), new Gray(255));

                    

                    
                    


                    int x = leftROI.X;
                    int y = leftROI.Y;

                    uint numWebcamBlobsFound = bDetect.Detect(bin2, resultingImgBlobs);
                    resultingImgBlobs.FilterByArea(Minpup, Maxpup);

                    uint numWebcamBlobsFound2 = bDetect2.Detect(bin, resultingImgBlobs2);
                    resultingImgBlobs2.FilterByArea(Minglint, Maxglint);

                    PointF pupilcor = new PointF();



                    if (resultingImgBlobs.Count > 0)
                    {
                        var size = new double[resultingImgBlobs.Count];
                        var keys = new uint[resultingImgBlobs.Count];
                        int ii = 0;

                        foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> item in resultingImgBlobs)
                        {
                            size[ii] = 1 / item.Value.Area;
                            keys[ii] = item.Value.Label;
                            ii++;

                        }
                        Array.Sort(size, keys);

                        int jj = 0;

                        foreach (int key in keys)
                        {
                            if (jj > 0)
                                resultingImgBlobs.Remove(keys[jj]);
                            jj++;
                        }

                        foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> item in resultingImgBlobs)
                        {
                            Emgu.CV.Cvb.CvBlob b = item.Value;
                            int width = leftROI.Width;
                            int height = leftROI.Height;


                            leftROI = new Rectangle((int)b.Centroid.X - width / 2 + x, (int)b.Centroid.Y - height / 2 + y, width, height);

                            Point[] Contour = b.GetContour();
                            Ellipse elps = PointCollection.EllipseLeastSquareFitting(Array.ConvertAll<Point, PointF>(Contour,
                            delegate(Point pt) { return new PointF(pt.X, pt.Y); }));
                            PupilLeft.X = b.Centroid.X + x;
                            PupilLeft.Y = b.Centroid.Y + y;
                            Sizepupleft = b.Area;
                            sizepup = b.Area;

                            PupilLeftellipse.X = elps.RotatedRect.Center.X + x;
                            PupilLeftellipse.Y = elps.RotatedRect.Center.Y + y;

                            Pupilleftangle = elps.RotatedRect.Angle;
                            Pupilleftwidth = elps.RotatedRect.Size.Width;
                            Pupilleftheight = elps.RotatedRect.Size.Height;


                            pupilcor = b.Centroid;

                            pupilLeft.X = PupilLeft.X - x;
                            pupilLeft.Y = PupilLeft.Y - y;

                        }

                        if (resultingImgBlobs2.Count > 2)
                        {

                            var dist = new double[resultingImgBlobs2.Count];
                            var keys2 = new uint[resultingImgBlobs2.Count];
                            int iii = 0;

                            foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                            {
                                dist[iii] = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y)); ;
                                keys2[iii] = blob.Value.Label;
                                iii++;

                            }
                            Array.Sort(dist, keys2);

                            int jjj = 0;

                            foreach (int key in keys2)
                            {
                                if (jjj > 1)
                                    resultingImgBlobs2.Remove(keys2[jjj]);
                                jjj++;
                            }


                        }

                        if (resultingImgBlobs2.Count == 2)
                        {
                            PointF pnts = new PointF();
                            double dist = new double();
                            SortedList<double, PointF> list = new SortedList<double, PointF>();
                            SortedList<double, double> list2 = new SortedList<double, double>();

                            foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                            {
                                pnts.X = blob.Value.Centroid.X + x;
                                pnts.Y = blob.Value.Centroid.Y + y;
                                dist = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y));
                                if (!list.ContainsKey(dist))
                                {
                                    list.Add(dist, pnts);
                                    list2.Add(dist, blob.Value.Area);

                                }
                                else
                                {
                                    list.Add(100, pnts);
                                    list2.Add(100, blob.Value.Area);

                                }
                            }

                            if (Math.Abs(list.Values[0].Y - list.Values[1].Y) < 20)
                            {
                                if (list.Keys[1] < 30 && list2.Keys[1] < 30 )
                                {
                                    missingpupleft = 0;

                                    if (list.Values[0].X < list.Values[1].X)
                                    {
                                        GlintLeft1 = list.Values[0];
                                        GlintLeft2 = list.Values[1];
                                        sizeglint1 = list2.Values[0];
                                        sizeglint2 = list2.Values[1];

                                        glintLeft1.X = GlintLeft1.X - x;
                                        glintLeft1.Y = GlintLeft1.Y - y;

                                        glintLeft2.X = GlintLeft2.X - x;
                                        glintLeft2.Y = GlintLeft2.Y - y;


                                    }
                                    else
                                    {
                                        GlintLeft1 = list.Values[1];
                                        GlintLeft2 = list.Values[0];
                                        sizeglint1 = list2.Values[1];
                                        sizeglint2 = list2.Values[0];

                                        glintLeft1.X = GlintLeft1.X - x;
                                        glintLeft1.Y = GlintLeft1.Y - y;

                                        glintLeft2.X = GlintLeft2.X - x;
                                        glintLeft2.Y = GlintLeft2.Y - y;
                                    }
                                }
                            }
                        }

                        if (resultingImgBlobs2.Count == 1)
                        {
                            Console.WriteLine(" {0} left glints", resultingImgBlobs2.Count);

                            PointF pnts = new PointF();
                            double dist = new double();
                            SortedList<double, PointF> list = new SortedList<double, PointF>();


                            foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                            {
                                pnts.X = blob.Value.Centroid.X + x;
                                pnts.Y = blob.Value.Centroid.Y + y;
                                dist = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y));
                                if (!list.ContainsKey(dist))
                                {
                                    list.Add(dist, pnts);
                                }
                                else
                                {
                                    list.Add(100, pnts);

                                }
                            }

                            GlintLeft1 = list.Values[0];

                            glintLeft1.X = GlintLeft1.X - x;
                            glintLeft1.Y = GlintLeft1.Y - y;

                            missingpupleft += 1;

                            if (missingpupleft > 100)
                            {
                                leftROIset = false;
                                leftROI = Rectangle.Empty;
                                missingpupleft = 0;
                            }

                           
                        }

                        if (resultingImgBlobs2.Count < 1)
                        {
                            missingpupleft += 1;

                            if (missingpupleft > 100)
                            {
                                leftROIset = false;
                                leftROI = Rectangle.Empty;
                                missingpupleft = 0;
                            }
                        }

                    }
                    else
                    {

                        Console.WriteLine(" {0} left blobs", resultingImgBlobs.Count);

                   
                        missingpupleft += 1;

                        if (missingpupleft > 100)
                        {
                            leftROIset = false;
                            leftROI = Rectangle.Empty;
                            missingpupleft = 0;
                        }


                    }



                }

                

                if (rightROIset == true)
                {
                    resultingImgBlobs = new Emgu.CV.Cvb.CvBlobs();
                    resultingImgBlobs2 = new Emgu.CV.Cvb.CvBlobs();

                    imgsmallr = img2.Copy(rightROI);

                    if (Eqhist)
                    {
                        imgsmallr._EqualizeHist();
                    }

             

                    CvInvoke.MinMaxLoc(imgsmallr, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                    minLoc.X += rightROI.X;
                    minLoc.Y += rightROI.Y;

                    if (maxVal < 220)
                    {
                        maxVal = 220;
                    }
                    if (minVal > 80)
                    {
                        minVal = 80;
                    }
                     treshpupil2 = minVal + Treshinc2;
                     treshglint2 = maxVal - Treshglintinc2;
                     binr = imgsmallr.ThresholdBinary(new Gray(treshglint2), new Gray(255));

                     if (Gauss)
                     {
                         imgsmallr._SmoothGaussian(Gausssize);
                     }

                     bin2r = imgsmallr.ThresholdBinaryInv(new Gray(treshpupil2), new Gray(255));

                    int x = rightROI.X;
                    int y = rightROI.Y;

                    uint numWebcamBlobsFound = bDetect.Detect(bin2r, resultingImgBlobs);
                    resultingImgBlobs.FilterByArea(50, 1000);

                    uint numWebcamBlobsFound2 = bDetect2.Detect(binr, resultingImgBlobs2);
                    resultingImgBlobs2.FilterByArea(Minglint, Maxglint);

                    PointF pupilcor = new PointF();

                    if (resultingImgBlobs.Count > 0)
                    {
                        var size = new double[resultingImgBlobs.Count];
                        var keys = new uint[resultingImgBlobs.Count];
                        int ii = 0;

                        foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> item in resultingImgBlobs)
                        {
                            size[ii] = 1 / item.Value.Area;
                            keys[ii] = item.Value.Label;
                            ii++;

                        }
                        Array.Sort(size, keys);

                        int jj = 0;

                        foreach (int key in keys)
                        {
                            if (jj > 0)
                                resultingImgBlobs.Remove(keys[jj]);

                            jj++;
                        }

                        foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> item in resultingImgBlobs)
                        {
                            Emgu.CV.Cvb.CvBlob b = item.Value;
                            int width = rightROI.Width;
                            int height = rightROI.Height;


                            rightROI = new Rectangle((int)b.Centroid.X - width / 2 + x, (int)b.Centroid.Y - height / 2 + y, width, height);

                            Point[] Contour = b.GetContour();
                            Ellipse elps = PointCollection.EllipseLeastSquareFitting(Array.ConvertAll<Point, PointF>(Contour,
                            delegate(Point pt) { return new PointF(pt.X, pt.Y); }));
                            PupilRight.X = b.Centroid.X + x;
                            PupilRight.Y = b.Centroid.Y + y; 
                            Sizepupright = b.Area;
                            sizepupr = b.Area;


                            PupilRightellipse.X = elps.RotatedRect.Center.X + x;
                            PupilRightellipse.Y = elps.RotatedRect.Center.Y + y;

                            pupilcor = b.Centroid;


                            Pupilrightangle = elps.RotatedRect.Angle;
                            Pupilrightwidth = elps.RotatedRect.Size.Width;
                            Pupilrightheight = elps.RotatedRect.Size.Height;

                            pupilRight.X = PupilRight.X - x;
                            pupilRight.Y = PupilRight.Y - y;
                        }


                        if (resultingImgBlobs2.Count > 2)
                        {

                            var dist = new double[resultingImgBlobs2.Count];
                            var keys2 = new uint[resultingImgBlobs2.Count];
                            int iii = 0;

                            foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                            {
                                dist[iii] = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y)); ;
                                keys2[iii] = blob.Value.Label;
                                iii++;

                            }
                            Array.Sort(dist, keys2);

                            int jjj = 0;

                            foreach (int key in keys2)
                            {
                                if (jjj > 1)
                                    resultingImgBlobs2.Remove(keys2[jjj]);
                                jjj++;
                            }


                        }

                        if (resultingImgBlobs2.Count == 2)
                        {
                            
                            

                            PointF pnts = new PointF();
                            double dist = new double();
                            SortedList<double, PointF> list = new SortedList<double, PointF>();
                            SortedList<double, double> list2 = new SortedList<double, double>();


                            foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                            {
                                pnts.X = blob.Value.Centroid.X + x;
                                pnts.Y = blob.Value.Centroid.Y + y;
                                dist = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y));

                                if (!list.ContainsKey(dist))
                                {
                                    list.Add(dist, pnts);
                                    list2.Add(dist, blob.Value.Area);

                                }
                                else
                                {
                                    list.Add(100, pnts);
                                    list2.Add(100, blob.Value.Area);

                                }
                            }

                            if (Math.Abs(list.Values[0].Y - list.Values[1].Y) < 20)
                            {
                                if (list.Keys[1] < 30 && list2.Keys[1] < 30)
                                {
                                    missingpupright = 0;
                                    if (list.Values[0].X < list.Values[1].X)
                                    {
                                        GlintRight1 = list.Values[0];
                                        GlintRight2 = list.Values[1];

                                        sizeglint1r = list2.Values[0];
                                        sizeglint2r = list2.Values[1];

                                        glintRight1.X = GlintRight1.X - x;
                                        glintRight1.Y = GlintRight1.Y - y;

                                        glintRight2.X = GlintRight2.X - x;
                                        glintRight2.Y = GlintRight2.Y - y;


                                    }
                                    else
                                    {
                                        GlintRight1 = list.Values[1];
                                        GlintRight2 = list.Values[0];

                                        sizeglint1r = list2.Values[1];
                                        sizeglint2r = list2.Values[0];

                                        glintRight1.X = GlintRight1.X - x;
                                        glintRight1.Y = GlintRight1.Y - y;

                                        glintRight2.X = GlintRight2.X - x;
                                        glintRight2.Y = GlintRight2.Y - y;
                                    }

                                }
                            }
                        }

                        if (resultingImgBlobs2.Count == 1)
                        {

                            Console.WriteLine(" {0} glints right", resultingImgBlobs2.Count);

                            PointF pnts = new PointF();
                            double dist = new double();
                            SortedList<double, PointF> list = new SortedList<double, PointF>();


                            foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                            {
                                pnts.X = blob.Value.Centroid.X + x;
                                pnts.Y = blob.Value.Centroid.Y + y;
                                dist = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y));

                                if (!list.ContainsKey(dist))
                                {
                                    list.Add(dist, pnts);
                                }
                                else
                                {
                                    list.Add(100, pnts);

                                }
                            }

                            GlintRight1 = list.Values[0];

                            missingpupright += 1;

                            if (missingpupright > 100)
                            {
                                rightROIset = false;
                                rightROI = Rectangle.Empty;
                                missingpupright = 0;
                            }
                        }

                        if (resultingImgBlobs2.Count < 1)
                        {
                            missingpupright += 1;

                            if (missingpupright > 100)
                            {
                                rightROIset = false;
                                rightROI = Rectangle.Empty;
                                missingpupright = 0;
                            }
                        }

                    }
                    else
                    {

                        Console.WriteLine(" {0} blobs right", resultingImgBlobs.Count);

                    
                        missingpupright += 1;

                        if (missingpupright > 100)
                        {
                            rightROIset = false;
                            rightROI = Rectangle.Empty;
                            missingpupright = 0;
                        }


                    }



                }

                lock (syncRoot)
                {
                    //Image<Bgr, Byte> mask = new Image<Bgr, byte>(imgsmall.Width, imgsmall.Height);
                    // Cam 1 left eye
                    if (imgsmall != null)
                    {
                        if (leftROIset == false)
                        {
                            imgsmall = new Image<Gray, byte>(80, 80);
                            imgsmall.SetValue(new Gray(255));
                        }

                        Mask = imgsmall.Convert<Bgr, Byte>();

                        //CvInvoke.CvtColor(imgsmall, mask, ColorConversion.Gray2Bgr);
                        if (leftROIset == true)
                        {

                            Mask.SetValue(new Bgr(Color.Red), bin);
                            Mask.SetValue(new Bgr(Color.Turquoise), bin2);

                            Cross2DF crosspup = new Cross2DF();

                            PointF locpup = new PointF();

                            locpup.X = pupilLeft.X ;
                            locpup.Y = pupilLeft.Y ;

                            
                            crosspup.Center = locpup;

                            Cross2DF crossglint1 = new Cross2DF();

                            PointF locglint1 = new PointF();

                            locglint1.X = glintLeft1.X ;
                            locglint1.Y = glintLeft1.Y ;


                            crossglint1.Center = locglint1;

                            Cross2DF crossglint2 = new Cross2DF();

                            PointF locglint2 = new PointF();

                            locglint2.X = glintLeft2.X ;
                            locglint2.Y = glintLeft2.Y ;


                            crossglint2.Center = locglint2;

                            crossglint1.Size = new SizeF(new PointF(10, 10));
                            crossglint2.Size = new SizeF(new PointF(10, 10));
                            crosspup.Size = new SizeF(new PointF(20, 20));


                            
                            Mask.Draw(crosspup, new Bgr(Color.White), 1);

                            Mask.Draw(crossglint1, new Bgr(Color.Yellow), 1);
                            Mask.Draw(crossglint2, new Bgr(Color.Yellow), 1);

                        }

                        
                        if (lastBitmap2 != null) lastBitmap2.Dispose();
                        lastBitmap2 = Mask;

                    }
                    // Cam 1 right eye
                    if (imgsmallr != null)
                    {
                        if (rightROIset == false)
                        {
                            imgsmallr = new Image<Gray, byte>(80, 80);
                            imgsmallr.SetValue(new Gray(255));
                        }

                        Mask2 = imgsmallr.Convert<Bgr, Byte>();

                        //CvInvoke.CvtColor(imgsmall, mask, ColorConversion.Gray2Bgr);
                        if (rightROIset == true)
                        {
                            Cross2DF crosspup = new Cross2DF();

                            PointF locpup = new PointF();

                            locpup.X = pupilRight.X ;
                            locpup.Y = pupilRight.Y ;


                            crosspup.Center = locpup;

                            Cross2DF crossglint1 = new Cross2DF();

                            PointF locglint1 = new PointF();

                            locglint1.X = glintRight1.X ;
                            locglint1.Y = glintRight1.Y ;


                            crossglint1.Center = locglint1;

                            Cross2DF crossglint2 = new Cross2DF();

                            PointF locglint2 = new PointF();

                            locglint2.X = glintRight2.X ;
                            locglint2.Y = glintRight2.Y ;


                            crossglint2.Center = locglint2;

                            crossglint1.Size = new SizeF(new PointF(10, 10));
                            crossglint2.Size = new SizeF(new PointF(10, 10));
                            crosspup.Size = new SizeF(new PointF(20, 20));
                                  
                            Mask2.SetValue(new Bgr(Color.Red), binr);
                            Mask2.SetValue(new Bgr(Color.Turquoise), bin2r);  
                    
                            Mask2.Draw(crosspup, new Bgr(Color.White), 1);

                            Mask2.Draw(crossglint1, new Bgr(Color.Yellow), 1);
                            Mask2.Draw(crossglint2, new Bgr(Color.Yellow), 1);

                           
                        }

                        if (lastBitmap3 != null) lastBitmap3.Dispose();
                        lastBitmap3 = Mask2;
                    }

                        Tresh = treshpupil;
                        Treshglint = treshglint;

                        Tresh2 = treshpupil2;
                        Treshglint2 = treshglint2;

                        pcrl.X = PupilLeft.X - GlintLeft1.X;
                        pcrl.Y = PupilLeft.Y - GlintLeft1.Y;

                        pcrr.X = PupilRight.X - GlintRight1.X;
                        pcrr.Y = PupilRight.Y - GlintRight1.Y;

                        pcrl2.X = PupilLeft.X - GlintLeft2.X;
                        pcrl2.Y = PupilLeft.Y - GlintLeft2.Y;

                        pcrr2.X = PupilRight.X - GlintRight2.X;
                        pcrr2.Y = PupilRight.Y - GlintRight2.Y;

                   
                }

                WriteLine(BuildLogLine(Timestamp, PupilLeft, PupilRight, GlintLeft1, GlintLeft2, GlintRight1, GlintRight2, PupilLeftellipse, PupilRightellipse, Sizepupleft, Sizepupright, Pupilleftangle, Pupilrightangle, Pupilleftwidth, Pupilrightwidth, Pupilleftheight, Pupilrightheight));
         

            }
      
        }


        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {

            while (CapturingProcess)
            {
                PupilRightcam2 = new PointF();
                GlintRight1cam2 = new PointF();
                GlintRight2cam2 = new PointF();
                PupilRightellipsecam2 = new PointF();

                PupilLeftcam2 = new PointF();
                GlintLeft1cam2 = new PointF();
                GlintLeft2cam2 = new PointF();
                PupilLeftellipsecam2 = new PointF();


                Sizepupleftcam2 = new int();
                Sizepuprightcam2 = new int();

                Pupilleftwidthcam2 = new float();
                Pupilleftheightcam2 = new float();

                Pupilrightwidthcam2 = new float();
                Pupilrightheightcam2 = new float();

                Pupilrightanglecam2 = new float();
                Pupilleftanglecam2 = new float();

                pupilLeftcam2 = new PointF();
                pupilRightcam2 = new PointF();
                pupilLeftellipsecam2 = new PointF();
                pupilRightellipsecam2 = new PointF();
                glintLeft1cam2 = new PointF();
                glintLeft2cam2 = new PointF();
                glintRight1cam2 = new PointF();
                glintRight2cam2 = new PointF();

                Emgu.CV.Cvb.CvBlobs resultingImgBlobs = new Emgu.CV.Cvb.CvBlobs();
                Emgu.CV.Cvb.CvBlobs resultingImgBlobs2 = new Emgu.CV.Cvb.CvBlobs();



                img1 = new Image<Gray, byte>(m_lffFormatcam2.Width, m_lffFormatcam2.Height);


                Lumenera.USB.dll.LucamTakeVideo(m_hCamera1, 1, pFrame1);

                Timestampcam2 = DateTime.UtcNow.Ticks;

                img1.Bytes = pFrame1;

                if (leftROIsetcam2 == false)
                {
                    //Rectangle[] rectangles = Classifiercam2.DetectMultiScale(img1, scl, minnb, new Size(20, 20), new Size(500, 500));
                    // Rectangle[] rectangles = Classifiercam2.DetectMultiScale(img1, 1.1, 4, new Size(20, 20), new Size(200, 200));
                    Rectangle[] rectangles = Classifiercam2.DetectMultiScale(img1, 4, 4, new Size(20, 20), new Size(200, 200));


                    Console.WriteLine(" {0} eyes classifier", rectangles.Length);

                    // 1 eye found
                    if (rectangles.Length == 1)
                    {
                        leftROIcam2 = rectangles[0];

                        leftROIcam2.Width += 20;
                        leftROIcam2.Height += 20;
                        leftROIsetcam2 = true;

                        rightROIsetcam2 = false;

                    }
                    if (Monoc == false)
                    {
                        // 2 eyes found
                        if (rectangles.Length == 2)
                        {
                            leftROIcam2 = rectangles[0];

                            leftROIcam2.Width += 20;
                            leftROIcam2.Height += 20;
                            leftROIsetcam2 = true;

                            rightROIcam2 = rectangles[1];

                            rightROIcam2.Width += 20;
                            rightROIcam2.Height += 20;
                            rightROIsetcam2 = true;
                        }
                    }

                }

                // If only one eye is found continue, but after 10 frames look for second eye again
                if (Monoc == false)
                {
                    if (rightROIsetcam2 == false)
                    {
                        missingeyecam2 += 1;

                        PupilRightcam2 = new PointF();
                        GlintRight1cam2 = new PointF();
                        GlintRight2cam2 = new PointF();

                        if (missingeyecam2 > 30)
                        {
                            leftROIsetcam2 = false;
                        }

                        if (leftROIsetcam2 == false)
                        {
                            Console.WriteLine(" no eyes found");

                        }
                    }
                }

                // Check whether ROI fits within image
                if (rightROIsetcam2 == true)
                {
                    if (rightROIcam2.X + rightROIcam2.Width > m_lffFormatcam2.Width - 1)
                    {
                        rightROIsetcam2 = false;
                    }

                    if (rightROIcam2.X < 0)
                    {
                        rightROIsetcam2 = false;
                    }

                    if (rightROIcam2.Y + rightROIcam2.Height > m_lffFormatcam2.Height - 1)
                    {
                        rightROIsetcam2 = false;
                    }

                    if (rightROIcam2.Y < 0)
                    {
                        rightROIsetcam2 = false;
                    }


                }

                if (leftROIsetcam2 == true)
                {
                    if (leftROIcam2.X + leftROIcam2.Width > m_lffFormatcam2.Width - 1)
                    {
                        leftROIsetcam2 = false;
                    }

                    if (leftROIcam2.X < 0)
                    {
                        leftROIsetcam2 = false;
                    }

                    if (leftROIcam2.Y + leftROIcam2.Height > m_lffFormatcam2.Height - 1)
                    {
                        leftROIsetcam2 = false;
                    }

                    if (leftROIcam2.Y < 0)
                    {
                        leftROIsetcam2 = false;
                    }


                }


                // Do actual tracking
                if (leftROIsetcam2 == true)
                {



                    imgsmallcam2 = img1.Copy(leftROIcam2);

                    if (Eqhist)
                    {                    
                        imgsmallcam2._EqualizeHist();
                    }


                 

                    CvInvoke.MinMaxLoc(imgsmallcam2, ref minValcam2, ref maxValcam2, ref minLoccam2, ref maxLoccam2);

                    minLoccam2.X += leftROIcam2.X;
                    minLoccam2.Y += leftROIcam2.Y;

                    if (maxValcam2 < 220)
                    {
                        maxValcam2 = 220;
                    }
                    if (minValcam2 > 80)
                    {
                        minValcam2 = 80;
                    }

                    treshpupilcam2 = minValcam2 + Treshinccam2;
                    treshglintcam2 = maxValcam2 - Treshglintinccam2;


                    bincam2 = imgsmallcam2.ThresholdBinary(new Gray(treshglintcam2), new Gray(255));

                    if (Gauss)
                    {                 
                        imgsmallcam2._SmoothGaussian(3);   
                    }
                    bin2cam2 = imgsmallcam2.ThresholdBinaryInv(new Gray(treshpupilcam2), new Gray(255));



                    int x = leftROIcam2.X;
                    int y = leftROIcam2.Y;

                uint numWebcamBlobsFound = bDetectcam2.Detect(bin2cam2, resultingImgBlobs);
                resultingImgBlobs.FilterByArea(Minpup, Maxpup);

                uint numWebcamBlobsFound2 = bDetect2cam2.Detect(bincam2, resultingImgBlobs2);
                resultingImgBlobs2.FilterByArea(Minglint, Maxglint);

                PointF pupilcor = new PointF();

                if (resultingImgBlobs.Count > 0)
                {
                    var size = new double[resultingImgBlobs.Count];
                    var keys = new uint[resultingImgBlobs.Count];
                    int ii = 0;

                    foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> item in resultingImgBlobs)
                    {
                        size[ii] = 1 / item.Value.Area;
                        keys[ii] = item.Value.Label;
                        ii++;

                    }
                    Array.Sort(size, keys);

                    int jj = 0;

                    foreach (int key in keys)
                    {
                        if (jj > 0)
                            resultingImgBlobs.Remove(keys[jj]);
                        jj++;
                    }

                    foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> item in resultingImgBlobs)
                    {
                        Emgu.CV.Cvb.CvBlob b = item.Value;
                        int width = leftROIcam2.Width;
                        int height = leftROIcam2.Height;


                        leftROIcam2 = new Rectangle((int)b.Centroid.X - width / 2 + x, (int)b.Centroid.Y - height / 2 + y, width, height);

                        Point[] Contour = b.GetContour();
                        Ellipse elps = PointCollection.EllipseLeastSquareFitting(Array.ConvertAll<Point, PointF>(Contour,
                        delegate(Point pt) { return new PointF(pt.X, pt.Y); }));
                        PupilLeftcam2.X = b.Centroid.X + x;
                        PupilLeftcam2.Y = b.Centroid.Y + y;
                        Sizepupleftcam2 = b.Area;

                        PupilLeftellipsecam2.X = elps.RotatedRect.Center.X + x;
                        PupilLeftellipsecam2.Y = elps.RotatedRect.Center.Y + y;

                        Pupilleftanglecam2 = elps.RotatedRect.Angle;
                        Pupilleftwidthcam2 = elps.RotatedRect.Size.Width;
                        Pupilleftheightcam2 = elps.RotatedRect.Size.Height;


                        pupilcor = b.Centroid;

                        pupilLeftcam2.X = PupilLeftcam2.X - x;
                        pupilLeftcam2.Y = PupilLeftcam2.Y - y;

                        sizepupcam2 = b.Area;

                    }

                    if (resultingImgBlobs2.Count > 2)
                    {

                        var dist = new double[resultingImgBlobs2.Count];
                        var keys2 = new uint[resultingImgBlobs2.Count];
                        int iii = 0;

                        foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                        {
                            dist[iii] = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y)); ;
                            keys2[iii] = blob.Value.Label;
                            iii++;

                        }
                        Array.Sort(dist, keys2);

                        int jjj = 0;

                        foreach (int key in keys2)
                        {
                            if (jjj > 1)
                                resultingImgBlobs2.Remove(keys2[jjj]);
                            jjj++;
                        }


                    }

                    if (resultingImgBlobs2.Count == 2)
                    {
                        
                        PointF pnts = new PointF();
                        double dist = new double();
                        SortedList<double, PointF> list = new SortedList<double, PointF>();
                        SortedList<double, double> list2 = new SortedList<double, double>();


                        foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                        {
                            pnts.X = blob.Value.Centroid.X + x;
                            pnts.Y = blob.Value.Centroid.Y + y;
                            dist = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y));
                            if (!list.ContainsKey(dist))
                            {
                                list.Add(dist, pnts);
                                list2.Add(dist, blob.Value.Area);

                            }
                            else
                            {
                                list.Add(100, pnts);
                                list2.Add(100, blob.Value.Area);

                            }
                        }

                        if (Math.Abs(list.Values[0].Y - list.Values[1].Y) < 20)
                        {
                            if (list.Keys[1] < 30 && list2.Keys[1] < 30)
                            {
                                missingpupleftcam2 = 0;
                                if (list.Values[0].X < list.Values[1].X)
                                {
                                    GlintLeft1cam2 = list.Values[0];
                                    GlintLeft2cam2 = list.Values[1];

                                    sizeglint1cam2 = list2.Values[0];
                                    sizeglint2cam2 = list2.Values[1];


                                    glintLeft1cam2.X = GlintLeft1cam2.X - x;
                                    glintLeft1cam2.Y = GlintLeft1cam2.Y - y;

                                    glintLeft2cam2.X = GlintLeft2cam2.X - x;
                                    glintLeft2cam2.Y = GlintLeft2cam2.Y - y;
                                }
                                else
                                {
                                    GlintLeft1cam2 = list.Values[1];
                                    GlintLeft2cam2 = list.Values[0];

                                    sizeglint1cam2 = list2.Values[1];
                                    sizeglint2cam2 = list2.Values[0];

                                    glintLeft1cam2.X = GlintLeft1cam2.X - x;
                                    glintLeft1cam2.Y = GlintLeft1cam2.Y - y;

                                    glintLeft2cam2.X = GlintLeft2cam2.X - x;
                                    glintLeft2cam2.Y = GlintLeft2cam2.Y - y;
                                }
                            }
                        }
                    }

                    if (resultingImgBlobs2.Count == 1)
                    {
                        Console.WriteLine(" {0} left glints", resultingImgBlobs2.Count);

                        PointF pnts = new PointF();
                        double dist = new double();
                        SortedList<double, PointF> list = new SortedList<double, PointF>();
                        SortedList<double, double> list2 = new SortedList<double, double>();


                        foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                        {
                            pnts.X = blob.Value.Centroid.X + x;
                            pnts.Y = blob.Value.Centroid.Y + y;
                            dist = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y));
                            if (!list.ContainsKey(dist))
                            {
                                list.Add(dist, pnts);
                                list2.Add(dist, blob.Value.Area);

                            }
                            else
                            {
                                list.Add(100, pnts);
                                list2.Add(100, blob.Value.Area);


                            }
                        }

                        GlintLeft1cam2 = list.Values[0];

                        sizeglint1cam2 = list2.Values[0];


                        glintLeft1cam2.X = GlintLeft1.X - x;
                        glintLeft1cam2.Y = GlintLeft1.Y - y;

                        missingpupleftcam2 += 1;

                        if (missingpupleftcam2 > 100)
                        {
                            leftROIsetcam2 = false;
                            leftROIcam2 = Rectangle.Empty;
                            missingpupleftcam2 = 0;
                        }
                    }

                    if (resultingImgBlobs2.Count < 1)
                    {
                        missingpupleftcam2 += 1;

                        if (missingpupleftcam2 > 100)
                        {
                            leftROIsetcam2 = false;
                            leftROIcam2 = Rectangle.Empty;
                            missingpupleftcam2 = 0;
                        }
                    }

                }
                else
                {

                    Console.WriteLine(" {0} left blobs", resultingImgBlobs.Count);

                    missingpupleftcam2 += 1;

                    if (missingpupleftcam2 > 100)
                    {
                        leftROIsetcam2 = false;
                        leftROIcam2 = Rectangle.Empty;
                        missingpupleftcam2 = 0;
                    }


                }

                }

                if (rightROIsetcam2 == true)
                {
                    resultingImgBlobs = new Emgu.CV.Cvb.CvBlobs();
                    resultingImgBlobs2 = new Emgu.CV.Cvb.CvBlobs();

                    imgsmallrcam2 = img1.Copy(rightROIcam2);


                    if (Eqhist)
                    {
                        imgsmallrcam2._EqualizeHist();
                    }



                    CvInvoke.MinMaxLoc(imgsmallrcam2, ref minValcam2, ref maxValcam2, ref minLoccam2, ref maxLoccam2);

                    minLoccam2.X += rightROIcam2.X;
                    minLoccam2.Y += rightROIcam2.Y;

                    if (maxVal < 220)
                    {
                        maxVal = 220;
                    }
                    if (minVal > 80)
                    {
                        minVal = 80;
                    }
                    treshpupil2cam2 = minValcam2 + Treshinc2cam2;
                    treshglint2cam2 = maxValcam2 - Treshglintinc2cam2;
                    binrcam2 = imgsmallrcam2.ThresholdBinary(new Gray(treshglint2cam2), new Gray(255));

                    if (Gauss)
                    {
                        imgsmallrcam2._SmoothGaussian(3);
                    }
                    

                    bin2rcam2 = imgsmallrcam2.ThresholdBinaryInv(new Gray(treshpupil2cam2), new Gray(255));

                    int x = rightROIcam2.X;
                    int y = rightROIcam2.Y;

                    uint numWebcamBlobsFound = bDetectcam2.Detect(bin2rcam2, resultingImgBlobs);
                    resultingImgBlobs.FilterByArea(Minpup, Maxpup);

                    uint numWebcamBlobsFound2 = bDetect2cam2.Detect(binrcam2, resultingImgBlobs2);
                    resultingImgBlobs2.FilterByArea(Minglint, Maxglint);

                    PointF pupilcor = new PointF();

                    if (resultingImgBlobs.Count > 0)
                    {
                        var size = new double[resultingImgBlobs.Count];
                        var keys = new uint[resultingImgBlobs.Count];
                        int ii = 0;

                        foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> item in resultingImgBlobs)
                        {
                            size[ii] = 1 / item.Value.Area;
                            keys[ii] = item.Value.Label;
                            ii++;

                        }
                        Array.Sort(size, keys);

                        int jj = 0;

                        foreach (int key in keys)
                        {
                            if (jj > 0)
                                resultingImgBlobs.Remove(keys[jj]);
                            jj++;
                        }

                        foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> item in resultingImgBlobs)
                        {
                            Emgu.CV.Cvb.CvBlob b = item.Value;
                            int width = rightROIcam2.Width;
                            int height = rightROIcam2.Height;


                            rightROIcam2 = new Rectangle((int)b.Centroid.X - width / 2 + x, (int)b.Centroid.Y - height / 2 + y, width, height);

                            //Console.WriteLine(" {0} x", b.Centroid.X + x);
                            Point[] Contour = b.GetContour();
                            Ellipse elps = PointCollection.EllipseLeastSquareFitting(Array.ConvertAll<Point, PointF>(Contour,
                            delegate(Point pt) { return new PointF(pt.X, pt.Y); }));
                            PupilRightcam2.X = b.Centroid.X + x;
                            PupilRightcam2.Y = b.Centroid.Y + y;
                            Sizepuprightcam2 = b.Area;

                            PupilRightellipsecam2.X = elps.RotatedRect.Center.X + x;
                            PupilRightellipsecam2.Y = elps.RotatedRect.Center.Y + y;

                            pupilcor = b.Centroid;


                            Pupilrightanglecam2 = elps.RotatedRect.Angle;
                            Pupilrightwidthcam2 = elps.RotatedRect.Size.Width;
                            Pupilrightheightcam2 = elps.RotatedRect.Size.Height;

                            pupilRightcam2.X = PupilRightcam2.X - x;
                            pupilRightcam2.Y = PupilRightcam2.Y - y;

                            sizepuprcam2 = b.Area;
                        }


                        if (resultingImgBlobs2.Count > 2)
                        {

                            var dist = new double[resultingImgBlobs2.Count];
                            var keys2 = new uint[resultingImgBlobs2.Count];
                            int iii = 0;

                            foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                            {
                                dist[iii] = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y)); ;
                                keys2[iii] = blob.Value.Label;
                                iii++;

                            }
                            Array.Sort(dist, keys2);

                            int jjj = 0;

                            foreach (int key in keys2)
                            {
                                if (jjj > 1)
                                    resultingImgBlobs2.Remove(keys2[jjj]);
                                jjj++;
                            }


                        }

                        if (resultingImgBlobs2.Count == 2)
                        {
                           


                            PointF pnts = new PointF();
                            double dist = new double();
                            SortedList<double, PointF> list = new SortedList<double, PointF>();
                            SortedList<double, double> list2 = new SortedList<double, double>();


                            foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                            {
                                pnts.X = blob.Value.Centroid.X + x;
                                pnts.Y = blob.Value.Centroid.Y + y;
                                dist = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y));

                                if (!list.ContainsKey(dist))
                                {
                                    list.Add(dist, pnts);
                                    list2.Add(dist, blob.Value.Area);
                                }
                                else
                                {
                                    list.Add(100, pnts);
                                    list2.Add(100, blob.Value.Area);


                                }
                            }

                            if (Math.Abs(list.Values[0].Y - list.Values[1].Y) < 20)
                            {
                                if (list.Keys[1] < 30 && list2.Keys[1] < 30)
                                {
                                    if (list.Values[0].X < list.Values[1].X)
                                    {
                                        GlintRight1cam2 = list.Values[0];
                                        GlintRight2cam2 = list.Values[1];

                                        sizeglint1rcam2 = list2.Values[0];
                                        sizeglint2rcam2 = list2.Values[1];


                                        glintRight1cam2.X = GlintRight1cam2.X - x;
                                        glintRight1cam2.Y = GlintRight1cam2.Y - y;

                                        glintRight2cam2.X = GlintRight2cam2.X - x;
                                        glintRight2cam2.Y = GlintRight2cam2.Y - y;
                                    }
                                    else
                                    {
                                        GlintRight1cam2 = list.Values[1];
                                        GlintRight2cam2 = list.Values[0];

                                        sizeglint1rcam2 = list2.Values[1];
                                        sizeglint2rcam2 = list2.Values[0];

                                        glintRight1cam2.X = GlintRight1cam2.X - x;
                                        glintRight1cam2.Y = GlintRight1cam2.Y - y;

                                        glintRight2cam2.X = GlintRight2cam2.X - x;
                                        glintRight2cam2.Y = GlintRight2cam2.Y - y;
                                    }

                                }
                            }
                        }

                        if (resultingImgBlobs2.Count == 1)
                        {

                            Console.WriteLine(" {0} glints right", resultingImgBlobs2.Count);

                            PointF pnts = new PointF();
                            double dist = new double();
                            SortedList<double, PointF> list = new SortedList<double, PointF>();
                            SortedList<double, double> list2 = new SortedList<double, double>();


                            foreach (KeyValuePair<uint, Emgu.CV.Cvb.CvBlob> blob in resultingImgBlobs2)
                            {
                                pnts.X = blob.Value.Centroid.X + x;
                                pnts.Y = blob.Value.Centroid.Y + y;
                                dist = Math.Sqrt((blob.Value.Centroid.X - pupilcor.X) * (blob.Value.Centroid.X - pupilcor.X) + (blob.Value.Centroid.Y - pupilcor.Y) * (blob.Value.Centroid.Y - pupilcor.Y));

                                if (!list.ContainsKey(dist))
                                {
                                    list.Add(dist, pnts);
                                    list2.Add(dist, blob.Value.Area);

                                }
                                else
                                {
                                    list.Add(100, pnts);
                                    list2.Add(100, blob.Value.Area);


                                }
                            }

                            GlintRight1cam2 = list.Values[0];
                            sizeglint1rcam2 = list2.Values[0];

                            glintRight1cam2.X = GlintRight1cam2.X - x;
                            glintRight1cam2.Y = GlintRight1cam2.Y - y;

                            missingpuprightcam2 += 1;

                            if (missingpuprightcam2 > 100)
                            {
                                rightROIsetcam2 = false;
                                rightROIcam2 = Rectangle.Empty;
                                missingpuprightcam2 = 0;
                            }
                        }

                        if (resultingImgBlobs2.Count < 1)
                        {
                            missingpuprightcam2 += 1;

                            if (missingpuprightcam2 > 100)
                            {
                                rightROIsetcam2 = false;
                                rightROIcam2 = Rectangle.Empty;
                                missingpuprightcam2 = 0;
                            }
                        }



                    }
                    else
                    {

                        Console.WriteLine(" {0} blobs right", resultingImgBlobs.Count);

                    
                        missingpuprightcam2 += 1;

                        if (missingpuprightcam2 > 100)
                        {
                            rightROIsetcam2 = false;
                            rightROIcam2 = Rectangle.Empty;
                            missingpuprightcam2 = 0;
                        }


                    }



                }


                lock (syncRoot2)
                {
                    //Image<Bgr, Byte> mask = new Image<Bgr, byte>(imgsmall.Width, imgsmall.Height);
                    // Cam 1 left eye
                    if (imgsmallcam2 != null)
                    {
                        if (leftROIsetcam2 == false)
                        {
                            imgsmallcam2 = new Image<Gray, byte>(80, 80);
                            imgsmallcam2.SetValue(new Gray(255));
                        }

                        Maskcam2 = imgsmallcam2.Convert<Bgr, Byte>();

                        //CvInvoke.CvtColor(imgsmall, mask, ColorConversion.Gray2Bgr);
                        if (leftROIsetcam2 == true)
                        {
                            Maskcam2.SetValue(new Bgr(Color.Red), bincam2);
                            Maskcam2.SetValue(new Bgr(Color.Turquoise), bin2cam2);

                            Cross2DF crosspup = new Cross2DF();

                            PointF locpup = new PointF();

                            locpup.X = pupilLeftcam2.X;
                            locpup.Y = pupilLeftcam2.Y;


                            crosspup.Center = locpup;

                            Cross2DF crossglint1 = new Cross2DF();

                            PointF locglint1 = new PointF();

                            locglint1.X = glintLeft1cam2.X;
                            locglint1.Y = glintLeft1cam2.Y;


                            crossglint1.Center = locglint1;

                            Cross2DF crossglint2 = new Cross2DF();

                            PointF locglint2 = new PointF();

                            locglint2.X = glintLeft2cam2.X;
                            locglint2.Y = glintLeft2cam2.Y;


                            crossglint2.Center = locglint2;

                            crossglint1.Size = new SizeF(new PointF(10, 10));
                            crossglint2.Size = new SizeF(new PointF(10, 10));
                            crosspup.Size = new SizeF(new PointF(20, 20));



                            Maskcam2.Draw(crosspup, new Bgr(Color.White), 1);

                            Maskcam2.Draw(crossglint1, new Bgr(Color.Yellow), 1);
                            Maskcam2.Draw(crossglint2, new Bgr(Color.Yellow), 1);
                        }


                        if (lastBitmap5 != null) lastBitmap5.Dispose();
                        lastBitmap5 = Maskcam2;

                    }

                    // Cam 1 right eye
                    if (imgsmallrcam2 != null)
                    {
                        if (rightROIsetcam2 == false)
                        {
                            imgsmallrcam2 = new Image<Gray, byte>(80, 80);
                            imgsmallrcam2.SetValue(new Gray(255));
                        }

                        Mask2cam2 = imgsmallrcam2.Convert<Bgr, Byte>();

                        if (rightROIsetcam2 == true)
                        {
                            Mask2cam2.SetValue(new Bgr(Color.Red), binrcam2);
                            Mask2cam2.SetValue(new Bgr(Color.Turquoise), bin2rcam2);

                            Cross2DF crosspup = new Cross2DF();

                            PointF locpup = new PointF();

                            locpup.X = pupilRightcam2.X;
                            locpup.Y = pupilRightcam2.Y;


                            crosspup.Center = locpup;

                            Cross2DF crossglint1 = new Cross2DF();

                            PointF locglint1 = new PointF();

                            locglint1.X = glintRight1cam2.X;
                            locglint1.Y = glintRight1cam2.Y;


                            crossglint1.Center = locglint1;

                            Cross2DF crossglint2 = new Cross2DF();

                            PointF locglint2 = new PointF();

                            locglint2.X = glintRight2cam2.X;
                            locglint2.Y = glintRight2cam2.Y;


                            crossglint2.Center = locglint2;

                            crossglint1.Size = new SizeF(new PointF(10, 10));
                            crossglint2.Size = new SizeF(new PointF(10, 10));
                            crosspup.Size = new SizeF(new PointF(20, 20));



                            Mask2cam2.Draw(crosspup, new Bgr(Color.White), 1);

                            Mask2cam2.Draw(crossglint1, new Bgr(Color.Yellow), 1);
                            Mask2cam2.Draw(crossglint2, new Bgr(Color.Yellow), 1);
                        }

                        if (lastBitmap6 != null) lastBitmap6.Dispose();
                        lastBitmap6 = Mask2cam2;
                    }

                        
                        Tresh2cam2 = treshpupil2cam2;
                        Treshglint2cam2 = treshglint2cam2;
                                   
                                       
                    Treshcam2 = treshpupilcam2;
                    Treshglintcam2 = treshglintcam2;

                    pcrlcam2.X = PupilLeftcam2.X - GlintLeft1cam2.X;
                    pcrlcam2.Y = PupilLeftcam2.Y - GlintLeft1cam2.Y;

                    pcrrcam2.X = PupilRightcam2.X - GlintRight1cam2.X;
                    pcrrcam2.Y = PupilRightcam2.Y - GlintRight1cam2.Y;

                    pcrl2cam2.X = PupilLeftcam2.X - GlintLeft2cam2.X;
                    pcrl2cam2.Y = PupilLeftcam2.Y - GlintLeft2cam2.Y;

                    pcrr2cam2.X = PupilRightcam2.X - GlintRight2cam2.X;
                    pcrr2cam2.Y = PupilRightcam2.Y - GlintRight2cam2.Y;

                    

                }

                WriteLinecam2(BuildLogLinecam2(Timestampcam2, PupilLeftcam2, PupilRightcam2, GlintLeft1cam2, GlintLeft2cam2, GlintRight1cam2, GlintRight2cam2, PupilLeftellipsecam2, PupilRightellipsecam2, Sizepupleftcam2, Sizepuprightcam2, Pupilleftanglecam2, Pupilrightanglecam2, Pupilleftwidthcam2, Pupilrightwidthcam2, Pupilleftheightcam2, Pupilrightheightcam2));

            }
            
        }




        public static void WriteLine(string line)
        {
            // logwriter is created when logfilepath is set, if not use default "gazeLog.txt"
            if (logWriter == null)
            {



                if (logFilePath == null)
                {
                    var rand = new Random();
                    var dt = DateTime.Now;

                    // Eg. 20100115122354 + random value 1000 to 9999 
                    // eg. maximum 10,000 unique calibrations per every second of every minute, every hour, day, month
                    // ought be unqiue enough

                    var today = string.Format("{0:yyyyMMddHHmm}", dt);
                    // string strID = today + rand.Next(1000, 9999).ToString();
                    string strID2 = "gazelog1cam" + today + rand.Next(1000, 9999).ToString() + ".txt";

                    // Directory in which the LogFiles are saved. Might need to change this. 

                    //logFilePath = "C:\\Users\\Annemiek\\AppData\\Roaming\\Gazetracker" +
                               //Path.DirectorySeparatorChar + strID2;

                    System.IO.Directory.CreateDirectory("C:\\Users\\Public\\Roaming\\StereoEyetracker");

                    logFilePath = "C:\\Users\\Public\\Roaming\\StereoEyetracker" +
                               Path.DirectorySeparatorChar + strID2;

                    fs =
                        new FileStream(
                            logFilePath,
                            FileMode.Create);
                }
                else
                {
                    fs = new FileStream(logFilePath, FileMode.Append);
                }

                logWriter = new StreamWriter(fs);
            }


            logWriter.WriteLine(line);


            logWriter.Flush();
        }

        public static void WriteLinecam2(string line)
        {
            // logwriter is created when logfilepath is set, if not use default "gazeLog.txt"
            if (logWritercam2 == null)
            {



                if (logFilePathcam2 == null)
                {
                    var rand = new Random();
                    var dt = DateTime.Now;

                    // Eg. 20100115122354 + random value 1000 to 9999 
                    // eg. maximum 10,000 unique calibrations per every second of every minute, every hour, day, month
                    // ought be unqiue enough

                    var today = string.Format("{0:yyyyMMddHHmm}", dt);
                    // string strID = today + rand.Next(1000, 9999).ToString();
                    string strID2 = "gazelog2cam" + today + rand.Next(1000, 9999).ToString() + ".txt";

                    // Directory in which the LogFiles are saved. Might need to change this. 

                    //logFilePath = "C:\\Users\\Annemiek\\AppData\\Roaming\\Gazetracker" +
                    //Path.DirectorySeparatorChar + strID2;
                    System.IO.Directory.CreateDirectory("C:\\Users\\Public\\Roaming\\StereoEyetracker");


                    logFilePathcam2 = "C:\\Users\\Public\\Roaming\\StereoEyetracker" +
                               Path.DirectorySeparatorChar + strID2;

                    fscam2 =
                        new FileStream(
                            logFilePathcam2,
                            FileMode.Create);
                }
                else
                {
                    fscam2 = new FileStream(logFilePathcam2, FileMode.Append);
                }

                logWritercam2 = new StreamWriter(fscam2);
            }


            logWritercam2.WriteLine(line);


            logWritercam2.Flush();
        }


        public static string BuildLogLine(long TimeStamp, PointF PupilLeft, PointF PupilRight, PointF LeftGlint1, PointF LeftGlint2, PointF RightGlint1, PointF RightGlint2, PointF PupilLeftellipse, PointF PupilRightellipse, float Sizepupleft, float Sizepupright, float Pupilleftangle, float Pupilrightangle, float Pupilleftwidth, float Pupilrightwidth, float Pupilleftheight, float Pupilrightheight)
        {
            var sb = new StringBuilder();
            string tab = "\t";

            sb.Append(TimeStamp + tab);

            sb.Append(PupilLeft.X + Offset.X + tab);
            sb.Append(PupilLeft.Y + Offset.Y + tab);

            sb.Append(PupilRight.X + Offset.X + tab);
            sb.Append(PupilRight.Y + Offset.Y + tab);

            sb.Append(LeftGlint1.X + Offset.X + tab);
            sb.Append(LeftGlint1.Y + Offset.Y + tab);


            sb.Append(LeftGlint2.X + Offset.X + tab);
            sb.Append(LeftGlint2.Y + Offset.Y + tab);


            sb.Append(RightGlint1.X + Offset.X + tab);
            sb.Append(RightGlint1.Y + Offset.Y + tab);

            sb.Append(RightGlint2.X + Offset.X + tab);
            sb.Append(RightGlint2.Y + Offset.Y + tab);

            sb.Append(PupilLeftellipse.X + Offset.X + tab);
            sb.Append(PupilLeftellipse.Y + Offset.Y + tab);

            sb.Append(PupilRightellipse.X + Offset.X + tab);
            sb.Append(PupilRightellipse.Y + Offset.Y + tab);

            sb.Append(Sizepupleft + tab);
            sb.Append(Sizepupright + tab);

            sb.Append(Pupilleftangle + tab);
            sb.Append(Pupilrightangle + tab);

            sb.Append(Pupilleftwidth + tab);
            sb.Append(Pupilrightwidth + tab);

            sb.Append(Pupilleftheight + tab);
            sb.Append(Pupilrightheight + tab);



            return sb.ToString();
        }


        public static string BuildLogLinecam2(long TimeStamp, PointF PupilLeft, PointF PupilRight, PointF LeftGlint1, PointF LeftGlint2, PointF RightGlint1, PointF RightGlint2, PointF PupilLeftellipse, PointF PupilRightellipse, float Sizepupleft, float Sizepupright, float Pupilleftangle, float Pupilrightangle, float Pupilleftwidth, float Pupilrightwidth, float Pupilleftheight, float Pupilrightheight)
        {
            var sb = new StringBuilder();
            string tab = "\t";

            sb.Append(TimeStamp + tab);

            sb.Append(PupilLeft.X + Offset.X + tab);
            sb.Append(PupilLeft.Y + Offset.Y + tab);

            sb.Append(PupilRight.X + Offset.X + tab);
            sb.Append(PupilRight.Y + Offset.Y + tab);

            sb.Append(LeftGlint1.X + Offset.X + tab);
            sb.Append(LeftGlint1.Y + Offset.Y + tab);


            sb.Append(LeftGlint2.X + Offset.X + tab);
            sb.Append(LeftGlint2.Y + Offset.Y + tab);


            sb.Append(RightGlint1.X + Offset.X + tab);
            sb.Append(RightGlint1.Y + Offset.Y + tab);

            sb.Append(RightGlint2.X + Offset.X + tab);
            sb.Append(RightGlint2.Y + Offset.Y + tab);

            sb.Append(PupilLeftellipse.X + Offset.X + tab);
            sb.Append(PupilLeftellipse.Y + Offset.Y + tab);

            sb.Append(PupilRightellipse.X + Offset.X + tab);
            sb.Append(PupilRightellipse.Y + Offset.Y + tab);

            sb.Append(Sizepupleft + tab);
            sb.Append(Sizepupright + tab);

            sb.Append(Pupilleftangle + tab);
            sb.Append(Pupilrightangle + tab);

            sb.Append(Pupilleftwidth + tab);
            sb.Append(Pupilrightwidth + tab);

            sb.Append(Pupilleftheight + tab);
            sb.Append(Pupilrightheight + tab);



            return sb.ToString();
        }



        












       



       





       

 

        

        

        

        

       
    
    }


}
