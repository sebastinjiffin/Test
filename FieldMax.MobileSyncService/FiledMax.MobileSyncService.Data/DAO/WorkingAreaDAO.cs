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
    class WorkingAreaDAO
    {
        internal int updateWorkingArea(WorkingAreaBO workingAreaBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(workingAreaBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspAttendanceModule";
            sqlHelper.AddParameter(command, "@Mode", "4", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", workingAreaBO.userId, ParameterDirection.Input);
    
            sqlHelper.AddParameter(command, "@SelectedWorkAreaId", workingAreaBO.workingAreaMasterId, ParameterDirection.Input);

            sqlHelper.AddParameter(command, "@MobilePunchInDate", workingAreaBO.mobileCaptureDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@lat", workingAreaBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@lon", workingAreaBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", workingAreaBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobRefNo", workingAreaBO.MobRefNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", workingAreaBO.GpsSource, ParameterDirection.Input);
            if (workingAreaBO.ServerSyncDate != "" && workingAreaBO.MobileSyncDate != "")
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", workingAreaBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ServerSyncDate", workingAreaBO.ServerSyncDate, ParameterDirection.Input);
            }
            if (workingAreaBO.signalStrength != null && workingAreaBO.signalStrength != string.Empty && workingAreaBO.signalStrength != "")
            {
                sqlHelper.AddParameter(command, "@signalStrength", workingAreaBO.signalStrength, ParameterDirection.Input);
            }
            if (workingAreaBO.networkProvider != null && workingAreaBO.networkProvider != string.Empty && workingAreaBO.networkProvider != "")
            {
                sqlHelper.AddParameter(command, "@networkProvider", workingAreaBO.networkProvider, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
