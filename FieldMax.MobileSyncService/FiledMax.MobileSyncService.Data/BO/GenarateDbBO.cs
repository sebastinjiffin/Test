using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.DAO;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class GenarateDbBO : BOBase
    {
        #region Properties
        public string DbName { get; set; }
        public int UserId { get; set; }


        #endregion
        GenarateDbDAO genarateDbDAO = new GenarateDbDAO();
        #region Methods

        public void UpdateGenerateDB()
        {
            genarateDbDAO.UpdateDb(this);
        }
        public Boolean CheckGenerateDb()
        {
            return genarateDbDAO.IsModifiedDb(this);
        }
        #endregion
    }
}
