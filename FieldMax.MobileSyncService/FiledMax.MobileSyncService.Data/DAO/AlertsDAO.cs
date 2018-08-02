using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class AlertsDAO //: DAOBase
    {
        internal int GetAlertsCount(AlertsBO alertsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(alertsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetAlertsCount";
            sqlHelper.AddParameter(command, "@UserId", alertsBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@AlertDate", alertsBO.AlertDate, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }


        public int UpdateMessageStatus(AlertsBO alertsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(alertsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "Update MessageSentDetails set downloadStatus = 'true' where SentUserId ='"+alertsBO.UserId +"'";
            SqlCommand command2 = sqlHelper.CreateCommand(CommandType.Text);
            command2.CommandText = "Update VWSL_Message set downloadStatus = 'true' where SentUserId ='"+alertsBO.UserId +"'";
            sqlHelper.ExecuteScalar(command);
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command2));
        }
    }
}
