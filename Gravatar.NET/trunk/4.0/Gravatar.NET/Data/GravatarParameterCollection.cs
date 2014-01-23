using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gravatar.NET.Data
{
	/// <summary>
	/// Represents a collection of Gravatar parameters
	/// </summary>
	[DataContract]
	public class GravatarParameterCollection : Dictionary<string, GravatarParameter>
	{
		private const string DUMMY_KEY = "key{0}";

		private int m_Counter;

		internal GravatarParameterCollection(): base()
		{
		}

		internal GravatarParameterCollection(int capacity) : base(capacity)			
		{
		}

		public void Add(GravatarParameter par)
		{			
			Add(par.Name, par);
		}

		public new void Add(string key, GravatarParameter value)
		{
			m_Counter++;

			var useKey = (String.IsNullOrEmpty(key) ? String.Format(DUMMY_KEY, m_Counter) : key);

			base.Add(useKey, value);
		}
	}
}
