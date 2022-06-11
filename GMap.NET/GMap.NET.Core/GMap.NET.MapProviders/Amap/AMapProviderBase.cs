using GMap.NET.MapProviders;
using GMap.NET.Projections;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMap.NET.MapProviders
{
    public abstract class AMapProviderBase : GMapProvider
    {
        public AMapProviderBase()
        {
            MaxZoom = 25;
            RefererUrl = "https://www.amap.com";
            Copyright = string.Format("©{0} 高德 Corporation, GS(2019)6379号 - 甲测资字11002004 - 京ICP证 070711号 - 京公网安备 11010502030880号", DateTime.Today.Year);
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
