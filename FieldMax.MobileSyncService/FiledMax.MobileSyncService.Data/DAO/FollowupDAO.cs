using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class FollowupDAO
    {
        internal int UpdateFollowUp(FollowupBO followUpBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(followUpBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSFollowup";
            sqlHelper.AddParameter(command, "@Mode", "1", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", followUpBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", followUpBO.ShopId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@FollowUpId", followUpBO.FollowUpId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Remarks", followUpBO.Remarks, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@FollowUpDate", followUpBO.FollowUpDate.ToString(), ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@latitude", followUpBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", followUpBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", followUpBO.ProcessName, ParameterDirection.Input);

            if (followUpBO.signalStrength != null && followUpBO.signalStrength != string.Empty && followUpBO.signalStrength != "")
            {
                sqlHelper.AddParameter(command, "@signalStrength", followUpBO.signalStrength, ParameterDirection.Input);
            }
            if (followUpBO.networkProvider != null && followUpBO.networkProvider != string.Empty && followUpBO.networkProvider != "")
            {
                sqlHelper.AddParameter(command, "@networkProvider", followUpBO.networkProvider, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

        internal int GetFollowUp(FollowupBO followupBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(followupBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetFollowupsCount";
            sqlHelper.AddParameter(command, "@UserId", followupBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@FollowUpDate", followupBO.FollowUpDate.ToString(), ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }
    }
}
