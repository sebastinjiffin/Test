using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
     public class TransactionResultDAO
    {
         internal int UpdateTransactionResults(TransactionResultBO transactionBO)
         {
             DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(transactionBO.ConString);
             SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
             command.CommandText = "uspTransactionQnAnswer";
             sqlHelper.AddParameter(command, "@Mode", transactionBO.Mode, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@UserId", transactionBO.UserId, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@ShopID", transactionBO.ShopId, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@Question", transactionBO.QuestionId, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@Answer", transactionBO.AnswerId, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@FreeText", transactionBO.ResultDescription, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@latitude", transactionBO.Latitude, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@longitude", transactionBO.Longitude, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@processName", transactionBO.ProcessName, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@RemarksDateSet", transactionBO.DateSet, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@RemarksTextSet", transactionBO.TextSet, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@RemarksNumberSet", transactionBO.NumberSet, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@ImageDataSet", transactionBO.ImageDataSet, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@mobileTransactionDate", transactionBO.MobileTransactionDate, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@MobileReferenceNo", transactionBO.MobileReferenceNo, ParameterDirection.Input);
             sqlHelper.AddParameter(command, "@GpsSource", transactionBO.GpsSource, ParameterDirection.Input);

             if (transactionBO.ServerSyncDate != "" && transactionBO.MobileSyncDate != "")
             {
                 sqlHelper.AddParameter(command, "@MobileSyncDate", transactionBO.MobileSyncDate, ParameterDirection.Input);
                 sqlHelper.AddParameter(command, "@ServerSyncDate", transactionBO.ServerSyncDate, ParameterDirection.Input);
             }
             if (transactionBO.signalStrength != null && transactionBO.signalStrength != string.Empty && transactionBO.signalStrength != "")
             {
                 sqlHelper.AddParameter(command, "@signalStrength", transactionBO.signalStrength, ParameterDirection.Input);
             }
             if (transactionBO.networkProvider != null && transactionBO.networkProvider != string.Empty && transactionBO.networkProvider != "")
             {
                 sqlHelper.AddParameter(command, "@networkProvider", transactionBO.networkProvider, ParameterDirection.Input);
             }
             return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
         }
    }
}
