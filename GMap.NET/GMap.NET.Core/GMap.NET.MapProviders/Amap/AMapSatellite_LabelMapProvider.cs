
namespace GMap.NET.MapProviders
{
    using GMap.NET.Projections;
    using System;
   
    public class AMapSatellite_LabelMapProvider : GoogleMapProviderBase
    {
        public static readonly AMapSatellite_LabelMapProvider Instance;

        readonly Guid id = new Guid("EBDAAC67-F01D-44CE-9D3A-717CC1DDA439");
        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "AMapSatellite_LabelMap";
        public override string Name
        {
            get
            {
                return name;
            }
        }

        static AMapSatellite_LabelMapProvider()
        {
            Instance = new AMapSatellite_LabelMapProvider();
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            try
            {
                string url = MakeTileImageUrl(pos, zoom);
                return GetTileImageUsingHttp(url);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        string MakeTileImageUrl(GPoint pos, int zoom)
        {
            var num = (pos.X + pos.Y) % 4 + 1;
            string url = string.Format(UrlFormat, pos.X, pos.Y, zoom, num);
            return url;
        }

        //卫星地图
        static readonly string UrlFormat = "http://webst0{3}.is.autonavi.com/appmaptile?style=8&x={0}&y={1}&z={2}";
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