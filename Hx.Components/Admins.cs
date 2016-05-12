using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;
using Hx.Components.Enumerations;

namespace Hx.Components
{
    public class Admins
    {
        #region 单例
        private static object sync_creater = new object();

        private static Admins _instance;
        public static Admins Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new Admins();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 成员方法

        public AdminInfo GetAdminByName(string name)
        {
            return CommonDataProvider.Instance().GetAdminByName(name);
        }

        /// <summary>
        /// 管理员是否已经存在
        /// </summary>
        /// <param name="name">管理员ID</param>
        /// <returns></returns>
        public bool ExistsAdmin(int id)
        {
            return CommonDataProvider.Instance().ExistsAdmin(id);
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="model">后台用户实体类</param>
        /// <returns>添加成功返回ID</returns>
        public int AddAdmin(AdminInfo model)
        {
            return CommonDataProvider.Instance().AddAdmin(model);
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="model">后台用户实体类</param>
        /// <returns>修改是否成功</returns>
        public bool UpdateAdmin(AdminInfo model)
        {
            return CommonDataProvider.Instance().UpdateAdmin(model);
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool DeleteAdmin(int ID)
        {
            return CommonDataProvider.Instance().DeleteAdmin(ID);
        }

        /// <summary>
        /// 通过ID获取管理员
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>管理员实体信息</returns>
        public AdminInfo GetAdmin(int id)
        {
            return CommonDataProvider.Instance().GetAdmin(id);
        }

        /// <summary>
        /// 验证用户登陆
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>用户ID</returns>
        public int ValiUser(string userName, string password)
        {
            return CommonDataProvider.Instance().ValiAdmin(userName, password);
        }

        /// <summary>
        /// 返回管理员用户
        /// </summary>
        /// <returns></returns>
        public List<AdminInfo> GetAllAdmins()
        {
            return CommonDataProvider.Instance().GetAllAdmins();
        }

        /// <summary>
        /// 获取普通用户
        /// </summary>
        /// <returns></returns>
        public List<AdminInfo> GetUsers(string corp,UserRoleType role)
        {
            List<AdminInfo> users = CommonDataProvider.Instance().GetUsers(role);

            if (corp != "-1")
            {
                users = users.FindAll(u=>u.Corporation == corp);
            }

            return users;
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
            return CommonDataProvider.Instance().ChangeAdminPw(userID, oldPassword, newPassword);
        }

        public string GetUserKey(int userid)
        {
            //string key= CommonDataProvider.Instance().GetAdminKey(userid);
            //key =EncryptString.MD5(key+EncryptString.MD5(userid.ToString(), false),false);
            //return key;
            return CommonDataProvider.Instance().GetAdminKey(userid);
        }

        #endregion
    }
}
