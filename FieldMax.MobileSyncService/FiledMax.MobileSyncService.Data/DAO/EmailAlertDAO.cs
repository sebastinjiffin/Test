using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;
using System.IO;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public  class EmailAlertDAO
    {
        internal DataSet DistributorEmailTrigger(EmailAlertBO emailAlertBO)
        {
            DataSet dt = new DataSet();
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(emailAlertBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspDistributorOrderReport";
            sqlHelper.AddParameter(command, "@DistributorId", emailAlertBO.DistributorID, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", emailAlertBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Mode", emailAlertBO.Mode, ParameterDirection.Input);
            dt = sqlHelper.ExecuteDataSet(command);
            return dt;
         }
        internal void DistributorEmailTriggerStatusLog(EmailAlertBO emailAlertBO)
        {
            DataSet dt = new DataSet();
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(emailAlertBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspDistributorOrderReport";
            sqlHelper.AddParameter(command, "@DistributorId", emailAlertBO.DistributorID, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", emailAlertBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Status", emailAlertBO.Status, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Mode", 10, ParameterDirection.Input);
            dt = sqlHelper.ExecuteDataSet(command);
        }
    }    
}
