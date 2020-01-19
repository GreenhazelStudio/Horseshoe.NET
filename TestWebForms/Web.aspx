<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Web.aspx.cs" Inherits="TestWebForms.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Styles" runat="server">
    <style type="text/css">

        .test-table {
            border: 2px solid black;
            border-collapse: collapse;
            text-align: left;
            margin-top: 20px;
        }

            .test-table td,
            .test-table th {
                padding: 0 3px;
            }

            .test-table .section-header {
                background-color: #2f0664;
                color: white;
                font-size: 1.35em;
            }

        .error-cell {
            font-family: Consolas, monospace;
            font-size: .85em;
            color: red;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel runat="server" ID="BootstrapAlertArea"></asp:Panel>
    <asp:Table ID="Table1" runat="server" CssClass="test-table">
        <asp:TableRow>
            <asp:TableHeaderCell>quick alerts</asp:TableHeaderCell>
            <asp:TableCell>
                <asp:LinkButton ID="infoAlertTest" runat="server" OnClick="infoAlertTest_Click">info alert test</asp:LinkButton><br />
                <asp:LinkButton ID="infoAlertHtmlTest" runat="server" OnClick="infoAlertHtmlTest_Click">info alert HTML test</asp:LinkButton><br />
                <asp:LinkButton ID="infoAlertHtmlEncodeTest" runat="server" OnClick="infoAlertHtmlEncodeTest_Click">info alert HTML encode test</asp:LinkButton><br />
                <asp:LinkButton ID="closeableWarningAlert" runat="server" OnClick="closeableWarningAlert_Click">closeable warning alert test</asp:LinkButton><br />
                <asp:LinkButton ID="errorAlertHtmlTest" runat="server" OnClick="errorAlertHtmlTest_Click">error alert HTML test</asp:LinkButton><br />
                <asp:LinkButton ID="errorAlertHtmlEncodeTest" runat="server" OnClick="errorAlertHtmlEncodeTest_Click">error alert HTML encode test</asp:LinkButton><br />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Content>
