<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RaffleWebRole.Default" %>
<asp:Content runat="server" ID="PageContent" ContentPlaceHolderID="PageContent">
    <asp:Label ID="Label1" runat="server" Text="Raffle ID: "></asp:Label>
    <asp:Label runat="server" ID="labelRaffleId"></asp:Label>
    <br/>
    <asp:Label ID="Label2" runat="server" Text="Raffle Status: "></asp:Label>
    <asp:Label runat="server" ID="labelRaffleStatus"></asp:Label>
    <br/>
    <asp:Label ID="labelMessage" runat="server" Text="Accepting bets"></asp:Label>
    <br/>
    <asp:Label runat="server" ID="labelBets" Text="Placed bets:"></asp:Label>
    <br/>
    <asp:ListBox ID="lbBets" runat="server" Width="400px" Height="100px"></asp:ListBox>
    <br/>
    <div runat="server" id="divPlaceBet">
        <asp:Label ID="Label3" runat="server" Text="Next bet: "></asp:Label>
        <asp:Label runat="server" ID="labelBetTicketNumber" Width="50"/>
        <asp:TextBox runat="server" ID="tbBetNumber"/>
        <asp:Button runat="server" ID="btnPlaceBet" Text="Palce Bet" Width="150" OnClick="btnPlaceBet_OnClick"/>
    </div>
    <br/>
    <br/>
    <asp:Button runat="server" ID="btnStartStopRaffle" OnClick="btnStartStopRaffle_OnClick" Width="150"/>
    <br/>
    <br/>
    <div runat="server" id="divResult">
        <asp:Button runat="server" ID="btnGetResults" Text="Check results" OnClick="btnGetResults_OnClick" Width="150"/>
        <br/>
        <asp:Label runat="server" ID="labelResult"></asp:Label>
    </div>
</asp:Content>