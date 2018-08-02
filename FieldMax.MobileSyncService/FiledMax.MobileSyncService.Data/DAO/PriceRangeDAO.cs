using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FieldMax.MobileSyncService.Data.BO;
using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class PriceRangeDAO
    {

        public DataSet GetPriceRange(PriceRangeBO priceRangeBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(priceRangeBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.Text);
            command.CommandText = "select RateChangeLowerLimit,RateChangeUpperLimit,FirstDiscountLimit,SecondDiscountLimit from ParameterSettings";
           
            //return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
            return sqlHelper.ExecuteDataSet(command);
        }
    }
}
