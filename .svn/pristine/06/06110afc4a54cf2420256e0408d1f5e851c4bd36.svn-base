using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gravatar.NET.Data
{
	/// <summary>
	/// Represents a Gravatar account-address parameter
	/// </summary>
	[DataContract]
	public class GravatarAddress
	{
		internal GravatarAddress() { }

		/// <summary>
		/// The email address
		/// </summary>
		[DataMember]
		public string Name { get; internal set; }

		/// <summary>
		/// The ID if the active image associated with this account
		/// </summary>
		[DataMember]
		public string ImageId { get; internal set; }

		/// <summary>
		/// The details of the active image associated with this account
		/// </summary>
		[DataMember]
		public GravatarUserImage Image {get;internal set;}
	}
}
