﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;

namespace Persisto
{
	public class ModelSupport
	{
		public Func<DbConnection> CreateConnectionFunc;

		public DbConnection CreateConnection()
		{
			if (CreateConnectionFunc != null)
			{
				return CreateConnectionFunc();
			}
			else
			{
				if (DbModelInfo.CreateConnectionFunc != null)
				{
					return DbModelInfo.CreateConnectionFunc();
				}
				else
				{
					throw new Exception("Load on demand ability not available, no CreateConnectionFunc provided");
				}
			}
		}

		private List<string> changedFields = new List<string>();
		private bool hasUnsavedChanges = true;

		public void FieldChanged(string fieldName)
		{
			changedFields.Add(fieldName);
		}

		public IEnumerable<string> GetChangedFields()
		{
			return changedFields;
		}

		public bool HasUnsavedChanges
		{
			get
			{
				return hasUnsavedChanges;
			}
			set
			{
				if (value == HasUnsavedChanges)
					return;

				if (value)
				{
					hasUnsavedChanges = true;
				}
				else
				{
					changedFields.Clear();
					hasUnsavedChanges = false;
				}
			}
		}

		public bool ExistsInDatabase { get; set; }
	}
}
