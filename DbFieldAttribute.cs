using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Persisto
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class DbFieldAttribute : Attribute
	{
		public DbFieldAttribute(
			string fieldName = "",
			DbType dataType = DbType.Object)
		{
			FieldName = fieldName;
			DataType = dataType;
		}

		public string FieldName { get; set; }

		public DbType DataType { get; set; }

		public bool IsIdentity { get; set; }

		internal protected virtual bool IsBackedByField { get { return true; } }
	}
}