using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class VersionLogDAO //: DAOBase
    {
        internal int GetVersionLog(VersionLogBO versionLogBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(versionLogBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetPreviousDayOrdersCount";
            sqlHelper.AddParameter(command, "@UserId", versionLogBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@OrderDate", versionLogBO.VersionNo, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }

        internal int UpdateVersionLog(VersionLogBO versionLogBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(versionLogBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspVersionLog";
            sqlHelper.AddParameter(command, "@userId", versionLogBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@versionNo", versionLogBO.VersionNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@buildNo", versionLogBO.BuildNo, ParameterDirection.Input);
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
        internal DataSet GetVersionNo(VersionLogBO versionLogBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(versionLogBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "usp_GETVersionNO";
            sqlHelper.AddParameter(command, "@Mode", "3", ParameterDirection.Input);
            //return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
            return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet GetBuildNo(VersionLogBO versionLogBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(versionLogBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "usp_GETVersionNO";
            sqlHelper.AddParameter(command, "@Mode", "4", ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
