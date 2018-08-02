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
    public class EditPlanBO : BOBase
    {
        #region Properties
        public string UserId { get; set; }
        public string BeatPlanId { get; set; }
        public string CustomerIdSet { get; set; }
        public string DateSet { get; set; }
        public string StatusSet { get; set; }
        public string UserIdSet { get; set; }
        public string ApprovedDateSet { get; set; }
        #endregion
        EditPlanDAO editPlanDAO = new EditPlanDAO();

        public DataSet GetAssignedBeats(EditPlanBO editPlanBO)
        {
            return editPlanDAO.GetAssignedBeats(editPlanBO);
        }
        public DataSet GetCustomerForBeat(EditPlanBO editPlanBO)
        {
            return editPlanDAO.GetCustomerForBeat(editPlanBO);
        }
        public void UpdateEditPlan(EditPlanBO editPlanBO)
        {
            editPlanDAO.UpdateEditPlan(editPlanBO);
        }

    }
}
