using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persisto
{
	public static class StringHelper
	{
		public static string ToCamelCase(this string s)
		{
			return s.Substring(0, 1).ToLower() + s.Substring(1);
		}
	}
}
