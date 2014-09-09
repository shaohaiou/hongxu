using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using Hx.Tools.Web;
using Hx.Components;
using Hx.Tools;

namespace Hx.BackAdmin
{
    public partial class weather : System.Web.UI.Page
    {
        private object sync_helper = new object();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                double weather = DataConvert.SafeDouble(MangaCache.Get("weather"));
                if (weather == 0)
                {
                    lock (sync_helper)
                    {
                        weather = DataConvert.SafeDouble(MangaCache.Get("weather"));
                        if (weather == 0)
                        {
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

                            string url_ra = "http://raqx.net/realtimedata.ashx?type=temp";
                            string httpstr_ra = Http.GetPage(url_ra);
                            double curweather_ra = 0;
                            Regex r_ra = new Regex("StationNum:\"K3164\"[\\s\\S]+?DryBulTemp:\"(\\d+)\"");
                            if (r_ra.IsMatch(httpstr_ra))
                            {
                                if (double.TryParse(r_ra.Match(httpstr_ra).Groups[1].Value, out curweather_ra))
                                {
                                    curweather_ra = curweather_ra / 10;
                                }
                            }
                            double[] ws = new double[] { curweather_zh, curweather_yahoo, curweather_ra };

                            weather = ws.Max();
                            MangaCache.Add("weather", weather, 60);
                        }
                    }
                }
                Response.Write(weather);
                Response.End();
            }
            catch { }
        }
    }
}