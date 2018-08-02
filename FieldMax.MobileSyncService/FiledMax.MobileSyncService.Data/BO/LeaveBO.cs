using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class LeaveBO : BOBase
    {
        LeaveDAO leaveDAO = new LeaveDAO();
        #region Properties
        public int resonId { get; set; }
        public int userId { get; set; }
        public string mobileCaptureDate { get; set; }
        public DateTime leaveFrom { get; set; }
        public DateTime leaveTo { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string MobRefNo { get; set; }
        public int GpsSource { get; set; }
        public string MobileSyncDate { get; set; }
        public string ServerSyncDate { get; set; }
        public string Remarks { get; set; }
        public string LeaveSessionIdFrom { get; set; }
        public string LeaveSessionIdTo { get; set; }
        #endregion

        #region Methods
        public int UpdateLeaveRequest()
        {
            return leaveDAO.updateLeave(this);
        }
        #endregion
    }
}
