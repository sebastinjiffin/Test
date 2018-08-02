using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class MobilePunchingBO : BOBase
    {
        MobilePunchingDAO mobilePunchingDAO = new MobilePunchingDAO();

        #region Properties
        public int mobilePunchingId { get; set; }
        public int userId { get; set; }
        public int GpsSource { get; set; }
        public string punchInTime { get; set; }
        public string punchOutTime { get; set; }
        public DateTime updatedDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string OdometerReading { get; set; }
        public string MobileRefNo { get; set; }
        public string MobileSyncDate { get; set; }
        public string ServerSyncDate { get; set; }
        public string TravelModeId { get; set; }
        public string TravelModeAnswer { get; set; }
        #endregion

        #region Methods
        public int UpdatePunch()
        {
            return mobilePunchingDAO.punch(this);
        }
        #endregion
    }
}
