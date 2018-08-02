using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.BO;
using Infocean.DataAccessHelper;
using System.Data;
using System.Data.SqlClient;



namespace FieldMax.MobileSyncService.Data.DAO
{
    class CheckDbDAO
    {
        internal Boolean IsModifiedDb(CheckDbBO checkDbBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(checkDbBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspCheckDbModified";
            sqlHelper.AddParameter(command, "@DbName", checkDbBO.DbName, ParameterDirection.Input);
            if (checkDbBO.DtLstModifiedDate != null)
            {
                sqlHelper.AddParameter(command, "@LastModifiedDate", checkDbBO.DtLstModifiedDate.ToString(), ParameterDirection.Input);
            }
            if (checkDbBO.UserId != null && checkDbBO.UserId != 0)
            {
                sqlHelper.AddParameter(command, "@UserId", checkDbBO.UserId, ParameterDirection.Input);
            }
            return Convert.ToBoolean(sqlHelper.ExecuteScalar(command));
            
        }

        internal Boolean IsMasterDataModified(CheckDbBO checkDbBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(checkDbBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspCheckMasterDataModified";
            sqlHelper.AddParameter(command, "@DbName", checkDbBO.DbName, ParameterDirection.Input);
            if (checkDbBO.DtLstModifiedDate != null)
            {
                sqlHelper.AddParameter(command, "@LastModifiedDate", checkDbBO.DtLstModifiedDate.ToString(), ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@UserId", checkDbBO.UserId, ParameterDirection.Input);
            return Convert.ToBoolean(sqlHelper.ExecuteScalar(command));

        }       

    }
}
