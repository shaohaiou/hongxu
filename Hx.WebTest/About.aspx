<%@ Page Title="关于我们" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="About.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        关于
    </h2>
    <p>
        <asp:Repeater runat="server" ID="rptRooms">
            <HeaderTemplate>
                <ul>
                <li><span style="display:inline-block;width:100px;font-weight:bolder;">ID</span><span style="display:inline-block;width:100px;font-weight:bolder;">Name</span></li>
            </HeaderTemplate>
            <ItemTemplate>
                <li><span style="display:inline-block;width:100px;"><%# Eval("ID")%></span><span style="display:inline-block;width:100px;"><%# Eval("Name")%></span></li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
    </p>
</asp:Content>
