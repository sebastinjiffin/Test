using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public class PromotionalActivitiesDAO
    {
       //public DataSet GetPromotionalActivities(PromotionalActivitiesBO prmBO)
       //{
       //    DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(prmBO.ConString);
       //    SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
       //    command.CommandText = "getPromoDetails";          
       //    return sqlHelper.ExecuteDataSet(command);
       //}

       public DataSet getProm(PromotionalActivitiesBO prmbo1)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(prmbo1.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "getPromoDetails";
           return sqlHelper.ExecuteDataSet(command);
       }

       internal int UpdatePromotionalActivities(PromotionalActivitiesBO prmBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(prmBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "uspPromo";
           sqlHelper.AddParameter(command, "@Mode", prmBO.Mode, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@UserId", prmBO.UserId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ShopID", prmBO.ShopId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Question", prmBO.QuestionId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Answer", prmBO.AnswerId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@FreeText", prmBO.ResultDescription, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@latitude", prmBO.Latitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@longitude", prmBO.Longitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@processName", prmBO.ProcessName, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@RemarksDateSet", prmBO.DateSet, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@RemarksTextSet", prmBO.TextSet, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@RemarksNumberSet", prmBO.NumberSet, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ImageDataSet", prmBO.ImageDataSet, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@mobileTransactionDate", prmBO.MobileTransactionDate, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@MobileReferenceNo", prmBO.MobileReferenceNo, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@GpsSource", prmBO.GpsSource, ParameterDirection.Input);

           if (prmBO.ServerSyncDate != "" && prmBO.MobileSyncDate != "")
           {
               sqlHelper.AddParameter(command, "@MobileSyncDate", prmBO.MobileSyncDate, ParameterDirection.Input);
               sqlHelper.AddParameter(command, "@ServerSyncDate", prmBO.ServerSyncDate, ParameterDirection.Input);
           }
           if (prmBO.signalStrength != null && prmBO.signalStrength != string.Empty && prmBO.signalStrength != "")
           {
               sqlHelper.AddParameter(command, "@signalStrength", prmBO.signalStrength, ParameterDirection.Input);
           }
           if (prmBO.networkProvider != null && prmBO.networkProvider != string.Empty && prmBO.networkProvider != "")
           {
               sqlHelper.AddParameter(command, "@networkProvider", prmBO.networkProvider, ParameterDirection.Input);
           }
           return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
       }
    }
}
