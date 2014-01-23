using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gravatar.NET.Data
{
	/// <summary>
	/// Represents a XML-RPC parameter
	/// </summary>
	[DataContract]
	public class GravatarParameter
	{
		private string m_StringValue;
		private bool m_BoolValue;
		private int m_IntValue;

		internal GravatarParameter() { }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string StringValue
		{
			get { return m_StringValue; }
			set { m_StringValue = value; }
		}

		[DataMember]
		public int IntegerValue 
		{
			get { return m_IntValue; }
			set
			{
				m_IntValue = value;
				m_StringValue = value.ToString();
			}
		}

		[DataMember]
		public bool BooleanValue 
		{
			get { return m_BoolValue; }
			set
			{
				m_BoolValue = value;
				m_StringValue = value.ToString();
			}
		}

		[DataMember]
		public GravatarArray ArrayValue { get; set; }

		[DataMember]
		public GravatarStruct StructValue { get; set; }

		[DataMember]
		public GravatarParType Type { get; set; }

		#region Static Initializers

		public static GravatarParameter NewStringParamter(string name, string value)
		{
			return new GravatarParameter { Name = name, Type = GravatarParType.String, StringValue = value };
		}

		public static GravatarParameter NewIntegerParameter(string name, int value)
		{
			return new GravatarParameter { Name = name, Type = GravatarParType.Integer, IntegerValue = value };
		}

		public static GravatarParameter NewArrayParameter(string name, GravatarArray value)
		{
			return new GravatarParameter { Name = name, Type = GravatarParType.Array, ArrayValue = value };
		}

		public static GravatarParameter NewStructParamater(string name, GravatarStruct value)
		{
			return new GravatarParameter { Name = name, Type = GravatarParType.Struct, StructValue = value };
		}

		public static GravatarParameter NewBooleanParameter(string name, bool value)
		{
			return new GravatarParameter { Name = name, Type = GravatarParType.Bool, BooleanValue = value };
		}

		#endregion
	}
}
