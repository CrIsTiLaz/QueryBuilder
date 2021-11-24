using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryBuilder.Models
{
	public class ColumnDef : IColumnDef
	{
		private string name;
		private bool lengthDefined = false;
		private int length;
		private bool precisionDefined = false;
		private byte precision;
		private byte scale;
		private bool identityDefined = false;
		private int identitySeed = 1;
		private int identityIncrement = 1;
		private string defaultValue = null;
		private string checkExpression = null;
		private bool computed = false;
		private string computedExpression = string.Empty;
		bool isNullable;
		DbType dataType;
		ITableDef relatedTable;
		
		public ColumnDef(string name, DbType dataType, int lenght, bool isNullable, ITableDef relatedTab)
		{
			this.name = name;
			this.dataType = dataType;
			this.lengthDefined = true;
			this.length = lenght;
			this.isNullable = isNullable;
			this.relatedTable = relatedTab;
		}
		public ColumnDef(string name, DbType dataType, ITableDef relatedTable)
		{
			this.name = name;
			this.dataType = dataType;
			this.relatedTable = relatedTable;
		}
		public ColumnDef(string name, DbType dataType, byte precision, byte scale, ITableDef relatedTab)
		{
			this.name = name;
			this.dataType = dataType;
			this.precisionDefined = true;
			this.precision = precision;
			this.scale = scale;
			this.relatedTable = relatedTab;
		}
		public ColumnDef(string name, DbType dataType, int seed, int increment, ITableDef relatedTab)
		{
			this.name = name;
			this.dataType = dataType;
			this.identityDefined = true;
			this.identitySeed = seed;
			this.identityIncrement = increment;
			this.relatedTable = relatedTab;
		}
		public ColumnDef(string name, DbType dataType, ITableDef relatedTab, string computedExpression)
		{
			this.name = name;
			this.dataType = dataType;
			this.relatedTable = relatedTab;
			this.computedExpression = computedExpression;
			this.computed = true;
		}

		public string CheckExpression
		{
			get
			{
				return checkExpression;
			}

			set
			{
				checkExpression = value;
            }
		}

		public bool Computed
		{
			get
			{
				return computed;
			}

			set
			{
				computed = value;
            }
		}

		public string ComputedExpression
		{
			get
			{
				return computedExpression;
			}

			set
			{
				computedExpression = value;
            }
		}

		public DbType DataType
		{
			get
			{
				return dataType;
            }

			set
			{
				dataType = value;
            }
		}

		public string DefaultValue
		{
			get
			{
				return defaultValue;
            }

			set
			{
				defaultValue = value;
            }
		}

		public bool IdentityDefined
		{
			get
			{
				return identityDefined;
			}

			set
			{
				identityDefined = value;
			}
		}

		public int IdentityIncrement
		{
			get
			{
				return identityIncrement;
			}

			set
			{
				identityIncrement = value; ;
			}
		}

		public int IdentitySeed
		{
			get
			{
				return identitySeed;
            }

			set
			{
				identitySeed = value;
			}
		}

		public int Length
		{
			get
			{
				return length;
			}

			set
			{
				length = value;
            }
		}

		public bool LengthDefined
		{
			get
			{
				return lengthDefined;
            }

			set
			{
				lengthDefined = value;
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

		public byte Precision
		{
			get
			{
				return precision;
			}

			set
			{
				precision = value;
            }
		}

		public bool PrecisionDefined
		{
			get
			{
				return precisionDefined;
			}

			set
			{
				precisionDefined = value;
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

		public byte Scale
		{
			get
			{
				return scale;
			}

			set
			{
				scale = value;
            }
		}

		public bool IsNullable
		{
			get
			{
				return isNullable;
			}

			set
			{
				isNullable = value;
            }
		}
	}
}
