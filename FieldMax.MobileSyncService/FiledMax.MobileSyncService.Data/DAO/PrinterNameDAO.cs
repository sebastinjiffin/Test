using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.BO;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using System.Data;

namespace FieldMax.MobileSyncService.Data.DAO
{
    class PrinterNameDAO
    {
        public string getPrinterName(PrinterNameBO printerNameBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(printerNameBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            try
            {
                command.CommandText = "SELECT PrinterName FROM PrinterConfig where UserId = '" + printerNameBO.UserId + "'";
                return (sqlHelper.ExecuteScalar(command)).ToString();
            }
            catch (Exception ex)
            {
                 return "No Printer";
            }
            
        }
    }
}
