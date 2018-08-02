using System.Data;
using System.Data.SqlClient;
using Infocean.DataAccessHelper;
using FieldMax.MobileSyncService.Data.BO;
using System;
using System.Collections.Generic;

namespace FieldMax.MobileSyncService.Data.DAO
{
    public class ShopDAO //: DAOBase
    {
        internal int GetShopCount(ShopBO shopBO)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(shopBO.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetShopCount";
            sqlHelper.AddParameter(command, "@UserId", shopBO.UserId, ParameterDirection.Input);
            return Convert.ToInt32( sqlHelper.ExecuteScalar(command));
        }

        public DataSet getCustomerShop(ShopBO shopBo)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(shopBo.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspWSGetTopThreeShopNames";
            sqlHelper.AddParameter(command, "@UserId", shopBo.UserId, ParameterDirection.Input);
            return sqlHelper.ExecuteDataSet(command);
        }

        internal int UpdateDoctorDetails(ShopBO shopBo)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(shopBo.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);

            command.CommandText = "UpdateDoctorDetailsAndroid"; //"uspWSUpdateCollectionDetails";
            sqlHelper.AddParameter(command, "@Name", shopBo.Name, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Address", shopBo.address, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@MobileNo", shopBo.mobileNo, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Email", shopBo.email, ParameterDirection.Input);
            try
            {
                Convert.ToDateTime(shopBo.dob);
                sqlHelper.AddParameter(command, "@DOB", shopBo.dob, ParameterDirection.Input);
            }
            catch
            {
            }
            try
            {
                Convert.ToDateTime(shopBo.dob);
                sqlHelper.AddParameter(command, "@WeddingDate", shopBo.weddingDate, ParameterDirection.Input);
            }
            catch
            {
            }
            sqlHelper.AddParameter(command, "@Qualifications", shopBo.qualifications, ParameterDirection.Input);
             
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }
        internal int UpdateShopDefaultDistributor(ShopBO shopBo)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(shopBo.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);

            command.CommandText = "uspShop";
            sqlHelper.AddParameter(command, "@Mode", "21", ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@ShopIds", shopBo.ShopIds, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@DistributorIds", shopBo.DistributorIds, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@userId", shopBo.UserId, ParameterDirection.Input);
            
            return Convert.ToInt32(sqlHelper.ExecuteNonQuery(command));
        }

        internal ShopBO.ShopData GetRouteOutlets(ShopBO shopBo)
        {
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(shopBo.ConString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            command.CommandText = "uspGetRouteOutlets";
            sqlHelper.AddParameter(command, "@BeatPlanId", shopBo.BeatPlanId, ParameterDirection.Input);
            SqlDataReader reader= sqlHelper.ExecuteReader(command);
            ShopBO.ShopData shopData = new ShopBO.ShopData();
            shopData.ShopList = new List<ShopBO.Shop>();
            while(reader.Read())
            {
                ShopBO.Shop shop = new ShopBO.Shop();
                shop.ShopId = Convert.ToInt32(reader["ShopId"]);
                shop.Name = Convert.ToString(reader["Name"]);
                shopData.ShopList.Add(shop);
            }
            reader.Close();
            return shopData;
        }
    }
}
