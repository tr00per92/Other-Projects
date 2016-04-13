<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebRoleTest.Default" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <asp:Button ID="Button1" runat="server" Text="Post queue message" OnClick="Button1_Click"/>
    <asp:Button ID="Button2" runat="server" Text="Upload pic to blob" OnClick="Button2_Click"/>
    <asp:Button ID="Button3" runat="server" Text="Download pic from blob" OnClick="Button3_Click"/>
    <asp:Button ID="Button4" runat="server" Text="Create entity in table storage" OnClick="Button4_Click"/>
    <asp:Button ID="Button5" runat="server" Text="Read entity from table storage" OnClick="Button5_Click"/>

</asp:Content>