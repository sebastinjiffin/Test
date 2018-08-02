using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class PromotionalActivitiesBO:BOBase
    {
        PromotionalActivitiesDAO promotionalActivitiesDAO = new PromotionalActivitiesDAO();

        #region Properties
        public int Mode { get; set; }
        public int UserId { get; set; }        
        public int ShopId { get; set; }
        public string QuestionId { get; set; }
        public string AnswerId { get; set; }
        public string ResultDescription { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string TextSet { get; set; }
        public string DateSet { get; set; }
        public string NumberSet { get; set; }
        public string ImageDataSet { get; set; }
        public string MobileTransactionDate { get; set; }
        public string MobileReferenceNo { get; set; }
        public int GpsSource { get; set; }
        public string MobileSyncDate { get; set; }
        public string ServerSyncDate { get; set; }
        public string networkProvider { get; set; }
        public string signalStrength { get; set; }

        #endregion

        #region Methods

        //public DataSet GetPromotionalActivities()
        //{
        //    return promotionalActivitiesDAO.GetPromotionalActivities(this);
        //}

        public DataSet getProm()
        {
            return promotionalActivitiesDAO.getProm(this);
        }

        public int UpdatePromotionalActivities()
        {
            return promotionalActivitiesDAO.UpdatePromotionalActivities(this);
        }
        #endregion
    }
}
