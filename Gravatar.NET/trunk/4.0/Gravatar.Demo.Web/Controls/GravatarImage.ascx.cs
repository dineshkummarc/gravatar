using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gravatar.NET;
using Gravatar.NET.Data;
using Gravatar.NET.Exceptions;

namespace Gravatar.Demo.Web.Controls
{
	public partial class GravatarImage : System.Web.UI.UserControl
	{
		private int m_Size = 80;
		private GravatarDefaultUrlOptions m_DefaultImage = GravatarDefaultUrlOptions.Monsterid;
		private GravatarImageRating m_Rating = GravatarImageRating.G;

		public string Email { get; set; }

		public int Size
		{
			get { return m_Size; }
			set { if (value > 0 && value < 512) m_Size = value; }
		}

		public GravatarDefaultUrlOptions DefaultImage
		{
			get { return m_DefaultImage; }
			set { m_DefaultImage = value; }				
		}

		public string DefaultCustomImageUrl { get; set; }

		public GravatarImageRating Rating
		{
			get { return m_Rating; }
			set { m_Rating = value; }
		}

		public string AltTest { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var pars = new GravatarUrlParameters
				{
					Size = this.Size,
					Rating = this.Rating,
					DefaultOption = this.DefaultImage,
					CustomDefaultUrl = this.DefaultCustomImageUrl				
				};

				var imageUrl = GravatarService.GetGravatarUrlForAddress(Email, pars);

				var imgControl = new Image();

				imgControl.ImageUrl = imageUrl;
				imgControl.AlternateText = AltTest;
				imgControl.ClientIDMode = System.Web.UI.ClientIDMode.AutoID;

				this.Controls.Add(imgControl);
			}
			catch (GravatarEmailHashFailedException ex)
			{
				Response.Write("Sorry, the email you supplied is invalid");
			}
		}
	}
}