using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;
using System.IO;
namespace FieldMax.MobileSyncService.Data.DAO
{
    public class TargetVsAchievementDAO
    {
        internal DataSet GetTargetVsAchievement(TargetVsAchievementBO targetBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(targetBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspTargetVsAchievement";
            sqlHelper.AddParameter(command, "@Mode", targetBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", targetBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", targetBO.ShopId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }


        internal bool GetParameterConfigurations(TargetVsAchievementBO targetVsAchievementBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(targetVsAchievementBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUploadTargetVsAchievement";
            sqlHelper.AddParameter(command, "@Mode", "P", ParameterDirection.Input);
            return (bool)sqlHelper.ExecuteScalar(command);
        }

        internal DataSet GetUploadTargetVsAchievement(TargetVsAchievementBO targetVsAchievementBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(targetVsAchievementBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUploadTargetVsAchievement";
            sqlHelper.AddParameter(command, "@Mode", targetVsAchievementBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", targetVsAchievementBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", targetVsAchievementBO.ShopId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal DataSet GetUploadTargetVsAchievementShopWise(TargetVsAchievementBO targetVsAchievementBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(targetVsAchievementBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUploadTargetVsAchievement";
            sqlHelper.AddParameter(command, "@Mode", targetVsAchievementBO.Mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", targetVsAchievementBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopId", targetVsAchievementBO.ShopId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
