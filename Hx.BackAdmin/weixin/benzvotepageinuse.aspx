<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="benzvotepageinuse.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.benzvotepage" EnableViewState="false" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title>投票页面</title>
    <meta content="红旭集团" name="keywords">
    <meta content="红旭集团" name="description">
    <link href="../css/benzvote.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/jweixin-1.0.0.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="wrap">
        <div class="dad">
            <img src="../images/benzvote/head.jpg" />
        </div>
        <% if (CurrentSetting != null)
           {%>
        <div class="xuzhi">
            <%= CurrentSetting.MustKnow.Replace("\r","<br>").Replace(" ","&nbsp;")%>
        </div>
        <%} %>
        <div class="content">
            <asp:Repeater runat="server" ID="rptData">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <div class="d-pic">
                            <img src="<%# Eval("PicPath") %>" alt="<%#Eval("Name") %>" />
                            <div class="flay">
                                <img src="../images/benzvote/flay.png" /></div>
                        </div>
                        <div class="d-info">
                            <span class="sn"><span class="yellow">
                                <%# Eval("SerialNumberDetail")%></span></span><br />
                            <span class="name yellow">
                                <%#Eval("Name") %></span><br />
                            <span class="ballot">票数:<span class="fense txtBallot"><%# Eval("Ballot")%></span></span>
                            <span class="paiming">排名:<span class="fense"><%#Eval("Order")%></span></span>
                            <div class="opt">
                                <span class="xiangqing"><a href="benzvotedetailinuse.aspx?id=<%# Eval("ID") %>&code=<%=Code %>&from=<%=CurrentUrl %>">
                                    <img src="../images/benzvote/xiangqing.png" alt="详情" /></a></span><span class="toupiao"><a
                                        href="javascript:void(0);" onclick="javascript:toupiao(<%# Eval("ID") %>,this);"><img
                                            src="../images/benzvote/toupiao.png" alt="投票" /></a></span></div>
                        </div>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul></FooterTemplate>
            </asp:Repeater>
        </div>
        <%if (PageCount > 1)
          { %>
        <table style="width: 38%; margin: 0 auto;" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td style="width: 30%; padding: 2%;">
                    <a href="<%=PrevUrl %>">
                        <img src="../images/jituanvote/prev.png" alt="上一页" style="display: block; width: 100%;" /></a>
                </td>
                <td style="width: 40%; text-align: center; color: White;">
                    <%=PageIndex %>
                    /
                    <%=PageCount %>
                </td>
                <td style="text-align: right; padding: 2%; width: 30%;">
                    <a href="<%=NextUrl %>">
                        <img src="../images/jituanvote/next.png" alt="下一页" style="display: block; float: right;
                            width: 100%;" /></a>
                </td>
            </tr>
        </table>
        <%} %>
    </div>
    </form>
</body>
<script type="text/javascript">
    var code = "<%= Code%>";
    var openid = "<%= Openid %>";
    var subscribe = "<%=Subscribe %>";

    if (code == "") {
        alert("用户没有授权");
    }
    else if (openid == "") {
        alert("网络异常,请重新进入页面");
    }
    //    if (subscribe == "0") {
    //        alert("请先关注 红旭集团 公众号再打开此页面");
    //        location.href = "http://m-hxjt.app2biz.com/activity_12962.html";
    //    }

    if(!openInWeixin()){
        alert("请在微信中进入此页面");
        location.href="http://m.hongxu.cn/";
    }
    
    function openInWeixin() {
        return /MicroMessenger/i.test(navigator.userAgent);
    }

    function toupiao(id, o) {
        $.ajax({
            url: "weixinaction.axd",
            data: { action: "benzvotetoupiao", openid: openid, id: id, d: new Date() },
            type: 'GET',
            dataType: "json",
            error: function (msg) {
                alert("发生错误");
            },
            success: function (data) {
                if (data.Value == "success") {
                    var t = $(o).parent().parent().parent().find(".txtBallot");
                    if (t && t.length > 0)
                        t.text(parseInt(t.text()) + 1);
                    alert("投票成功，感谢您的参与。");
                }
                else {
                    alert(data.Msg);
                }
            }
        });
    }

    wx.config({
        debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
        appId: 'wx0c9b37c9d5ddf8a8', // 必填，公众号的唯一标识
        timestamp: <%=Timestamp %>, // 必填，生成签名的时间戳
        nonceStr: '<%=NonceStr %>', // 必填，生成签名的随机串
        signature: '<%=Signature %>',// 必填，签名，见附录1
        jsApiList: [
        'checkJsApi',
        'onMenuShareTimeline',
        'onMenuShareAppMessage',
        'onMenuShareQQ',
        'onMenuShareWeibo'] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
    });

    $(function () {

        wx.ready(function () {            
            // 微信分享的数据
            var wxData = {
                "imgUrl": 'http://<%= CurrentDomain %><%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareImgUrl %>',
                "link": "<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareLinkUrl %>",
                "desc": '<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareDesc %>',
                "title": "<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareTitle %>"
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
