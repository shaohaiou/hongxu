<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="benzvotedetailinuse.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.benzvotedetail" EnableViewState="false" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title>选手详细信息</title>
    <meta content="" name="keywords">
    <meta content="" name="description">
    <link href="../css/benzvote.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/jweixin-1.0.0.js" type="text/javascript"></script>
</head>
<body>
    <div class="dwrap">
        <div class="dad">
            <img src="../images/benzvote/head.jpg" />
        </div>
        <div class="nav">
            <a href="<%= FromUrl %>">
                <img src="../images/benzvote/back.png" /></a>
        </div>
        <div class="dcontent">
            <div class="c1">
                <div class="d-pic1">
                    <img src="<%= CurrentPothunterInfo.PicPath %>" alt="<%= CurrentPothunterInfo.Name %>" />
                </div>
                <div class="detailinfo">
                    <span class="sn">
                        <span class="yellow"><%= CurrentPothunterInfo.SerialNumberDetail%></span></span><br />
                    <span class="name yellow">
                        <%= CurrentPothunterInfo.Name %></span><br />
                    <span class="ballot">票数:<%= CurrentPothunterInfo.Ballot%></span> <span class="paiming">
                        排名:<%= CurrentPothunterInfo.Order%></span>
                    <div class="opt">
                        <span class="weitalapiao"><a href="javascript:void(0);" id="btnshare">
                            <img src="../images/benzvote/weitalapiao.png" alt="为她拉票" /></a></span><span class="weitatoupiao"><a
                                href="javascript:void(0);" onclick="javascript:toupiao(<%= CurrentPothunterInfo.ID %>);"><img
                                    src="../images/benzvote/weitatoupiao.png" alt="为她投票" /></a></span></div>
                </div>
            </div>
            <div class="cintro">
                <%--<dt>
                    <dl>
                        <img src="../images/benzvote/gerenjieshao.png" />
                    </dl>
                    <dr><%= CurrentPothunterInfo.Introduce.Replace("\r","<br>").Replace(" ","&nbsp;")%></dr>
                </dt>
                <br />
                <dt>
                    <dl>
                        <img src="../images/benzvote/cansaixuanyan.png" />
                    </dl>
                    <dr><%= CurrentPothunterInfo.Declare.Replace("\r", "<br>").Replace(" ", "&nbsp;")%></dr>
                </dt>
                <br />--%>
                <dt>
                    <dl>
                        <img src="../images/benzvote/fengcaizhanshi.png" />
                    </dl>
                    <br />
                    <dr style="width: 100%!important; text-align: center;">
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic1))
                      {%>
                    <img src="http://<%= CurrentDomain %><%= CurrentPothunterInfo.IntroducePic1%>" class="picpreview"  /><br />
                    <%} %>
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic2))
                      {%>
                    <img src="http://<%= CurrentDomain %><%= CurrentPothunterInfo.IntroducePic2%>" class="picpreview"  /><br />
                    <%} %>
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic3))
                      {%>
                    <img src="http://<%= CurrentDomain %><%= CurrentPothunterInfo.IntroducePic3%>" class="picpreview"  /><br />
                    <%} %>
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic4))
                      {%>
                    <img src="http://<%= CurrentDomain %><%= CurrentPothunterInfo.IntroducePic4%>" class="picpreview"  /><br />
                    <%} %>
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic5))
                      {%>
                    <img src="http://<%= CurrentDomain %><%= CurrentPothunterInfo.IntroducePic5%>" class="picpreview"  /><br />
                    <%} %>
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic6))
                      {%>
                    <img src="http://<%= CurrentDomain %><%= CurrentPothunterInfo.IntroducePic6%>" class="picpreview"  /><br />
                    <%} %></dr>
                </dt>
            </div>
        </div>
        <div class="dflay">
            <img src="../images/benzvote/ysj.png" style="position:absolute;" />
        </div>
    </div>
</body>
<script type="text/javascript">
    var openid = "<%= Openid %>";
    var subscribe = "<%=Subscribe %>";

    if (openid == "") {
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

    function toupiao(id) {
        $.ajax({
            url: "weixinaction.axd",
            data: { action: "benzvotetoupiao", openid: openid, id: id, d: new Date() },
            type: 'GET',
            dataType: "json",
            error: function (msg) {
                alert("系统繁忙，请稍后再试!");
            },
            success: function (data) {
                if (data.Value == "success") {
                    $(".ballot").text("票数:<%= CurrentPothunterInfo.Ballot + 1%>");
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
    'onMenuShareTimeline',
    'onMenuShareAppMessage',
    'onMenuShareQQ',
    'onMenuShareWeibo'] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
    });

    $(function () {
        $("#btnshare").click(function () {
            $(".dflay img").css({top:$(document).scrollTop()});
            $(".dflay").show();
            setTimeout(function () {
                $(".dflay").hide();
            }, 2000);
        });

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
