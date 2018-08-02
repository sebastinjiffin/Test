using System;
using System.Collections.Generic;
using System.Text;

namespace FieldMax.MobileSyncService.DbAccess
{
    public class SQLiteColumnSchema
    {
        public string ColumnName;

        public string ColumnType;

        public int Length;

        public bool? NotNull = null;

        public string DefaultValue;

        public bool? IsPrimaryKey = null;
    }
}
