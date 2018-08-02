using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public class EnquiryDAO
    {
       internal int UpdateEnquiry(EnquiryBO enquiryBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(enquiryBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "uspEnquiry";
           sqlHelper.AddParameter(command, "@Mode", "J", ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ShopId", enquiryBO.ShopId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@EnquiredBy", enquiryBO.UserId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@EnquiryActivityId", enquiryBO.ActivityId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Remarks", enquiryBO.Remarks, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@lattitude", enquiryBO.Latitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@longitude", enquiryBO.Longitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@processName", enquiryBO.ProcessName, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@mobileOrderDate", enquiryBO.MobileTransactionDate, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@MobileReferenceNo", enquiryBO.MobileReferenceNo, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@GpsSource", enquiryBO.GpsSource, ParameterDirection.Input);
           if (enquiryBO.ProductId > 0)
           {
               sqlHelper.AddParameter(command, "@ProductId", enquiryBO.ProductId, ParameterDirection.Input);
           }
           if (enquiryBO.MobileSyncDate != "" && enquiryBO.MobileDate != "")
           {
               sqlHelper.AddParameter(command, "@MobileSyncDate", enquiryBO.MobileDate, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ServerSyncDate", enquiryBO.MobileSyncDate, ParameterDirection.Input);
           }
           if (!string.IsNullOrEmpty(enquiryBO.TempShopId))
           {
               sqlHelper.AddParameter(command, "@TempShopId", enquiryBO.TempShopId, ParameterDirection.Input);
           }
           if (enquiryBO.signalStrength != null && enquiryBO.signalStrength != string.Empty && enquiryBO.signalStrength != "")
           {
               sqlHelper.AddParameter(command, "@signalStrength", enquiryBO.signalStrength, ParameterDirection.Input);
           }
           if (enquiryBO.networkProvider != null && enquiryBO.networkProvider != string.Empty && enquiryBO.networkProvider != "")
           {
               sqlHelper.AddParameter(command, "@networkProvider", enquiryBO.networkProvider, ParameterDirection.Input);
           }
           return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
       }
       internal int getSMSAlertSetting(EnquiryBO enquiryBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(enquiryBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "uspEnquiry";
           sqlHelper.AddParameter(command, "@Mode", "1", ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ShopId", enquiryBO.ShopId, ParameterDirection.Input);           
           if (!string.IsNullOrEmpty(enquiryBO.TempShopId))
           {
               sqlHelper.AddParameter(command, "@TempShopId", enquiryBO.TempShopId, ParameterDirection.Input);
           }
           DataSet dsreturnVal = sqlHelper.ExecuteDataSet(command);
           int returnVal = Convert.ToInt32(dsreturnVal.Tables[0].Rows[0][0]);
           return returnVal;
       }
       internal DataSet getSMSData(EnquiryBO enquiryBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(enquiryBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "uspEnquiry";
           sqlHelper.AddParameter(command, "@Mode", "2", ParameterDirection.Input);
           //sqlHelper.AddParameter(command, "@EnquiryActivityId", enquiryBO.ActivityId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@UserId", enquiryBO.UserId, ParameterDirection.Input);
           if (!string.IsNullOrEmpty(enquiryBO.TempShopId))
           {
               sqlHelper.AddParameter(command, "@TempShopId", enquiryBO.TempShopId, ParameterDirection.Input);
           }
           if (enquiryBO.ShopId != null)
           {
               sqlHelper.AddParameter(command, "@ShopId", enquiryBO.ShopId, ParameterDirection.Input);
           }
           return sqlHelper.ExecuteDataSet(command);
       }
    }
}
