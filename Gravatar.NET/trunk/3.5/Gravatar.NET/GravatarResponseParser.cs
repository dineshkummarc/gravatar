using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gravatar.NET.Data;
using Gravatar.NET.Exceptions;

namespace Gravatar.NET
{
	/// <summary>
	/// Used to centerally populte the appropriate response properties of the <see cref="Gravatar.NET.GravatarServiceResponse"/>
	/// instance that is returned after an API method call.
	/// </summary>
	internal class GravatarResponseParser
	{
		private string m_Method;

		internal GravatarResponseParser(string method)
		{
			m_Method = method;
		}

		internal void ParseResponseForMethod(GravatarServiceResponse response)
		{
			switch (m_Method)
			{
				case GravatarConstants.METHOD_TEST:
					SetTestMethodResponse(response);
					break;
				
				case GravatarConstants.METHOD_EXISTS:
					SetExistsMethodResponse(response);
					break;

				case GravatarConstants.METHOD_ADDRESSES:
					SetAddressesMethodResponse(response);
					break;
				case GravatarConstants.METHOD_USE_USER_IMAGE:
					SetUseUserImageMethodResponse(response);
					break;
				case GravatarConstants.METHOD_USER_IMAGES:
					SetUserImagesMethodResponse(response);
					break;
				case GravatarConstants.METHOD_SAVE_DATA:
				case GravatarConstants.METHOD_SAVE_URL:
					SetSaveMethodResponse(response);
					break;
				case GravatarConstants.METHOD_DELETE_USER_IMAGE:
					SetDeleteImageMethodResponse(response);
					break;
				default:
					throw new UnknownGravatarMethodException(m_Method);
			}			
		}

		private void SetAddressesMethodResponse(GravatarServiceResponse response)
		{
			if (response != null)
			{
				if (!response.IsError)
				{
					var addresses = new List<GravatarAddress>(response.ResponseParameters.Count());

					foreach (var addr in response.ResponseParameters)
					{
						var addrPar = addr.Value;

						addresses.Add(new GravatarAddress
						{
							Name = addrPar.Name,
							ImageId = addrPar.StructValue.Parameters["userimage"].StringValue,
							Image = new GravatarUserImage
							{
								Rating = (GravatarImageRating)addrPar.StructValue.Parameters["rating"].IntegerValue,
								Url = addrPar.StructValue.Parameters["userimage_url"].StringValue
							}
						});
					}

					response.AddressesResponse = addresses;
				}
			}
		}

		private void SetDeleteImageMethodResponse(GravatarServiceResponse response)
		{
			if (response != null)
			{
				if (!response.IsError)
				{
					var responsePar = response.ResponseParameters.First().Value;

					if (responsePar.Type == GravatarParType.Bool) response.BooleanResponse = responsePar.BooleanValue;
				}
			}
		}

		private void SetSaveMethodResponse(GravatarServiceResponse response)
		{
			if (response != null)
			{
				if (!response.IsError)
				{
					var responsePar = response.ResponseParameters.First().Value;
					var saveResponse = new GravatarSaveResponse();

					if (responsePar.Type == GravatarParType.String)
					{
						saveResponse.Success = true;
						saveResponse.SavedImageId = responsePar.StringValue;
					}

					response.SaveResponse = saveResponse;
				}
			}
		}

		private void SetUserImagesMethodResponse(GravatarServiceResponse response)
		{
			if (response != null)
			{
				if (!response.IsError)
				{
					var images = new List<GravatarUserImage>();

					foreach (var par in response.ResponseParameters)
					{
						if (par.Value.Type == GravatarParType.Array)
						{
							var arrPars = par.Value.ArrayValue.Parameters;

							var rating = int.Parse(arrPars.First().Value.StringValue);

							images.Add(new GravatarUserImage { Name = par.Value.Name, Rating = (GravatarImageRating)rating, Url = arrPars.Last().Value.StringValue });
						}
					}

					response.ImagesResponse = images;
				}
			}
		}

		private void SetUseUserImageMethodResponse(GravatarServiceResponse response)
		{
			if (response != null)
			{
				if (!response.IsError)
				{
					var parsArr = response.ResponseParameters.ToArray();

					var changed = new bool[parsArr.Length];

					for (var i = 0; i < changed.Length; i++)
					{
						changed[i] = parsArr[i].Value.BooleanValue;
					}

					response.MultipleOperationResponse = changed;
				}
			}
		}

		private void SetTestMethodResponse(GravatarServiceResponse response)
		{
			if (response != null)
			{
				if (!response.IsError)
				{
					var responsePar = response.ResponseParameters.Last().Value;

					if (responsePar.Type == GravatarParType.Integer) response.IntegerResponse = responsePar.IntegerValue;
				}
			}
		}

		private void SetExistsMethodResponse(GravatarServiceResponse response)
		{
			if (response != null)
			{
				if (!response.IsError)
				{
					var parsArr = response.ResponseParameters.ToArray();

					var exists = new bool[parsArr.Length];

					for (var i = 0; i < exists.Length; i++)
					{
						exists[i] = Convert.ToBoolean(parsArr[i].Value.IntegerValue);
					}

					response.MultipleOperationResponse = exists;
				}
			}
		}
	}
}
