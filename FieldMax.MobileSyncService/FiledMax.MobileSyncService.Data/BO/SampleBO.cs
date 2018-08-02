using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;
namespace FieldMax.MobileSyncService.Data.BO
{
    public class SampleBO:BOBase
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string ProductData { get; set; }
        public string DeliveredQty { get; set; }
        public string RequestedQty { get; set; }
        public int ShopId { get; set; }
        public int UserId { get; set; }
        public string mobileTransactionDate { get; set; }
        public string SubmissionDate { get; set; }
        public string SyncDate { get; set; }
        public string mobileReferenceNo { get; set; }

        SampleDAO sampleRequesDAO = new SampleDAO();
        public int UpdateSampleRequest()
        {
            return sampleRequesDAO.UpdateSampleRequest(this);
        }
    }
}
