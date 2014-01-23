using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gravatar.NET.Exceptions;

namespace Gravatar.NET.Data
{
	/// <summary>
	/// Represents the possible URL parameters which can be used when making a request
	/// to the active account's image url
	/// </summary>
	public class GravatarUrlParameters
	{
		private int m_Size;
		private string m_DefaultUrl;

		/// <summary>
		/// The size to retrieve the image from Gravatar
		/// Possible value between 1 and 512
		/// If value is not in the acceptable range a <see cref="Gravatar.NET.GravatarUrlInvalidSizeException"/> is thrown
		/// </summary>
		public int Size
		{
			get { return m_Size; }

			set {

				if (value < 1 || value > 512) throw new GravatarUrlInvalidSizeExcetion(value);
				m_Size = value;
			}
		}

		/// <summary>
		/// The minimum rating of the image to retrieve from Gravatar
		/// </summary>
		public GravatarImageRating Rating { get; set; }
	
		/// <summary>
		/// The default option of an image to retrieve if the requested image doesnt exist for the account
		/// </summary>
		public GravatarDefaultUrlOptions DefaultOption{get;set;}

		/// <summary>
		/// If the Default option property is set to 'Custom', and the image doesnt exist for the account, 
		/// this custom URL will be used to return a default image
		/// </summary>
		public string CustomDefaultUrl
		{
			get { return m_DefaultUrl; }

			set
			{
				m_DefaultUrl = value;
				if (!String.IsNullOrEmpty(value)) DefaultOption = GravatarDefaultUrlOptions.Custom;
			}
		}

	}
}
