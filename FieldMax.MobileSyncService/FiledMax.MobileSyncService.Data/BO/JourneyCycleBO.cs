using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;
using System.Data;
namespace FieldMax.MobileSyncService.Data.BO
{
    public class JourneyCycleBO : BOBase
    {
        #region Properties

        public int UserId { get; set; }

        #endregion
        JourneyCycleDAO JourneyCycleDAO = new JourneyCycleDAO();
        public DataSet GetJourneyCycle()
        {
            return JourneyCycleDAO.GetJourneyCycle(this);
        }
    }
}
