<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dianzan.aspx.cs" Inherits="Hx.BackAdmin.weixin.dianzan" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title></title>
    <meta content="" name="keywords">
    <meta content="" name="description">
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <%if (ActInfo != null)
          { %>
        您进入的是
        <%= ActInfo.Nickname%>
        的活动页面，他已经收集了 <span style="color:Red;"><%=ActValue%></span> 个赞，点击 <a href="javascript:dianzan();">赞</a> 为他再添一赞吧<br />
        您只要关注 红旭集团 就可以到活动页面参加此活动，神秘奖品等你拿，奖品真的很神秘哦！
        <%}%>
    </div>
    </form>
</body>
<script type="text/javascript">
    var code = "<%= Code%>";
    var openid = "<%= Openid %>";
    var vopenid = "";
    <%if (ActInfo != null)
          { %>
          vopenid = "<%= ActInfo.Openid%>";
          <%}%>

    if (code == "") {
        alert("用户没有授权");
    }
    if (openid == "") {
        alert("网络异常");
    }

    function dianzan() {
        $.ajax({
            url: "weixinaction.axd",
            data: { action: "weixindianzan", openid: openid,vopenid:vopenid, d: new Date() },
            type: 'GET',
            dataType: "json",
            error: function (msg) {
                alert("发生错误");
            },
            success: function (data) {
                if (data.Value == "success") {
                    alert("提交成功");
                }
                else {
                    alert(data.Msg);
                }
            }
        });
    }

</script>
</html>
