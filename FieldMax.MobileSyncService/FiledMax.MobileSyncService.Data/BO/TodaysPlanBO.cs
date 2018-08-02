using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class TodaysPlanBO : BOBase
    {
        
        public int UserId { get; set; }
        public string planIdSet { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public int GpsSource { get; set; }
        public string SyncDate { get; set; }
        public string MobileSyncDate { get; set; }
        public string MobileRefNo { get; set; }
        public string MobileTransactionDate { get; set; }
        public string PlanDate { get; set; }

        TodaysPlanDAO todaysPlanDAO = new TodaysPlanDAO();
        #region Methods
        public int UpdateTodaysPlan()
        {
            return todaysPlanDAO.UpdateToDaysPlan(this);
        }
        #endregion
    }
}
