using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class QuatationDetailsBO : BOBase
    {
        QuatationDetailsDAO quatationDetailsDAO = new QuatationDetailsDAO();
        #region Properties

        public int shopId { get; set; }
        public int userId { get; set; }
        public string shopName { get; set; }
        public int port { get; set; }
        public string shopAddress { get; set; }
        public string userPhoneNumber { get; set; }
        public string userName { get; set; }
        public string userAddress { get; set; }
        public string productData { get; set; }
        public string quantityData { get; set; }
        public string rateData { get; set; }
        public string lattitude { get; set; }
        public string longitude { get; set; }
        public string totalAmount { get; set;}
        public string emailAddress { get; set; }
        public string processName { get; set; }
        public string userRole { get; set; }
        public string mobileTransactionDate { get; set; }
        public string mobRefNo { get; set; }
        public string SyncDate { get; set; }
        public string MobileSyncDate { get; set; }
        public string NewCustomerName { get; set; }
        public string NewCustomerAddress { get; set; }
        public string TempShopId { get; set; }
        public int GpsSource { get; set; }
        public string mobileDate { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }
        #endregion

        #region Methods
        public QuatationDetailsBO getQuatationDetails(int val)
        {
            QuatationDetailsBO bo = new QuatationDetailsBO();
            SqlDataReader sqlDataReader;
            if (val == 0)
            {
                sqlDataReader = quatationDetailsDAO.getNewCustomerDetails(this);
            }
            else
            {
                sqlDataReader = quatationDetailsDAO.getShopDetails(this);
            }
            
            SqlDataReader sqlDataReader2 =  quatationDetailsDAO.getUserDetails(this);
            bo = readDetailsFromReader(sqlDataReader, sqlDataReader2);
            return bo;

        }

        public string saveQuatation()
        {
            return quatationDetailsDAO.saveQuatation(this);
        }

        internal QuatationDetailsBO readDetailsFromReader(SqlDataReader reader1, SqlDataReader reader2)
        {
            QuatationDetailsBO bo = new QuatationDetailsBO();
            while(reader1.Read())
            {
                bo.shopName = reader1.GetString(0);
                bo.shopAddress = reader1.GetString(1);
            }
            while (reader2.Read())
            {
                bo.userName = reader2.GetString(0);
                bo.userAddress = reader2.GetString(1);
                bo.userRole = reader2.GetString(2);
            }
            return bo;
        }

        public string getHeaderNameForPDF(string productId)
        {
            return quatationDetailsDAO.getHeaderNameForPDF(this, productId);
        }

        public int isCustomizedImageNeeded()
        {
            return quatationDetailsDAO.isCustomizedImageNeeded(this);
        }

        #endregion
    }
}
