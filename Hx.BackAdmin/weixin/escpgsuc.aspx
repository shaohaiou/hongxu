<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="escpgsuc.aspx.cs" Inherits="Hx.BackAdmin.weixin.escpgsuc" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title>红旭集团-爱车车价评估器</title>
    <meta content="红旭集团,爱车车价评估器" name="keywords">
    <meta content="红旭集团,爱车车价评估器" name="description">
    <link href="../css/escpg.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="dwrap">
        <div class="dad">
            <img alt="" src="../images/weixin/escpg/head.png" />
        </div>
        <div class="main">
            <div class="mhead">
                <p>认证评估师玩命评估中...</p>
                <p>敬请期待</p>
            </div>
            <div class="des">
                <p>
                    评估结果将以短信你方式发送到您手机上</p>
                <p>
                    凭本短信可在车展现场领取礼品一份</p>
            </div>
            <div class="ewm">
                <p>领取奖品：关注红旭智选车行微信回复2</p>
                <img src="../images/weixin/escpg/ewm.png" />
                <p>长按二维码即可关注</p>
            </div>
        </div>
    </div>
</body>
<script type="text/javascript">
    if(!openInWeixin()){
        alert("请在微信中进入此页面");
        location.href="http://m.hongxu.cn/";
    }
    
    function openInWeixin() {
        return /MicroMessenger/i.test(navigator.userAgent);
    }
    
    wx.config({
        debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
        appId: 'wx0c9b37c9d5ddf8a8', // 必填，公众号的唯一标识
        timestamp: <%=Timestamp %>, // 必填，生成签名的时间戳
        nonceStr: '<%=NonceStr %>', // 必填，生成签名的随机串
        signature: '<%=Signature %>',// 必填，签名，见附录1
        jsApiList: [
    'onMenuShareTimeline',
    'onMenuShareAppMessage',
    'onMenuShareQQ',
    'onMenuShareWeibo'] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
    });

    $(function () {
        wx.ready(function () {
            
            // 微信分享的数据
            var wxData = {
                "imgUrl": 'http://bj.hongxu.cn/images/logo.png',
                "link": 'http://bj.hongxu.cn/weixin/escpg.aspx',
                "desc": '红旭集团-爱车车价评估器',
                "title": '爱车车价评估器'
            };

            wx.onMenuShareTimeline({
              title: wxData.title,
              link: wxData.link,
              imgUrl: wxData.imgUrl,
              success: function (res) {
              },
              cancel: function (res) {
              }
            });

            wx.onMenuShareAppMessage({
              title: wxData.title,
              desc: wxData.desc,
              link: wxData.link,
              imgUrl: wxData.imgUrl,
              success: function (res) {
              },
              cancel: function (res) {
              }
            });

            wx.onMenuShareQQ({
              title: wxData.title,
              desc: wxData.desc,
              link: wxData.link,
              imgUrl: wxData.imgUrl,
              success: function (res) {
              },
              cancel: function (res) {
              }
            });
  
            wx.onMenuShareWeibo({
              title: wxData.title,
              desc: wxData.desc,
              link: wxData.link,
              imgUrl: wxData.imgUrl,
              success: function (res) {
              },
              cancel: function (res) {
              }
            });
        });
    })
</script>
</html>
