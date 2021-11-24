using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public interface ITableDef
	{
		string Name { get; set; }
		List<IColumnDef> Columns { get; }
		IPrimaryKey PrimaryKey { get; }
		IUnique[] Uniques { get; }
		IForeignKey[] ForeignKeys { get; }
		IDatabaseDef RelatedDb { get; }
		IColumnDef AddStringColumn(string name, int length, bool isNullable, bool computed = false, string computedExpression = "");
		IColumnDef AddStringFixedLengthColumn(string name, int length, bool isNullable);
		IColumnDef AddAnsiStringColumn(string name, int length, bool isNullable);
		IColumnDef AddByteColumn(string name, bool isNullable);
		IColumnDef AddBinaryColumn(string name, bool isNullable);
		IColumnDef AddInt16Column(string name, bool isNullable);
		IColumnDef AddInt16IdentityColumn(string name, int seed, int increment);
		IColumnDef AddInt32Column(string name, bool isNullable);
		IColumnDef AddInt32IdentityColumn(string name, int seed, int increment);
		IColumnDef AddInt64Column(string name, bool isNullable);
		IColumnDef AddInt64IdentityColumn(string name, int seed, int increment);
		IColumnDef AddDoubleColumn(string name, bool isNullable);
		IColumnDef AddDecimalColumn(string name, byte precision, byte scale, bool isNullable);
		IColumnDef AddBooleanColumn(string name, bool isNullable);
		IColumnDef AddDateColumn(string name, bool isNullable);
		IColumnDef AddDateTimeColumn(string name, bool isNullable);
		IColumnDef AddDateTime2Column(string name, bool isNullable);
		IColumnDef AddGuidColumn(string name, bool isNullable);
		IColumnDef AddGuidColumn(string name, bool isNullable, string defaultValue);
		IColumnDef AddComputedColumn(string name, DbType dbType, string expression);
		IPrimaryKey AddPrimaryKeyConstraint(string name, IColumnDef column);
		IPrimaryKey AddPrimaryKeyConstraint(IColumnDef column);
		IPrimaryKey AddPrimaryKeyConstraint(string name, IColumnDef[] columnList);
		IPrimaryKey AddPrimaryKeyConstraint(IColumnDef[] columnList);
		IPrimaryKey AddPrimaryKeyConstraint(string name, string[] columnsNameList);
		IForeignKey AddForeignKeyConstraint(ITableDef referencedTable, string nameForImportedColumn, bool allowNull);
		IForeignKey AddForeignKeyConstraint(string name, ITableDef referencedTable, string[] namesForImportedColumns, bool allowNull);
		IForeignKey AddForeignKeyConstraint(string name, ITableDef referencedTable, string nameForImportedColumn, bool allowNull);
		IUnique AddUniqueConstraint(string name, IColumnDef column);
        IUnique AddUniqueConstraint(IColumnDef column);
		IUnique AddUniqueConstraint(string name, IColumnDef[] columnList);
		IUnique AddUniqueConstraint(IColumnDef[] columnList);
	}
}
