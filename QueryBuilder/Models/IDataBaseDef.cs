using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public interface IDatabaseDef
	{
		string Name { get; set; }
		string Description { get; set; }
		//List<ITableDef> TableDefs { get; }
		ITableDef[] TableDefs { get; }
		ITableDef AddTable(string tableName);
		ITableDef GetTable(string tableName);
	}
}
