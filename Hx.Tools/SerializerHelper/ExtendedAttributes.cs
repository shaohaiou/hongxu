//------------------------------------------------------------------------------
// <copyright company="Telligent Systems">
//     Copyright (c) Telligent Systems Corporation.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace Hx.Tools
{
    /// <summary>
    /// 用于建立当前实例的副本
    /// </summary>
    [Serializable]
    public abstract class CSCopy
    {
        private static readonly Hashtable objects = new Hashtable();

        /// <summary>
        /// 建立当前类型的新实例
        /// </summary>
        /// <returns></returns>
        protected object CreateNewInstance()
        {
            ConstructorInfo ci = objects[this.GetType()] as ConstructorInfo;
            if (ci == null)
            {
                ci = this.GetType().GetConstructor(new Type[0]);
                objects[this.GetType()] = ci;
            }

            return ci.Invoke(null);
        }

        /// <summary>
        /// 建立当前实例的副本
        /// </summary>
        /// <returns></returns>
        public virtual object Copy()
        {
            return CreateNewInstance();
        }
    }

    /// <summary>
    /// 继承该类可以为对象提供简单的扩展信息
    /// </summary>
    [Serializable]
    public class ExtendedAttributes : CSCopy
    {


        public ExtendedAttributes()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        NameValueCollection extendedAttributes = new NameValueCollection();//用于保存扩展信息的NameValueCollection

        /// <summary>
        /// 获取扩展信息中的值
        /// </summary>
        /// <param name="name">扩展的属性名</param>
        /// <param name="value">扩展的属性值</param>
        public string GetExtendedAttribute(string name)
        {
            string returnValue = extendedAttributes[name];

            if (returnValue == null)
                return string.Empty;
            else
                return returnValue;
        }

        /// <summary>
        /// 获取扩展信息中
        /// </summary>
        /// <param name="name">扩展的属性名</param>
        /// <param name="value">扩展的属性值</param>
        public NameValueCollection GetExtendedAttributes()
        {
            return extendedAttributes;
        }

        /// <summary>
        /// 设置扩展信息中的值
        /// </summary>
        /// <param name="name">扩展的属性名</param>
        /// <param name="value">扩展的属性值</param>
        public void SetExtendedAttribute(string name, string value)
        {
            if ((value == null) || (value == string.Empty))
                extendedAttributes.Remove(name);
            else
                extendedAttributes[name] = value;
        }

        /// <summary>
        /// 存储扩展信息的条目数
        /// </summary>
        public int ExtendedAttributesCount
        {
            get { return extendedAttributes.Count; }
        }

        /// <summary>
        /// 获取扩展信息中的值并且转化为bool
        /// </summary>
        /// <param name="name">扩展的属性名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>扩展的属性值</returns>
        public bool GetBool(string name, bool defaultValue)
        {
            string b = GetExtendedAttribute(name);
            if (b == null || b.Trim().Length == 0)
                return defaultValue;
            try
            {
                return bool.Parse(b);
            }
            catch { }
            return defaultValue;
        }

        /// <summary>
        /// 获取扩展信息中的值并且转化为int
        /// </summary>
        /// <param name="name">扩展的属性名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>扩展的属性值</returns>
        public int GetInt(string name, int defaultValue)
        {
            string i = GetExtendedAttribute(name);
            if (i == null || i.Trim().Length == 0)
                return defaultValue;

            return Int32.Parse(i);
        }


        /// <summary>
        /// 获取扩展信息中的值并且转化为DateTime
        /// </summary>
        /// <param name="name">扩展的属性名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>扩展的属性值</returns>
        public DateTime GetDateTime(string name, DateTime defaultValue)
        {
            string d = GetExtendedAttribute(name);
            if (d == null || d.Trim().Length == 0)
                return defaultValue;

            return DateTime.Parse(d);
        }

        /// <summary>
        /// 获取扩展信息中的值并且转化为String
        /// </summary>
        /// <param name="name">扩展的属性名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>扩展的属性值</returns>
        public string GetString(string name, string defaultValue)
        {
            string v = GetExtendedAttribute(name);
            return (string.IsNullOrEmpty(v)) ? defaultValue : v;
        }

        public string this[string str]
        {
            set
            {
                SetExtendedAttribute(str, value);
            }
            get
            {
                return GetExtendedAttribute(str);
            }
        }
        /// <summary>
        /// 复制当前扩展信息
        /// </summary>
        /// <returns></returns>
        public override object Copy()
        {
            ExtendedAttributes ea = this.CreateNewInstance() as ExtendedAttributes;
            ea.extendedAttributes = new NameValueCollection(this.extendedAttributes);
            return ea;
        }

        #region 扩展信息的序列化

        /// <summary>
        /// 获取经过序列转化后的数据
        /// </summary>
        /// <returns>序列化后的数据</returns>
        public SerializerData GetSerializerData()
        {
            SerializerData data = new SerializerData();
            string keys = null;
            string values = null;

            Serializer.ConvertFromNameValueCollection(this.extendedAttributes, ref keys, ref values);
            data.Keys = keys;
            data.Values = values;

            return data;
        }

        /// <summary>
        /// 将数据反序列化
        /// </summary>
        /// <param name="data">序列化的数据</param>
        public void SetSerializerData(SerializerData data)
        {
            if (this.extendedAttributes == null || this.extendedAttributes.Count == 0)
            {
                this.extendedAttributes = Serializer.ConvertToNameValueCollection(data.Keys, data.Values);
            }

            if (this.extendedAttributes == null)
                extendedAttributes = new NameValueCollection();
        }
        #endregion
    }

    /// <summary>
    /// 序列化后的数据
    /// </summary>
    public struct SerializerData
    {
        public string Keys;
        public string Values;

    }
}
