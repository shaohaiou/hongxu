<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cxedit.aspx.cs" Inherits="Hx.BackAdmin.car.cxedit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>车型管理</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/comm.js type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".stext").dblclick(function () {
                $(this).toggleClass("hide");
                $(this).parent().find(".sname").toggleClass("hide");
                $(this).parent().find(".ssave").toggleClass("hide");
            });
            $(".ssave").click(function () {
                $(this).toggleClass("hide");
                $(this).parent().find(".stext").toggleClass("hide");
                $(this).parent().find(".sname").toggleClass("hide");

                var sname = $(this).parent().find(".sname").val();
                var snameold = $(this).attr("data-sname");
                if (sname == snameold) return;

                showLoading("正在保存数据，请稍候...");

                var stext = $(this).parent().find(".stext");
                $.ajax({
                    url: "backadminaction.axd",
                    data: { action: "updateseriesname", bname: "<%=CurrentCarbrand.Name %>", sname: sname, snameold: snameold, d: new Date() },
                    type: 'POST',
                    dataType: "json",
                    error: function (msg) {
                        alert("发生错误");
                        closeLoading();
                    },
                    success: function (data) {
                        if (data.Value == "success") {
                            stext.text(sname);
                            stext.parent().find(".ssave").attr("data-sname", sname);
                            stext.parent().parent().find(".mtext").each(function () {
                                var mtext = $(this).text();
                                var mname = sname + mtext.substring(snameold.length);
                                $(this).text(mname);
                                $(this).next().val(mname);
                                $(this).parent().find(".msave").attr("data-mname", mname);
                            });
                            closeLoading();
                        }
                        else {
                            alert(data.Msg);
                            closeLoading();
                        }
                    }
                });
            });
            $(".mtext").dblclick(function () {
                $(this).toggleClass("hide");
                $(this).parent().find(".mname").toggleClass("hide");
                $(this).parent().find(".medit").toggleClass("hide");
                $(this).parent().find(".msave").toggleClass("hide");
            });
            $(".msave").click(function () {
                $(this).toggleClass("hide");
                $(this).parent().find(".mtext").toggleClass("hide");
                $(this).parent().find(".mname").toggleClass("hide");
                $(this).parent().find(".medit").toggleClass("hide");

                var mname = $(this).parent().find(".mname").val();
                var mnameold = $(this).attr("data-mname");
                if (mname == mnameold) return;

                showLoading("正在保存数据，请稍候...");

                var mtext = $(this).parent().find(".mtext");
                $.ajax({
                    url: "backadminaction.axd",
                    data: { action: "updatecxmc", id: $(this).attr("data-id"), mname: mname, d: new Date() },
                    type: 'POST',
                    dataType: "json",
                    error: function (msg) {
                        alert("发生错误");
                        closeLoading();
                    },
                    success: function (data) {
                        if (data.Value == "success") {
                            mtext.text(mname);
                            mtext.parent().find(".msave").attr("data-mname", mname);
                            closeLoading();
                        }
                        else {
                            alert(data.Msg);
                            closeLoading();
                        }
                    }
                });
            });
        })
    </script>
    <style type="text/css">
    .yxcx ul{display:inline-block;width:330px;*display:inline;*zoom:1;vertical-align:top;margin-left:3px;}
    .nh{color:White;font-weight:bold;font-size:18px;background:#222;text-indent:5px;}
    .sname{border:0px;}
    .mname{padding:0px;width:280px;font-size:11px;}
    .medit{width:18px;height:18px;background:url(../images/edit.png) no-repeat;position:absolute;margin-left:2px;}
    .msave{width:18px;height:18px;background:url(../images/save.png) no-repeat;position:absolute;margin-left:2px;}
    .sedit{width:21px;height:21px;background:url(../images/editb.png) no-repeat;position:absolute;margin-left:2px;}
    .ssave{width:21px;height:21px;background:url(../images/saveb.png) no-repeat;position:absolute;margin-left:2px;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                车型管理</caption>
            <tr>
                <td class="bg1">
                    品牌：
                </td>
                <td>
                    <asp:Label runat="server" ID="lblCarbrand"></asp:Label>
                    <a href="carmodeledit.aspx?bid=<%=CurrentCarbrand.ID %>&from=<%=CurrentUrl %>" class="blue">+新增车型</a>
                    <asp:Button runat="server" ID="btnBack" Text="返回" OnClick="btnBack_Click" CssClass="an1" />
                </td>
            </tr>
            <tr>
                <td class="bg1">
                </td>
                <td class="yxcx">
                    <asp:Repeater runat="server" ID="rptAutomotivetype">
                        <ItemTemplate>
                            <%# GetNewAutomotivetypeStr(Eval("cCxmc").ToString())%>
                            <li class="blockinline" style="width: 330px; line-height: 18px;">
                                <span class="mtext" style="line-height: 18px;"><%# Eval("cCxmc")%></span><input type="text" value="<%# Eval("cCxmc")%>" class="hide mname"><a href="carmodeledit.aspx?id=<%#Eval("ID") %>&from=<%=CurrentUrl %>" class="medit"><a href="javascript:void(0);" data-id="<%#Eval("ID") %>" data-mname="<%# Eval("cCxmc")%>" class="msave hide"></a></a>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </td>
            </tr>
            <tbody>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
