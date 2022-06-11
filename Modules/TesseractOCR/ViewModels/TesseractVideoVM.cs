using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TesseractOCR.ViewModels
{
    /// <summary>
    /// 视频图像OCR识别
    /// </summary>
    public class TesseractVideoVM : BindableBase
    {
        public WriteableBitmap VideoViewBitmap { get; set; } = new WriteableBitmap(480, 640, 96.0, 96.0, PixelFormats.Rgb24, null);

        private OpenCvSharp.VideoCapture VideoCapture { get; set; }

        public TesseractVideoVM()
        {
            VideoCapture = new OpenCvSharp.VideoCapture(0);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    OpenCvSharp.Mat mat = VideoCapture.RetrieveMat();

                    if (!mat.Empty())
                    {
                        OpenCvSharp.Cv2.Rotate(mat, mat, OpenCvSharp.RotateFlags.Rotate90Clockwise);

                        if (System.Windows.Application.Current != null)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                VideoViewBitmap.Lock();

                                OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(mat, VideoViewBitmap);

                                VideoViewBitmap.AddDirtyRect(new Int32Rect(0, 0, VideoViewBitmap.PixelWidth, VideoViewBitmap.PixelHeight));

                                VideoViewBitmap.Unlock();
                            }), DispatcherPriority.Render);
                        }

                    }

                    mat.Dispose();
                }
            });


        }


    }
}
