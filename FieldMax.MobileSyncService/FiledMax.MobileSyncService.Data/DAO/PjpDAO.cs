using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class PjpDAO
    {
        internal int updatePjp(PjpBO pjpBO)
        {

            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(pjpBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUpdatePjp";
            sqlHelper.AddParameter(command, "@Mode", pjpBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@DateList", pjpBO.Date, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@RouteList", pjpBO.Routes, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileTransactionDate", pjpBO.MobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@RemarkList", pjpBO.Remarks, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@CommonRemarks", pjpBO.CommonRemarks, ParameterDirection.Input);
            if (pjpBO.Mode == "1")
            {
                sqlHelper.AddParameter(command, "@UserId", pjpBO.UserId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Status", pjpBO.Status, ParameterDirection.Input);                
            }
            else
            {
                sqlHelper.AddParameter(command, "@UserIdList", pjpBO.UserIdList, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@StatusList", pjpBO.StatusList, ParameterDirection.Input);
                if (pjpBO.MobileSyncDate != "")
                {
                    sqlHelper.AddParameter(command, "@MobileSyncDate", pjpBO.MobileSyncDate, ParameterDirection.Input);                    
                }
            }                                                   
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

        internal DataSet RequireMailForPjp(PjpBO pjpBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(pjpBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUpdatePjp";
            sqlHelper.AddParameter(command, "@Mode", "3", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", pjpBO.UserId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

    }
}
