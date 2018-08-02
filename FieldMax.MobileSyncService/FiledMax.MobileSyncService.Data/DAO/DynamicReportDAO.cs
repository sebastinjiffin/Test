using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;


namespace FieldMax.MobileSyncService.Data.DAO
{
   public class DynamicReportDAO
    {
       public DataSet GetReportName(DynamicReportBO reportBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(reportBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "uspWSGetDynamicReportName";
           sqlHelper.AddParameter(command, "@userId", reportBO.UserId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@level", reportBO.Level, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Mode", 2, ParameterDirection.Input);
           return sqlHelper.ExecuteDataSet(command);
       }

       public DataSet GetQueryResult(DynamicReportBO reportBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(reportBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "uspWSGetDynamicReport";
           sqlHelper.AddParameter(command, "@DynamicReportId", reportBO.DynamicReportId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@UserId", reportBO.UserId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@ShopId", reportBO.ShopId, ParameterDirection.Input);
           sqlHelper.AddParameter(command, "@Mode", reportBO.mode, ParameterDirection.Input);
           return sqlHelper.ExecuteDataSet(command);
       }

       public DataSet GetQueryResultForChild(DynamicReportBO reportBO)
       {
           DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(reportBO.ConString);
           SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
           command.CommandText = "mdr_getDynamicChildRprt";
           sqlHelper.AddParameter(command, "@Param", reportBO.paramValue, ParameterDirection.Input);
           return sqlHelper.ExecuteDataSet(command);
       }
    }
}
