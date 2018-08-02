using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
   public  class DynamicReportBO:BOBase
    {
       DynamicReportDAO dynamicReportDAO = new DynamicReportDAO();
        #region Properties

        public int UserId { get; set; }
        public int DynamicReportId { get; set; }
        public string DynamicReportName  { get; set; }
        public string Query { get; set; }
        public string Level { get; set; }
        public int ShopId { get; set; }
        public string paramValue { get; set; }
        public string mode { get; set; }
        
        #endregion

        #region Methods



        public DataSet GetReportName()
        {
            return dynamicReportDAO.GetReportName(this);
        }
        public DataSet GetQueryReultReport()
        {
            return dynamicReportDAO.GetQueryResult(this);
        }

        public DataSet GetQueryResultForChild()
        {
            return dynamicReportDAO.GetQueryResultForChild(this);
        }

        #endregion
    }
}
