using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public class DatabaseDef : IDatabaseDef
	{
		string name;
		string description;
		public List<ITableDef> tableDefs = new List<ITableDef>();

		public DatabaseDef(string name)
		{
			this.name = name;
		}
		public string Description
		{
			get
			{
				return description;
			}

			set
			{
				description = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				name = value;
			}
		}

		public ITableDef[] TableDefs
		{
			get
			{
				return tableDefs.ToArray();
			}
		}

		public ITableDef AddTable(string tableName)
		{
			var tableDef = GetTable(tableName);
			if (tableDef != null)
			{
				throw new ApplicationException(string.Format("Table {0} allready exist", tableName));
			}
			tableDef = new TableDef(tableName, this);
			tableDefs.Add(tableDef);
			return tableDef;
		}

		public ITableDef GetTable(string tableName)
		{
			foreach (var tblDef in tableDefs)
			{
				if (tblDef.Name == tableName)
				{
					return tblDef;
				}
			}
			return null;
		}

		public bool ContainsTable(string tableName)
		{
			return null != GetTable(tableName);
			//var x = GetTable(tableName);
			//return x != null;

		}

		public void DeleteTable(string tableName)
		{
			var tbl = GetTable(tableName);
			if (tbl != null)
			{
				tableDefs.Remove(tbl);
			}
		}
	}
}
