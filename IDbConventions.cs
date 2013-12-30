using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persisto
{
	// TODO: Implement IoC and use conventions
	public interface IDbConventions
	{
		string GetTableName(IDbModelInfo modelInfo);

		DbMemberInfo GetIdentityField(IDbModelInfo modelInfo);

		//GetFieldValues//...
	}
}
