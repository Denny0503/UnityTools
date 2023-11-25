using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using PhoenixControl.Events;
using TopMethods.Files;
using PhoenixControl.Fonts;
using GDI = System.Drawing;
using PhoenixControl;

namespace WaterMarkTools.ViewModels
{
    public struct ImagesListInfo
    {
        public string FileName;
        public string FileSrc;
    }

    public enum WaterMarkControlType
    {
        Enlarge,
        Left,
        ZoomOut,
        Right,
        Up,
        Down,
        Center,
        Rotate_Left,
        Rotate_Right,
        OriginSrc,
        DestinationSrc,
        WaterMarkSrc,
        ListImages,
        SignatureTxt,
        StartAddWaterMark,
        TopLeft,
        BottomLeft,
        TopRight,
        BottomRight,
    }

    public class WaterMarkImageInfo
    {
        public Point WaterMarkPosition { set; get; }
        public Size WaterMarkSize { set; get; }
        public int WaterMarkOpacity { set; get; }
        public double WaterMarkRotateAngle { set; get; }
        public double WaterMarkScale_X { set; get; }
        public double WaterMarkScale_Y { set; get; }
        public string DestinationDir { set; get; }
        public string OriginDir { set; get; }
        public string WaterMarkSrc { set; get; }
        public WaterMarkControlType ControlType { set; get; }

        public WaterMarkImageInfo() { }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public class WaterMarkVM : BindableBase, IActiveAware
    {
        private readonly string ModuleName = "批量添加图片水印";

        #region 保存设置使用常量字符串

        private readonly string Section_WaterMark = "WaterMarkSettings";
        private readonly string Key_OriginImageSrc = "OriginImageSrc";
        private readonly string Key_DesImageSrc = "DesImageSrc";
        private readonly string Key_WaterMarkImageSrc = "WaterMarkImageSrc";
        private readonly string Key_SignatureTxt = "SignatureTxt";
        private readonly string Key_WaterMarkOpacity = "WaterMarkOpacity";
        private readonly string _tempPath = "./temp/temp.png";

        #endregion

        #region 文字水印的样式

        private double _fontSize = 16;
        private Brush _foreground;
        private string _fontFamily = "微软雅黑";
        private FontWeight _fontWeight;
        private FontStyle _fontStyle;

        #endregion

        private readonly string[] FileExList = new string[] { "PNG", "png", "jpg", "JPG", "jpeg", "JPEG", "bmp", "BMP" };

        private bool IsConvertFontToImage = false;

        private Thread _taskThread;
        private Thread _taskAddMarkThread;
        private bool _taskAddMarkThread_IsStart = false;
        private bool _taskThread_IsStart = false;

        private List<ImagesListInfo> _filesLists = new List<ImagesListInfo>();
        private WaterMarkImageInfo _waterMarkImageInfo = new WaterMarkImageInfo();


        //private ILoggerFacade _logger;
        private IEventAggregator _sendEvent;

        public DelegateCommand<string> WaterMarkControlCommands { get; private set; }
        public DelegateCommand<TextBox> FormatLineStyleControlCommands { get; private set; }
        public DelegateCommand<Image> StartAddWaterMarkControlCommands { get; private set; }

        #region 属性定义

        private string _originImageSrc = "";
        public string OriginImageSrc
        {
            get { return _originImageSrc; }
            set
            {
                SetProperty(ref _originImageSrc, value);
                if (value.Equals("") || !Directory.Exists(value))
                {
                    BtnListImageIsActive = false;
                }
                else
                {
                    BtnListImageIsActive = true;
                }
            }
        }

        private string _desImageSrc = "";
        public string DesImageSrc
        {
            get { return _desImageSrc; }
            set { SetProperty(ref _desImageSrc, value); }
        }

        private string _waterMarkImageSrc = "";
        public string WaterMarkImageSrc
        {
            get { return _waterMarkImageSrc; }
            set
            {
                SetProperty(ref _waterMarkImageSrc, value);
                if (value.Equals("") || !File.Exists(value))
                {
                    BtnIsActive = false;
                    PreviewImage = null;
                }
                else
                {
                    BtnIsActive = true;
                    PreviewImage = GetBitmapImage(value);
                    IsConvertFontToImage = false;
                }
            }
        }

        private string _progressInfo = "";
        public string ProgressInfo
        {
            get { return _progressInfo; }
            set { SetProperty(ref _progressInfo, value); }
        }

        private ImageSource _previewImage;
        public ImageSource PreviewImage
        {
            get { return _previewImage; }
            set { SetProperty(ref _previewImage, value); }
        }

        private double _waterMarkScale_X = 1;
        public double WaterMarkScale_X
        {
            get { return _waterMarkScale_X; }
            set { SetProperty(ref _waterMarkScale_X, value); }
        }

        private double _waterMarkScale_Y = 1;
        public double WaterMarkScale_Y
        {
            get { return _waterMarkScale_Y; }
            set { SetProperty(ref _waterMarkScale_Y, value); }
        }

        private double _waterMarkRoate = 0;
        public double WaterMarkRoate
        {
            get { return _waterMarkRoate; }
            set { SetProperty(ref _waterMarkRoate, value); }
        }

        private double _waterMarkTranslate_X = 0;
        public double WaterMarkTranslate_X
        {
            get { return _waterMarkTranslate_X; }
            set { SetProperty(ref _waterMarkTranslate_X, value); }
        }

        private double _waterMarkTranslate_Y = 0;
        public double WaterMarkTranslate_Y
        {
            get { return _waterMarkTranslate_Y; }
            set { SetProperty(ref _waterMarkTranslate_Y, value); }
        }

        private int _position_X = 0;
        public int Position_X
        {
            get { return _position_X; }
            set
            {
                SetProperty(ref _position_X, value);
                WaterMarkTranslate_X = value;
            }
        }

        private int _position_Y = 0;
        public int Position_Y
        {
            get { return _position_Y; }
            set
            {
                SetProperty(ref _position_Y, value);
                WaterMarkTranslate_Y = value;
            }
        }

        private int _imageOpacity = 80;
        public int ImageOpacity
        {
            get { return _imageOpacity; }
            set
            {
                SetProperty(ref _imageOpacity, value);
                PreviewOpacity = value;
            }
        }

        private int _previewOpacity = 100;
        public int PreviewOpacity
        {
            get { return _previewOpacity; }
            set { SetProperty(ref _previewOpacity, value); }
        }

        private bool _btnIsActive = false;
        public bool BtnIsActive
        {
            get { return _btnIsActive; }
            set { SetProperty(ref _btnIsActive, value); }
        }

        private bool _btnListImageIsActive = false;
        public bool BtnListImageIsActive
        {
            get { return _btnListImageIsActive; }
            set { SetProperty(ref _btnListImageIsActive, value); }
        }

        private bool _btnStartAddIsActive = true;
        public bool BtnStartAddIsActive
        {
            get { return _btnStartAddIsActive; }
            set { SetProperty(ref _btnStartAddIsActive, value); }
        }

        private string _signatureTxt = "www.vcblog.top";
        public string SignatureTxt
        {
            get { return _signatureTxt; }
            set
            {
                SetProperty(ref _signatureTxt, value);
            }
        }

        private bool _topLeft_Check = false;
        public bool TopLeft_Check
        {
            get { return _topLeft_Check; }
            set
            {
                SetProperty(ref _topLeft_Check, value);
            }
        }

        private bool _up_Check = false;
        public bool Up_Check
        {
            get { return _up_Check; }
            set
            {
                SetProperty(ref _up_Check, value);
            }
        }

        private bool _topRight_Check = false;
        public bool TopRight_Check
        {
            get { return _topRight_Check; }
            set
            {
                SetProperty(ref _topRight_Check, value);
            }
        }

        private bool _left_Check = false;
        public bool Left_Check
        {
            get { return _left_Check; }
            set
            {
                SetProperty(ref _left_Check, value);
            }
        }

        private bool _center_Check = true;
        public bool Center_Check
        {
            get { return _center_Check; }
            set
            {
                SetProperty(ref _center_Check, value);
                if (value)
                {
                    _waterMarkImageInfo.ControlType = WaterMarkControlType.Center;
                }
            }
        }

        private bool _right_Check = false;
        public bool Right_Check
        {
            get { return _right_Check; }
            set
            {
                SetProperty(ref _right_Check, value);
            }
        }

        private bool _bottomLeft_Check = false;
        public bool BottomLeft_Check
        {
            get { return _bottomLeft_Check; }
            set
            {
                SetProperty(ref _bottomLeft_Check, value);
            }
        }

        private bool _down_Check = false;
        public bool Down_Check
        {
            get { return _down_Check; }
            set
            {
                SetProperty(ref _down_Check, value);
            }
        }

        private bool _bottomRight_Check = false;
        public bool BottomRight_Check
        {
            get { return _bottomRight_Check; }
            set
            {
                SetProperty(ref _bottomRight_Check, value);
            }
        }

        #endregion

        //public InteractionRequest<INotification> NotificationRequest { get; set; }
        public WaterMarkVM(/*ILoggerFacade logger,*/ IEventAggregator ea)
        {
            //_logger = logger;
            _sendEvent = ea;

            WaterMarkControlCommands = new DelegateCommand<string>(MoveOrRotateMark);
            FormatLineStyleControlCommands = new DelegateCommand<TextBox>(FormatLineStyle_Click);
            //StartAddWaterMarkControlCommands = new DelegateCommand<Image>(SaveWaterMark_Temp);
            //NotificationRequest = new InteractionRequest<INotification>();

            ReadUserSettings();

            SetButtnNoCheck();
            Center_Check = true;
            _waterMarkImageInfo.ControlType = WaterMarkControlType.Center;

            _sendEvent.GetEvent<MessagesNotificationEvent>().Publish(new MessageInfo() { MsgType = MessageType.ShowOnStatusBar, Message = ModuleName });
        }

        private void SendProgressInfo(ProgressInfo info)
        {
            _sendEvent.GetEvent<ProgressSentEvent>().Publish(info);
        }

        private void FormatLineStyle_Click(TextBox obj)
        {
            ShowFontDialog(obj);
        }

        /// <summary>
        /// 移动或旋转水印图片
        /// </summary>
        /// <param name="obj"></param>
        private void MoveOrRotateMark(string obj)
        {
            WaterMarkControlType type = (WaterMarkControlType)Enum.Parse(typeof(WaterMarkControlType), obj);
            switch (type)
            {
                case WaterMarkControlType.Center:
                    SetButtnNoCheck();
                    Center_Check = true;
                    _waterMarkImageInfo.ControlType = type;
                    break;
                case WaterMarkControlType.Left:
                    SetButtnNoCheck();
                    Left_Check = true;
                    _waterMarkImageInfo.ControlType = type;
                    break;
                case WaterMarkControlType.Right:
                    SetButtnNoCheck();
                    Right_Check = true;
                    _waterMarkImageInfo.ControlType = type;
                    break;
                case WaterMarkControlType.Up:
                    SetButtnNoCheck();
                    Up_Check = true;
                    _waterMarkImageInfo.ControlType = type;
                    break;
                case WaterMarkControlType.Down:
                    SetButtnNoCheck();
                    Down_Check = true;
                    _waterMarkImageInfo.ControlType = type;
                    break;
                case WaterMarkControlType.BottomLeft:
                    SetButtnNoCheck();
                    BottomLeft_Check = true;
                    _waterMarkImageInfo.ControlType = type;
                    break;
                case WaterMarkControlType.BottomRight:
                    SetButtnNoCheck();
                    BottomRight_Check = true;
                    _waterMarkImageInfo.ControlType = type;
                    break;
                case WaterMarkControlType.TopLeft:
                    SetButtnNoCheck();
                    TopLeft_Check = true;
                    _waterMarkImageInfo.ControlType = type;
                    break;
                case WaterMarkControlType.TopRight:
                    SetButtnNoCheck();
                    TopRight_Check = true;
                    _waterMarkImageInfo.ControlType = type;
                    break;
                case WaterMarkControlType.ZoomOut:
                    WaterMarkScale_X -= 0.05;
                    WaterMarkScale_Y -= 0.05;
                    break;
                case WaterMarkControlType.Enlarge:
                    WaterMarkScale_X += 0.05;
                    WaterMarkScale_Y += 0.05;
                    break;
                case WaterMarkControlType.Rotate_Left:
                    WaterMarkRoate -= 15;
                    break;
                case WaterMarkControlType.Rotate_Right:
                    WaterMarkRoate += 15;
                    break;
                case WaterMarkControlType.OriginSrc:
                    OriginImageSrc = FileDialogTools.OpenFolderBrowserDialog("选择待添加水印的图片路径");
                    break;
                case WaterMarkControlType.DestinationSrc:
                    DesImageSrc = FileDialogTools.OpenFolderBrowserDialog("选择添加水印后的图片保存路径");
                    break;
                case WaterMarkControlType.WaterMarkSrc:
                    WaterMarkImageSrc = FileDialogTools.OpenFileDialog("选择水印图片");
                    if (!WaterMarkImageSrc.Equals(""))
                    {
                        PreviewImage = GetBitmapImage(WaterMarkImageSrc);
                        ResetImagePosition();
                    }
                    break;
                case WaterMarkControlType.ListImages:
                    ScanFileList();
                    break;
                case WaterMarkControlType.SignatureTxt:
                    //添加文字水印
                    PreviewImage = ConvertFontToImage(SignatureTxt);
                    IsConvertFontToImage = true;
                    break;
                case WaterMarkControlType.StartAddWaterMark:
                    SaveUserSettings();
                    StartAddMark();
                    break;
            }
        }

        private void StartAddMark()
        {
            if (!_taskAddMarkThread_IsStart)
            {
                if (OriginImageSrc.Equals("") || DesImageSrc.Equals("") || WaterMarkImageSrc.Equals("") ||
                    !Directory.Exists(OriginImageSrc) || !Directory.Exists(DesImageSrc) || !File.Exists(WaterMarkImageSrc))
                {
                    //NotificationRequest.Raise(new Notification { Content = "图片路径和水印图片不能为无效值！", Title = "警告" });
                }
                else
                {
                    if (_filesLists.Count <= 0)
                    {
                        //NotificationRequest.Raise(new Notification { Content = "请先读取源文件图片列表！", Title = "警告" });
                        ProgressInfo += "警告：请先读取源文件图片列表！\r\n";
                    }
                    else
                    {
                        _waterMarkImageInfo.WaterMarkPosition = new Point(WaterMarkTranslate_X, WaterMarkTranslate_Y);
                        _waterMarkImageInfo.WaterMarkRotateAngle = WaterMarkRoate;
                        _waterMarkImageInfo.WaterMarkScale_X = WaterMarkScale_X;
                        _waterMarkImageInfo.WaterMarkScale_Y = WaterMarkScale_Y;
                        _waterMarkImageInfo.WaterMarkSize = new Size(PreviewImage.Width, PreviewImage.Height);
                        _waterMarkImageInfo.DestinationDir = DesImageSrc;
                        _waterMarkImageInfo.OriginDir = OriginImageSrc;
                        _waterMarkImageInfo.WaterMarkOpacity = ImageOpacity;

                        if (IsConvertFontToImage)
                        {
                            _waterMarkImageInfo.WaterMarkSrc = _tempPath;
                        }
                        else
                        {
                            _waterMarkImageInfo.WaterMarkSrc = WaterMarkImageSrc;
                        }

                        BtnListImageIsActive = false;
                        BtnStartAddIsActive = false;
                        _taskAddMarkThread = new Thread(new ParameterizedThreadStart(StartAddMaterMarkThread));
                        _taskAddMarkThread.Start();
                        _taskAddMarkThread_IsStart = true;
                    }
                }
            }
        }

        private void StartAddMaterMarkThread(object obj)
        {
            int i = 1;
            foreach (ImagesListInfo info in _filesLists)
            {
                SendProgressInfo(new ProgressInfo() { TotalProgress = _filesLists.Count, CurrentProgress = i });
                MakePictureGDI(info.FileSrc, _waterMarkImageInfo.WaterMarkSrc, "", info.FileSrc.Replace(_waterMarkImageInfo.OriginDir, _waterMarkImageInfo.DestinationDir));
                i++;
            }
            SendProgressInfo(new ProgressInfo() { TotalProgress = _filesLists.Count, CurrentProgress = 0 });
            BtnListImageIsActive = true;
            BtnStartAddIsActive = true;
            _taskAddMarkThread_IsStart = false;
        }

        private void ScanFileList()
        {
            if (!OriginImageSrc.Equals(""))
            {
                _filesLists.Clear();
                BtnListImageIsActive = false;
                _taskThread = new Thread(new ParameterizedThreadStart(ScanFilesListThread));
                _taskThread.Start(OriginImageSrc);
            }
            else
            {
                //NotificationRequest.Raise(new Notification { Content = "请选择图片源路径文件夹", Title = "警告" });
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
            }
        }
        private void OnIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler IsActiveChanged;

        private void ScanFilesListThread(object obj)
        {
            ProgressInfo += "读取待添加水印的图片列表：\r\n";
            ListFiles(new DirectoryInfo((string)obj));
            BtnListImageIsActive = true;
        }

        private void ListFiles(FileSystemInfo dirInfo)
        {
            if (!dirInfo.Exists)
            {
                return;
            }

            if (!(dirInfo is DirectoryInfo dir))
            {
                return;
            }

            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                SendProgressInfo(new ProgressInfo() { TotalProgress = files.Length, CurrentProgress = i + 1 });
                if (files[i] is FileInfo file)
                {
                    string fileEx = Path.GetExtension(file.FullName).Replace(".", "");
                    if (FileExList.Contains(fileEx))
                    {
                        ImagesListInfo fileInfo = new ImagesListInfo
                        {
                            FileName = file.Name,
                            FileSrc = file.FullName
                        };
                        _filesLists.Add(fileInfo);

                        ProgressInfo += file.FullName + "\r\n";
                    }
                }
                else
                {
                    ListFiles(files[i]);
                }
            }
            SendProgressInfo(new ProgressInfo() { TotalProgress = files.Length, CurrentProgress = 0 });
        }

        #region 图片合成

        private void MakePicture(string originImg, string waterMarkImg, string signature, string destination)
        {
            string fileEx = Path.GetExtension(originImg).Replace(".", "");
            string desDir = Path.GetDirectoryName(destination);
            Directory.CreateDirectory(desDir);

            BitmapSource bgImage = GetBitmapImage(originImg);

            BitmapSource headerImage = GetBitmapImage(waterMarkImg);

            RenderTargetBitmap composeImage = new RenderTargetBitmap(bgImage.PixelWidth, bgImage.PixelHeight, bgImage.DpiX, bgImage.DpiY, PixelFormats.Default);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(bgImage, new Rect(0, 0, bgImage.Width, bgImage.Height));
            //添加水印
            double waterMark_Y = (composeImage.Height / 2) - headerImage.Height / 2 + (_waterMarkImageInfo.WaterMarkPosition.Y / 360.0) * composeImage.Height;
            double waterMark_X = (_waterMarkImageInfo.WaterMarkPosition.X / 360.0) * composeImage.Width;
            double w = headerImage.Width;
            double h = headerImage.Height;
            double wh = w / h;
            if (w > bgImage.Width)
            {
                w = bgImage.Width / 2;
                h = w / wh;
            }
            if (h > bgImage.Height)
            {
                h = bgImage.Height / 2;
                w = h * wh;
            }
            drawingContext.DrawImage(headerImage, new Rect(waterMark_X, waterMark_Y, w * _waterMarkImageInfo.WaterMarkScale_X, h * _waterMarkImageInfo.WaterMarkScale_Y));

            drawingContext.Close();
            composeImage.Render(drawingVisual);

            FileStream fileStream = null;

            try
            {
                fileStream = new FileStream(destination, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                switch (fileEx)
                {
                    case "png":
                        //定义一个PNG编码器
                        PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();

                        pngBitmapEncoder.Frames.Add(BitmapFrame.Create(composeImage));
                        pngBitmapEncoder.Save(fileStream);
                        break;
                    case "bmp":
                        //定义一个BMP编码器
                        BmpBitmapEncoder bmpBitmapEncoder = new BmpBitmapEncoder();

                        bmpBitmapEncoder.Frames.Add(BitmapFrame.Create(composeImage));
                        bmpBitmapEncoder.Save(fileStream);
                        break;
                    default:
                        //定义一个JPG编码器
                        JpegBitmapEncoder bitmapEncoder = new JpegBitmapEncoder();

                        bitmapEncoder.Frames.Add(BitmapFrame.Create(composeImage));
                        bitmapEncoder.Save(fileStream);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != fileStream)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }

            ProgressInfo += "添加水印图片：" + originImg + "，保存路径：" + destination + "\r\n";
        }

        private BitmapSource ConvertFontToImage(string signatureTxt)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            FormattedText formattedText = new FormattedText(signatureTxt, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
        new Typeface(_fontFamily), _fontSize * (96.0 / 72.0), _foreground);

            formattedText.SetFontWeight(_fontWeight);
            formattedText.SetFontStyle(_fontStyle);
            double x = formattedText.WidthIncludingTrailingWhitespace + 10;
            double y = formattedText.Height + 10;
            drawingContext.DrawText(formattedText, new Point(0, 0));
            drawingContext.Close();

            RenderTargetBitmap renderTargetBitmap = RenderVisaulToBitmap(drawingVisual, (int)x, (int)y);

            if (!Directory.Exists(Path.GetDirectoryName(_tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_tempPath));
            }
            PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
            FileStream fs = null;
            try
            {
                fs = new FileStream(_tempPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                pngBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                pngBitmapEncoder.Save(fs);
            }
            catch (Exception ex)
            {
                //_logger.Log(ex.Message, Category.Exception, Priority.High);
            }
            finally
            {
                if (null != fs)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

            return renderTargetBitmap;
        }

        private void SaveWaterMark_Temp(Image image)
        {
            SaveUserSettings();

            //double w = image.ActualWidth * WaterMarkScale_X;
            //double h = image.ActualHeight * WaterMarkScale_Y;

            //int width = Math.Abs((int)Math.Round(Math.Cos(Math.Abs(WaterMarkRoate)) * w + Math.Sin(Math.Abs(WaterMarkRoate)) * h, 0));
            //int height = Math.Abs((int)Math.Round(Math.Sin(Math.Abs(WaterMarkRoate)) * w + Math.Cos(Math.Abs(WaterMarkRoate)) * h, 0));

            RenderTargetBitmap renderTargetBitmap = RenderVisaulToBitmap(image, (int)image.ActualWidth, (int)image.ActualHeight);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            FileStream fs = null;
            try
            {
                fs = new FileStream(_tempPath, FileMode.OpenOrCreate);
                encoder.Save(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != fs)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }

            StartAddMark();
        }

        private RenderTargetBitmap RenderVisaulToBitmap(Visual vsual, int width, int height)
        {
            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            rtb.Render(vsual);
            return rtb;
        }

        private void MakePictureGDI(string originImg, string waterMarkImg, string signature, string destination)
        {
            string fileEx = Path.GetExtension(originImg).Replace(".", "");
            string desDir = Path.GetDirectoryName(destination);
            Directory.CreateDirectory(desDir);

            GDI.Image bgImage = GDI.Image.FromFile(originImg);
            GDI.Image headerImage = GDI.Image.FromFile(waterMarkImg);

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(bgImage.Width, bgImage.Height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);

            //先在画板上面画底图
            g.DrawImage(bgImage, new GDI.Rectangle(0, 0, bitmap.Width, bitmap.Height));

            int waterMark_Y = 0;
            int waterMark_X = 0;
            int width = headerImage.Width;
            int height = headerImage.Height;
            double wh = width / height;
            if (width > bgImage.Width)
            {
                width = bgImage.Width / 5 * 4;
                height = (int)Math.Ceiling(width / wh);
            }
            if (height > bgImage.Height)
            {
                height = bgImage.Height / 5 * 4;
                width = (int)Math.Ceiling(height * wh);
            }

            switch (_waterMarkImageInfo.ControlType)
            {
                case WaterMarkControlType.Center:
                    waterMark_Y = bgImage.Height / 2 - height / 2;
                    waterMark_X = bgImage.Width / 2 - width / 2;
                    break;
                case WaterMarkControlType.Left:
                    waterMark_Y = bgImage.Height / 2 - height / 2;
                    waterMark_X = 0;
                    break;
                case WaterMarkControlType.Right:
                    waterMark_Y = bgImage.Height / 2 - height / 2;
                    waterMark_X = bgImage.Width - width;
                    break;
                case WaterMarkControlType.Up:
                    waterMark_Y = 0;
                    waterMark_X = bgImage.Width / 2 - width / 2;
                    break;
                case WaterMarkControlType.Down:
                    waterMark_Y = bgImage.Height - height;
                    waterMark_X = bgImage.Width / 2 - width / 2;
                    break;
                case WaterMarkControlType.BottomLeft:
                    waterMark_Y = bgImage.Height - height;
                    waterMark_X = 0;
                    break;
                case WaterMarkControlType.BottomRight:
                    waterMark_Y = bgImage.Height - height;
                    waterMark_X = bgImage.Width - width;
                    break;
                case WaterMarkControlType.TopLeft:
                    waterMark_Y = 0;
                    waterMark_X = 0;
                    break;
                case WaterMarkControlType.TopRight:
                    waterMark_Y = 0;
                    waterMark_X = bgImage.Width - width;
                    break;
            }
            if (_waterMarkImageInfo.WaterMarkOpacity != 100)
            {
                GDI.Imaging.ImageAttributes imageAtt = GetAlphaImgAttr(_waterMarkImageInfo.WaterMarkOpacity);
                g.DrawImage(headerImage, new GDI.Rectangle(waterMark_X, waterMark_Y, width, height),
                                     0, 0, headerImage.Width, headerImage.Height,
                                     GDI.GraphicsUnit.Pixel, imageAtt);
            }
            else
            {
                g.DrawImage(headerImage, new GDI.Rectangle(waterMark_X, waterMark_Y, width, height),
                                     new GDI.Rectangle(0, 0, headerImage.Width, headerImage.Height),
                                     GDI.GraphicsUnit.Pixel);
            }

            ////在画板上写文字
            //using (GDI.Font f = new GDI.Font("Arial", 20, GDI.FontStyle.Bold))
            //{
            //    using (GDI.Brush b = new GDI.SolidBrush(GDI.Color.White))
            //    {
            //        float fontWidth = g.MeasureString(signature, f).Width;
            //        float x2 = (bgImage.Width / 2 - fontWidth) / 2;
            //        float y2 = y + headerImage.Height + 20;
            //        g.DrawString(signature, f, b, x2, y2);
            //    }
            //}

            //FileStream fileStream = null;

            try
            {
                //fileStream = new FileStream(destination, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //BitmapSource composeImage = ToBitmapSource(bitmap);

                switch (fileEx)
                {
                    case "png":
                        bitmap.Save(destination, System.Drawing.Imaging.ImageFormat.Png);
                        //定义一个PNG编码器
                        //PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();

                        //pngBitmapEncoder.Frames.Add(BitmapFrame.Create(composeImage));
                        //pngBitmapEncoder.Save(fileStream);
                        break;
                    case "bmp":
                        bitmap.Save(destination, System.Drawing.Imaging.ImageFormat.Bmp);
                        //定义一个BMP编码器
                        //BmpBitmapEncoder bmpBitmapEncoder = new BmpBitmapEncoder();

                        //bmpBitmapEncoder.Frames.Add(BitmapFrame.Create(composeImage));
                        //bmpBitmapEncoder.Save(fileStream);
                        break;
                    default:
                        bitmap.Save(destination, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //定义一个JPG编码器
                        //JpegBitmapEncoder bitmapEncoder = new JpegBitmapEncoder();

                        //bitmapEncoder.Frames.Add(BitmapFrame.Create(composeImage));
                        //bitmapEncoder.Save(fileStream);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //if (null != fileStream)
                //{
                //    fileStream.Close();
                //    fileStream.Dispose();
                //}
                bgImage.Dispose();
                headerImage.Dispose();
                g.Dispose();
            }

            ProgressInfo += "添加水印图片：" + originImg + "，保存路径：" + destination + "\r\n";
        }

        #region GDI+ Image 转化成 BitmapSource

        [System.Runtime.InteropServices.DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public BitmapSource ToBitmapSource(GDI.Bitmap bitmap)
        {
            IntPtr ip = bitmap.GetHbitmap();

            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ip, IntPtr.Zero, System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ip);//释放对象
            return bitmapSource;
        }

        #endregion

        #endregion

        public BitmapImage GetBitmapImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();

            if (File.Exists(imagePath))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
            }

            return bitmap;
        }

        /// <summary>
        /// 重置水印图片设置
        /// </summary>
        private void ResetImagePosition()
        {
            Position_X = 0;
            Position_Y = 0;
            WaterMarkRoate = 0;
            WaterMarkScale_X = 1;
            WaterMarkScale_Y = 1;
            WaterMarkTranslate_X = 0;
            WaterMarkTranslate_Y = 0;
        }

        /// <summary>
        /// 保存用户使用记录
        /// </summary>
        private void SaveUserSettings()
        {
            OperateSettingsFile.WriteSettingsData(Section_WaterMark, Key_OriginImageSrc, OriginImageSrc);
            OperateSettingsFile.WriteSettingsData(Section_WaterMark, Key_DesImageSrc, DesImageSrc);
            OperateSettingsFile.WriteSettingsData(Section_WaterMark, Key_WaterMarkImageSrc, WaterMarkImageSrc);
            OperateSettingsFile.WriteSettingsData(Section_WaterMark, Key_SignatureTxt, SignatureTxt);
            OperateSettingsFile.WriteSettingsData(Section_WaterMark, Key_WaterMarkOpacity, ImageOpacity.ToString());
        }

        /// <summary>
        /// 读取用户使用记录
        /// </summary>
        private void ReadUserSettings()
        {
            OriginImageSrc = OperateSettingsFile.ReadSettingsData(Section_WaterMark, Key_OriginImageSrc, OriginImageSrc);
            DesImageSrc = OperateSettingsFile.ReadSettingsData(Section_WaterMark, Key_DesImageSrc, DesImageSrc);
            WaterMarkImageSrc = OperateSettingsFile.ReadSettingsData(Section_WaterMark, Key_WaterMarkImageSrc, WaterMarkImageSrc);
            SignatureTxt = OperateSettingsFile.ReadSettingsData(Section_WaterMark, Key_SignatureTxt, SignatureTxt);

            if (!int.TryParse(OperateSettingsFile.ReadSettingsData(Section_WaterMark, Key_WaterMarkOpacity, ImageOpacity.ToString()), out int valueTmp))
            {
                ImageOpacity = valueTmp;
            }
            else
            {
                ImageOpacity = 80;
            }
        }

        private void ShowFontDialog(TextBox textBox)
        {
            FontChooser fontChooser = new FontChooser();

            fontChooser.SetPropertiesFromObject(textBox);
            fontChooser.PreviewSampleText = textBox.SelectedText;

            if (fontChooser.ShowDialog().Value)
            {
                fontChooser.ApplyPropertiesToObject(textBox);
                BrushConverter brushConverter = new BrushConverter();
                textBox.Foreground = (Brush)brushConverter.ConvertFromString(fontChooser.GetSelectColor().Replace("System.Windows.Media.Color", ""));

                _fontSize = double.Parse(fontChooser.GetFontSize());
                _fontFamily = textBox.FontFamily.ToString();
                _foreground = textBox.Foreground;
                _fontWeight = textBox.FontWeight;
                _fontStyle = textBox.FontStyle;
            }
        }

        /// <summary>
        /// 设置所有按钮未选中
        /// </summary>
        private void SetButtnNoCheck()
        {
            TopLeft_Check = false;
            TopRight_Check = false;
            BottomLeft_Check = false;
            BottomRight_Check = false;
            Up_Check = false;
            Down_Check = false;
            Left_Check = false;
            Right_Check = false;
            Center_Check = false;
        }

        /// <summary>
        /// 获取一个带有透明度的ImageAttributes
        /// </summary>
        /// <param name="opcity"></param>
        /// <returns></returns>
        public GDI.Imaging.ImageAttributes GetAlphaImgAttr(int opcity)
        {
            if (opcity < 0 || opcity > 100)
            {
                throw new ArgumentOutOfRangeException("opcity 值为 0~100");
            }
            //颜色矩阵
            float[][] matrixItems =
            {
                new float[]{1,0,0,0,0},
                new float[]{0,1,0,0,0},
                new float[]{0,0,1,0,0},
                new float[]{0,0,0,(float)opcity / 100,0},
                new float[]{0,0,0,0,1}
            };
            GDI.Imaging.ColorMatrix colorMatrix = new GDI.Imaging.ColorMatrix(matrixItems);
            GDI.Imaging.ImageAttributes imageAtt = new GDI.Imaging.ImageAttributes();
            imageAtt.SetColorMatrix(colorMatrix, GDI.Imaging.ColorMatrixFlag.Default, GDI.Imaging.ColorAdjustType.Bitmap);
            return imageAtt;
        }
    }
}
