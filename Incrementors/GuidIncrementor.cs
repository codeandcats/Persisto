using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persisto
{
	public class GuidIncrementor : IIncrementor
	{
		public object Increment(IDbModelInfo modelInfo, IDbMemberInfo memberInfo, object model)
		{
			return Guid.NewGuid();
		}
	}
}
