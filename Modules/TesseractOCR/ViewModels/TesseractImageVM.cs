using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism;
using Prism.Commands;
using UnityControl;
using Tesseract;
using UnityMethods.Extend;
using System.IO;
using OpenCvSharp;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Collections.ObjectModel;

namespace TesseractOCR.ViewModels
{
    public class TesseractImageVM : BindableBase
    {
        public ObservableCollection<ComboBoxDataBing> TesseractLangItems { get; set; } = new ObservableCollection<ComboBoxDataBing>();

        public string TesseractLang { get; set; }

        public string OriginImageSource { get; set; }

        public WriteableBitmap ThresholdImageSource { get; set; }

        public double thresh { get; set; } = 28;
        public double maxval { get; set; } = 255;

        public string OcrTextInfo { get; set; }
        public DelegateCommand OpenImagePathCommand => new DelegateCommand(() =>
        {
            string filePath = FileDialogTools.OpenFileDialog("请选择需要识别的图片", "Image|*.png;*.jpeg;*.jpg");

            if (!filePath.IsEmpty() && File.Exists(filePath))
            {
                OriginImageSource = filePath;
            }
        });

        public DelegateCommand ThresholdImageCommand => new DelegateCommand(() =>
        {
            if (File.Exists(OriginImageSource))
            {
                //降噪
                Mat origin = Cv2.ImRead(OriginImageSource, ImreadModes.Grayscale);

                //阈值操作 阈值参数可以用一些可视化工具来调试得到
                Mat thresholdImg = origin.Threshold(thresh, maxval, ThresholdTypes.Binary);

                ThresholdImageSource = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(thresholdImg);
            }
        });

        private Mat HsvSeg(Mat mat)
        {
            Mat hsv = new Mat();
            Mat[] imgHsv = new Mat[3];
            Cv2.CvtColor(mat, hsv, ColorConversionCodes.BGR2HSV);
            Cv2.Split(hsv, out imgHsv);
            Cv2.InRange(imgHsv[0], new Scalar(94), new Scalar(115), imgHsv[0]);
            Cv2.InRange(imgHsv[1], new Scalar(90), new Scalar(255), imgHsv[1]);
            Cv2.InRange(imgHsv[2], new Scalar(36), new Scalar(255), imgHsv[2]);
            Cv2.BitwiseAnd(imgHsv[0], imgHsv[1], imgHsv[0]);
            Cv2.BitwiseAnd(imgHsv[0], imgHsv[2], imgHsv[0]);
            return imgHsv[0];
        }

        public TesseractImageVM()
        {
            string traineddata = Path.Combine(Directory.GetCurrentDirectory(), "traineddata");
            if (Directory.Exists(traineddata))
            {
                TesseractLangItems.AddRange(Directory.GetFiles(traineddata, "*.traineddata").Select(x => new ComboBoxDataBing() { Key = Path.GetFileNameWithoutExtension(x) }).ToList());
            }

        }

        public DelegateCommand TesseractCommand => new DelegateCommand(() =>
        {
            OcrTextInfo = ImageToText(OriginImageSource);
        });

        public string ImageToText(string imgPath)
        {
            /*降噪处理*/
            Mat simg = Cv2.ImRead(imgPath, ImreadModes.Grayscale);
            //阈值操作 阈值参数可以用一些可视化工具来调试得到
            Mat thresholdImg = simg.Threshold(thresh, maxval, ThresholdTypes.Binary);

            using (var engine = new TesseractEngine("traineddata", TesseractLang, EngineMode.Default))
            {
                using (var img = Pix.LoadFromMemory(thresholdImg.ToBytes()))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
        }

    }
}
