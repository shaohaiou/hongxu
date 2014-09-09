using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Tools
{
    public class DataConvert
    {
        #region Bool
        /// <summary>
        /// 将字符串转化为bool型，转化失败时默认返回false
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        public static bool SafeBool(string input)
        {
            return SafeBool(input, false);
        }

        /// <summary>
        /// 将字符串转化为bool型
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的bool</returns>
        public static bool SafeBool(string input, bool defaultValue)
        {
            bool result;
            if (!bool.TryParse(input, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 将字符串转化为bool型,转化失败时默认返回false
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <returns>转化后的bool</returns>
        public static bool SafeBool(object input)
        {
            return SafeBool(input, false);
        }

        /// <summary>
        /// 将字符串转化为bool型
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的bool</returns>
        public static bool SafeBool(object input, bool defaultValue)
        {
            return SafeBool(input.ToString(), defaultValue);
        }
        #endregion

        #region Date
        /// <summary>
        /// 将字符串转化为日期型，转化失败时为DateTime.MaxValue
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <returns>转化后的DateTime</returns>
        public static DateTime SafeDate(string input)
        {
            return SafeDate(input, DateTime.MaxValue);
        }

        /// <summary>
        /// 将字符串转化为日期型
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的DateTime</returns>
        public static DateTime SafeDate(string input, DateTime defaultValue)
        {
            DateTime result;
            if (!DateTime.TryParse(input, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 将对象转化为日期型，转化失败时为Now
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <returns>转化后的DateTime</returns>
        public static DateTime SafeDate(object input)
        {
            return SafeDate(input, DateTime.MaxValue);
        }



        /// <summary>
        /// 将对象转化为日期型
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的DateTime</returns>
        public static DateTime SafeDate(object input, DateTime defaultValue)
        {
            if (!Convert.IsDBNull(input) && !object.Equals(input, null))
            {
                return SafeDate(input.ToString(), defaultValue);
            }
            return DateTime.Now;
        }

        /// <summary>
        /// 判断字符串是否是日期型的
        /// </summary>
        /// <param name="text">需要判断的字符串</param>
        /// <returns>是否日期型</returns>
        public static bool IsDate(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            DateTime dt = DateTime.Now;
            return DateTime.TryParse(text, out dt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dl"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static string DateToChina(DateTime d2)
        {
            DateTime dl = DateTime.Now;
            TimeSpan ts = d2.Subtract(dl);
            string result = null;
            bool isAfter = false;
            if (ts.Ticks > 0)
            {
                result = "{0}之后";
                isAfter = true;
            }
            else
            {
                result = "{0}之前";
            }


            ts = ts.Duration();

            //由于两台服务器时间误差，容易出现 多少秒之后 的数据 所以暂时禁用 一分钟之内 之后类型的字符串
            if (isAfter && ts.TotalSeconds < 60)
            {
                return "10秒内";
            }

            if (ts.TotalDays > 10)
            {
                return d2.ToString("yyyy-MM-dd");
            }
            else
            {
                if (ts.TotalDays >= 1)
                {
                    result = string.Format(result, (int)ts.TotalDays + "天");
                }
                else
                {
                    if (ts.TotalHours >= 1)
                    {
                        result = string.Format(result, (int)ts.TotalHours + "小时");
                    }
                    else
                    {
                        if (ts.TotalMinutes >= 1)
                        {
                            result = string.Format(result, (int)ts.TotalMinutes + "分钟");
                        }
                        else
                        {
                            if (ts.TotalSeconds > 10)
                            {
                                result = string.Format(result, (int)ts.TotalSeconds + "秒");
                            }
                            else
                            {
                                result = "10秒前";
                            }
                        }
                    }
                }
            }
            return result.ToString();
        }

        #endregion

        #region Decimal

        /// <summary>
        /// 将字符串转化为decimal型,转化失败为0
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的decimal</returns>
        public static decimal SafeDecimal(string input)
        {
            return SafeDecimal(input, 0);
        }

        /// <summary>
        /// 将字符串转化为decimal型
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的decimal</returns>
        public static decimal SafeDecimal(string input, decimal defaultValue)
        {
            decimal result;
            if (!decimal.TryParse(input, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 将对象转化为decimal型,转化失败为0
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <returns>转化后的decimal</returns>
        public static decimal SafeDecimal(object input)
        {
            return SafeDecimal(input, 0);
        }


        /// <summary>
        /// 将对象转化为decimal型
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的decimal</returns>
        public static decimal SafeDecimal(object input, decimal defaultValue)
        {
            if (!Convert.IsDBNull(input) && !object.Equals(input, null))
            {
                return SafeDecimal(input.ToString(), defaultValue);
            }
            return defaultValue;
        }

        #endregion

        #region
        /// <summary>
        /// 将字符串转化为double型,转化失败为0
        /// </summary>
        /// <param name="input">要转化的要转化的字符型</param>
        /// <returns>转化后的double</returns>
        public static double SafeDouble(string input)
        {
            return SafeDouble(input, 0);
        }

        /// <summary>
        /// 将字符串转化为double型
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的double</returns>
        public static double SafeDouble(string input, double defaultValue)
        {
            double result;
            if (!double.TryParse(input, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 将字符串转化为double型,转化失败为0
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <returns>转化后的double</returns>
        public static double SafeDouble(object input)
        {
            return SafeDouble(input, 0);
        }

        /// <summary>
        /// 将字符串转化为double型
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的double</returns>
        public static double SafeDouble(object input, double defaultValue)
        {
            if (!Convert.IsDBNull(input) && !object.Equals(input, null))
            {
                return SafeDouble(input.ToString(), defaultValue);
            }
            return defaultValue;
        }

        #endregion

        #region Int

        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object input)
        {
            try
            {
                Convert.ToInt32(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将字符串转化为int型,转化失败时为0
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <returns>转化后的int</returns>
        public static int SafeInt(string input)
        {
            return SafeInt(input, 0);
        }

        /// <summary>
        /// 将字符串转化为int型
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的int</returns>
        public static int SafeInt(string input, int defaultValue)
        {
            int result;
            if (!int.TryParse(input, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 将对象转化为int型,转化失败时为0
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <returns>转化后的int</returns>
        public static int SafeInt(object input)
        {
            return SafeInt(input, 0);
        }

        /// <summary>
        /// 将对象转化为int型
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的int</returns>
        public static int SafeInt(object input, int defaultValue)
        {
            if (!Convert.IsDBNull(input) && !object.Equals(input, null))
            {
                return SafeInt(input.ToString(), defaultValue);
            }
            return defaultValue;
        }
        #endregion

        #region Long
        /// <summary>
        /// 将字符串转化为long型,转化失败后为0
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <returns>转化后的long</returns>
        public static long SafeLong(string input)
        {
            return SafeLong(input, 0);
        }

        /// <summary>
        /// 将字符串转化为long型
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的long</returns>
        public static long SafeLong(string input, long defaultValue)
        {
            long result;
            if (!long.TryParse(input, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 将对象转化为long型,转化失败后为0
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <returns>转化后的long</returns>
        public static long SafeLong(object input)
        {
            return SafeLong(input, 0);
        }


        /// <summary>
        /// 将对象转化为long型
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的long</returns>
        public static long SafeLong(object input, long defaultValue)
        {
            if (!Convert.IsDBNull(input) && !object.Equals(input, null))
            {
                return SafeLong(input.ToString(), defaultValue);
            }
            return defaultValue;
        }

        #endregion

        #region Float
        /// <summary>
        /// 将字符串转化为float型,转化失败时为0
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <returns>转化后的float</returns>
        public static float SafeFloat(string input)
        {
            return SafeFloat(input, 0);
        }


        /// <summary>
        /// 将字符串转化为float型
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的float</returns>
        public static float SafeFloat(string input, float defaultValue)
        {
            float result;
            if (!float.TryParse(input, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 将字符串转化为float型,转化失败时为0
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的float</returns>
        public static float SafeFloat(object input)
        {
            return SafeFloat(input, 0);
        }

        /// <summary>
        /// 将字符串转化为float型
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的float</returns>
        public static float SafeFloat(object input, float defaultValue)
        {
            if (!Convert.IsDBNull(input) && !object.Equals(input, null))
            {
                return SafeFloat(input.ToString(), defaultValue);
            }
            return defaultValue;
        }

        #endregion

        #region Byte
        /// <summary>
        /// 将字符串转化为Byte型,转化失败时为0
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <returns>转化后的Byte</returns>
        public static byte SafeByte(string input)
        {
            return SafeByte(input, 0);
        }


        /// <summary>
        /// 将字符串转化为Byte型
        /// </summary>
        /// <param name="input">要转化的字符型</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的Byte</returns>
        public static byte SafeByte(string input, byte defaultValue)
        {
            byte result;
            if (!byte.TryParse(input, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 将字符串转化为Byte型,转化失败时为0
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的Byte</returns>
        public static byte SafeByte(object input)
        {
            return SafeByte(input, 0);
        }

        /// <summary>
        /// 将字符串转化为Byte型
        /// </summary>
        /// <param name="input">要转化的对象</param>
        /// <param name="defaultValue">转化失败后的默认值</param>
        /// <returns>转化后的Byte</returns>
        public static byte SafeByte(object input, byte defaultValue)
        {
            if (!Convert.IsDBNull(input) && !object.Equals(input, null))
            {
                return SafeByte(input.ToString(), defaultValue);
            }
            return defaultValue;
        }

        #endregion
    }
}
