using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Persisto
{
	public interface IDbMemberInfo
	{
		string Name { get; }

		Type MemberType { get; }

		bool IsVirtual { get; }

		bool IsAbstract { get; }

		DbType DataType { get; }

		bool IsIdentity { get; }

		DbRelation Relation { get; }

		IIncrementor Incrementor { get; }

		object GetValue(object model);
		void SetValue(object model, object value);
	}

	public class DbMemberInfo : IDbMemberInfo
	{
		public DbMemberInfo(MemberInfo memberInfo)
		{
			this.info = memberInfo;

			if ((memberInfo.MemberType != MemberTypes.Field) && (memberInfo.MemberType != MemberTypes.Property))
			{
				throw new Exception("MemberType not supported: " + memberInfo.MemberType.ToString());
			}

			fieldInfo = memberInfo as FieldInfo;
			propertyInfo = memberInfo as PropertyInfo;

			MemberType =
				(memberInfo.MemberType == MemberTypes.Field) ?
				(memberInfo as FieldInfo).FieldType :
				(memberInfo as PropertyInfo).PropertyType;

			MethodInfo getMethod;
			MethodInfo setMethod;

			if (propertyInfo != null)
			{
				getMethod = propertyInfo.GetGetMethod();
				setMethod = propertyInfo.GetSetMethod();

				IsVirtual = getMethod.IsVirtual || setMethod.IsVirtual;
				IsAbstract = getMethod.IsAbstract || setMethod.IsAbstract;
			}
			else
			{
				IsVirtual = false;
				IsAbstract = false;
			}

			var dbFieldAttribute = memberInfo.GetCustomAttribute<DbFieldAttribute>();
			if (dbFieldAttribute == null)
			{
				dbFieldAttribute = new DbFieldAttribute();
			}

			Relation = dbFieldAttribute as DbRelation;

			if (Relation != null)
			{
				Relation.LoadOnDemand = IsVirtual;
			}

			IsBackedByField = dbFieldAttribute.IsBackedByField;

			var manyToOne = Relation as DbRelation.ManyToOneAttribute;
			var oneToMany = Relation as DbRelation.OneToManyAttribute;

			if (Relation == null)
			{
				// If the Field DataType hasn't been set then guess it based on the member's Type
				if (IsBackedByField && dbFieldAttribute.DataType == DbType.Object)
				{
					if (MemberType.IsEnum)
					{
						throw new Exception("Cannot infer database field type from Enum type");
					}
					else
					{
						dbFieldAttribute.DataType = DbExtensions.DbTypeFromType(MemberType);
					}
				}
			}
			else
			{
				// If the ForeignType hasn't been set then infer it
				if (Relation.ForeignType == null)
				{
					if (manyToOne != null)
					{
						// Many to One is easy to work out, it's simply the member's type
						Relation.ForeignType = MemberType;
					}
					else
					{
						// For OneToMany & ManyToMany relationships the member's type should be some sort of
						// IEnumerable<ForeignType>. So we need to work out the generic argument type 
						// to get the ForeignType
						Type genericArgumentType = MemberType.GetGenericArguments().FirstOrDefault();

						if (genericArgumentType != null)
						{
							Type enumerableInterfaceType = typeof(IEnumerable<>);
							enumerableInterfaceType = enumerableInterfaceType.MakeGenericType(genericArgumentType);

							if (enumerableInterfaceType.IsAssignableFrom(MemberType))
							{
								Relation.ForeignType = genericArgumentType;
							}
						}
					}
				}
			}

			IDbModelInfo foreignModelInfo = Relation == null ? null : DbModelInfo.Get(Relation.ForeignType);

			// If the FieldName hasn't been set then guess it based on the member's name
			if (string.IsNullOrEmpty(dbFieldAttribute.FieldName))
			{
				if (manyToOne != null)
				{
					dbFieldAttribute.FieldName = foreignModelInfo.TableName + "_" + foreignModelInfo.ID.FieldName;
				}
				else
				{
					dbFieldAttribute.FieldName = DbExtensions.IdentifierToDbIdentifier(memberInfo.Name);
				}
			}

			FieldName = dbFieldAttribute.FieldName;

			DataType = dbFieldAttribute.DataType;

			IsIdentity = dbFieldAttribute.IsIdentity;

			var autoIncrement = memberInfo.GetCustomAttribute<AutoIncrementAttribute>();

			if (autoIncrement != null)
			{
				var autoIncrementType = autoIncrement.Type;
				if (autoIncrementType == null)
				{
					// Guess the increment type
					if (MemberType == typeof(Guid))
					{
						autoIncrementType = typeof(GuidIncrementor); 
					}
					else
					{
						throw new Exception("Could not infer incrementor type for " + 
							memberInfo.DeclaringType.FullName);
					}
				}

				Incrementor = (IIncrementor)Activator.CreateInstance(autoIncrementType);
			}
		}

		private MemberInfo info;
		private FieldInfo fieldInfo;
		private PropertyInfo propertyInfo;

		public string Name { get { return info.Name; } }

		public Type MemberType { get; private set; }

		public bool IsVirtual { get; private set; }

		public bool IsAbstract { get; private set; }

		public bool IsBackedByField { get; private set; }

		public string FieldName { get; private set; }

		public DbType DataType { get; private set; }

		public DbRelation Relation { get; private set; }

		public bool IsIdentity { get; private set; }

		public IIncrementor Incrementor { get; private set; }

		public object GetValue(object model)
		{
			if (info.MemberType == MemberTypes.Field)
				return fieldInfo.GetValue(model);
			else
				return propertyInfo.GetValue(model);
		}

		public void SetValue(object model, object value)
		{
			if (info.MemberType == MemberTypes.Field)
				fieldInfo.SetValue(model, value);
			else
				propertyInfo.SetValue(model, value);
		}
	}
}
