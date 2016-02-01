<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cardpg.aspx.cs" Inherits="Hx.BackAdmin.weixin.cardpg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=0.5, maximum-scale=2.0, user-scalable=yes" />
    <meta http-equiv="pragma" content="no-cache">
    <meta http-equiv="Cache-Control" content="no-store, must-revalidate">
    <meta http-equiv="expires" content="Wed, 26 Feb 1997 08:21:57 GMT">
    <meta http-equiv="expires" content="0">
    <title>红旭集团-卡券活动</title>
    <meta content="红旭集团,卡券活动" name="keywords">
    <meta content="红旭集团,卡券活动" name="description">
    <link href="../css/card.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/jweixin-1.0.0.js" type="text/javascript"></script>
    <script src="../js/jQuery-eraser-0.4.2.js" type="text/javascript"></script>
    <%if (!string.IsNullOrEmpty(CurrentSetting.ColorAwards))
      {%>
    <style type="text/css">
    .award{color:<%=CurrentSetting.ColorAwards%>!important;}
    .award a{color:<%=CurrentSetting.ColorAwards%>!important;}
    .award a:link{color:<%=CurrentSetting.ColorAwards%>!important;}
    .award a:visited{color:<%=CurrentSetting.ColorAwards%>!important;}
    </style>
    <%} %>
    <%if (!string.IsNullOrEmpty(CurrentSetting.ColorRule))
      {%>
    <style type="text/css">
    .rule{color:<%=CurrentSetting.ColorRule%>!important;}
    .rule a{color:<%=CurrentSetting.ColorRule%>!important;}
    .rule a:link{color:<%=CurrentSetting.ColorRule%>!important;}
    .rule a:visited{color:<%=CurrentSetting.ColorRule%>!important;}
    </style>
    <%} %>
</head>
<body>
    <div class="m-flay" id="attention" style="position: relative;">
        <div class="attention">
            <div class="attention-content">
                <span class="attention-title">请先关注我们再参与活动</span>
                <div class="attention-msg" id="attention-msg">
                    <img src="<%=CurrentSetting.AppImgUrl %>" style="width: 40%;" /><br />
                    长按二维码图片即可关注
                </div>
            </div>
        </div>
    </div>
    <div class="main" id="main">
        <img class="bg" src="<%=string.IsNullOrEmpty(CurrentSetting.BgImgUrl) ? "../images/weixin/card/cardbg.png" : CurrentSetting.BgImgUrl %>" />
        <div class="m-content">
            <div class="card">
                <img id="imgcard" src="../images/weixin/card/original.png" />
                <img id="imgcardflay" src="../images/weixin/card/original.png" />
            </div>
            <div class="award" <%= string.IsNullOrEmpty(CurrentSetting.ColorAwards) ? string.Empty : ("style=\"color:" + CurrentSetting.ColorAwards + "!important;\"") %>>
                <div class="awardtop">
                    &nbsp;</div>
                <%=RecoverHtml(CurrentSetting.Awards.Replace("\r", "<br>").Replace(" ", "&nbsp;"))%>
            </div>
            <div class="rule">
                <div class="ruletop">
                    &nbsp;</div>
                <%=RecoverHtml(CurrentSetting.ActRule.Replace("\r", "<br>").Replace(" ", "&nbsp;"))%>
            </div>
        </div>
        <div class="m-flay" id="pull-flay">
            <div class="m-flaybg">
            </div>
            <div class="pullcard">
                <div class="pullcard-content">
                    <img class="flaybg" src="../images/weixin/card/zjbg.png" />
                    <div class="pullcard-lay">
                        <p id="txtaward">
                            &nbsp;
                        </p>
                        <p id="txttitle">
                            &nbsp;
                        </p>
                        <img class="imgzj" src="" />
                        <a href="javascript:void(0);" id="btnpull"></a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
<script type="text/javascript">    
    var openid = "<%= Openid %>";
    var sid = <%= SID %>;
    var cardid = "";
    var timestamp = "";
    var signature = "";
    if(!openInWeixin()){
        alert("请在微信中进入此页面");
        location.href="http://m.hongxu.cn/";
    }
    
    function openInWeixin() {
        return /MicroMessenger/i.test(navigator.userAgent);
    }
    
    wx.config({
        debug: true, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
        appId: '<%=CurrentSetting.AppID %>', // 必填，公众号的唯一标识
        timestamp: <%=Timestamp %>, // 必填，生成签名的时间戳
        nonceStr: '<%=NonceStr %>', // 必填，生成签名的随机串
        signature: '<%=Signature %>',// 必填，签名，见附录1
        jsApiList: [
    'onMenuShareTimeline',
    'onMenuShareAppMessage',
    'onMenuShareQQ',
    'onMenuShareWeibo',
    'addCard'] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
    });

    $(function () {
        wx.ready(function () {
            
            // 微信分享的数据
            var wxData = {
                "imgUrl": 'http://<%= ImgServer %><%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareImgUrl %>',
                "link": '<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareLinkUrl %>',
                "desc": '<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareDesc %>',
                "title": '<%=CurrentSetting == null ? string.Empty : CurrentSetting.ShareTitle %>'
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
                location.href="https://open.weixin.qq.com/connect/oauth2/authorize?appid=<%=CurrentSetting.AppID %>&redirect_uri=http%3A%2F%2Frb.hongxucar.com%2Fweixin%2Fcardpg.aspx%3Fwechat_card_js=1%26sid=<%=SID %>&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
                return;
            }
            else{
                $.ajax({
                    url: "weixinaction.axd",
                    data: { action: "carddraw", openid: openid,sid:sid, d: new Date() },
                    type: 'POST',
                    dataType: "json",
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    },
                    success: function (data) {
                        if (data.Value == "success") {
                            if(data.Msg == "0"){
                                $("#imgcard").attr("src","../images/weixin/card/wzj.png");
                            }else{
                                cardid = data.Msg.split('|')[0];
                                var title = data.Msg.split('|')[1];
                                var award = data.Msg.split('|')[2]
                                $("#txttitle").html(title);
                                $("#txtaward").html(award);
                                $(".imgzj").attr("src",data.Msg.split('|')[3]);
                                timestamp = data.Msg.split('|')[4];
                                signature = data.Msg.split('|')[5];
                                $("#txttimestamp").text(timestamp);
                                $("#txtsignature").text(signature);
                                $("#txtcardid").text(cardid);

                                var imgname = "zjl.png";
                                if(award == "特等奖") imgname = "tdj.png";
                                if(award == "一等奖") imgname = "ydj.png";
                                if(award == "二等奖") imgname = "edj.png";
                                if(award == "三等奖") imgname = "sdj.png";
                                if(award == "四等奖") imgname = "sidj.png";
                                $("#imgcard").attr("src","../images/weixin/card/" + imgname);

                                $("#btnpull").click(function(){
                                    $("#btnpull").unbind("click");
                                    if(cardid != ""){
                                        $.ajax({
                                            url: "weixinaction.axd",
                                            data: { action: "cardpull", openid: openid,sid:sid, d: new Date() },
                                            type: 'POST',
                                            dataType: "json",
                                            error: function (msg) {
                                                alert("发生错误");
                                            },
                                            success: function (data) {
                                                if (data.Value == "success") {
                                                    if(cardid != ""){
                                                        wx.addCard({
                                                          cardList: [
                                                            {
                                                              cardId: cardid,
                                                              cardExt: "{\"timestamp\":\"" + timestamp + "\",\"signature\":\"" + signature +"\"}"
                                                            }
                                                          ],
                                                          success: function (res) {
//                                                            alert('已添加卡券：' + JSON.stringify(res.cardList));
                                                              alert('卡券已成功领取，请前往：我 -> 卡包 查看卡券。');
                                                          }
                                                        });
                                                    }
                                                }
                                                else {
                                                    alert(data.Msg);
                                                }
                                            }
                                        });
                                    }
                                });
                            }
                            
                            
                            $("#imgcardflay").eraser({
                                completeRatio: .4,
                                completeFunction: function(){
                                    if(cardid != "")
                                        $("#pull-flay").show();
                                }
                            });
                        }
                        else if(data.Value == "attention"){
                            if("<%=CurrentSetting.AttentionUrl %>" != ""){
                                location.href = "<%=CurrentSetting.AttentionUrl %>";
                            }else{
                                $("#attention").show();
                            }
                        }
                        else {
                            alert(data.Msg);
                        }
                    }
                });
            }
        });
    })
</script>
</html>
