using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;

namespace GMap.NET.MapProviders
{
    public class BaiduOrdinaryMapProvider : BaiduMapProviderBase
    {
        public static readonly BaiduOrdinaryMapProvider Instance;

        BaiduOrdinaryMapProvider()
        {            
        }

        static BaiduOrdinaryMapProvider()
        {
            Instance = new BaiduOrdinaryMapProvider();
        }

        readonly Guid id = new Guid("C16BB8A8-C8A4-4F0A-8D43-0A8707C74E39");
        public override Guid Id
        {
            get
            {
                return id;
            }
        }

        readonly string name = "BaiduOrdinaryMap";
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

        static readonly string UrlFormat = "http://maponline{3}.bdimg.com/tile/?qt=vtile&x={0}&y={1}&z={2}&lang={4}&styles=pl&scaler=1&udt=20200225";

    }
}
