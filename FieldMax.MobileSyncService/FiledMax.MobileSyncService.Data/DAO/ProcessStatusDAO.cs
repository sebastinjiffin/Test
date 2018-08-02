using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class ProcessStatusDAO
    {
        internal int getOrderConfirmStatusId(ProcessStatusBO processStatusBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(processStatusBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT Status FROM ProcessStatus where StatusDescription = 'Confirmed'";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int getOrderCorrectionStatusId(ProcessStatusBO processStatusBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(processStatusBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT Status FROM ProcessStatus where StatusDescription = 'Withheld'";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }
    }
}
