using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Persisto
{
	public static class DbExtensions
	{
		#region Generic Model Persistence

		public static Model New<Model>()
		{
			var modelInfo = DbModelInfo.Get<Model>();
			var model = (Model)Activator.CreateInstance(modelInfo.GeneratedModelType);
			return model;
		}

		public static Model New<Model>(this DbConnection db)
		{
			return New<Model>();
		}

		public static IEnumerable<Model> LoadModels<Model>(
			this DbConnection db,
			dynamic options = null)
		{
			var modelInfo = DbModelInfo.Get<Model>();
			var persistor = modelInfo.Persistor;
			return persistor.LoadModels(db, LoadOptions.From(options));
		}

		public static Model LoadModel<Model>(this DbConnection db,
			dynamic options = null)
		{
			var modelInfo = DbModelInfo.Get<Model>();
			var persistor = modelInfo.Persistor;
			return persistor.LoadModel(db, LoadOptions.From(options));
		}

		public static Model LoadModelById<Model>(this DbConnection db,
			object id)
		{
			var info = DbModelInfo.Get<Model>();

			var options = new LoadOptions()
			{
				Where = string.Format("{0} = @{0}", info.ID.FieldName),
				ParamValues = new[] { id }
			};

			return info.Persistor.LoadModel(db, options);
		}

		public static void DeleteModel<Model>(this DbConnection db, Model model)
		{
			DbModelInfo.Get<Model>().Persistor.DeleteModel(db, model);
		}

		#endregion

		#region Non-Generic Model Persistence

		public static IEnumerable LoadModels(
			this DbConnection db,
			Type modelType,
			dynamic options = null)
		{
			var info = DbModelInfo.Get(modelType);

			var persistor = info.Persistor;

			return persistor.LoadModels(db, LoadOptions.From(options));
		}

		public static void InsertModel(this DbConnection db, object model)
		{
			var info = DbModelInfo.Get(model.GetType());

			var persistor = info.Persistor;

			persistor.InsertModel(db, model);
		}

		public static void UpdateModel(this DbConnection db, object model)
		{
			var info = DbModelInfo.Get(model.GetType());

			var persistor = info.Persistor;

			persistor.UpdateModel(db, model);
		}

		/*
		public static void SaveModel(this DbConnection db, Type modelType, object model)
		{
			var info = DbModelInfo.Get(modelType);

			var persistor = info.Persistor;

			persistor.SaveModel(db, model);
		}
		*/
		public static void DeleteModel(this DbConnection db, Type modelType, object model)
		{
			var info = DbModelInfo.Get(modelType);

			var persistor = info.Persistor;

			persistor.DeleteModel(db, model);
		}

		#endregion

		#region Helper Methods

		public static DbType DbTypeFromType(Type type)
		{
			if (type == typeof(Int16))
				return DbType.Int16;
			else if (type == typeof(Int32))
				return DbType.Int32;
			else if (type == typeof(Int64))
				return DbType.Int64;
			else if (type == typeof(DateTime))
				return DbType.DateTime;
			else if (type == typeof(double))
				return DbType.Double;
			else if (type == typeof(Single) || type == typeof(float))
				return DbType.Single;
			else if (type == typeof(string))
				return DbType.String;
			else if (type == typeof(Guid))
				return DbType.String;
			else if (type == null)
				return DbType.String;
			else
				throw new Exception(string.Format("Cannot convert from Type to DbType, unexpected type: \"{0}\"", type.Name));
		}

		public static DbParameter CreateParameter(
			this DbCommand command,
			string parameterName,
			object value,
			bool addToCommand = true)
		{
			var dataType = value == null ? DbType.String : DbTypeFromType(value.GetType());

			return command.CreateParameter(parameterName, dataType, value, addToCommand);
		}

		public static DbParameter CreateParameter(
			this DbCommand command,
			string parameterName,
			DbType dataType,
			object value,
			bool addToCommand = true)
		{
			var parameter = command.CreateParameter();
			parameter.ParameterName = parameterName;
			parameter.DbType = dataType;
			parameter.Value = value;

			if (addToCommand)
				command.Parameters.Add(parameter);

			return parameter;
		}

		public static DbCommand CreateCommand(
			this DbConnection connection,
			string commandText,
			params object[] parameters)
		{
			var command = connection.CreateCommand();
			command.CommandText = commandText;

			var paramNames = ExtractParamNamesFromSQL(commandText);

			for (var paramIndex = 0; paramIndex < parameters.Length; paramIndex++)
			{
				var param = parameters[paramIndex];

				if (param is DbParameter)
					command.Parameters.Add((DbParameter)param);
				else
				{
					var newParam = command.CreateParameter();

					if (paramIndex < paramNames.Length)
					{
						newParam.ParameterName = paramNames[paramIndex];
					}

					newParam.DbType = DbTypeFromType(param == null ? null : param.GetType());

					newParam.Value = param;

					command.Parameters.Add(newParam);
				}
			}

			return command;
		}

		public static object ExecuteScalar(
			this DbConnection connection,
			string commandText,
			params object[] parameters)
		{
			using (var command = connection.CreateCommand(commandText, parameters))
			{
				object result = command.ExecuteScalar();

				return result;
			}
		}

		public static void ExecuteNonQuery(
			this DbConnection connection,
			string commandText,
			params object[] parameters)
		{
			using (var command = connection.CreateCommand(commandText, parameters))
			{
				command.ExecuteNonQuery();
			}
		}

		public static bool TableExists(this DbConnection db, string tableName)
		{
			using (var data = db.GetSchema("TABLES", new string[] { null, null, tableName }))
			{
				return (data.Rows.Count > 0);
			}
		}

		private static string RemoveCommentsAndStringsFromSQL(string sql)
		{
			// Remove multiline comments
			sql = Regex.Replace(sql, @"(?:/\*.*?\*/)", "");

			// Remove double-quote strings
			sql = Regex.Replace(sql, "(?:\".*?\")", "");

			// Remove single-line strings
			sql = Regex.Replace(sql, "(?:'.*?')", "");

			// Remove single-line comments
			sql = Regex.Replace(sql, @"(?:--[^\n]*)", "");

			return sql;
		}

		public static string[] ExtractParamNamesFromSQL(string sql)
		{
			var commandNoCommentsOrStrings = RemoveCommentsAndStringsFromSQL(sql);

			const string pattern = @"@[a-zA-Z_]\w*";

			var matches = Regex.Matches(sql, pattern);

			var paramNames = matches
				.Cast<Match>()
				.Select(m => m.Value)
				.ToArray();

			return paramNames;
		}

		public static string RemoveComments(string sql)
		{
			var inMultilineComment = false;
			var inSingleLineComment = false;
			var inDoubleQuotes = false;
			var inSingleQuotes = false;

			var index = 0;
			var len = sql.Length;

			var result = new StringBuilder();

			var blockStart = 0;

			Action addChunk = () =>
			{
				var chunk = sql.Substring(blockStart, index - blockStart);

				if (chunk != "")
				{
					result.Append(chunk);
				}
			};

			while (index < len)
			{
				var chr = sql[index];
				var nextChr = index + 1 < len ? sql[index + 1] : (char)0;

				if (inMultilineComment)
				{
					if ((chr == '*') && (nextChr == '/'))
					{
						inMultilineComment = false;
						blockStart = index + 2;
						index++;
					}
					index++;
				}
				else if (inSingleLineComment)
				{
					if ((chr == '\n') || (chr == '\r'))
					{
						inSingleLineComment = false;
						blockStart = index;
					}
					index++;
				}
				else if (inSingleQuotes)
				{
					if (chr == '\'')
					{
						inSingleQuotes = false;
					}
					index++;
				}
				else if (inDoubleQuotes)
				{
					if (chr == '"')
					{
						inDoubleQuotes = false;
					}
					index++;
				}
				else if ((chr == '/') && (nextChr == '*'))
				{
					addChunk();
					inMultilineComment = true;
					index += 2;
				}
				else if ((chr == '-') && (nextChr == '-'))
				{
					addChunk();
					inSingleLineComment = true;
					index += 2;
				}
				else if (chr == '\'')
				{
					inSingleQuotes = true;
					index++;
				}
				else if (chr == '"')
				{
					inDoubleQuotes = true;
					index++;
				}
				else
				{
					index++;
				}
			}

			addChunk();

			return result.ToString();
		}

		public static string IdentifierToDbIdentifier(string identifier)
		{
			var sb = new StringBuilder();

			var prevWasUpperCase = true;

			foreach (var chr in identifier)
			{
				var isUpperCase = chr.ToString() == chr.ToString().ToUpper();

				if (isUpperCase && !prevWasUpperCase)
				{
					sb.Append("_");
				}

				prevWasUpperCase = isUpperCase;

				sb.Append(chr);
			}

			return sb.ToString().ToUpper();
		}

		#endregion
	}
}