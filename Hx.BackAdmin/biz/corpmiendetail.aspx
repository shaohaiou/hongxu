<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="corpmiendetail.aspx.cs"
    Inherits="Hx.BackAdmin.biz.corpmiendetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>集团荣誉预览</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        var runner_timer;
        var width_souce;
        var childcount;
        $(function () {
            width_souce = $(".content ul").width();
            if (width_souce > 480) {
                childcount = $(".content ul li").length;
                $(".content ul").width($(".content ul").width() * 2);
                $(".content ul").html($(".content ul").html() + $(".content ul").html());
                Run();
            }

            $(".content").hover(function () {
                if (runner_timer != null)
                    clearTimeout(runner_timer);
            }, function () { 
                if(width_souce > 480)
                    Run();
            });
        })

        function Run() {
            runner_timer = setTimeout(function () {
                var ml = parseInt($(".content ul").css("marginLeft"));
                if ((width_souce + ml) == 0) {
                    $(".content ul li:lt(" + childcount + ")").remove();
                    $(".content ul").html($(".content ul").html() + $(".content ul").html());
                    $(".content ul").css("margin-left", "0px");
                    ml = 0;
                }
                $(".content ul").css("margin-left", (ml - 1) + "px");
                Run();
            }, 20);
        }
    </script>
    <style type="text/css">
        .content
        {
            max-width: 480px;
            height: 160px;
            margin: 0 auto;
            position: relative;
            overflow: hidden;
        }
        .content ul li
        {
            float: left;
            width: 120px;
            height: 114px;
            padding: 23px 0;
            display: inline;
            margin:0 5px;
        }
    </style>
</head>
<body>
    <div class="content">
        <ul style="width: <%= RecordCount * 120%>px">
            <asp:Repeater runat="server" ID="rptData">
                <ItemTemplate>
                    <li><a href="corpmienview.aspx?id=<%#Eval("ID") %>" title="<%#Eval("Introduce") %>"
                        target="_blank" style="display: block; position: relative; padding-top: 100px;
                        width: 120px; text-align: center;">
                        <img src="<%= ImgServer%><%#Eval("Pic")%>" alt="" style="width: 100px; height: 100px; position: absolute;
                            left: 10px; top: 0;" />
                        <%#Hx.Tools.StrHelper.GetFuzzyChar(Eval("Introduce").ToString(),25) %></a></li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>
</body>
</html>
