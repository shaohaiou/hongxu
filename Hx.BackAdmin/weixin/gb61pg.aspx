<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gb61pg.aspx.cs" Inherits="Hx.BackAdmin.weixin.gb61pg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>玩转六一 缤纷梦工场——报名活动开始了</title>
    <link href=<%=ResourceServer%>/css/gb61.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script type="text/javascript">        
        $(function () {
            $(".begin-btn").bind('click', function () {
//                $(".main-inner").stop().animate({ "margin-left": "-100%" }, 200);
                $("#spec_name").val("111");
                $(".main-inner").stop().animate({ "margin-left": "-200%" }, 200);
            });

            $(".mod-list li").each(function () {
                var $this = $(this);

                $this.bind('click', function () {
                    $(this).addClass('selected').siblings("li").removeClass('selected');

                    $(".next-btn").attr("data-val", $(this).attr("data-name"));
                    $(".next-btn").hasClass("checked") ? $(".next-btn").removeClass('checked') : "";
                });

            });

            $(".next-btn").bind('click', function () {
                if ($(this).attr("data-val") != "") {
                    $("#spec_name").val($(this).attr("data-val"));
                    $(".main-inner").stop().animate({ "margin-left": "-200%" }, 200);
                }
            });


            $(".share-btn").bind('click', function () {
                $("body").append('<div class="tipDiv"></div><div class="shadow"></div>');
                $(".tipDiv,.shadow").bind('click', function () {
                    $(".tipDiv,.shadow").remove();
                });
            });



            $(".lxbtn").bind('click', function () {

                var reg = /^1\d{10}$/;
                if ($("#cname").val() == '') {
                    alert("请输入姓名");
                    $("#cname").focus();
                    return false;
                } else if ($("#phone").val() == '') {
                    alert("请输入手机号码");
                    $("#phone").focus();
                    return false;
                } else if (!reg.test($("#phone").val())) {
                    alert("请输入正确的手机号码");
                    $("#phone").focus();
                    return false;
                } else {
                    $.ajax({
                        url: "/weixinaction.axd",
                        data: { action: "gb61", cname: $("#cname").val(), phone: $("#phone").val(), spec_name: $("#spec_name").val(), d: new Date() },
                        type: 'post',
                        dataType: 'json',
                        success: function (data) {
                            if (data.Value == "success") {
                                alert("提交成功");
                            } else {
                                alert(data.Msg);
                            }

                        },
                        error: function (a, b, c) {
                            dump_obj(a);
                        }
                    })
                }
            })


        });

        function dump_obj(myObject) {
            var s = "";
            for (var property in myObject) {
                s = s + "\n " + property + ": " + myObject[property];
            }
            alert(s);
        }
    </script>
</head>
<body>
    <div id="wraper">
        <img src="../images/weixin/gb61/logo.png" style="width:60%;margin:1% 0 0 2%;float:left;" />
        <div class="topic">
        </div>
        <div class="main">
            <div class="main-inner">
                <div class="mod">
                    <div class="info">
                        1、活动日预约订车客户+10元送市场价550元儿童座椅或市场价560元山地自行车（不折现）<br />2、"你游园，我送礼"来店客户参加游园活动即可抽奖，人人中奖；趣味无限，惊喜不断！</div>
                    <div class="main-btn">
                        <a href="javascript:void(0)" class="btn begin-btn">我要报名</a></div>
                </div>
                <div class="mod">
                    <h3 class="mod-title">
                        选择您的意向车型</h3>
                    <div class="mod-list">
                        <ul>
                            <li data-name="奥德赛">
                                <div>
                                    <b>奥德赛</b></div>
                            </li>
                            <li data-name="飞度">
                                <div>
                                    <b>飞度</b></div>
                            </li>
                            <li data-name="雅阁">
                                <div>
                                    <b>雅阁</b></div>
                            </li>
                            <li data-name="缤智">
                                <div>
                                    <b>缤智</b></div>
                            </li>
                            <li data-name="凌派">
                                <div>
                                    <b>凌派</b></div>
                            </li>
                            <li data-name="锋范">
                                <div>
                                    <b>锋范</b></div>
                            </li>
                            <li data-name="歌诗图">
                                <div>
                                    <b>歌诗图</b></div>
                            </li>
                            <li data-name="理念">
                                <div>
                                    <b>理念</b></div>
                            </li>
                        </ul>
                    </div>
                    <div class="main-btn">
                        <a href="javascript:void(0)" class="btn next-btn checked" data-val="">下一步</a><!--  data-val的值 对应li选择后（selected）的 data-name  --></div>
                </div>
                <div class="mod">
                    <h3 class="mod-title">
                        请输入您的联系方式</h3>
                    <div class="form-wrap">
                        <dl>
                            <dt>姓名：</dt><dd><input name="cname" id="cname" type="text" class="input" /></dd></dl>
                        <dl>
                            <dt>手机：</dt><dd><input name="phone" id="phone" type="text" class="input" /><input
                                name="spec_name" id="spec_name" type="hidden" /></dd></dl>
                    </div>
                    <div class="main-btn">
                        <a href="javascript:void(0);" class="btn lxbtn">确定提交</a></div>
                    <span class="share-btn">分享到朋友圈</span></div>
            </div>
        </div>
    </div>
    <div id="wx_pic" style="margin: 0 auto; display: none;">
        <img src="../images/weixin/gb61/111.png" /></div>
</body>
</html>
