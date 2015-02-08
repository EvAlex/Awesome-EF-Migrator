using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceSoft.Aquas.Model.Initializer.Models
{
	public class DbType
	{
		public DbType(string name, bool nullable, int? characterLength)
		{
			Type = ParseType(name);
			Nullable = nullable;
			CharacterLength = characterLength;
		}

		public SqlDbType Type { get; private set; }

		public bool Nullable { get; private set; }

		public int? CharacterLength { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}{1} {2}", 
				Type, 
				CharacterLength.HasValue ? "(" + CharacterLength + ")" : "", 
				Nullable ? "NULL" : "NOT NULL");
		}

		private SqlDbType ParseType(string name)
		{
			SqlDbType res;

            switch (name.ToLower())
			{
				case "int":
					res = SqlDbType.Int;
					break;
				case "float":
					res = SqlDbType.Float;
					break;
				case "decimal":
				case "numeric":
					res = SqlDbType.Decimal;
					break;
				case "real":
					res = SqlDbType.Real;
					break;
				case "bigint":
					res = SqlDbType.BigInt;
					break;
				case "tinyint":
					res = SqlDbType.TinyInt;
					break;
				case "smallint":
					res = SqlDbType.SmallInt;
					break;

				case "char":
					res = SqlDbType.Char;
					break;
				case "nchar":
					res = SqlDbType.NChar;
					break;
				case "varchar":
					res = SqlDbType.VarChar;
					break;
				case "geometry":
				case "geography":
				case "hierarchyid":
				case "sysname":
                case "nvarchar":
					res = SqlDbType.NVarChar;
					break;
				case "text":
					res = SqlDbType.Text;
					break;
				case "ntext":
					res = SqlDbType.NText;
					break;

				case "datetime":
					res = SqlDbType.DateTime;
					break;
				case "datetime2":
					res = SqlDbType.DateTime2;
					break;
				case "smalldatetime":
					res = SqlDbType.SmallDateTime;
					break;
                case "datetimeoffset":
					res = SqlDbType.DateTimeOffset;
					break;
				case "date":
					res = SqlDbType.Date;
					break;
				case "time":
					res = SqlDbType.Time;
					break;
				case "timestamp":
					res = SqlDbType.Timestamp;
					break;

				case "varbinary":
					res = SqlDbType.VarBinary;
					break;
				case "binary":
					res = SqlDbType.Binary;
					break;
				case "image":
					res = SqlDbType.Image;
					break;

				case "money":
					res = SqlDbType.Money;
					break;
				case "smallmoney":
					res = SqlDbType.SmallMoney;
					break;

				case "sql_variant":
					res = SqlDbType.Variant;
					break;

				case "uniqueidentifier":
					res = SqlDbType.UniqueIdentifier;
					break;

				case "bit":
					res = SqlDbType.Bit;
					break;

				case "xml":
					res = SqlDbType.Xml;
					break;

				default:
					throw new ArgumentException("Unexpected SQL type: " + name);
			}

			return res;
		}
	}
}

