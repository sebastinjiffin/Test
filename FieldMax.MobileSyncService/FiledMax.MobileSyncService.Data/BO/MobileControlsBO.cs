using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class MobileControlsBO:BOBase
    {
        MobileControlsDAO mobileControlsDAO = new MobileControlsDAO();

        public int isBillwiseColection()
        {
            return mobileControlsDAO.isBillwiseColection(this);
        }

        public int isExpenseTransactionNeeded()
        {
            return mobileControlsDAO.isExpenseTransactionNeeded(this);
        }
    }
}
