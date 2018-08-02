using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class WorkingAreaBO : BOBase
    {
        WorkingAreaDAO workingAreaDAO = new WorkingAreaDAO();

        #region Properties
        public int workingAreaMasterId { get; set; }
        public int userId { get; set; }
        public DateTime mobileCaptureDate { get; set; }
        public DateTime updatedDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string MobRefNo { get; set; }
        public int GpsSource { get; set; }
        public string MobileSyncDate { get; set; }
        public string ServerSyncDate { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }
        #endregion

        #region Methods
        public int UpdateWorkArea()
        {
            return workingAreaDAO.updateWorkingArea(this);
        }
        #endregion
    }
}
