using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace Hx.Tools
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 转换Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// 得到所有的枚举项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// 将一个或多个枚举常数的名称或数字值的字符串表示转换成等效的枚举对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T TryParse<T>(string value, T defaultValue = default(T)) where T : struct
        {
            T result;
            return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }

        /// <summary>
        /// 将字符串转成可空枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T? TryPareseNullableEnum<T>(string value, T? defaultValue = null) where T : struct
        {
            T result;
            return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }

        /// <summary>
        /// 将枚举值、枚举名转成datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DataTable ToTable<T>()
        {
            DataTable dtEnum = new DataTable();
            dtEnum.Columns.Add("Name");
            dtEnum.Columns.Add("Value");
            dtEnum.Columns.Add("Text");
            DataRow dr;
            foreach (int v in Enum.GetValues(typeof(T)))
            {
                dr = dtEnum.NewRow();
                dr["Value"] = v;
                dr["Name"] = Enum.GetName(typeof(T), v);
                dr["Text"] = GetDescription<T>(dr["Name"].ToString());

                dtEnum.Rows.Add(dr);
            }
            return dtEnum;
        }

        public static string GetDescription<T>(string value)
        {
            string result = string.Empty;

            Object[] obj = typeof(T).GetField(value).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (obj != null && obj.Length != 0)
            {
                DescriptionAttribute des = (DescriptionAttribute)obj[0];
                result = des.Description;
            }

            return result;
        }
    }
}
