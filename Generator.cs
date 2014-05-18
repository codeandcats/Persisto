using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace Persisto
{
	internal static class Generator
	{
		static Generator()
		{
			GenerationInitiated = false;
		}

		internal static bool GenerationInitiated { get; private set; }

		public static void Generate()
		{
			if (GenerationInitiated)
			{
				return;
			}

			GenerationInitiated = true;

			var modelTypes = GetAllModelTypes(AppDomain.CurrentDomain.GetAssemblies());

			GenerateForTypes(modelTypes);
		}

		private static Type[] GetAllModelTypes(Assembly[] assemblies)
		{
			var types = new List<Type>();

			foreach (var assembly in assemblies)
			{
				foreach (var type in assembly
					.GetTypes()
					.Where(t => t.GetCustomAttribute<DbModelAttribute>() != null))
				{
					types.Add(type);
				}
			}

			return types.ToArray();
		}

		private static Assembly GenerateForTypes(Type[] modelTypes)
		{
			// Compile an in-memory assembly from our source code
			var provider = new CSharpCodeProvider();

			var options = new CompilerParameters();
			options.GenerateInMemory = true;
			options.IncludeDebugInformation = Debugger.IsAttached;
			options.ReferencedAssemblies.Add("System.Core.dll");
			options.ReferencedAssemblies.Add(typeof(System.ComponentModel.Component).Assembly.Location);
			options.ReferencedAssemblies.Add(typeof(Generator).Assembly.Location);
			options.ReferencedAssemblies.Add(typeof(DbConnection).Assembly.Location);

			var sources = new List<string>();
			var fileNames = new List<string>();

			// Firstly, create DbModelInfo instances for each Model type
			foreach (var modelType in modelTypes)
			{
				DbModelInfo.Get(modelType);
			}

			foreach (var modelType in modelTypes)
			{
				if (!options.ReferencedAssemblies.Contains(modelType.Assembly.Location))
				{
					options.ReferencedAssemblies.Add(modelType.Assembly.Location);
				}

				// Generate the source code
				var generatedModelSource = GenerateModelSource(modelType);
				var persistorSource = GeneratePersistorSource(modelType);

				sources.Add(generatedModelSource);
				sources.Add(persistorSource);

				if (Debugger.IsAttached)
				{
					fileNames.Add(WriteGeneratedSource(string.Format("{0}.cs", modelType.FullName), generatedModelSource));
					fileNames.Add(WriteGeneratedSource(string.Format("{0}_Persistor.cs", modelType.FullName), persistorSource));
				}
			}

			CompilerResults results;

			if (Debugger.IsAttached)
			{
				results = provider.CompileAssemblyFromFile(options, fileNames.ToArray());
			}
			else
			{
				results = provider.CompileAssemblyFromSource(options, sources.ToArray());
			}

			if (results.Errors.Count > 0)
			{
				if (Debugger.IsAttached)
				{
					foreach (var fileErrors in
						results.Errors
						.Cast<CompilerError>()
						.GroupBy(e => e.FileName))
					{
						var fileName = fileErrors.Key;

						var sb = new StringBuilder();
						sb.AppendLine("Compiler Errors:");

						var lineOffset = fileErrors.Count() + 2;

						foreach (var error in fileErrors)
						{
							sb.AppendLine(string.Format(
								"{0} at Line {1}, Column {2}", 
								error.ErrorText,
								error.Line + lineOffset, 
								error.Column));
						}

						sb.AppendLine();

						sb.Append(File.ReadAllText(fileName));

						fileName = Path.GetTempFileName();

						File.WriteAllText(fileName, sb.ToString());

						Process.Start(
							new ProcessStartInfo()
							{
								FileName = "notepad.exe",
								Arguments = fileName,
								WindowStyle = ProcessWindowStyle.Maximized
							});
					}

					Process.GetCurrentProcess().Kill();
				}

				throw new Exception("Compiler Error generating supporting Persisto classes");
			}

			// Now loop through all the model types and assign the generated model types
			// and persistor instances
			foreach (var modelType in modelTypes)
			{
				var modelInfo = (DbModelInfo)DbModelInfo.Get(modelType);

				var generatedTypeFullName = "Persisto.Generated.Models." + modelInfo.GeneratedModelTypeName;

				var generatedType = results.CompiledAssembly.GetType(generatedTypeFullName);

				modelInfo.GeneratedModelType = generatedType;

				var persistorTypeName = "Persisto.Generated.Persistors." + modelType.Name + "Persistor";

				var persistorType = results.CompiledAssembly.GetType(persistorTypeName);

				var persistor = (IPersistor)Activator.CreateInstance(persistorType, new object[] { modelInfo });

				modelInfo.Persistor = persistor;
			}

			return results.CompiledAssembly;
		}

		private static string WriteGeneratedSource(string fileName, string source)
		{
			fileName = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"Automedia",
				"Generated",
				fileName);

			Directory.CreateDirectory(Path.GetDirectoryName(fileName));

			if (File.Exists(fileName))
			{
				var existingSource = File.ReadAllText(fileName);
				if (existingSource.GetHashCode() == source.GetHashCode())
				{
					return fileName;
				}
			}

			File.WriteAllText(fileName, source);

			return fileName;
		}

		private static string GeneratePersistorSource(Type modelType)
		{
			var template = new Templates.PersistorTemplate();

			template.Session = new Dictionary<string, object>();

			template.Session["ModelInfo"] = DbModelInfo.Get(modelType);

			template.Initialize();

			return template.TransformText();
		}

		private static string GenerateModelSource(Type modelType)
		{
			var template = new Templates.GeneratedModelTemplate();

			template.Session = new Dictionary<string, object>();

			template.Session["ModelInfo"] = DbModelInfo.Get(modelType);

			template.Initialize();

			var source = template.TransformText();
			
			return source;
		}

		/*
		internal static string GenerateModelSelectSql(IDbModelInfo modelInfo)
		{
			var baseModelInfos = new Stack<IDbModelInfo>();

			var info = modelInfo;

			while (info != null)
			{
				baseModelInfos.Push(info);
				info = info.BaseModelInfo;
			}
			
			var selectSql = new StringBuilder();
			selectSql.AppendLine("SELECT ");

			var fromSql = new StringBuilder();

			var addedFirstTable = false;
			IDbModelInfo firstTable;

			while (baseModelInfos.Count > 0)
			{
				info = baseModelInfos.Pop();

				selectSql.Append("\t");

				var addedFirstField = false;

				foreach (var memberInfo in info.Members.Where(m => m.IsBackedByField))
				{
					if (addedFirstField)
					{
						selectSql.Append(", ");
					}
					else
					{
						addedFirstField = true;
					}
					selectSql.Append(info.TableName + "." + memberInfo.FieldName);
				}

				if (baseModelInfos.Count > 0)
				{
					selectSql.Append(", ");
				}

				if (addedFirstTable)
				{
					fromSql.AppendLine(string.Format(
						"JOIN {0} ON ({0}.{1} = {2}.{3}){4}",
						info.TableName,
						info.ID.FieldName,
						info.BaseModelInfo.TableName,
						info.BaseModelInfo.ID.FieldName,
						string.IsNullOrWhiteSpace(info.Filter) ? "" : " AND (" + info.Filter + ")"));
				}
				else
				{
					firstTable = info;
					fromSql.AppendLine("FROM " + info.TableName);
				}

				foreach (var subTypeInfo in info.Descendents)
				{
					fromSql.AppendLine(string.Format(
						"LEFT OUTER JOIN {0} ON ({0}.{1} = {2}.{3}){4}",
						subTypeInfo.TableName,
						subTypeInfo.ID.FieldName,
						info.TableName,
						info.ID.FieldName,
						string.IsNullOrWhiteSpace(subTypeInfo.Filter) ? "" : " AND (" + subTypeInfo.Filter + ")"));
				}
			}

			return selectSql.ToString() + "\n" + fromSql.ToString();
		}
		*/

		private static void DebugError(
			CompilerResults results,
			string generatedTypeSource,
			string persistorSource)
		{
			var fileName = Path.GetTempFileName();

			var sb = new StringBuilder();

			for (var index = 0; index < results.Errors.Count; index++)
			{
				var error = results.Errors[index];

				sb.AppendFormat(
					"{0}: {1} in {2} at line {3}, column {4}",
					error.IsWarning ? "Warning" : "Error",
					error.ErrorText,
					error.FileName,
					error.Line,
					error.Column);

				sb.AppendLine();
			}
			sb.AppendLine();
			sb.Append(persistorSource);
			sb.AppendLine();
			sb.AppendLine("----------------------------------------------------------------");
			sb.AppendLine();
			sb.Append(generatedTypeSource);

			System.IO.File.WriteAllText(fileName, sb.ToString());

			var startInfo = new ProcessStartInfo("notepad.exe", fileName);
			startInfo.WindowStyle = ProcessWindowStyle.Maximized;

			Process.Start(startInfo);

			System.Threading.Thread.Sleep(500);

			// Kill off the current process
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
	}
}
