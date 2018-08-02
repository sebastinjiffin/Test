using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class JointWorkShopInAndOutBO : BOBase
    {
        #region Properties
        public int UserId { get; set; }

        public string ShopInId { get; set; }
        public string ShopInTime { get; set; }
        public string ShopOutId { get; set; }
        public string ShopOutTime { get; set; }

        public string JointWorkUserId { get; set; }

        public string MobileSyncDate { get; set; }
        public string MobileTransactionDate { get; set; }
        public string MobileReferenceNo { get; set; }
        public string BeatPlanId { get; set; }
        public string ShopName { get; set; }
        public string BeatPlanDeviationReasionId { get; set; }

        public string BeatPlanActivityConfigsId { get; set; }
        public string SurveyGroupId { get; set; }
        public string ShopId { get; set; }

        public string SurveyQuestionAnswers { get; set; }

        //gps
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string GpsSource { get; set; }
        public string SignalStrength { get; set; }
        public string NetworkProvider { get; set; }

        //gps

        // offline mode
        public string QuestionId { get; set; }
        public string AnswerId { get; set; }
        public string Remarks { get; set; }
        // offline mode
        
        #endregion

        JointWorkShopInAndOutDAO jointWorkShopInAndOutDAO = new JointWorkShopInAndOutDAO();

        public int updateJointWorkingShopInDetails()
        {
            return jointWorkShopInAndOutDAO.updateJointWorkingShopInDetails(this);
        }

        public int InsertJointWorkData()
        {
            return jointWorkShopInAndOutDAO.InsertJointWorkData(this);
        }

        public int updateJointWorkingShopInDetailsOffline()
        {
            return jointWorkShopInAndOutDAO.updateJointWorkingShopInDetailsOffline(this);
        }
    }
}
