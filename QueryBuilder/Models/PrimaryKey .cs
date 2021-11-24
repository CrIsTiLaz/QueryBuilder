using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public class PrimaryKey : IPrimaryKey
	{
		private string name;
		private IColumnDef[] columns;
		private ITableDef relatedTable;
		public PrimaryKey(string name, IColumnDef[] columns)
		{
			this.name = name;
			this.columns = columns;

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

		public IColumnDef[] Columns
		{
			get
			{
				return columns.ToArray();
			}
		}

		public ITableDef RelatedTable
		{
			get
			{
				return relatedTable;
			}

			set
			{
				relatedTable = value;
			}
		}
	}
}
