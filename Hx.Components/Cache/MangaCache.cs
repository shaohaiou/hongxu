using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Collections;
using System.Text.RegularExpressions;

namespace Hx.Components
{
    /// <summary>
    /// 缓存
    /// </summary>
    public class MangaCache
    {
        public static readonly int DayFactor = 17280;
        public static readonly int HourFactor = 720;
        public static readonly int MinuteFactor = 12;
        public static readonly double SecondFactor = 0.2;
        public static readonly int Timeout = 10 * MinuteFactor;

        private static readonly Cache _cache;
        //private static readonly ICacheStrategy _membercache;
        private static int Factor = 5;

        public static void ReSetFactor(int cacheFactor)
        {
            Factor = cacheFactor;
        }

        public static int GetFactor()
        {
            return Factor;
        }

        static MangaCache()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                //Web应用程序
                _cache = context.Cache;
            }
            else
            {
                //非Web应用程序
                _cache = HttpRuntime.Cache;
            }
        }

        #region 缓存

        public static void Add(string key, object o, int time, TimeSpan timespan, CacheItemPriority priority, CacheItemRemovedCallback callback)
        {
            if (o == null)
            {
                return;
            }
            _cache.Insert(key, o, null, DateTime.Now.AddSeconds(time), timespan, priority, callback);
        }

        public static void Add(string key, object o, int time)
        {
            if (o == null)
            {
                return;
            }
            _cache.Insert(key, o, null, DateTime.Now.AddSeconds(time), TimeSpan.Zero, CacheItemPriority.Normal, null);
            //ExtendedCacheStoreServer.Instance().Add(key, o, time);
        }

        public static void Add(string key, object o)
        {
            if (Factor == 0 || o == null)
            {
                return;
            }
            _cache.Insert(key, o, null, DateTime.Now.AddSeconds(Timeout * Factor / 2), TimeSpan.Zero, CacheItemPriority.Normal, null);
            //ExtendedCacheStoreServer.Instance().Add(key, o, Timeout * Factor);
        }

        public static void Update(string key, object o, int time)
        {
            if (o == null)
            {
                return;
            }
            _cache.Insert(key, o, null, DateTime.Now.AddSeconds(time), TimeSpan.Zero, CacheItemPriority.Normal, null);
            //ExtendedCacheStoreServer.Instance().Add(key, o, time);

        }

        public static void Update(string key, object o)
        {
            if (Factor == 0 || o == null)
            {
                return;
            }

            _cache.Insert(key, o, null, DateTime.Now.AddSeconds(Timeout * Factor), TimeSpan.Zero, CacheItemPriority.Normal, null);
            //ExtendedCacheStoreServer.Instance().Add(key, o, Timeout * Factor);
        }

        public static void Max(string key, object o)
        {
            if (Factor == 0 || o == null)
            {
                return;
            }
            _cache.Insert(key, o, null, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
            MaxMemberCache(key, o);
        }

        public static object Get(string key)
        {
            if (Factor <= 0)
            {
                return null;
            }
            object o = _cache.Get(key);
            if (o == null)
            {
                o = GetMemberCache(key);
                if (o != null)
                {
                    AddLocal(key, o);
                }
            }
            return o;
        }



        public static void Remove(string key)
        {
            _cache.Remove(key);
            RemoveMemberCache(key);
        }

        public static void RemoveByPattern(string key)
        {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            Regex regex = new Regex(key, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            while (CacheEnum.MoveNext())
            {
                if (regex.IsMatch(CacheEnum.Key.ToString()))
                {
                    Remove(CacheEnum.Key.ToString());
                }
            }
        }


        #endregion

        #region 本地缓存

        public static void AddLocal(string key, object o)
        {
            if (Factor == 0)
            {
                return;
            }
            AddLocal(key, o, Timeout * Factor);
        }

        public static void AddLocal(string key, object o, int time)
        {
            if (o == null)
            {
                return;
            }
            _cache.Insert(key, o, null, DateTime.Now.AddSeconds(time), TimeSpan.Zero, CacheItemPriority.Normal, null);
        }

        public static void AddLocalWithKey(string key, object o, string withkey)
        {
            if (Factor == 0)
            {
                return;
            }
            AddLocalWithKey(key, o, Timeout * Factor, withkey);
        }

        public static void AddLocalWithKey(string key, object o, int time, string withkey)
        {
            if (o == null)
            {
                return;
            }
            string[] keys = new string[1];
            keys[0] = withkey;
            _cache.Insert(key, o, new CacheDependency(null, keys), DateTime.Now.AddSeconds(time), TimeSpan.Zero, CacheItemPriority.Normal, null);
        }

        public static void AddLocalWithFile(string key, object o, string file)
        {
            if (Factor == 0)
            {
                return;
            }
            AddLocalWithFile(key, o, Timeout * Factor, file);
        }

        public static void AddLocalWithFile(string key, object o, int time, string file)
        {
            if (o == null)
            {
                return;
            }
            _cache.Insert(key, o, new CacheDependency(file), DateTime.Now.AddSeconds(time), TimeSpan.Zero, CacheItemPriority.Normal, null);
        }

        public static void AddLocal(string key, object o, string[] files, string[] withkey, int time, CacheItemRemovedCallback callback)
        {
            if (o == null)
            {
                return;
            }
            if ((files == null || files.Length == 0) && (withkey == null || withkey.Length == 0))
            {
                _cache.Insert(key, o, null, DateTime.Now.AddSeconds(time), TimeSpan.Zero, CacheItemPriority.Normal, callback);
            }
            else
            {
                _cache.Insert(key, o, new CacheDependency(files, withkey), DateTime.Now.AddSeconds(time), TimeSpan.Zero, CacheItemPriority.Normal, callback);
            }
        }

        public static void MaxLocal(string key, object o)
        {
            if (o == null)
            {
                return;
            }
            _cache.Insert(key, o, null, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
        }

        public static void MaxLocalWithFile(string key, object o, string file, CacheItemRemovedCallback callback)
        {
            if (o == null)
            {
                return;
            }
            _cache.Insert(key, o, new CacheDependency(file), DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, callback);
        }

        public static void MaxLocalWithFile(string key, object o, string file)
        {
            if (o == null)
            {
                return;
            }
            _cache.Insert(key, o, new CacheDependency(file), DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
        }

        public static void MaxLocalWithKey(string key, object o, string withkey)
        {
            if (o == null)
            {
                return;
            }
            string[] keys = new string[1];
            keys[0] = withkey;
            _cache.Insert(key, o, new CacheDependency(null, keys), DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
        }

        public static void MaxLocalWithKey(string key, object o, string withkey, CacheItemRemovedCallback callback)
        {
            if (o == null)
            {
                return;
            }
            string[] keys = new string[1];
            keys[0] = withkey;
            _cache.Insert(key, o, new CacheDependency(null, keys), DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, callback);
        }

        public static void MaxLocal(string key, object o, string[] files, string[] withkey, CacheItemRemovedCallback callback)
        {
            if (o == null)
            {
                return;
            }
            if ((files == null || files.Length == 0) && (withkey == null || withkey.Length == 0))
            {
                _cache.Insert(key, o, null, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, callback);
            }
            else
            {
                _cache.Insert(key, o, new CacheDependency(files, withkey), DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, callback);
            }
        }

        public static object GetLocal(string key)
        {
            object o = _cache.Get(key);
            return o;
        }

        public static void RemoveLocal(string key)
        {
            _cache.Remove(key);
        }

        public static void RemoveLocalByPattern(string key)
        {
            //IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            //Regex regex = new Regex(key, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            //while (CacheEnum.MoveNext())
            //{
            //    if (regex.IsMatch(CacheEnum.Key.ToString()))
            //    {
            //        _cache.Remove(CacheEnum.Key.ToString());
            //    }
            //}
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();

            while (CacheEnum.MoveNext())
            {
                if (CacheEnum.Key.ToString().IndexOf(key) >= 0)
                {
                    _cache.Remove(CacheEnum.Key.ToString());
                }
            }
        }

        #endregion

        #region 外部缓存

        public static void AddMemberCache(string key, object o)
        {
            if (Factor == 0 || o == null)
            {
                return;
            }
            //if (!ExtendedCacheStoreServer.Instance().Add(key, o, Timeout))
            //{
            //    _cache.Insert(key, o, null, DateTime.Now.AddSeconds(Timeout), TimeSpan.Zero, CacheItemPriority.Normal, null);
            //}
        }

        public static void AddMemberCache(string key, object o, int time)
        {
            if (o == null)
            {
                return;
            }


            //if (!ExtendedCacheStoreServer.Instance().Add(key, o, time))
            //{
            //    _cache.Insert(key, o, null, DateTime.Now.AddSeconds(time), TimeSpan.Zero, CacheItemPriority.Normal, null);
            //}
        }

        public static void MaxMemberCache(string key, object o)
        {
            if (o == null)
            {
                return;
            }
            //if (!ExtendedCacheStoreServer.Instance().Max(key, o))
            //{
            //    _cache.Insert(key, o, null, DateTime.Now.AddHours(1), TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
            //}
        }

        public static void RemoveMemberCache(string key)
        {
            //if (!ExtendedCacheStoreServer.Instance().Remove(key))
            //{
            //    _cache.Remove(key);
            //}
        }

        public static object GetMemberCache(string key)
        {

            object o = _cache.Get(key);
            //if (o == null)
            //{
            //    return ExtendedCacheStoreServer.Instance().Get(key);
            //}

            return o;
        }

        //public static bool MemberStatus
        //{
        //    get
        //    {
        //        return ExtendedCacheStoreServer.Instance().Status;
        //    }
        //}

        #endregion
    }
}
