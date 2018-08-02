using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using System.Data;
using FieldMax.MobileSyncService.Data.BO;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class JointWorkShopInAndOutDAO
    {
        internal int updateJointWorkingShopInDetails(JointWorkShopInAndOutBO jointWorkShopInAndOutBO)
        {
            try
            {
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(jointWorkShopInAndOutBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUpdateJointWorkingShopInAndOut";
            sqlHelper.AddParameter(command, "@Mode", "1", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", jointWorkShopInAndOutBO.UserId, ParameterDirection.Input);

            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.ShopInId) && String.Compare(jointWorkShopInAndOutBO.ShopInId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@ShopInId", jointWorkShopInAndOutBO.ShopInId, ParameterDirection.Input);
            }
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.ShopInTime) && String.Compare(jointWorkShopInAndOutBO.ShopInTime, "1/1/1900 12:00:00 AM") != 0)
            {
                sqlHelper.AddParameter(command, "@ShopInTime", jointWorkShopInAndOutBO.ShopInTime, ParameterDirection.Input);
            }
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.ShopOutId) && String.Compare(jointWorkShopInAndOutBO.ShopOutId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@ShopOutId", jointWorkShopInAndOutBO.ShopOutId, ParameterDirection.Input);
            }
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.ShopOutTime) && String.Compare(jointWorkShopInAndOutBO.ShopOutTime, "1/1/1900 12:00:00 AM") != 0)
            {
                sqlHelper.AddParameter(command, "@ShopOutTime", jointWorkShopInAndOutBO.ShopOutTime, ParameterDirection.Input);
            }
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.JointWorkUserId))
            {
                sqlHelper.AddParameter(command, "@JointWorkingUserId", jointWorkShopInAndOutBO.JointWorkUserId, ParameterDirection.Input);
            }

            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.BeatPlanDeviationReasionId) && String.Compare(jointWorkShopInAndOutBO.BeatPlanDeviationReasionId,"0")!=0)
            {
                sqlHelper.AddParameter(command, "@BeatPlanDeviationReasonId", jointWorkShopInAndOutBO.BeatPlanDeviationReasionId, ParameterDirection.Input);
            }
           
            sqlHelper.AddParameter(command, "@MobileTransactionDate", jointWorkShopInAndOutBO.MobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileRefNo", jointWorkShopInAndOutBO.MobileReferenceNo, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.BeatPlanId))
            {
                sqlHelper.AddParameter(command, "@BeatPlanId", jointWorkShopInAndOutBO.BeatPlanId, ParameterDirection.Input);
            }
             
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.MobileSyncDate))
            {
                sqlHelper.AddParameter(command, "@MobileSyncDate", jointWorkShopInAndOutBO.MobileSyncDate, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@ShopName", jointWorkShopInAndOutBO.ShopName, ParameterDirection.Input);
             
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }


        /// <summary>
        /// Online Mode
        /// </summary>
        /// <param name="jointWorkShopInAndOutBO"></param>
        /// <returns></returns>
        internal int InsertJointWorkData(JointWorkShopInAndOutBO jointWorkShopInAndOutBO)
        {
            try{
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(jointWorkShopInAndOutBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUpdateJointWorkingShopInAndOut";
            sqlHelper.AddParameter(command, "@Mode", "2", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", jointWorkShopInAndOutBO.UserId, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.BeatPlanActivityConfigsId) && String.Compare(jointWorkShopInAndOutBO.BeatPlanActivityConfigsId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@BeatPlanActivityConfigsId", jointWorkShopInAndOutBO.BeatPlanActivityConfigsId, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@SurveyGroupId", jointWorkShopInAndOutBO.SurveyGroupId, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.BeatPlanId) && String.Compare(jointWorkShopInAndOutBO.BeatPlanId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@BeatPlanId", jointWorkShopInAndOutBO.BeatPlanId, ParameterDirection.Input);
            }
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.ShopId) && String.Compare(jointWorkShopInAndOutBO.ShopId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@ShopId", jointWorkShopInAndOutBO.ShopId, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@MobileRefNo", jointWorkShopInAndOutBO.MobileReferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopName", jointWorkShopInAndOutBO.ShopName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@SurveyQuestionAnswer", jointWorkShopInAndOutBO.SurveyQuestionAnswers, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileTransactionDate", jointWorkShopInAndOutBO.MobileTransactionDate, ParameterDirection.Input);

            sqlHelper.AddParameter(command, "@latitude", jointWorkShopInAndOutBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", jointWorkShopInAndOutBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@gpsSource", jointWorkShopInAndOutBO.GpsSource, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@networkProvider", jointWorkShopInAndOutBO.NetworkProvider, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@signalStrength", jointWorkShopInAndOutBO.SignalStrength, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.JointWorkUserId) && String.Compare(jointWorkShopInAndOutBO.JointWorkUserId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@JointWorkingUserId", jointWorkShopInAndOutBO.JointWorkUserId, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        internal int updateJointWorkingShopInDetailsOffline(JointWorkShopInAndOutBO jointWorkShopInAndOutBO)
        {
            try
            {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(jointWorkShopInAndOutBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspUpdateJointWorkingShopInAndOut";
            sqlHelper.AddParameter(command, "@Mode", "3", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", jointWorkShopInAndOutBO.UserId, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.BeatPlanActivityConfigsId) && String.Compare(jointWorkShopInAndOutBO.BeatPlanActivityConfigsId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@BeatPlanActivityConfigsId", jointWorkShopInAndOutBO.BeatPlanActivityConfigsId, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@SurveyGroupId", jointWorkShopInAndOutBO.SurveyGroupId, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.BeatPlanId) && String.Compare(jointWorkShopInAndOutBO.BeatPlanId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@BeatPlanId", jointWorkShopInAndOutBO.BeatPlanId, ParameterDirection.Input);
            }
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.ShopId) && String.Compare(jointWorkShopInAndOutBO.ShopId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@ShopId", jointWorkShopInAndOutBO.ShopId, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@MobileRefNo", jointWorkShopInAndOutBO.MobileReferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopName", jointWorkShopInAndOutBO.ShopName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileSyncDate", jointWorkShopInAndOutBO.MobileSyncDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileTransactionDate", jointWorkShopInAndOutBO.MobileTransactionDate, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.QuestionId) && String.Compare(jointWorkShopInAndOutBO.QuestionId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@QuestionId", jointWorkShopInAndOutBO.QuestionId, ParameterDirection.Input);
            }
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.AnswerId) && String.Compare(jointWorkShopInAndOutBO.AnswerId, "0") != 0)
            {
                sqlHelper.AddParameter(command, "@AnswerId", jointWorkShopInAndOutBO.AnswerId, ParameterDirection.Input);
            }
            sqlHelper.AddParameter(command, "@latitude", jointWorkShopInAndOutBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", jointWorkShopInAndOutBO.Longitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@gpsSource", jointWorkShopInAndOutBO.GpsSource, ParameterDirection.Input);
            
            sqlHelper.AddParameter(command, "@Remarks", jointWorkShopInAndOutBO.Remarks, ParameterDirection.Input);
            if (!String.IsNullOrEmpty(jointWorkShopInAndOutBO.JointWorkUserId))
            {
                sqlHelper.AddParameter(command, "@JointWorkingUserId", jointWorkShopInAndOutBO.JointWorkUserId, ParameterDirection.Input);
            }
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
