using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class EditCustomerBO : BOBase
    {
        #region Properties
        public int UserId { get; set; }
        public string ShopId { get; set; }

        public string ContactName { get; set; }
        public string MobileNo { get; set; }
        public string PinCode { get; set; }
        public string Address { get; set; }
        public string StreetName { get; set; }

        public string EditedDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string MobileReferenceNo { get; set; }
        public string MobileTransactionDate { get; set; }
        public string ProcessName { get; set; }
        public string MobileSyncDate { get; set; }
        public string GpsSource { get; set; }

        public string EditedContactName { get; set; }
        public string EditedMobile { get; set; }
        public string EditedPinCode { get; set; }
        public string EditedAddress { get; set; }
        public string Landmark { get; set; }

        public string UserEmail { get; set; }
        public string RequestedUser { get; set; }
        public string ReportingUser { get; set; }
        public string ShopName { get; set; }
        public bool? IsEditCustomerMailReq { get; set; }
        public string Mode { get; set; }
        public decimal ShopPotential { get; set; }
        public decimal EditedShopPotential { get; set; }
        public string GstNumber { get; set; }
        public string EditedGstNumber { get; set; }
        #endregion

        EditCustomerDAO editCustomerDAO = new EditCustomerDAO();

        public int updateCustomerDetails()
        {
           return editCustomerDAO.updateCustomerDetails(this);
        }

        public EditCustomerBO GetPendingApprovalsToReportingUser(EditCustomerBO editCustomerBO)
        {
            return editCustomerDAO.GetPendingApprovalsToReportingUser(editCustomerBO);
        }
      
        
    }

   
}
