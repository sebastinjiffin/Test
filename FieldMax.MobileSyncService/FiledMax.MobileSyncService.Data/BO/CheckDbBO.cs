using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;


namespace FieldMax.MobileSyncService.Data.BO
{
    public class CheckDbBO : BOBase
    {
        #region Properties
        public string DbName { get; set; }
        public DateTime DtLstModifiedDate { get; set; }
        public int UserId { get; set; }
      

        #endregion
        CheckDbDAO checkDbDAO = new CheckDbDAO();
        #region Methods

        public Boolean IsModifiedDb()
        {
            return checkDbDAO.IsModifiedDb(this);
        }

        public Boolean IsMasterDataModified()
        {
            return checkDbDAO.IsMasterDataModified(this);
        }
        
        #endregion

    }
}
