using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public interface ITable
	{
		ITableDef RelatedTable { get; set; }
		List<IRow> Rows { get; }
    }
}
