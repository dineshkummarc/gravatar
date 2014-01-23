<%@ Page Title="Gravatar.NET : Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
	CodeBehind="Default.aspx.cs" Inherits="Gravatar.Demo.Web._Default" %>
	<%@ Register Src="~/Controls/GravatarImage.ascx" TagPrefix="grav" TagName="Image" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<h2>
		Showing Gravatar image for address: <%=TestImage.Email%>
	</h2>

	<p>
		<grav:Image Id="TestImage" runat="server" Email="<enter gravatar account email here>"
												  Size="120"/>
	</p>

	<h2>
		Showing Gravatar image for non-existing address:
	</h2>

	<p>
		<grav:Image Id="Image1" runat="server" Email="abc123@gmeil.com"
													Size="120"
													DefaultImage="Monsterid"/>
													  
	</p>

	<h2>
		Showing Gravatar image for non-existing address with custom fallback:
	</h2>

	<p>
		<grav:Image Id="Image2" runat="server" Email="abc123@gmeil.com"
													Size="120"
													DefaultImage="Custom"
													DefaultCustomImageUrl="http://cdn3.sbnation.com/imported_assets/122780/sad_smiley_by_shangyne.jpg"/>
													  
	</p>

</asp:Content>
