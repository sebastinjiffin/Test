using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class MyActivityDAO
    {
        public int UpdateActivity(MyActivityBO myActivityBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(myActivityBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspMyActivity";
            sqlHelper.AddParameter(command, "@Mode", myActivityBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", myActivityBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ActivityId", myActivityBO.ActivityId, ParameterDirection.Input);
            if (myActivityBO.ActivityDeviationId != 0)
            {
                sqlHelper.AddParameter(command, "@ActivityDeviationId", myActivityBO.ActivityDeviationId, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@MobileReferenceNo", myActivityBO.MobileReferenceNo, ParameterDirection.Input);
            if (!string.IsNullOrEmpty(myActivityBO.MobileSyncDate))
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", Convert.ToDateTime(myActivityBO.MobileSyncDate), ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@MobileTransactionDate", Convert.ToDateTime(myActivityBO.MobileTransactionDate), ParameterDirection.Input);
            if (!string.IsNullOrEmpty(myActivityBO.CheckIn))
            {
                sqlHelper.AddParameter(command, "@CheckIn", Convert.ToDateTime(myActivityBO.CheckIn), ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(myActivityBO.CheckOut))
            {
                sqlHelper.AddParameter(command, "@CheckOut", Convert.ToDateTime(myActivityBO.CheckOut), ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(myActivityBO.Latitude))
            {
                sqlHelper.AddParameter(command, "@Latitude", myActivityBO.Latitude, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(myActivityBO.Longitude))
            {
                sqlHelper.AddParameter(command, "@Longitude", myActivityBO.Longitude, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(myActivityBO.signalStrength))
            {
                sqlHelper.AddParameter(command, "@signalStrength", myActivityBO.signalStrength, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(myActivityBO.NetworkProvider))
            {
                sqlHelper.AddParameter(command, "@NetworkProvider", myActivityBO.NetworkProvider, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(myActivityBO.ConfigFieldIds))
            {
                sqlHelper.AddParameter(command, "@ConfigFieldIds", myActivityBO.ConfigFieldIds, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(myActivityBO.ConfigFieldValues))
            {
                sqlHelper.AddParameter(command, "@ConfigFieldValues", myActivityBO.ConfigFieldValues, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(myActivityBO.ActivityPlannedDate))
            {
                sqlHelper.AddParameter(command, "@ActivityPlannedDate", Convert.ToDateTime(myActivityBO.ActivityPlannedDate), ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));

        }


        public int UpdateActivityLog(MyActivityBO myActivityBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(myActivityBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspMyActivity";
            sqlHelper.AddParameter(command, "@Mode", myActivityBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", myActivityBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ActivityId", myActivityBO.ActivityId, ParameterDirection.Input);
            if (myActivityBO.ActivityDeviationId != 0)
            {
                sqlHelper.AddParameter(command, "@ActivityDeviationId", myActivityBO.ActivityDeviationId, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@MobileReferenceNo", myActivityBO.MobileReferenceNo, ParameterDirection.Input);
            if (!string.IsNullOrEmpty(myActivityBO.MobileSyncDate))
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate",Convert.ToDateTime(myActivityBO.MobileSyncDate), ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@MobileTransactionDate", Convert.ToDateTime(myActivityBO.MobileTransactionDate), ParameterDirection.Input);
            if (!string.IsNullOrEmpty(myActivityBO.CheckIn))
            {
                sqlHelper.AddParameter(command, "@CheckIn", Convert.ToDateTime(myActivityBO.CheckIn), ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(myActivityBO.CheckOut))
            {
                sqlHelper.AddParameter(command, "@CheckOut", Convert.ToDateTime(myActivityBO.CheckOut), ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(myActivityBO.ActivityPlannedDate))
            {
                sqlHelper.AddParameter(command, "@ActivityPlannedDate", Convert.ToDateTime(myActivityBO.ActivityPlannedDate), ParameterDirection.Input);
            }

            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));

        }
    }
}
