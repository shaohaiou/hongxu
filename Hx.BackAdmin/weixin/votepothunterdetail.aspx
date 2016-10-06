<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="votepothunterdetail.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.votepothunterdetail" EnableViewState="false" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <title><%=CurrentSetting.Name %>-选手信息</title>
    <meta content="" name="keywords">
    <meta content="" name="description">
    <link href=<%=ResourceServer%>/css/benzvote.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src="/js/jweixin-1.0.0.js" type="text/javascript"></script>
</head>
<body>
    <%if (NeedAttention && CurrentSetting.ShowAppImg == 1)
      { %>
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
    <%} %>
    <div class="dwrap">
        <%if (!string.IsNullOrEmpty(CurrentSetting.AD1Path))
          { %>
        <div class="dad" style="background: white;">
            <a href='<%=string.IsNullOrEmpty(CurrentSetting.AD1Url) ? "javascript:void(0);" : CurrentSetting.AD1Url%>'>
                <img src="<%=ImgServer %><%=CurrentSetting.AD1Path %>" style="width: 96%; padding: 2%;" /></a>
        </div>
        <%} %>
        <%if (!string.IsNullOrEmpty(CurrentSetting.AD2Path))
          { %>
        <div class="dad" style="background: white;">
            <a href='<%=string.IsNullOrEmpty(CurrentSetting.AD2Url) ? "javascript:void(0);" : CurrentSetting.AD2Url%>'>
                <img src="<%=ImgServer %><%=CurrentSetting.AD2Path %>" style="width: 96%; padding: 2%;" /></a>
        </div>
        <%} %>
        <%if (!string.IsNullOrEmpty(CurrentSetting.AD5Path))
          { %>
        <div class="dad" style="background: white;">
            <a href='<%=string.IsNullOrEmpty(CurrentSetting.AD5Url) ? "javascript:void(0);" : CurrentSetting.AD5Url%>'>
                <img src="<%=ImgServer %><%=CurrentSetting.AD5Path %>" style="width: 96%; padding: 2%;" /></a>
        </div>
        <%} %>
        <div class="nav">
            <a href="<%= FromUrl %>">
                <img src="../images/benzvote/back.png" /></a>
        </div>
        <div class="dcontent">
            <div class="c1">
                <div class="d-pic1">
                    <img src="<%=ImgServer %><%= CurrentPothunterInfo.PicPath %>" alt="<%= CurrentPothunterInfo.Name %>" />
                </div>
                <div class="detailinfo">
                    <span class="name green">
                        <%= CurrentPothunterInfo.Name %></span><br />
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.SerialNumberDetail))
                      { %>
                    <span class="sn"><span class="green">
                        <%= CurrentPothunterInfo.SerialNumberDetail%></span></span><br />
                    <%} %>
                    <span class="ballot green">票数:<%= CurrentPothunterInfo.Ballot%></span> <span class="paiming green">
                        排名:<%= CurrentPothunterInfo.Order%></span>
                    <div class="opt">
                        <a href="javascript:void(0);" class="btnComment">马上留言</a><%if (CurrentSetting.IsMulselect == 0)
                                                                                   { %> <a href="javascript:toupiao(<%= CurrentPothunterInfo.ID %>);"
                            class="btnVote hide">投票</a><%} %></div>
                </div>
            </div>
            <div class="cintro">
                <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.Declare)){ %>
                <dt>
                    <dl>
                        <img src="../images/benzvote/cansaixuanyan.png" /></dl>
                    <br />
                    <dr>
                        <%= CurrentPothunterInfo.Declare%>
                    </dr>
                </dt>
               <br />
               <%} %>
                <dt>
                    <dl>
                        <img src="../images/benzvote/fengcaizhanshi.png" />
                    </dl>
                    <br />
                    <dr style="width: 100%!important; text-align: center;">
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic1))
                      {%>
                    <img src="http://rb.hongxu.cn<%= CurrentPothunterInfo.IntroducePic1%>" class="picpreview"  /><br />
                    <%} %>
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic2))
                      {%>
                    <img src="http://rb.hongxu.cn<%= CurrentPothunterInfo.IntroducePic2%>" class="picpreview"  /><br />
                    <%} %>
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic3))
                      {%>
                    <img src="http://rb.hongxu.cn<%= CurrentPothunterInfo.IntroducePic3%>" class="picpreview"  /><br />
                    <%} %>
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic4))
                      {%>
                    <img src="http://rb.hongxu.cn<%= CurrentPothunterInfo.IntroducePic4%>" class="picpreview"  /><br />
                    <%} %>
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic5))
                      {%>
                    <img src="http://rb.hongxu.cn<%= CurrentPothunterInfo.IntroducePic5%>" class="picpreview"  /><br />
                    <%} %>
                    <%if (!string.IsNullOrEmpty(CurrentPothunterInfo.IntroducePic6))
                      {%>
                    <img src="http://<%= ImgServer %><%= CurrentPothunterInfo.IntroducePic6%>" class="picpreview"  /><br />
                    <%} %></dr>
                </dt>
                <dt id="posComment" name="posComment">
                    <dl>
                        <a class="btnDetail2" href="javascript:void(0);">留言内容</a>
                    </dl>
                    <dr>
                       <a href="javascript:void(0);" class="btnComment fr" style="font-size:90%;">马上留言</a>
                    </dr>
                </dt>
                <dt>
                    <div class="d-comment">
                        <div class="commenthead">
                            最新留言</div>
                        <table id="tblCommentFirstTwo" style="margin-bottom: 10px;">
                            <asp:Repeater runat="server" ID="rptCommentFirstTwo">
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
                        <%if (!string.IsNullOrEmpty(CurrentSetting.AD4Path))
                          { %>
                        <div class="dad" style="background: white;">
                            <a href='<%=string.IsNullOrEmpty(CurrentSetting.AD4Url) ? "javascript:void(0);" : CurrentSetting.AD4Url%>'>
                                <img src="<%=ImgServer %><%=CurrentSetting.AD4Path %>" style="width: 96%; padding: 2%;" /></a>
                        </div>
                        <%} %>
                        <div class="commenthead" id="dvAllComment">
                            全部留言<a class="fr btnCommentMore" href="javascript:void(0)">更多>></a></div>
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
                        <div id="dvLoading">
                            <img src="../images/waiting.gif" />
                        </div>
                        <div class="commenthead">
                            &nbsp; <a href="javascript:void(0)" class="fr btnCommentMore">更多>></a></div>
                    </div>
                </dt>
            </div>
        </div>
        <div class="dflay">
            <img src="../images/benzvote/ysj.png" style="position: absolute;" />
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
    var sid = <%= SID %>;
    var openid = "<%= Openid %>";
    var commentpageindex = 1;
    var commentpagecount = <%=CommentPageCount %>;

    <%if(NeedAttention){ %>
    if("<%=CurrentSetting.AttentionUrl %>" != "" && "<%=CurrentSetting.MustAttention %>" == 1){
        location.href = "<%=CurrentSetting.AttentionUrl %>";
    }else{
        $("#attention").show();
    }
    <%} %>

    if(!openInWeixin()){
        alert("请在微信中进入此页面");
//        location.href="http://m.hongxu.cn/";
    }
    
    function openInWeixin() {
        return /MicroMessenger/i.test(navigator.userAgent);
    }

    function toupiao(id) {
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
        appId: '<%=CurrentSetting.AppID %>', // 必填，公众号的唯一标识
        timestamp: <%=Timestamp %>, // 必填，生成签名的时间戳
        nonceStr: '<%=NonceStr %>', // 必填，生成签名的随机串
        signature: '<%=CurrentSignature %>',// 必填，签名，见附录1
        jsApiList: [
    'onMenuShareTimeline',
    'onMenuShareAppMessage',
    'onMenuShareQQ',
    'onMenuShareWeibo',
    'previewImage',
    'checkJsApi'] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
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
            var srcList = [];
            $.each($(".picpreview,#drIntroduce img"), function (i, item) {
                if (item.src) {
                    srcList.push(item.src);
                    $(item).click(function (e) {
                        // 通过这个API就能直接调起微信客户端的图片播放组件了
                        wx.previewImage({
                            current:this.src, 
                            urls:srcList
                        });
                    });
                }
            });

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

        //提交留言
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
                            $("#tblCommentFirstTwo").prepend($newrow);
                            var comments = parseInt($("#txtComments").text().replace("被留言:", "")) + 1;
                            $("#txtComments").text("被留言:" + comments);

                            $("html,body").animate({ scrollTop: $("#posComment").offset().top }, 500);

                            $newrow.find(".btnPraise").click(function () {
                                CommentPraise(data.Msg);
                            });
                            $newrow.find(".btnBelittle").click(function () {
                                CommentBelittle(data.Msg);
                            });
                        }
                        else if(data.Msg == "openid,vopenid为空")
                            location.href ="https://open.weixin.qq.com/connect/oauth2/authorize?appid=<%=CurrentSetting.AppID%>&redirect_uri=http%3A%2F%2Frb.hongxucar.com%2Fweixin%2Fvotepg.aspx%3Fsid=<%=CurrentSetting.ID%>&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                        else {
                            alert(data.Msg);
                        }
                    }
                });

                $("#dvComment").hide();
            } else {
                $("#commentmsg").text("请输入留言内容");
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
        //更多留言
        $(".btnCommentMore").click(function () {
            GetCommentMore();
        });
    })
    
    function CommentPraise(id) {
        $.ajax({
            url: "weixinaction.axd",
            data: { action: "votecommentpraise",sid:sid, openid: openid, id: id, acttype: 0, d: new Date() },
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
            data: { action: "votecommentbelittle",sid:sid, openid: openid, id: id, acttype: 0, d: new Date() },
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
            alert("没有更多留言了");
            return;
        }
        else{
            $("#dvLoading").show();
            $("#dvlComment").hide();
            commentpageindex++;
            $.ajax({
                url: "weixinaction.axd",
                data: { action: "getvotecommentmore",sid:sid, openid: openid, id: <%=CurrentPothunterInfo.ID %>, acttype: 0,pageindex:commentpageindex, d: new Date() },
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
