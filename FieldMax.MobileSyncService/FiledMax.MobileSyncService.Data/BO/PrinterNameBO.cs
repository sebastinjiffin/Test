using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class PrinterNameBO : BOBase
    {
        #region Properties
        public string UserId { get; set; }
        #endregion
        PrinterNameDAO printerNameDAO = new PrinterNameDAO();
        #region Methods
        public string getPrinterName()
        {
            return printerNameDAO.getPrinterName(this);
        }

        #endregion
    }
}
