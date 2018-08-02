using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class TargetVsAchievementBO:BOBase
    {
        TargetVsAchievementDAO targetDAO = new TargetVsAchievementDAO();

        public string Mode { get; set; }
        public int UserId { get; set; }
        public int ShopId { get; set; }

        #region Methods
        public DataSet GetTargetVsAchievement()
        {
            return targetDAO.GetTargetVsAchievement(this);
        }
        public bool GetParameterConfigurations()
        {
            return targetDAO.GetParameterConfigurations(this);
        }
        public DataSet GetUploadTargetVsAchievement()
        {
            return targetDAO.GetUploadTargetVsAchievement(this);
        }
        public DataSet GetUploadTargetVsAchievementShopWise()
        {
            return targetDAO.GetUploadTargetVsAchievementShopWise(this);
        }

        

        #endregion

    }            
}
