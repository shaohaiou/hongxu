﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Car;
using Hx.Components;
using System.Text.RegularExpressions;
using Hx.Tools.Web;
using Hx.Tools;

namespace Hx.TaskAndJob.Job
{
    public class RefreshBackadminCacheJob : IJob
    {
        public void Execute(System.Xml.XmlNode node)
        {
            try
            {
                double weather = 0;
                string url_zh = "http://www.weather.com.cn/data/sk/101210705.html";
                string httpstr_zh = Http.GetPage(url_zh);
                double curweather_zh = 0;
                Regex r_zh = new Regex("\"temp\":\"(\\d+)\"");
                if (r_zh.IsMatch(httpstr_zh))
                {
                    double.TryParse(r_zh.Match(httpstr_zh).Groups[1].Value, out curweather_zh);
                }

                //string url_yahoo = "http://weather.yahooapis.com/forecastrss?w=2132701&u=c";
                //string httpstr_yahoo = Http.GetPage(url_yahoo);
                double curweather_yahoo = 0;
                //Regex r_yahoo = new Regex("temp=\"(\\d+)\"");
                //if (r_yahoo.IsMatch(httpstr_yahoo))
                //{
                //    double.TryParse(r_yahoo.Match(httpstr_yahoo).Groups[1].Value, out curweather_yahoo);
                //}

                string url_ra = "http://raqx.net/Module/Ajax/GetRealtimedata.ashx?type=temp";
                string httpstr_ra = Http.GetPage(url_ra);
                double curweather_ra = 0;
                Regex r_ra = new Regex("StationNum:\"K3164\"[\\s\\S]+?Val:\"(\\d+)\"");
                if (r_ra.IsMatch(httpstr_ra))
                {
                    if (double.TryParse(r_ra.Match(httpstr_ra).Groups[1].Value, out curweather_ra))
                    {
                        curweather_ra = curweather_ra / 10;
                    }
                }
                double[] ws = new double[] { curweather_zh, curweather_yahoo, curweather_ra };

                weather = ws.Max();
                double weatherold = DataConvert.SafeDouble(MangaCache.Get("weatherold"));
                if (weatherold > weather && (weatherold - weather) > 5)
                    weather = weatherold;
                else
                    weatherold = weather;
                MangaCache.Add("weather", weather, 120);
                MangaCache.Max("weatherold", weatherold);
            }
            catch { }
        }
    }
}
