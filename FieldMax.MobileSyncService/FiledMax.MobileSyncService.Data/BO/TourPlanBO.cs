using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class TourPlanBO : BOBase
    {
        TourPlanDAO tourPlanDAO = new TourPlanDAO();
        #region Properties

        public int UserId { get; set; }                                                
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public DateTime MobileTransactionDate { get; set; }
        public DateTime MobileSyncDate { get; set; }
        public string MobileDate { get; set; }
        public string MobileReferenceNo { get; set; }
        public int GpsSource { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }
        #endregion
        public DateTime TourPlanDate { get; set; }
        public string SubmissionRemark  { get; set; }
        public DateTime SubmissionDate  { get; set; }
        public string SubmittedBy  { get; set; }
        public string MonthlyTourPlanId { get; set; }
        public int monthlyTourPlanId { get; set; }
        public string DailyData { get; set; }
        public int dailyUserId { get; set; }
        public DateTime Date { get; set; }
        public int Status { get; set; }
        public int ActionById { get; set; }
        public DateTime ActionDate { get; set; }
        public string dailyMobileReferenceNo { get; set; }
        public DateTime dailyMobileTransactionDate { get; set; }
        public string RoutePlanData { get; set; }
        public string JointWorkData { get; set; }
        public string ActivityPlanData { get; set; }
        public string LeaveData { get; set; }
        //leave
        public int resonId { get; set; }
        public int userId { get; set; }
        public DateTime mobileCaptureDate { get; set; }
        public DateTime leaveFrom { get; set; }
        public DateTime leaveTo { get; set; }
        public string LeaveLatitude { get; set; }
        public string LeaveLongitude { get; set; }
        public string LeaveProcessName { get; set; }
        public string MobRefNo { get; set; }
        public int LeaveGpsSource { get; set; }
        public string LeaveMobileSyncDate { get; set; }
        public DateTime ServerSyncDate { get; set; }
        public string Remarks { get; set; }
        public string LeaveSessionIdFrom { get; set; }
        public string LeaveSessionIdTo { get; set; }

        #region Methods
        public int UpdateMonthlyTourPlan()
        {
                        
            return tourPlanDAO.UpdateMonthlyTourPlan(this);
        }
        public void UpdateDailyTourPlan()
        {
            tourPlanDAO.UpdateDailyTourPlan(this);
        }        
        #endregion
    }
}
