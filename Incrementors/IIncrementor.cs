using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persisto
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class AutoIncrementAttribute : Attribute
	{
		public Type Type { get; set; }
	}

	public interface IIncrementor
	{
		object Increment(IDbModelInfo modelInfo, IDbMemberInfo memberInfo, object model);
	}
}
