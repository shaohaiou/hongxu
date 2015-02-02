using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Enumerations;

namespace Hx.Components.Entity
{
    public class WeixinActCommentInfo
    {
        public int ID { get; set; }

        /// <summary>
        /// 活动类型
        /// </summary>
        public WeixinActType WeixinActType { get; set; }

        /// <summary>
        /// 选手ID
        /// </summary>
        public int AthleteID { get; set; }

        /// <summary>
        /// 评论人
        /// </summary>
        public string Commenter { get; set; }

        /// <summary>
        /// 获赞数量
        /// </summary>
        public int PraiseNum { get; set; }

        /// <summary>
        /// 被砸鸡蛋数量
        /// </summary>
        public int BelittleNum { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime AddTime { get; set; }
    }
}
