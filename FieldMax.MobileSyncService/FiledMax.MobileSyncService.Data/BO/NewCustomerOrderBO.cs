using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class NewCustomerOrderBO : BOBase 
    {
        #region Properties
        public string TotalAmount { get; set; }
        public string FirstDiscount { get; set; }
        public string SecondDiscount { get; set; }
        public string OrderDate { get; set; }
        public string OrderCount { get; set; }
        public int UserId { get; set; }
        public string Mode { get; set; }
        public string ShopId { get; set; }
        public string Priority { get; set; }
        public string ProductData { get; set; }
        public string QuantityData { get; set; }
        public string PaymentModeId { get; set; }
        public string UnitData { get; set; }
        public string Rate { get; set; }
        public string SpecialInstnSet { get; set; }
        public string SchemeId { get; set; }
        public string OtherInstn { get; set; }
        public string SiteAddress { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string MobileNo { get; set; }
        public string OrderTakenBy { get; set; }
        public string OutletStock { get; set; }
        public string mobileReferenceNo { get; set; }
        public string orderDiscountIds { get; set; }
        public string orderDiscountVals { get; set; }
        public string deliveryDate { get; set; }
        public int paymentMode { get; set; }
        public string orderDisc { get; set; }
        public string source { get; set; }
        public int distributorId { get; set; }
        public string ReceiptNumber { get; set; }
        public string MobileSyncDate { get; set; }
        public string FreeQuantity { get; set; }
        public string UnitDiscount { get; set; }
        public string MobileDiscountFlag { get; set; }
        public string TotalDiscount { get; set; }
        public string TaxAmount { get; set; }
        public int Length { get; set; }
        #endregion

        NewCustomerOrderDAO orderDAO = new NewCustomerOrderDAO();

            #region Methods

        public int UpdateNewCustomerOrder()
        {
            return orderDAO.UpdateNewCustomerOrder(this);
        }
        public DataSet GetSMSOrderDetails()
        {
            return orderDAO.GetSMSOrderDetails(this);
        }

            #endregion
    }
}
