using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public class TableDef : ITableDef
	{
		private string name;
		public List<IColumnDef> columns = new List<IColumnDef>();
		private IPrimaryKey primaryKey;
		private List<IUnique> uniques = new List<IUnique>();
		private List<IForeignKey> foreignKeys = new List<IForeignKey>();
		private IDatabaseDef relatedDb;
		public TableDef(string name, IDatabaseDef relatedDb)
		{
			this.name = name;
			this.relatedDb = relatedDb;
		}
		public List<IColumnDef> Columns
		{
			get
			{
				return columns;
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

		public IPrimaryKey PrimaryKey
		{
			get
			{
				return primaryKey;
			}
		}

		public IUnique[] Uniques
		{
			get
			{
				return uniques.ToArray();
			}
		}

		public IForeignKey[] ForeignKeys
		{
			get
			{
				return foreignKeys.ToArray();
			}
		}

		public IDatabaseDef RelatedDb
		{
			get
			{
				return relatedDb;
			}
		}

		public IColumnDef AddAnsiStringColumn(string name, int length, bool isNullable)
		{
			IColumnDef col = new ColumnDef(name, DbType.AnsiString, length, isNullable, this);
			columns.Add(col);
			col.RelatedTable = this;
			return col;
		}

		public IColumnDef AddStringColumn(string name, int length, bool isNullable, bool computed = false, string computedExpression = "")
		{
			IColumnDef column = new ColumnDef(name, DbType.String, length, isNullable, this);
			column.Computed = computed;
			column.ComputedExpression = computedExpression;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddStringFixedLengthColumn(string name, int length, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.StringFixedLength, length, isNullable, this);
			columns.Add(column);
			return column;
		}
		//public static string TestSql()
		//{
		//	var t = new TableDef("test");
		//	var fnc = t.AddAnsiStringColumn("FirstName", 50, false);
		//	var lnc = t.AddAnsiStringColumn("LastName", 100, false);
		//	t.AddPrimaryKeyConstraint(new IColumnDef[] { fnc, lnc });
		//	t.AddAnsiStringColumn("ProfileId", 90, false);
		//	t.AddAnsiStringColumn("NextConfirmation", 10, true);
		//	t.AddBooleanColumn("Bool", true);
		//	t.AddDateColumn("DateCol", false);
		//	t.AddDecimalColumn("DecimalCol", 10, 5, false);
		//	t.AddDoubleColumn("DoubleCol", true);
		//	t.AddBinaryColumn("BinaryCol", true);
		//	//t.AddComputedColumn("ComputedCol", DbType.AnsiString, "FirstName+LastName");
		//	t.AddDateTime2Column("DateTime2Col", false);
		//	t.AddGuidColumn("GuidCol", false);
		//	t.AddInt16Column("int16Col", true);
		//	t.AddInt32Column("int32Col", true);
		//	//t.AddInt32IdentityColumn("int32IdentityCol", 2, 1);
		//	t.AddInt64Column("int64Col", false);
		//	t.AddInt64IdentityColumn("int64IdenCol", 0, 1);
		//	t.AddStringColumn("stringCol", 12, true);
		//	t.AddStringColumn("stringColComputed", 12, true, true, "expresion to compute");
		//	t.AddStringFixedLengthColumn("stringFixLenCol", 32, true);
		//	t.AddUniqueConstraint(fnc);
		//	t.AddUniqueConstraint(lnc);
		//	t.AddUniqueConstraint(new IColumnDef[] { fnc, lnc });
		//	return CreateTable(t);
		//}

		/// <summary>
		/// Add a column coresponding to DbType.Byte
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isNullable"></param>
		/// <returns></returns>
		public IColumnDef AddByteColumn(string name, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.Byte, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddBinaryColumn(string name, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.Binary, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddInt16Column(string name, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.Int16, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddInt16IdentityColumn(string name, int seed, int increment)
		{
			IColumnDef column = new ColumnDef(name, DbType.Int16, seed, increment, this);
			column.IsNullable = false;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddInt32Column(string name, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.Int32, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddInt32IdentityColumn(string name, int seed, int increment)
		{
			IColumnDef column = new ColumnDef(name, DbType.Int32, seed, increment, this);
			column.IsNullable = false;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddInt64Column(string name, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.Int64, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddInt64IdentityColumn(string name, int seed, int increment)
		{
			IColumnDef column = new ColumnDef(name, DbType.Int64, seed, increment, this);
			column.IsNullable = false;
			columns.Add(column);
			//if the sequence is not specified then create a default one
			//Sequence defaultSeq = this.Database.AddSequence("seq_" + this.name, seed, increment);
			//column.ColumnType.SequenceName = defaultSeq.Name;

			//string triggerName = "BI_" + this.name + "_" + defaultSeq.Name;
			//Trigger trigger = new Trigger(triggerName, Trigger.TriggerType.forInsert);
			//trigger.Table = this;
			//this.triggerForSequenceList.Add(trigger);
			return column;
		}

		//public IColumnDef AddInt64IdentityColumn(string name, int seed, int increment, string sequenceName)
		//{
		//	if (!this.databaseSchema.sequenceList.Contains(sequenceName))
		//	{
		//		throw new ApplicationException("the specified sequence does not exist:" + sequenceName);
		//	}
		//	IColumnDef column = new ColumnDef(name, DbType.Int64, seed, increment, sequenceName), this);
		//	column.IsNullable = false;
		//	AddColumn(column);
		//	//now create a trigger for that column
		//	string triggerName = "BI_" + this.name + "_" + sequenceName;
		//	Trigger trigger = new Trigger(triggerName, Trigger.TriggerType.forInsert);
		//	trigger.Table = this;
		//	this.triggerForSequenceList.Add(trigger);
		//	return column;
		//}

		//public IColumnDef AddInt64SequenceColumn(string name, string sequenceName)
		//{
		//	if (!this.databaseSchema.sequenceList.Contains(sequenceName))
		//	{
		//		throw new ApplicationException("the specified sequence does not exist:" + sequenceName);
		//	}
		//	return AddInt64SequenceColumn(name, this.databaseSchema.sequenceList[sequenceName]);
		//}

		//public IColumnDef AddInt64SequenceColumn(string name, Sequence sequence)
		//{
		//	//if (!this.databaseSchema.sequenceList.Contains(sequence.Name))
		//	//{
		//	//	throw new ApplicationException("the specified sequence does not exist:" + sequence.Name);
		//	//}

		//	IColumnDef column = new ColumnDef(name, DbType.Int64, this);
		//	column.IsNullable = false;
		//	column.RelatedSequence = sequence;
		//	columns.Add(column);
		//	return column;
		//}

		public IColumnDef AddDoubleColumn(string name, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.Double, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddDecimalColumn(string name, byte precision, byte scale, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.Decimal, precision, scale, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddBooleanColumn(string name, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.Boolean, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddDateColumn(string name, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.Date, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}

		public IColumnDef AddDateTimeColumn(string name, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.DateTime, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}
		public IColumnDef AddDateTime2Column(string name, bool isNullable)
		{
			IColumnDef column = new ColumnDef(name, DbType.DateTime2, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			return column;
		}
		public IColumnDef AddGuidColumn(string name, bool isNullable)
		{
			return AddGuidColumn(name, isNullable, null);
		}

		public IColumnDef AddGuidColumn(string name, bool isNullable, string defaultValue)
		{
			IColumnDef column = new ColumnDef(name, DbType.Guid, this);
			column.IsNullable = isNullable;
			columns.Add(column);
			if (defaultValue != null)
			{
				column.DefaultValue = defaultValue;
			}
			return column;
		}

		public IColumnDef AddComputedColumn(string name, DbType dbType, string expression)
		{
			IColumnDef column = new ColumnDef(name, dbType, this, expression);
			columns.Add(column);
			return column;
		}


		public static string CreateTable(ITableDef tblDef)
		{
			var sql = new StringBuilder();
			sql.AppendFormat("CREATE TABLE [dbo].[{0}] (", tblDef.Name)
				.Append(Environment.NewLine);
			var first = true;
			for (int i = 0; i < tblDef.Columns.Count; i++)
			{
				if (!first)
				{
					sql.Append(",")
						.Append(Environment.NewLine);
                }
				first = false;
				sql.Append(GetStatementForColumnCreation(tblDef.Columns[i], false));
						

				//if (i == tblDef.Columns.Count - 1)
				//{
				//	//sql.Append("\t[" + tblDef.Columns[i].Name + "] [nvarchar](250)")
				//	//	.Append(Environment.NewLine)
				//	//	.Append(")");

				//	sql.Append(GetStatementForColumnCreation(tblDef.Columns[i], false))
				//		.Append(Environment.NewLine)
				//		.Append(")");

				//}
				//else
				//{
				//	//sql.Append("\t[" + tblDef.Columns[i].Name + "] [nvarchar](250) ,")
				//	//.Append(Environment.NewLine);
				//	sql.Append(GetStatementForColumnCreation(tblDef.Columns[i], false))
				//		.Append(",")
				//		.Append(Environment.NewLine);

			}
			if (tblDef.PrimaryKey != null)
			{
				sql.Append(GetStatementForPrimaryKeyCreation(tblDef.PrimaryKey, false));
			}
			if (tblDef.Uniques.Length > 0)
			{
				for (int i = 0; i < tblDef.Uniques.Length; i++)
				{
					sql.Append(GetStatementForUniqueCreation(tblDef.Uniques[i], false));
				}
				
			}
			if (tblDef.ForeignKeys.Length > 0)
			{
				for (int i = 0; i < tblDef.ForeignKeys.Length; i++)
				{
					sql.Append(GetStatementForForeignKeyCreation(tblDef.ForeignKeys[i], false));
				}

			}
			sql.Append(")");
			return sql.ToString();
		}
		public static string GetStatementForPrimaryKeyCreation(IPrimaryKey pk, bool pretty)
		{
			var t = "";
			for (int i = 0; i < pk.Columns.Length; i++)
			{
				t = t + pk.Columns[i].Name;
				if (i != pk.Columns.Length-1)
				{
					t = t + ",";
				}
			}
			var primaryKeyStatement = new StringBuilder("," + Environment.NewLine);
			primaryKeyStatement.Append("CONSTRAINT ")
								.AppendFormat("{0} PRIMARY KEY ({1})", pk.Name, t);
			return primaryKeyStatement.ToString();
								
		}
		public static string GetStatementForUniqueCreation(IUnique unq, bool pretty)
		{
			var t = "";
			for (int i = 0; i < unq.Columns.Length; i++)
			{
				t = t + unq.Columns[i].Name;
				if (i != unq.Columns.Length - 1)
				{
					t = t + ",";
				}
			}
			var primaryKeyStatement = new StringBuilder("," + Environment.NewLine);
			primaryKeyStatement.Append("CONSTRAINT ")
								.AppendFormat("{0} UNIQUE ({1})", unq.Name, t);
			return primaryKeyStatement.ToString();
		}
		public static string GetStatementForForeignKeyCreation(IForeignKey fk, bool pretty)
		{
			var t = "";
			for (int i = 0; i < fk.Columns.Length; i++)
			{
				t = t + fk.Columns[i].Name;
				if (i != fk.Columns.Length - 1)
				{
					t = t + ",";
				}
			}
			var s = new StringBuilder("," + Environment.NewLine);
			s.Append("CONSTRAINT ")
								.AppendFormat("{0} FOREIGN KEY ({1})", fk.Name, t);
			t = "";
			var cols = fk.ReferencedTable.PrimaryKey.Columns;
            for (int i = 0; i < cols.Length; i++)
			{
				t = t + cols[i].Name;
				if (i != cols.Length - 1)
				{
					t = t + ",";
				}
			}
			s.AppendFormat("REFERENCES {0}({1})", fk.ReferencedTable.Name, t);
            return s.ToString();
		}
		public static string GetStatementForColumnCreation(IColumnDef column, /*INamesGenerator namesGenerator,*/ bool pretty)
		{
			StringBuilder columnType = new StringBuilder(GetSqlTypeForDbType(column.DataType));
			StringBuilder columnDefault = new StringBuilder();
			if (column.DefaultValue != null)
			{
				//if (column.Table.Database.IsReverseEng)
				//{
				//	columnDefault.Append(column.DefaultValue);
				//}
				//else
				//{
				columnDefault.Append("(").Append(column.DefaultValue).Append(")");
				//	}
			}



			//if (column.RelatedSequence != null)
			//{
			//	columnDefault.Append("(").AppendFormat("NEXT VALUE FOR {0}", namesGenerator.GenerateSequenceName(column.RelatedSequence.Name)).Append((")"));
			//}



			switch (column.DataType)
			{
				case DbType.String:
				case DbType.StringFixedLength:
					if (column.Length >= 4000)
					{
						columnType.Append("(").Append("max").Append(")");
					}
					else
					{
						columnType.Append("(").Append(column.Length).Append(")");
					}
					break;
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
					//columnType.Append("(").Append(column.Length).Append(")");
					if (column.Length >= 8000)
					{
						columnType.Append("(").Append("max").Append(")");
					}
					else
					{
						columnType.Append("(").Append(column.Length).Append(")");
					}
					break;



				case DbType.Byte:
					break;



				case DbType.Int16:
					if (column.IdentityDefined)
					{
						columnType.Append(" IDENTITY (").Append(column.IdentitySeed).Append(',')
							.Append(column.IdentityIncrement).Append(" )");
					}
					break;



				case DbType.Int32:
					if (column.IdentityDefined)
					{
						columnType.Append(" IDENTITY(").Append(column.IdentitySeed).Append(',')
							.Append(column.IdentityIncrement).Append(") ");
					}
					break;



				case DbType.Int64:
					if (column.IdentityDefined)
					{
						columnType.Append(" IDENTITY (").Append(column.IdentitySeed).Append(',')
							.Append(column.IdentityIncrement).Append(") ");
					}
					break;



				case DbType.Double:
					//if (column.DefaultValue != null)
					//{
					//    columnDefault.Append("(").Append(column.DefaultValue).Append(")");
					//}
					break;



				case DbType.Decimal:
					columnType.Append("(").Append(column.Precision).Append(",").Append(column.Scale).Append(")");
					break;



				case DbType.Boolean:
					break;



				case DbType.Date:
				case DbType.DateTime:
				case DbType.DateTime2:
					break;



				case DbType.Guid:
					break;



				case DbType.Binary:
					break;



				default:
					columnDefault.Append("error(GetStatementForColumnCreation) - DbType ").Append(column.DataType).Append(" is not implemented ");
					break;
			}



			StringBuilder columnCheck = new StringBuilder();
			if (column.CheckExpression != null)
			{
				columnCheck.Append(" CHECK( ").Append(column.CheckExpression).Append(" )");
			}



			string result = null;
			if (pretty)
			{
				result = String.Format("{0, -50}{1, -30}{2, -10}{3, -20}{4, -10}",
									Normalize(column.Name),
									columnType.ToString(),
									column.IsNullable ? "NULL" : "NOT NULL",
									columnDefault.Length > 0 ? "DEFAULT " + columnDefault.ToString() : "",
									columnCheck.Length > 0 ? columnCheck.ToString() : ""
									);
			}
			else
			{
				result = String.Format("{0} {1} {2} {3} {4}",
									Normalize(column.Name),
									columnType.ToString(),
									column.IsNullable ? "NULL" : "NOT NULL",
									columnDefault.Length > 0 ? "CONSTRAINT dft_" + column.RelatedTable.Name + "_" + column.Name + " DEFAULT " + columnDefault.ToString() : "",
									columnCheck.Length > 0 ? columnCheck.ToString() : ""
									);
			}
			return result;
		}

		public static string GetSqlTypeForDbType(System.Data.DbType aDbType)
		{
			string result = null;



			switch (aDbType)
			{
				case DbType.String:
					result = "NVARCHAR";
					break;



				case DbType.StringFixedLength:
					result = "NCHAR";
					break;



				case DbType.AnsiString:
					result = "VARCHAR";
					break;



				case DbType.AnsiStringFixedLength:
					result = "CHAR";
					break;




				case DbType.Byte:
					result = "TINYINT";
					break;



				case DbType.Int16:
					result = "SMALLINT";
					break;



				case DbType.Int32:
					result = "INT";
					break;



				case DbType.Int64:
					result = "BIGINT";
					break;



				case DbType.Double:
					result = "FLOAT(53)";
					break;



				case DbType.Decimal:
					result = "DECIMAL";
					break;



				case DbType.Boolean:
					result = "BIT";
					break;



				case DbType.Date:
					result = "DATE";
					break;



				case DbType.DateTime:
					result = "DATETIME";
					break;



				case DbType.DateTime2:
					result = "DATETIME2";
					break;



				case DbType.Guid:
					result = "uniqueIdentifier";
					break;



				case DbType.Binary:
					result = "IMAGE";
					break;



				default:
					result = "error(GetSqlTypeForDbType) - DbType " + aDbType + " is not implemented ";
					break;
			}
			return result;
		}
		
		#region PrimaryKey
		public IPrimaryKey AddPrimaryKeyConstraint(IColumnDef column)
		{
			return AddPrimaryKeyConstraint("pk_" + this.Name, column);
		}

		public IPrimaryKey AddPrimaryKeyConstraint(IColumnDef[] columnList)
		{
			return AddPrimaryKeyConstraint("pk_" + this.Name, columnList);
		}
		public IPrimaryKey AddPrimaryKeyConstraint(string name, IColumnDef column)
		{
			return AddPrimaryKeyConstraint(name, new IColumnDef[] { column });
		}
		public IPrimaryKey AddPrimaryKeyConstraint(string name, IColumnDef[] columnList)
		{
			if (this.PrimaryKey != null)
			{
				throw new ApplicationException("Table contain already a primary key");
			}
			var nullableColumns = new List<string>();
			foreach (var column in columnList)
			{
				if (column.IsNullable)
				{
				
					nullableColumns.Add(column.Name);
				}
			}
			if (nullableColumns.Count > 0)
			{
				
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Cannot define PRIMARY KEY constraint on nullable column in table: {0} ", Name);
				sb.Append("[ ");
				for (int i = 0; i < nullableColumns.Count; i++)
				{
					sb.Append(nullableColumns[i]);
					if (i < nullableColumns.Count-1 )
					{
						sb.Append(", ");
					}					
				}
				sb.Append(" ]");
				throw new ApplicationException(sb.ToString());
			}

			IPrimaryKey primaryKey = new PrimaryKey(name, columnList);
			primaryKey.RelatedTable = this;
			this.primaryKey = primaryKey;
			return primaryKey;
		}
		public IPrimaryKey AddPrimaryKeyConstraint(string name, string[] columnsNameList)
		{
			ColumnDef[] columnList = new ColumnDef[columnsNameList.Length];
			for (int i = 0; i < columnsNameList.Length; i++)
			{
				ColumnDef column = (ColumnDef)this.columns[i];
				columnList[i] = column;
			}
			return AddPrimaryKeyConstraint(name, (IColumnDef)columns);
		}
		#endregion
		public static string Normalize(string sqlName)
		{
			return "[" + sqlName + "]";
		}

		public IUnique AddUniqueConstraint(string name, IColumnDef column)
		{
			return AddUniqueConstraint(name, new IColumnDef[] {column});
		}

		public IUnique AddUniqueConstraint(IColumnDef column)
		{
			return AddUniqueConstraint(new IColumnDef[] { column });
		}

		public IUnique AddUniqueConstraint(string name, IColumnDef[] columnList)
		{
			IUnique unq = new Unique(name, columnList);
			unq.RelatedTable = this;
			uniques.Add(unq);
			return unq;
		}

		public IUnique AddUniqueConstraint(IColumnDef[] columnList)
		{
			return AddUniqueConstraint("UNQ_" + uniques.Count.ToString() + name, columnList);
		}
		public IForeignKey AddForeignKeyConstraint(ITableDef referencedTable, string nameForImportedColumn, bool allowNull)
		{
			return AddForeignKeyConstraint("fk_" + this.name + "_" + nameForImportedColumn, referencedTable, new string[] { nameForImportedColumn }, allowNull);
		}
		public IForeignKey AddForeignKeyConstraint(string name, ITableDef referencedTable, string nameForImportedColumn, bool allowNull)
		{
			return AddForeignKeyConstraint(name, referencedTable, new string[] { nameForImportedColumn }, allowNull);
		}
		public IForeignKey AddForeignKeyConstraint(string name, ITableDef referencedTable, string[] namesForImportedColumns, bool allowNull)
		{
			//check if already exist the specified foreign key
			foreach (var fk in foreignKeys)
			{
				if (fk.Name == name)
				{
					throw new ApplicationException(" the foreign key having the name=" + name + " already exist");
				}
			}
			IPrimaryKey primaryKeyForReferencedTable = referencedTable.PrimaryKey;
			if (namesForImportedColumns.Length != primaryKeyForReferencedTable.Columns.Length)
			{
				throw new ApplicationException("number of columns from pk in referenced table and namesForImported columns are different !");
			}

			var columsForFk = new List<IColumnDef>();
			for (int i = 0; i < primaryKeyForReferencedTable.Columns.Length; i++)
			{
				IColumnDef sourceColumn = (IColumnDef)primaryKeyForReferencedTable.Columns[i];
				//please note that each new column will have a reference to sourceColumn.DataType - changed
				//there is a problem because it can be identity in parent table and must not be identity in child table
				//make a clone without 

				IColumnDef columnToCreate = new ColumnDef(namesForImportedColumns[i], sourceColumn.DataType, this);
				columnToCreate.IdentityDefined = false; //deactivate the identity field for imported columns
				columnToCreate.DefaultValue = null;//deactivate the default value for imported columns
				//columnToCreate.RelatedSequence = null;//deactivate the sequence for imported columns
				columnToCreate.IsNullable = allowNull;
				columns.Add(columnToCreate);
				columsForFk.Add(columnToCreate);
			}
			var foreignKey =  new ForeignKey(name, columsForFk.ToArray(), this, referencedTable);
			foreignKeys.Add(foreignKey);
			return foreignKey;
		}

		public IColumnDef GetColumn(string columnName)
		{
			foreach (var columnDef in columns)
			{
				if (columnDef.Name == columnName)
				{
					return columnDef;
				}
			}
			return null;
		}
	}
}
