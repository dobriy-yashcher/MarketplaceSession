using AForge.Video;
using AForge.Video.DirectShow;
using MarketplaceSession.Components;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using ProductDelivery.Components;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using ZXing;
using ZXing.Common;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using System.Diagnostics.Tracing;
using Microsoft.Win32;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace MarketplaceSession.Windows
{
    /// <summary>
    /// Interaction logic for QRScanWindow.xaml
    /// </summary>
    public partial class QRScanWindow : Window
    {
        VideoCaptureDevice videoCaptureDevice;
        Timer timer;

        public QRScanWindow()
        {
            InitializeComponent();

            cbDevice.ItemsSource = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            cbDevice.SelectedIndex = 0;

            timer = new Timer()
            {
                Interval = 1000,
                Enabled = true
            };
            timer.Stop();
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //if (scanImage.Source != null)
            //{
            Bitmap bitmap;

            try
            {
                Dispatcher.Invoke(() =>
                {
                    QRCodeDecoder dec = new QRCodeDecoder();

                    var bitmap1 = scanImage.Source; 

                    //var bp = ConvertImage.BitmapSourceToBitmapImage(scanImage.Source as BitmapSource);

                    string result = null;
                    //var result = dec.Decode(new QRCodeBitmapImage(new Bitmap (BitmapImage2Bitmap(bp))));

                    if (result != null)
                    {
                        var cartId = int.Parse(result);
                        MessageBox.Show(result);

                        timer.Stop();
                        if (videoCaptureDevice.IsRunning)
                            videoCaptureDevice.Stop();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Timer_Elapsed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbDevice.SelectedItem == null)
                    throw new Exception("Camera is not selected");

                var filter = cbDevice.SelectedItem as FilterInfo;
                videoCaptureDevice = new VideoCaptureDevice(filter.MonikerString);
                videoCaptureDevice.Start();
                videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
                timer.Start();

                //CheckQR();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " btnStart_Click", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    scanImage.Source = eventArgs.Frame.ToBitmapImage();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " VideoCaptureDevice_NewFrame", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void StopCapture()
        {
            if (videoCaptureDevice == null) return;

            videoCaptureDevice.NewFrame -= VideoCaptureDevice_NewFrame;
            videoCaptureDevice.SignalToStop();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();

            Manager.CurrentWindow.IsEnabled = true;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private async Task CheckQR()
        {
            while (true)
            {
                await Task.Delay(1000);

                if (scanImage.Source != null)
                {
                    Bitmap bitmap;

                    using (MemoryStream outStream = new MemoryStream())
                    {
                        BitmapEncoder enc = new BmpBitmapEncoder();
                        enc.Frames.Add(BitmapFrame.Create((scanImage.Source as BitmapImage)));
                        enc.Save(outStream);
                        bitmap = new Bitmap(outStream);
                    }

                    //QRCodeDecoder decoder = new QRCodeDecoder();
                    //var result = decoder.Decode(new QRCodeBitmapImage(bitmap));

                    //if (result != null)
                    //{
                    //    MessageBox.Show(result);
                    //    if (videoCaptureDevice.IsRunning)
                    //        videoCaptureDevice.Stop();
                    //    return;
                    //}
                }
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            StopCapture();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (videoCaptureDevice.IsRunning)
                videoCaptureDevice.Stop();
        }
    }
}
