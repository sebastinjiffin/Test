using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class NewCustomerBO : BOBase 
    {

        #region Properties

        public string ShopId { get; set; }
        public string Mode { get; set; }
        public string ShopName { get; set; }
        public string Phone { get; set; }
        public int reportedBy { get; set; }
        public string reportedDate { get; set; }
        public string TempShopId { get; set; }
        public string contactName { get; set; }
        public string shortName { get; set; }
        public string ShopType {get;set;}
        public string ShopClass { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Continent { get; set; }
        public int RouteId { get; set; }
        public int DistributorId { get; set; }
        public string District { get; set; }
        public string Email { get; set; }
        public string pinCode { get; set; }
        public string TinNo { get; set; }
        public string CSTNo { get; set; }
        public string ThirdParty { get; set; }
        public string CustomerGroup { get; set; }
        public string CreditLimit { get; set; }
        public string DoorNo { get; set; }
        public string StreetName { get; set; }
        public string Transporter { get; set; }
        public string ParentShopId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string ProcesName { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Field6 { get; set; }
        public string Field7 { get; set; }
        public string Field8 { get; set; }
        public string Field9 { get; set; }
        public string Field10 { get; set; }
        public string Field11 { get; set; }
        public string Field12 { get; set; }
        public string Field13 { get; set; }
        public string Field14 { get; set; }
        public string Field15 { get; set; }
        public string Field16 { get; set; }
        public string MobRefNo { get; set; }
        public int GpsSource { get; set; }
        public string MobileSyncDate { get; set; }
        public string NewCustomerConfig { get; set; }
        public string isSMSAlertRequired { get; set; }
        public int CustomerCategoryid { get; set; }
        public int Length { get; set; }
        public string gstin { get; set; }
       
        #endregion

        NewCustomerDAO newCustomerDAO = new NewCustomerDAO();

        #region Methods

        public int GetNewCustomer()
        {
            return newCustomerDAO.GetNewCustomer(this);
        }

        public DataSet UpdateNewCustomer()
        {
            return newCustomerDAO.UpdateNewCustomer(this);
        }
        public int getNewCustomerId(string id)
        {
            return newCustomerDAO.getNewCustomerId(this,id);
        }
        public DataSet UpdateNewCustomerSync()
        {
            return newCustomerDAO.UpdateNewCustomerSync(this);
        }
        public DataSet GetSMSDetails()
        {
            return newCustomerDAO.GetSMSDetails(this);
        }
        public int IsEmailAlertRequiredNewCustomer()
        {
            return newCustomerDAO.IsEmailAlertRequiredNewCustomer(this);
        }
        
        #endregion

    }
}
