using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Config;

namespace Hx.Components
{
    public class DataProvider
    {
        /// <summary>
        /// 根据提供者信息，创建实现类
        /// </summary>
        /// <param name="dataProvider"></param>
        /// <returns></returns>
        public static Object Instance(Provider dataProvider)
        {
            if (dataProvider == null)
            {
                throw new Exception("Provider不存在,请确认配置文件中的节点Provider中的信息");
            }
            Type type = Type.GetType(dataProvider.Type);
            object newObject = null;
            if (type != null)
            {
                string connectionString = null;
                string databaseOwner = null;
                GetDataStoreParameters(dataProvider, out connectionString, out databaseOwner);
                newObject = Activator.CreateInstance(type, connectionString, databaseOwner);
                if (newObject == null)
                {
                    throw new Exception("建立实例出错");
                }
            }
            else
            {
                throw new Exception("加载类型出错");
            }

            return newObject;

        }

        /// <summary>
        /// 不允许实例化
        /// </summary>
        private DataProvider()
        {
        }

        /// <summary>
        /// 加载具体实现类需要的参数
        /// </summary>
        /// <param name="dataProvider">数据提供信息类</param>
        /// <param name="connectionString">连接字符串名</param>
        /// <param name="databaseOwner">数据库所有者</param>
        private static void GetDataStoreParameters(Provider dataProvider, out string connectionString, out string databaseOwner)
        {

            databaseOwner = dataProvider.Attributes["databaseOwner"];
            CommConfig config = CommConfig.GetConfig();
            if (databaseOwner == null || databaseOwner.Trim().Length == 0)
            {
                databaseOwner = config.AppSetting[dataProvider.Attributes["databaseOwnerStringName"]];//数据库所有者
            }

            connectionString = dataProvider.Attributes["connectionString"];
            if (connectionString == null || connectionString.Trim().Length == 0)
            {
                connectionString = config.AppSetting[dataProvider.Attributes["connectionStringName"]];//数据库连接字符串名
            }
        }
    }
}
