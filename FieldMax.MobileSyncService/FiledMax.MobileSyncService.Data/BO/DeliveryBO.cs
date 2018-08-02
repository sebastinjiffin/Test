using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;
using System.Data;
using System.Data.SqlClient;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class DeliveryBO :BOBase
    {
        #region Properties
        public int UserId { get; set; }
        public int shopId { get; set; }
        public int mode { get; set; }
        public string billNo { get; set; }
        public string productIdSet { get; set; }
        public string productUnitSet { get; set; }
        public string productSchemeSet { get; set; }
        public string deliveryQuantitySet { get; set; }
        public int orderId { get; set; }
        public int isClosed { get; set; }
        public string mobileTransactionDate { get; set; }
        public string mobileReferenceNo { get; set; }
        public string invoiceQtySet { get; set; }

        public string latitude { get; set; }
        public string longitude { get; set; }
        public string processName { get; set; }
        public int gpsSource { get; set; }
        public string signalStrength { get; set; }
        public string networkProvider { get; set; }
        public string returnReasonId { get; set; }


        #endregion
        DeliveryDAO deliveryDAO = new DeliveryDAO();
        
        #region Methods

        public int UpdateDeliveryData()
        {
            return deliveryDAO.UpdateDeliveryDate(this);
        }

        public DataSet getDeliveryDetails()
        {
            return deliveryDAO.getDeliveryDetails(this);
        }

        public DataSet getBills()
        {
            return deliveryDAO.getBills(this);
        }

        #endregion
    }
}
