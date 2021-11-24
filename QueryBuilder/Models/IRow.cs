using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public interface IRow
	{
		ITableDef RelatedTable { get; set; }
		object [] Values { get; }

	}
}
