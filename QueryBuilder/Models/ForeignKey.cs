using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public class ForeignKey : IForeignKey
	{
		private string name;
		private IColumnDef[] columns;
		private ITableDef relatedTable;
		private ITableDef referencedTable;
		public ForeignKey(string name, IColumnDef[] columns, ITableDef relatedTable, ITableDef referencedTable)
		{
			this.name = name;
			this.columns = columns;
			this.relatedTable = relatedTable;
			this.referencedTable = referencedTable;
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

		public ITableDef ReferencedTable
		{
			get
			{
				return referencedTable;
			}
		}

	}
}
