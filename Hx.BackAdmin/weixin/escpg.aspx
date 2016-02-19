<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="escpg.aspx.cs" Inherits="Hx.BackAdmin.weixin.escpg" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title>红旭集团-爱车车价评估器</title>
    <meta content="红旭集团,爱车车价评估器" name="keywords">
    <meta content="红旭集团,爱车车价评估器" name="description">
    <link href=<%=ResourceServer%>/css/escpg.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src="/js/jweixin-1.0.0.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        $(function () {
            $("#btnsubmit").click(function () {
                if ($("#ddlBrand").val() == "-1") {
                    alert("请选择品牌");
                    return;
                }
                if ($("#ddlChexi").val() == "-1") {
                    alert("请选择车系");
                    return;
                }
                if ($("#ddlNianfen").val() == "-1") {
                    alert("请选择年份");
                    return;
                }
                if ($("#txtPhone").val() == "") {
                    alert("请填写手机号码");
                    return;
                }
                if (!(/^1[3|4|5|8][0-9]\d{4,8}$/.test($("#txtPhone").val()))) {
                    alert("请填写正确的手机号码");
                    return;
                }
                $("#hdnIspostdata").val("1");
                form1.submit();
            });
        })
    </script>
</head>
<body>
    <div class="dwrap">
        <div class="dad">
            <img alt="" src="../images/weixin/escpg/head.png" />
        </div>
        <div class="main">
            <div class="title">
                <span>爱车信息</span></div>
            <div class="content">
                <form id="form1" runat="server">
                <asp:ScriptManager ID="sm1" runat="server">
                </asp:ScriptManager>
                <asp:UpdatePanel runat="server" ID="upl1">
                    <ContentTemplate>
                        <ul>
                            <li><span class="ctxt">品牌</span><asp:DropDownList runat="server" ID="ddlBrand" AutoPostBack="true" OnSelectedIndexChanged="ddlBrand_SelectedIndexChanged">
                            </asp:DropDownList>
                            </li>
                            <li><span class="ctxt">车系</span><asp:DropDownList runat="server" ID="ddlChexi" AutoPostBack="true" OnSelectedIndexChanged="ddlChexi_SelectedIndexChanged">
                            </asp:DropDownList>
                            </li>
                            <li><span class="ctxt">年份</span><asp:DropDownList runat="server" ID="ddlNianfen" AutoPostBack="true" OnSelectedIndexChanged="ddlNianfen_SelectedIndexChanged">
                            </asp:DropDownList>
                            </li>
                            <li><span class="ctxt">款式</span><asp:DropDownList runat="server" ID="ddlKuanshi">
                            </asp:DropDownList>
                            </li>
                            <li><span class="ctxt">里程</span><input type="text" class="txt" id="txtLicheng" runat="server" placeholder="请填写" /></li>
                            <li class="cl2"><span class="ctxt">手机号码</span><input type="text" class="txt" id="txtPhone" runat="server" placeholder="请填写" /></li>
                        </ul>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <input type="hidden" runat="server" id="hdnIspostdata" value="0" />
                </form>
            </div>
            <div class="des">
                <p>
                    为确保您的信息私密性，我们将评估结果</p>
                <p>
                    以短信方式发送至您手机上，请注意查收</p>
                <p>
                    凭本短信可在车展现场领取礼品一份</p>
            </div>
            <div class="submit">
                <a href="javascript:void(0);" id="btnsubmit">
                    <img src="../images/weixin/escpg/btnsubmit.png" style="width: 30%" /></a>
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
