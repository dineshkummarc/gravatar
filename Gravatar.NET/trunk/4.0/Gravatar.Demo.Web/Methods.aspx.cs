using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gravatar.NET;
using Gravatar.NET.Data;

namespace Gravatar.Demo.Web
{
	public partial class Methods : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void OnLogAuthenticate(object sender, AuthenticateEventArgs e)
		{
			var c = sender as Login;

			var service = new GravatarService(c.UserName, c.Password);

			GravatarServiceResponse response = service.Test();

			if (!response.IsError)
			{
				if (response.IntegerResponse > 0) //If integer response is higher than 0 then the method was successful and credentials were accepted by Gravatar
				{
					e.Authenticated = true;
					InfoLabel.Text = "Successfully logged in to Gravatar";

					Session["G_Email"] = c.UserName;
					Session["G_Password"] = c.Password;
				}
			}
			else
			{
				c.FailureText = response.ErrorInfo;
			}
		}

		protected void ActivatePhoto(object sender, EventArgs e)
		{
			var service = new GravatarService((string)Session["G_Email"], (string)Session["G_Password"]);

			GravatarServiceResponse response = service.UseUserImage(PhotoId.Value, new[] { service.Email });

			if (!response.IsError)
			{
				if (response.MultipleOperationResponse.First()) //An image can be activated for single or multiple accounts therefore the MultipleOperationResponse is used
				{
					InfoLabel.Text = "Photo was selected";
				}
				else
					InfoLabel.Text = "Gravatar refused to set photo as active";
			}
			else
			{
				InfoLabel.Text = "Couldn't set active photo: " + response.ErrorInfo;
			}
		}

		protected void LoadPhotos(object sender, EventArgs e)
		{
			var service = new GravatarService((string)Session["G_Email"], (string)Session["G_Password"]);

			GravatarServiceResponse response = service.UserImages();

			if (!response.IsError)
			{
				foreach (GravatarUserImage img in response.ImagesResponse) //ImagesResponse returns a collection of Gravatar images for this account
				{
					var imgControl = new Image
					{
						ImageUrl = img.Url,
						Width = Unit.Pixel(100),
						Height = Unit.Pixel(100),
						CssClass = "gravPhoto",
						ClientIDMode = System.Web.UI.ClientIDMode.Static,
						ID = img.Name
					};

					PhotosPanel.Controls.Add(imgControl);
				}
			}
			else
			{
				InfoLabel.Text = "Couldn't get photos: " + response.ErrorInfo;
			}
		}

		protected void DeletePhoto(object sender, EventArgs e)
		{
			var service = new GravatarService((string)Session["G_Email"], (string)Session["G_Password"]);

			GravatarServiceResponse response = service.DeleteUserImage(PhotoId.Value);

			if (!response.IsError)
			{
				if (response.BooleanResponse) //boolean response specifying whether delete was successful or not
				{
					InfoLabel.Text = "Deleted photo";
				}
				else
					InfoLabel.Text = "Gravatar refused to delete the photo";
			}
			else
			{
				InfoLabel.Text = "Couldn't delete photo: " + response.ErrorInfo;
			}

		}
	}
}