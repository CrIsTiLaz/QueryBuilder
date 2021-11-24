using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public interface IUnique
	{
		string Name { get; set; }
		IColumnDef[] Columns{ get; }
		ITableDef RelatedTable { get; set; }
    }
}
