using System;
using System.Collections.Generic;
using System.Linq;
using FieldMax.MobileSyncService.Data.BO;
using System.Text;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using System.Data;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class GenarateDbDAO
    {
        internal void UpdateDb(GenarateDbBO genarateDbBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(genarateDbBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "UpdateGenerateDbLog";
            sqlHelper.AddParameter(command, "@Mode", 1, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", genarateDbBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@DbName", genarateDbBO.DbName, ParameterDirection.Input);
            sqlHelper.ExecuteScalar(command);
        }
        internal Boolean IsModifiedDb(GenarateDbBO genarateDbBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(genarateDbBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "UpdateGenerateDbLog";
            sqlHelper.AddParameter(command, "@Mode", 2, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", genarateDbBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@DbName", genarateDbBO.DbName, ParameterDirection.Input);
            return Convert.ToBoolean(sqlHelper.ExecuteScalar(command));
        }
    }
}
