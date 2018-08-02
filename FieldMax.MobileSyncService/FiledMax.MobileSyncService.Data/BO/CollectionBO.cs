using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class CollectionBO : BOBase 
    {
        #region Properties

        public string Mode { get; set; }
        public string CollectionCount { get; set; }
        public int UserId { get; set; }
        public int ShopId { get; set; }
        public int CollectedBy { get; set; }
        public string Amount { get; set; }
        public string ReceiptNo { get; set; }
        public string PaymentModeId { get; set; }
        public string InstrumentNo { get; set; }
        public int InvoiceNo { get; set; }
        public string Narration { get; set; }
        public int BankId { get; set; }
        public DateTime? InstrumentDate { get; set; }
        public string Discount { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string BillNo { get; set; }
        public string OsBalance { get; set; }
        public double TotalAmount { get; set; }
        public string IsRemitted { get; set; }
        public int RemittedAt { get; set; }
        public string IsRemittanceWithCollection { get; set; }
        public string IsMultipleDiscountCollection { get; set; }
        public string Discount1 { get; set; }
        public string Discount2 { get; set; }
        public string Discount3 { get; set; }
        public string MobilePaymentDate { get; set; }
        public string MobileSyncDate { get; set; }
        public string MobileDate { get; set; }
        public string MobileReferenceNo { get; set; }
        public string CollectionDiscount { get; set; }
        public int GpsSource { get; set; }
        public string TempShopId { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }
        
        #endregion

        CollectionDAO collectionDAO = new CollectionDAO();

        #region Methods

        public int GetCollectionCount()
        {
            return collectionDAO.GetCollectionCount(this);
        }

        public int UpdateCollection()
        {
            return collectionDAO.UpdateCollection(this);
        }

        public string GetTodaysCashCollection()
        {
            return collectionDAO.GetTodaysCashCollection(this);
        }
        public DataSet GetCollectionData()
        {
            return collectionDAO.GetCollectionData(this);
        }
        public DataSet GetCollectionDetails()
        {
            return collectionDAO.GetCollectionDetails(this);
        }
        #endregion

    }
}
