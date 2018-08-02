using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class OrderBO : BOBase 
    {
        #region Properties
        public string TotalAmount { get; set; }
        public string FirstDiscount { get; set; }
        public string SecondDiscount { get; set; }
        public string OrderDate { get; set; }
        public string OrderCount { get; set; }
        public int UserId { get; set; }
        public string Mode { get; set; }
        public int ShopId { get; set; }
        public string Priority { get; set; }
        public string ProductData { get; set; }
        public string QuantityData { get; set; }
        public string PaymentModeId { get; set; }
        public string UnitData { get; set; }
        public string Rate { get; set; }
        public string SpecialInstnSet { get; set; }
        public int SchemeId { get; set; }
        public string OtherInstn { get; set; }
        public string SiteAddress { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string MobileNo { get; set; }
        public string OrderTakenBy { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string BankName { get; set; }
        public string unitDiscount { get; set; }
        public string freeQuantity { get; set; }
        public string mobileDiscFlag { get; set; }
        public string orderDisc { get; set; }
        public string syncDate { get; set; }
        public string mobileTransactionDate { get; set; }
        public string distributorId { get; set; }
        public string mobileReferenceNo { get; set; }
        public int paymentReferenceNo { get; set; }
        public string userRef { get; set; }
        public string orderDiscountIds { get; set; }
        public string orderDiscountVals { get; set; }
        public string deliveryDate { get; set; }
        public int paymentMode { get; set; }
        public string source { get; set; }
        public string orderId { get; set; }
        public string schemeFromMobile { get; set; }
        public string mobileDate { get; set; }
        public int CommandTimeout { get; set; }
        public string signalStrength { get; set; }
        public string RequestedQuantityData { get; set; }
        public string InvoiceNo { get; set; }
        public string TempShopId { get; set; }
        public string TotalDiscount { get; set; }
        public string TaxAmount { get; set; }
        public string networkProvider { get; set; }
        public string IsDefaultDistributor { get; set; }
        public int length { get; set; }
        #endregion

        OrderDAO orderDAO = new OrderDAO();

        #region Methods

        public int GetPreviousDayOrdersCount()
        {
            return orderDAO.GetPreviousDayOrdersCount(this);
        }

        public string UpdateOrder()
        {
           return orderDAO.UpdateOrder(this);
        }
        public string ModifyOrder()
        {
            return orderDAO.ModifyOrder(this);
        }

        public Int64 GetActualAmount()
        {
            return orderDAO.GetActualAmount(this);
        }
        public DataSet GetInvoiceData()
        {
            return orderDAO.GetInvoiceData(this);
        }
        public DataSet GetOrderInvoiceDetails()
        {
            return orderDAO.GetOrderInvoiceDetails(this);
        }
        public DataSet GetSMSExistingCustomersOrder()
        {
            return orderDAO.GetSMSExistingCustomersOrder(this);
        }
        public DataSet GetOrderDispatchDetails()
        {
            return orderDAO.GetOrderDispatchDetails(this);
        }
        public DataSet UpdateDispatchDetails()
        {
            return orderDAO.UpdateDispatchDetails(this);
        }
        #endregion

    }
}
