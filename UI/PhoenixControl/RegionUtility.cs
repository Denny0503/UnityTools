using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhoenixControl
{
    public static class RegionUtility
    {
        /// <summary>
        /// 已打开的界面列表
        /// </summary>
        private static List<string> RegionViews = new List<string>();

        /// <summary>
        /// 添加视图
        /// </summary>
        /// <param name="header"></param>
        public static void AddRegionViewsHeader(string header)
        {
            if (!RegionViews.Contains(header))
            {
                RegionViews.Add(header);
            }
        }

        /// <summary>
        /// 移除视图
        /// </summary>
        /// <param name="header"></param>
        public static void RemoveRegionViewsHeader(string header)
        {
            if (RegionViews.Contains(header))
            {
                RegionViews.Remove(header);
            }
        }

        /// <summary>
        /// 是否存在视图
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static bool ExistRegionViews(string header)
        {
            return RegionViews.Contains(header);
        }

        /// <summary>
        /// 清空视图
        /// </summary>
        public static void ClearRegionViews()
        {
            RegionViews.Clear();
        }

        /// <summary>
        /// 重复导航的视图名称
        /// </summary>
        public static string RegionName { set; get; } = "";

        public static T GetInterfaceFromView<T>(object view)
        {
            if (view is T)
            {
                return (T)view;
            }
            else
            {
                FrameworkElement viewAsFrameworkElement = view as FrameworkElement;

                if (viewAsFrameworkElement?.DataContext is T)
                {
                    return (T)viewAsFrameworkElement.DataContext;
                }
            }

            return default(T);
        }
    }
}
