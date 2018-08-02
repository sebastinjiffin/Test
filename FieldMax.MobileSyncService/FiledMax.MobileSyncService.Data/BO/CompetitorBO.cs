using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
   public class CompetitorBO:BOBase
    {
       CompetitorDAO competitorDAO = new CompetitorDAO();

        #region Properties
           public int UserId { get; set; }
           public int ShopId { get; set; }
           public int ProductId { get; set; }
           public string ParameterList { get; set;}
           public string QuantityData { get; set; }
           public string Latitude { get; set; }
           public string Longitude { get; set; }
           public string ProcessName { get; set; }
           public string MobRefNo { get; set; }
           public int GpsSource { get; set; }
           public string MobileTransactionDate { get; set; }
           public string ServerSyncDate { get; set; }
           public string MobileSyncDate { get; set; }
           public string networkProvider { get; set; }
           public string signalStrength { get; set; }
        #endregion

        #region Methods
           public int UpdateParameters()
           {
               return competitorDAO.UpdateParameters(this);
           }
        #endregion
    }
}
