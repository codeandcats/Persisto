using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persisto
{
	public abstract class DbRelation : DbFieldAttribute
	{
		public Type ForeignType { get; set; }

		internal protected override bool IsBackedByField { get { return false; } }

		public bool LoadOnDemand { get; internal set; }

		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
		public class OneToManyAttribute : DbRelation
		{
			//public string ForeignKey { get; set; }
		}

		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
		public class ManyToOneAttribute : DbRelation
		{
			internal protected override bool IsBackedByField { get { return true; } }
		}

		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
		public class ManyToManyAttribute : DbRelation
		{
			public string TableName { get; set; }
			public string Key { get; set; }
			public string ForeignKey1 { get; set; }
			public string ForeignKey2 { get; set; }
		}
	}
}
