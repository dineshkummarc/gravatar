using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gravatar.NET
{
	/// <summary>
	/// References a method to call when a Asynchronous Gravatar API method call completes
	/// </summary>	
	public delegate void GravatarCallBack(GravatarServiceResponse response, object state);
}
