using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Gravatar.NET.Data;

namespace Gravatar.NET
{
	/// <summary>
	/// Represents a single request to the Gravatar server, encapsulating a Gravatar method
	/// </summary>
	public class GravatarServiceRequest
	{
		internal GravatarServiceRequest()
		{
		}
		
		#region Properties

		private List<GravatarParameter> m_Parameters;

		public string Email { get; internal set; }

		/// <summary>
		/// The Gravatar method called in the context of this request
		/// </summary>
		public string MethodName { get; internal set; }

		/// <summary>
		/// The list of parameters sent to Gravatar in this method call
		/// </summary>
		public List<GravatarParameter> Parameters
		{
			get
			{
				if (m_Parameters == null) m_Parameters = new List<GravatarParameter>();

				return m_Parameters;
			}

			internal set { m_Parameters = value; }
		}
		
		#endregion
	
		private string CreateGravatarRequestXml()
		{
			var sb = new StringBuilder();

			using (var sw = new StringWriter(sb))
			{
				using (var xw = new XmlTextWriter(sw))
				{
					xw.WriteProcessingInstruction("xml", "version='1.0'");

					xw.WriteStartElement("methodCall");
					xw.WriteElementString("methodName", MethodName);
					xw.WriteStartElement("params");

					xw.WriteStartElement(GravatarConstants.XML_PARAM);
					xw.WriteStartElement(GravatarConstants.XML_VALUE);
					xw.WriteStartElement(GravatarConstants.XML_STRUCT);

					foreach (var p in Parameters)
					{
						WriteGravatarRequestParam(xw, p);
					}

					xw.WriteEndElement(); //struct
					xw.WriteEndElement(); //value
					xw.WriteEndElement();//param

					xw.WriteEndElement(); //params					
					xw.WriteEndElement(); //methodCall					
				}
			}

			return sb.ToString();
		}

		private void WriteGravatarRequestParam(XmlTextWriter writer, GravatarParameter par)
		{
			writer.WriteStartElement(GravatarConstants.XML_MEMBER);

			writer.WriteElementString(GravatarConstants.XML_NAME, par.Name);
			
			if (par.Type == GravatarParType.Array)
			{
				writer.WriteStartElement(GravatarConstants.XML_VALUE);
				writer.WriteStartElement(GravatarConstants.XML_ARRAY);
				writer.WriteStartElement(GravatarConstants.XML_DATA);

				foreach (var arrPar in par.ArrayValue.Parameters)
				{
					writer.WriteStartElement(GravatarConstants.XML_VALUE);
					
					var requestPar = arrPar.Value;

					switch (requestPar.Type)
					{
						case GravatarParType.Bool:
							writer.WriteElementString(GravatarConstants.XML_BOOL, requestPar.StringValue);
							break;
						case GravatarParType.Integer:
							writer.WriteElementString(GravatarConstants.XML_INT, requestPar.StringValue);
							break;
						default:
							writer.WriteElementString(GravatarConstants.XML_STRING, requestPar.StringValue);
							break;
					}
					
					writer.WriteEndElement();//value
				}

				writer.WriteEndElement(); //data
				writer.WriteEndElement(); //array
				writer.WriteEndElement();//value
			}
			else
			{
				writer.WriteElementString(GravatarConstants.XML_VALUE, par.StringValue);
			}

			writer.WriteEndElement(); //member			
		}

		/// <summary>
		/// returns the XML structure for the method call
		/// </summary>		
		public override string ToString()
		{
			return CreateGravatarRequestXml();
		}
	}
}
