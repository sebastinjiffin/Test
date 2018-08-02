using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;


namespace FieldMax.MobileSyncService.Data.DAO
{
   public  class CompetitorDAO
    {
       internal int UpdateParameters(CompetitorBO competitorBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(competitorBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "updateParameters";           
           sqlHelper.AddParameter(command, "@ShopId", competitorBO.ShopId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@UserId", competitorBO.UserId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ProductID", competitorBO.ProductId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ParameterList", competitorBO.ParameterList, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@QuantityData", competitorBO.QuantityData, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@latitude", competitorBO.Latitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@longitude", competitorBO.Longitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@processName", competitorBO.ProcessName, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@mobileTransactionDate", competitorBO.MobileTransactionDate, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@mobRefNo", competitorBO.MobRefNo, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@GpsSource", competitorBO.GpsSource, ParameterDirection.Input);
           if (competitorBO.MobileSyncDate != "" && competitorBO.ServerSyncDate != "")
           {
               sqlHelper.AddParameter(command, "@MobileSyncDate", competitorBO.MobileSyncDate, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ServerSyncDate", competitorBO.ServerSyncDate, ParameterDirection.Input);
           }
           if (competitorBO.signalStrength != null && competitorBO.signalStrength != string.Empty && competitorBO.signalStrength != "")
           {
               sqlHelper.AddParameter(command, "@signalStrength", competitorBO.signalStrength, ParameterDirection.Input);
           }
           if (competitorBO.networkProvider != null && competitorBO.networkProvider != string.Empty && competitorBO.networkProvider != "")
           {
               sqlHelper.AddParameter(command, "@networkProvider", competitorBO.networkProvider, ParameterDirection.Input);
           }
           return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
       }
    }
}
