using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;


namespace FieldMax.MobileSyncService.Data.BO
{
    public class BeatPlanDeviationBO : BOBase
    {
        BeatPlanDeviationDAO beatPlanDeviationDAO = new BeatPlanDeviationDAO();

        #region Properties
        public int UserId { get; set; }
        public int BeatPlanId { get; set; }
        public string MobileReferenceNo { get; set; }
        public int DeviationReasonId { get; set; }
        public string MobileTransactionDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string source { get; set; }
        public string signalStrength { get; set; }
        public string SyncDate { get; set; }
        #endregion

        #region Methods
        public int UpdateBeatPlanDeviation()
        {
            return beatPlanDeviationDAO.UpdateBeatPlanDeviation(this);
        }
        #endregion
    }
}
