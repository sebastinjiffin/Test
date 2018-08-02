using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ChequeRemittanceBO : BOBase
    {
        #region Properties
        public string Mode { get; set; }
        public string ChequeNo { get; set; }
        public int BankId { get; set; }
        public string mobileReferenceNo { get; set; }
        public string Remarks { get;set; }
        #endregion
        ChequeRemittanceDAO chequeRemittanceDAO = new ChequeRemittanceDAO();

        #region Methods
        public int UpdateChequeRemittance()
        {
            return chequeRemittanceDAO.UpdateChequeRemittance(this);
        }

        #endregion
    }
}
