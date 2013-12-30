using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Common;

namespace Persisto
{
	public struct LoadOptions
	{
		public string Joins;
		public string Where;
		public object[] ParamValues;
		public string OrderBy;
		public bool FetchAll; // TODO: Implement FetchAll switch
		public Func<DbConnection> CreateConnectionFunc;

		public string[] GetParamNames()
		{
			var paramNames = new List<string>();

			if (!string.IsNullOrWhiteSpace(Joins))
			{
				paramNames.AddRange(DbExtensions.ExtractParamNamesFromSQL(Joins));
			}

			if (!string.IsNullOrWhiteSpace(Where))
			{
				paramNames.AddRange(DbExtensions.ExtractParamNamesFromSQL(Where));
			}

			return paramNames.ToArray();
		}

		#region Static Members

		private static Dictionary<string, FieldInfo> loadOptionsFields;
		
		public static LoadOptions From(object o)
		{
			if (o is LoadOptions)
			{
				return (LoadOptions)o;
			}

			if (loadOptionsFields == null)
			{
				loadOptionsFields = new Dictionary<string, FieldInfo>();
				foreach (var field in typeof(LoadOptions).GetFields())
				{
					loadOptionsFields[field.Name.ToLower()] = field;
				}
			}

			if (o == null)
			{
				return default(LoadOptions);
			}

			var options = new LoadOptions();

			var dynamicType = o.GetType();

			var paramsProperty = dynamicType.GetProperty("Params");
			if (paramsProperty != null)
			{
				dynamic paramsValue = paramsProperty.GetValue(o);

				Type paramsType = paramsValue.GetType();

				var where = new StringBuilder();
				var added = false;

				var paramValues = new List<object>();

				foreach (var dynamicProperty in paramsType.GetProperties())
				{
					if (added)
					{
						where.Append(" AND ");
					}

					where.Append(string.Format("({0} = @{0})", dynamicProperty.Name));

					paramValues.Add(dynamicProperty.GetValue(paramsValue));

					added = true;
				}

				options.Where = where.ToString();
				options.ParamValues = paramValues.ToArray();
			}

			object boxedOptions = (options);

			foreach (var dynamicProperty in dynamicType.GetProperties())
			{
				FieldInfo loadOptionsField;
				if (loadOptionsFields.TryGetValue(dynamicProperty.Name.ToLower(), out loadOptionsField))
				{
					object value = dynamicProperty.GetValue(o);
					loadOptionsField.SetValue(boxedOptions, value);
				}
			}

			options = (LoadOptions)boxedOptions;

			return options;
		}

		#endregion
	}

    public interface IPersistor
	{
		IDbModelInfo ModelInfo { get; }

		IEnumerable LoadModels(DbConnection db, LoadOptions options);

		object LoadModel(DbConnection db, LoadOptions options);

		// TODO: Add InsertModels, UpdateModels & DeleteModels

		void InsertModel(DbConnection db, object model);

		void UpdateModel(DbConnection db, object model);

		void DeleteModel(DbConnection db, object model);
	}

	public interface IPersistor<Model> : IPersistor
	{
		new IDbModelInfo<Model> ModelInfo { get; }

		new IEnumerable<Model> LoadModels(DbConnection db, LoadOptions options);

		new Model LoadModel(DbConnection db, LoadOptions options);

		//void SaveModel(DbConnection db, Model model);

		void DeleteModel(DbConnection db, Model model);
	}

	public abstract class PersistorBase : IPersistor
	{
		public PersistorBase(IDbModelInfo modelInfo)
		{
			ModelInfo = modelInfo;

			loadModelMethod = GetMethod("LoadModel", 2);
			loadModelsMethod = GetMethod("LoadModels", 2);

			insertModelMethod = GetMethod("InsertModel", 2);

			updateModelMethod = GetMethod("UpdateModel", 2);

			deleteModelMethod = GetMethod("DeleteModel", 2);

			/*
			switch (member.DataType)
			{
				case System.Data.DbType.Binary:
					break;

				case System.Data.DbType.Boolean:
					break;

				case System.Data.DbType.Byte:
					break;

				case System.Data.DbType.Currency:
					break;
				
				case System.Data.DbType.Date:
				case System.Data.DbType.DateTime:
				case System.Data.DbType.DateTime2:
					break;
				
				case System.Data.DbType.DateTimeOffset:
					break;
				
				case System.Data.DbType.Decimal:
				case System.Data.DbType.Double:
					break;
				
				case System.Data.DbType.Guid:

					break;
				
				case System.Data.DbType.Int16:
				case System.Data.DbType.Int32:
				case System.Data.DbType.Int64:
					break;
				
				case System.Data.DbType.Object:
					break;

				case System.Data.DbType.SByte:

					break;
				case System.Data.DbType.Single:
					break;

				case System.Data.DbType.AnsiString:
				case System.Data.DbType.AnsiStringFixedLength:
				case System.Data.DbType.String:
					break;
				
				case System.Data.DbType.StringFixedLength:
					break;
				case System.Data.DbType.Time:
					break;
				
				case System.Data.DbType.UInt16:
					break;
				case System.Data.DbType.UInt32:
					break;
				case System.Data.DbType.UInt64:
					break;
				
				case System.Data.DbType.VarNumeric:
					break;
				
				case System.Data.DbType.Xml:
					break;
				
				default:
					break;
			}
			*/
		}

		public IDbModelInfo ModelInfo { get; private set; }

		private MethodInfo loadModelMethod;
		private MethodInfo loadModelsMethod;

		private MethodInfo insertModelMethod;

		private MethodInfo updateModelMethod;

		private MethodInfo deleteModelMethod;

		private MethodInfo GetMethod(string name, int paramCount)
		{
			return GetType()
				.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.Name == name)
				.Where(m => m.GetParameters().Length == paramCount)
				.OrderByDescending(m => m.GetParameters().Sum(p => CountAncestors(p.ParameterType)))
				.FirstOrDefault();
		}

		private int CountAncestors(Type type)
		{
			int count = 0;
			while ((type != typeof(object)) && (type != null))
			{
				type = type.BaseType;
				count++;
			}
			return count;
		}

		public IEnumerable LoadModels(DbConnection db, LoadOptions options)
		{
			return (IEnumerable)loadModelsMethod.Invoke(this, new object[2] { db, options });
		}

		public object LoadModel(DbConnection db, LoadOptions options)
		{
			return loadModelMethod.Invoke(this, new object[2] { db, options });
		}

		public void InsertModel(DbConnection db, object model)
		{
			insertModelMethod.Invoke(this, new object[2] { db, model });
		}

		public void UpdateModel(DbConnection db, object model)
		{
			updateModelMethod.Invoke(this, new object[2] { db, model });
		}

		/*
		public void SaveModel(DbConnection db, object model)
		{
			var saveMethod = this.GetType().GetMethod(
				"SaveModel", BindingFlags.Public | BindingFlags.Instance);

			saveMethod.Invoke(this, new object[2] { db, model });
		}
		*/
		public void DeleteModel(DbConnection db, object model)
		{
			deleteModelMethod.Invoke(this, new object[2] { db, model });
		}
	}
}
