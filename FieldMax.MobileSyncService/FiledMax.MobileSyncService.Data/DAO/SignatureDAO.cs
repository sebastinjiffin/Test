using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public class SignatureDAO
    {
       internal int UpdateSignature(SignatureBO signatureBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(signatureBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "uspWSUpdateSignature";
           sqlHelper.AddParameter(command, "@ShopId", signatureBO.ShopId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@UserId", signatureBO.UserId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Data", signatureBO.Data, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ShopkeeperName", signatureBO.Shopkeepername, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@latitude", signatureBO.Latitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@longitude", signatureBO.Longitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@processName", signatureBO.ProcessName, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@MobileReferenceNo", signatureBO.MobileReferenceNo, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@GpsSource", signatureBO.GpsSource, ParameterDirection.Input);

           if (signatureBO.ServerSyncDate != "" && signatureBO.MobileSyncDate != "")
           {
               sqlHelper.AddParameter(command, "@MobileSyncDate", signatureBO.MobileSyncDate, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ServerSyncDate", signatureBO.ServerSyncDate, ParameterDirection.Input);
           }

           return Convert.ToInt32(sqlHelper.ExecuteScalar(command));

       }

       internal string GetProcessName(SignatureBO signatureBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(signatureBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
           command.CommandText = "select ProcessName from Process where ProcessId = " + signatureBO.ProcessId;
           return Convert.ToString(sqlHelper.ExecuteScalar(command));
       }
    }
}
