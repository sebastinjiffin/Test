using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.BO;
using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class IncentiveDashboardDAO
    {
        public DataSet GetPeriod(IncentiveDashboardBO incentiveDashboardBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(incentiveDashboardBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspIncentiveDashBoardReport";
            sqlHelper.AddParameter(command, "@userId", incentiveDashboardBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mode", incentiveDashboardBO.Mode, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        public DataSet GetPerformanceTarget(IncentiveDashboardBO incentiveDashboardBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(incentiveDashboardBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspIncentiveDashBoardReport";
            sqlHelper.AddParameter(command, "@userId", incentiveDashboardBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mode", incentiveDashboardBO.Mode, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        public DataSet GetPerformanceAchieved(IncentiveDashboardBO incentiveDashboardBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(incentiveDashboardBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspIncentiveDashBoardReport";
            sqlHelper.AddParameter(command, "@userId", incentiveDashboardBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mode", incentiveDashboardBO.Mode, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }


        public DataSet GetSKUDriveTarget(IncentiveDashboardBO incentiveDashboardBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(incentiveDashboardBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspIncentiveDashBoardReport";
            sqlHelper.AddParameter(command, "@userId", incentiveDashboardBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mode", incentiveDashboardBO.Mode, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        public DataSet GetSKUDriveAchieved(IncentiveDashboardBO incentiveDashboardBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(incentiveDashboardBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspIncentiveDashBoardReport";
            sqlHelper.AddParameter(command, "@userId", incentiveDashboardBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mode", incentiveDashboardBO.Mode, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
