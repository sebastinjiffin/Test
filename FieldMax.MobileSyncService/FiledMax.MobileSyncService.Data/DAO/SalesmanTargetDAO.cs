using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public class SalesmanTargetDAO
    {
       internal int GetTargetAmount(SalesmanTargetBO targetBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(targetBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "uspWSGetTargetAmount";
           sqlHelper.AddParameter(command, "@UserId", targetBO.SalesManId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Date", targetBO.Month, ParameterDirection.Input);
           return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
       }
    }
}
