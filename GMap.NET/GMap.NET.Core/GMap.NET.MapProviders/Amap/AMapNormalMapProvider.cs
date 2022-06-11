
namespace GMap.NET.MapProviders
{
    using System;

    public class AMapNormalMapProvider : AMapProviderBase
    {
        public static readonly AMapNormalMapProvider Instance;

        AMapNormalMapProvider()
        {
            MaxZoom = 18;
            MinZoom = 3;
        }

        readonly Guid id = new Guid("4650C8C4-CB65-4311-87B4-BF8912633DCB");
        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "AMapNormalMap";
        public override string Name
        {
            get
            {
                return name;
            }
        }

        static AMapNormalMapProvider()
        {
            Instance = new AMapNormalMapProvider();
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            try
            {
                var num = (pos.X + pos.Y) % 4 + 1;
                string url = string.Format(UrlFormat, pos.X, pos.Y, zoom, LanguageStr, num);
                return GetTileImageUsingHttp(url);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static readonly string UrlFormat = "http://webrd0{4}.is.autonavi.com/appmaptile?lang={3}&size=1&scale=1&style=8&x={0}&y={1}&z={2}";

        /*图片陈旧*/
        //static readonly string UrlFormat = "http://wprd0{4}.is.autonavi.com/appmaptile?x={0}&y={1}&z={2}&lang={3}&size=1&scl=1&style=7";
    }
}


//Normal:
//{
//    url: 'http://webrd01.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x={x}&y={y}&z={z}'
//},
//Satellite: {
//    url: 'http://webst01.is.autonavi.com/appmaptile?style=6&x={x}&y={y}&z={z}'
//},
//Satellite_Label: {
//    url: 'http://webst01.is.autonavi.com/appmaptile?style=8&x={x}&y={y}&z={z}'
//}

//http://wprd0{1-4}.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&lang=zh_cn&size=1&scl=1&style=7 为矢量图（含路网、含注记）

//http://wprd0{1-4}.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&lang=zh_cn&size=1&scl=2&style=7 为矢量图（含路网，不含注记）

//http://wprd0{1-4}.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&lang=zh_cn&size=1&scl=1&style=6 为影像底图（不含路网，不含注记）

//http://wprd0{1-4}.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&lang=zh_cn&size=1&scl=2&style=6 为影像底图（不含路网、不含注记）

//http://wprd0{1-4}.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&lang=zh_cn&size=1&scl=1&style=8 为影像路图（含路网，含注记）

//http://wprd0{1-4}.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&lang=zh_cn&size=1&scl=2&style=8 为影像路网（含路网，不含注记）
//————————————————
//版权声明：本文为CSDN博主「fredricen」的原创文章，遵循 CC 4.0 BY-SA 版权协议，转载请附上原文出处链接及本声明。
//原文链接：https://blog.csdn.net/fredricen/article/details/77189453