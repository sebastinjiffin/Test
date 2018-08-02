using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infocean.DataAccessHelper;
using System.Data;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.BO;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class ApplicationSettingsDAO //: DAOBase
    {
        public DataSet GetApplicationSettings(ApplicationSettingsBO objApplicationSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(objApplicationSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "usp_GetApplicationSettings";
            sqlHelper.AddParameter(command, "@Mode", objApplicationSettingsBO.Mode, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet GetEditableFlag(ApplicationSettingsBO objApplicationSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(objApplicationSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "usp_WSUserConfiguration";
            sqlHelper.AddParameter(command, "@Mode", objApplicationSettingsBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", objApplicationSettingsBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Date", objApplicationSettingsBO.Date, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet GetShopType(ApplicationSettingsBO objApplicationSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(objApplicationSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspGetShopType";
            return sqlHelper.ExecuteDataSet(command);
        }

        internal string GetUserRole(ApplicationSettingsBO objApplicationSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(objApplicationSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            sqlHelper.AddParameter(command, "@UserId", objApplicationSettingsBO.UserId, ParameterDirection.Input);
            command.CommandText = "GetVanSalesUserRole";
            DataSet ds = sqlHelper.ExecuteDataSet(command);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["UserRole"].ToString();
            }
            else
            {
                return "";
            }
        }
        public DataSet GetModifiedDate(ApplicationSettingsBO objApplicationSettingsBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(objApplicationSettingsBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspGetModifiedDate";
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
