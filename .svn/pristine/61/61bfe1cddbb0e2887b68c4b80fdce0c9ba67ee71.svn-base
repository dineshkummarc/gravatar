<%@ Page Title="Gravatar.NET : Methods Demo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Methods.aspx.cs" Inherits="Gravatar.Demo.Web.Methods" Async="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

	<h2>
		Demoing different methods exposed by the Gravatar.NET API
	</h2>
	
	<asp:Label runat="server" ID="InfoLabel" ForeColor="Blue" Font-Size="18px"/>		
	<br />

	<asp:LoginView ID="LoginViewControl" runat="server">
		<AnonymousTemplate>
			<asp:Login runat="server" ID="LogControl" RememberMeSet="False" LoginButtonText="Login to Gravatar" 
																			TitleText = "Gravatar Login Form"
																			UserNameLabelText = "Email:"
																			OnAuthenticate="OnLogAuthenticate" />		
		</AnonymousTemplate>
		<LoggedInTemplate>
			
			<asp:Button runat="server" ID="ButtonLoadPhotos" OnClick="LoadPhotos" Text="Load Gravatar Photos" />			
		</LoggedInTemplate>		
	</asp:LoginView>			

	<asp:Panel runat="server" ID="PhotosPanel">
		
	
	</asp:Panel>
	<br />
	
	<asp:Button runat="server" ID="ButtonDelete" Visible="true" Text="Delete Selected" CssClass="hidden" ClientIDMode="Static" OnClick="DeletePhoto" />
	&nbsp;&nbsp;
	<asp:Button runat="server" ID="ButtonSet" Visible="true" Text="Activate Selected" CssClass="hidden" ClientIDMode="Static" OnClick="ActivatePhoto" />
	<br />

	<asp:HiddenField ID="PhotoId" runat="server"  ClientIDMode="Static"/>

	<script type="text/javascript">

		$(document).ready(function ()
		{
			$("#ButtonDelete").click(function ()
			{
				return confirm("Are you sure you wish to delete this photo?");
			});

			$(".gravPhoto").click(function ()
			{
				$(".gravPhoto").removeClass("selectedPhoto");

				var selected = $(this).addClass("selectedPhoto");

				$("#PhotoId").val(selected.attr("id"));
				$("#ButtonDelete, #ButtonSet").removeClass("hidden");				
			});
		});

	</script>
</asp:Content>
