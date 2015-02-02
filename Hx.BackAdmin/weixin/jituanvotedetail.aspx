<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="jituanvotedetail.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.jituanvotedetail" EnableViewState="false" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title>选手详细信息</title>
    <meta content="" name="keywords">
    <meta content="" name="description">
    <link href="../css/jituanvote.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/weixinapi2.js" type="text/javascript"></script>
    <script src="../js/comm.js" type="text/javascript"></script>
</head>
<body>
    <div class="dwrap">
        <div class="nav">
            <a href="jituanvotepage.aspx?openid=<%=Openid %>&code=<%=Code %>" style="color: black;font-weight: bold;">←返回</a>
        </div>
        <div class="dcontent">
            <div class="c1">
                <div class="d-pic1">
                    <img src="<%= CurrentPothunterInfo.PicPath %>" alt="<%= CurrentPothunterInfo.Name %>" />
                </div>
                <div class="detailinfo">
                    <span class="name">
                        <%= CurrentPothunterInfo.Name %></span><br />
                    <span class="ballot">集赞:<span class="txtBallot"><%= CurrentPothunterInfo.Ballot%></span></span>
                    <span class="paiming" id="txtComments">被评论:<%= CurrentPothunterInfo.Comments%></span>
                    <div class="opt">
                        <a href="javascript:void(0);" class="btnComment">马上评论</a> <a href="javascript:void(0);"
                            class="btnVote hide">点赞</a></div>
                </div>
            </div>
            <div class="cintro">
                <dt>
                    <dl>
                        <p>
                            <a class="btnDetail1" href="javascript:void(0);">十年历程</a></p>
                    </dl>
                    <dr id="drIntroduce"><%= CurrentPothunterInfo.Introduce%></dr>
                </dt>
                <dt>
                    <dl>
                        <a class="btnDetail1" href="javascript:void(0);">个人风采</a>
                    </dl>
                    <br />
                    <dr style="width: 100%!important; text-align: center;"><img src="http://<%= CurrentDomain %><%= CurrentPothunterInfo.IntroducePic1%>" class="picpreview"  /><br />
                    <img src="http://<%= CurrentDomain %><%= CurrentPothunterInfo.IntroducePic2%>" class="picpreview"  /><br />
                    <img src="http://<%= CurrentDomain %><%= CurrentPothunterInfo.IntroducePic3%>" class="picpreview"  /></dr>
                </dt>
                <dt id="posComment" name="posComment">
                    <dl>
                        <a class="btnDetail2" href="javascript:void(0);">评论内容</a>
                    </dl>
                    <dr>
                       <a href="javascript:void(0);" class="btnComment fr" style="font-size:90%;">马上评论</a>
                    </dr>
                </dt>
                <dt>
                    <div class="d-comment">
                        <div class="commenthead">
                            最新评论</div>
                        <table id="tblCommentFirstTwo" style="margin-bottom: 10px;">
                            <asp:Repeater runat="server" ID="rptCommentFirstTwo">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <p>
                                                <%#Eval("Comment")%></p>
                                            <div class="dvcommentinfo">
                                                <span>
                                                    <%#Eval("AddTime","{0:yyyy-MM-dd HH:mm:ss}") %></span> <span>
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
                        <div class="commenthead" id="dvAllComment">
                            全部评论<a class="fr btnCommentMore" href="javascript:void(0)">更多>></a></div>
                        <div id="dvlComment">
                            <table id="tblComment">
                                <asp:Repeater runat="server" ID="rptComment">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <p>
                                                    <%#Eval("Comment")%></p>
                                                <div class="dvcommentinfo">
                                                    <span>
                                                        <%#Eval("AddTime","{0:yyyy-MM-dd HH:mm:ss}") %></span> <span>
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
                        <div id="dvLoading">
                            <img src="../images/waiting.gif" />
                        </div>
                        <div class="commenthead">
                            &nbsp; <a href="javascript:void(0)" class="fr btnCommentMore">更多>></a></div>
                    </div>
                </dt>
            </div>
        </div>
        <div id="dvComment">
            <input type="hidden" id="hdnCurrentid" value="<%= CurrentPothunterInfo.ID%>" />
            <textarea id="txtComment"></textarea>
            <a id="btnCommentCancel" href="javascript:void(0);">取消</a> <a id="btnCommentSubmit"
                href="javascript:void(0);">提交</a><span id="commentmsg"> </span>
        </div>
    </div>
</body>
<script type="text/javascript">
    var animatetimer = null;
    var openid = "<%= Openid %>";
    var commentpageindex = 1;
    var commentpagecount = <%=CommentPageCount %>;

    if (openid == "") {
        alert("网络异常");
    }

    $(function () {

        //检查微信内置浏览器
        var flag = WeixinApi.openInWeixin();
        if (!flag) {
            alert("请在微信内打开此页面");
            location.href = "http://m.hongxu.cn/";
        }

        $("#btnshare").click(function () {
            $(".dflay").show();
            setTimeout(function () {
                $(".dflay").hide();
            }, 2000);
        });

        // 所有功能必须包含在 WeixinApi.ready 中进行
        WeixinApi.ready(function (Api) {
            var srcList = [];
            $.each($(".picpreview,#drIntroduce img"), function (i, item) {
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
                "imgUrl": '<%= CurrentDomain %><%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareImgUrl %>',
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
                            $("#tblCommentFirstTwo").prepend($newrow);
                            var comments = parseInt($("#txtComments").text().replace("被评论:", "")) + 1;
                            $("#txtComments").text("被评论:" + comments);

                            $("html,body").animate({ scrollTop: $("#posComment").offset().top }, 500);

                            $newrow.find(".btnPraise").click(function () {
                                CommentPraise(data.Msg);
                            });
                            $newrow.find(".btnBelittle").click(function () {
                                CommentBelittle(data.Msg);
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
            var id = $("#hdnCurrentid").val();
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
        //更多评论
        $(".btnCommentMore").click(function () {
            GetCommentMore();
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
    function GetCommentMore() { 
        if(commentpageindex >= commentpagecount){
            alert("没有更多评论了");
            return;
        }
        else{
            $("#dvLoading").show();
            $("#dvlComment").hide();
            commentpageindex++;
            $.ajax({
                url: "weixinaction.axd",
                data: { action: "getcommentmore", openid: openid, id: <%=CurrentPothunterInfo.ID %>, acttype: 0,pageindex:commentpageindex, d: new Date() },
                type: 'GET',
                dataType: "json",
                error: function (msg) {
                    $("#dvLoading").hide();
                    $("#dvlComment").show();
                    alert("发生错误");
                },
                success: function (data) {
                    if (data.Value == "success") {
                        $("#dvLoading").hide();
                        $("#tblComment").html(data.Msg);
                        //鲜花
                        $(".btnPraise").unbind("click").click(function () {
                            var id = $(this).attr("val");
                            CommentPraise(id);
                        });
                        //鸡蛋
                        $(".btnBelittle").unbind("click").click(function () {
                            var id = $(this).attr("val");
                            CommentBelittle(id);
                        });
                        $("#dvlComment").show();
                        location.href = "#dvAllComment";
                    }
                    else {
                        $("#dvLoading").hide();
                        $("#dvlComment").show();
                        alert(data.Msg);
                    }
                }
            });
        }
    }
</script>
</html>
