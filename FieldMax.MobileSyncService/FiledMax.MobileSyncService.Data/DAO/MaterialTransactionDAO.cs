using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class MaterialTransactionDAO
    {
        internal int deleteAndInsertData(MaterialTransactionBO materialTransactionBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(materialTransactionBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "InsertMaterialTransaction";
            sqlHelper.AddParameter(command, "@UserId", materialTransactionBO.UserId, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

        internal int UpdateLogsForMaterialDelivery(MaterialTransactionBO materialTransactionBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(materialTransactionBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUpdateLog";            
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

       
    }
}
