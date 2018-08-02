using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ProductDetailedBO : BOBase
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string ProductData { get; set; }
        public int ShopId { get; set; }
        public int UserId { get; set; }
        public string mobileTransactionDate { get; set; }
        public string DetailedDate { get; set; }
        public string SyncDate { get; set; }
        public string mobileReferenceNo { get; set; }
        public string remark { get; set; }


        ProductDetailedDAO productDetailedDAO = new ProductDetailedDAO();
        public int UpdateProductDetailed()
        {
            return productDetailedDAO.UpdateProductDetailed(this);
        }
    }
}
