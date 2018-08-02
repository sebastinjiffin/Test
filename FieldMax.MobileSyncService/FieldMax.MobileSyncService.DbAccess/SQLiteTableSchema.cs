using System;
using System.Collections.Generic;
using System.Text;

namespace FieldMax.MobileSyncService.DbAccess
{
    public class SQLiteTableSchema
    {
        public string TableName;

        public List<SQLiteColumnSchema> Columns;
    }
}
