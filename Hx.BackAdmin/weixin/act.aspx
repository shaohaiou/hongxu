<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="act.aspx.cs" Inherits="Hx.BackAdmin.weixin.act" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title></title>
    <meta content="" name="keywords">
    <meta content="" name="description">
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/weixinapi.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <%if (ActInfo != null)
          { %>
        您已经收集了<%=ActValue%>个赞
        <%}
          else
          { %>
        您只要将此页面分享到朋友圈就可以参与活动，神秘奖品等你拿，奖品真的很神秘哦！
        <%} %>
    </div>
    </form>
</body>
<script type="text/javascript">
    var code = "<%= Code%>";
    var openid = "<%= Openid %>";

    if (code == "") {
        alert("用户没有授权");
    }
    if (openid == "") {
        alert("网络异常");
    }

    $(function () {
        // 所有功能必须包含在 WeixinApi.ready 中进行
        WeixinApi.ready(function (Api) {
            // 微信分享的数据
            var wxData = {
                "imgUrl": 'http://bj.hongxu.cn/images/chabei.jpg',
                "link": "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx0c9b37c9d5ddf8a8&redirect_uri=http%3a%2f%2fbj.hongxu.cn%2fweixin%2fdianzan.aspx&response_type=code&scope=snsapi_base&state=" + openid + "#wechat_redirect",
                "desc": '参与红旭活动，得神秘奖品！',
                "title": "红旭又送礼物啦"
            };
            // 分享的回调
            var wxCallbacks = {
                // 分享操作开始之前
                ready: function () {
                    // 你可以在这里对分享的数据进行重组
                },
                // 分享被用户自动取消
                cancel: function (resp) {
                    // 你可以在你的页面上给用户一个小Tip，为什么要取消呢？
                },
                // 分享失败了
                fail: function (resp) {
                    // 分享失败了，是不是可以告诉用户：不要紧，可能是网络问题，一会儿再试试？
                },
                // 分享成功
                confirm: function (resp) {
                    // 分享成功了，我们是不是可以做一些分享统计呢？
                    doact();
                },
                // 整个分享过程结束
                all: function (resp) {
                    // 如果你做的是一个鼓励用户进行分享的产品，在这里是不是可以给用户一些反馈了？
                }
            };

            // 点击分享到朋友圈，会执行下面这个代码
            Api.shareToTimeline(wxData, wxCallbacks);
        });
    });

    function doact() {
    
        $.ajax({
            url: "weixinaction.axd",
            data: { action: "weixindoact", openid: openid, d: new Date() },
            type: 'GET',
            dataType: "json",
            error: function (msg) {
                alert("发生错误");
            },
            success: function (data) {
                if (data.Value == "success") {
                    alert("成功参与活动");
                }
                else {
                    alert(data.Msg);
                }
            }
        });
    }

</script>
</html>
