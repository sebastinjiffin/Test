using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class PopBO : BOBase
    {
        #region Properties

        public int userId { get; set; }
        public int Mode { get; set; }
        public string popIdSet { get; set; }
        public string quantitySet { get; set; }
        public string lattitude { get; set; }
        public string longitude { get; set; }
        public string shopId { get; set; }
        public string processName { get; set; }
        public string mobileTransactionDate { get; set; }
        public string mobRefNo { get; set; }
        public string remarkSet { get; set; }
        public string MobileSyncDate { get; set; }
        public string ServerSyncDate { get; set; }
        #endregion

        PopDAO popDAO = new PopDAO();

        #region Methods

        public int updatePop()
        {
            return popDAO.updatePop(this);
        }



        #endregion

    }
}

