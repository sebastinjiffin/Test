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
    class MobilePunchingDAO
    {
        internal int punch(MobilePunchingBO mobilePunchingBO)
        {
            
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(mobilePunchingBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspAttendanceModule";
            sqlHelper.AddParameter(command, "@Mode", "3", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", mobilePunchingBO.userId, ParameterDirection.Input);
            if (mobilePunchingBO.punchInTime.Equals(""))
            {
                sqlHelper.AddParameter(command, "@MobilePunchOutDate", Convert.ToDateTime(mobilePunchingBO.punchOutTime), ParameterDirection.Input);
            }
            else
            {
                sqlHelper.AddParameter(command, "@MobilePunchInDate", Convert.ToDateTime(mobilePunchingBO.punchInTime), ParameterDirection.Input);
            }
            //if (mobilePunchingBO.punchOutTime.Equals(""))
            //{
            //   // sqlHelper.AddParameter(command, "@MobilePunchOutDate", DbNull.Value, ParameterDirection.Input);
            //}
            //else
            //{
            // //   sqlHelper.AddParameter(command, "@MobilePunchOutDate", mobilePunchingBO.punchOutTime, ParameterDirection.Input);
            //}
            sqlHelper.AddParameter(command, "@lat", mobilePunchingBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@lon", mobilePunchingBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", mobilePunchingBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", mobilePunchingBO.GpsSource, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OdoMeterReading", mobilePunchingBO.OdometerReading, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobRefNo", mobilePunchingBO.MobileRefNo, ParameterDirection.Input);

            if (mobilePunchingBO.ServerSyncDate != "" && mobilePunchingBO.MobileSyncDate != "")
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", mobilePunchingBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ServerSyncDate", mobilePunchingBO.ServerSyncDate, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(mobilePunchingBO.TravelModeId))
            {
                sqlHelper.AddParameter(command, "@TravelModeId", mobilePunchingBO.TravelModeId, ParameterDirection.Input);                
            }
            if (!string.IsNullOrEmpty(mobilePunchingBO.TravelModeAnswer))
            {
                sqlHelper.AddParameter(command, "@TravelModeAnswer", mobilePunchingBO.TravelModeAnswer, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
