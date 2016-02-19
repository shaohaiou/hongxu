<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="jituanvotepage.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.jituanvotepage" EnableViewState="false" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title>红旭集团历年十年功勋员工</title>
    <meta content="" name="keywords">
    <meta content="" name="description">
    <link href=<%=ResourceServer%>/css/jituanvote.css?t=1.0.1" rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/weixinapi2.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/comm.js type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="wrap">
        <div class="dad">
            <img src="../images/jituanvote/logo.jpg" />
        </div>
        <div class="dad">
            <img src="../images/jituanvote/shinianyuangong.jpg" />
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
                        </div>
                        <div class="d-info">
                            <span class="name">
                                <%#Eval("Name") %></span><br />
                            <span class="ballot">集赞:<span class="txtBallot"><%# Eval("Ballot")%></span></span>
                            <span class="paiming" id="txtComments<%#Eval("ID") %>">被评论:<%#Eval("Comments")%></span>
                            <div class="opt">
                                <a href="jituanvotedetail.aspx?id=<%# Eval("ID") %>&openid=<%=Openid %>&code=<%=Code %>"
                                    class="btnDetail" val="<%#Eval("ID") %>">十年历程</a> <a href="javascript:void(0);" class="btnVote"
                                        val="<%#Eval("ID") %>">点赞</a><a href="javascript:void(0);" class="btnComment hide" val="<%#Eval("ID") %>">
                                            评论</a> <a href="javascript:void(0);" class="btnCommentMore" val="<%#Eval("ID") %>">更多评论 </a>
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
        <div style="width: 50%; margin: 0 auto; font-size: 90%;display:none;">

            关注红旭集团官方微信，<br />
            点击下方#走进红旭#可以“领取关注礼”
        </div>
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
    var code = "<%= Code%>";
    var openid = "<%= Openid %>";
    var animatetimer = null;

    if (code == "") {
        location.href = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx0c9b37c9d5ddf8a8&redirect_uri=http%3A%2F%2Fbj.hongxu.cn%2Fweixin%2Fjituanvotepage.aspx&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
    }
    else if (openid == "") {
        location.href = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx0c9b37c9d5ddf8a8&redirect_uri=http%3A%2F%2Fbj.hongxu.cn%2Fweixin%2Fjituanvotepage.aspx&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
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
                "imgUrl": '<%= ImgServer %><%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareImgUrl %>',
                "link": "<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareLinkUrl %>",
                "desc": '<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareDesc %>',
                "title": "<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareTitle %>"
            };
            // 点击分享到朋友圈，会执行下面这个代码
            Api.shareToTimeline(wxData, null);
            Api.shareToFriend(wxData, null);
            Api.shareToWeibo(wxData, null);
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
            location.href = "jituanvotedetail.aspx?id=" + $(this).attr("val") + "&openid=<%= Openid %>&code<%= Code %>#dvAllComment";
            //$("#tblCommentMore" + $(this).attr("val")).slideToggle();
        });
        //提交评论
        $("#btnCommentSubmit").click(function () {
            if ($.trim($("#txtComment").val()) != "") {
                $.ajax({
                    url: "weixinaction.axd",
                    data: { action: "comment", openid: openid, id: $("#hdnCurrentid").val(), comment: $("#txtComment").val(), acttype: 0, d: new Date() },
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
                            var comments = parseInt($("#txtComments" + $("#hdnCurrentid").val()).text().replace("被评论:", "")) + 1;
                            $("#txtComments" + $("#hdnCurrentid").val()).text("被评论:" + comments);

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
        //点赞
        $(".btnVote").click(function () {
            var id = $(this).attr("val");
            var o = $(this);
            $.ajax({
                url: "weixinaction.axd",
                data: { action: "jituanvotetoupiao", openid: openid, id: id, d: new Date() },
                type: 'GET',
                dataType: "json",
                error: function (msg) {
                    alert("发生错误");
                },
                success: function (data) {
                    if (data.Value == "success") {
                        var t = o.parent().parent().find(".txtBallot");
                        if (t && t.length > 0)
                            t.text(parseInt(t.text()) + 1);
                        alert("点赞成功，谢谢参与！");
                    }
                    else {
                        alert(data.Msg);
                    }
                }
            });
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
            data: { action: "commentpraise", openid: openid, id: id, acttype: 0, d: new Date() },
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
            data: { action: "commentbelittle", openid: openid, id: id, acttype: 0, d: new Date() },
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
    //"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx0c9b37c9d5ddf8a8&redirect_uri=http%3A%2F%2Fbj.hongxu.cn%2Fweixin%2Fjituanvotepage.aspx&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect"
</script>
</html>
