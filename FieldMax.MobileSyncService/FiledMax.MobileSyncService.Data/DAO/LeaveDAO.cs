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
    class LeaveDAO
    {
        internal int updateLeave(LeaveBO leaveBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(leaveBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspLeaveRequest";
            sqlHelper.AddParameter(command, "@Mode", "1", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", leaveBO.userId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ReasonId", leaveBO.resonId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileTransactionDate", leaveBO.mobileCaptureDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@LeaveFrom", leaveBO.leaveFrom, ParameterDirection.Input);
            if (leaveBO.LeaveSessionIdFrom != "" && leaveBO.LeaveSessionIdFrom != "0")
            {
                sqlHelper.AddParameter(command, "@LeaveSessionIdFrom", leaveBO.LeaveSessionIdFrom, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@LeaveTo", leaveBO.leaveTo, ParameterDirection.Input);
            if (leaveBO.LeaveSessionIdTo != "" && leaveBO.LeaveSessionIdTo != "0")
            {
                sqlHelper.AddParameter(command, "@LeaveSessionIdTo", leaveBO.LeaveSessionIdTo, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@latitude", leaveBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", leaveBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@processName", leaveBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobRefNo", leaveBO.MobRefNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", leaveBO.GpsSource, ParameterDirection.Input);
            if (leaveBO.ServerSyncDate != "" && leaveBO.MobileSyncDate != "")
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", leaveBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ServerSyncDate", leaveBO.ServerSyncDate, ParameterDirection.Input);
            }
            if(!string.IsNullOrEmpty(leaveBO.Remarks))
            {
                sqlHelper.AddParameter(command, "@Remarks", leaveBO.Remarks, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
    }
}
