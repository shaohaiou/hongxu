<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="votepg.aspx.cs" Inherits="Hx.BackAdmin.weixin.votepg"
    EnableViewState="false" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title>
        <%=CurrentSetting.Name %></title>
    <meta content="红旭集团,<%=CurrentSetting.Name %>" name="keywords">
    <meta content="红旭集团,<%=CurrentSetting.Name %>" name="description">
    <link href=<%=ResourceServer%>/css/benzvote.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src="/js/jweixin-1.0.0.js" type="text/javascript"></script>
    <%if (!string.IsNullOrEmpty(CurrentSetting.ColorMustKnow))
      {%>
    <style type="text/css">
        .xuzhi
        {
            color: <%=CurrentSetting.ColorMustKnow%> !important;
        }
        .xuzhi a
        {
            color: <%=CurrentSetting.ColorMustKnow%> !important;
        }
        .xuzhi a:link
        {
            color: <%=CurrentSetting.ColorMustKnow%> !important;
        }
        .xuzhi a:visited
        {
            color: <%=CurrentSetting.ColorMustKnow%> !important;
        }
    </style>
    <%} %>
</head>
<body>
    <div class="m-flay" id="attention" style="position: relative;">
        <div class="attention">
            <div class="attention-content">
                <div class="attention-msg" id="attention-msg">
                    <img src="<%=ImgServer %><%=CurrentSetting.AppImgUrl %>" style="width: 100%;" /><br />
                    长按二维码图片即可关注
                </div>
            </div>
        </div>
    </div>
    <form id="form1" runat="server">
    <div class="wrap">
        <div class="dad">
            <%if (CurrentSetting != null && !string.IsNullOrEmpty(CurrentSetting.PageHeadImg))
              { %>
            <img src="<%=ImgServer %><%= CurrentSetting.PageHeadImg%>" />
            <%}
              else
              { %>
            <img src="../images/benzvote/head.jpg" />
            <%} %>
        </div>
        <% if (CurrentSetting != null)
           {%>
        <div class="xuzhi">
            <%= CurrentSetting.MustKnow.Replace("\r","<br>").Replace(" ","&nbsp;")%>
        </div>
        <%} %>
        <div class="content">
            <asp:Repeater runat="server" ID="rptData" OnItemDataBound="rptData_ItemDataBound">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <div class="d-pic">
                            <img src="<%=ImgServer %><%# Eval("PicPath") %>" alt="<%#Eval("Name") %>" />
                            <div class="flay">
                                <img src="../images/benzvote/flay.png" /></div>
                        </div>
                        <div class="d-info">
                            <span class="name green">
                                <%#Eval("Name") %></span><br />
                            <%# string.IsNullOrEmpty(Eval("SerialNumberDetail").ToString()) ? string.Empty : string.Format("<span class=\"sn\"><span class=\"green\">{0}</span></span><br />",Eval("SerialNumberDetail").ToString())%>
                            <span class="ballot green">票数:<span class="fense txtBallot"><%# Eval("Ballot")%></span></span>
                            <span class="paiming green">排名:<span class="fense"><%#Eval("Order")%></span></span>
                            <div class="opt">
                                <span class="xiangqing"><a href="votepothunterdetail.aspx?sid=<%=SID %>&id=<%# Eval("ID") %>&code=<%=Code %>&from=<%=CurrentUrl %>">
                                    <img src="../images/benzvote/xiangqing.png" alt="详情" /></a></span><span class="toupiao"><a
                                        href="javascript:void(0);" onclick="javascript:toupiao(<%# Eval("ID") %>,this);"><img
                                            src="../images/benzvote/toupiao.png" alt="投票" /></a></span><a href="javascript:void(0);"
                                                class="btnComment hide" val="<%#Eval("ID") %>"> 评论</a> <a href="javascript:void(0);"
                                                    class="btnCommentMore" val="<%#Eval("ID") %>">更多评论 </a>
                            </div>
                        </div>
                        <div class="d-comment" id="d-comment<%#Eval("ID") %>">
                            <table id="tblCommentFirstOne<%#Eval("ID") %>">
                                <asp:Repeater runat="server" ID="rptCommentFirstOne">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <p>
                                                    <%#Eval("Comment")%></p>
                                                <div class="dvcommentinfo">
                                                    <span>
                                                        <%#Eval("AddTime","{0:HH:mm:ss}") %></span> <span>
                                                            <%# string.IsNullOrEmpty(Eval("Commenter").ToString().Trim()) ? "匿名" : Eval("Commenter").ToString()%></span>
                                                </div>
                                                <div class="dvcommentopt">
                                                    <a href="javascript:void(0);" class="btnPraise" val="<%#Eval("ID") %>">鲜花</a>(<span
                                                        id="spPraise<%#Eval("ID") %>"><%#Eval("PraiseNum")%></span>) <a href="javascript:void(0);"
                                                            class="btnBelittle" val="<%#Eval("ID") %>">鸡蛋</a>(<span id="spBelittle<%#Eval("ID") %>"><%#Eval("BelittleNum")%></span>)
                                                </div>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
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
        <%if (!string.IsNullOrEmpty(CurrentSetting.AD3Path))
          { %>
        <div class="dad" style="background: white;">
            <a href='<%=string.IsNullOrEmpty(CurrentSetting.AD3Url) ? "javascript:void(0);" : CurrentSetting.AD3Url%>'>
                <img src="<%=ImgServer %><%=CurrentSetting.AD3Path %>" style="width: 96%; padding: 2%;" /></a>
        </div>
        <%} %>
        <div id="dvComment">
            <input type="hidden" id="hdnCurrentid" />
            <textarea id="txtComment"></textarea>
            <a id="btnCommentCancel" href="javascript:void(0);">取消</a> <a id="btnCommentSubmit"
                href="javascript:void(0);">提交</a><span id="commentmsg"> </span>
        </div>
    </div>
    </form>
</body>
<script type="text/javascript">
    var sid = <%= SID %>;
    var code = "<%= Code%>";
    var openid = "<%= Openid %>";
    var animatetimer = null;

    <%if(NeedAttention){ %>
    if("<%=CurrentSetting.AttentionUrl %>" != ""){
        location.href = "<%=CurrentSetting.AttentionUrl %>";
    }else{
        $("#attention").show();
    }
    <%} %>

    if(!openInWeixin()){
        alert("请在微信中进入此页面");
    }
    
    function openInWeixin() {
        return /MicroMessenger/i.test(navigator.userAgent);
    }

    function toupiao(id, o) {
        $.ajax({
            url: "weixinaction.axd",
            data: { action: "votetoupiao",sid:sid, openid: openid, id: id, d: new Date() },
            type: 'GET',
            dataType: "json",
            error: function (msg) {
                alert("系统繁忙，请稍后再试!");
            },
            success: function (data) {
                if (data.Value == "success") {
                    var t = $(o).parent().parent().parent().find(".txtBallot");
                    if (t && t.length > 0)
                        t.text(parseInt(t.text()) + 1);
                    alert("投票成功，感谢您的参与。");
                }
                else if(data.Msg == "openid,vopenid为空")
                    location.href ="https://open.weixin.qq.com/connect/oauth2/authorize?appid=<%=CurrentSetting.AppID%>&redirect_uri=http%3A%2F%2Frb.hongxucar.com%2Fweixin%2Fvotepg.aspx%3Fsid=<%=CurrentSetting.ID%>&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                else 
                    alert(data.Msg);
            }
        });
    }

    wx.config({
        debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
        appId: '<%=CurrentSetting.AppID %>', // 必填，公众号的唯一标识
        timestamp: <%=Timestamp %>, // 必填，生成签名的时间戳
        nonceStr: '<%=NonceStr %>', // 必填，生成签名的随机串
        signature: '<%=CurrentSignature %>',// 必填，签名，见附录1
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
                "imgUrl": '<%= ImgServer %><%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareImgUrl %>',
                "link": "<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareLinkUrl %>",
                "desc": '<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareDesc.Replace("\r\n",string.Empty) %>',
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

            if(openid == ""){
                location.href="https://open.weixin.qq.com/connect/oauth2/authorize?appid=<%=CurrentSetting.AppID%>&redirect_uri=http%3A%2F%2Frb.hongxucar.com%2Fweixin%2Fvotepg.aspx%3Fsid=<%=CurrentSetting.ID%>&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                return;
            }

        });
        

        $(".btnComment").click(function () {
            var top = ($("body").scrollTop() + 20) + "px";
            $("#txtComment").val("");
            $("#dvComment").css({ top: top });
            $("#hdnCurrentid").val($(this).attr("val"));
            $("#dvComment").fadeIn(200);
        });
        $(document).scroll(function () {
            if (animatetimer)
                clearTimeout(animatetimer);
            animatetimer = setTimeout(function () {
                var top = ($("body").scrollTop() + 20) + "px";
                $("#dvComment").animate({ top: top }, { speed: 200 });
            }, 100);
        });
        $("#btnCommentCancel").click(function () {
            $("#dvComment").fadeOut(200);
        });
        $(".btnCommentMore").click(function () {
            location.href = "votepothunterdetail.aspx?id=" + $(this).attr("val") + "&sid=<%=SID %>&code=<%=Code %>&from=<%=CurrentUrl %>#dvAllComment";
        });
        //提交评论
        $("#btnCommentSubmit").click(function () {
            if ($.trim($("#txtComment").val()) != "") {
                $.ajax({
                    url: "weixinaction.axd",
                    data: { action: "votecomment",sid:sid, openid: openid, id: $("#hdnCurrentid").val(), comment: $("#txtComment").val(), acttype: 0, d: new Date() },
                    type: 'POST',
                    dataType: "json",
                    error: function (msg) {
                        alert("发生错误");
                    },
                    success: function (data) {
                        if (data.Value == "success") {
                            var id = data.Msg.split(",")[0];
                            var commenter = data.Msg.split(",")[1];
                            var $newrow = $("<tr><td><p>"
                            + $("#txtComment").val() + "</p><div class=\"dvcommentinfo\"><span>"
                            + (new Date()).Format("hh:mm:ss") + "</span> <span>" + commenter + "</span>"
                            + "</div><div class=\"dvcommentopt\">"
                            + "<a href=\"javascript:void(0);\" class=\"btnPraise\" val=\"" + id + "\">鲜花</a>(<span id=\"spPraise" + id + "\">0</span>)"
                            + "<a href=\"javascript:void(0);\" class=\"btnBelittle\" val=\"" + id + "\">鸡蛋</a>(<span id=\"spBelittle" + id + "\">0</span>)"
                            + "</div></td></tr>");
                            if ($("#d-comment" + $("#hdnCurrentid").val()).is(":hidden")) {
                                $("#d-comment" + $("#hdnCurrentid").val()).slideDown(function () {
                                    $("#tblCommentFirstOne" + $("#hdnCurrentid").val()).prepend($newrow);
                                });
                            }
                            else {
                                $("#tblCommentFirstOne" + $("#hdnCurrentid").val()).prepend($newrow);
                            }
//                            var comments = parseInt($("#txtComments" + $("#hdnCurrentid").val()).text().replace("被评论:", "")) + 1;
//                            $("#txtComments" + $("#hdnCurrentid").val()).text("被评论:" + comments);

                            $newrow.find(".btnPraise").click(function () {
                                CommentPraise(id);
                            });
                            $newrow.find(".btnBelittle").click(function () {
                                CommentBelittle(id);
                            });
                        }
                        else {
                            alert(data.Msg);
                        }
                    }
                });

                $("#dvComment").hide();
            } else {
                $("#commentmsg").text("请输入评论内容");
                setTimeout(function () {
                    $("#commentmsg").text("");
                }, 1000);
            }
        });
        //鲜花
        $(".btnPraise").click(function () {
            var id = $(this).attr("val");
            CommentPraise(id);
        });
        //鸡蛋
        $(".btnBelittle").click(function () {
            var id = $(this).attr("val");
            CommentBelittle(id);
        });
    })
    function CommentPraise(id) {
        $.ajax({
            url: "weixinaction.axd",
            data: { action: "votecommentpraise",sid:sid,  openid: openid, id: id, d: new Date() },
            type: 'GET',
            dataType: "json",
            error: function (msg) {
                alert("发生错误");
            },
            success: function (data) {
                if (data.Value == "success") {
                    var t = $("#spPraise" + id);
                    if (t && t.length > 0) {
                        t.text(parseInt(t.text()) + 1);
                        t.parent().find(".btnPraise").addClass("gray");
                        t.parent().find(".btnPraise").unbind("click");
                    }
                }
                else {
                    alert(data.Msg);
                }
            }
        });
    }
    function CommentBelittle(id) {
        $.ajax({
            url: "weixinaction.axd",
            data: { action: "votecommentbelittle",sid:sid,  openid: openid, id: id, d: new Date() },
            type: 'GET',
            dataType: "json",
            error: function (msg) {
                alert("发生错误");
            },
            success: function (data) {
                if (data.Value == "success") {
                    var t = $("#spBelittle" + id);
                    if (t && t.length > 0) {
                        t.text(parseInt(t.text()) + 1);
                        t.parent().find(".btnBelittle").addClass("gray");
                        t.parent().find(".btnBelittle").unbind("click");
                    }
                }
                else {
                    alert(data.Msg);
                }
            }
        });
    }
</script>
</html>
