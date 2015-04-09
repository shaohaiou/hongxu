using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;

namespace Hx.Components.BasePage
{
    public class WeixinBase : AdminBase
    {
        private int timestamp = 0;
        public int Timestamp
        {
            get
            {
                if (timestamp == 0)
                    timestamp = Utils.ConvertDateTimeInt(DateTime.Now);
                return timestamp;
            }
        }

        private string nonceStr = string.Empty;
        public string NonceStr
        {
            get
            {
                if (string.IsNullOrEmpty(nonceStr))
                {
                    nonceStr = EncryptString.MD5(DateTime.Now.ToString("yyyyMMddHHmiss") + DateTime.Now.Millisecond, true);
                }
                return nonceStr;
            }
        }

        private string signature = string.Empty;
        public string Signature
        {
            get
            {
                if (string.IsNullOrEmpty(signature))
                    signature = EncryptString.SHA1_Hash(string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", WeixinActs.Instance.GetJsapiTicket(), NonceStr, Timestamp, Request.Url.AbsoluteUri));
                return signature;
            }
        }
    }
}
