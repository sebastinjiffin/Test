using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ProductTransactionBO : BOBase
    {
        #region Properties

        //public int UserId { get; set; }
        //public string VersionNo { get; set; }
        //public string BuildNo { get; set; }
        //public string Mode { get; set; }
        public int productId { get; set; }
        public double orderedQty { get; set; }
        public int organizationId { get; set; }
        public int unitId { get; set; }
        public int UserId { get; set; }

        public string ProductName { get; set; }
        public string ThirdPartyCode { get; set; }
        public int StockQuantity { get; set; }

        public int TotalDeliveredQuantity { get; set; }
        public int AvailableBalance { get; set; }
        #endregion
        ProductTransactionDAO productTransactionDAO = new ProductTransactionDAO();
        #region Methods
        public System.Data.DataSet GetOrderedQty()
        {
            return productTransactionDAO.GetProductQty(this);
        }
        public System.Data.DataSet GetDeliveredQty()
        {
            return productTransactionDAO.GetDeliveredProductQty(this);
        }
        public System.Data.DataSet GetReturnedQty()
        {
            return productTransactionDAO.GetReturnedQty(this);
        }
        public void insertInto()
        {
            productTransactionDAO.InsertInto(this);
        }
        public void deleteData()
        {
            productTransactionDAO.deleteData(this);
        }
        #endregion
    }
}
