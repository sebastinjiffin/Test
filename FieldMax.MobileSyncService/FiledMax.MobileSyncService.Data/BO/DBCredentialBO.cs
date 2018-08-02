using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class DBCredentialBO : BOBase 
    {
        #region Properties

        //public string Constring { get; set; }

        public int User { get; set; }
        public string CustomerCode { get; set; }
        public string Date { get; set; }
        public string Loginname { get; set; }
        public string Mode { get; set; }
        public string Password { get; set; }
        public string UserID { get; set; }
        public string imeiKey { get; set; } 


        #endregion

        DBCredentialDAO dBCredentialDAO = new DBCredentialDAO();

        #region Methods

        public DataSet ValidateCustomerLogin()
        {
            return dBCredentialDAO.ValidateCustomerLogin(this);
        }

        public DataSet GetLicenceOfUser()
        {
            return dBCredentialDAO.GetLicenceOfUser(this);
        }

        public int getUserValidity()
        {
            return dBCredentialDAO.getUserValidity(this);
        }
        public int checkRequireShopWiseProductSorting()
        {
            return dBCredentialDAO.checkRequireShopWiseProductSorting(this);
        }
       
        #endregion

    }
}
