using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class TourPlanDAO
    {
        
        internal int UpdateMonthlyTourPlan(TourPlanBO tourPlanBO)
        {            
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(tourPlanBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspTourPlan";
            sqlHelper.AddParameter(command, "@Mode", "1", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@TourPlanDate", tourPlanBO.TourPlanDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@UserId", tourPlanBO.UserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@SubmissionRemark", tourPlanBO.SubmissionRemark, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@SubmissionDate", tourPlanBO.SubmissionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@SubmittedBy", Convert.ToInt32(tourPlanBO.SubmittedBy), ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@lattitude", tourPlanBO.Latitude, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@longitude", tourPlanBO.Longitude, ParameterDirection.Input);
            if(!String.IsNullOrEmpty(tourPlanBO.ProcessName))
                sqlHelper.AddParameter(command, "@processName", tourPlanBO.ProcessName, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@mobileOrderDate", tourPlanBO.MobileTransactionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileReferenceNo", tourPlanBO.MobileReferenceNo, ParameterDirection.Input);
            if(tourPlanBO.MobileSyncDate != DateTime.MinValue)
                sqlHelper.AddParameter(command, "@MobileSyncDate", tourPlanBO.MobileSyncDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@GpsSource", tourPlanBO.GpsSource, ParameterDirection.Input);
            SqlParameter outparam = command.Parameters.Add("@MonthlyTourPlanId", SqlDbType.Int);
            outparam.Direction = ParameterDirection.Output;
            sqlHelper.ExecuteNonQuery(command);
            return Convert.ToInt32(outparam.Value);                        
        }
        internal void UpdateDailyTourPlan(TourPlanBO tourPlanBO)
        {
            if (String.IsNullOrEmpty(tourPlanBO.RoutePlanData))
                tourPlanBO.RoutePlanData = "";
            if (String.IsNullOrEmpty(tourPlanBO.JointWorkData))
                tourPlanBO.JointWorkData = "";
            if (String.IsNullOrEmpty(tourPlanBO.ActivityPlanData))
                tourPlanBO.ActivityPlanData = "";
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(tourPlanBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspTourPlan";
            sqlHelper.AddParameter(command, "@Mode", "2", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MonthlyTourPlanIdSave", tourPlanBO.monthlyTourPlanId, ParameterDirection.Input);            
            sqlHelper.AddParameter(command, "@dailyUserId", tourPlanBO.dailyUserId, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Date", tourPlanBO.Date, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Status", tourPlanBO.Status, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ActionById", tourPlanBO.ActionById, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ActionDate", tourPlanBO.ActionDate, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@dailyMobileReferenceNo", tourPlanBO.dailyMobileReferenceNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@dailyMobileTransactionDate", tourPlanBO.dailyMobileTransactionDate, ParameterDirection.Input);           
            sqlHelper.AddParameter(command, "@RoutePlanData", tourPlanBO.RoutePlanData, ParameterDirection.Input);         
            sqlHelper.AddParameter(command, "@JointWorkData", tourPlanBO.JointWorkData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ActivityPlanData", tourPlanBO.ActivityPlanData, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@LeaveUserId", tourPlanBO.dailyUserId, ParameterDirection.Input);
            if (tourPlanBO.resonId != -1 && tourPlanBO.resonId != 0)
            sqlHelper.AddParameter(command, "@ReasonId", tourPlanBO.resonId, ParameterDirection.Input);
            if (tourPlanBO.mobileCaptureDate  != DateTime.MinValue)
                sqlHelper.AddParameter(command, "@LeaveMobileTransactionDate", tourPlanBO.mobileCaptureDate, ParameterDirection.Input);
            if (tourPlanBO.leaveFrom != DateTime.MinValue)
                sqlHelper.AddParameter(command, "@LeaveFrom", tourPlanBO.leaveFrom, ParameterDirection.Input);
            if (tourPlanBO.LeaveSessionIdFrom != "" && tourPlanBO.LeaveSessionIdFrom != "0" && tourPlanBO.LeaveSessionIdFrom != null && tourPlanBO.LeaveSessionIdFrom != "-1")
            {
                sqlHelper.AddParameter(command, "@LeaveSessionIdFrom", tourPlanBO.LeaveSessionIdFrom, ParameterDirection.Input);
            }
            if (tourPlanBO.leaveTo != DateTime.MinValue)
                sqlHelper.AddParameter(command, "@LeaveTo", tourPlanBO.leaveTo, ParameterDirection.Input);
            if (tourPlanBO.LeaveSessionIdTo != "" && tourPlanBO.LeaveSessionIdTo != "0" && tourPlanBO.LeaveSessionIdTo != null && tourPlanBO.LeaveSessionIdTo != "-1")
            {
                sqlHelper.AddParameter(command, "@LeaveSessionIdTo", tourPlanBO.LeaveSessionIdTo, ParameterDirection.Input);
            }
            if (tourPlanBO.LeaveLatitude != null)
                sqlHelper.AddParameter(command, "@Leavelatitude", tourPlanBO.LeaveLatitude, ParameterDirection.Input);
            if (tourPlanBO.LeaveLongitude != null)
                sqlHelper.AddParameter(command, "@Leavelongitude", tourPlanBO.LeaveLongitude, ParameterDirection.Input);
            if (tourPlanBO.LeaveProcessName != null)
                sqlHelper.AddParameter(command, "@LeaveprocessName", tourPlanBO.LeaveProcessName, ParameterDirection.Input);
            if (tourPlanBO.MobRefNo != null)
                sqlHelper.AddParameter(command, "@mobRefNo", tourPlanBO.MobRefNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@LeaveGpsSource", tourPlanBO.LeaveGpsSource, ParameterDirection.Input);
            if ( tourPlanBO.ServerSyncDate != DateTime.MinValue)
            {
                sqlHelper.AddParameter(command, "@LeaveMobileSyncDate", tourPlanBO.MobileSyncDate, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@LeaveServerSyncDate", tourPlanBO.MobileSyncDate, ParameterDirection.Input);
            }
            if (!string.IsNullOrEmpty(tourPlanBO.Remarks))
            {
                sqlHelper.AddParameter(command, "@Remarks", tourPlanBO.Remarks, ParameterDirection.Input);
            }
            sqlHelper.ExecuteNonQuery(command);
        }
    }
}
