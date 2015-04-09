using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;
using Hx.Components.Enumerations;

namespace Hx.Components.Entity
{
    [Serializable]
    public class JcbUserInfo : ExtendedAttributes
    {
        #region 属性

        /// <summary>
        /// 用户ID
        /// </summary>
        [JsonProperty("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [JsonProperty("UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [JsonProperty("Password")]
        public string Password { get; set; }

        /// <summary>
        /// 用户是否是超级管理员
        /// </summary>
        [JsonProperty("Administrator")]
        public bool Administrator { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        [JsonProperty("LastLoginIP")]
        public string LastLoginIP { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [JsonProperty("LastLoginTime")]
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [JsonIgnore]
        public string Mobile
        {
            get { return GetString("Mobile", ""); }
            set { SetExtendedAttribute("Mobile", value); }
        }

        /// <summary>
        /// 联系人 
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get { return GetString("Name", ""); }
            set { SetExtendedAttribute("Name", value); }
        }

        /// <summary>
        /// 公司类型
        /// </summary>
        [JsonIgnore]
        public JcbCompanyType CompanyType
        {
            get { return (JcbCompanyType)GetInt("CompanyType", 0); }
            set { SetExtendedAttribute("CompanyType", ((int)value).ToString()); }
        }

        /// <summary>
        /// 公司名称
        /// </summary>
        [JsonIgnore]
        public string CompanyName
        {
            get { return GetString("CompanyName", ""); }
            set { SetExtendedAttribute("CompanyName", value); }
        }

        /// <summary>
        /// 手机号
        /// </summary>
        [JsonIgnore]
        public string Phone
        {
            get { return GetString("Phone", ""); }
            set { SetExtendedAttribute("Phone", value); }
        }

        /// <summary>
        /// QQ号
        /// </summary>
        [JsonIgnore]
        public string QQ
        {
            get { return GetString("QQ", ""); }
            set { SetExtendedAttribute("QQ", value); }
        }

        /// <summary>
        /// 省份
        /// </summary>
        [JsonIgnore]
        public string Province
        {
            get { return GetString("Province", ""); }
            set { SetExtendedAttribute("Province", value); }
        }

        /// <summary>
        /// 城市
        /// </summary>
        [JsonIgnore]
        public string City
        {
            get { return GetString("City", ""); }
            set { SetExtendedAttribute("City", value); }
        }

        /// <summary>
        /// 详细地址
        /// </summary>
        [JsonIgnore]
        public string Address
        {
            get { return GetString("Address", ""); }
            set { SetExtendedAttribute("Address", value); }
        }

        #endregion Model
    }
}
