using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class ParameterSettingsDAO
    {
        internal int getIsDateWiseBeat(ParameterSettingsBO parameterSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(parameterSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT isDateWiseBeat FROM ParameterSettings";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int getSqliteCount(ParameterSettingsBO dBCredentialBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(dBCredentialBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "select SqliteCount from ParameterSettings";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int getIsDailyBeat(ParameterSettingsBO parameterSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(parameterSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT isDailyBeat FROM ParameterSettings";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int getRequireErb(ParameterSettingsBO parameterSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(parameterSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT RequireERP FROM ParameterSettings";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int RequireSMSForOrder(ParameterSettingsBO parameterSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(parameterSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "select IsRequiredSMSOrder from parametersettings";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int RequireSMSForcollection(ParameterSettingsBO parameterSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(parameterSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "select IsRequiredSMSCollection from parametersettings";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }
        internal string isValidDeleiveryDateForMilma(ParameterSettingsBO parameterSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(parameterSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "validateMilmaDeliveryDate";
            return Convert.ToString(sqlHelper.ExecuteScalar(command));
        }

        internal DataSet GetUserParameters(ParameterSettingsBO parameterSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(parameterSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "GetInstanceDetails";
            sqlHelper.AddParameter(command, "@UserId", parameterSettingsBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Date", parameterSettingsBO.date, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
