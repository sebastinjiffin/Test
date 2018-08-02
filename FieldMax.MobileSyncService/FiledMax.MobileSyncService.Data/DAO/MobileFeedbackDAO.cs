using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using System.Data;
using FieldMax.MobileSyncService.Data.BO;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class MobileFeedbackDAO
    {
        internal int updateFeedback(MobileFeedbackBO mobileFeedbackBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(mobileFeedbackBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspAttendanceModule";
            sqlHelper.AddParameter(command, "@Mode", "5", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", mobileFeedbackBO.userId, ParameterDirection.Input);

            sqlHelper.AddParameter(command, "@SelectedAnsIdSet", mobileFeedbackBO.selectedAnsIdSet, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@SelectedAnsSet", mobileFeedbackBO.selectedAnsSet, ParameterDirection.Input);

            sqlHelper.AddParameter(command, "@MobilePunchInDate", mobileFeedbackBO.mobileCaptureDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@lat", mobileFeedbackBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@lon", mobileFeedbackBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", mobileFeedbackBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobRefNo", mobileFeedbackBO.MobRefNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", mobileFeedbackBO.GpsSource, ParameterDirection.Input);
            if (mobileFeedbackBO.ServerSyncDate != "" && mobileFeedbackBO.MobileSyncDate != "")
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", mobileFeedbackBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ServerSyncDate", mobileFeedbackBO.ServerSyncDate, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
