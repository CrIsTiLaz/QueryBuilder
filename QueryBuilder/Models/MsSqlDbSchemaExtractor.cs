using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using QueryBuilder.Models;
//using QueryBuilder.AppMessage;
using System.IO;
using QueryBuilder.Models.Log;

namespace Q.Core.Db.Tools.Impl.MsSqlServer.MetadataExtractor
{
	public class MsSqlDbSchemaExtractor
	{
		public const string SCHEMA = "dbo";
		public const String TABLE_NAME = "TABLE_NAME";
		public Logger logger = new Logger();

		public DatabaseDef ExtractMetadata(string connectionString)
		{
			return ExtractMetadata(connectionString, null);
		}

		public DatabaseDef ExtractMetadata(string connectionString, string[] tableNames)
		{
			SqlConnection con = new SqlConnection(connectionString);
			SqlCommand command = null;
			DatabaseDef dbMetadata = new DatabaseDef(connectionString);
			//var logger = new Logger();

			try
			{
				con.Open();
				command = con.CreateCommand();
				ExtractTables(dbMetadata, command, tableNames);
				//var logger = new Logger();
				
			}
			catch (Exception ex)
			{
				logger.Log(Path.Combine("Exception: message=[{0}] stackTrace=[{1}]", ex.Message, ex.StackTrace));
				throw;

			}
			finally
			{
				if (con.State == ConnectionState.Open)
				{
					logger.Log("closing the connection");
					con.Close();
				}
			}

			return dbMetadata;
		}

		public DatabaseDef ExtractTables(DatabaseDef schema, SqlCommand command, string[] tableNames)
		{
			//reinitialize the counters because this method can be called multiple times

			command.CommandText = @"select TABLE_NAME, IDENT_SEED(TABLE_NAME) AS IDENT_SEED, IDENT_INCR(TABLE_NAME) AS IDENT_INCR from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME <> 'sysdiagrams'";
			command.CommandType = CommandType.Text;
			logger.Log("Extract tables; executing query: " + command.CommandText);
			//SendMessage("Extract tables; executing query: " + command.CommandText, AppMessageLevel.TRACE);

			SqlDataReader reader = command.ExecuteReader();
			if (!reader.HasRows)
			{
				reader.Close();
				logger.Log("There are no rows.Will return ");
				//SendMessage("There are no rows. Will return", AppMessageLevel.TRACE);
				return schema;
			}
			Hashtable identityTable = new Hashtable();

			if (tableNames != null)
			{
				//add the tables from tableNames to the schema (the order is esential)
				for (int i = 0; i < tableNames.Length; i++)
				{
					schema.AddTable(tableNames[i]);
				}
			}

			//if there are tables in the list which do not belongs to the db, will remove them
			//create a hashtable and add the tables
			Hashtable tablesInDb = new Hashtable();

			while (reader.Read())
			{
				string tableName = reader.GetString(reader.GetOrdinal("TABLE_NAME"));
				//add the tableName to the hashtbl
				tablesInDb.Add(tableName, tableName);
				//if the table is not in the tableNames then add it to the schema at the last position
				if(!schema.ContainsTable(tableName))
				{
					schema.AddTable(tableName);
					logger.Log(String.Format("tbl [{0}]", tableName));
					//SendMessage(String.Format("tbl [{0}]", tableName), AppMessageLevel.TRACE);
				}


				if (!reader.IsDBNull(reader.GetOrdinal("IDENT_SEED")))
				{
					IdentityInfo iinfo = new IdentityInfo();
					string s = reader.GetDataTypeName(reader.GetOrdinal("IDENT_SEED"));
					iinfo.Seed = reader.GetDecimal(reader.GetOrdinal("IDENT_SEED"));
					iinfo.Increment = reader.GetDecimal(reader.GetOrdinal("IDENT_INCR"));
					identityTable.Add(tableName, iinfo);
					logger.Log(String.Format("Table [{0}] contain identity field", tableName));
					//SendMessage(String.Format("Table [{0}] contain identity field", tableName), AppMessageLevel.TRACE);
				}
			}

			//now remove tables which are in tableList but do not exist in the db
			if (tableNames != null)
			{
				for (int i = 0; i < tableNames.Length; i++)
				{
					if (!tablesInDb.ContainsKey(tableNames[i]))
					{
						schema.DeleteTable(tableNames[i]);
					}
				}
			}
			tablesInDb.Clear();
			tablesInDb = null;
			//remove done

			reader.Close();
			CreateColumnsUsingSp_Columns(schema, command, identityTable);

			foreach (TableDef tbl in schema.tableDefs)
			{
				if (tbl.columns.Count == 0)
				{
					continue;//cristi: this is a hack
				}
				logger.Log(String.Format("[{0}].pk", tbl.Name));
				//SendMessage(String.Format("[{0}].pk", tbl.Name), AppMessageLevel.TRACE);
				try
				{
					CreatePrimaryKey(tbl, command);
				}
				catch (Exception ex)
				{
					logger.Log(String.Format(@"
					Exception:
					message=[{0}]
					stackTrace=[{1}]", ex.Message, ex.StackTrace));
					//SendMessage(String.Format(@"
					//Exception:
					//message=[{0}]
					//stackTrace=[{1}]", //ex.Message, ex.StackTrace), AppMessageLevel.VALIDATION);
				}
			}

			foreach (TableDef tbl in schema.tableDefs)
			{
				if (tbl.columns.Count == 0)
				{
					continue;
				}
				logger.Log(String.Format("[{0}].fk", tbl.Name));
				//SendMessage(String.Format("[{0}].fk", tbl.Name), AppMessageLevel.TRACE);
				try
				{
					CreateForeignKey(schema, tbl, command);
				}
				catch (Exception ex)
				{
					logger.Log(String.Format(@"
								Exception:
								message=[{0}]
								stackTrace=[{1}]", ex.Message, ex.StackTrace));
					//SendMessage(String.Format(@"
//Exception:
//message=[{0}]
//stackTrace=[{1}]", ex.Message, ex.StackTrace), AppMessageLevel.VALIDATION);
				}
			}

			foreach (TableDef tbl in schema.tableDefs)
			{
				if (tbl.columns.Count == 0)
				{
					continue;
				}
				logger.Log(String.Format("[{0}].unq", tbl.Name));
				//SendMessage(String.Format("[{0}].unq", tbl.Name), AppMessageLevel.TRACE);
				try
				{
					CreateUniques(schema, tbl, command);
				}
				catch (Exception ex)
				{
					logger.Log(String.Format(@"
					Exception:
					message=[{0}]
					stackTrace=[{1}]", ex.Message, ex.StackTrace));
//					SendMessage(String.Format(@"
//Exception:
//message=[{0}]
//stackTrace=[{1}]", ex.Message, ex.StackTrace), AppMessageLevel.VALIDATION);
				}
			}
			return schema;
		}


		private void CreateColumnsUsingSp_Columns(DatabaseDef schema, SqlCommand command, Hashtable identityTable)
		{
			foreach (TableDef tbl in schema.tableDefs)
			{
				command.CommandText = @"exec sp_columns @table_name = '" + tbl.Name + "'";
				command.CommandType = CommandType.Text;
				logger.Log("Execute: " + command.CommandText);
				//SendMessage("Execute: " + command.CommandText, AppMessageLevel.TRACE);

				SqlDataReader reader = command.ExecuteReader();
				if (!reader.HasRows)
				{
					reader.Close();
					continue;
				}

				while (reader.Read())
				{
					string tableName = reader.GetString(reader.GetOrdinal("TABLE_NAME"));
					string columnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
					string dataType = reader.GetString(reader.GetOrdinal("TYPE_NAME"));
					bool isIdentity = false;
					IdentityInfo iiInfo = null;
					if (dataType.IndexOf(' ') > 0) //for example "bigint identity"
					{
						dataType = dataType.Split(new char[] { ' ' }, 2)[0];
						isIdentity = true;

						iiInfo = (IdentityInfo)identityTable[tableName];

					}

					short isNullableByte = reader.GetInt16(reader.GetOrdinal("NULLABLE"));
					bool isNullable = isNullableByte == 1 ? true : false;
					int maxCharLength = -1;

					if (!reader.IsDBNull(reader.GetOrdinal("LENGTH")))
					{
						maxCharLength = reader.GetInt32(reader.GetOrdinal("LENGTH"));
						//please note:
						//	the length is the the size in bytes not the dedined length for varChar or nvarchar
						//	the real length is the precision field
					}

					int numericPrecision = 0;
					short numericScale = 0;

					if (!reader.IsDBNull(reader.GetOrdinal("PRECISION")))
					{
						string s = reader.GetDataTypeName(reader.GetOrdinal("PRECISION"));
						numericPrecision = reader.GetInt32(reader.GetOrdinal("PRECISION"));
						maxCharLength = numericPrecision; //see the notes from LENGTH
					}

					if (!reader.IsDBNull(reader.GetOrdinal("SCALE")))
					{
						string s = reader.GetDataTypeName(reader.GetOrdinal("SCALE"));
						numericScale = reader.GetInt16(reader.GetOrdinal("SCALE"));
					}

					string defaultValue = null;

					if (!reader.IsDBNull(reader.GetOrdinal("COLUMN_DEF")))
					{
						string s = reader.GetDataTypeName(reader.GetOrdinal("COLUMN_DEF"));
						defaultValue = reader.GetString(reader.GetOrdinal("COLUMN_DEF"));
					}
					logger.Log(String.Format("[{0}].[{1}]", tableName, columnName));
					//SendMessage(String.Format("[{0}].[{1}]", tableName, columnName), AppMessageLevel.TRACE);
					try
					{			
							CreateColumn(schema, tableName, columnName, dataType, maxCharLength, isNullable, numericPrecision, numericScale, isIdentity, iiInfo, defaultValue);
					}
					catch (Exception ex)
					{
						logger.Log(String.Format(@"
									Exception:
									message=[{0}]
									stackTrace=[{1}]", ex.Message, ex.StackTrace));
//						SendMessage(String.Format(@"
//Exception:
//message=[{0}]
//stackTrace=[{1}]", ex.Message, ex.StackTrace), AppMessageLevel.VALIDATION);
					}
				}
				reader.Close();
			}
		}

		private void CreateColumn(DatabaseDef dbSchema, string tableName, string columnName, string dataType, int maxCharLength, bool isNullable, int numericPrecision, int numericScale, bool isIdentity, IdentityInfo iiInfo, string defaultValue)
		{
			TableDef table = (TableDef)dbSchema.GetTable(tableName);
			DbType dbType = GetDbTypeForSqlType(dataType);

			ColumnDef column;
			switch (dbType)
			{
				case DbType.String:
					column = (ColumnDef)table.AddStringColumn(columnName, (int)maxCharLength, isNullable);
					break;

				case DbType.AnsiString:
					column = (ColumnDef)table.AddAnsiStringColumn(columnName, (int)maxCharLength, isNullable);
					break;

				case DbType.StringFixedLength:
					column = (ColumnDef)table.AddStringFixedLengthColumn(columnName, (int)maxCharLength, isNullable);
					break;

				case DbType.Boolean:
					column = (ColumnDef)table.AddBooleanColumn(columnName, isNullable);
					break;

				case DbType.Double:
					column = (ColumnDef)table.AddDoubleColumn(columnName, isNullable);
					break;

				case DbType.Decimal:
					column = (ColumnDef)table.AddDecimalColumn(columnName, Convert.ToByte(numericPrecision), Convert.ToByte(numericScale), isNullable);
					break;

				case DbType.Int64:
					if (isIdentity)
					{
						column = (ColumnDef)table.AddInt64IdentityColumn(columnName, Convert.ToInt32(iiInfo.Seed), Convert.ToInt32(iiInfo.Increment));
					}
					else
					{

						column = (ColumnDef)table.AddInt64Column(columnName, isNullable);
					}
					break;
				case DbType.Int32:
					if (isIdentity)
					{
						column = (ColumnDef)table.AddInt32IdentityColumn(columnName, Convert.ToInt32(iiInfo.Seed), Convert.ToInt32(iiInfo.Increment));
					}
					else
					{

						column = (ColumnDef)table.AddInt32Column(columnName, isNullable);
					}
					break;

				case DbType.Int16:
					if (isIdentity)
					{
						column = (ColumnDef)table.AddInt16IdentityColumn(columnName, Convert.ToInt32(iiInfo.Seed), Convert.ToInt32(iiInfo.Increment));
					}
					else
					{

						column = (ColumnDef)table.AddInt16Column(columnName, isNullable);
					}
					break;

				case DbType.Byte:
					column = (ColumnDef)table.AddByteColumn(columnName, isNullable);
					break;

				case DbType.Guid:
					column = (ColumnDef)table.AddGuidColumn(columnName, isNullable);
					break;
				case DbType.Binary:
					column = (ColumnDef)table.AddBinaryColumn(columnName, isNullable);
					break;
				case DbType.DateTime:
					column = (ColumnDef)table.AddDateTimeColumn(columnName, isNullable);
					break;
                case DbType.DateTime2:
                    column = (ColumnDef)table.AddDateTime2Column(columnName, isNullable);
                    break;
				case DbType.Date:
					column = (ColumnDef)table.AddDateColumn(columnName, isNullable);
					break;
				default:


					throw new ApplicationException("the dbType -" + dbType.ToString() + " is not implemented yet !");

			}

			column.DefaultValue = defaultValue;

		}

		private void CreatePrimaryKey(ITableDef table, SqlCommand command)
		{
			string sqlSelect = @"SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME = '" + table.Name + "' AND CONSTRAINT_TYPE = 'PRIMARY KEY'";

			command.CommandText = sqlSelect;
			command.CommandType = CommandType.Text;

			SqlDataReader reader = command.ExecuteReader();
			if (!reader.HasRows)
			{
				reader.Close();
				return;
			}

			reader.Read();
			string pkName = reader.GetString(reader.GetOrdinal("CONSTRAINT_NAME"));
			reader.Close();
			sqlSelect = "select COLUMN_NAME from INFORMATION_SCHEMA.KEY_COLUMN_USAGE where CONSTRAINT_NAME = '" + pkName + "' ORDER BY ORDINAL_POSITION ";

			command.CommandText = sqlSelect;
			reader = command.ExecuteReader();

			ArrayList pkColumnList = new ArrayList();
			string columnName = null;

			while (reader.Read())
			{
				columnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
				pkColumnList.Add(columnName);
			}
			String[] colList = new string[pkColumnList.Count];
			int i = 0;
			foreach (string columnNameString in pkColumnList)
			{
				colList[i++] = columnNameString;
			}
			table.AddPrimaryKeyConstraint(pkName, colList);
			reader.Close();

		}

		private void CreateForeignKey(IDatabaseDef schema, ITableDef table, SqlCommand command)
		{

			string sqlSelect = @"SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME = '" + table.Name + "' AND CONSTRAINT_TYPE = 'FOREIGN KEY'";
			command.CommandText = sqlSelect;
			command.CommandType = CommandType.Text;

			ArrayList fkNameList = new ArrayList();

			SqlDataReader reader = command.ExecuteReader();
			if (!reader.HasRows)
			{
				reader.Close();
				return;
			}

			while (reader.Read())
			{
				string fkName = reader.GetString(reader.GetOrdinal("CONSTRAINT_NAME"));
				fkNameList.Add(fkName);
			}
			reader.Close();


			//read the columns for each fk
			foreach (String fkName in fkNameList)
			{
				ArrayList fkColumnList = new ArrayList();
				sqlSelect = "select COLUMN_NAME from INFORMATION_SCHEMA.KEY_COLUMN_USAGE where CONSTRAINT_NAME = '" + fkName + "' ORDER BY ORDINAL_POSITION";
				command.CommandText = sqlSelect;
				reader = command.ExecuteReader();

				while (reader.Read())
				{
					string columnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
					fkColumnList.Add(columnName);
				}
				string[] colList = new string[fkColumnList.Count];
				int i = 0;
				foreach (string columnNameString in fkColumnList)
				{
					colList[i++] = columnNameString;
				}

				reader.Close();
				string referencedTableName = GetReferencedTableNameForForeignKey(fkName, command);
				ITableDef referencedTable = (TableDef)schema.GetTable(referencedTableName);
				table.AddForeignKeyConstraint(fkName, referencedTable, colList, true);
			}
		}

		private void CreateUniques(IDatabaseDef schema, TableDef table, SqlCommand command)
		{
			string sqlSelect = @"SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME = '" + table.Name + "' AND CONSTRAINT_TYPE = 'UNIQUE'";
			command.CommandText = sqlSelect;
			command.CommandType = CommandType.Text;
			List<String> unqNameList = new List<String>();

			SqlDataReader reader = command.ExecuteReader();
			if (!reader.HasRows)
			{
				reader.Close();
				return;
			}

			while (reader.Read())
			{
				string unqName = reader.GetString(reader.GetOrdinal("CONSTRAINT_NAME"));
				unqNameList.Add(unqName);
			}
			reader.Close();

			//read the columns for each fk
			foreach (String unqName in unqNameList)
			{
				ArrayList unqColumnList = new ArrayList();
				sqlSelect = "select COLUMN_NAME from INFORMATION_SCHEMA.KEY_COLUMN_USAGE where CONSTRAINT_NAME = '" + unqName + "' ORDER BY ORDINAL_POSITION";
				command.CommandText = sqlSelect;
				reader = command.ExecuteReader();

				while (reader.Read())
				{
					string columnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
					unqColumnList.Add(columnName);
				}
				ColumnDef[] colList = new ColumnDef[unqColumnList.Count];
				int i = 0;
				foreach (string columnNameString in unqColumnList)
				{
					colList[i++] = (ColumnDef)table.GetColumn(columnNameString);
				}

				reader.Close();
				table.AddUniqueConstraint(unqName, colList);
			}
		}

		private string GetReferencedTableNameForForeignKey(string fkName, SqlCommand command)
		{
			string sqlSelect = @"SELECT UNIQUE_CONSTRAINT_NAME FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = '" + fkName + "'";
			command.CommandText = sqlSelect;
			command.CommandType = CommandType.Text;

			SqlDataReader reader = command.ExecuteReader();
			if (!reader.HasRows)
			{
				reader.Close();
				return null;
			}

			reader.Read();
			string relatedPkName = reader.GetString(reader.GetOrdinal("UNIQUE_CONSTRAINT_NAME"));
			reader.Close();

			sqlSelect = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE WHERE CONSTRAINT_NAME = '" + relatedPkName + "'";
			command.CommandText = sqlSelect;
			command.CommandType = CommandType.Text;

			reader = command.ExecuteReader();
			if (!reader.HasRows)
			{
				reader.Close();
				return null;
			}

			reader.Read();
			string tblName = reader.GetString(reader.GetOrdinal("TABLE_NAME"));
			reader.Close();
			return tblName;
		}

		class IdentityInfo
		{
			public decimal Seed;
			public decimal Increment;
		}

		public class UserDefinedTypesInfo
		{
			public string name;
			public string sqlName;
			public short max_length;
			public byte precision;
			public byte scale;
			public bool is_nullable;
		}

		public class UserDefinedTableTypesInfo : UserDefinedTypesInfo
		{
			public string tblName;
		}

		public System.Data.DbType GetDbTypeForSqlType(string sqlType)
		{
			DbType result;
			string upperSqlType = sqlType.ToUpper();

			switch (upperSqlType)
			{
				case "NVARCHAR":
					result = DbType.String;
					break;

				case "VARCHAR":
					result = DbType.AnsiString;
					break;

				case "CHAR":
					result = DbType.AnsiStringFixedLength;
					break;
				case "NCHAR":
					result = DbType.StringFixedLength;
					break;

				case "TINYINT":
					result = DbType.Byte;
					break;

				case "SMALLINT":
					result = DbType.Int16;
					break;

				case "INT":
					result = DbType.Int32;
					break;

				case "BIGINT":
					result = DbType.Int64;
					break;

				case "FLOAT":
				case "DOUBLE":
					result = DbType.Double;
					break;

				case "DECIMAL":
				case "NUMERIC":
					result = DbType.Decimal;
					break;

				case "BIT":
					result = DbType.Boolean;
					break;

				case "DATETIME":
				case "TIMESTAMP":
					result = DbType.DateTime;
					break;

				case "DATETIME2":
					result = DbType.DateTime2;
					break;

				case "SMALLDATETIME":
				case "DATE":
					result = DbType.Date;
					break;

				case "UNIQUEIDENTIFIER":
					result = DbType.Guid;
					break;

				case "IMAGE":
					result = DbType.Binary;
					break;

				case "TEXT":
					result = DbType.AnsiString; //it can be varchar(max)
					break;

				case "NTEXT":
					result = DbType.String;//it can be nvarchar(max)
					break;

				default:
					throw new ApplicationException("Cannot find the DbType for:" + sqlType);
			}

			return result;
		}

		///// <summary>
		///// used to perform topological sort for dbItems
		///// </summary>
		//class DependencyInfo
		//{
		//    public string itemName;
		//    public List<String> referenceList = new List<string>();
		//}

		/// <summary>
		/// send messages using events
		/// </summary>
		/// <param name="message"></param>
		//private void SendMessage(string message, AppMessageLevel level)
		//{
		//	if (null != OnAppMessage)
		//	{
		//		OnAppMessage(this, new AppMessageArgs(message, level));
		//	}
		//}

	}
}
