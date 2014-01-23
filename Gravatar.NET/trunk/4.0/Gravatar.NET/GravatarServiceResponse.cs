using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Gravatar.NET.Data;
using Gravatar.NET.Exceptions;

namespace Gravatar.NET
{
	/// <summary>
	/// Represents the response returned from Gravatar after a request has been made.
	/// The object has different properties representing the different types of results which can
	/// be returned by a method call.
	/// </summary>
	public class GravatarServiceResponse
	{
		/// <summary>
		/// Create a new Gravatar response for the method that was called.
		/// Throws a <see cref="Gravatar.Net.GravatarInvalidResponseXmlException"/> if the XML returned from Gravatar
		/// is in an unexpected format
		/// </summary>		
		internal GravatarServiceResponse(HttpWebResponse webResponse, string methodName)
		{						
			using (var responseStream = webResponse.GetResponseStream())
			{
				using (var reader = new StreamReader(responseStream))
				{
					GravatarResponseXml = reader.ReadToEnd();

					InitializeGravatarResponse(GravatarResponseXml, methodName);
				}
			}		
		}

		/// <summary>
		/// Create a new Gravatar response with the exception information that occurred
		/// while making a request
		/// </summary>
		/// <param name="ex"></param>
		internal GravatarServiceResponse(Exception ex)
		{
			IsError = true;
			ErrorCode = GravatarConstants.CLIENT_ERROR;
			ErrorInfo = ex.Message;
		}

		#region Error Info

		/// <summary>
		/// Indication whether the method call resulted in an Error or not
		/// </summary>
		public bool IsError { get; private set; }

		/// <summary>
		/// The Gravatar error code
		/// </summary>
		public int ErrorCode { get; private set; }

		/// <summary>
		/// Additional information about the Gravatar error
		/// </summary>
		public string ErrorInfo { get; private set; }

		#endregion

		#region Properties

		/// <summary>
		/// The raw XML returned from Gravatar as a result of a method call
		/// </summary>
		public string GravatarResponseXml { get; private set; }
		
		/// <summary>
		/// The list of all parameters returned from the method call
		/// </summary>
		public GravatarParameterCollection ResponseParameters { get; private set; }
		#endregion

		#region Method Responses

		/// <summary>
		/// Used when the result of a method call is of an integer type
		/// </summary>
		public int IntegerResponse { get; internal set; }

		/// <summary>
		/// Used when the result of a method call is of a Boolean type
		/// </summary>
		public bool BooleanResponse { get; internal set; }

		/// <summary>
		/// Used when the result of a method call is a list of user images
		/// </summary>
		public IEnumerable<GravatarUserImage> ImagesResponse { get; internal set; }

		/// <summary>
		/// Used when the result of a method is a Boolean indication for multiple operations such as updating a list
		/// of email addresses with an image
		/// </summary>
		public bool[] MultipleOperationResponse { get; internal set; }

		/// <summary>
		/// Used when the result of a method call is a list of email addresses
		/// </summary>
		public IEnumerable<GravatarAddress> AddressesResponse { get; internal set; }

		/// <summary>
		/// Used when the operation is an image save operation
		/// </summary>
		public GravatarSaveResponse SaveResponse { get; internal set; }

		#endregion

		#region Private Methods

		private void InitializeGravatarResponse(string response, string methodName)
		{
			var responseXmlDoc = XDocument.Parse(response);

			var rspElement = responseXmlDoc.Element(GravatarConstants.XML_METHOD_RESPONSE);

			if (rspElement != null)
			{
				var fault = rspElement.Element(GravatarConstants.XML_FAULT);

				XElement firstStruct = GetFirstResponseStructElement(responseXmlDoc);

				GravatarParameterCollection pars = null;

				if (firstStruct != null)
				{
					pars = GetGravatarXmlMembers(firstStruct);
				}
				else
				{
					pars = GetGravatarXmlParameters(responseXmlDoc);	
				}

				if (fault == null) //request was accepted, no error sent back
				{
					ResponseParameters = pars;
				}
				else
				{
					SetErrorInfo(pars.First().Value.IntegerValue, pars.Last().Value.StringValue);
				}

				var parser = new GravatarResponseParser(methodName);

				parser.ParseResponseForMethod(this);//set the proper response based on the method that was called
			}
			else
				throw new GravatarInvalidResponseXmlException();
		}

		private void SetErrorInfo(int errorCode, string errorInfo)
		{
			this.IsError = true;
			this.ErrorCode = errorCode;
			this.ErrorInfo = errorInfo;
		}

		private XElement GetFirstResponseStructElement(XDocument responseDoc)
		{
			try
			{
				var q = from s in responseDoc.Descendants(GravatarConstants.XML_STRUCT)
						select s;

				return q.FirstOrDefault();
			}
			catch (Exception)
			{
				throw new GravatarInvalidResponseXmlException();
			}
		}

		private GravatarParameterCollection GetGravatarXmlParameters(XDocument responseDoc)
		{
			var pars = responseDoc.Descendants(GravatarConstants.XML_PARAM);

			var parsColl = new GravatarParameterCollection(pars.Count());
			
			foreach (var p in pars)
			{
				parsColl.Add(GetParameterFromXml(p));
			}

			return parsColl;
		}

		private GravatarParameterCollection GetGravatarXmlMembers(XElement structElement)
		{
			var members = structElement.Elements(GravatarConstants.XML_MEMBER);

			var parsColl = new GravatarParameterCollection(members.Count());

			foreach (var m in members)
			{
				parsColl.Add(GetParameterFromXml(m));
			}

			return parsColl;
		}

		private GravatarParameter GetParameterFromXml(XElement memberXml)
		{
			var par = new GravatarParameter();

			var nameElm = memberXml.Element(GravatarConstants.XML_NAME);

			if (nameElm != null) par.Name = nameElm.Value;

			PopulateParFromXmlValue(par, memberXml.Element(GravatarConstants.XML_VALUE));

			return par;
		}

		private void PopulateParFromXmlValue(GravatarParameter par, XElement valNode)
		{
			var valType = valNode.Elements().First();

			if (valType.Name == GravatarConstants.XML_STRUCT)
			{
				par.Type = GravatarParType.Struct;
				par.StructValue = GetGravatarStructFromXml(valType);
			}
			else if (valType.Name == GravatarConstants.XML_ARRAY)
			{
				par.Type = GravatarParType.Array;
				par.ArrayValue = GetGravatarArrayFromXml(valType);
			}
			else if (valType.Name == GravatarConstants.XML_INT)
			{
				par.Type = GravatarParType.Integer;
				par.IntegerValue = int.Parse(valType.Value);
			}
			else if (valType.Name == GravatarConstants.XML_STRING)
			{
				par.Type = GravatarParType.String;
				par.StringValue = valType.Value;
			}
			else if (valType.Name == GravatarConstants.XML_BOOL || valType.Name == GravatarConstants.XML_BOOLEAN)
			{
				par.Type = GravatarParType.Bool;

				var boolVal = valType.Value;

				if (boolVal == "1")
				{
					par.BooleanValue = true;
				}
				else if (boolVal == "0")
				{
					par.BooleanValue = false;
				}
				else
					par.BooleanValue = bool.Parse(valType.Value);
			}
		}

		private GravatarStruct GetGravatarStructFromXml(XElement valType)
		{
			return new GravatarStruct { Parameters = GetGravatarXmlMembers(valType) };
		}

		private GravatarArray GetGravatarArrayFromXml(XElement valType)
		{
			var arrValues = valType.Descendants(GravatarConstants.XML_VALUE);

			var arrPars = new GravatarParameterCollection(arrValues.Count());

			foreach (var av in arrValues)
			{
				var par = new GravatarParameter();

				PopulateParFromXmlValue(par, av);

				arrPars.Add(par);
			}

			return new GravatarArray { Parameters = arrPars };
		}		

		#endregion
	}
}
