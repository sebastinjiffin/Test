using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
   public  class SqliteBackUpBo:BOBase
   {
       SqliteBackUpDAO sqliteBackUpDAO = new SqliteBackUpDAO();

       #region Properties
       public int UserId { get; set; }
       public string SqlitePath { get; set; }
       public string mobileDate { get; set; }
       #endregion

    #region Methods
           public void BackUp()
           {
                sqliteBackUpDAO.BackUpSqlite(this);
           }
    #endregion

   }
}
