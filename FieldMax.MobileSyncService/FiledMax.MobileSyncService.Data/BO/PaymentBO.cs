using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;
using Infocean.DataAccessHelper;
using System.Data;
using System.Data.SqlClient;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class PaymentBO : BOBase
    {
        public string ResponseCode { get; set; }
        public string Response { get; set; }
        public string MerRefNo { get; set; }
        public string RRN { get; set; }
        public string TransactionDate { get; set; }
        public string RemittorName { get; set; }
        public string CheckSum { get; set; }
        public int ShopId { get; set; }
        public int userId { get; set; }
        public string MMID { get; set; }
        public string phoneNo { get; set; }
        PaymentDAO paymentDAO = new PaymentDAO(); 
        public string UpdatePaymentResponse()
        {
            return paymentDAO.UpdatePaymentResponse(this);
        }
        public DataSet getShopPaymentParam()
        {
            return paymentDAO.getShopPaymentParam(this);
        }
        public void updateCustomerParam()
        {
            paymentDAO.updateCustomerParam(this);
        }
    }
}
