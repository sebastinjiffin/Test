using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class RemittanceDAO
    {

        internal int UpdateRemittance(RemittanceBO remittanceBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(remittanceBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspRemittance";
            sqlHelper.AddParameter(command, "@Mode", "A", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", remittanceBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@BankId", remittanceBO.BankId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Amount", remittanceBO.Amount, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", remittanceBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", remittanceBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", remittanceBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileTransactionDate", remittanceBO.MobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileReferenceNo", remittanceBO.MobRefNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", remittanceBO.GpsSource, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Remarks", remittanceBO.Remarks, ParameterDirection.Input);

            if (remittanceBO.ServerSyncDate != "" && remittanceBO.MobileSyncDate != "")
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", remittanceBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ServerSyncDate", remittanceBO.ServerSyncDate, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(remittanceBO.DenominationIds))
            {
                sqlHelper.AddParameter(command, "@DenominationIds", remittanceBO.DenominationIds, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(remittanceBO.Denominations))
            {
                sqlHelper.AddParameter(command, "@Denominations", remittanceBO.Denominations, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(remittanceBO.ApprovedBy))
            {
                sqlHelper.AddParameter(command, "@ApprovedBy", remittanceBO.ApprovedBy, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

        internal string GetTodaysRemittance(RemittanceBO remittanceBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(remittanceBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT ISNULL(SUM(AMOUNT),0) FROM Remittance where CONVERT(varchar(100),RemittedDate,103) = CONVERT(varchar(100),getdate(),103) and RemittedBy = " + remittanceBO.UserId;
            return Convert.ToString(sqlHelper.ExecuteScalar(command));
        }
    }
}
