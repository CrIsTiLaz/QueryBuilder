using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public interface IColumnDef
	{
		string Name { get; set; }
		DbType DataType { get; set; }	
		bool LengthDefined { get; set; }
		int Length { get; set; }
		bool PrecisionDefined { get; set; }
		byte Precision { get; set; }
		byte Scale { get; set; }
		bool IdentityDefined { get; set; }
		int IdentitySeed { get; set; }
		int IdentityIncrement { get; set; }
		string DefaultValue { get; set; }
		string CheckExpression { get; set; }
		bool Computed { get; set; }
		string ComputedExpression { get; set; }
		bool IsNullable { get; set; }
		ITableDef RelatedTable { get; set; }
	}
}
