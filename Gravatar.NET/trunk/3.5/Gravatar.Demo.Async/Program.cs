using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Gravatar.NET;
using Gravatar.NET.Data;

namespace Gravatar.Demo.Async
{
	class Program
	{
		static string m_Email = ""; //set Gravatar account ID
		static string m_Password = ""; //set Gravatar account password

		static ManualResetEvent m_Reset = new ManualResetEvent(false);

		static void Main(string[] args)
		{
			//TestMethod(); //uncomment to test
			//AddressesMethod(); //uncomment to test
			//SaveUrlMethod("http://technmarketing.com/wp-content/uploads/2009/03/2141638.jpg"); //uncomment to test (uploads a tweetie photo)
			//DeleteMethod("<set image id>"); //uncomment to test (image id must match an existing photo for this account)

			Console.WriteLine("Waiting for async method..");
			m_Reset.WaitOne(); //wait on main thread

			Console.WriteLine("Press Enter to exit");
			Console.ReadLine();
		}

		static void TestMethod()
		{
			Console.WriteLine("About to execute the Gravatar Test Method");

			var service = new GravatarService(m_Email, m_Password);

			service.SetCallBack((res, state) =>
			{
				if (!res.IsError)
				{
					Console.WriteLine("Method Response: '{0}'", res.IntegerResponse);
				}
				else
				{
					Console.WriteLine("An Error occurred while executing method: ({0}){1}", res.ErrorCode, res.ErrorInfo);
				}

				m_Reset.Set(); //continue main thread

			}).TestAsync(null);
		}

		static void AddressesMethod()
		{
			Console.WriteLine("About to execute the Gravatar Addresses Method");

			var service = new GravatarService(m_Email, m_Password);

			service.SetCallBack((res, state) =>
			{
				if (!res.IsError)
				{
					foreach (var adr in res.AddressesResponse)
					{
						Console.WriteLine("Address: {0} - {1}", adr.Name, adr.Image.Url);
					}
				}
				else
				{
					Console.WriteLine("An Error occurred while executing method: ({0}){1}", res.ErrorCode, res.ErrorInfo);
				}

				m_Reset.Set(); //continue main thread				

			}).AddressesAsync(null);
		}

		static void SaveUrlMethod(string url)
		{
			Console.WriteLine("About to execute the Gravatar SaveUrl Method");

			var service = new GravatarService(m_Email, m_Password);

			service.SetCallBack((res, state) =>
			{
				if (!res.IsError)
				{
					Console.WriteLine("Save Success: '{0}', new image url: '{1}'", res.SaveResponse.Success, res.SaveResponse.SavedImageId);
				}
				else
				{
					Console.WriteLine("An Error occurred while executing method: ({0}){1}", res.ErrorCode, res.ErrorInfo);
				}

				m_Reset.Set(); //continue main thread				

			}).SaveUrlAsync(url, GravatarImageRating.G, null);
		}

		static void DeleteMethod(string imageId)
		{
			Console.WriteLine("About to execute the Gravatar DeleteUserImage Method");

			var service = new GravatarService(m_Email, m_Password);

			service.SetCallBack((res, state) =>
			{
				if (!res.IsError)
				{
					Console.WriteLine("Delete image successful: {0}", res.BooleanResponse);
				}
				else
				{
					Console.WriteLine("An Error occurred while executing method: ({0}){1}", res.ErrorCode, res.ErrorInfo);
				}

				m_Reset.Set(); //continue main thread				

			}).DeleteUserImageAsync(imageId, null);
		}
	}
}
