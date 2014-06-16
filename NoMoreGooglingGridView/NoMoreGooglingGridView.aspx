<%@ Page Language="C#" ValidateRequest="false" AutoEventWireup="true" CodeFile="NoMoreGooglingGridView.aspx.cs"
    Inherits="NoMoreGooglingGridView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="txtAspContent" runat="server" TextMode="MultiLine" Width="80%" Height="411px"></asp:TextBox>
        <asp:Button ID="btn1" runat="server" Text="click me" OnClick="btn1_Click" />
        <asp:GridView runat="server" ID="gg1" AutoGenerateColumns="false">
            <Columns>
                <asp:TemplateField HeaderText="ID">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lbl1" Text="Name1"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Name2" Text="Name2"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:PlaceHolder runat="server" ID="plcHolder1"></asp:PlaceHolder>
    </div>
    </form>
</body>
</html>
