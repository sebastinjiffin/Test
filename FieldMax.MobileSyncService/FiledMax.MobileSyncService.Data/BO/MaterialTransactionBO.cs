using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class MaterialTransactionBO : BOBase
    {
        #region Properties
        public int UserId { get; set; }
        #endregion

        MaterialTransactionDAO materialTransactionDAO = new MaterialTransactionDAO();
        #region Methods
        public int deleteAndInsertData()
        {
            return materialTransactionDAO.deleteAndInsertData(this);
        }

        public int UpdateLogsForMaterialDelivery()
        {
            return materialTransactionDAO.UpdateLogsForMaterialDelivery(this);
        }
        #endregion
    }
}
