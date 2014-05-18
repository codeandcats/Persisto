using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persisto
{
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Interface,
		AllowMultiple = false)]
	public class DbModelAttribute : Attribute
	{
		public string TableName { get; set; }

		public bool SingleReference { get; set; }

		public string TypeNameFieldName { get; set; }

		public string Filter { get; set; }
	}
}
