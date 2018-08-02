using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System.Collections.Generic;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ShopBO : BOBase 
    {
        #region Properties

        public int ShopId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string user { get; set; }
        public string address { get; set; }
        public string mobileNo { get; set; }
        public string email { get; set; }
        public string dob { get; set; }
        public string weddingDate { get; set; }
        public string qualifications { get; set; }
        public string ShopIds { get; set; }
        public string DistributorIds { get; set; }
        public int BeatPlanId { get; set; }

        public class Shop
        {
            public int ShopId { get; set; }
            public string Name { get; set; }
        }
        public class ShopData
        {
          public List<Shop> ShopList {get;set;}
        }
        #endregion
       
        ShopDAO shopDAO = new ShopDAO();
        
        #region Methods

        public int GetShopCount()
        {
            return shopDAO.GetShopCount(this);
        }

        public DataSet getCustomerShop()
        {
            return shopDAO.getCustomerShop(this);
        }

        public int UpdateDoctorDetails()
        {
            return shopDAO.UpdateDoctorDetails(this);
        }
        public int UpdateShopDefaultDistributor()
        {
            return shopDAO.UpdateShopDefaultDistributor(this);
        }
        
        public ShopBO.ShopData GetRouteOutlets()
        {
            return shopDAO.GetRouteOutlets(this);
        }
        #endregion

    }

}
