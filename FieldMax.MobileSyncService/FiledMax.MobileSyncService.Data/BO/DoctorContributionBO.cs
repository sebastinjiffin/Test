using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class DoctorContributionBO : BOBase
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string ProductData { get; set; }
        public string Qty { get; set; }
        public string ContributedBy { get; set; }
        public int ShopId { get; set; }
        public int UserId { get; set; }
        public string mobileTransactionDate { get; set; }
        public string Date { get; set; }
        public string SyncDate { get; set; }
        public string mobileReferenceNo { get; set; }

        DoctorContributionDAO doctorContributionDAO = new DoctorContributionDAO();
        public int UpdateDoctorContribution()
        {
            return doctorContributionDAO.UpdateDoctorContribution(this);
        }
    }
}
