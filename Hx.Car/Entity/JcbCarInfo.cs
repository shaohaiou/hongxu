using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Car.Entity
{
    [Serializable]
    public class JcbCarInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        /// <summary>
        /// VIN码
        /// </summary>
        [JsonProperty("VINCode")]
        public string VINCode { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [JsonProperty("Cph")]
        public string Cph { get; set; }

        /// <summary>
        /// 预售价
        /// </summary>
        [JsonProperty("Ysj")]
        public decimal Ysj { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [JsonProperty("LastUpdateTime")]
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 出兑价格
        /// </summary>
        [JsonProperty("Cdjg")]
        public decimal Cdjg { get; set; }

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        [JsonProperty("UserID")]
        public int UserID { get; set; }

        #region 基本信息

        /// <summary>
        /// 品牌
        /// </summary>
        [JsonIgnore]
        public string cChangs
        {
            get { return GetString("cChangs", ""); }
            set { SetExtendedAttribute("cChangs", value); }
        }

        /// <summary>
        /// 车型
        /// </summary>
        [JsonIgnore]
        public string cCxmc
        {
            get { return GetString("cCxmc", ""); }
            set { SetExtendedAttribute("cCxmc", value); }
        }

        /// <summary>
        /// 排量
        /// </summary>
        [JsonIgnore]
        public string Pailiang
        {
            get { return GetString("Pailiang", ""); }
            set { SetExtendedAttribute("Pailiang", value); }
        }

        /// <summary>
        /// 是否涡轮增压
        /// </summary>
        [JsonIgnore]
        public bool Iswlzy
        {
            get { return GetBool("Iswlzy", false); }
            set { SetExtendedAttribute("Iswlzy", value.ToString()); }
        }

        /// <summary>
        /// 变速箱
        /// </summary>
        [JsonIgnore]
        public string Bsx
        {
            get { return GetString("Bsx", ""); }
            set { SetExtendedAttribute("Bsx", value); }
        }

        /// <summary>
        /// 排放标准
        /// </summary>
        [JsonIgnore]
        public string Pfbz
        {
            get { return GetString("Pfbz", ""); }
            set { SetExtendedAttribute("Pfbz", value); }
        }

        /// <summary>
        /// 是否OBD
        /// </summary>
        [JsonIgnore]
        public bool Isobd
        {
            get { return GetBool("Isobd", false); }
            set { SetExtendedAttribute("Isobd", value.ToString()); }
        }

        /// <summary>
        /// 外观颜色
        /// </summary>
        [JsonIgnore]
        public string Wgys
        {
            get { return GetString("Wgys", ""); }
            set { SetExtendedAttribute("Wgys", value); }
        }

        /// <summary>
        /// 内饰颜色
        /// </summary>
        [JsonIgnore]
        public string Nsys
        {
            get { return GetString("Nsys", ""); }
            set { SetExtendedAttribute("Nsys", value); }
        }

        /// <summary>
        /// 车辆配置大灯
        /// </summary>
        [JsonIgnore]
        public string Cardeploydd
        {
            get { return GetString("Cardeploydd", ""); }
            set { SetExtendedAttribute("Cardeploydd", value); }
        }

        /// <summary>
        /// 车辆配置座椅
        /// </summary>
        [JsonIgnore]
        public string Cardeployzy
        {
            get { return GetString("Cardeployzy", ""); }
            set { SetExtendedAttribute("Cardeployzy", value); }
        }

        /// <summary>
        /// 车辆配置音响
        /// </summary>
        [JsonIgnore]
        public string Cardeployyx
        {
            get { return GetString("Cardeployyx", ""); }
            set { SetExtendedAttribute("Cardeployyx", value); }
        }

        /// <summary>
        /// 车辆配置安全气囊
        /// </summary>
        [JsonIgnore]
        public string Cardeployaqqn
        {
            get { return GetString("Cardeployaqqn", ""); }
            set { SetExtendedAttribute("Cardeployaqqn", value); }
        }

        /// <summary>
        /// 车辆配置其他
        /// </summary>
        [JsonIgnore]
        public string Cardeployqt
        {
            get { return GetString("Cardeployqt", ""); }
            set { SetExtendedAttribute("Cardeployqt", value); }
        }

        #endregion

        #region 使用信息

        /// <summary>
        /// 表显里程
        /// </summary>
        [JsonIgnore]
        public string Bxlc
        {
            get { return GetString("Bxlc", ""); }
            set { SetExtendedAttribute("Bxlc", value); }
        }

        /// <summary>
        /// 是否一手车
        /// </summary>
        [JsonIgnore]
        public string Sfysc
        {
            get { return GetString("Sfysc", ""); }
            set { SetExtendedAttribute("Sfysc", value); }
        }

        /// <summary>
        /// 车辆所在地省份
        /// </summary>
        [JsonIgnore]
        public string CarPromary
        {
            get { return GetString("CarPromary", ""); }
            set { SetExtendedAttribute("CarPromary", value); }
        }

        /// <summary>
        /// 车辆所在地城市
        /// </summary>
        [JsonIgnore]
        public string CarCity
        {
            get { return GetString("CarCity", ""); }
            set { SetExtendedAttribute("CarCity", value); }
        }

        /// <summary>
        /// 使用性质
        /// </summary>
        [JsonIgnore]
        public string Syxz
        {
            get { return GetString("Syxz", ""); }
            set { SetExtendedAttribute("Syxz", value); }
        }

        /// <summary>
        /// 有无重大事故
        /// </summary>
        [JsonIgnore]
        public string Ywzdsg
        {
            get { return GetString("Ywzdsg", ""); }
            set { SetExtendedAttribute("Ywzdsg", value); }
        }

        /// <summary>
        /// 外观成色
        /// </summary>
        [JsonIgnore]
        public string Wgcs
        {
            get { return GetString("Wgcs", ""); }
            set { SetExtendedAttribute("Wgcs", value); }
        }

        /// <summary>
        /// 内饰状态
        /// </summary>
        [JsonIgnore]
        public string Nszt
        {
            get { return GetString("Nszt", ""); }
            set { SetExtendedAttribute("Nszt", value); }
        }

        /// <summary>
        /// 仪表台状态
        /// </summary>
        [JsonIgnore]
        public string Ybtzt
        {
            get { return GetString("Ybtzt", ""); }
            set { SetExtendedAttribute("Ybtzt", value); }
        }

        /// <summary>
        /// 座椅使用情况
        /// </summary>
        [JsonIgnore]
        public string Zysyqk
        {
            get { return GetString("Zysyqk", ""); }
            set { SetExtendedAttribute("Zysyqk", value); }
        }

        /// <summary>
        /// 定期保养
        /// </summary>
        [JsonIgnore]
        public string Dqby
        {
            get { return GetString("Dqby", ""); }
            set { SetExtendedAttribute("Dqby", value); }
        }

        /// <summary>
        /// 维修保养记录
        /// </summary>
        [JsonIgnore]
        public string Wxbyjl
        {
            get { return GetString("Wxbyjl", ""); }
            set { SetExtendedAttribute("Wxbyjl", value); }
        }

        /// <summary>
        /// 一句话广告
        /// </summary>
        [JsonIgnore]
        public string Yjhgg
        {
            get { return GetString("Yjhgg", ""); }
            set { SetExtendedAttribute("Yjhgg", value); }
        }

        /// <summary>
        /// 真实油耗
        /// </summary>
        [JsonIgnore]
        public string Zsyh
        {
            get { return GetString("Zsyh", ""); }
            set { SetExtendedAttribute("Zsyh", value); }
        }

        /// <summary>
        /// 保养费用
        /// </summary>
        [JsonIgnore]
        public string Byfy
        {
            get { return GetString("Byfy", ""); }
            set { SetExtendedAttribute("Byfy", value); }
        }

        #endregion

        #region 牌照信息

        /// <summary>
        /// 证件是否齐全
        /// </summary>
        [JsonIgnore]
        public string Zjsfqq
        {
            get { return GetString("Zjsfqq", ""); }
            set { SetExtendedAttribute("Zjsfqq", value); }
        }

        /// <summary>
        /// 行驶证
        /// </summary>
        [JsonIgnore]
        public string Xsz
        {
            get { return GetString("Xsz", ""); }
            set { SetExtendedAttribute("Xsz", value); }
        }

        /// <summary>
        /// 行驶证图片
        /// </summary>
        [JsonIgnore]
        public string XszPic
        {
            get { return GetString("XszPic", ""); }
            set { SetExtendedAttribute("XszPic", value); }
        }

        /// <summary>
        /// 登记证
        /// </summary>
        [JsonIgnore]
        public string Djz
        {
            get { return GetString("Djz", ""); }
            set { SetExtendedAttribute("Djz", value); }
        }

        /// <summary>
        /// 登记证图片
        /// </summary>
        [JsonIgnore]
        public string DjzPic
        {
            get { return GetString("DjzPic", ""); }
            set { SetExtendedAttribute("DjzPic", value); }
        }

        /// <summary>
        /// 购车发票
        /// </summary>
        [JsonIgnore]
        public string Gcfp
        {
            get { return GetString("Gcfp", ""); }
            set { SetExtendedAttribute("Gcfp", value); }
        }

        /// <summary>
        /// 购车发票图片
        /// </summary>
        [JsonIgnore]
        public string GcfpPic
        {
            get { return GetString("GcfpPic", ""); }
            set { SetExtendedAttribute("GcfpPic", value); }
        }

        /// <summary>
        /// 购置税
        /// </summary>
        [JsonIgnore]
        public string Gzs
        {
            get { return GetString("Gzs", ""); }
            set { SetExtendedAttribute("Gzs", value); }
        }

        /// <summary>
        /// 是否可以外迁
        /// </summary>
        [JsonIgnore]
        public string Sfkywq
        {
            get { return GetString("Sfkywq", ""); }
            set { SetExtendedAttribute("Sfkywq", value); }
        }

        /// <summary>
        /// 首次上牌日期年份
        /// </summary>
        [JsonIgnore]
        public string YearScsprq
        {
            get { return GetString("YearScsprq", ""); }
            set { SetExtendedAttribute("YearScsprq", value); }
        }

        /// <summary>
        /// 首次上牌日期
        /// </summary>
        [JsonIgnore]
        public string Scsprq
        {
            get { return GetString("Scsprq", ""); }
            set { SetExtendedAttribute("Scsprq", value); }
        }

        /// <summary>
        /// 是否未上牌
        /// </summary>
        [JsonIgnore]
        public bool Iswsp
        {
            get { return GetBool("Iswsp", false); }
            set { SetExtendedAttribute("Iswsp", value.ToString()); }
        }

        /// <summary>
        /// 年检有效期至
        /// </summary>
        [JsonIgnore]
        public string Njyxq
        {
            get { return GetString("Njyxq", ""); }
            set { SetExtendedAttribute("Njyxq", value); }
        }

        /// <summary>
        /// 是否年检已过期
        /// </summary>
        [JsonIgnore]
        public bool Isnjygq
        {
            get { return GetBool("Isnjygq", false); }
            set { SetExtendedAttribute("Isnjygq", value.ToString()); }
        }

        /// <summary>
        /// 交强险有效期至
        /// </summary>
        [JsonIgnore]
        public string Jqxyxq
        {
            get { return GetString("Jqxyxq", ""); }
            set { SetExtendedAttribute("Jqxyxq", value); }
        }

        /// <summary>
        /// 是否交强险已过期
        /// </summary>
        [JsonIgnore]
        public bool Isjqxygq
        {
            get { return GetBool("Isjqxygq", false); }
            set { SetExtendedAttribute("Isjqxygq", value.ToString()); }
        }

        /// <summary>
        /// 商业险有效期至
        /// </summary>
        [JsonIgnore]
        public string Syxyxq
        {
            get { return GetString("Syxyxq", ""); }
            set { SetExtendedAttribute("Syxyxq", value); }
        }

        /// <summary>
        /// 是否商业险已过期
        /// </summary>
        [JsonIgnore]
        public bool Issyxygq
        {
            get { return GetBool("Issyxygq", false); }
            set { SetExtendedAttribute("Issyxygq", value.ToString()); }
        }

        #endregion

        #region 价格信息

        /// <summary>
        /// 预售价是否包含过户费
        /// </summary>
        [JsonIgnore]
        public bool IsYsjbhghf
        {
            get { return GetBool("IsYsjbhghf", false); }
            set { SetExtendedAttribute("IsYsjbhghf", value.ToString()); }
        }

        /// <summary>
        /// 预售价是否一口价
        /// </summary>
        [JsonIgnore]
        public bool IsYsjykj
        {
            get { return GetBool("IsYsjykj", false); }
            set { SetExtendedAttribute("IsYsjykj", value.ToString()); }
        }

        /// <summary>
        /// 预售价是否可按揭
        /// </summary>
        [JsonIgnore]
        public bool IsYsjkaj
        {
            get { return GetBool("IsYsjkaj", false); }
            set { SetExtendedAttribute("IsYsjkaj", value.ToString()); }
        }

        /// <summary>
        /// 售后质保
        /// </summary>
        [JsonIgnore]
        public string Shzb
        {
            get { return GetString("Shzb", ""); }
            set { SetExtendedAttribute("Shzb", value); }
        }

        /// <summary>
        /// 商家延保内容月数
        /// </summary>
        [JsonIgnore]
        public string Sjybnry
        {
            get { return GetString("Sjybnry", ""); }
            set { SetExtendedAttribute("Sjybnry", value); }
        }

        /// <summary>
        /// 商家延保内容公里数
        /// </summary>
        [JsonIgnore]
        public string Sjybnrgl
        {
            get { return GetString("Sjybnrgl", ""); }
            set { SetExtendedAttribute("Sjybnrgl", value); }
        }

        /// <summary>
        /// 出兑价格是否可议价
        /// </summary>
        [JsonIgnore]
        public bool IsCdjgkyj
        {
            get { return GetBool("IsCdjgkyj", false); }
            set { SetExtendedAttribute("IsCdjgkyj", value.ToString()); }
        }

        #endregion

        #region 车主自述

        /// <summary>
        /// 车主自述
        /// </summary>
        [JsonIgnore]
        public string Czzs
        {
            get { return GetString("Czzs", ""); }
            set { SetExtendedAttribute("Czzs", value); }
        }

        #endregion

        #region 车辆照片

        public List<JcbcarpicInfo> Picslist { get; set; }

        /// <summary>
        /// 图片信息
        /// </summary>
        [JsonIgnore]
        public string Pics
        {
            get { return GetString("Pics", ""); }
            set { SetExtendedAttribute("Pics", value); }
        }

        public string FirstPic
        {
            get
            {
                string result = "../images/fm.jpg";
                if (Picslist != null && Picslist.Count > 0)
                {
                    if (Picslist.Exists(p => p.IsFirstpic))
                        result = Picslist.Find(p => p.IsFirstpic).PicUrl;
                }
                return result;
            }
        }

        public int PicCount
        {
            get
            {
                int result = 0;
                if (Picslist != null)
                    result = Picslist.Count;
                return result;
            }
        }

        #endregion
    }
}
