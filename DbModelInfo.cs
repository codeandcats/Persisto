using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Persisto
{
	// TODO: Add way of inferring all missing field info by looking it up in
	// supplied connection's schema

	public interface IDbModelInfo
	{
		Type ModelType { get; }

		string GeneratedModelTypeName { get; }

		string GeneratedModelTypeFullName { get; }

		Type GeneratedModelType { get; }

		string TableName { get; }

		string TypeNameFieldName { get; }

		string Filter { get; }

		DbMemberInfo[] Members { get; }

		DbMemberInfo this[string memberName] { get; }

		DbMemberInfo this[int index] { get; }

		DbMemberInfo ID { get; }

		IPersistor Persistor { get; }

		IDbModelInfo BaseModelInfo { get; }

		List<IDbModelInfo> Descendents { get; }
	}

	public interface IDbModelInfo<Model> : IDbModelInfo
	{
		new IPersistor<Model> Persistor { get; }
	}

	public abstract class DbModelInfo : IDbModelInfo
	{
		protected DbModelInfo(Type modelType)
		{
			AllModelInfos.Add(modelType, this);

			ModelType = modelType;

			// Work out the name to use for the Generated Model Type
			string generatedTypeName = "";
			if (ModelType.IsInterface)
			{
				if ((ModelType.Name.Substring(0, 1) == "I") && (char.IsUpper(ModelType.Name, 1)))
				{
					generatedTypeName = ModelType.Name.Substring(1);
				}
			}

			if (generatedTypeName == "")
			{
				generatedTypeName = ModelType.Name;
			}

			GeneratedModelTypeName = generatedTypeName;
			GeneratedModelTypeFullName = "Persisto.Generated.Models." + generatedTypeName;


			var dbModelAttribute = modelType.GetCustomAttribute<DbModelAttribute>();

			if (string.IsNullOrWhiteSpace(dbModelAttribute.TableName))
			{
				dbModelAttribute.TableName = DbExtensions.IdentifierToDbIdentifier(GeneratedModelTypeName);
			}

			TableName = dbModelAttribute.TableName;

			TypeNameFieldName = dbModelAttribute.TypeNameFieldName;

			Filter = dbModelAttribute.Filter;

			SingleReference = dbModelAttribute.SingleReference;

			if (ModelType.IsInterface)
			{
				var members = new List<DbMemberInfo>();

				// If it's an interface, assume all properties are members we want to persist
				foreach (var propertyInfo in ModelType.GetProperties())
				{
					var memberInfo = new DbMemberInfo(propertyInfo);
					members.Add(memberInfo);
				}
				Members = members.ToArray();
			}
			else
			{
				Members =
					ModelType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(member => member.GetCustomAttribute<DbFieldAttribute>() != null)
					.Select(member => new DbMemberInfo(member))
					.ToArray();
			}

			ID = Members.FirstOrDefault(m => m.IsIdentity || m.Name.ToUpper() == "ID");
			
			if (ID == null)
			{
				throw new Exception("Model must contain an identity member");
			}

			// Does this model type actually descend from another model type?
			var modelParentType = modelType.BaseType;
			while (modelParentType != null)
			{
				if (DbModelInfo.IsModelType(modelParentType))
				{
					BaseModelInfo = DbModelInfo.Get(modelParentType);
					BaseModelInfo.Descendents.Add(this);
					break;
				}
				modelParentType = modelParentType.BaseType;
			}
		}

		private static Dictionary<Type, Type> GenericModelInfoTypes = new Dictionary<Type, Type>();
		private static Dictionary<Type, IDbModelInfo> AllModelInfos = new Dictionary<Type, IDbModelInfo>();

		public static bool IsModelType(Type type)
		{
			return (type != null) && (type.GetCustomAttribute<DbModelAttribute>() != null);
		}

		public static IDbModelInfo<Model> Get<Model>()
		{
			var modelType = typeof(Model);
			var modelInfo = DbModelInfo.Get(modelType);
			return (IDbModelInfo<Model>)modelInfo;
		}

		public static IDbModelInfo Get(Type modelType)
		{
			// Make sure we're not using a generated model type - get the ancestor instead!
			while ((modelType != null) && (modelType.Namespace == "Persisto.Generated.Models"))
			{
				modelType = modelType.BaseType;
			}

			IDbModelInfo modelInfo;

			// Okay, first check to see if we've already instantiated a DbModelInfo instance
			// for this model type...
			if (DbModelInfo.AllModelInfos.TryGetValue(modelType, out modelInfo))
			{
				return modelInfo;
			}

			Type genericModelInfoType;

			// Okay, it seems we haven't instantiated a DbModelInfo instance for our model type
			// so we'll need to instantiate it, but first we need a reference to the specific 
			// generic DbModelInfo<> type we're going to create a instance of.
			// Let's check to see if we've constructed a generic type for this model already
			if (!GenericModelInfoTypes.TryGetValue(modelType, out genericModelInfoType))
			{
				// There's no DbModelInfo<> generic type created for this model type yet,
				// so construct this generic type now
				genericModelInfoType = typeof(DbModelInfo<>).MakeGenericType(modelType);

				GenericModelInfoTypes.Add(modelType, genericModelInfoType);
			}

			// Okay, now we're ready to create an instance of our DbModelInfo type
			var constructor = genericModelInfoType.GetConstructor(
				BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
			modelInfo = (IDbModelInfo)constructor.Invoke(new object[0]);

			return modelInfo;
		}

		public static Func<DbConnection> CreateConnectionFunc { get; set; }

		public Type ModelType { get; private set; }

		public string GeneratedModelTypeName { get; private set; }

		public string GeneratedModelTypeFullName { get; private set; }

		public Type GeneratedModelType { get; internal set; }

		public string TableName { get; private set; }

		public string TypeNameFieldName { get; private set; }

		public string Filter { get; private set; }

		public bool SingleReference { get; private set; }

		public DbMemberInfo[] Members { get; private set; }

		public DbMemberInfo this[string memberName]
		{
			get
			{
				var member = Members.FirstOrDefault(m => m.Name == memberName);

				if (member == null)
					throw new ArgumentException("Member not found: " + memberName);

				return member;
			}
		}

		public DbMemberInfo this[int index]
		{
			get { return Members[index]; }
		}

		public DbMemberInfo ID { get; private set; }

		private IPersistor persistor;
		public IPersistor Persistor
		{
			get
			{
				//Console.WriteLine("GET {0}.Persistor = {1}", GetType().Name, persistor == null ? "null" : persistor.GetType().Name);
				return persistor;
			}
			internal set
			{
				//Console.WriteLine("SET {0}.Persistor = {1}", GetType().Name, value == null ? "null" : value.GetType().Name);
				persistor = value;
			}
		}

		public IDbModelInfo BaseModelInfo { get; private set; }

		private List<IDbModelInfo> descendents = new List<IDbModelInfo>();
		public List<IDbModelInfo> Descendents
		{
			get
			{
				return this.descendents;
			}
		}

		#region Initialisation

		public static void Prepare()
		{
			Generator.Generate();
		}

		#endregion
	}

	public class DbModelInfo<Model> : DbModelInfo, IDbModelInfo<Model>
	{
		internal DbModelInfo()
			: base(typeof(Model))
		{
		}

		new public IPersistor<Model> Persistor
		{
			get
			{
				var persistor = (IPersistor<Model>)base.Persistor;
				//Console.WriteLine("GET {0}.Persistor = {1}", GetType().Name, persistor == null ? "null" : persistor.GetType().Name);
				return persistor;
			}
			private set
			{
				//Console.WriteLine("SET {0}.Persistor = {1}", GetType().Name, value == null ? "null" : value.GetType().Name);
				base.Persistor = value;
			}
		}
	}
}