using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ExpenseBO : BOBase
    {
        #region Properties

        public int UserId { get; set; }
        public string Mode { get; set; }
        public string Parameters { get; set; }
        public string Quantity { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ProcessName { get; set; }
        public string MobileTransactionDate { get; set; }
        public string mobilereferenceNo { get; set; }
        public string Remarks { get; set; }
        public int GpsSource { get; set; }
        public string MobileSyncDate { get; set; }
        public string ServerSyncDate { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string ExpenseDate { get; set; }
        public string UniqueKey { get; set; }
        #endregion

        ExpenseDAO expenseDAO = new ExpenseDAO();

        #region Methods

        public int GetExpense()
        {
            return expenseDAO.GetExpense(this);
        }

        public int UpdateExpense()
        {
            return expenseDAO.UpdateExpense(this);
        }
        public string GetTodaysExpense()
        {
            return expenseDAO.GetTodaysExpense(this);
        }

        public void InsertTransactionData()
        {
            expenseDAO.InsertTransactionData(this);
        }

        #endregion

    }
}
