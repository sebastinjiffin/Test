using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class MobileFeedbackBO : BOBase
    {
        MobileFeedbackDAO mobileFeedbackDAO = new MobileFeedbackDAO();

        #region Properties
        public string selectedAnsIdSet { get; set; }
        public string selectedAnsSet { get; set; }
        public int userId { get; set; }
        public int GpsSource { get; set; }
        public DateTime mobileCaptureDate { get; set; }
      
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string MobRefNo { get; set; }
        public string MobileSyncDate { get; set; }
        public string ServerSyncDate { get; set; }
        #endregion

        #region Methods
        public int UpdateMobileFeedback()
        {
            return mobileFeedbackDAO.updateFeedback(this);
        }
        #endregion
    }
}
