﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 11.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Persisto.Templates
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data;
    using System.Data.Common;
    using Persisto;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "11.0.0.0")]
    public partial class PersistorTemplate : PersistorTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing Syste" +
                    "m.Text;\r\nusing System.Diagnostics;\r\nusing System.Data.Common;\r\nusing Persisto;\r\n" +
                    "using ");
            
            #line 18 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.Namespace));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\nnamespace Persisto.Generated.Persistors\r\n{\r\n\tpublic class ");
            
            #line 22 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.Name));
            
            #line default
            #line hidden
            this.Write("Persistor : PersistorBase, IPersistor<");
            
            #line 22 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.FullName));
            
            #line default
            #line hidden
            this.Write(">\r\n\t{\r\n\t\tpublic ");
            
            #line 24 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.Name));
            
            #line default
            #line hidden
            this.Write("Persistor(IDbModelInfo<");
            
            #line 24 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.Name));
            
            #line default
            #line hidden
            this.Write("> modelInfo)\r\n\t\t\t: base(modelInfo)\r\n\t\t{\r\n\t\t\tModelInfo = modelInfo;\r\n\r\n");
            
            #line 29 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"

var membersWithFields = 
	ModelInfo.Members
	.Where(m => m.IsBackedByField)
	.ToArray();

foreach (IDbMemberInfo member in ModelInfo.Members)
{
	WriteLine("\t\t\tMemberInfo_{0} = modelInfo[\"{0}\"];", member.Name);
}

            
            #line default
            #line hidden
            this.Write("\t\t}\r\n\t\t\r\n\t\tnew public IDbModelInfo<");
            
            #line 42 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.Name));
            
            #line default
            #line hidden
            this.Write("> ModelInfo { get; private set; }\r\n\t\t\r\n");
            
            #line 44 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"

foreach (IDbMemberInfo member in ModelInfo.Members)
{
	WriteLine("\t\tinternal IDbMemberInfo MemberInfo_" + member.Name + " { get; private set; }");
}

            
            #line default
            #line hidden
            this.Write("\r\n\t\tnew public IEnumerable<");
            
            #line 51 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.FullName));
            
            #line default
            #line hidden
            this.Write("> LoadModels(\r\n\t\t\tSystem.Data.Common.DbConnection db,\r\n\t\t\tPersisto.LoadOptions op" +
                    "tions)\r\n\t\t{\r\n\t\t\tvar models = new List<");
            
            #line 55 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.FullName));
            
            #line default
            #line hidden
            this.Write(@">();
			
			Func<System.Data.Common.DbConnection> createConnection = options.CreateConnectionFunc;
			if (createConnection == null)
			{
				createConnection = DbModelInfo.CreateConnectionFunc;
			}

			using (var command = db.CreateCommand())
			{
");
            
            #line 65 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"

// Build sql for loading models
var loadSql = new StringBuilder("SELECT " + ModelInfo.TableName + ".");

loadSql.Append(string.Join(", " + ModelInfo.TableName + ".", membersWithFields.Select(m => m.FieldName).ToArray()));

loadSql.Append(" FROM " + ModelInfo.TableName);

            
            #line default
            #line hidden
            this.Write("\t\t\t\tvar loadSql = new StringBuilder(@\"");
            
            #line 73 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(loadSql.ToString()));
            
            #line default
            #line hidden
            this.Write(@""");

				if (!string.IsNullOrWhiteSpace(options.Joins))
				{
					loadSql.Append("" "" + options.Joins + "" "");
				}
				
				if (!string.IsNullOrWhiteSpace(options.Where))
				{
					if (!options.Where.Trim().ToLower().StartsWith(""where""))
					{
						loadSql.Append("" WHERE"");
					}
					loadSql.Append("" "" + options.Where + "" "");
				}

				if (!string.IsNullOrWhiteSpace(options.OrderBy))
				{
					if (!options.Where.Trim().ToLower().StartsWith(""order by""))
					{
						loadSql.Append("" ORDER BY"");
					}
					loadSql.Append("" "" + options.OrderBy + "" "");
				}

				command.CommandText = loadSql.ToString();
				
				var paramNames = options.GetParamNames();

				if (options.ParamValues != null)
				{
					for (var index = 0; index < options.ParamValues.Length; index++)
					{
						command.CreateParameter(paramNames[index], options.ParamValues[index]);
					}
				}
				
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var model = new Persisto.Generated.Models.");
            
            #line 114 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.GeneratedModelTypeName));
            
            #line default
            #line hidden
            this.Write("();\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\tmodel._Support.CreateConnectionFunc = createConnection;\r\n\t\t\t\t\t" +
                    "\t\r\n");
            
            #line 118 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"

for (var memberIndex = 0; memberIndex < ModelInfo.Members.Length; memberIndex++)
{
	DbMemberInfo member = ModelInfo.Members[memberIndex];
	string memberName = member.Name;
	
	Type memberType = member.MemberType;
	
	string method = "";
	
	var manyToOne = member.Relation as DbRelation.ManyToOneAttribute;
	var oneToMany = member.Relation as DbRelation.OneToManyAttribute;
	var manyToMany = member.Relation as DbRelation.ManyToManyAttribute;

	var foreignTypeModelInfo = member.Relation == null ? null : DbModelInfo.Get(member.Relation.ForeignType);
	
	if (member.Relation != null)
	{
		if (manyToOne != null)
		{
			if ((bool)member.Relation.LoadOnDemand)
			{
				IDbModelInfo foreignModelInfo = DbModelInfo.Get(member.Relation.ForeignType);
				
				memberName = member.Name + foreignModelInfo.ID.Name;
				
				memberType = foreignModelInfo.ID.MemberType;
				
				WriteLine("{0}model.HasLoaded{1} = false;", "\t\t\t\t\t\t", member.Name);
			}
			else
			{
				method = 
					"DbModelInfo.Get<" + member.MemberType.FullName +
					">().Persistor.LoadModel(db, new LoadOptions() { Where = \"" +
					foreignTypeModelInfo.ID.FieldName + " = @" + foreignTypeModelInfo.ID.FieldName +
					"\", ParamValues = new object[] { reader.GetValue(" + memberIndex.ToString() + ") } })";
				
				WriteLine("{0}model.{1} = {2};", "\t\t\t\t\t\t", memberName, method);
				
				continue;
			}
		}
		else if (oneToMany != null)
		{
			if (!member.Relation.LoadOnDemand)
			{

            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t\tmodel.");
            
            #line 166 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.Name));
            
            #line default
            #line hidden
            this.Write(" = db.LoadModels<");
            
            #line 166 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.Relation.ForeignType.FullName));
            
            #line default
            #line hidden
            this.Write(">(\r\n\t\t\t\t\t\t\tnew LoadOptions()\r\n\t\t\t\t\t\t\t{\r\n\t\t\t\t\t\t\t\tWhere = \"");
            
            #line 169 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.FieldName));
            
            #line default
            #line hidden
            this.Write(" = @");
            
            #line 169 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(member.FieldName));
            
            #line default
            #line hidden
            this.Write("\",\r\n\t\t\t\t\t\t\t\tParamValues = new object[] { model.");
            
            #line 170 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ID.Name));
            
            #line default
            #line hidden
            this.Write(" } \r\n\t\t\t\t\t\t\t}).ToList();\r\n");
            
            #line 172 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"

			}
			continue;
		}
		else
		{
			// TODO: Support ManyToMany
			continue;
		}
	}

	if (!member.IsBackedByField)
	{
		continue;
	}

	if (memberType == typeof(Guid))
	{
		method = string.Format("reader.IsDBNull({0}) ? default(Guid) : new Guid(reader.GetString({0}))", memberIndex);
	}
	else if (memberType == typeof(int))
	{
		method = string.Format("reader.GetInt32({0})", memberIndex);
	}
	else if (memberType == typeof(Int16))
	{
		method = string.Format("reader.GetInt16({0})", memberIndex);
	}
	else if (memberType == typeof(Int64))
	{
		method = string.Format("reader.GetInt64({0})", memberIndex);
	}
	else if (memberType == typeof(DateTime))
	{
		method = string.Format("reader.GetDateTime({0})", memberIndex);
	}
	else if (memberType == typeof(string))
	{
		method = string.Format("reader.GetString({0})", memberIndex);
	}
	else if (memberType == typeof(double))
	{
		method = string.Format("reader.GetDouble({0})", memberIndex);
	}
	else if (memberType.IsEnum)
	{
		if ((member.DataType == DbType.String) || (member.DataType == DbType.AnsiString))
		{
			method = string.Format("({0})Enum.Parse(typeof({0}), reader.GetString({1}))",
				member.MemberType.FullName, memberIndex);
		}
		else if (member.DataType == DbType.Int16)
		{
			method = string.Format("({0})reader.GetInt16({1})",
				member.MemberType.FullName, memberIndex);
		}
		else if (member.DataType == DbType.Int32)
		{
			method = string.Format("({0})reader.GetInt32({1})",
				member.MemberType.FullName, memberIndex);
		}
		else if (member.DataType == DbType.Int64)
		{
			method = string.Format("({0})reader.GetInt64({1})",
				member.MemberType.FullName, memberIndex);
		}
		else if (member.DataType == DbType.Byte)
		{
			method = string.Format("({0})reader.GetByte({1})",
				member.MemberType.FullName, memberIndex);
		}
		else
		{
			continue;
		}
	}
	else if (memberType == typeof(bool))
	{
		if (member.DataType == DbType.Boolean)
		{
			method = string.Format("reader.GetBoolean({0})", memberIndex);
		}
		else if (member.DataType == DbType.Int16)
		{
			method = string.Format("reader.GetInt16({0}) > 0", memberIndex);
		}
		else if (member.DataType == DbType.Int32)
		{
			method = string.Format("reader.GetInt32({0}) > 0", memberIndex);
		}
		else if (member.DataType == DbType.Int64)
		{
			method = string.Format("reader.GetInt64({0}) > 0", memberIndex);
		}
		else if (member.DataType == DbType.Byte)
		{
			method = string.Format("reader.GetByte({0}) > 0", memberIndex);
		}
		else if ((member.DataType == DbType.String) || (member.DataType == DbType.AnsiString))
		{
			method = string.Format(
				"!reader.GetString({0}).Equals(\"N\", StringComparison.CurrentCultureIgnoreCase)", memberIndex);
		}
		else
		{
			continue;
		}
	}
	else
	{
		continue;
	}
	
	WriteLine("{0}model.{1} = {2};", "\t\t\t\t\t\t", memberName, method);
}

            
            #line default
            #line hidden
            this.Write("\r\n\t\t\t\t\t\tmodel._Support.ExistsInDatabase = true;\r\n\t\t\t\t\t\t\r\n\t\t\t\t\t\tmodels.Add(model);" +
                    "\r\n\t\t\t\t\t}\r\n\t\t\t\t}\r\n\t\t\t}\r\n\r\n\t\t\treturn models;\r\n\t\t}\r\n\r\n");
            
            #line 299 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"


Action<bool, bool> setFields =
	delegate(bool includeId, bool useIncrementorValues)
	{
		for (var memberIndex = 0; memberIndex < membersWithFields.Length; memberIndex++)
		{
			DbMemberInfo member = membersWithFields[memberIndex];
			string memberName = member.Name;
			var expression = "";

			if (member.IsIdentity && !includeId)
			{
				continue;
			}

			if (useIncrementorValues && (member.Incrementor != null))
			{
				expression = "newValueFor" + memberName + ".ToString()";
			}
			else if (member.Relation != null)
			{
				if (member.Relation is DbRelation.ManyToOneAttribute)
				{
					IDbModelInfo foreignModelInfo = DbModelInfo.Get(member.Relation.ForeignType);
					
					if ((bool)member.Relation.LoadOnDemand)
					{
						string generatedModelTypeName = "Persisto.Generated.Models." + ModelInfo.ModelType.Name;						
						
						string foreignModelIdName = foreignModelInfo.ID.Name;
						if (foreignModelInfo.ID.MemberType == typeof(Guid))
						{
							foreignModelIdName += ".ToString(true)";
						}
						
						expression = string.Format(
							"model is {0} ? (({0})model).{1}{2} : (model.{1} == null ? null : (object)model.{1}.{2})",
							generatedModelTypeName,
							member.Name,
							foreignModelIdName);
					}
					else
					{
						string foreignModelIdName = foreignModelInfo.ID.Name;
						if (foreignModelInfo.ID.MemberType == typeof(Guid))
						{
							foreignModelIdName += ".ToString(true)";
						}
						
						expression = string.Format("model.{0} == null ? null : model.{0}.{1}",
							member.Name,
							foreignModelIdName);
					}
				}
				else if (member.Relation is DbRelation.OneToManyAttribute)
				{
					continue;
				}
				else
				{
					// TODO: Support OneToMany + ManyToMany
					continue;
				}
			}
			else
			{
				if (member.MemberType.IsEnum)
				{
					if ((member.DataType == DbType.Int16) || (member.DataType == DbType.Int32) ||
						(member.DataType == DbType.Int64) || (member.DataType == DbType.Byte) ||
						(member.DataType == DbType.SByte))
					{
						expression = string.Format("(int)model.{0}", memberName);
					}
					else if ((member.DataType == DbType.String) || (member.DataType == DbType.AnsiString) ||
							(member.DataType == DbType.AnsiStringFixedLength))
					{
						expression = string.Format("model.{0}.ToString()", memberName);
					}
				}
				else if (member.MemberType == typeof(bool))
				{
					if ((member.DataType == DbType.Int16) || (member.DataType == DbType.Int32) ||
						(member.DataType == DbType.Int64) || (member.DataType == DbType.Byte) ||
						(member.DataType == DbType.SByte))
					{
						expression = string.Format("model.{0} ? 1 : 0", memberName);
					}
					else if ((member.DataType == DbType.String) || (member.DataType == DbType.AnsiString) ||
							(member.DataType == DbType.AnsiStringFixedLength))
					{
						expression = string.Format("model.{0} ? \"Y\" : \"N\"", memberName);
					}
					else
					{
						expression = "model." + memberName;
					}
				}
				else if (member.MemberType == typeof(Guid))
				{
					expression = "model." + memberName + ".ToString(true)";
				}
				else
				{
					expression = "model." + memberName;
				}
			}

			WriteLine("{0}command.CreateParameter(\"@{1}\", {2});", "\t\t\t\t", member.FieldName, expression);
		}
	};


            
            #line default
            #line hidden
            this.Write("\t\t\r\n\t\tnew public ");
            
            #line 414 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.FullName));
            
            #line default
            #line hidden
            this.Write(@" LoadModel(
			System.Data.Common.DbConnection db,
			Persisto.LoadOptions options)
		{
			var models = LoadModels(db, options);

			var model = models.FirstOrDefault();

			return model;
		}
		
		public void InsertModel(
			System.Data.Common.DbConnection db,
			");
            
            #line 427 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.FullName));
            
            #line default
            #line hidden
            this.Write(" model)\r\n\t\t{\r\n\t\t\tif (ModelInfo.BaseModelInfo != null)\r\n\t\t\t{\r\n\t\t\t\tModelInfo.BaseMo" +
                    "delInfo.Persistor.InsertModel(db, model);\r\n\t\t\t}\r\n\t\t\t\r\n\t\t\tusing (var command = db" +
                    ".CreateCommand())\r\n\t\t\t{\r\n");
            
            #line 436 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"


StringBuilder insertSql = new StringBuilder("INSERT INTO " + ModelInfo.TableName);

insertSql.Append(" (" + string.Join(", ", membersWithFields.Select(m => m.FieldName).ToArray()) + ")");
insertSql.Append(" VALUES (@" + string.Join(", @", membersWithFields.Select(m => m.FieldName).ToArray()) + ")");


            
            #line default
            #line hidden
            this.Write("\t\t\t\tcommand.CommandText = \"");
            
            #line 444 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(insertSql.ToString()));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n");
            
            #line 446 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"

foreach (IDbMemberInfo member in ModelInfo.Members.Where(m => m.Incrementor != null))
{
	WriteLine("\t\t\t\tvar newValueFor{0} = model.{0} != default({1}) ? model.{0} : ({1})MemberInfo_{0}.Incrementor.Increment(ModelInfo, MemberInfo_{0}, model);", member.Name, member.MemberType.FullName);
}

WriteLine("");

setFields(true, true);

            
            #line default
            #line hidden
            this.Write("\r\n\t\t\t\tcommand.ExecuteNonQuery();\r\n\r\n");
            
            #line 459 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"

foreach (IDbMemberInfo member in ModelInfo.Members.Where(m => m.Incrementor != null))
{
	WriteLine("\t\t\t\tmodel.{0} = newValueFor{0};", member.Name);
}

            
            #line default
            #line hidden
            this.Write("\t\t\t\tvar generatedModel = model as Persisto.IGeneratedModel;\r\n\t\t\t\tif (generatedMod" +
                    "el != null)\r\n\t\t\t\t{\r\n\t\t\t\t\tgeneratedModel._Support.ExistsInDatabase = true;\r\n\t\t\t\t}" +
                    "\r\n\t\t\t}\r\n\t\t}\r\n\t\t\r\n\t\tpublic void UpdateModel(\r\n\t\t\tSystem.Data.Common.DbConnection " +
                    "db,\r\n\t\t\t");
            
            #line 474 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.FullName));
            
            #line default
            #line hidden
            this.Write(" model)\r\n\t\t{\r\n\t\t\tif (ModelInfo.BaseModelInfo != null)\r\n\t\t\t{\r\n\t\t\t\tModelInfo.BaseMo" +
                    "delInfo.Persistor.UpdateModel(db, model);\r\n\t\t\t}\r\n\t\t\t\r\n\t\t\tusing (var command = db" +
                    ".CreateCommand())\r\n\t\t\t{\r\n");
            
            #line 483 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"

StringBuilder updateSql = new StringBuilder("UPDATE " + ModelInfo.TableName + " SET ");
updateSql.Append(string.Join(", ", membersWithFields
	.Where(m => !m.IsIdentity)
	.Select(m => string.Format("{0} = @{0}", m.FieldName)).ToArray()));
updateSql.Append(" WHERE " + string.Format("{0} = @{0}", ModelInfo.ID.FieldName));

            
            #line default
            #line hidden
            this.Write("\t\t\t\tcommand.CommandText = @\"");
            
            #line 490 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(updateSql.ToString()));
            
            #line default
            #line hidden
            this.Write("\";\r\n\t\t\t\t\r\n");
            
            #line 492 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
 setFields(false, false); 
            
            #line default
            #line hidden
            this.Write(@"				
				command.ExecuteNonQuery();
				
				var generatedModel = model as Persisto.IGeneratedModel;
				if (generatedModel != null)
				{
					generatedModel._Support.ExistsInDatabase = true;
				}
			}
		}
		
		public void DeleteModel(
			System.Data.Common.DbConnection db,
			");
            
            #line 506 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ModelType.FullName));
            
            #line default
            #line hidden
            this.Write(" model)\r\n\t\t{\r\n\t\t\tusing (var command = db.CreateCommand())\r\n\t\t\t{\r\n\t\t\t\tcommand.Comm" +
                    "andText = @\"DELETE FROM ");
            
            #line 510 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.TableName));
            
            #line default
            #line hidden
            this.Write(" WHERE ");
            
            #line 510 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ID.FieldName));
            
            #line default
            #line hidden
            this.Write(" = @");
            
            #line 510 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ID.FieldName));
            
            #line default
            #line hidden
            this.Write("\";\r\n\t\t\t\t\r\n\t\t\t\tcommand.CreateParameter(\r\n\t\t\t\t\t\"@");
            
            #line 513 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ID.FieldName));
            
            #line default
            #line hidden
            this.Write("\",\r\n\t\t\t\t\tSystem.Data.DbType.");
            
            #line 514 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ID.DataType.ToString()));
            
            #line default
            #line hidden
            this.Write(",\r\n\t\t\t\t\tmodel.");
            
            #line 515 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ModelInfo.ID.Name));
            
            #line default
            #line hidden
            this.Write(",\r\n\t\t\t\t\ttrue);\r\n\t\t\t\t\r\n\t\t\t\tcommand.ExecuteNonQuery();\r\n\t\t\t\t\r\n\t\t\t\tvar generatedMode" +
                    "l = model as Persisto.IGeneratedModel;\r\n\t\t\t\tif (generatedModel != null)\r\n\t\t\t\t{\r\n" +
                    "\t\t\t\t\tgeneratedModel._Support.ExistsInDatabase = false;\r\n\t\t\t\t}\r\n\t\t\t}\r\n\t\t}\r\n\t}\r\n}");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 1 "C:\Users\Ben Daniel\documents\Projects\Sandbox\Automedia\Persisto\Templates\PersistorTemplate.tt"

private global::Persisto.IDbModelInfo _ModelInfoField;

/// <summary>
/// Access the ModelInfo parameter of the template.
/// </summary>
private global::Persisto.IDbModelInfo ModelInfo
{
    get
    {
        return this._ModelInfoField;
    }
}


/// <summary>
/// Initialize the template
/// </summary>
public virtual void Initialize()
{
    if ((this.Errors.HasErrors == false))
    {
bool ModelInfoValueAcquired = false;
if (this.Session.ContainsKey("ModelInfo"))
{
    if ((typeof(global::Persisto.IDbModelInfo).IsAssignableFrom(this.Session["ModelInfo"].GetType()) == false))
    {
        this.Error("The type \'Persisto.IDbModelInfo\' of the parameter \'ModelInfo\' did not match the t" +
                "ype of the data passed to the template.");
    }
    else
    {
        this._ModelInfoField = ((global::Persisto.IDbModelInfo)(this.Session["ModelInfo"]));
        ModelInfoValueAcquired = true;
    }
}
if ((ModelInfoValueAcquired == false))
{
    object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData("ModelInfo");
    if ((data != null))
    {
        if ((typeof(global::Persisto.IDbModelInfo).IsAssignableFrom(data.GetType()) == false))
        {
            this.Error("The type \'Persisto.IDbModelInfo\' of the parameter \'ModelInfo\' did not match the t" +
                    "ype of the data passed to the template.");
        }
        else
        {
            this._ModelInfoField = ((global::Persisto.IDbModelInfo)(data));
        }
    }
}


    }
}


        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "11.0.0.0")]
    public class PersistorTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
