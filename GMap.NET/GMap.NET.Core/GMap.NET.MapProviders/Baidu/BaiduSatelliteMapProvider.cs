using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;

namespace GMap.NET.MapProviders
{
    public class BaiduSatelliteMapProvider : BaiduMapProviderBase
    {
        public static readonly BaiduSatelliteMapProvider Instance;

        BaiduSatelliteMapProvider()
        {
            RefererUrl = string.Format("http://ditu.{0}/", "com");
        }

        static BaiduSatelliteMapProvider()
        {
            Instance = new BaiduSatelliteMapProvider();
        }

        public string Version = "m@298";
        readonly Guid id = new Guid("8BEB3061-8867-49FE-A3A8-D355355AB737");
        public override Guid Id
        {
            get
            {
                return id;
            }
        }

        readonly string name = "BaiduSatelliteMap";
        public override string Name
        {
            get
            {
                return name;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom, LanguageStr);

            return GetTileImageUsingHttp(url);
        }

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            zoom -= 1;
            var offsetX = Math.Pow(2, zoom);
            var offsetY = offsetX - 1;

            var numX = pos.X - offsetX;
            var numY = -pos.Y + offsetY;

            zoom += 1;
            var num = (pos.X + pos.Y) % 4;

            return string.Format(UrlFormat, numX, numY, zoom, num, language);
        }

        static readonly string UrlFormat = "http://shangetu{3}.map.bdimg.com/it/u=x={0};y={1};z={2};lang={4};v=009;type=sate&fm=46&udt=20200225";
    }
}
