using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    [Serializable]
    public class CardInfo
    {
        /// <summary>
        /// 卡券类型
        /// </summary>
        public string card_type { get; set; }

        /// <summary>
        /// 通用券
        /// </summary>
        public General_couponInfo general_coupon { get; set; }

        /// <summary>
        /// 团购券
        /// </summary>
        public GrouponInfo groupon { get; set; }

        /// <summary>
        /// 礼品券
        /// </summary>
        public GiftInfo gift { get; set; }

        /// <summary>
        /// 代金券
        /// </summary>
        public CashInfo cash { get; set; }

        /// <summary>
        /// 折扣券
        /// </summary>
        public DiscountInfo discount { get; set; }

        /// <summary>
        /// 积分券
        /// </summary>
        public Member_cardInfo member_cardInfo { get; set; }

        /// <summary>
        /// 旅游票
        /// </summary>
        public Scenic_ticketInfo scenic_ticket { get; set; }

        /// <summary>
        /// 电影券
        /// </summary>
        public Movie_ticketInfo movie_ticket { get; set; }

        /// <summary>
        /// 机票
        /// </summary>
        public Boarding_passInfo boarding_pass { get; set; }

        /// <summary>
        /// 会议票
        /// </summary>
        public Meeting_ticketInfo meeting_ticket { get; set; }

        [JsonIgnore]
        public CardbaseInfo base_info
        {
            get
            {
                if (general_coupon != null) return general_coupon.base_info;
                if (groupon != null) return groupon.base_info;
                if (gift != null) return gift.base_info;
                if (cash != null) return cash.base_info;
                if (discount != null) return discount.base_info;
                if (member_cardInfo != null) return member_cardInfo.base_info;
                if (scenic_ticket != null) return scenic_ticket.base_info;
                if (movie_ticket != null) return movie_ticket.base_info;
                if (boarding_pass != null) return boarding_pass.base_info;
                if (meeting_ticket != null) return meeting_ticket.base_info;
                return null;
            }
        }
    }

    /// <summary>
    /// 通用券
    /// </summary>
    public class General_couponInfo
    {
        public CardbaseInfo base_info { get; set; }

        /// <summary>
        /// 描述文本
        /// </summary>
        public string default_detail { get; set; }
    }

    /// <summary>
    /// 团购券
    /// </summary>
    public class GrouponInfo
    {
        public CardbaseInfo base_info { get; set; }

        /// <summary>
        /// 团购详情
        /// </summary>
        public string deal_detail { get; set; }
    }

    /// <summary>
    /// 礼品券
    /// </summary>
    public class GiftInfo
    {
        public CardbaseInfo base_info { get; set; }

        /// <summary>
        /// 礼品名字
        /// </summary>
        public string gift { get; set; }
    }

    /// <summary>
    /// 代金券
    /// </summary>
    public class CashInfo
    {
        public CardbaseInfo base_info { get; set; }

        /// <summary>
        /// 起用金额（单位为分）
        /// </summary>
        public string least_cost { get; set; }

        /// <summary>
        /// 减免金额（单位为分）
        /// </summary>
        public string reduce_cost { get; set; }
    }

    /// <summary>
    /// 折扣券
    /// </summary>
    public class DiscountInfo
    {
        public CardbaseInfo base_info { get; set; }

        /// <summary>
        /// 打折额度（百分比）。填30 就是七折
        /// </summary>
        public string discount { get; set; }
    }

    /// <summary>
    /// 积分券
    /// </summary>
    public class Member_cardInfo
    {
        public CardbaseInfo base_info { get; set; }

        /// <summary>
        /// <para>是否支持积分，填写true 或false，如填写</para>
        /// <para>true，积分相关字段均为必填。填写false，</para>
        /// <para>积分字段无需填写。储值字段处理方式相同</para>
        /// </summary>
        public string supply_bonus { get; set; }

        /// <summary>
        /// 是否支持储值，填写true 或false
        /// </summary>
        public string supply_balance { get; set; }

        /// <summary>
        /// 积分清零规则
        /// </summary>
        public string bonus_cleared { get; set; }

        /// <summary>
        /// 积分规则。
        /// </summary>
        public string bonus_rules { get; set; }

        /// <summary>
        /// 储值说明。
        /// </summary>
        public string balance_rules { get; set; }

        /// <summary>
        /// 特权说明。
        /// </summary>
        public string prerogative { get; set; }

        /// <summary>
        /// 绑定旧卡的url。与“activate_url”二选一必填。
        /// </summary>
        public string bind_old_card_url { get; set; }

        /// <summary>
        /// 激活会员卡的url。与“bind_old_card_url”二选一必填
        /// </summary>
        public string activate_url { get; set; }
    }

    /// <summary>
    /// 旅游票
    /// </summary>
    public class Scenic_ticketInfo
    {
        public CardbaseInfo base_info { get; set; }

        /// <summary>
        /// 票类型，例如平日全票，套票等
        /// </summary>
        public string ticket_class { get; set; }

        /// <summary>
        /// 导览图url
        /// </summary>
        public string guide_url { get; set; }
    }

    /// <summary>
    /// 电影券
    /// </summary>
    public class Movie_ticketInfo
    {
        public CardbaseInfo base_info { get; set; }

        /// <summary>
        /// 电影票详情
        /// </summary>
        public string detail { get; set; }
    }

    /// <summary>
    /// 机票
    /// </summary>
    public class Boarding_passInfo
    {
        public CardbaseInfo base_info { get; set; }

        /// <summary>
        /// 起点
        /// </summary>
        public string from { get; set; }

        /// <summary>
        /// 终点
        /// </summary>
        public string to { get; set; }

        /// <summary>
        /// 航班
        /// </summary>
        public string flight { get; set; }

        /// <summary>
        /// 起飞时间
        /// </summary>
        public string departure_time { get; set; }

        /// <summary>
        /// 降落时间
        /// </summary>
        public string landing_time { get; set; }

        /// <summary>
        /// 在线值机的链接
        /// </summary>
        public string check_in_url { get; set; }

        /// <summary>
        /// 机型
        /// </summary>
        public string air_model { get; set; }
    }

    /// <summary>
    /// 会议券
    /// </summary>
    public class Meeting_ticketInfo
    {
        public CardbaseInfo base_info { get; set; }

        /// <summary>
        /// 会议详情
        /// </summary>
        public string meeting_detail { get; set; }

        /// <summary>
        /// 会场导览图
        /// </summary>
        public string map_url { get; set; }
    }

    /// <summary>
    /// 基本卡券数据
    /// </summary>
    public class CardbaseInfo
    {
        /// <summary>
        /// card_id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 卡券的商户logo
        /// </summary>
        public string logo_url { get; set; }

        /// <summary>
        /// <para>code 码展示类型</para>
        /// <para>"CODE_TYPE_TEXT"，文本；</para>
        /// <para>"CODE_TYPE_BARCODE"，一维码；</para>
        /// <para>"CODE_TYPE_QRCODE"，二维码；</para>
        /// <para>“CODE_TYPE_ONLY_QRCODE”,二维码无code 显示；</para>
        /// <para>“CODE_TYPE_ONLY_BARCODE”,一维码无code 显示；</para>
        /// </summary>
        public string code_type { get; set; }

        /// <summary>
        /// 商户名字
        /// </summary>
        public string brand_name { get; set; }

        /// <summary>
        /// 券名
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 券颜色。色彩规范标注值对应的色值。如#3373bb
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// 使用提醒。（一句话描述，展示在首页）
        /// </summary>
        public string notice { get; set; }

        /// <summary>
        /// 客服电话
        /// </summary>
        public string service_phone { get; set; }

        /// <summary>
        /// 使用说明。长文本描述，可以分行。
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 每人使用次数限制。
        /// </summary>
        public string use_limit { get; set; }

        /// <summary>
        /// 每人最大领取次数
        /// </summary>
        public int get_limit { get; set; }

        /// <summary>
        /// 是否自定义code 码
        /// </summary>
        public string use_custom_code { get; set; }

        /// <summary>
        /// 是否指定用户领取
        /// </summary>
        public string bind_openid { get; set; }

        /// <summary>
        /// 领取卡券原生页面是否可分享，填写true 或false，true 代表可分享。默认可分享。
        /// </summary>
        public bool can_share { get; set; }

        /// <summary>
        /// 卡券是否可转赠，填写true 或false,true 代表可转赠。默认可转赠
        /// </summary>
        public bool can_give_friend { get; set; }

        /// <summary>
        /// 门店位置ID
        /// </summary>
        public int[] location_id_list { get; set; }

        /// <summary>
        /// 使用日期，有效期的信息
        /// </summary>
        public DateInfo date_info { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        public SkuInfo sku { get; set; }

        /// <summary>
        /// 商户自定义入口名称，与custom_url 字段共同使用，长度限制在5 个汉字内。
        /// </summary>
        public string custom_url_name { get; set; }

        /// <summary>
        /// 商户自定义cell 跳转外链的地址链接,跳转页面内容需与自定义cell 名称保持匹配
        /// </summary>
        public string custom_url { get; set; }

        /// <summary>
        /// 显示在cell 右侧的tips，长度限制在6 个汉字内
        /// </summary>
        public string custom_url_sub_title { get; set; }

        /// <summary>
        /// 营销场景的自定义入口。
        /// </summary>
        public string promotion_url_name { get; set; }

        /// <summary>
        /// 入口跳转外链的地址链接
        /// </summary>
        public string promotion_url { get; set; }

        /// <summary>
        /// 显示在入口右侧的tips，长度限制在6 个汉字内。
        /// </summary>
        public string promotion_url_sub_title { get; set; }

        /// <summary>
        /// 第三方来源名，例如同程旅游、格瓦拉。
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// <para>“CARD_STATUS_NOT_VERIFY”,待审核</para>
        /// <para>“CARD_STATUS_VERIFY_FALL”,审核失败</para>
        /// <para>“CARD_STATUS_VERIFY_OK”，通过审核</para>
        /// <para>“CARD_STATUS_USER_DELETE”，卡券被用户删除</para>
        /// <para>CARD_STATUS_USER_DISPATCH ”，在公众平台投放过的卡券</para>
        /// </summary>
        public string status { get; set; }
    }

    /// <summary>
    /// 卡券使用有效期
    /// </summary>
    public class DateInfo
    {
        /// <summary>
        /// <para>使用时间的类型</para>
        /// <para>1：固定日期区间，2：固定时长（自领取后按天算）</para>
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 固定日期区间专用，表示起用时间。
        /// </summary>
        public string begin_timestamp { get; set; }

        /// <summary>
        /// 固定日期区间专用，表示结束时间。
        /// </summary>
        public string end_timestamp { get; set; }

        /// <summary>
        /// 固定时长专用，表示自领取后多少天内有效。（单位为天）
        /// </summary>
        public int fixed_term { get; set; }

        /// <summary>
        /// 固定时长专用，表示自领取后多少天开始生效。（单位为天）
        /// </summary>
        public int fixed_begin_term { get; set; }
    }

    /// <summary>
    /// 商品信息
    /// </summary>
    public class SkuInfo
    {
        /// <summary>
        /// 库存数量
        /// </summary>
        public int quantity { get; set; }
    }
}
