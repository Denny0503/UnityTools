using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;

namespace GMap.NET.MapProviders
{
    public class BaiduHybridMapProvider : BaiduMapProviderBase
    {
        public static readonly BaiduHybridMapProvider Instance;

        BaiduHybridMapProvider()
        { }

        static BaiduHybridMapProvider()
        {
            Instance = new BaiduHybridMapProvider();
        }

        readonly Guid id = new Guid("D3394179-FCD2-4731-865A-79CC77667786");

        public override Guid Id { get { return id; } }

        readonly string name = "BaiduHybridMap";
        public override string Name { get { return name; } }

        GMapProvider[] overlays;
        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider[] { BaiduSatelliteMapProvider.Instance, this };
                }
                return overlays;
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

        static readonly string UrlFormat = "http://maponline{3}.bdimg.com/tile/?qt=vtile&x={0}&y={1}&z={2}&lang={4}&styles=sl&udt=20200225";

        //static readonly string UrlFormat = "http://shangetu3.map.bdimg.com/it/u=x={0};y={1};z={2};v=009;type=sate&fm=46&udt=20150504&app=webearth2&v=009&udt=20150601";
    }
}
