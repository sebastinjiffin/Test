using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public  class SqliteBackUpDAO
    {
       internal void BackUpSqlite(SqliteBackUpBo sqliteBackUpbo)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(sqliteBackUpbo.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "uspWSSqliteBackup";
           sqlHelper.AddParameter(command, "@UserId", sqliteBackUpbo.UserId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@SqlitePath", sqliteBackUpbo.SqlitePath, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@MobileArchiveDate", sqliteBackUpbo.mobileDate, ParameterDirection.Input);
           sqlHelper.ExecuteNonQuery(command);
       }
    }
}
