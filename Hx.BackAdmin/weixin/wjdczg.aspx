<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wjdczg.aspx.cs" Inherits="Hx.BackAdmin.weixin.wjdczg" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <script src="../js/jquery-1.9.1.js" type="text/javascript"></script>
    <script src="../js/jweixin-1.0.0.js" type="text/javascript"></script>
    <title>2015年总经理满意度问卷-主管</title>
    <script language="javascript" type="text/javascript">
        var code = "<%= Code%>";
        var openid = "<%= Openid %>";
        var url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=<%=appid%>&redirect_uri=http%3A%2F%2Frb.hongxucar.com%2Fweixin%2Fwjdczg.aspx%3Fcid=<%=CurrentCompany==null?string.Empty:CurrentCompany.ID.ToString()%>&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
        if (!openInWeixin()) {
            alert("请在微信中进入此页面");
        }
        function openInWeixin() {
            return /MicroMessenger/i.test(navigator.userAgent);
        }
        wx.config({
            debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
            appId: '<%=appid %>', // 必填，公众号的唯一标识
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
            $("#btnSubmit").click(function () {
                if ($(".itemscore input[type='radio']:checked").length < 20) {
                    alert("请对每一道题都打分后再提交！");
                    return false;
                }
                else {
                    $(this).attr("disabled", "disabled");
                    form1.submit();
                }
            });
            wx.ready(function () {            
                // 微信分享的数据
                var wxData = {
                    "imgUrl": 'http://rb.hongxucar.com/images/weixin/weixinquestionlog.jpg',
                    "link": url,
                    "desc": '<%=CurrentCompany == null ? string.Empty : CurrentCompany.Name%>',
                    "title": "2015年红旭集团总经理满意度问卷-主管"
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

            if(openid == ""){
                location.href=url;
                return;
            }
        })
    </script>
    <style type="text/css">
    .itemscore{padding:0 1em;}
    </style>
</head>
<body style="background-color: #F3E1B0;">
    <div style="font-weight: bold;font-size: 1.1em;">
        各位经理、主管：大家好，为了更好的了解4S店总经理日常管理及员工满意度方面情况，集团人资部特此制作该电子满意度问卷，请各位红旭家人认真仔细的研读，公平公正的对待此次问卷调查，感谢您的支持与参与！
    </div>
    <div style="margin: 10px 0;font-size: 0.8em;">
        问卷说明：<br />
        1、共涉及五大方面，共计20个要素描述，共计100分；<br /> 2、以五点计分法打分，5分为最高分，1分为最低分。在方框中点“圈”标示即可，每个要素只选择1个分数；<br /> 3、5分非常同意或满意；4分比较同意或满意；3分一般同意或满意；2分比较不同意或不满意；1分非常不同意或不满意；
    </div>
    <div style="font-size: 0.8em;font-weight: bold;">
        被调查人：<%=CurrentCompany == null ? string.Empty : CurrentCompany.Manager%> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;职位：总经理&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br />所属品牌：<%=CurrentCompany == null ? string.Empty : CurrentCompany.Name%>
    </div>
    <form id="form1" runat="server" style="margin:10px 0;">
    <asp:Repeater runat="server" ID="rptProject" OnItemDataBound="rptProject_ItemDataBound">
        <ItemTemplate>
            <div style="font-weight: bold;background-color: #DCA33C;margin-bottom: 5px;font-size: 0.9em;padding: 4px 2px;">
                <%# GetNumCH(Container.ItemIndex + 1)%>、<%#Eval("Name") %></div>
            <asp:Repeater runat="server" ID="rptQuestion" OnItemDataBound="rptQuestion_ItemDataBound">
                <ItemTemplate>
                    <div style="margin-left: 12px;font-size: 0.8em;">
                        <%# Container.ItemIndex +1 %>.<%# Eval("QuestionFacor")%>：<%#Eval("QuestionIntroduce")%></div>
                    <div style="margin-left: 12px;font-size: 0.8em;margin-bottom: 5px;">
                    <asp:RadioButtonList runat="server" ID="rblScore" RepeatDirection="Horizontal" RepeatLayout="Flow" Width="100%">
                        <asp:ListItem Text="5分" Value="5" class="itemscore"></asp:ListItem>
                        <asp:ListItem Text="4分" Value="4" class="itemscore"></asp:ListItem>
                        <asp:ListItem Text="3分" Value="3" class="itemscore"></asp:ListItem>
                        <asp:ListItem Text="2分" Value="2" class="itemscore"></asp:ListItem>
                        <asp:ListItem Text="1分" Value="1" class="itemscore"></asp:ListItem>
                    </asp:RadioButtonList>
                    <input type="hidden" runat="server" id="hdnQuestionID" />
                    <input type="hidden" runat="server" id="hdnQuestionFactor" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </ItemTemplate>
    </asp:Repeater>
    <div style="margin:20px 0;">
        <asp:Button runat="server" ID="btnSubmit" style="font-size: 2em;padding: 0 2em;background-color:rgba(214, 155, 47, 0.69);" Text="提交" />
    </div>
    </form>
</body>
</html>
