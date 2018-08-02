using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class EmailDAO
    {
        internal DataSet getEmailSettings(EmailBO emailBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(emailBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspEmailSettings";
            sqlHelper.AddParameter(command, "@Mode", 5, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal string getOrganistionEmail(EmailBO emailBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(emailBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "SELECT email FROM organization WHERE organizationId = (SELECT TOP 1 organizationid FROM UserOrganization WHERE userId ="+ emailBO.id+")";
            return Convert.ToString(sqlHelper.ExecuteScalar(command));
        }
    }
}
