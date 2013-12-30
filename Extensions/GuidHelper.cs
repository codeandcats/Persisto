using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persisto
{
	public static class GuidHelper
	{
		public static string ToString(this Guid guid, bool nullIfZero)
		{
			if ((guid == default(Guid)) && nullIfZero)
			{
				return null;
			}
			else
			{
				return guid.ToString();
			}
		}
	}
}
