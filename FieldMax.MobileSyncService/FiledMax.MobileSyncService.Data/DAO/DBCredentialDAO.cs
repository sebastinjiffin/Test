using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class DBCredentialDAO //: DAOBase
    {
        internal DataSet ValidateCustomerLogin(DBCredentialBO dBCredentialBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(dBCredentialBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetLoginCredentials";
            sqlHelper.AddParameter(command, "@LoginName", dBCredentialBO.Loginname, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@PassWord", dBCredentialBO.Password, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ImeiNo", dBCredentialBO.imeiKey, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet GetLicenceOfUser(DBCredentialBO dBCredentialBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(dBCredentialBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspGetLicenceOfUser";
            sqlHelper.AddParameter(command, "@UserId", dBCredentialBO.User, ParameterDirection.Input);            
            return sqlHelper.ExecuteDataSet(command);
        }

        internal int getUserValidity(DBCredentialBO dBCredentialBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(dBCredentialBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "select Active from [User] where UserId = '"+dBCredentialBO.UserID+"'";
            return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
        }
        internal int checkRequireShopWiseProductSorting(DBCredentialBO dBCredentialBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(dBCredentialBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspGetRequireShopwiseProductFiltering";
            sqlHelper.AddParameter(command, "@UserId", dBCredentialBO.UserID, ParameterDirection.Input);
            try
            {
                return Convert.ToInt32(sqlHelper.ExecuteScalar(command));
            }
            catch
            {
                return 0;
            }
        }
    }
}
