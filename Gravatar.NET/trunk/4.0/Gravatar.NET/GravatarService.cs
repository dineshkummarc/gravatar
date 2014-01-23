// Gravatar.NET
// Copyright (c) 2010 Yoav Niran
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Gravatar.NET.Data;
using Gravatar.NET.Exceptions;

namespace Gravatar.NET
{
	/// <summary>
	/// The GravatarService class wraps around the raw xml-rpc API of Gravatar.com
	/// to give .NET clients easy and structured way of using the API to manage, upload and remove
	/// images for a Gravatar Account
	/// </summary>	
	public sealed partial class GravatarService
	{
		#region Private Members

		private string m_HashedEmail = null;

		private const string GRAVATAR_API_URL = "https://secure.gravatar.com/xmlrpc?user={0}";

		private const string XML_TYPE = "text/xml";
		private const string HTTP_POST = "POST";
		
		private const string PAR_PASSWORD = "password";
		private const string PAR_HASHES = "hashes";
		private const string PAR_DATA = "data";
		private const string PAR_RATING = "rating";
		private const string PAR_URL = "url";
		private const string PAR_USER_IMAGE = "userimage";
		private const string PAR_ADDRESSES = "addresses";

		private const string URL_PAR_SIZE = "s={0}&";
		private const string URL_PAR_RATING = "r={0}&";
		private const string URL_PAR_DEFAULT = "d={0}";
		private const string URL_PAR_404 = "404";

		#endregion

		public GravatarService(string email, string password)
		{
			this.Email = email;
			this.Password = password;
		}

		#region Properties

		public const string GRAVATAR_URL_BASE = "http://www.gravatar.com/avatar/";

		public string Email { get; private set; }
		public string Password { get; private set; }

		/// <summary>
		/// The last Request XML that was sent to the Gravatar server
		/// </summary>
		public string LastRequestXml { get; private set; }
		#endregion

		#region Gravatar API Methods

		/// <summary>
		/// A test function
		/// </summary>
		/// <returns>If the method call is successful, the result will be returned using the IntegerResponse property of the 		
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method.
		/// </returns>
		public GravatarServiceResponse Test()
		{
			var request = GetTestMethodRequest();

			var response = ExecuteGravatarMethod(request);

			return response;
		}

		/// <summary>
		/// Check whether a hash has a gravatar 
		/// </summary>
		/// <returns>If the method call is successful, the result will be returned using the MultipleOperationResponse property
		/// of the <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method</returns>
		/// <param name="addresses">an array of hashes to check</param>
		public GravatarServiceResponse Exists(string[] addresses)
		{
			var request = GetExistsMethodRequest(addresses);

			var response = ExecuteGravatarMethod(request);
			
			return response;
		}
	
		/// <summary>
		/// Get a list of addresses for this account 
		/// </summary>
		/// <returns>If the method call is successful, the result will be returned using the AddressesResponse property of the
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method</returns>		
		public GravatarServiceResponse Addresses()
		{
			var request = GetAddressesMethodRequest();

			var response = ExecuteGravatarMethod(request);

			return response;
		}

		/// <summary>
		/// Return an array of userimages for this account 
		/// </summary>
		/// <returns>If the method call is successful, the result will be returned using the ImagesResponse property of the
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method</returns>
		public GravatarServiceResponse UserImages()
		{
			var request = GetUserImagesMethodRequest();

			var response = ExecuteGravatarMethod(request);
		
			return response;
		}

		/// <summary>
		/// Save binary image data as a userimage for this account 
		/// </summary>
		/// <param name="data">The image data in bytes</param>
		/// <param name="rating">The rating of the image (g, pg, r, x)</param>
		/// <returns>If the method call is successful, the result will be returned using the SaveResponse property of the
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method</returns>
		public GravatarServiceResponse SaveData(byte[] data, GravatarImageRating rating)
		{
			var request = GetSaveDataMethodRequest(data, rating);

			var response = ExecuteGravatarMethod(request);
			
			return response;
		}

		/// <summary>
		/// Read an image via its URL and save that as a userimage for this account 
		/// </summary>
		/// <param name="url">a full url to an image</param>
		/// <param name="rating">The rating of the image (g, pg, r, x)</param>
		/// <returns>If the method call is successful, the result will be returned using the SaveResponse property of the
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method</returns>
		public GravatarServiceResponse SaveUrl(string url, GravatarImageRating rating)
		{
			var request = GetSaveUrlMethodRequest(url, rating);

			var response = ExecuteGravatarMethod(request);
		
			return response;
		}

		/// <summary>
		/// Use a userimage as a gravatar for one of more addresses on this account
		/// </summary>
		/// <param name="imageId">The userimage you wish to use</param>
		/// <param name="addresses">A list of the email addresses you wish to use this userimage for</param>
		/// <returns>If the method call is successful, the result will be returned using the MultipleOperationResponse property
		/// of the <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method</returns>
		public GravatarServiceResponse UseUserImage(string userImage, string[] addresses)
		{
			var request = GetUseUserImagesMethodRequest(userImage, addresses);

			var response = ExecuteGravatarMethod(request);
			
			return response;
		}

		/// <summary>
		/// Remove the userimage associated with one or more email addresses 
		/// </summary>
		/// <param name="addresses"></param>
		/// <returns></returns>
		//public GravatarServiceResponse RemoveImage(string[] addresses)
		//{
		//    throw new NotImplementedException("unclear yet what this method does");
		//}

		/// <summary>
		/// Remove a userimage from the account and any email addresses with which it is associated 
		/// </summary>
		/// <param name="userImage">The user image you wish to remove from the account</param>
		/// <returns>If the method call is successful, the result will be returned using the BooleanResponse property of the
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method</returns>
		public GravatarServiceResponse DeleteUserImage(string userImage)
		{
			var request = GetDeleteImageMethodRequest(userImage);

			var response = ExecuteGravatarMethod(request);
			
			return response;
		}

		#endregion

		#region Get Gravatar Photo URL

		/// <summary>
		/// Gets the currently active Gravatar image URL for the email address used to instantiate the service with
		/// Throws a <see cref="Gravatar.NET.GravatarEmailHashFailedException"/> if the provided email address is invalid
		/// </summary>
		/// <returns>The Gravatar image URL</returns>
		public string GetGravatarUrl()
		{
			return GetGravatarUrlForAddress(Email);
		}

		/// <summary>
		/// Gets the currently active Gravatar image URL for the email address used to instantiate the service with		
		/// Throws a <see cref="Gravatar.NET.GravatarEmailHashFailedException"/> if the provided email address is invalid
		/// </summary>
		/// <param name="pars">The available parameters passed by the request to Gravatar when retrieving the image</param>
		/// <returns>The Gravatar image URL</returns>
		public string GetGravatarUrl(GravatarUrlParameters pars)
		{
			return GetGravatarUrlForAddress(Email, pars);
		}

		/// <summary>
		/// Gets the currently active Gravatar image URL for the email address supplied to this method call
		/// Throws a <see cref="Gravatar.NET.GravatarEmailHashFailedException"/> if the provided email address is invalid
		/// </summary>
		/// <param name="address">The address to retireve the image for</param>
		/// <returns>The Gravatar image URL</returns>
		public static string GetGravatarUrlForAddress(string address)
		{
			return GetGravatarUrlForAddress(address, null);
		}

		/// <summary>
		/// Gets the currently active Gravatar image URL for the email address supplied to this method call
		/// Throws a <see cref="Gravatar.NET.GravatarEmailHashFailedException"/> if the provided email address is invalid
		/// </summary>
		/// <param name="address">The address to retireve the image for</param>
		/// /// <param name="pars">The available parameters passed by the request to Gravatar when retrieving the image</param>
		/// <returns>The Gravatar image URL</returns>
		public static string GetGravatarUrlForAddress(string address, GravatarUrlParameters pars)
		{
			var sbResult = new StringBuilder(GRAVATAR_URL_BASE);

			sbResult.Append(HashEmailAddress(address));

			if (pars != null)
			{
				sbResult.Append("?");

				if (pars.Size > 0) sbResult.AppendFormat(URL_PAR_SIZE, pars.Size);

				if (pars.Rating != GravatarImageRating.G) sbResult.AppendFormat(URL_PAR_RATING, pars.Rating.ToString().ToLower());

				if (pars.DefaultOption != GravatarDefaultUrlOptions.None)
				{
					if (pars.DefaultOption == GravatarDefaultUrlOptions.Custom)
					{
						sbResult.AppendFormat(URL_PAR_DEFAULT, HttpUtility.UrlEncode(pars.CustomDefaultUrl));
					}
					else
					{
						sbResult.AppendFormat(URL_PAR_DEFAULT, 
							(pars.DefaultOption == GravatarDefaultUrlOptions.Error ? URL_PAR_404 : pars.DefaultOption.ToString().ToLower() ));
					}
				}
			}

			return sbResult.ToString();
		}

		#endregion

		#region Private Methods

		private GravatarServiceResponse ExecuteGravatarMethod(GravatarServiceRequest request)
		{
			HashServiceEmail();

			var webRequest = (HttpWebRequest)WebRequest.Create(String.Format(GRAVATAR_API_URL, m_HashedEmail));

			var requestXml = request.ToString();
			LastRequestXml = requestXml;

			var data = Encoding.UTF8.GetBytes(requestXml);

			webRequest.Method = HTTP_POST;
			webRequest.ContentType = XML_TYPE;
			webRequest.ContentLength = data.Length;

			try
			{

				using (var requestStream = webRequest.GetRequestStream())
				{
					requestStream.Write(data, 0, data.Length); // requestXml.Length);
					requestStream.Close();
				}

				var webResponse = (HttpWebResponse)webRequest.GetResponse();

				return new GravatarServiceResponse(webResponse, request.MethodName);
			}
			catch (Exception ex)
			{
				return new GravatarServiceResponse(ex);
			}
		}
									
		private static string HashEmailAddress(string address)
		{
			try
			{
				MD5 md5 = new MD5CryptoServiceProvider();

				var hasedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(address));

				var sb = new StringBuilder();

				for (var i = 0; i < hasedBytes.Length; i++)
				{
					sb.Append(hasedBytes[i].ToString("X2"));
				}

				return sb.ToString().ToLower();
			}
			catch (Exception ex)
			{
				throw new GravatarEmailHashFailedException(address, ex);
			}
		}

		private void HashServiceEmail()
		{
			if (String.IsNullOrEmpty(m_HashedEmail)) m_HashedEmail = HashEmailAddress(Email);			
		}

		#endregion

		#region Create Methods Requests

		private GravatarServiceRequest GetExistsMethodRequest(string[] addresses)
		{
			var pars = new List<GravatarParameter>();

			var hashes = new GravatarArray();
			var hasedAddresses = new GravatarParameterCollection(addresses.Length);

			foreach (var adr in addresses)
			{
				hasedAddresses.Add(GravatarParameter.NewStringParamter(String.Empty, HashEmailAddress(adr)));
			}

			hashes.Parameters = hasedAddresses;

			pars.Add(GravatarParameter.NewStringParamter(PAR_PASSWORD, Password));
			pars.Add(GravatarParameter.NewArrayParameter(PAR_HASHES, hashes));

			return new GravatarServiceRequest { Email = Email, MethodName = GravatarConstants.METHOD_EXISTS, Parameters = pars };
		}

		private GravatarServiceRequest GetTestMethodRequest()
		{
			var pars = new List<GravatarParameter> { GravatarParameter.NewStringParamter(PAR_PASSWORD, Password) };

			return new GravatarServiceRequest { Email = Email, MethodName = GravatarConstants.METHOD_TEST, Parameters = pars };
		}

		private GravatarServiceRequest GetAddressesMethodRequest()
		{
			var pars = new List<GravatarParameter> { GravatarParameter.NewStringParamter(PAR_PASSWORD, Password) };

			return new GravatarServiceRequest { Email = Email, MethodName = GravatarConstants.METHOD_ADDRESSES, Parameters = pars };
		}

		private GravatarServiceRequest GetUserImagesMethodRequest()
		{
			var pars = new List<GravatarParameter> { GravatarParameter.NewStringParamter(PAR_PASSWORD, Password) };

			return new GravatarServiceRequest { Email = Email, MethodName = GravatarConstants.METHOD_USER_IMAGES, Parameters = pars };
		}

		private GravatarServiceRequest GetSaveDataMethodRequest(byte[] data, GravatarImageRating rating)
		{
			var pars = new List<GravatarParameter>
			{
				GravatarParameter.NewStringParamter(PAR_DATA, Convert.ToBase64String(data)),
				GravatarParameter.NewIntegerParameter(PAR_RATING, (int)rating),
				GravatarParameter.NewStringParamter(PAR_PASSWORD, Password)
			};

			return new GravatarServiceRequest { Email = Email, MethodName = GravatarConstants.METHOD_SAVE_DATA, Parameters = pars };
		}

		private GravatarServiceRequest GetSaveUrlMethodRequest(string url, GravatarImageRating rating)
		{
			var pars = new List<GravatarParameter>
			{
				GravatarParameter.NewStringParamter(PAR_URL, url),
				GravatarParameter.NewIntegerParameter(PAR_RATING, (int)rating),
				GravatarParameter.NewStringParamter(PAR_PASSWORD ,Password)
			};

			return new GravatarServiceRequest { Email = Email, MethodName = GravatarConstants.METHOD_SAVE_URL, Parameters = pars };
		}

		private GravatarServiceRequest GetUseUserImagesMethodRequest(string userImage, string[] addresses)
		{
			var adrsArr = new GravatarArray();
			var requestAddresses = new GravatarParameterCollection(addresses.Length);

			foreach (var adr in addresses)
			{
				requestAddresses.Add(GravatarParameter.NewStringParamter(String.Empty, adr));
			}

			adrsArr.Parameters = requestAddresses;

			var pars = new List<GravatarParameter>
			{
				GravatarParameter.NewStringParamter(PAR_USER_IMAGE, userImage),
				GravatarParameter.NewArrayParameter(PAR_ADDRESSES, adrsArr),
				GravatarParameter.NewStringParamter(PAR_PASSWORD, Password)
			};

			return new GravatarServiceRequest { Email = Email, MethodName = GravatarConstants.METHOD_USE_USER_IMAGE, Parameters = pars };
		}

		private GravatarServiceRequest GetDeleteImageMethodRequest(string userImage)
		{
			var pars = new List<GravatarParameter>
			{
				GravatarParameter.NewStringParamter(PAR_USER_IMAGE, userImage),
				GravatarParameter.NewStringParamter(PAR_PASSWORD, Password)
			};

			return new GravatarServiceRequest { Email = Email, MethodName = GravatarConstants.METHOD_DELETE_USER_IMAGE, Parameters = pars };
		}

		#endregion
	}
}
