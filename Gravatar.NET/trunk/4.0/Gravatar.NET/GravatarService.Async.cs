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
using System.Threading;

namespace Gravatar.NET
{
	public sealed partial class GravatarService
	{
		private GravatarCallBack m_Callback;
		
		/// <summary>
		/// Call this method before calling any of the xxxxAsync methods to set the method
		/// callback which will be invoked when the asynchronous operation completes
		/// </summary>		
		public GravatarService SetCallBack(GravatarCallBack callback)
		{
			m_Callback = callback;
			
			return this;
		}

		#region Gravatar Async API Methods

		/// <summary>
		/// A test function - called asynchronously
		/// </summary>
		/// <returns>If the method call is successful, the result will be returned using the IntegerResponse property of the 		
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method.
		/// The response is returned to the callback method
		/// </returns>
		public void TestAsync(object state)
		{
			var pars = new List<GravatarParameter> { GravatarParameter.NewStringParamter(PAR_PASSWORD, Password) };

			var request = new GravatarServiceRequest { Email = Email, MethodName = GravatarConstants.METHOD_TEST, Parameters = pars };

			ExecuteGravatarMethodAsync(request, state);
		}

		/// <summary>
		/// Check whether a hash has a gravatar - called asynchronously
		/// </summary>
		/// <returns>If the method call is successful, the result will be returned using the MultipleOperationResponse property
		/// of the <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method
		/// The response is returned to the callback method</returns>
		/// <param name="addresses">an array of hashes to check</param>
		public void ExistsAsync(string[] addresses, object state)
		{
			var request = GetExistsMethodRequest(addresses);

			ExecuteGravatarMethodAsync(request, state);
		}

		/// <summary>
		/// Get a list of addresses for this account - called asynchronously
		/// </summary>
		/// <returns>If the method call is successful, the result will be returned using the AddressesResponse property of the
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method
		/// The response is returned to the callback method</returns>		
		public void AddressesAsync(object state)
		{
			var request = GetAddressesMethodRequest();

			ExecuteGravatarMethodAsync(request, state);
		}

		/// <summary>
		/// Return an array of userimages for this account - called asynchronously
		/// </summary>
		/// <returns>If the method call is successful, the result will be returned using the ImagesResponse property of the
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method
		/// The response is returned to the callback method</returns>
		public void UserImagesAsync(object state)
		{
			var request = GetUserImagesMethodRequest();

			ExecuteGravatarMethodAsync(request, state);
		}

		/// <summary>
		/// Save binary image data as a userimage for this account - called asynchronously
		/// </summary>
		/// <param name="data">The image data in bytes</param>
		/// <param name="rating">The rating of the image (g, pg, r, x)</param>
		/// <returns>If the method call is successful, the result will be returned using the SaveResponse property of the
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method
		/// The response is returned to the callback method</returns>
		public void SaveDataAsync(byte[] data, GravatarImageRating rating, object state)
		{
			var request = GetSaveDataMethodRequest(data, rating);

			ExecuteGravatarMethodAsync(request, state);
		}

		/// <summary>
		/// Read an image via its URL and save that as a userimage for this account - called asynchronously 
		/// </summary>
		/// <param name="url">a full url to an image</param>
		/// <param name="rating">The rating of the image (g, pg, r, x)</param>
		/// <returns>If the method call is successful, the result will be returned using the SaveResponse property of the
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method
		/// The response is returned to the callback method</returns>
		public void SaveUrlAsync(string url, GravatarImageRating rating, object state)
		{
			var request = GetSaveUrlMethodRequest(url, rating);

			ExecuteGravatarMethodAsync(request, state);
		}

		/// <summary>
		/// Use a userimage as a gravatar for one of more addresses on this account - called asynchronously 
		/// </summary>
		/// <param name="imageId">The userimage you wish to use</param>
		/// <param name="addresses">A list of the email addresses you wish to use this userimage for</param>
		/// <returns>If the method call is successful, the result will be returned using the MultipleOperationResponse property
		/// of the <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method
		/// The response is returned to the callback method</returns>
		public void UseUserImageAsync(string userImage, string[] addresses, object state)
		{
			var request = GetUseUserImagesMethodRequest(userImage, addresses);

			ExecuteGravatarMethodAsync(request, state);
		}

		/// <summary>
		/// Remove a userimage from the account and any email addresses with which it is associated - called asynchronously 
		/// </summary>
		/// <param name="userImage">The user image you wish to remove from the account</param>
		/// <returns>If the method call is successful, the result will be returned using the BooleanResponse property of the
		/// <see cref="Gravatar.NET.GravatarServiceResponse"/> instance returned by this method
		/// The response is returned to the callback method</returns>
		public void DeleteUserImageAsync(string userImage, object state)
		{
			var request = GetDeleteImageMethodRequest(userImage);

			ExecuteGravatarMethodAsync(request, state);
		}

		#endregion

		#region Private Methods

		private void ExecuteGravatarMethodAsync(GravatarServiceRequest request, object state)
		{
			HashServiceEmail();

			var webRequest = (HttpWebRequest)WebRequest.Create(String.Format(GRAVATAR_API_URL, m_HashedEmail));

			webRequest.Method = HTTP_POST;
			webRequest.ContentType = XML_TYPE;
						
			webRequest.BeginGetRequestStream(OnGetRequestStream, 
											new GravatarRequestState { 
												WebRequest = webRequest, 
												GravatarRequest = request,
												UserState = state,
												CallBack = this.m_Callback
											});			
		}

		private void OnGetRequestStream(IAsyncResult ar)
		{
			var requestState = (GravatarRequestState)ar.AsyncState;

			try
			{
				var requestXml = requestState.GravatarRequest.ToString();

				using (Stream requestStream = requestState.WebRequest.EndGetRequestStream(ar))
				{
					var data = Encoding.UTF8.GetBytes(requestXml);

					requestStream.Write(data, 0, requestXml.Length);
					requestStream.Close();
				}

				requestState.WebRequest.BeginGetResponse(OnGetResponse, requestState);
			}
			catch (Exception ex)
			{
				var response = new GravatarServiceResponse(ex);

				requestState.CallBack(response, requestState.UserState);
			}
		}

		private void OnGetResponse(IAsyncResult ar)
		{
			GravatarServiceResponse gravatarResponse = null;

			try
			{
				var requestState = (GravatarRequestState)ar.AsyncState;

				var webResponse = (HttpWebResponse)requestState.WebRequest.EndGetResponse(ar);

				gravatarResponse = new GravatarServiceResponse(webResponse, requestState.GravatarRequest.MethodName);

				requestState.CallBack(gravatarResponse, requestState.UserState);
			}
			catch (Exception ex)
			{
				gravatarResponse = new GravatarServiceResponse(ex);
			}
		}

		#endregion

		private class GravatarRequestState
		{
			public HttpWebRequest WebRequest { get; set; }
			public GravatarServiceRequest GravatarRequest { get; set; }
			public object UserState { get; set; }
			public GravatarCallBack CallBack { get; set; }
		}
	}
}
