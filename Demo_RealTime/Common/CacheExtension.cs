using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Demo_RealTime.Common
{
    public class CacheExtension
    {
        public static object GetCacheList()
        {
            var obj = HttpContext.Current.Cache.GetEnumerator();
            return obj;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        public static object GetCache(string key)
        {
            object obj = HttpContext.Current.Cache.Get(key);
            return obj;
        }

        /// <summary>
        /// 添加 绝对到期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="minutes">保存多少分钟</param>
        public static void AddCacheAbsolute(string key, object obj, int minutes)
        {
            HttpContext.Current.Cache.Add(key, obj, null, DateTime.Now.AddMinutes(minutes), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }

        /// <summary>
        /// 相对到期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="minutes"></param>
        public static void AddCacheRelative(string key, object obj, int minutes)
        {
            HttpContext.Current.Cache.Add(key, obj, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10), CacheItemPriority.Default, null);
        }

    }
}