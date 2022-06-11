using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using GMap.NET.Projections;

namespace GMap.NET.MapProviders
{
    /// <summary>
    /// 百度地图数据源的抽象类
    /// </summary>
    public abstract class BaiduMapProviderBase : GMapProvider
    {
        public BaiduMapProviderBase()
        {
            MaxZoom = null;
            RefererUrl = "https://map.baidu.com/";
            Copyright = string.Format("©{0} Baidu - GS(2019)5218号 - 甲测资字1100930 - 京ICP证030173号 ", DateTime.Today.Year);
        }

        public override PureProjection Projection
        {
            get { return MercatorProjection.Instance; }
        }

        GMapProvider[] overlays;
        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider[] { this };
                }
                return overlays;
            }
        }
    }
}
