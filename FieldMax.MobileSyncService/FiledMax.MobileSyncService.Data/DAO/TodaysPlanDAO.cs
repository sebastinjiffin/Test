using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
   public class TodaysPlanDAO
    {
       internal int UpdateToDaysPlan(TodaysPlanBO todaysPlanBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(todaysPlanBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "[UpdateTodaysPlan]";
           sqlHelper.AddParameter(command, "@UserId", todaysPlanBO.UserId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@PlanIdSet", todaysPlanBO.planIdSet, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@latitude", todaysPlanBO.Latitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@longitude", todaysPlanBO.Longitude, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@processName", todaysPlanBO.ProcessName, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@GpsSource", todaysPlanBO.GpsSource, ParameterDirection.Input);
          
           sqlHelper.AddParameter(command, "@mobileTransactionDate", todaysPlanBO.MobileTransactionDate, ParameterDirection.Input);
           
           if (todaysPlanBO.MobileSyncDate != null)
           {
               sqlHelper.AddParameter(command, "@MobileSyncDate", todaysPlanBO.MobileSyncDate, ParameterDirection.Input);
           }
         
           if (todaysPlanBO.MobileRefNo != null && todaysPlanBO.MobileRefNo != string.Empty && todaysPlanBO.MobileRefNo != "")
           {
               sqlHelper.AddParameter(command, "@mobileRefNumber", todaysPlanBO.MobileRefNo, ParameterDirection.Input);
           }
           return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
 
       }
    }
}
