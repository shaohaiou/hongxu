<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="benzvotedetail.aspx.cs"
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
    <script src="../js/weixinapi2.js" type="text/javascript"></script>
</head>
<body>
    <div class="dwrap">
        <div class="dcontent">
            <div class="c1">
                <div class="d-pic1">
                    <img src="<%= CurrentPothunterInfo.PicPath %>" alt="<%= CurrentPothunterInfo.Name %>" />
                </div>
                <div class="detailinfo">
                    <span class="sn">NO.<%= CurrentPothunterInfo.SerialNumber%></span><span class="name">
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
                <dt>
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
                <br />
                <dt>
                    <dl>
                        <img src="../images/benzvote/fengcaizhanshi.png" />
                    </dl>
                    <br />
                    <dr style="width: 100%!important; text-align: center;"><img src="http://bj.hongxu.cn<%= CurrentPothunterInfo.IntroducePic1%>" class="picpreview"  /><br />
                    <img src="http://bj.hongxu.cn<%= CurrentPothunterInfo.IntroducePic2%>" class="picpreview"  /><br />
                    <img src="http://bj.hongxu.cn<%= CurrentPothunterInfo.IntroducePic3%>" class="picpreview"  /></dr>
                </dt>
            </div>
        </div>
    </div>
</body>
<script type="text/javascript">
    var openid = "<%= Openid %>";

    if (openid == "") {
        alert("网络异常");
    }

    function toupiao(id) {
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
                    $(".ballot").text("票数:<%= CurrentPothunterInfo.Ballot + 1%>");
                    alert("投票成功，感谢您的参与。");
                }
                else {
                    alert(data.Msg);
                }
            }
        });
    }

    $(function () {

        //检查微信内置浏览器
        var flag = WeixinApi.openInWeixin();
        if (!flag) {
            alert("请在微信内打开此页面");
            location.href = "http://www.hongxu.cn/";
        }

        $("#btnshare").click(function () {
            alert("请点击右上角的分享按钮进行分享。");
        });

        // 所有功能必须包含在 WeixinApi.ready 中进行
        WeixinApi.ready(function (Api) {
            var srcList = [];
            $.each($(".picpreview"), function (i, item) {
                if (item.src) {
                    srcList.push(item.src);
                    $(item).click(function (e) {
                        // 通过这个API就能直接调起微信客户端的图片播放组件了
                        Api.imagePreview(srcList, this.src);
                    });
                }
            });

            // 微信分享的数据
            var wxData = {
                "imgUrl": 'http://bj.hongxu.cn<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareImgUrl %>',
                "link": "<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareLinkUrl %>",
                "desc": '<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareDesc %>',
                "title": "<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareTitle %>"
            };
            // 点击分享到朋友圈，会执行下面这个代码
            Api.shareToTimeline(wxData, null);
        });
    })
</script>
</html>
