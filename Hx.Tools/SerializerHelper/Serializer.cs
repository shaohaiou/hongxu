
using System;
using System.IO;
using System.Collections.Specialized;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Hx.Tools
{
    /// <summary>
    /// 序列化工具类
    /// </summary>
    public class Serializer
    {
        //禁止被实例化
        private Serializer()
        {

        }

        /// <summary>
        /// 静态构造函数用来初始化是否可序列化为字节的值
        /// </summary>
        static Serializer()
        {
            SecurityPermission sp = new SecurityPermission(SecurityPermissionFlag.SerializationFormatter);
            try
            {
                sp.Demand();//判断是否有权限执行序列化？
                CanBinarySerialize = true;
            }
            catch (SecurityException)
            {
                CanBinarySerialize = false;
            }
        }

        public static readonly bool CanBinarySerialize;//是否有序列化的权限

        /// <summary>
        /// 将对象序列化为字节数组
        /// </summary>
        /// <param name="objectToConvert">需要序列化的对象</param>
        /// <returns>如果CanBinarySerialize为 false则返回null，否则返回字节数组</returns>
        public static byte[] ConvertToBytes(object objectToConvert)
        {
            byte[] byteArray = null;

            if (CanBinarySerialize)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    binaryFormatter.Serialize(ms, objectToConvert);
                    ms.Position = 0;
                    byteArray = new Byte[ms.Length];
                    ms.Read(byteArray, 0, byteArray.Length);
                    ms.Close();
                }
            }
            return byteArray;
        }


        /// <summary>
        /// 将对象序列化为字节流并保存为文件
        /// </summary>
        /// <param name="objectToSave">需要序列化的对象</param>
        /// <param name="path">文件路径</param>
        /// <returns>保存成功则返回true</returns>
        public static bool SaveAsBinary(object objectToSave, string path)
        {
            if (objectToSave != null && CanBinarySerialize)
            {
                byte[] ba = ConvertToBytes(objectToSave);
                if (ba != null)
                {
                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            bw.Write(ba);
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 将对象序列化为xml的字符串
        /// will be thrown.
        /// </summary>
        /// <param name="objectToConvert">需要序列化的对象</param>
        /// <returns>返回xml字符串</returns>
        public static string ConvertToXML(object objectToConvert)
        {
            string xml = null;

            if (objectToConvert != null)
            {
                //获取对象的类型
                Type t = objectToConvert.GetType();

                XmlSerializer ser = new XmlSerializer(t);

                using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
                {
                    ser.Serialize(writer, objectToConvert);
                    xml = writer.ToString();
                    writer.Close();
                }
            }

            return xml;
        }

        public static string ConvertToString(object objectToConvert)
        {
            string result = string.Empty;
            if (CanBinarySerialize && objectToConvert != null)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream serializationStream = new MemoryStream())
                {
                    formatter.Serialize(serializationStream, objectToConvert);
                    result = Convert.ToBase64String(serializationStream.ToArray());
                    serializationStream.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 将对象序列化为xml文件
        /// </summary>
        /// <param name="objectToConvert">需要转化的对象</param>
        /// <param name="path">文件保存的路径</param>
        public static void SaveAsXML(object objectToConvert, string path)
        {
            if (objectToConvert != null)
            {
                Type t = objectToConvert.GetType();

                XmlSerializer ser = new XmlSerializer(t);

                using (StreamWriter writer = new StreamWriter(path))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");
                    ser.Serialize(writer, objectToConvert, namespaces);
                    writer.Close();
                    writer.Dispose();
                }
            }

        }


        /// <summary>
        /// 将字节数组反序列化为对象
        /// </summary>
        /// <param name="byteArray">字节数组</param>
        /// <returns>当字节数组为null或长度为0时，返回null，否则返回反序列化的后的实例</returns>
        public static object ConvertToObject(byte[] byteArray)
        {
            object convertedObject = null;
            if (CanBinarySerialize && byteArray != null && byteArray.Length > 0)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(byteArray, 0, byteArray.Length);

                    ms.Position = 0;

                    if (byteArray.Length > 4)
                        convertedObject = binaryFormatter.Deserialize(ms);

                    ms.Close();
                    ms.Dispose();
                }
            }
            return convertedObject;
        }

        /// <summary>
        /// 将XML文件反序列化为对象
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="objectType">对象类型</param>
        /// <returns>反序列化后的实例</returns>
        public static object ConvertFileToObject(string path, Type objectType)
        {
            object convertedObject = null;

            if (path != null && path.Length > 0)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer ser = new XmlSerializer(objectType);
                    convertedObject = ser.Deserialize(fs);
                    fs.Close();
                }
            }
            return convertedObject;
        }



        /// <summary>
        /// 将xml字符串反序列化为对象
        /// </summary>
        /// <param name="xml">xml字符串</param>
        /// <param name="objectType">对象的类型</param>
        /// <returns>反序列化后的对象实例</returns>
        public static object ConvertToObject(string xml, Type objectType)
        {
            object convertedObject = null;

            if (!string.IsNullOrEmpty(xml))
            {
                using (StringReader reader = new StringReader(xml))
                {
                    XmlSerializer ser = new XmlSerializer(objectType);
                    convertedObject = ser.Deserialize(reader);
                    reader.Close();
                }
            }
            return convertedObject;
        }

        /// <summary>
        /// 将字符串反序列化为对象
        /// </summary>
        /// <param name="str">需要反序列化的字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static object ConvertToObject(string str)
        {
            object o = null;
            if (CanBinarySerialize && str != null && str.Length > 0)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(str)))
                {
                    o = formatter.Deserialize(ms);
                    ms.Close();
                }
            }
            return o;
        }

        /// <summary>
        ///将xml节点反序列化为对象 
        /// </summary>
        /// <param name="xml">xml节点</param>
        /// <param name="objectType">对象类型</param>
        /// <returns>反序列化后的对象实例</returns>
        public static object ConvertToObject(XmlNode node, Type objectType)
        {
            object convertedObject = null;

            if (node != null)
            {
                using (StringReader reader = new StringReader(node.OuterXml))
                {

                    XmlSerializer ser = new XmlSerializer(objectType);

                    convertedObject = ser.Deserialize(reader);

                    reader.Close();
                }
            }
            return convertedObject;
        }

        /// <summary>
        /// 将文件中的字节流反序列化为对象实例
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>当文件存在时返回反序列化后的对象实例</returns>
        public static object LoadBinaryFile(string path)
        {
            if (!File.Exists(path))
                return null;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BinaryReader br = new BinaryReader(fs);
                byte[] ba = new byte[fs.Length];
                br.Read(ba, 0, (int)fs.Length);
                return ConvertToObject(ba);
            }
        }

        /// <summary>
        /// 将包含keys值信息的字符串和values信息的字符串转化为一个NameValueCollection
        /// keys值的格式："键名：键的类型(暂时只能用S也就是字符串类型)：值开始索引：值的长度.  如：key1:S:0:3:key2:S:3:2:"
        /// values表示值的类型。
        /// </summary>
        /// <param name="keys">keys值</param>
        /// <param name="values">values信息</param>
        /// <returns>转化后的NameValueCollection</returns>
        /// <example>
        /// 例子如下
        /// string keys = "chen:S:0:3:liu:S:3:2:";
        /// string values = "12345";
        /// NameValueCollection nvc=ConvertToNameValueCollection(keys,values);
        /// 得出的nvc为
        /// nvc["chen"]=123;
        /// nvc["liu"]=45;
        /// </example>
        public static NameValueCollection ConvertToNameValueCollection(string keys, string values)
        {
            NameValueCollection nvc = new NameValueCollection();

            if (keys != null && values != null && keys.Length > 0 && values.Length > 0)
            {
                char[] splitter = new char[1] { ':' };
                string[] keyNames = keys.Split(splitter);

                //填充nvc
                for (int i = 0; i < (keyNames.Length / 4); i++)
                {
                    int start = int.Parse(keyNames[(i * 4) + 2], CultureInfo.InvariantCulture);//开始索引
                    int len = int.Parse(keyNames[(i * 4) + 3], CultureInfo.InvariantCulture);//长度
                    string key = keyNames[i * 4];//键值名

                    //未来将支持更多复杂的类型，现在只支持字符串类型：S 表示
                    if (((keyNames[(i * 4) + 1] == "S") && (start >= 0)) && (len > 0) && (values.Length >= (start + len)))
                    {
                        nvc[key] = values.Substring(start, len);
                    }
                }
            }

            return nvc;
        }

        /// <summary>
        /// 将NameValueCollection转化为两个输出的字符串：一个包含keys值信息的字符串，还有一个包含values信息的字符串
        /// </summary>
        /// <param name="nvc">需要转化的NameValueCollection</param>
        /// <param name="keys">输出的键值信息</param>
        /// <param name="values">输出的值信息</param>
        public static void ConvertFromNameValueCollection(NameValueCollection nvc, ref string keys, ref string values)
        {
            if (nvc == null || nvc.Count == 0)
                return;

            StringBuilder sbKey = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();

            int index = 0;
            foreach (string key in nvc.AllKeys)
            {
                if (key.IndexOf(':') != -1)
                    throw new ArgumentException("扩展的信息中没有字符 \":\"");

                string v = nvc[key];
                if (!string.IsNullOrEmpty(v))
                {
                    sbKey.AppendFormat("{0}:S:{1}:{2}:", key, index, v.Length);//键值：类型：开始索引：长度
                    sbValue.Append(v);
                    index += v.Length;
                }
            }
            keys = sbKey.ToString();
            values = sbValue.ToString();
        }
    }
}
