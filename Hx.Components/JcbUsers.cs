using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;
using Hx.Components.Enumerations;

namespace Hx.Components
{
    public class JcbUsers
    {
        #region 单例
        private static object sync_creater = new object();

        private static JcbUsers _instance;
        public static JcbUsers Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new JcbUsers();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 成员方法

        public JcbUserInfo GetUserByName(string name)
        {
            return CommonDataProvider.Instance().GetJcbUserByName(name);
        }

        /// <summary>
        /// 管理员是否已经存在
        /// </summary>
        /// <param name="name">管理员ID</param>
        /// <returns></returns>
        public bool ExistsUser(int id)
        {
            return CommonDataProvider.Instance().ExistsJcbUser(id);
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="model">后台用户实体类</param>
        /// <returns>添加成功返回ID</returns>
        public int AddUser(JcbUserInfo model)
        {
            return CommonDataProvider.Instance().AddJcbUser(model);
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="model">后台用户实体类</param>
        /// <returns>修改是否成功</returns>
        public bool UpdateUser(JcbUserInfo model)
        {
            return CommonDataProvider.Instance().UpdateJcbUser(model);
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool DeleteUser(int ID)
        {
            return CommonDataProvider.Instance().DeleteJcbUser(ID);
        }

        /// <summary>
        /// 通过ID获取管理员
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>管理员实体信息</returns>
        public JcbUserInfo GetUserInfo(int id)
        {
            return CommonDataProvider.Instance().GetJcbUser(id);
        }

        /// <summary>
        /// 验证用户登陆
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>用户ID</returns>
        public int ValiUser(string userName, string password)
        {
            return CommonDataProvider.Instance().ValiJcbUser(userName, password);
        }

        /// <summary>
        /// 返回所有用户
        /// </summary>
        /// <returns></returns>
        public List<JcbUserInfo> GetAllUsers()
        {
            return CommonDataProvider.Instance().GetAllJcbUsers();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userID">管理员ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        public bool ChangePassword(int userID, string oldPassword, string newPassword)
        {
            return CommonDataProvider.Instance().ChangeJcbUserPw(userID, oldPassword, newPassword);
        }

        public string GetUserKey(int userid)
        {
            //string key= CommonDataProvider.Instance().GetAdminKey(userid);
            //key =EncryptString.MD5(key+EncryptString.MD5(userid.ToString(), false),false);
            //return key;
            return CommonDataProvider.Instance().GetJcbUserKey(userid);
        }

        #endregion
    }
}
