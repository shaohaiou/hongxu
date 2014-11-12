<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="benzvotepage.aspx.cs" Inherits="Hx.BackAdmin.weixin.benzvotepage"
    EnableViewState="false" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title>投票页面</title>
    <meta content="" name="keywords">
    <meta content="" name="description">
    <link href="../css/benzvote.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/weixinapi2.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="wrap">
        <div class="dad">
            <img src="../images/benzvote/adtest.png" />
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
                            <div class="flay"><img src="../images/benzvote/flay.png" /></div>
                        </div>
                        <div class="d-info">
                            <span class="sn">NO.<span class="yellow"><%# Eval("SerialNumber")%></span></span>
                            <span class="name">
                                <%#Eval("Name") %></span><br />
                            <span class="ballot">票数:<span class="fense txtBallot"><%# Eval("Ballot")%></span></span> <span class="paiming">
                                排名:<span class="fense"><%#Eval("Order")%></span></span>
                            <div class="opt">
                                <span class="xiangqing"><a href="benzvotedetail.aspx?id=<%# Eval("ID") %>&openid=<%=Openid %>&from=<%=CurrentUrl %>">
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
        <table style="width:100%;">
            <tr>
                <td style="width: 45%;text-align: right;"><a href="<%=PrevUrl %>">
                <img src="../images/benzvote/prev.png" alt="上一页" /></a></td>
                <td style="width: 10%;text-align: center;color:White;"><%=PageIndex %> / <%=PageCount %></td>
                <td><a href="<%=NextUrl %>">
                <img src="../images/benzvote/next.png" alt="下一页" /></a></td>
            </tr>
        </table>
    </div>
    </form>
</body>
<script type="text/javascript">
    var code = "<%= Code%>";
    var openid = "<%= Openid %>";

        if (code == "") {
            alert("用户没有授权");
        }
        else if (openid == "") {
            alert("网络异常");
        }

    function toupiao(id,o) {
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

    $(function () {
        //检查微信内置浏览器
        var flag = WeixinApi.openInWeixin();
        if (!flag) {
            alert("请在微信内打开此页面");
            location.href = "http://m.hongxu.cn/";
        }


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
                "imgUrl": 'http://<%= CurrentDomain %><%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareImgUrl %>',
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
