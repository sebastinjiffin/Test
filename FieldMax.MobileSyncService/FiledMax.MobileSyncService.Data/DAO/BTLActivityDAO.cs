using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class BTLActivityDAO
    {
        internal int UpdateBTLDetails(BTLActivityBO btlActivityBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(btlActivityBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspBTLActivity";
            sqlHelper.AddParameter(command, "@Mode", btlActivityBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", btlActivityBO.UserId, ParameterDirection.Input);
            if (!string.IsNullOrEmpty(btlActivityBO.OrganizationId))
            {
                sqlHelper.AddParameter(command, "@OrganizationId", btlActivityBO.OrganizationId, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@BTLActivityId", btlActivityBO.BTLActivityId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ConfigFieldIds", btlActivityBO.ConfigFieldIds, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ConfigFieldValues", btlActivityBO.ConfigFieldValues, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Latitude", btlActivityBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Longitude", btlActivityBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileTransactionDate", btlActivityBO.MobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileReferenceNo", btlActivityBO.MobileReferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", btlActivityBO.GpsSource, ParameterDirection.Input);
            if (!string.IsNullOrEmpty(btlActivityBO.Attendees))
            {
                sqlHelper.AddParameter(command, "@Attendees", btlActivityBO.Attendees, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(btlActivityBO.MobileSyncDate))
            {
                sqlHelper.AddParameter(command, "@Syncdate", btlActivityBO.MobileSyncDate, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

    }
}
