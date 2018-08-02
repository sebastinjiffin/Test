using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using LogWriter;
using System.IO.Compression;
using System.Diagnostics;
using Ionic.Zip;
using FieldMax.MobileSyncService.Data;
using FieldMax.MobileSyncService.Data.BO;

namespace FieldMax.MobileSyncService.DbAccess
{
    /// <summary>
    /// This class is resposible to take a SQLite database file
    /// and convert it to an SQL Server database.
    /// </summary>
    /// <remarks>The class involvs how to query Sqlite table and return data set.</remarks>
    public class SQLiteToDB : SQLiteDatabase
    {

        #region Variables        

        static ErrorLogUtilities _log = new ErrorLogUtilities();

        public static string mobileSyncDateVal;

        #endregion

        #region Public Methods

        public void UnzipFile(string path, string fileName)
        {
            //TO UNZIP FILE
            ZipFile zip1 = ZipFile.Read(path + fileName);
            //zip1.ExtractAll("TestSqliteShop", ExtractExistingFileAction.OverwriteSilently);
            zip1.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
        }

        /// <summary>
        /// Read given sqlite db and prepares SQLite Table schema and SQLite Column schema  
        /// for all transaction tables
        /// </summary>
        /// <param name="fileName">File name with path</param>
        private List<SQLiteTableSchema> ReadSQLiteDB(string fileName, string query)
        {
            SetSqliteConnectionString(fileName);
            List<SQLiteTableSchema> syncTables = new List<SQLiteTableSchema>();
            SQLiteTableSchema sQLiteTableSchema;
            SQLiteColumnSchema sQLiteColumnSchema,mobileSyncDateColumnSchema;
            string[] strArray;
            //Read all transaction tables(tables which contain IsSync column) and add that tables to SQLiteTableSchema List.
            DataTableReader dtr = GetDataTable(query).CreateDataReader();


            while (dtr.Read())
            {
                sQLiteTableSchema = new SQLiteTableSchema();
                sQLiteTableSchema.Columns = new List<SQLiteColumnSchema>();
                string tableName = dtr["name"].ToString();
                sQLiteTableSchema.TableName = tableName;
                DataTableReader dtrColumns = GetDataTable("pragma table_info(" + tableName + ")").CreateDataReader();
                while (dtrColumns.Read())
                {
                    sQLiteColumnSchema = new SQLiteColumnSchema();
                    sQLiteColumnSchema.ColumnName = dtrColumns["name"].ToString();
                    sQLiteColumnSchema.ColumnType = dtrColumns["type"].ToString().Split('(')[0];
                    strArray = dtrColumns["type"].ToString().Split('(');
                    if (strArray.Length > 1)
                    {
                        sQLiteColumnSchema.Length = Convert.ToInt32(strArray[1].Split(')')[0]);
                    }
                    else
                    {
                        sQLiteColumnSchema.Length = 0;
                    }
                    sQLiteColumnSchema.NotNull = Convert.ToBoolean(dtrColumns["notnull"]);
                    sQLiteColumnSchema.DefaultValue = dtrColumns["dflt_value"].ToString();
                    sQLiteColumnSchema.IsPrimaryKey = Convert.ToBoolean(dtrColumns["pk"]);
                    sQLiteTableSchema.Columns.Add(sQLiteColumnSchema);
                }

                //Add SyncDate field(SQLite Column Schema) to the SQLite Table Schema 
                if (sQLiteTableSchema.TableName != "NewCustomerDetails")
                {
                    sQLiteColumnSchema = new SQLiteColumnSchema();
                    sQLiteColumnSchema.ColumnName = "SyncDate";
                    sQLiteColumnSchema.ColumnType = "datetime";
                    sQLiteColumnSchema.Length = 0;
                    sQLiteColumnSchema.NotNull = false;
                    sQLiteColumnSchema.DefaultValue = System.DateTime.Now.ToString();
                    sQLiteColumnSchema.IsPrimaryKey = false;
                    sQLiteTableSchema.Columns.Add(sQLiteColumnSchema);

                    mobileSyncDateColumnSchema=new SQLiteColumnSchema();
                    mobileSyncDateColumnSchema.ColumnName = "MobileSyncDate";
                    mobileSyncDateColumnSchema.ColumnType = "datetime";
                    mobileSyncDateColumnSchema.Length = 0;
                    mobileSyncDateColumnSchema.NotNull = false;
                    mobileSyncDateColumnSchema.DefaultValue = System.DateTime.Now.ToString();
                    mobileSyncDateColumnSchema.IsPrimaryKey = false;
                    sQLiteTableSchema.Columns.Add(mobileSyncDateColumnSchema);
                }

                syncTables.Add(sQLiteTableSchema);
            }

            return syncTables;
        }

        /*************GPS method*********************/
        private void insertGPSInfo(String processId, String processDetailsIdServer, String processDetailsIdSqlite, SqlConnection ssconn, SqlTransaction trn,List<SQLiteTableSchema> syncTables)
        {
            SQLiteTableSchema newCustomerGPsSchema = getCurrentSchema("GPSLocationInfo", syncTables);

            DataTableReader dtrgps = GetDataTable("select * from GPSLocationInfo WHERE  ProcessDetailsId = " + processDetailsIdSqlite + " and ProcessId =" + processId).CreateDataReader();
            SqlCommand insertGPSLoc = BuildSQLServerInsertforGPS(newCustomerGPsSchema);
            //_log.LogMessage("insertGPSInfo", "finished inserting all rows for table ", "L1");
            while (dtrgps.Read()) //dtr have all data from Temp_OrderHeader, this loop will exc. no. of rows times 
            {
                insertGPSLoc.Connection = ssconn;
                insertGPSLoc.Transaction = trn;
                List<string> pnamesNewCusGPS = new List<string>();

                int counterNew = 0;
                foreach (SQLiteColumnSchema column in newCustomerGPsSchema.Columns)
                {
                    if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                    {
                        if (column.ColumnName == "SyncDate")
                        {
                            string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                            insertGPSLoc.Parameters[pname].Value = DateTime.Now.ToString();
                            pnamesNewCusGPS.Add(pname);
                        } 
                        else if (column.ColumnName == "MobileSyncDate")
                        {
                            string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                            insertGPSLoc.Parameters[pname].Value = mobileSyncDateVal;
                            pnamesNewCusGPS.Add(pname);
                        }
                        else if (column.ColumnName == "Latitude" || column.ColumnName == "Longitude" || 
                            column.ColumnName == "ProcessId" || column.ColumnName == "UserId" || 
                            column.ColumnName == "ShopId" || column.ColumnName == "Source" || column.ColumnName == "MobileReferenceNo")
                        //|| column.ColumnName == "LocationSource")
                        {
                            var value = dtrgps[counterNew];
                            if (dtrgps[counterNew] != null)
                            {
                                string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                                insertGPSLoc.Parameters[pname].Value = CastValueForColumn(dtrgps[counterNew], column);
                                pnamesNewCusGPS.Add(pname);
                            }
                        }
                        else if (column.ColumnName == "ProcessDetailsId")
                        {
                            string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                            insertGPSLoc.Parameters[pname].Value = CastValueForColumn(processDetailsIdServer, column);
                            pnamesNewCusGPS.Add(pname);
                        }
                        else if (column.ColumnName == "CapturedDate")
                        {
                            string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                            insertGPSLoc.Parameters[pname].Value = DateTime.Now.ToString();
                            pnamesNewCusGPS.Add(pname);
                        }

                        else if (column.ColumnName == "MobileTransactionDate")
                        {
                            string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                            insertGPSLoc.Parameters[pname].Value = dtrgps[counterNew].ToString();
                            pnamesNewCusGPS.Add(pname);
                        }
                        
                    }

                    counterNew++;
                }
                int gpsId = Convert.ToInt32(insertGPSLoc.ExecuteScalar());
            }
        }
        /**************************without shop id*********/
        private void insertGPSInfoWithoutShop(String processId, String processDetailsIdServer, String processDetailsIdSqlite, SqlConnection ssconn, SqlTransaction trn,List<SQLiteTableSchema> syncTables)
        {
            //_log.LogMessage("insertGPSInfoWithoutShop", "finished inserting all rows for table ", "L1");
            SQLiteTableSchema newCustomerGPsSchema = getCurrentSchema("GPSLocationInfo", syncTables);

            DataTableReader dtrgps = GetDataTable("select * from GPSLocationInfo WHERE  ProcessDetailsId = " + processDetailsIdSqlite + " and ProcessId =" + processId).CreateDataReader();
            SqlCommand insertGPSLoc = BuildSQLServerInsertforGPSWithoutShop(newCustomerGPsSchema);

            while (dtrgps.Read()) //dtr have all data from Temp_OrderHeader, this loop will exc. no. of rows times 
            {
                insertGPSLoc.Connection = ssconn;
                insertGPSLoc.Transaction = trn;
                List<string> pnamesNewCusGPS = new List<string>();

                int counterNew = 0;
                foreach (SQLiteColumnSchema column in newCustomerGPsSchema.Columns)
                {
                    if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                    {
                        if (column.ColumnName == "SyncDate")
                        {
                            string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                            insertGPSLoc.Parameters[pname].Value = DateTime.Now.ToString();
                            pnamesNewCusGPS.Add(pname);
                        } 
                        else if (column.ColumnName == "MobileSyncDate")
                        {
                            string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                            insertGPSLoc.Parameters[pname].Value = mobileSyncDateVal;
                            pnamesNewCusGPS.Add(pname);
                        }
                        else if (column.ColumnName == "Latitude" || column.ColumnName == "Longitude"
                            || column.ColumnName == "ProcessId" || column.ColumnName == "UserId"
                            || column.ColumnName == "Source" || column.ColumnName == "MobileReferenceNo")
                        {
                            //_log.LogMessage("insertGPSInfoWithoutShop", "inside if ", "L1");
                            var value = dtrgps[counterNew];
                            if (dtrgps[counterNew] != null)
                            {
                                string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                                insertGPSLoc.Parameters[pname].Value = CastValueForColumn(dtrgps[counterNew], column);
                                pnamesNewCusGPS.Add(pname);
                            }
                        }
                        else if (column.ColumnName == "ProcessDetailsId")
                        {
                            string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                            insertGPSLoc.Parameters[pname].Value = CastValueForColumn(processDetailsIdServer, column);
                            pnamesNewCusGPS.Add(pname);
                        }
                            
                        else if (column.ColumnName == "CapturedDate")
                        {
                            string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                            insertGPSLoc.Parameters[pname].Value = DateTime.Now.ToString();
                            pnamesNewCusGPS.Add(pname);
                        }

                        else if (column.ColumnName == "MobileTransactionDate")
                        {
                            string pname = "@" + GetNormalizedName(column.ColumnName, pnamesNewCusGPS);
                            insertGPSLoc.Parameters[pname].Value = dtrgps[counterNew].ToString(); 
                            pnamesNewCusGPS.Add(pname);
                        }
                       
                    }

                    counterNew++;
                }
                //_log.LogMessage("insertGPSInfoWithoutShop", "after for", "L1");
                int gpsId = Convert.ToInt32(insertGPSLoc.ExecuteScalar());
            }
        }
        /****************************************************/

        public string[] CopySQLiteDBRowsToSqlServer(string connectionString, string fileName, Dictionary<string, string> query
            , string mobileSyncDate, string tableQuery, string mobileDate)
        {
            if (mobileSyncDate.Equals(string.Empty) || mobileSyncDate == null)
            {
                mobileSyncDate = DateTime.Now.ToString();
            }
 
            string[] dataArray = new string[51];
 

            mobileSyncDateVal = mobileDate;
            string Data = "";
            string DataQuotation = "";
            string quotationData = string.Empty;
            List<SQLiteTableSchema> syncTables= ReadSQLiteDB(fileName, tableQuery);

            // Connect to the SQL Server database
            using (SqlConnection ssconn = new SqlConnection(connectionString))
            {

                ssconn.Open();


                //Read SqLite DB tables 
                StringBuilder sb = new StringBuilder();
                string[] keys = new string[query.Keys.Count];


                foreach (SQLiteTableSchema tableSchema in syncTables)
                {
                    SqlTransaction trn = ssconn.BeginTransaction();
                    //SqlTransaction trn1 = ssconn.BeginTransaction();
                    try
                    {
                        DataTableReader dtr = null;
                        //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "started table [" + tableSchema.TableName + "]", "L1");
                        if (tableSchema.TableName == "NewCustomer")
                        {
                            dtr = GetDataTable("select * from " + tableSchema.TableName).CreateDataReader();
                        }
                        else if (tableSchema.TableName != "sqlite_sequence" && tableSchema.TableName != "android_metadata" && tableSchema.TableName != "VWSL_OrderDetails" && tableSchema.TableName != "LocalStore")
                        {
                            dtr = GetDataTable("select * from " + tableSchema.TableName).CreateDataReader();
                        }

                        if (tableSchema.TableName != "QuotationHeader"
                            && tableSchema.TableName != "VW_QuotationDetails"
                            && tableSchema.TableName != "VWSL_OrderDetails"
                            && tableSchema.TableName != "VWSL_FollowUpEntry"
                            && tableSchema.TableName != "LocalStore"
                            && tableSchema.TableName != "OrderHeader"
                             && tableSchema.TableName != "OrderDiscount"
                            && tableSchema.TableName != "Temp_OrderHeader"
                            && tableSchema.TableName != "VW_TempOrderDetails"
                            && tableSchema.TableName != "NewCustomer"
                            && tableSchema.TableName != "NewCustomerDetails"
                            && tableSchema.TableName != "StockEntryHeader"
                            && tableSchema.TableName != "StockEntryDetails"
                            && tableSchema.TableName != "PaymentHeader"
                            && tableSchema.TableName != "PaymentDetails"
                            && tableSchema.TableName != "MobileWorkingWithHeader"
                            && tableSchema.TableName != "MobileWorkingWithDetails"
                            && tableSchema.TableName != "MobilePunching"
                            && tableSchema.TableName != "VW_ChequeRemittance"
                            && tableSchema.TableName != "OrderSpecialInstruction"
                            && tableSchema.TableName != "BankForOrder"
                            && tableSchema.TableName != "SalesPromotionHeader"
                            && tableSchema.TableName != "SalesPromotionDetails"
                            && tableSchema.TableName != "ExpenseEntry"
                            && tableSchema.TableName != "PromotionalResult"
                             && tableSchema.TableName != "LeaveRequest"
                             && tableSchema.TableName != "MobileFeedback"
                            && tableSchema.TableName != "WorkingArea"
                            && tableSchema.TableName != "POPEntry"
                            && tableSchema.TableName != "Enquiry"
                             && tableSchema.TableName != "Appointment"
                             && tableSchema.TableName != "ComplaintEntry"
                            && tableSchema.TableName != "StockReturn"
                            && tableSchema.TableName != "ParameterCapture"
                            && tableSchema.TableName != "sqlite_sequence"
                            && tableSchema.TableName != "android_metadata"
                             && tableSchema.TableName != "GPSLocationInfo"
                             && tableSchema.TableName != "PaymentDiscountDetails"
                            && tableSchema.TableName != "ShopInAndOutLog"
                            && tableSchema.TableName != "StockHeader"
                            && tableSchema.TableName != "Stock"
                            && tableSchema.TableName != "PhotoCapture"
                            && tableSchema.TableName != "ProductDetailedHeader"
                            && tableSchema.TableName != "ProductDetailedDetails"
                             && tableSchema.TableName != "SampleRequestHeader"
                            && tableSchema.TableName != "SampleRequestDetails"
                            && tableSchema.TableName != "DoctorsContributionHeader"
                               && tableSchema.TableName != "DoctorsContributionDetails"
                            && tableSchema.TableName != "MaterialDelivery"
                             && tableSchema.TableName != "Temp_OrderDiscounts"
                            && tableSchema.TableName != "ShopGpsLocations"
                            && tableSchema.TableName != "StockReconcileHeader"
                            && tableSchema.TableName != "StockReconcile"
                            && tableSchema.TableName != "Signature"
                            && tableSchema.TableName != "Shop"
                            && tableSchema.TableName != "DoctorVisitTime"
                            && tableSchema.TableName != "ReceivedStockHeader"
                            && tableSchema.TableName != "ReceivedStockDetails"
                            && tableSchema.TableName != "Remittance"
                            && tableSchema.TableName != "PreferredVisitTime"
                            && tableSchema.TableName != "StockAgingHeader"
                            && tableSchema.TableName != "StockAgingDetails"
                            && tableSchema.TableName != "ExpenseEntryDetails"
                            && tableSchema.TableName != "ExpenseEntryHead"
                            && tableSchema.TableName != "PunchInDetails"
                            && tableSchema.TableName != "TodaysPlanDetails"
                            && tableSchema.TableName != "BeatPlanDeviation"
                            && tableSchema.TableName != "PjpHeader"
                            && tableSchema.TableName != "PjpDetails"
                            && tableSchema.TableName != "EditCustomers"
                            /* asset master */
                            && tableSchema.TableName != "AssetRequests"
                            /* asset master */
                            && tableSchema.TableName != "ShopWiseDistributor"
                            && tableSchema.TableName != "TransactionResult"
                            && tableSchema.TableName != "VW_DeliveryHeader"
                            && tableSchema.TableName != "VW_Delivery"
                            && tableSchema.TableName != "VW_StockTable"
                            && tableSchema.TableName != "BTLActivityDetails"
                            && tableSchema.TableName != "BTLDetailsConfig"
                            && tableSchema.TableName != "BTLActivityAttendees"
                            && tableSchema.TableName != "MonthlyTourPlans"
                            && tableSchema.TableName != "DailyTourPlans"
                            && tableSchema.TableName != "RoutePlans"
                            && tableSchema.TableName != "JointWorkPlans"                           
                            && tableSchema.TableName != "ActivityPlans"
 
                            /* Joint Working   */
                            && tableSchema.TableName != "JointWorkSurveyResults_Transaction"
                            /* Joint Working */
                            /* Joint Working   */
                            && tableSchema.TableName != "JointWorkShopInAndOutLog_Transaction"
                            /* Joint Working */
                            /* Collection Settlement   */
                            && tableSchema.TableName != "CollectionSettlement_Transaction"
                            /* Collection Settlement */
 
                            && tableSchema.TableName != "ActivityHeader"
                            && tableSchema.TableName != "ActivityDetails"
                            && tableSchema.TableName != "ActivityInandOutLog"
                            && tableSchema.TableName != "ActivityDeviations"
                            && tableSchema.TableName  != "RemittanceDenominations"
                            )
                        {
                            SqlCommand insert = BuildSQLServerInsert(tableSchema);
                            int counter = 0;
                            int k = 0;
                            while (dtr.Read())
                            {
                                insert.Connection = ssconn;

                                insert.Transaction = trn;
                                List<string> pnames = new List<string>();

                                k = 0;
                                foreach (SQLiteColumnSchema column in tableSchema.Columns)
                                {
                                    if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                                    {
                                        if (column.ColumnName == "SyncDate")
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = mobileSyncDate;
                                            pnames.Add(pname);
                                        }
                                        else if (column.ColumnName == "MobileSyncDate")
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = mobileDate;
                                            pnames.Add(pname);
                                        }

                                        else
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = CastValueForColumn(dtr[k], column);
                                            pnames.Add(pname);
                                        }
                                    }
                                    k++;
                                }
                                insert.ExecuteNonQuery();
                                counter++;
                                trn.Commit();                                
                                trn = ssconn.BeginTransaction();
                            }
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }
                        else if (tableSchema.TableName == "MobilePunching")
                        {
                            SqlCommand insert = BuildSQLServerInsert(tableSchema);
                            //int counter = 0;
                            int k = 0;
                            
                            int mobilePunchInId = 0;
                            string mobilePunchData = "";

                            string userId = "", punchInTime = "", punchOutTime = "", latitude = ""
                            , longitude = "", processName = "", mobileTransactionDate = "", startReading = "", endReading = "", mobileReferenceNo = "", gpsSource = "",
                            TravelModeData = "", TravelModeAnswerData = "";
                            
                            DataTable dtrMobilePunchInMaster = GetDataTable("select * from MobilePunching");


                            foreach (DataRow row in dtrMobilePunchInMaster.Rows)
                            {
                                string processId = "";
                                punchInTime = dtrMobilePunchInMaster.Rows[k]["PunchInTime"].ToString();
                                punchOutTime = dtrMobilePunchInMaster.Rows[k]["PunchOutTime"].ToString();
                                
                               
                                userId = dtrMobilePunchInMaster.Rows[k]["UserId"].ToString();
                                mobileReferenceNo = dtrMobilePunchInMaster.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrMobilePunchInMaster.Rows[k]["MobilePunchingId"].ToString(), out mobilePunchInId);

                                if (String.Compare(punchInTime, "1/1/1900 12:00:00 AM") == 0) 
                                {
                                    startReading = "";
                                    endReading = dtrMobilePunchInMaster.Rows[k]["EndReading"].ToString();
                                    processName = "btnPunchOut";
                                    punchInTime = "";
                                    processId = "20";
                                } 
                                else 
                                {
                                    startReading = dtrMobilePunchInMaster.Rows[k]["StartReading"].ToString();
                                    endReading = "";
                                    processName = "btnPunchIn";
                                    punchOutTime = "";
                                    processId = "17";
                                }
                                
                                int k1 = 0;

                                DataTable dtrGPSForPunch = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + mobilePunchInId + " and ProcessId =" + processId);

                                if (dtrGPSForPunch.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForPunch.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForPunch.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForPunch.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForPunch.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForPunch.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForPunch.Rows[0]["Source"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                }

                                DataTable dtrPunchInDetails = GetDataTable("select TravelModeId,TravelModeAnswer from PunchInDetails WHERE  MobilePunchingId=" + mobilePunchInId);

                                foreach (DataRow rows in dtrPunchInDetails.Rows)
                                {
                                    string travelMode = dtrPunchInDetails.Rows[k1]["TravelModeId"].ToString();
                                    string travelModeAnswer = dtrPunchInDetails.Rows[k1]["TravelModeAnswer"].ToString();                                    

                                    if (TravelModeData != "")
                                    {
                                        TravelModeData = TravelModeData + "," + travelMode; ;
                                    }
                                    else
                                    {
                                        TravelModeData = travelMode;
                                    }

                                    if (TravelModeAnswerData != "")
                                    {
                                        TravelModeAnswerData = TravelModeAnswerData + "," + travelModeAnswer; ;
                                    }
                                    else
                                    {
                                        TravelModeAnswerData = travelModeAnswer;
                                    }
                                    k1++;
                                }

                                string punchSubData = "";
                                punchSubData = punchInTime + "@" + punchOutTime + "@" + startReading + "@" + endReading + "@" + latitude + "@" + longitude
                                    + "@" + processName + "@" + mobileTransactionDate + "@" + mobileReferenceNo + "@" + gpsSource + "@" + TravelModeData
                                    + "@" + TravelModeAnswerData;

                                if (mobilePunchData != "")
                                {
                                    mobilePunchData = mobilePunchData + "#" + punchSubData;
                                }
                                else
                                {
                                    mobilePunchData = punchSubData;
                                }
                                k++;
                            }

                            dataArray[17] = mobilePunchData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");      
                        }


                        else if (tableSchema.TableName == "ShopInAndOutLog")
                        {                            
                            int k = 0;


                            int shopInId = 0;
                            string shopInData = "";

                            string userId = "", shopIn = "", shopOut = "", latitude = "", shopId = ""
                            , longitude = "", processName = "", mobileReferenceNo = "", gpsSource = "", mobileTransactionDate = "", SignalStrength = "", NetworkProvider = "", IsGpsForciblyEnabled = "";

                            DataTable dtrShopInMaster = GetDataTable("select * from ShopInAndOutLog");


                            foreach (DataRow row in dtrShopInMaster.Rows)
                            {
                                string processId = "";
                                shopIn = dtrShopInMaster.Rows[k]["ShopIn"].ToString();
                                shopOut = dtrShopInMaster.Rows[k]["ShopOut"].ToString();
                                userId = dtrShopInMaster.Rows[k]["UserId"].ToString();
                                mobileReferenceNo = dtrShopInMaster.Rows[k]["MobileReferenceNo"].ToString();
                                shopId = dtrShopInMaster.Rows[k]["ShopId"].ToString();
                                mobileTransactionDate = dtrShopInMaster.Rows[k]["MobileTransactionDate"].ToString();
                                int.TryParse(dtrShopInMaster.Rows[k]["LogId"].ToString(), out shopInId);

                                if (String.Compare(shopIn, "1/1/1900 12:00:00 AM") == 0)
                                {
                                    processName = "btnShopOut";
                                    shopIn = "";
                                    processId = "33";
                                }
                                else
                                {
                                    processName = "btnShopIn";
                                    shopOut = "";
                                    processId = "32";
                                }

                                DataTable dtrGPSForShopIn = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId,IsGpsForciblyEnabled from GPSLocationInfo WHERE  ProcessDetailsId=" + shopInId + " and ProcessId =" + processId);

                                if (dtrGPSForShopIn.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForShopIn.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForShopIn.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForShopIn.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForShopIn.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForShopIn.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForShopIn.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForShopIn.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForShopIn.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForShopIn.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForShopIn.Rows[0]["NetworkProviderId"].ToString();
                                    IsGpsForciblyEnabled = dtrGPSForShopIn.Rows[0]["IsGpsForciblyEnabled"].ToString() == "" ? "0" : dtrGPSForShopIn.Rows[0]["IsGpsForciblyEnabled"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                    IsGpsForciblyEnabled = "0";
                                }


                                string shopInSubData = "";
                                shopInSubData = shopIn + "@" + shopOut + "@" + latitude + "@" + longitude
                                    + "@" + processName + "@" + mobileReferenceNo + "@" + gpsSource + "@" + shopId + "@" + mobileTransactionDate + "@" + SignalStrength + "@" + NetworkProvider
                                    + "@" + IsGpsForciblyEnabled; ;

                                if (shopInData != "")
                                {
                                    shopInData = shopInData + "#" + shopInSubData;
                                }
                                else
                                {
                                    shopInData = shopInSubData;
                                }
                                k++;
                            }

                            dataArray[18] = shopInData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }


                        else if (tableSchema.TableName == "ReceivedStockHeader")
                        {                            
                            int k = 0;


                            int receivedStockHeaderId = 0;
                            string receivedStockData = "";

                            string userId = "", shopId = "",  latitude = "", longitude = "", productData = "", quantityData = "", unitData = "",
                                processName = "", mobileReferenceNo = "", gpsSource = "", mobileTransactionDate = "";

                            DataTable dtrReceivedStockMaster = GetDataTable("select * from ReceivedStockHeader");

                            processName = "btnRecievedQuantity";

                            foreach (DataRow row in dtrReceivedStockMaster.Rows)
                            {
                                shopId = dtrReceivedStockMaster.Rows[k]["ShopId"].ToString();
                                mobileReferenceNo = dtrReceivedStockMaster.Rows[k]["MobileReferenceNo"].ToString();
                                mobileTransactionDate = dtrReceivedStockMaster.Rows[k]["MobileTransactionDate"].ToString();
                                int.TryParse(dtrReceivedStockMaster.Rows[k]["Id"].ToString(), out receivedStockHeaderId);



                                DataTable dtrGPSForReceivedStock = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + receivedStockHeaderId + " and ProcessId =" + 44);

                                if (dtrGPSForReceivedStock.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForReceivedStock.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForReceivedStock.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForReceivedStock.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForReceivedStock.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForReceivedStock.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForReceivedStock.Rows[0]["Source"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                }
                                int k1 = 0;
                                DataTable dtrReceivedStockDetails = GetDataTable("select * from ReceivedStockDetails WHERE  ReceivedStockHeaderId=" + receivedStockHeaderId);
                                foreach (DataRow rows in dtrReceivedStockDetails.Rows)
                                {
                                    string productId = dtrReceivedStockDetails.Rows[k1]["ProductAttributeId"].ToString();
                                    string quantity = dtrReceivedStockDetails.Rows[k1]["Quantity"].ToString();
                                    string unit = dtrReceivedStockDetails.Rows[k1]["UnitId"].ToString();

                                    if (productData != "")
                                    {
                                        productData = productData + "," + productId; ;
                                    }
                                    else
                                    {
                                        productData = productId;
                                    }
                                    if (quantityData != "")
                                    {
                                        quantityData = quantityData + "," + quantity;

                                    }
                                    else
                                    {
                                        quantityData = quantity;
                                    }
                                    if (unitData != "")
                                    {
                                        unitData = unitData + "," + unit;
                                    }
                                    else
                                    {
                                        unitData = unit;
                                    }

                                    k1++;

                                }


                                string receivedStockSubData = "";
                                receivedStockSubData = shopId + "@" + productData + "@" + quantityData + "@" + unitData
                                    + "@" + processName + "@" + mobileReferenceNo + "@" + latitude + "@" + longitude + "@" + gpsSource + "@" + mobileTransactionDate;

                                if (receivedStockData != "")
                                {
                                    receivedStockData = receivedStockData + "#" + receivedStockSubData;
                                }
                                else
                                {
                                    receivedStockData = receivedStockSubData;
                                }
                                k++;
                            }

                            dataArray[28] = receivedStockData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }

            /*************************************************************************************************************************************************************/


                        else if (tableSchema.TableName == "PhotoCapture")
                        {

                            int k = 0;
                            int PhotoCaptureId = 0;


                            string PhotoCaptureData = "";

                            string userId = "", shopId = "", imageName = "", captureDate = "", imageDescription = "",
                            lattitude = "", longitude = "", mobRefNo = "", imageData = "", photoDescTypeId = "", processId = "", source = "",processDetailsId="",
                            tempShopId = "", SignalStrength = "", NetworkProvider = "";


                            DataTable dtrPhotoCapture = GetDataTable("select * from PhotoCapture ");

                            foreach (DataRow row in dtrPhotoCapture.Rows)
                            {
                                userId = string.Empty;
                                shopId = string.Empty;
                                imageName = string.Empty;
                                captureDate = string.Empty;
                                imageDescription = string.Empty;
                                lattitude = string.Empty;
                                longitude = string.Empty;
                                imageData = string.Empty;
                                mobRefNo = string.Empty;
                                photoDescTypeId = string.Empty;
                                processDetailsId = string.Empty;
                                tempShopId = string.Empty;

                                userId = dtrPhotoCapture.Rows[k]["UserId"].ToString();
                                shopId = dtrPhotoCapture.Rows[k]["ShopId"].ToString();
                                imageName = dtrPhotoCapture.Rows[k]["ImageName"].ToString();
                                int.TryParse(dtrPhotoCapture.Rows[k]["PhotoCaptureId"].ToString(), out PhotoCaptureId);
                                captureDate = dtrPhotoCapture.Rows[k]["CaptureDate"].ToString();
                                mobRefNo = dtrPhotoCapture.Rows[k]["MobileReferenceNo"].ToString();
                                imageData = dtrPhotoCapture.Rows[k]["ImageData"].ToString();
                                imageDescription = dtrPhotoCapture.Rows[k]["ImageDescription"].ToString();
                                imageDescription = Regex.Replace(imageDescription, "#", "");
                                photoDescTypeId = dtrPhotoCapture.Rows[k]["PhotoDescriptionTypeId"].ToString();
                                processId = dtrPhotoCapture.Rows[k]["ProcessId"].ToString();
                                processDetailsId = dtrPhotoCapture.Rows[k]["ProcessDetailsId"].ToString();
                                tempShopId =  dtrPhotoCapture.Rows[k]["TempShopId"].ToString();

                                DataTable dtrGPSForStockHeader = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + PhotoCaptureId + " and ProcessId =" + processId);

                                if (dtrGPSForStockHeader.Rows.Count > 0)
                                {
                                    lattitude = dtrGPSForStockHeader.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForStockHeader.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForStockHeader.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForStockHeader.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForStockHeader.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForStockHeader.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForStockHeader.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForStockHeader.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForStockHeader.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForStockHeader.Rows[0]["NetworkProviderId"].ToString();
                                }
                                else
                                {
                                    lattitude = "999";
                                    longitude = "999";
                                    source = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }
                                string PhotoCaptureMasterData = "";
                                PhotoCaptureMasterData = imageData + "~" + imageName + "~" + lattitude + "~" + longitude + "~" + imageDescription + "~" +
                                              mobRefNo + "~" + shopId + "~" + photoDescTypeId + "~" + captureDate + "~" + processId + "~" + source + "~" + mobileDate + "~" + processDetailsId
                                              + "~" + tempShopId + "~" + SignalStrength + "~" + NetworkProvider;

                                if (PhotoCaptureData != "")
                                {
                                    PhotoCaptureData = PhotoCaptureData + "#" + PhotoCaptureMasterData;

                                }
                                else
                                {
                                    PhotoCaptureData = PhotoCaptureMasterData;
                                }

                                k++;

                            }

                            DataQuotation = PhotoCaptureData;
                            dataArray[5] = DataQuotation;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }

            /*************************************************************************************************************************************************************/


                        else if (tableSchema.TableName == "Shop")
                        {

                            int k = 0;
                            string ShopData = "";

                            string shopName = "", address = "", mobileNo = "",
                            email = "", dob = "", weddingDate = "", qualifications = "";


                            DataTable dtrPhotoCapture = GetDataTable("select * from Shop");

                            foreach (DataRow row in dtrPhotoCapture.Rows)
                            {
                                shopName = string.Empty;
                                address = string.Empty;
                                mobileNo = string.Empty;
                                email = string.Empty;
                                dob = string.Empty;
                                weddingDate = string.Empty;
                                qualifications = string.Empty;

                                shopName = dtrPhotoCapture.Rows[k]["Name"].ToString();
                                address = dtrPhotoCapture.Rows[k]["Address"].ToString();
                                mobileNo = dtrPhotoCapture.Rows[k]["MobileNo"].ToString();
                                email = dtrPhotoCapture.Rows[k]["Email"].ToString();
                                dob = dtrPhotoCapture.Rows[k]["DateOfBirth"].ToString();
                                weddingDate = dtrPhotoCapture.Rows[k]["WeddingDate"].ToString();
                                qualifications = dtrPhotoCapture.Rows[k]["Qualifications"].ToString();
                                
                                string ShopMasterData = "";
                                ShopMasterData = shopName + "~" + address + "~" + mobileNo + "~" + email + "~" + dob + "~" +
                                              weddingDate + "~" + qualifications ;

                                if (ShopData != "")
                                {
                                    ShopData = ShopData + "#" + ShopMasterData;

                                }
                                else
                                {
                                    ShopData = ShopMasterData;
                                }

                                k++;

                            }
                            dataArray[30] = ShopData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }

                        else if (tableSchema.TableName == "DoctorVisitTime")
                        {

                            int k = 0;
                            string VisitTimeData = "";

                            string hospitalVisitId = "", visitDay = "", fromTime = "",
                            toTime = "";


                            DataTable dtrPhotoCapture = GetDataTable("select * from DoctorVisitTime ");

                            foreach (DataRow row in dtrPhotoCapture.Rows)
                            {
                                hospitalVisitId = string.Empty;
                                visitDay = string.Empty;
                                fromTime = string.Empty;
                                toTime = string.Empty;

                                hospitalVisitId = dtrPhotoCapture.Rows[k]["HospitalVisitId"].ToString();
                                visitDay = dtrPhotoCapture.Rows[k]["VisitDay"].ToString();
                                fromTime = dtrPhotoCapture.Rows[k]["FromTime"].ToString();
                                toTime = dtrPhotoCapture.Rows[k]["ToTime"].ToString();

                                string VisitTimeMasterData = "";
                                VisitTimeMasterData = hospitalVisitId + "~" + visitDay + "~" + fromTime + "~" + toTime;

                                if (VisitTimeData != "")
                                {
                                    VisitTimeData = VisitTimeData + "#" + VisitTimeMasterData;

                                }
                                else
                                {
                                    VisitTimeData = VisitTimeMasterData;
                                }

                                k++;

                            }
                            dataArray[29] = VisitTimeData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }

                        else if (tableSchema.TableName == "PreferredVisitTime")
                        {

                            int k = 0;
                            string PrefVisitTimeData = "";

                            string doctorId = "", visitDay = "", fromTime = "",
                            toTime = "";


                            DataTable dtrPhotoCapture = GetDataTable("select * from PreferredVisitTime ");

                            foreach (DataRow row in dtrPhotoCapture.Rows)
                            {
                                doctorId = string.Empty;
                                visitDay = string.Empty;
                                fromTime = string.Empty;
                                toTime = string.Empty;

                                doctorId = dtrPhotoCapture.Rows[k]["DoctorId"].ToString();
                                visitDay = dtrPhotoCapture.Rows[k]["VisitDay"].ToString();
                                fromTime = dtrPhotoCapture.Rows[k]["FromTime"].ToString();
                                toTime = dtrPhotoCapture.Rows[k]["ToTime"].ToString();

                                string VisitTimeMasterData = "";
                                VisitTimeMasterData = doctorId + "~" + visitDay + "~" + fromTime + "~" + toTime;

                                if (PrefVisitTimeData != "")
                                {
                                    PrefVisitTimeData = PrefVisitTimeData + "#" + VisitTimeMasterData;

                                }
                                else
                                {
                                    PrefVisitTimeData = VisitTimeMasterData;
                                }

                                k++;

                            }
                            dataArray[31] = PrefVisitTimeData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }


                        else if (tableSchema.TableName == "VWSL_FollowUpEntry")
                        {

                            SqlCommand insert = BuildSQLServerInsertForFollowup(tableSchema);
                            int counter = 0;
                            int k = 0;
                            while (dtr.Read())
                            {
                                insert.Connection = ssconn;
                                insert.Transaction = trn;
                                List<string> pnames = new List<string>();

                                k = 0;
                                foreach (SQLiteColumnSchema column in tableSchema.Columns)
                                {
                                    if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                                    {
                                        if (column.ColumnName == "SyncDate")
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = mobileSyncDate;
                                            pnames.Add(pname);
                                        }
                                        else if (column.ColumnName == "MobileSyncDate")
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = mobileDate;
                                            pnames.Add(pname);
                                        }
                                        else if (column.ColumnName == "ShopId" || column.ColumnName == "UserId" || column.ColumnName == "FollowUpId" || column.ColumnName == "Remarks" || column.ColumnName == "FollowUpDate" || column.ColumnName == "IsCalendarSync")
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = CastValueForColumn(dtr[k], column);
                                            pnames.Add(pname);
                                        }
                                    }
                                    k++;
                                }
                                //SqlCommand cmd = new SqlCommand(s, ssconn);
                                insert.ExecuteNonQuery();
                                counter++;
                                //if (counter % 1000 == 0)
                                //{
                                trn.Commit();
                                //    //"Added " + counter + " rows to table " + schema[i].TableName + " so far");
                                trn = ssconn.BeginTransaction();
                                //}
                            }
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }
                        //****************************************Sales Promotion********************************************//
                        else if (tableSchema.TableName == "SalesPromotionHeader")
                        {
                            int k = 0;
                            int counter = 0;
                            int SalesHeaerId = 0;


                            string SalesData = "";

                            string userId = "", shopId = "", productData = "", quantitydata = "",
                           lattitude = "999", longitude = "999", processName = "", narrartion = "", source = "",
                            UnitId = "", mobileTransactionDate = "", mobileReferenceNo = "", SignalStrength = "", NetworkProvider = "";

                            DataTable dtrSalesPromotionHeader = GetDataTable("select * from SalesPromotionHeader ");
                            foreach (DataRow row in dtrSalesPromotionHeader.Rows)
                            {
                                productData = string.Empty;
                                quantitydata = string.Empty;
                                UnitId = string.Empty;


                                shopId = dtrSalesPromotionHeader.Rows[k]["ShopId"].ToString();
                                userId = dtrSalesPromotionHeader.Rows[k]["UserId"].ToString();
                                narrartion = dtrSalesPromotionHeader.Rows[k]["Narration"].ToString();
                                mobileTransactionDate = dtrSalesPromotionHeader.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = dtrSalesPromotionHeader.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrSalesPromotionHeader.Rows[k]["SalesHeaderId"].ToString(), out SalesHeaerId);
                                int k1 = 0;

                                DataTable dtrSalesPromotionDetails = GetDataTable("select * from SalesPromotionDetails WHERE  SalesHeaderId=" + SalesHeaerId);
                                DataTable dtrGPSForSales = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + SalesHeaerId + " and ProcessId =" + 31);
                                foreach (DataRow rows in dtrSalesPromotionDetails.Rows)
                                {
                                    string productId = dtrSalesPromotionDetails.Rows[k1]["ProductId"].ToString();
                                    string quantity = dtrSalesPromotionDetails.Rows[k1]["Quantity"].ToString();
                                    string unit = dtrSalesPromotionDetails.Rows[k1]["UnitId"].ToString();

                                    if (productData != "")
                                    {
                                        productData = productData + "," + productId; ;
                                    }
                                    else
                                    {
                                        productData = productId;
                                    }
                                    if (quantitydata != "")
                                    {
                                        quantitydata = quantitydata + "," + quantity;

                                    }
                                    else
                                    {
                                        quantitydata = quantity;
                                    }
                                    if (UnitId != "")
                                    {
                                        UnitId = UnitId + "," + unit;
                                    }
                                    else
                                    {
                                        UnitId = unit;
                                    }

                                    k1++;

                                }
                                if (dtrGPSForSales.Rows.Count > 0)
                                {
                                    lattitude = dtrGPSForSales.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForSales.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForSales.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForSales.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForSales.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForSales.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForSales.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForSales.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForSales.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForSales.Rows[0]["NetworkProviderId"].ToString();
                                }
                                else
                                {
                                    lattitude = "999";
                                    longitude = "999";
                                    source = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }
                                string SalesMasterData = "";
                                SalesMasterData = userId + "~" + shopId + "~" + productData + "~" + narrartion + "~" + quantitydata + "~" + lattitude + "~" +
                                              longitude + "~" + "Sales" + "~" + UnitId + "~" + mobileTransactionDate + "~" + mobileReferenceNo + "~" + source + "~" + mobileDate + "~" + SignalStrength + "~" + NetworkProvider;

                                if (SalesData != "")
                                {
                                    SalesData = SalesData + "#" + SalesMasterData;

                                }
                                else
                                {
                                    SalesData = SalesMasterData;
                                }

                                k++;

                            }
                            dataArray[2] = SalesData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");


                        }
                        else if (tableSchema.TableName == "ProductDetailedHeader")
                        {
                            int k = 0;
                            int ProductDetailedHeaderId = 0;
                            string ProductDetailedData = "";
                            string userId = "", shopId = "", productData = "",
                            lattitude = "999", longitude = "999", detailedDate = "", mobileTransactionDate = "", mobileReferenceNo = "", source = "",remark="";

                            DataTable ProductDetailedHeader = GetDataTable("select * from ProductDetailedHeader ");
                            foreach (DataRow row in ProductDetailedHeader.Rows)
                            {
                                productData = string.Empty;


                                shopId = ProductDetailedHeader.Rows[k]["ShopId"].ToString();
                                userId = ProductDetailedHeader.Rows[k]["UserId"].ToString();
                                mobileTransactionDate = ProductDetailedHeader.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = ProductDetailedHeader.Rows[k]["MobileReferenceNo"].ToString();
                                detailedDate = ProductDetailedHeader.Rows[k]["DetailedDate"].ToString();
                                remark = ProductDetailedHeader.Rows[k]["Remark"].ToString();
                                int.TryParse(ProductDetailedHeader.Rows[k]["Id"].ToString(), out ProductDetailedHeaderId);
                                int k1 = 0;

                                DataTable dtrProductDetailedDetails = GetDataTable("select * from ProductDetailedDetails WHERE  ProductDetailedHeaderId=" + ProductDetailedHeaderId);
                                DataTable dtrGPSForSales = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + ProductDetailedHeaderId + " and ProcessId =" + 37);
                                foreach (DataRow rows in dtrProductDetailedDetails.Rows)
                                {
                                    string productId = dtrProductDetailedDetails.Rows[k1]["MaterialId"].ToString();

                                    if (productData != "")
                                    {
                                        productData = productData + "," + productId; ;
                                    }
                                    else
                                    {
                                        productData = productId;
                                    }
                                    k1++;

                                }
                                if (dtrGPSForSales.Rows.Count > 0)
                                {
                                    lattitude = dtrGPSForSales.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForSales.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForSales.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForSales.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForSales.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForSales.Rows[0]["Source"].ToString();
                                }
                                else
                                {
                                    lattitude = "999";
                                    longitude = "999";
                                    source = "";
                                }
                                string SalesMasterData = "";
                                SalesMasterData = userId + "~" + shopId + "~" + productData + "~" + lattitude + "~" +
                                              longitude + "~" + "btnProductDetailed" + "~" + detailedDate + "~" + mobileTransactionDate + "~" + mobileReferenceNo + "~" + source + "~" + mobileDate + "~" + remark;

                                if (ProductDetailedData != "")
                                {
                                    ProductDetailedData = ProductDetailedData + "#" + SalesMasterData;

                                }
                                else
                                {
                                    ProductDetailedData = SalesMasterData;
                                }

                                k++;

                            }

                            dataArray[6] = ProductDetailedData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");


                        }
                        else if (tableSchema.TableName == "SampleRequestHeader")
                        {
                            int k = 0;
                            int SampleRequestHeaderId = 0;
                            string ampleRequestData = "";
                            string userId = "", shopId = "", productData = "", deliveredQty = "", requestedQty = "",
                            lattitude = "999", longitude = "999", submissionDate = "", mobileTransactionDate = "", mobileReferenceNo = "", source = "";

                            DataTable SampleRequestHeader = GetDataTable("select * from SampleRequestHeader ");
                            foreach (DataRow row in SampleRequestHeader.Rows)
                            {
                                productData = string.Empty;


                                shopId = SampleRequestHeader.Rows[k]["ShopId"].ToString();
                                userId = SampleRequestHeader.Rows[k]["UserId"].ToString();
                                mobileTransactionDate = SampleRequestHeader.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = SampleRequestHeader.Rows[k]["MobileReferenceNo"].ToString();
                                submissionDate = SampleRequestHeader.Rows[k]["SubmissionDate"].ToString();
                                int.TryParse(SampleRequestHeader.Rows[k]["Id"].ToString(), out SampleRequestHeaderId);
                                int k1 = 0;

                                DataTable dtrSampleRequestDetails = GetDataTable("select * from SampleRequestDetails WHERE  SampleRequestHeaderId=" + SampleRequestHeaderId);
                                DataTable dtrGPSForSales = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + SampleRequestHeaderId + " and ProcessId =" + 36);
                                foreach (DataRow rows in dtrSampleRequestDetails.Rows)
                                {
                                    string productId = dtrSampleRequestDetails.Rows[k1]["ProductAttributeId"].ToString();
                                    string delQuantity = dtrSampleRequestDetails.Rows[k1]["DeliveredQuantity"].ToString();
                                    string reqQuantity = dtrSampleRequestDetails.Rows[k1]["RequestedQuantity"].ToString();

                                    if (productData != "")
                                    {
                                        productData = productData + "," + productId; ;
                                    }
                                    else
                                    {
                                        productData = productId;
                                    }
                                    if (deliveredQty != "")
                                    {
                                        deliveredQty = deliveredQty + "," + delQuantity; ;
                                    }
                                    else
                                    {
                                        deliveredQty = delQuantity;
                                    }
                                    if (requestedQty != "")
                                    {
                                        requestedQty = requestedQty + "," + reqQuantity;
                                    }
                                    else
                                    {
                                        requestedQty = reqQuantity;
                                    }
                                    k1++;

                                }
                                if (dtrGPSForSales.Rows.Count > 0)
                                {
                                    lattitude = dtrGPSForSales.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForSales.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForSales.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForSales.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForSales.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForSales.Rows[0]["Source"].ToString();
                                }
                                else
                                {
                                    lattitude = "999";
                                    longitude = "999";
                                    source = "1";
                                }
                                string SalesMasterData = "";
                                SalesMasterData = userId + "~" + shopId + "~" + productData + "~" + deliveredQty + "~" + requestedQty + "~" + lattitude + "~" +
                                              longitude + "~" + "btnSamples" + "~" + submissionDate + "~" + mobileTransactionDate + "~" + mobileReferenceNo + "~" + source + "~" + mobileDate;

                                if (ampleRequestData != "")
                                {
                                    ampleRequestData = ampleRequestData + "#" + SalesMasterData;

                                }
                                else
                                {
                                    ampleRequestData = SalesMasterData;
                                }

                                k++;

                            }

                            dataArray[7] = ampleRequestData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }
                        else if (tableSchema.TableName == "DoctorsContributionHeader")
                        {
                            int k = 0;
                            int DoctorsContributionHeaderId = 0;
                            string ampleRequestData = "";
                            string userId = "", shopId = "", productData = "", Qty = "", contrbutedBy = "",
                            lattitude = "999", longitude = "999", date = "", mobileTransactionDate = "", mobileReferenceNo = "", source = "";

                            DataTable DoctorsContributionHeader = GetDataTable("select * from DoctorsContributionHeader ");
                            foreach (DataRow row in DoctorsContributionHeader.Rows)
                            {
                                productData = string.Empty;


                                shopId = DoctorsContributionHeader.Rows[k]["ShopId"].ToString();
                                userId = DoctorsContributionHeader.Rows[k]["UserId"].ToString();
                                mobileTransactionDate = DoctorsContributionHeader.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = DoctorsContributionHeader.Rows[k]["MobileReferenceNo"].ToString();
                                date = DoctorsContributionHeader.Rows[k]["Date"].ToString();
                                int.TryParse(DoctorsContributionHeader.Rows[k]["Id"].ToString(), out DoctorsContributionHeaderId);
                                int k1 = 0;

                                DataTable dtrDoctorsContributionDetails = GetDataTable("select * from DoctorsContributionDetails WHERE  DoctorsContributionHeaderId=" + DoctorsContributionHeaderId);
                                DataTable dtrGPSForSales = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + DoctorsContributionHeaderId + " and ProcessId =" + 35);
                                foreach (DataRow rows in dtrDoctorsContributionDetails.Rows)
                                {
                                    string productId = dtrDoctorsContributionDetails.Rows[k1]["ProductAttributeId"].ToString();
                                    string quantity = dtrDoctorsContributionDetails.Rows[k1]["Quantity"].ToString();
                                    string contributed = dtrDoctorsContributionDetails.Rows[k1]["ContributedBy"].ToString();

                                    if (productData != "")
                                    {
                                        productData = productData + "," + productId; ;
                                    }
                                    else
                                    {
                                        productData = productId;
                                    }
                                    if (Qty != "")
                                    {
                                        Qty = Qty + "," + quantity; ;
                                    }
                                    else
                                    {
                                        Qty = quantity;
                                    }
                                    if (contrbutedBy != "")
                                    {
                                        contrbutedBy = contrbutedBy + "," + contributed;
                                    }
                                    else
                                    {
                                        contrbutedBy = contributed;
                                    }
                                    k1++;

                                }
                                if (dtrGPSForSales.Rows.Count > 0)
                                {
                                    lattitude = dtrGPSForSales.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForSales.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForSales.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForSales.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForSales.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForSales.Rows[0]["Source"].ToString();
                                }
                                else
                                {
                                    lattitude = "999";
                                    longitude = "999";
                                    source = "1";
                                }
                                string SalesMasterData = "";
                                SalesMasterData = userId + "~" + shopId + "~" + productData + "~" + Qty + "~" + contrbutedBy + "~" + lattitude + "~" +
                                              longitude + "~" + "btnDoctorContribution" + "~" + date + "~" + mobileTransactionDate + "~" + mobileReferenceNo + "~" + source
                                              + "~" + mobileDate;

                                if (ampleRequestData != "")
                                {
                                    ampleRequestData = ampleRequestData + "#" + SalesMasterData;

                                }
                                else
                                {
                                    ampleRequestData = SalesMasterData;
                                }

                                k++;

                            }
                            dataArray[8] = ampleRequestData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }
                        else if (tableSchema.TableName == "ShopGpsLocations")
                        {
                            int k = 0;

                            string userId = "", shopId = "", latitude = "", longitude = "", Id = ""
                                , currrentlattitude = "", currrentlongitude = "", status = "", mobilereferenceNo = "", SignalStrength = "", NetworkProvider = "", tempShopId = "";
                            DataTable dtrSalesPromotionHeader = GetDataTable("select * from ShopGpsLocations ");
                            foreach (DataRow row in dtrSalesPromotionHeader.Rows)
                            {
                                Id = dtrSalesPromotionHeader.Rows[k]["ShopGPSLocationId"].ToString();

                                DataTable dtrGPSForSales = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + Id + " and mobilereferenceNo='" + dtrSalesPromotionHeader.Rows[k]["MobileReferenceNo"].ToString() + "'");
                                if (shopId.Trim().Length == 0)
                                {
                                    shopId = dtrSalesPromotionHeader.Rows[k]["ShopId"].ToString();
                                    userId = dtrSalesPromotionHeader.Rows[k]["CreatedBy"].ToString();
                                    latitude = dtrSalesPromotionHeader.Rows[k]["Latitude"].ToString();
                                    longitude = dtrSalesPromotionHeader.Rows[k]["Longitude"].ToString();
                                    mobilereferenceNo = dtrSalesPromotionHeader.Rows[k]["MobileReferenceNo"].ToString();
                                    tempShopId = dtrSalesPromotionHeader.Rows[k]["TempShopId"].ToString();
                                    if (dtrGPSForSales.Rows.Count > 0)
                                    {
                                        currrentlattitude = dtrGPSForSales.Rows[0]["latitude"].ToString();
                                        currrentlongitude = dtrGPSForSales.Rows[0]["longitude"].ToString();
                                        status = dtrGPSForSales.Rows[0]["Source"].ToString();
                                        SignalStrength = dtrGPSForSales.Rows[0]["SignalStrength"].ToString();
                                        NetworkProvider = dtrGPSForSales.Rows[0]["NetworkProviderId"].ToString();
                                    }
                                    else
                                    {
                                        currrentlattitude = "999";
                                        currrentlongitude = "999";
                                        status = "1";
                                        SignalStrength = "0";
                                        NetworkProvider = "0";
                                    
                                    }
                                   
                                }
                                else
                                {
                                    shopId += "," + dtrSalesPromotionHeader.Rows[k]["ShopId"].ToString();
                                    userId += "," + dtrSalesPromotionHeader.Rows[k]["CreatedBy"].ToString();
                                    latitude += "," + dtrSalesPromotionHeader.Rows[k]["Latitude"].ToString();
                                    longitude += "," + dtrSalesPromotionHeader.Rows[k]["Longitude"].ToString();
                                    mobilereferenceNo += "," + dtrSalesPromotionHeader.Rows[k]["MobileReferenceNo"].ToString();
                                    tempShopId += "," + dtrSalesPromotionHeader.Rows[k]["TempShopId"].ToString();
                                    if (dtrGPSForSales.Rows.Count > 0)
                                    {
                                        currrentlattitude += "," + dtrGPSForSales.Rows[0]["latitude"].ToString();
                                        currrentlongitude += "," + dtrGPSForSales.Rows[0]["longitude"].ToString();
                                        status += "," + dtrGPSForSales.Rows[0]["Source"].ToString();
                                        SignalStrength += "," + dtrGPSForSales.Rows[0]["SignalStrength"].ToString();
                                        NetworkProvider += "," + dtrGPSForSales.Rows[0]["NetworkProviderId"].ToString();
                                    }
                                    else
                                    {

                                        currrentlattitude += ",999";
                                        currrentlongitude += ",999";
                                        status += ",1";
                                        SignalStrength += ",0";
                                        NetworkProvider += ",0";
 
                                    }
                                    
                                }
                                k++;
                            }
                            string ShoplocationData = "";
                            if (userId.Length > 0)
                            {
                                ShoplocationData = userId + "~" + shopId + "~" + latitude + "~" + longitude + "~" + currrentlattitude + "~" + currrentlongitude + "~" + status + "~" + mobileDate + "~" + mobilereferenceNo + "~" + SignalStrength + "~" + NetworkProvider + "~" + tempShopId;
                            }
                            dataArray[9] = ShoplocationData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");


                        }

                        else if (tableSchema.TableName == "VanUnloadStatus")
                        {
                            SqlCommand insert = BuildSQLServerInsert(tableSchema);
                            int counter = 0;
                            int k = 0;
                            int Id = 0;
                            while (dtr.Read())
                            {
                                insert.Connection = ssconn;
                                insert.Transaction = trn;
                                List<string> pnames = new List<string>();
                                String processdetailsIdSqlite = String.Empty;
                                int processId = 44;
                                k = 0;
                                foreach (SQLiteColumnSchema column in tableSchema.Columns)
                                {
                                    if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                                    {

                                        if (column.ColumnName == "SyncDate")
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = mobileSyncDate;
                                            pnames.Add(pname);
                                        }
                                        else if (column.ColumnName == "MobileSyncDate")
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = mobileDate;
                                            pnames.Add(pname);
                                        }

                                        else
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            var val = dtr[k].ToString();
                                            insert.Parameters[pname].Value = CastValueForColumn(dtr[k], column);
                                            pnames.Add(pname);
                                        }
                                    }
                                    else if (column.ColumnName == "Id")
                                    {
                                        Id = Convert.ToInt32(CastValueForColumn(dtr[k], column));
                                    }

                                    k++;
                                }
                                int id = Convert.ToInt32(insert.ExecuteScalar());
                                //insert.ExecuteNonQuery();

                                trn.Commit();
                                trn = ssconn.BeginTransaction();
                                insertGPSInfoWithoutShop(Convert.ToString(processId), Convert.ToString(id), Convert.ToString(Id), ssconn, trn,syncTables);
                                counter++;
                            }
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }

                        else if (tableSchema.TableName == "MaterialDelivery")
                        {
                            SqlCommand insert = BuildSQLServerInsert(tableSchema);
                            int counter = 0;
                            int k = 0;
                            int Id = 0;
                            while (dtr.Read())
                            {
                                insert.Connection = ssconn;
                                insert.Transaction = trn;
                                List<string> pnames = new List<string>();
                                String processdetailsIdSqlite = String.Empty;
                                int processId = 39;
                                k = 0;
                                foreach (SQLiteColumnSchema column in tableSchema.Columns)
                                {
                                    if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                                    {
                                        if (column.ColumnName == "DeliveryDate")
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = mobileSyncDate;
                                            pnames.Add(pname);
                                        }
                                        else if (column.ColumnName == "SyncDate")
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = mobileSyncDate;
                                            pnames.Add(pname);
                                        }
                                        else if (column.ColumnName == "MobileSyncDate")
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            insert.Parameters[pname].Value = mobileDate;
                                            pnames.Add(pname);
                                        }
                                        else
                                        {
                                            string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                                            var val = dtr[k].ToString();

                                            insert.Parameters[pname].Value = CastValueForColumn(dtr[k], column);

                                            pnames.Add(pname);
                                        }
                                    }
                                    else if (column.ColumnName == "Id")
                                    {
                                        Id = Convert.ToInt32(CastValueForColumn(dtr[k], column));
                                    }

                                    k++;
                                }
                                int id = Convert.ToInt32(insert.ExecuteScalar());
                                //insert.ExecuteNonQuery();

                                trn.Commit();
                                trn = ssconn.BeginTransaction();
                                insertGPSInfo(Convert.ToString(processId), Convert.ToString(id), Convert.ToString(Id), ssconn, trn,syncTables);
                                counter++;

                                //dataArray[33] = "MaterialDelivery";
                            }
                            trn.Commit();

                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }


                        else if (tableSchema.TableName == "VW_ChequeRemittance")
                        {
                            SqlCommand insert = new SqlCommand();
                            Boolean isRemitted;
                            String mobRefNo;
                            String bankId;
                            String remittedDate;
                            string remarks;
                            int counter = 0;
                            int k = 0;
                            while (dtr.Read())
                            {
                                insert.Connection = ssconn;
                                insert.Transaction = trn;
                                List<string> pnames = new List<string>();
                                isRemitted = (Boolean)dtr[4];

                                if (isRemitted)
                                {
                                    mobRefNo = dtr["MobileReferenceNo"].ToString();
                                    bankId = dtr["RemittedAt"].ToString();
                                    remittedDate = dtr["RemittedDate"].ToString();
                                    remarks = dtr["Remarks"].ToString();
                                    insert.CommandText = "Update PaymentHeader SET isRemitted = 1, remittedat = '" + bankId + "', RemittedDate = '" + remittedDate + "' , Remarks = '" + remarks + "' WHERE MobileReferenceNo = '" + mobRefNo + "'";
                                    insert.ExecuteNonQuery();
                                }
                                //,RemittedDate = " + remittedDate + ",RemittedAt = '" + bankId + "'
                                //SqlCommand cmd = new SqlCommand(s, ssconn);

                                counter++;
                                //if (counter % 1000 == 0)
                                //{
                                trn.Commit();
                                //    //"Added " + counter + " rows to table " + schema[i].TableName + " so far");
                                trn = ssconn.BeginTransaction();
                                //}
                            }
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }

                        else if (tableSchema.TableName == "OrderHeader")
                        {
                            int k = 0;
                            int OrderId = 0;
                            string distributorId = "";


                            string OrderData = "";

                            string userId = "", shopId = "", unitSet = "", productData = "", quantitydata = "",
                            scheme = "", mode = "5", specialInstruction = "", otherInstns = "", priority = "",
                            siteAddress = "", contactPerson = "", phone = "", price = "", mobileNo = "", latitude = "", longitude = "", processName = "", totalAmount = "",
                            firstDiscount = "", secondDiscount = "", orderDate = "", bankName = "", orderDiscount = "", freeQty = "", unitDiscount = "", mobileDiscFlag = "", mobileReferenceNo = "", orderDiscountIds = "", orderDiscountValues = "",
                            deliveryDate, paymentmode, source, schemIdFromMobile = "", SignalStrength="",requestedQuantityData="",tempShopId="",totalDiscountData="",taxAmountData=""
                            ,NetworkProvider="",invoiceNo="";

                            DataTable dtrorderMaster = GetDataTable("select * from OrderHeader "); //dtr have all data from orderheader, this loop will exc. no. of rows times 

                            foreach (DataRow row in dtrorderMaster.Rows)
                            {
                                productData = string.Empty;
                                quantitydata = string.Empty;
                                unitSet = string.Empty;
                                price = string.Empty;
                                totalAmount = string.Empty;
                                firstDiscount = string.Empty;
                                secondDiscount = string.Empty;
                                deliveryDate = string.Empty;
                                paymentmode = string.Empty;
                                unitDiscount = string.Empty;
                                freeQty = string.Empty;
                                mobileDiscFlag = string.Empty;
                                orderDiscountIds = string.Empty;
                                orderDiscountValues = string.Empty;
                                requestedQuantityData = string.Empty;
                                totalDiscountData = string.Empty;
                                taxAmountData = string.Empty;
                                orderDate = dtrorderMaster.Rows[k]["MobileOrderDate"].ToString();
                                shopId = dtrorderMaster.Rows[k]["ShopId"].ToString();
                                userId = dtrorderMaster.Rows[k]["OrderTakenBy"].ToString();
                                mobileReferenceNo = dtrorderMaster.Rows[k]["MobileReferenceNo"].ToString();
                                otherInstns = dtrorderMaster.Rows[k]["OtherInstruction"].ToString();
                                orderDiscount = dtrorderMaster.Rows[k]["OrderDiscount"].ToString();
                                int.TryParse(dtrorderMaster.Rows[k]["OrderId"].ToString(), out OrderId);
                                priority = dtrorderMaster.Rows[k]["Priority"].ToString();
                                scheme = dtrorderMaster.Rows[k]["SchemeId"].ToString() == "" ? "0" : dtrorderMaster.Rows[k]["SchemeId"].ToString();
                                deliveryDate = dtrorderMaster.Rows[k]["DeliveryDate"].ToString();
                                paymentmode = dtrorderMaster.Rows[k]["PaymentMode"].ToString();
                                tempShopId = dtrorderMaster.Rows[k]["TempShopId"].ToString();
                                invoiceNo = dtrorderMaster.Rows[k]["InvoiceNo"].ToString();
                                int k1 = 0;
                                int k2 = 0;

                                DataTable dtrOrderDetails = GetDataTable("select * from VWSL_OrderDetails WHERE orderId=" + OrderId);
                                DataTable dtrSpecialInstructions = GetDataTable("select SpecialInstructionId from OrderSpecialInstruction WHERE orderId=" + OrderId);
                                DataTable dtrBankForOrder = GetDataTable("select BankName from BankForOrder WHERE orderId=" + OrderId);
                                DataTable dtrGPSForOrder = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + OrderId + " and ProcessId =" + 12);
                                DataTable dtrOrderDiscounts = GetDataTable("select * from OrderDiscount Where OrderId = " + OrderId);
                                if (dtrGPSForOrder.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForOrder.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForOrder.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForOrder.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForOrder.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForOrder.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForOrder.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForOrder.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + source, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    source = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }
                                if (dtrSpecialInstructions.Rows.Count > 0)
                                {
                                    specialInstruction = dtrSpecialInstructions.Rows[0]["SpecialInstructionId"].ToString() == "" ? "0" : dtrSpecialInstructions.Rows[0]["SpecialInstructionId"].ToString();
                                }
                                else
                                {
                                    specialInstruction = "0";
                                }
                                if (dtrBankForOrder.Rows.Count > 0)
                                {
                                    bankName = dtrBankForOrder.Rows[0]["BankName"].ToString();
                                }
                                else
                                {
                                    bankName = "";
                                }

                                foreach (DataRow rows in dtrOrderDetails.Rows)
                                {
                                    string productId = dtrOrderDetails.Rows[k1]["ProductAttributeId"].ToString();
                                    string quantity = dtrOrderDetails.Rows[k1]["Quantity"].ToString();
                                    string unit = dtrOrderDetails.Rows[k1]["UnitId"].ToString();
                                    string schemeId = dtrOrderDetails.Rows[k1]["SchemeId"].ToString();
                                    string rate = dtrOrderDetails.Rows[k1]["Rate"].ToString();
                                    string Amount = dtrOrderDetails.Rows[k1]["Amount"].ToString();
                                    string firstDisc = dtrOrderDetails.Rows[k1]["FirstDiscount"].ToString();
                                    string secondDis = dtrOrderDetails.Rows[k1]["SecondDiscount"].ToString();

                                    string unitDisc = dtrOrderDetails.Rows[k1]["UnitDiscount"].ToString();
                                    string freeQuantity = dtrOrderDetails.Rows[k1]["FreeQuantity"].ToString();
                                    string mobileDiscountFlag = dtrOrderDetails.Rows[k1]["MobileDiscountFlag"].ToString();
                                    string requestedQuantity = dtrOrderDetails.Rows[k1]["RequestedQuantity"].ToString();
                                    string totalDiscount = dtrOrderDetails.Rows[k1]["TotalDiscount"].ToString();
                                    string taxAmount = dtrOrderDetails.Rows[k1]["TaxAmount"].ToString();

                                    if (mobileDiscountFlag.Equals("True"))
                                    {
                                        mobileDiscountFlag = "1";
                                    }
                                    else
                                    {
                                        mobileDiscountFlag = "0";
                                    }
                                    distributorId = dtrOrderDetails.Rows[k1]["DistributorId"].ToString();
                                    if (productData != "")
                                    {
                                        productData = productData + "," + productId; ;
                                    }
                                    else
                                    {
                                        productData = productId;
                                    }
                                    if (quantitydata != "")
                                    {
                                        quantitydata = quantitydata + "," + quantity;

                                    }
                                    else
                                    {
                                        quantitydata = quantity;
                                    }
                                    if (requestedQuantityData != "")
                                    {
                                        requestedQuantityData = requestedQuantityData + "," + requestedQuantity; ;
                                    }
                                    else
                                    {
                                        requestedQuantityData = requestedQuantity;
                                    }
                                    if (unitSet != "")
                                    {
                                        unitSet = unitSet + "," + unit;
                                    }
                                    else
                                    {
                                        unitSet = unit;
                                    }
                                    if (schemIdFromMobile != "")
                                    {

                                        schemIdFromMobile = schemIdFromMobile + "," + schemeId;
                                    }
                                    else
                                    {
                                        schemIdFromMobile = schemeId;
                                    }
                                    if (price != "")
                                    {
                                        price = price + "," + rate;

                                    }
                                    else
                                    {
                                        price = rate;
                                    }
                                    if (totalAmount != "")
                                    {
                                        totalAmount = totalAmount + "," + Amount;
                                    }
                                    else
                                    {
                                        totalAmount = Amount;
                                    }
                                    if (firstDiscount != "")
                                    {

                                        firstDiscount = firstDiscount + "," + firstDisc;
                                    }
                                    else
                                    {
                                        firstDiscount = firstDisc;
                                    }
                                    if (secondDiscount != "")
                                    {
                                        secondDiscount = secondDiscount + "," + secondDis;
                                    }
                                    else
                                    {
                                        secondDiscount = secondDis;
                                    }
                                    if (unitDiscount != "")
                                    {
                                        unitDiscount = unitDiscount + "," + unitDisc;
                                    }
                                    else
                                    {
                                        unitDiscount = unitDisc;
                                    }
                                    if (freeQty != "")
                                    {
                                        freeQty = freeQty + "," + freeQuantity;
                                    }
                                    else
                                    {
                                        freeQty = freeQuantity;
                                    }
                                    if (mobileDiscFlag != "")
                                    {
                                        mobileDiscFlag = mobileDiscFlag + "," + mobileDiscountFlag;
                                    }
                                    else
                                    {
                                        mobileDiscFlag = mobileDiscountFlag;
                                    }
                                    if (totalDiscountData != "")
                                    {
                                        totalDiscountData = totalDiscountData + "," + totalDiscount; 
                                    }
                                    else
                                    {
                                        totalDiscountData = totalDiscount;
                                    }
                                    if (taxAmountData != "")
                                    {
                                        taxAmountData = taxAmountData + "," + taxAmount; ;
                                    }
                                    else
                                    {
                                        taxAmountData = taxAmount;
                                    }
                                    processName = "btnOrderOnly";

                                    k1++;

                                }
                                if (dtrOrderDiscounts.Rows.Count > 0)
                                {
                                    foreach (DataRow rows in dtrOrderDiscounts.Rows)
                                    {
                                        string discounId = dtrOrderDiscounts.Rows[k2]["DiscountType"].ToString();
                                        string discountVal = dtrOrderDiscounts.Rows[k2]["Discount"].ToString();
                                        if (orderDiscountIds == "")
                                        {
                                            orderDiscountIds = discounId;
                                            orderDiscountValues = discountVal;
                                        }
                                        else
                                        {
                                            orderDiscountIds = orderDiscountIds + "," + discounId;
                                            orderDiscountValues = orderDiscountValues + "," + discountVal;
                                        }
                                        k2++;
                                    }
                                }





                                string OrderMasterData = "";
                                OrderMasterData = userId + "@" + shopId + "@" + unitSet + "@" + productData + "@" + quantitydata + "@" + scheme + "@" + mode + "@" +
                                               specialInstruction + "@" + otherInstns + "@" + priority + "@" + siteAddress + "@" + contactPerson + "@" + phone + "@" + price + "@" +
                                              mobileNo + "@" + latitude + "@" + longitude + "@" + processName + "@" + totalAmount + "@" + firstDiscount + "@" + secondDiscount + "@" + orderDate + "@"
                                              + bankName + "@" + orderDiscount + "@" + unitDiscount + "@" + freeQty + "@"
                                              + mobileDiscFlag + "@" + distributorId + "@" + mobileReferenceNo + "@" + OrderId + "@" + orderDiscountIds + "@" + orderDiscountValues + "@" + deliveryDate + "@" + paymentmode
                                              + "@" + source + "@" + schemIdFromMobile + "@" + SignalStrength + "@" + requestedQuantityData + "@" + tempShopId + "@" + totalDiscountData + "@" + taxAmountData
                                              + "@" + NetworkProvider+ "@" + invoiceNo;

                                if (OrderData != "")
                                {
                                    OrderData = OrderData + "#" + OrderMasterData;

                                }
                                else
                                {
                                    OrderData = OrderMasterData;
                                }

                                k++;

                            }

                            Data = OrderData;
                            dataArray[0] = Data;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }



                         /***************************************Delivery***********************************************/
                        else if (tableSchema.TableName == "VW_DeliveryHeader")
                        {
                            int k = 0;
                            string deliveryData = string.Empty;
                            int deliveryOrderHeaderId = 0;
                            string   productIdSet = "", deliveredQtySet = "", invoiceQtySet = ""
                                , billNo = "", isClosed = "", mobileTransactionDate = "", userId = "", shopId = "", BillNo=""
                                , mobileReferenceNo = "", unitIdSet = "", schemeIdSet = "", orderId="" ,latitude="",longitude = "", processName = "",
                             gpsSource = "", signalStrength = "", networkProvider = "", returnReasonIdSet = "";;

                            DataTable dtrDeliveryHeader = GetDataTable("select * from  VW_DeliveryHeader"); //dtr have all data from orderheadOrderHeaderer, this loop will exc. no. of rows times 

                            foreach (DataRow row in dtrDeliveryHeader.Rows)
                            {
                                productIdSet = string.Empty;
                                deliveredQtySet = string.Empty;
                                invoiceQtySet = string.Empty;
                                unitIdSet = string.Empty;
                                schemeIdSet = string.Empty;
                                invoiceQtySet = string.Empty;
                                returnReasonIdSet = string.Empty;

                                mobileTransactionDate = dtrDeliveryHeader.Rows[k]["MobileHeaderDate"].ToString();
                                shopId = dtrDeliveryHeader.Rows[k]["ShopId"].ToString();
                                userId = dtrDeliveryHeader.Rows[k]["UserId"].ToString();
                                mobileReferenceNo = dtrDeliveryHeader.Rows[k]["MobileReferenceNo"].ToString();
                                billNo = dtrDeliveryHeader.Rows[k]["BillNo"].ToString();
                                int.TryParse(dtrDeliveryHeader.Rows[k]["DeliveryHeaderId"].ToString(), out deliveryOrderHeaderId);
                                orderId = dtrDeliveryHeader.Rows[k]["OrderId"].ToString();
                                isClosed = dtrDeliveryHeader.Rows[k]["IsClosed"].ToString();


                                DataTable dtrGPSForDelivery = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE      ProcessId =" + 57 + " and mobilereferenceNo='" + mobileReferenceNo + "'");

                                if (dtrGPSForDelivery.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForDelivery.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForDelivery.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForDelivery.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForDelivery.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForDelivery.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForDelivery.Rows[0]["Source"].ToString();
                                    signalStrength = dtrGPSForDelivery.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForDelivery.Rows[0]["SignalStrength"].ToString();
                                    networkProvider = dtrGPSForDelivery.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForDelivery.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + source, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                    signalStrength = "0";
                                    networkProvider = "0";
                                }
                                int k1 = 0;
                                processName = "btnDeliveryDetails";
                                DataTable dtrDeliveryDetails = GetDataTable("select * from VW_Delivery WHERE DeliveryOrderHeaderId=" + deliveryOrderHeaderId);
                                foreach (DataRow rows in dtrDeliveryDetails.Rows)
                                {
                                    string productId = dtrDeliveryDetails.Rows[k1]["ProductAttributeId"].ToString();
                                    string quantity = dtrDeliveryDetails.Rows[k1]["DeliveryQty"].ToString();
                                    string unit = dtrDeliveryDetails.Rows[k1]["UnitId"].ToString();
                                    string schemeId = dtrDeliveryDetails.Rows[k1]["SchemeId"].ToString();
                                    string invoiceQty = dtrDeliveryDetails.Rows[k1]["InvoiceQty"].ToString();
                                    string returnreasonId = dtrDeliveryDetails.Rows[k1]["ReturnReasonId"].ToString();
                                    k1++;


                                    if (productIdSet != "")
                                    {
                                        productIdSet = productIdSet + "," + productId; ;
                                    }
                                    else
                                    {
                                        productIdSet = productId;
                                    }

                                    if (deliveredQtySet != "")
                                    {
                                        deliveredQtySet = deliveredQtySet + "," + quantity; ;
                                    }
                                    else
                                    {
                                        deliveredQtySet = quantity;
                                    }


                                    if (unitIdSet != "")
                                    {
                                        unitIdSet = unitIdSet + "," + unit;
                                    }
                                    else
                                    {
                                        unitIdSet = unit;
                                    }


                                    
                                    if (schemeIdSet != "")
                                    {
                                        schemeIdSet = schemeIdSet + "," + schemeId;
                                    }
                                    else
                                    {
                                        schemeIdSet = schemeId;
                                    }
                                    if (invoiceQtySet != "")
                                    {
                                        invoiceQtySet = invoiceQtySet + "," + invoiceQty;
                                    }
                                    else
                                    {
                                        invoiceQtySet = invoiceQty;
                                    }

                                    if (returnReasonIdSet != "")
                                    {
                                        returnReasonIdSet = returnReasonIdSet + "," + returnreasonId; ;
                                    }
                                    else
                                    {
                                        returnReasonIdSet = returnreasonId;
                                    }



                                }


                                string DeliveryMasterData = "";
                                DeliveryMasterData = userId + "@" + shopId + "@" + mobileTransactionDate + "@" + mobileReferenceNo + "@" + billNo + "@" + productIdSet + "@" + deliveredQtySet + "@" +
                                               unitIdSet + "@" + schemeIdSet + "@" + invoiceQtySet + "@" + orderId + "@" + isClosed + "@" + latitude + "@" + longitude + "@" + processName + "@" + gpsSource + "@" + signalStrength + "@" + networkProvider
                                               + "@" + returnReasonIdSet;

                                if (deliveryData != "")
                                {
                                    deliveryData = deliveryData + "#" + DeliveryMasterData;

                                }
                                else
                                {
                                    deliveryData = DeliveryMasterData;
                                }

                                k++;

                            }
                            Data = deliveryData;
                            dataArray[42] = Data;

                            trn.Commit();
                             

                        }
                        //****************************************quotation********************************************//
                        else if (tableSchema.TableName == "QuotationHeader")
                        {
                            int k = 0;
                            int counter = 0;
                            int QuotationId = 0;


                            string QuotationData = "";

                            string userId = "", shopId = "", rateData = "", productData = "", quantitydata = "",
                            to = "", lattitude = "999", longitude = "999", processName = "", totalAmount = "",
                            productIdData = "", mobileTransactionDate = "", mobRefNo = "", source = "", SignalStrength = "", NetworkProvider = "";

                            DataTable dtrQuotationHeader = GetDataTable("select * from QuotationHeader ");

                            foreach (DataRow row in dtrQuotationHeader.Rows)
                            {
                                productData = string.Empty;
                                quantitydata = string.Empty;
                                rateData = string.Empty;
                                productIdData = string.Empty;
                                totalAmount = string.Empty;

                                shopId = dtrQuotationHeader.Rows[k]["ShopId"].ToString();
                                userId = dtrQuotationHeader.Rows[k]["UserId"].ToString();
                                to = dtrQuotationHeader.Rows[k]["EmailAddress"].ToString();
                                totalAmount = dtrQuotationHeader.Rows[k]["TotalAmount"].ToString();
                                int.TryParse(dtrQuotationHeader.Rows[k]["QuotationId"].ToString(), out QuotationId);
                                mobileTransactionDate = dtrQuotationHeader.Rows[k]["MobileTransactionDate"].ToString();
                                mobRefNo = dtrQuotationHeader.Rows[k]["MobileReferenceNo"].ToString();
                                string newCustomerId = dtrQuotationHeader.Rows[k]["NewCustomerId"].ToString();


                                int k1 = 0;
                                string tempShopId = "";
                                DataTable dtrQuotationDetails = GetDataTable("select * from VW_QuotationDetails WHERE quotationId=" + QuotationId);
                                if (!newCustomerId.Equals("0"))
                                {                                   
                                    DataTable dtrNewCustomer = GetDataTable("select TempMShopId FROM NewCustomer where NewCustomerId = " + newCustomerId);
                                    tempShopId = dtrNewCustomer.Rows[0]["TempMShopId"].ToString();
                                    
                                }

                                DataTable dtrGPSForQuotation = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + QuotationId + " and ProcessId =" + 33);
                                foreach (DataRow rows in dtrQuotationDetails.Rows)
                                {
                                    string productId = dtrQuotationDetails.Rows[k1]["ProductAttributeId"].ToString();
                                    string quantity = dtrQuotationDetails.Rows[k1]["Quantity"].ToString();
                                    string rate = dtrQuotationDetails.Rows[k1]["Rate"].ToString();
                                    string product = dtrQuotationDetails.Rows[k1]["ProductName"].ToString();


                                    if (productData != "")
                                    {
                                        productData = productData + "," + product; ;
                                    }
                                    else
                                    {
                                        productData = product;
                                    }
                                    if (quantitydata != "")
                                    {
                                        quantitydata = quantitydata + "," + quantity;

                                    }
                                    else
                                    {
                                        quantitydata = quantity;
                                    }
                                    if (rateData != "")
                                    {
                                        rateData = rateData + "," + rate;
                                    }
                                    else
                                    {
                                        rateData = rate;
                                    }
                                    if (productIdData != "")
                                    {
                                        productIdData = productIdData + "," + productId;

                                    }
                                    else
                                    {
                                        productIdData = productId;
                                    }
                                    k1++;

                                }
                                if (dtrGPSForQuotation.Rows.Count > 0)
                                {
                                    lattitude = dtrGPSForQuotation.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForQuotation.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForQuotation.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForQuotation.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForQuotation.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForQuotation.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForQuotation.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForQuotation.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForQuotation.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForQuotation.Rows[0]["NetworkProviderId"].ToString();
                                }
                                else
                                {
                                    lattitude = "999";
                                    longitude = "999";
                                    source = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }
                                string QuotationMasterData = "";
                                QuotationMasterData = userId + "~" + to + "~" + shopId + "~" + productData + "~" + rateData + "~" + quantitydata + "~" + lattitude + "~" +
                                              longitude + "~" + "btnQuotation" + "~" + totalAmount + "~" + productIdData + "~" + mobileTransactionDate + "~" + mobRefNo + "~" + tempShopId + "~" + source + "~" + mobileDate + "~" + SignalStrength + "~" + NetworkProvider;

                                if (QuotationData != "")
                                {
                                    QuotationData = QuotationData + "#" + QuotationMasterData;

                                }
                                else
                                {
                                    QuotationData = QuotationMasterData;
                                }

                                k++;

                            }

                            DataQuotation = QuotationData;
                            dataArray[1] = DataQuotation;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }


                            /*****************************************stock request******************************************/



                        else if (tableSchema.TableName == "StockHeader")
                        {
                            int k = 0;
                            int counter = 0;
                            int StockHeaderId = 0;


                            string StockRequestData = "";

                            string userId = "", storeId = "", unitData = "", productIdData = "", stockData = "",
                            lattitude = "999", longitude = "999", processName = "", createdDate = "",
                            mobRefNo = "", stockStatus = "", source = "";

                            DataTable dtrStockHeader = GetDataTable("select * from StockHeader ");

                            foreach (DataRow row in dtrStockHeader.Rows)
                            {
                                productIdData = string.Empty;
                                stockData = string.Empty;
                                unitData = string.Empty;

                                storeId = dtrStockHeader.Rows[k]["StoreId"].ToString();
                                userId = dtrStockHeader.Rows[k]["CreatedBy"].ToString();
                                stockStatus = dtrStockHeader.Rows[k]["StockStatus"].ToString();
                                int.TryParse(dtrStockHeader.Rows[k]["StockHeaderId"].ToString(), out StockHeaderId);
                                createdDate = dtrStockHeader.Rows[k]["CreatedDate"].ToString();
                                mobRefNo = dtrStockHeader.Rows[k]["MobileReferenceNo"].ToString();


                                int k1 = 0;

                                DataTable dtrStock = GetDataTable("select * from Stock WHERE StockHeaderId=" + StockHeaderId);
                                DataTable dtrGPSForStockHeader = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + StockHeaderId + " and ProcessId =" + 34);
                                foreach (DataRow rows in dtrStock.Rows)
                                {
                                    string productId = dtrStock.Rows[k1]["ProductId"].ToString();
                                    string stock = dtrStock.Rows[k1]["Stock"].ToString();
                                    string unit = dtrStock.Rows[k1]["UnitId"].ToString();


                                    if (productIdData != "")
                                    {
                                        productIdData = productIdData + "," + productId; ;
                                    }
                                    else
                                    {
                                        productIdData = productId;
                                    }
                                    if (stockData != "")
                                    {
                                        stockData = stockData + "," + stock;

                                    }
                                    else
                                    {
                                        stockData = stock;
                                    }
                                    if (unitData != "")
                                    {
                                        unitData = unitData + "," + unit;
                                    }
                                    else
                                    {
                                        unitData = unit;
                                    }

                                    k1++;

                                }
                                if (dtrGPSForStockHeader.Rows.Count > 0)
                                {
                                    lattitude = dtrGPSForStockHeader.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForStockHeader.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForStockHeader.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForStockHeader.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForStockHeader.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForStockHeader.Rows[0]["Source"].ToString();
                                }
                                else
                                {
                                    lattitude = "999";
                                    longitude = "999";
                                    source = "1";
                                }
                                string StockRequestMasterData = "";
                                StockRequestMasterData = userId + "~" + storeId + "~" + productIdData + "~" + stockData + "~" + unitData + "~" + lattitude + "~" +
                                              longitude + "~" + "btnVanStockRequest" + "~" + stockStatus + "~" + createdDate + "~" + mobRefNo + "~" + source + "~" + mobileDate;

                                if (StockRequestData != "")
                                {
                                    StockRequestData = StockRequestData + "#" + StockRequestMasterData;

                                }
                                else
                                {
                                    StockRequestData = StockRequestMasterData;
                                }

                                k++;

                            }

                            DataQuotation = StockRequestData;
                            dataArray[4] = DataQuotation;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }




                    /****************************************************************************************************/

                     /********************************stokAgingHeader********************************************************************/
                        else if (tableSchema.TableName == "StockAgingHeader")
                        {
                            int k = 0;
                            int counter = 0;
                            int stockAgingHeaderId = 0;


                            string StockAgingData = "";

                            string userId = "", shopId = "", mobileTransactionDate = "", unitSet = "", productData = "", quantitydata = "",
                              batchNoData = "", manufactureddate = "", expiryDate = "", remarkdata = "", mobileReferenceNo = "", ageData = "", processName = "", latitude = "999", longitude = "999", source = "", SignalStrength = "", NetworkProvider = "";

                            //string userId = "", storeId = "", unitData = "", productIdData = "", stockData = "",
                            //lattitude = "999", longitude = "999", processName = "", createdDate = "",
                            //mobRefNo = "", stockStatus = "", source = "";

                            DataTable dtrStockAgingHeader = GetDataTable("select * from StockAgingHeader ");

                            foreach (DataRow row in dtrStockAgingHeader.Rows)
                            {
                                productData = string.Empty;
                                quantitydata = string.Empty;
                                unitSet = string.Empty;
                                batchNoData = string.Empty;
                                manufactureddate = string.Empty;
                                expiryDate = string.Empty;
                                remarkdata = string.Empty;
                                ageData = string.Empty;


                                userId = dtrStockAgingHeader.Rows[k]["UserId"].ToString();
                                shopId = dtrStockAgingHeader.Rows[k]["ShopId"].ToString();
                                mobileTransactionDate = dtrStockAgingHeader.Rows[k]["MobileTransactionDate"].ToString();
                                int.TryParse(dtrStockAgingHeader.Rows[k]["Id"].ToString(), out stockAgingHeaderId);
                                mobileReferenceNo = dtrStockAgingHeader.Rows[k]["MobileReferenceNo"].ToString();


                                int k1 = 0;

                                DataTable dtrStockAgingDetails = GetDataTable("select * from StockAgingDetails WHERE StockAgingHeaderId=" + stockAgingHeaderId);
                                DataTable dtrGPSForStockAgingHeader = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + stockAgingHeaderId + " and ProcessId =" + 34);
                                foreach (DataRow rows in dtrStockAgingDetails.Rows)
                                {
                                    string productId = dtrStockAgingDetails.Rows[k1]["ProductAttributeId"].ToString();
                                    string quantity = dtrStockAgingDetails.Rows[k1]["Quantity"].ToString();
                                    string unit = dtrStockAgingDetails.Rows[k1]["UnitId"].ToString();
                                    string age = dtrStockAgingDetails.Rows[k1]["Age"].ToString();

                                    string batchNo = dtrStockAgingDetails.Rows[k1]["BatchNo"].ToString();
                                    string manufacturedDates = dtrStockAgingDetails.Rows[k1]["ManufacturedDate"].ToString();
                                    string expiryDates = dtrStockAgingDetails.Rows[k1]["ExpiryDate"].ToString();
                                    string remarks = dtrStockAgingDetails.Rows[k1]["Remarks"].ToString();

                                    if (productData != "")
                                    {
                                        productData = productData + "," + productId; ;
                                    }
                                    else
                                    {
                                        productData = productId;
                                    }
                                    if (quantitydata != "")
                                    {
                                        quantitydata = quantitydata + "," + quantity; ;
                                    }
                                    else
                                    {
                                        quantitydata = quantity;
                                    }
                                    if (batchNoData != "")
                                    {
                                        batchNoData = batchNoData + "," + batchNo;

                                    }
                                    else
                                    {
                                        batchNoData = batchNo;
                                    }
                                    if (unitSet != "")
                                    {
                                        unitSet = unitSet + "," + unit;
                                    }
                                    else
                                    {
                                        unitSet = unit;
                                    }




                                    if (manufactureddate != "")
                                    {
                                        manufactureddate = manufactureddate + "," + manufacturedDates;

                                    }
                                    else
                                    {
                                        manufactureddate = manufacturedDates;
                                    }
                                    if (expiryDate != "")
                                    {
                                        expiryDate = expiryDate + "," + expiryDates;
                                    }
                                    else
                                    {
                                        expiryDate = expiryDates;
                                    }

                                    if (remarkdata != "")
                                    {
                                        remarkdata = remarkdata + "," + remarks;
                                    }
                                    else
                                    {
                                        remarkdata = remarks;
                                    }
                                    if (ageData != "")
                                    {
                                        ageData = ageData + "," + age;
                                    }
                                    else
                                    {
                                        ageData = age;
                                    }
                                    k1++;

                                }
                                if (dtrGPSForStockAgingHeader.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForStockAgingHeader.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForStockAgingHeader.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForStockAgingHeader.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForStockAgingHeader.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForStockAgingHeader.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForStockAgingHeader.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForStockAgingHeader.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForStockAgingHeader.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForStockAgingHeader.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForStockAgingHeader.Rows[0]["NetworkProviderId"].ToString();
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    source = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }
                                string StockAgingMasterData = "";
                                StockAgingMasterData = userId + "~" + shopId + "~" + productData + "~" + quantitydata + "~" + unitSet+ "~"+batchNoData+"~" +manufactureddate+ "~"+expiryDate+"~"+ageData+"~" + latitude + "~" +
                                              longitude + "~" + "btnStockAging" + "~" + remarkdata + "~" + mobileReferenceNo + "~" + source + "~" + mobileDate + "~" + SignalStrength + "~" + NetworkProvider;

                                if (StockAgingData != "")
                                {
                                    StockAgingData = StockAgingData + "#" + StockAgingMasterData;

                                }
                                else
                                {
                                    StockAgingData = StockAgingMasterData;
                                }

                                k++;

                            }

                            DataQuotation = StockAgingData;
                            dataArray[29] = DataQuotation;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

 
                        }



                        /****************************************************************************************************/
                        /******************TodaysPlan****************************************************************************/
                        else if (tableSchema.TableName == "TodaysPlanDetails")
                        {
                            int k = 0;
                            int counter = 0;
                            string TodaysPlanData = "";

                            string userId = "",todaysPlanDetailsId="", mobileTransactionDate = "", planId = "", mobileReferenceNo = "", processName = "", latitude = "999", longitude = "999", source = "",mobileSyncdate="";
                            //string userId = "", storeId = "", unitData = "", productIdData = "", stockData = "",
                            //lattitude = "999", longitude = "999", processName = "", createdDate = "",
                            //mobRefNo = "", stockStatus = "", source = "";

                            DataTable dtrTodaysPlan = GetDataTable("select * from TodaysPlanDetails ");
                         //   DataTable dtrTodaysPlanGpsData = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + StockReconcileHeaderId + " and ProcessId =" + 35);
                            foreach (DataRow row in dtrTodaysPlan.Rows)
                            {


                                todaysPlanDetailsId = dtrTodaysPlan.Rows[k]["Id"].ToString();
                                userId = dtrTodaysPlan.Rows[k]["UserId"].ToString();
                                planId = dtrTodaysPlan.Rows[k]["PlanId"].ToString();
                                mobileTransactionDate = dtrTodaysPlan.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = dtrTodaysPlan.Rows[k]["MobileReferenceNo"].ToString();
                                mobileSyncdate = dtrTodaysPlan.Rows[k]["MobileSyncDate"].ToString();

                                DataTable dtrTodaysPlanGpsData = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + todaysPlanDetailsId + " and MobileReferenceNo ='" + mobileReferenceNo+"'");



                                if (dtrTodaysPlanGpsData.Rows.Count > 0)
                                {
                                    latitude = dtrTodaysPlanGpsData.Rows[0]["latitude"].ToString() == "" ? "999" : dtrTodaysPlanGpsData.Rows[0]["latitude"].ToString();
                                    longitude = dtrTodaysPlanGpsData.Rows[0]["longitude"].ToString() == "" ? "999" : dtrTodaysPlanGpsData.Rows[0]["longitude"].ToString();
                                    source = dtrTodaysPlanGpsData.Rows[0]["Source"].ToString() == "" ? "1" : dtrTodaysPlanGpsData.Rows[0]["Source"].ToString();
                                  
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    source = "1";
                                    
                                }
                                string TodaysPlanMasterData = "";
                                TodaysPlanMasterData = userId + "~" +  planId + "~" +mobileSyncdate+ "~" + latitude + "~" +
                                              longitude + "~" + "btnTodaysPlan" + "~" + mobileReferenceNo + "~" + source + "~" + mobileDate + "~" ;

                                if (TodaysPlanData != "")
                                {
                                    TodaysPlanData = TodaysPlanData + "#" + TodaysPlanMasterData;

                                }
                                else
                                {
                                    TodaysPlanData = TodaysPlanMasterData;
                                }

                                k++;
                            

                            }
                            DataQuotation = TodaysPlanData;
                            dataArray[34] = DataQuotation;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }
                        /****************************************************************************************************/
                        /******************EditCustomer****************************************************************************/

                        else if (tableSchema.TableName == "EditCustomers")
                        {
                            int k = 0;
                            int counter = 0;
                            string EditCustomerData = "";

                            string EditCustomerId = "", userId = "", shopId = "", contactPersonName = "", mobileNumber = "", pinCode = "", address = ""
                             , landmark = "", latitude = "", longitude = "", processName = "", mobileTransactionDate = ""
                         , mobileReferenceNo = "", gpsSource = "", mobileSyncdate = "", shopPotential ="" , gstNumber="";

                            DataTable dtrEditCustomer = GetDataTable("select * from EditCustomers ");
                            //   DataTable dtrTodaysPlanGpsData = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + StockReconcileHeaderId + " and ProcessId =" + 35);
                            foreach (DataRow row in dtrEditCustomer.Rows)
                            {


                                EditCustomerId = dtrEditCustomer.Rows[k]["Id"].ToString();
                                userId = dtrEditCustomer.Rows[k]["EditedBy"].ToString();
                                shopId = dtrEditCustomer.Rows[k]["ShopId"].ToString();
                                contactPersonName = dtrEditCustomer.Rows[k]["ContactName"].ToString();
                                mobileNumber = dtrEditCustomer.Rows[k]["MobileNo"].ToString();
                                pinCode = dtrEditCustomer.Rows[k]["PinCode"].ToString();
                                address = dtrEditCustomer.Rows[k]["Address"].ToString();
                                landmark = dtrEditCustomer.Rows[k]["Landmark"].ToString();
                               
                                mobileReferenceNo = dtrEditCustomer.Rows[k]["MobileReferenceNo"].ToString();

                                mobileTransactionDate = dtrEditCustomer.Rows[k]["MobileTransactionDate"].ToString();


                                mobileSyncdate = dtrEditCustomer.Rows[k]["MobileSyncDate"].ToString();

                                shopPotential= dtrEditCustomer.Rows[k]["ShopPotential"].ToString();

                                gstNumber = dtrEditCustomer.Rows[k]["GstNumber"].ToString();
                                DataTable dtrEditGpsData = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + EditCustomerId + " and MobileReferenceNo ='" + mobileReferenceNo + "'");



                                if (dtrEditGpsData.Rows.Count > 0)
                                {
                                    latitude = dtrEditGpsData.Rows[0]["latitude"].ToString() == "" ? "999" : dtrEditGpsData.Rows[0]["latitude"].ToString();
                                    longitude = dtrEditGpsData.Rows[0]["longitude"].ToString() == "" ? "999" : dtrEditGpsData.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrEditGpsData.Rows[0]["Source"].ToString() == "" ? "1" : dtrEditGpsData.Rows[0]["Source"].ToString();

                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";

                                }
                                string EditCustomerMasterData = "";
                                EditCustomerMasterData = userId + "~" + shopId + "~" + contactPersonName + "~" + mobileNumber + "~" + pinCode + "~" + address + "~" + landmark + "~" + latitude + "~" +
                                              longitude + "~" + "btnEditCustomer" + "~" + mobileTransactionDate + "~" + mobileReferenceNo + "~" + gpsSource + "~" + mobileSyncdate + "~" +shopPotential+ "~" +gstNumber+ "~";

                                if (EditCustomerData != "")
                                {
                                    EditCustomerData = EditCustomerData + "#" + EditCustomerMasterData;

                                }
                                else
                                {
                                    EditCustomerData = EditCustomerMasterData;
                                }

                                k++;


                            }
                            DataQuotation = EditCustomerData;
                            dataArray[39] = DataQuotation;
                            trn.Commit();




                        }


                         /*******************************************AssetRequest ***************************
                          * ******************************************************************/

                        else if (tableSchema.TableName == "AssetRequests")
                        {
                            int k = 0;
                            string AssetRequestData = "";

                            string userId = "", AssetId = "", mobileTransactionDate = "", ShopID = "", mobileReferenceNo = "",
                                latitude = "999", longitude = "999", source = "", mobileSyncdate = "", Field1 = "", Field2 = "", Field3 = ""
                                , Field4 = "", Field5 = "", RequestType = "", AssetNo = "";
                            //string userId = "", storeId = "", unitData = "", productIdData = "", stockData = "",
                            //lattitude = "999", longitude = "999", processName = "", createdDate = "",
                            //mobRefNo = "", stockStatus = "", source = "";

                            DataTable dtrTodaysPlan = GetDataTable("select * from assetrequests");
                            //   DataTable dtrTodaysPlanGpsData = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + StockReconcileHeaderId + " and ProcessId =" + 35);
                            foreach (DataRow row in dtrTodaysPlan.Rows)
                            {
                                userId = dtrTodaysPlan.Rows[k]["UserId"].ToString();
                                ShopID = dtrTodaysPlan.Rows[k]["ShopId"].ToString();
                                AssetId = dtrTodaysPlan.Rows[k]["AssetId"].ToString();
                                Field1 = dtrTodaysPlan.Rows[k]["Field1"].ToString();
                                Field2 = dtrTodaysPlan.Rows[k]["Field2"].ToString();
                                Field3 = dtrTodaysPlan.Rows[k]["Field3"].ToString();
                                Field4 = dtrTodaysPlan.Rows[k]["Field4"].ToString();
                                Field5 = dtrTodaysPlan.Rows[k]["Field5"].ToString();
                                mobileTransactionDate = dtrTodaysPlan.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = dtrTodaysPlan.Rows[k]["MobileReferenceNo"].ToString();
                                RequestType = dtrTodaysPlan.Rows[k]["RequestType"].ToString();
                                AssetNo = dtrTodaysPlan.Rows[k]["AssetNo"].ToString();
                                DataTable dtrAssetRequestGpsData = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  MobileReferenceNo ='" + mobileReferenceNo + "'");
                                if (dtrAssetRequestGpsData.Rows.Count > 0)
                                {
                                    latitude = dtrAssetRequestGpsData.Rows[0]["latitude"].ToString() == "" ? "999" : dtrAssetRequestGpsData.Rows[0]["latitude"].ToString();
                                    longitude = dtrAssetRequestGpsData.Rows[0]["longitude"].ToString() == "" ? "999" : dtrAssetRequestGpsData.Rows[0]["longitude"].ToString();
                                    source = dtrAssetRequestGpsData.Rows[0]["Source"].ToString() == "" ? "1" : dtrAssetRequestGpsData.Rows[0]["Source"].ToString();

                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    source = "1";

                                }
                                string AssetRequestMasterData = "";
                                AssetRequestMasterData =
                                    userId + "~" +
                                    AssetId + "~" +
                                    ShopID + "~" +
                                    Field1 + "~" +
                                    Field2 + "~" +
                                    Field3 + "~" +
                                    Field4 + "~" +
                                    Field5 + "~" +
                                    latitude + "~" +
                                    longitude + "~" +
                                    "btnAssetRequest" + "~" +
                                    mobileDate + "~" +
                                    mobileReferenceNo + "~" +
                                    source + "~" +
                                    RequestType + "~" +
                                    AssetNo + "~";


                                if (AssetRequestData != "")
                                {
                                    AssetRequestData = AssetRequestData + "#" + AssetRequestMasterData;

                                }
                                else
                                {
                                    AssetRequestData = AssetRequestMasterData;
                                }

                                k++;


                            }
                            DataQuotation = AssetRequestData;
                            dataArray[36] = DataQuotation;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }



                      /**************************************************************************************************/
                        /*******************************************Joint Working ***************************
                         * ******************************************************************/

                        else if (tableSchema.TableName == "JointWorkSurveyResults_Transaction")
                        {
                            int k = 0;
                            string JointWorkingData_Save = "";

                            string UserID = "", BeatPlanActivityConfigsId = "", SurveyGroupId = "", ShopId = "", ShopName = "",
                                latitude = "999", longitude = "999", AnswerId = "", QuestionId = "", Remarks = "", MobileTransactionDate = "", MobileReferenceNo = ""
                                , BeatPlanId = "", source = "", JointWorkUserId = "";
                            //string userId = "", storeId = "", unitData = "", productIdData = "", stockData = "",
                            //lattitude = "999", longitude = "999", processName = "", createdDate = "",
                            //mobRefNo = "", stockStatus = "", source = "";

                            DataTable dtrJointWork = GetDataTable("select * from JointWorkSurveyResults_Transaction");
                           // DataTable dtrGPSForStock = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  MobileReferenceNo=" + MobileReferenceNo );
                            foreach (DataRow row in dtrJointWork.Rows)
                            {
                                UserID = dtrJointWork.Rows[k]["UserId"].ToString();
                                BeatPlanActivityConfigsId = dtrJointWork.Rows[k]["BeatPlanActivityConfigsId"].ToString();
                                SurveyGroupId = dtrJointWork.Rows[k]["SurveyGroupId"].ToString();
                                ShopId = dtrJointWork.Rows[k]["ShopId"].ToString();
                                ShopName = dtrJointWork.Rows[k]["ShopName"].ToString();
                                 
                                AnswerId = dtrJointWork.Rows[k]["AnswerId"].ToString();
                                QuestionId = dtrJointWork.Rows[k]["QuestionId"].ToString();
                                Remarks = dtrJointWork.Rows[k]["Remarks"].ToString();
                                MobileTransactionDate = dtrJointWork.Rows[k]["MobileTransactionDate"].ToString();
                                MobileReferenceNo = dtrJointWork.Rows[k]["MobileReferenceNo"].ToString();
                                BeatPlanId = dtrJointWork.Rows[k]["BeatPlanId"].ToString();
                                JointWorkUserId = dtrJointWork.Rows[k]["JointWorkUserId"].ToString();

                                DataTable dtrAssetRequestGpsData = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  MobileReferenceNo ='" + MobileReferenceNo + "'");
                                if (dtrAssetRequestGpsData.Rows.Count > 0)
                                {
                                    latitude = dtrAssetRequestGpsData.Rows[0]["latitude"].ToString() == "" ? "999" : dtrAssetRequestGpsData.Rows[0]["latitude"].ToString();
                                    longitude = dtrAssetRequestGpsData.Rows[0]["longitude"].ToString() == "" ? "999" : dtrAssetRequestGpsData.Rows[0]["longitude"].ToString();
                                    source = dtrAssetRequestGpsData.Rows[0]["Source"].ToString() == "" ? "1" : dtrAssetRequestGpsData.Rows[0]["Source"].ToString();

                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    source = "1";

                                }
                                string JointWorkingData  = "";
                                JointWorkingData =
                                    UserID + "~" +
                                    BeatPlanActivityConfigsId + "~" +
                                    SurveyGroupId + "~" +
                                    ShopId + "~" +
                                    ShopName + "~" +
                                    AnswerId + "~" +
                                    QuestionId + "~" +
                                    Remarks + "~" +
                                    MobileTransactionDate + "~" +
                                    MobileReferenceNo + "~" +
                                    BeatPlanId + "~" +
                                    latitude + "~" +
                                    longitude + "~" +
                                    source + "~" +
                                    JointWorkUserId + "~";


                                if (JointWorkingData_Save != "")
                                {
                                    JointWorkingData_Save = JointWorkingData_Save + "#" + JointWorkingData;

                                }
                                else
                                {
                                    JointWorkingData_Save = JointWorkingData;
                                }

                                k++;


                            }
                            DataQuotation = JointWorkingData_Save;
                            dataArray[49] = DataQuotation;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }



                      /**************************************************************************************************/

                            /**************************************************************************************************/
                        /*******************************************Joint Working ***************************
                         * ******************************************************************/

                        else if (tableSchema.TableName == "JointWorkShopInAndOutLog_Transaction")
                        {
                            int k = 0;
                            string JointWorkingData_Save = "";

                            string UserId = "", ShopInId = "", ShopInTime = "", ShopOutId = "", ShopOutTime = "",
                                JointWorkUserId = "", MobileTransactionDate = "", MobileReferenceNo = "", BeatPlanId = "", ShopName = ""
                                , DeviationReasonId = "";
                            //string userId = "", storeId = "", unitData = "", productIdData = "", stockData = "",
                            //lattitude = "999", longitude = "999", processName = "", createdDate = "",
                            //mobRefNo = "", stockStatus = "", source = "";

                            DataTable dtrJointWork = GetDataTable("select * from JointWorkShopInAndOutLog_Transaction");
                            // DataTable dtrGPSForStock = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  MobileReferenceNo=" + MobileReferenceNo );
                            foreach (DataRow row in dtrJointWork.Rows)
                            {
                                UserId = dtrJointWork.Rows[k]["UserId"].ToString();
                                ShopInId = dtrJointWork.Rows[k]["ShopInId"].ToString();
                                ShopInTime = dtrJointWork.Rows[k]["ShopInTime"].ToString();
                                ShopOutId = dtrJointWork.Rows[k]["ShopOutId"].ToString();
                                ShopOutTime = dtrJointWork.Rows[k]["ShopOutTime"].ToString();

                                JointWorkUserId = dtrJointWork.Rows[k]["JointWorkUserId"].ToString();
                                MobileTransactionDate = dtrJointWork.Rows[k]["MobileTransactionDate"].ToString();
                                MobileReferenceNo = dtrJointWork.Rows[k]["MobileReferenceNo"].ToString();
                                BeatPlanId = dtrJointWork.Rows[k]["BeatPlanId"].ToString();
                                ShopName = dtrJointWork.Rows[k]["ShopName"].ToString();
                                DeviationReasonId = dtrJointWork.Rows[k]["DeviationReasonId"].ToString();
                                 
                                
                                string JointWorkingData = "";
                                JointWorkingData =
                                    UserId + "~" +
                                    ShopInId + "~" +
                                    ShopInTime + "~" +
                                    ShopOutId + "~" +
                                    ShopOutTime + "~" +
                                    JointWorkUserId + "~" +
                                    MobileTransactionDate + "~" +
                                    MobileReferenceNo + "~" +
                                    BeatPlanId + "~" +
                                    ShopName + "~" +
                                    DeviationReasonId + "~";


                                if (JointWorkingData_Save != "")
                                {
                                    JointWorkingData_Save = JointWorkingData_Save + "#" + JointWorkingData;

                                }
                                else
                                {
                                    JointWorkingData_Save = JointWorkingData;
                                }

                                k++;


                            }
                            DataQuotation = JointWorkingData_Save;
                            dataArray[48] = DataQuotation;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }



                      /**************************************************************************************************/
                        /**************************************************************************************************/

                            /**************************************************************************************************/
                        /*******************************************Collection Settlement ***************************
                         * ******************************************************************/

                        else if (tableSchema.TableName == "CollectionSettlement_Transaction")
                        {
                            int k = 0;
                            string CollectionSettlement_Save = "";

                            string PaymentHeaderId = "", InstrumentNo = "", CashAmount = "", RemittedBy = "" ,
                            //string userId = "", storeId = "", unitData = "", productIdData = "", stockData = "",
                            latitude = "999", longitude = "999", processName = "", createdDate = "",MobilereferenceNo = "",
                              stockStatus = "", source = "",MobileTransactionDate = ""  ;

                            DataTable dtrCollectionSettlement = GetDataTable("select * from CollectionSettlement_Transaction");

                            foreach (DataRow row in dtrCollectionSettlement.Rows)
                            {
                                PaymentHeaderId = dtrCollectionSettlement.Rows[k]["PaymentHeaderId"].ToString();
                                InstrumentNo = dtrCollectionSettlement.Rows[k]["InstrumentNo"].ToString();
                                CashAmount = dtrCollectionSettlement.Rows[k]["CashAmount"].ToString();
                                RemittedBy = dtrCollectionSettlement.Rows[k]["Remittedby"].ToString();
                                MobilereferenceNo = dtrCollectionSettlement.Rows[k]["MobilereferenceNo"].ToString();
                                MobileTransactionDate = dtrCollectionSettlement.Rows[k]["MobileTransactionDate"].ToString();
                               
                               

                                DataTable dtrGPSCollectionSettlement = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  MobileReferenceNo='" + MobilereferenceNo + "' and ProcessId =" + 63);

                                latitude = dtrGPSCollectionSettlement.Rows[0]["latitude"].ToString();
                                longitude = dtrGPSCollectionSettlement.Rows[0]["longitude"].ToString();
                                source = dtrGPSCollectionSettlement.Rows[0]["source"].ToString();
                               
                                string CollectionSettlementData = "";
                                CollectionSettlementData =
                                    PaymentHeaderId + "~" +
                                    InstrumentNo + "~" +
                                    CashAmount + "~" +
                                    RemittedBy + "~" +
                                    MobilereferenceNo + "~" +
                                    MobileTransactionDate + "~" +
                                    latitude + "~" +
                                    longitude + "~" +
                                    source + "~";


                                if (CollectionSettlement_Save != "")
                                {
                                    CollectionSettlement_Save = CollectionSettlement_Save + "#" + CollectionSettlementData;

                                }
                                else
                                {
                                    CollectionSettlement_Save = CollectionSettlementData;
                                }

                                k++;


                            }
                            DataQuotation = CollectionSettlement_Save;
                            dataArray[50] = DataQuotation;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }



                      /**************************************************************************************************/

                        else if (tableSchema.TableName == "StockReconcileHeader")
                        {
                            int k = 0;
                            int counter = 0;
                            int StockReconcileHeaderId = 0;

                            string mobRefNo = "";
                            string StockReconcileData = "";

                            string userId = "", storeId = "", quantityData = "", productAttributeIdData = "", rateData = "", reasonIdData = "",
                            lattitude = "999", longitude = "999", processName = "", updatedDate = "", remarksData = "",
                            unloadStatus = "", source = "", unitIdData = "";

                            DataTable dtrStockReconcileHeader = GetDataTable("select * from StockReconcileHeader ");

                            foreach (DataRow row in dtrStockReconcileHeader.Rows)
                            {
                                productAttributeIdData = string.Empty;
                                quantityData = string.Empty;
                                rateData = string.Empty;

                                storeId = dtrStockReconcileHeader.Rows[k]["StoreId"].ToString();
                                userId = dtrStockReconcileHeader.Rows[k]["CreatedBy"].ToString();
                                int.TryParse(dtrStockReconcileHeader.Rows[k]["Id"].ToString(), out StockReconcileHeaderId);
                                updatedDate = dtrStockReconcileHeader.Rows[k]["UpdatedDate"].ToString();
                                unloadStatus = dtrStockReconcileHeader.Rows[k]["unloadStatus"].ToString();
                                mobRefNo = dtrStockReconcileHeader.Rows[k]["MobileReferenceNo"].ToString();


                                int k1 = 0;

                                DataTable dtrStockReconcile = GetDataTable("select * from StockReconcile WHERE StockReconcileHeaderId=" + StockReconcileHeaderId);
                                DataTable dtrGPSForStockReconcileHeader = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + StockReconcileHeaderId + " and ProcessId =" + 35);
                                foreach (DataRow rows in dtrStockReconcile.Rows)
                                {
                                    string productAttributeId = dtrStockReconcile.Rows[k1]["ProductAttributeId"].ToString();
                                    string rate = dtrStockReconcile.Rows[k1]["Rate"].ToString();
                                    string quantity = dtrStockReconcile.Rows[k1]["Quantity"].ToString();
                                    string reasonId = dtrStockReconcile.Rows[k1]["ReturnReasonId"].ToString();
                                    string unitId = dtrStockReconcile.Rows[k1]["UnitId"].ToString();
                                    string remarks = dtrStockReconcile.Rows[k1]["Remarks"].ToString();
                                    if (productAttributeIdData != "")
                                    {
                                        productAttributeIdData = productAttributeIdData + "," + productAttributeId;
                                    }
                                    else
                                    {
                                        productAttributeIdData = productAttributeId;
                                    }
                                    if (rateData != "")
                                    {
                                        rateData = rateData + "," + rate;

                                    }
                                    else
                                    {
                                        rateData = rate;
                                    }
                                    if (quantityData != "")
                                    {
                                        quantityData = quantityData + "," + quantity;
                                    }
                                    else
                                    {
                                        quantityData = quantity;
                                    }

                                    if (reasonIdData != "")
                                    {
                                        reasonIdData = reasonIdData + "," + reasonId;
                                    }
                                    else
                                    {
                                        reasonIdData = reasonId;
                                    }
                                    if (unitIdData != "")
                                    {
                                        unitIdData = unitIdData + "," + unitId;
                                    }
                                    else
                                    {
                                        unitIdData = unitId;
                                    }
                                    if (remarksData != "")
                                    {
                                        remarksData = remarksData + "," + remarks;
                                    }
                                    else
                                    {
                                        remarksData = remarks;
                                    }
                                    k1++;

                                }
                                if (dtrGPSForStockReconcileHeader.Rows.Count > 0)
                                {
                                    lattitude = dtrGPSForStockReconcileHeader.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForStockReconcileHeader.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForStockReconcileHeader.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForStockReconcileHeader.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForStockReconcileHeader.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForStockReconcileHeader.Rows[0]["Source"].ToString();
                                }
                                else
                                {
                                    lattitude = "999";
                                    longitude = "999";
                                    source = "1";
                                }
                                string StockReconcileMasterData = "";
                                StockReconcileMasterData = userId + "~" + storeId + "~" + productAttributeIdData + "~" + quantityData + "~" + reasonIdData + "~" + rateData + "~" + lattitude + "~" +
                                              longitude + "~" + updatedDate + "~" + unloadStatus + "~" + source + "~" + mobRefNo + "~" + unitIdData + "~" + remarksData;

                                if (StockReconcileData != "")
                                {
                                    StockReconcileData = StockReconcileData + "#" + StockReconcileMasterData;

                                }
                                else
                                {
                                    StockReconcileData = StockReconcileMasterData;
                                }

                                k++;

                            }

                            DataQuotation = StockReconcileData;
                            dataArray[10] = StockReconcileData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }


                     /*****************************************************************************************************/
                        else if (tableSchema.TableName == "NewCustomer")
                        {

                            int k = 0;

                            string newCustomerData = "";

                            string newCustId = "", customerNames = "", phones = "", reportedBy = "", reportedDates = "",
                             tempMShopIds = "", contactNames = "", shortNames = "", shopTypes = "", shopClasses = "",
                             locations = "", addresses = "", citys = "", states = "", countrys = "", continents = "",
                             beatPlanIds = "", distributorIds = "", emails = "", pincodes = "", tinNos = "", cstNos = "",
                             thirdPartyShopCodes = "", customerGroups = "", creditLimits = "", doorNos = "", streetNames = "",
                             transporterNames = "", districts = "", specialitys = "", mobileTransactionDates = "", parentShopIds = "",
                             mobileReferenceNos = "", field1Set = "", field2Set = "", field3Set = "", field4Set = "", field5Set = "", field6Set = "",
                             field7Set = "", field8Set = "", field9Set = "", field10Set = "", field11Set = "", field12Set = "", field13Set = ""
                             , field14Set = "", field15Set = "", field16Set = "", latitude = "", longitude = ""
                             , processId = "", userId = "", source = "", gpsmobileTransactionDate = "", gpsMobileReferenceNo = "", signalStrength = "",customerCategoryIds=""
                             ,gstin = "";


                            DataTable dtrNewCustomer = GetDataTable("select * from NewCustomer "); //dtr have all data from NewCustomer, this loop will exc. no. of rows times 

                            foreach (DataRow row in dtrNewCustomer.Rows)
                            {
                                newCustId = dtrNewCustomer.Rows[k]["NewCustomerId"].ToString();

                                customerNames = dtrNewCustomer.Rows[k]["CustomerName"].ToString();


                                phones = dtrNewCustomer.Rows[k]["Phone"].ToString();

                                reportedBy = dtrNewCustomer.Rows[k]["ReportedBy"].ToString();


                                reportedDates = dtrNewCustomer.Rows[k]["ReportedDate"].ToString();


                                tempMShopIds = dtrNewCustomer.Rows[k]["TempMShopId"].ToString();


                                contactNames = dtrNewCustomer.Rows[k]["ContactName"].ToString();


                                shortNames = dtrNewCustomer.Rows[k]["ShortName"].ToString();


                                shopTypes = dtrNewCustomer.Rows[k]["ShopType"].ToString();


                                shopClasses = dtrNewCustomer.Rows[k]["ShopClass"].ToString();


                                locations = dtrNewCustomer.Rows[k]["Location"].ToString();


                                addresses = dtrNewCustomer.Rows[k]["Address"].ToString();


                                citys = dtrNewCustomer.Rows[k]["City"].ToString();


                                states = dtrNewCustomer.Rows[k]["State"].ToString();

                                countrys = dtrNewCustomer.Rows[k]["Country"].ToString();

                                continents = dtrNewCustomer.Rows[k]["Continent"].ToString();



                                beatPlanIds = dtrNewCustomer.Rows[k]["BeatPlanId"].ToString();


                                distributorIds = dtrNewCustomer.Rows[k]["DistributorId"].ToString();


                                emails = dtrNewCustomer.Rows[k]["Email"].ToString();


                                pincodes = dtrNewCustomer.Rows[k]["Pincode"].ToString();


                                tinNos = dtrNewCustomer.Rows[k]["TinNo"].ToString();


                                cstNos = dtrNewCustomer.Rows[k]["CstNo"].ToString();


                                thirdPartyShopCodes = dtrNewCustomer.Rows[k]["ThirdPartyShopCode"].ToString();


                                customerGroups = dtrNewCustomer.Rows[k]["CustomerGroup"].ToString();


                                creditLimits = dtrNewCustomer.Rows[k]["CreditLimit"].ToString();


                                doorNos = dtrNewCustomer.Rows[k]["DoorNo"].ToString();


                                streetNames = dtrNewCustomer.Rows[k]["StreetName"].ToString();

                                transporterNames = dtrNewCustomer.Rows[k]["TransporterName"].ToString();

                                districts = dtrNewCustomer.Rows[k]["District"].ToString();

                                specialitys = dtrNewCustomer.Rows[k]["Speciality"].ToString();


                                mobileTransactionDates = dtrNewCustomer.Rows[k]["MobileTransactionDate"].ToString();


                                parentShopIds = dtrNewCustomer.Rows[k]["ParentShopId"].ToString();

                                mobileReferenceNos = dtrNewCustomer.Rows[k]["MobileReferenceNo"].ToString();

                                customerCategoryIds = dtrNewCustomer.Rows[k]["CustomerCategoryId"].ToString();
                                gstin = dtrNewCustomer.Rows[k]["Gstin"].ToString();

                                DataTable dtrCustomerDetails = GetDataTable("select * from NewCustomerDetails WHERE  NewCustomerId=" + newCustId);

                                int k1 = 0;
                                foreach (DataRow dtrrow in dtrCustomerDetails.Rows)
                                {
                                    field1Set = dtrCustomerDetails.Rows[k1]["Field1"].ToString();


                                    field2Set = dtrCustomerDetails.Rows[k1]["Field2"].ToString();


                                    field3Set = dtrCustomerDetails.Rows[k1]["Field3"].ToString();


                                    field4Set = dtrCustomerDetails.Rows[k1]["Field4"].ToString();


                                    field5Set = dtrCustomerDetails.Rows[k1]["Field5"].ToString();

                                    field6Set = dtrCustomerDetails.Rows[k1]["Field6"].ToString();



                                    field7Set = dtrCustomerDetails.Rows[k1]["Field7"].ToString();

                                    field8Set = dtrCustomerDetails.Rows[k1]["Field8"].ToString();


                                    field9Set = dtrCustomerDetails.Rows[k1]["Field9"].ToString();


                                    field10Set = dtrCustomerDetails.Rows[k1]["Field10"].ToString();


                                    field11Set = dtrCustomerDetails.Rows[k1]["Field11"].ToString();


                                    field12Set = dtrCustomerDetails.Rows[k1]["Field12"].ToString();


                                    field13Set = dtrCustomerDetails.Rows[k1]["Field13"].ToString();

                                    field14Set = dtrCustomerDetails.Rows[k1]["Field14"].ToString();

                                    field15Set = dtrCustomerDetails.Rows[k1]["Field15"].ToString();

                                    field16Set = dtrCustomerDetails.Rows[k1]["Field16"].ToString();

                                    k1++;
                                }
                                int k2 = 0;
                                DataTable dtrGpsDetails = GetDataTable("select * from GPSLocationInfo WHERE  ProcessDetailsId=" + newCustId + " and ProcessId =" + 29);
                                foreach (DataRow dtrgpsrow in dtrGpsDetails.Rows)
                                {
                                    latitude = dtrGpsDetails.Rows[k2]["Latitude"].ToString();

                                    longitude = dtrGpsDetails.Rows[k2]["Longitude"].ToString();

                                    processId = dtrGpsDetails.Rows[k2]["ProcessId"].ToString();

                                    userId = dtrGpsDetails.Rows[k2]["UserId"].ToString();

                                    source = dtrGpsDetails.Rows[k2]["Source"].ToString();

                                    gpsmobileTransactionDate = dtrGpsDetails.Rows[k2]["MobileTransactionDate"].ToString();

                                    gpsMobileReferenceNo = dtrGpsDetails.Rows[k2]["MobileReferenceNo"].ToString();

                                    signalStrength = dtrGpsDetails.Rows[k2]["SignalStrength"].ToString();

                                    k2++;
                                }

                                int k3 = 0;
                                DataTable dtrTempOrderHeader = GetDataTable("select * from Temp_OrderHeader WHERE  NewCustomerId=" + newCustId);
                                string OrderData = "";


                                foreach (DataRow dtrOrder in dtrTempOrderHeader.Rows)
                                {

                                    string tempOrderHeaderId = "", orderTakenBy = "", schemeId = "", otherInstruction = "", priority = "", lastPrice = "", shopName = ""
                                   , orderMobileReferenceNo = "", deliveryDate = "", orderDiscount = "", paymentMode = "", receiptNumber = "", orderDate = "",
                                   productData = "", quantityData = "", amountData = "", schemeIdFromMobile = "", unitSet = "", priceIdSet = "", rateSet = ""
                                   , rateDiscountSet = "", attributeIdSet = "", firstDiscount = "", secondDiscount = "", outletStock = "", orderdistributorId = "",
                                   orderDiscountIds = "", orderDiscountValues = "", unitDiscount = "", freeQty = "", mobileDiscountFlags = "", totalDiscountData = "", taxAmountData = "";

                                    orderDate = dtrTempOrderHeader.Rows[k3]["OrderDate"].ToString();
                                    orderTakenBy = dtrTempOrderHeader.Rows[k3]["OrderTakenBy"].ToString();
                                    tempOrderHeaderId = dtrTempOrderHeader.Rows[k3]["TempOrderHeaderId"].ToString();
                                    schemeId = dtrTempOrderHeader.Rows[k3]["SchemeId"].ToString() == "" ? "0" : dtrTempOrderHeader.Rows[k3]["SchemeId"].ToString();
                                    otherInstruction = dtrTempOrderHeader.Rows[k3]["OtherInstruction"].ToString();
                                    priority = dtrTempOrderHeader.Rows[k3]["Priority"].ToString();
                                    lastPrice = dtrTempOrderHeader.Rows[k3]["LastPrice"].ToString();
                                    shopName = dtrTempOrderHeader.Rows[k3]["ShopName"].ToString();
                                    orderMobileReferenceNo = dtrTempOrderHeader.Rows[k3]["MobileReferenceNo"].ToString();
                                    deliveryDate = dtrTempOrderHeader.Rows[k3]["DeliveryDate"].ToString();
                                    orderDiscount = dtrTempOrderHeader.Rows[k3]["OrderDiscount"].ToString();
                                    paymentMode = dtrTempOrderHeader.Rows[k3]["PaymentMode"].ToString();
                                    receiptNumber = dtrTempOrderHeader.Rows[k3]["ReceiptNumber"].ToString();

                                    int k4 = 0;
                                    int k5 = 0;

                                    DataTable dtrOrderDetails = GetDataTable("select * from VW_TempOrderDetails WHERE  TempOrderHeaderId=" + tempOrderHeaderId);
                                    DataTable dtrOrderDiscounts = GetDataTable("select * from Temp_OrderDiscounts Where TempOrderHeaderId = " + tempOrderHeaderId);

                                    foreach (DataRow rows in dtrOrderDetails.Rows)
                                    {
                                        string productId = dtrOrderDetails.Rows[k4]["ProductAttributeId"].ToString();
                                        string quantity = dtrOrderDetails.Rows[k4]["Quantity"].ToString();
                                        string unit = dtrOrderDetails.Rows[k4]["UnitId"].ToString();
                                        string orderSchemeId = dtrOrderDetails.Rows[k4]["SchemeId"].ToString();
                                        string rate = dtrOrderDetails.Rows[k4]["Rate"].ToString();
                                        string Amount = dtrOrderDetails.Rows[k4]["Amount"].ToString();
                                        string firstDisc = dtrOrderDetails.Rows[k4]["FirstDiscount"].ToString();
                                        string secondDis = dtrOrderDetails.Rows[k4]["SecondDiscount"].ToString();
                                        string priceId = dtrOrderDetails.Rows[k4]["PriceId"].ToString();
                                        string ratediscount = dtrOrderDetails.Rows[k4]["RateDiscount"].ToString();
                                        string attributeId = dtrOrderDetails.Rows[k4]["CategoryId"].ToString();
                                        string outletStk = dtrOrderDetails.Rows[k4]["OutletStock"].ToString();
                                        string unitDisc = dtrOrderDetails.Rows[k4]["UnitDiscount"].ToString();
                                        string freeQuantity = dtrOrderDetails.Rows[k4]["FreeQuantity"].ToString();
                                        string mobileDiscountFlag = dtrOrderDetails.Rows[k4]["MobileDiscountFlag"].ToString();
                                        string totalDiscount = dtrOrderDetails.Rows[k4]["TotalDiscount"].ToString();
                                        string taxAmount = dtrOrderDetails.Rows[k4]["TaxAmount"].ToString();


                                        orderdistributorId = dtrOrderDetails.Rows[k4]["DistributorId"].ToString();
                                        if (productData != "")
                                        {
                                            productData = productData + "," + productId; ;
                                        }
                                        else
                                        {
                                            productData = productId;
                                        }
                                        if (quantityData != "")
                                        {
                                            quantityData = quantityData + "," + quantity;

                                        }
                                        else
                                        {
                                            quantityData = quantity;
                                        }
                                        if (unitSet != "")
                                        {
                                            unitSet = unitSet + "," + unit;
                                        }
                                        else
                                        {
                                            unitSet = unit;
                                        }
                                        if (schemeIdFromMobile != "")
                                        {

                                            schemeIdFromMobile = schemeIdFromMobile + "," + orderSchemeId;
                                        }
                                        else
                                        {
                                            schemeIdFromMobile = orderSchemeId;
                                        }
                                        if (rateSet != "")
                                        {
                                            rateSet = rateSet + "," + rate;

                                        }
                                        else
                                        {
                                            rateSet = rate;
                                        }
                                        if (amountData != "")
                                        {
                                            amountData = amountData + "," + Amount;
                                        }
                                        else
                                        {
                                            amountData = Amount;
                                        }
                                        if (firstDiscount != "")
                                        {

                                            firstDiscount = firstDiscount + "," + firstDisc;
                                        }
                                        else
                                        {
                                            firstDiscount = firstDisc;
                                        }
                                        if (secondDiscount != "")
                                        {
                                            secondDiscount = secondDiscount + "," + secondDis;
                                        }
                                        else
                                        {
                                            secondDiscount = secondDis;
                                        }
                                        if (priceIdSet != "")
                                        {
                                            priceIdSet = priceIdSet + "," + priceId;
                                        }
                                        else
                                        {
                                            priceIdSet = priceId;
                                        }
                                        if (rateDiscountSet != "")
                                        {
                                            rateDiscountSet = rateDiscountSet + "," + ratediscount;
                                        }
                                        else
                                        {
                                            rateDiscountSet = ratediscount;
                                        }
                                        if (outletStock != "")
                                        {
                                            outletStock = outletStock + "," + outletStk;
                                        }
                                        else
                                        {
                                            outletStock = outletStk;
                                        }
                                        if (attributeIdSet != "")
                                        {
                                            attributeIdSet = attributeIdSet + "," + attributeId;
                                        }
                                        else
                                        {
                                            attributeIdSet = attributeId;
                                        }
                                        if (unitDiscount != "")
                                        {

                                            unitDiscount = unitDiscount + "," + unitDisc;
                                        }
                                        else
                                        {
                                            unitDiscount = unitDisc;
                                        }
                                        if (freeQty != "")
                                        {

                                            freeQty = freeQty + "," + freeQuantity;
                                        }
                                        else
                                        {
                                            freeQty = freeQuantity;
                                        }
                                        if (mobileDiscountFlags != "")
                                        {

                                            mobileDiscountFlags = mobileDiscountFlags + "," + mobileDiscountFlag;
                                        }
                                        else
                                        {
                                            mobileDiscountFlags = mobileDiscountFlag;
                                        }
                                        if (totalDiscountData != "")
                                        {
                                            totalDiscountData = totalDiscountData + "," + totalDiscount;
                                        }
                                        else
                                        {
                                            totalDiscountData = totalDiscount;
                                        }
                                        if (taxAmountData != "")
                                        {
                                            taxAmountData = taxAmountData + "," + taxAmount; ;
                                        }
                                        else
                                        {
                                            taxAmountData = taxAmount;
                                        }
                                        k4++;

                                    }


                                    if (dtrOrderDiscounts.Rows.Count > 0)
                                    {
                                        foreach (DataRow rows in dtrOrderDiscounts.Rows)
                                        {
                                            string discounId = dtrOrderDiscounts.Rows[k5]["DiscountType"].ToString();
                                            string discountVal = dtrOrderDiscounts.Rows[k5]["Discount"].ToString();
                                            if (orderDiscountIds == "")
                                            {
                                                orderDiscountIds = discounId;
                                                orderDiscountValues = discountVal;
                                            }
                                            else
                                            {
                                                orderDiscountIds = orderDiscountIds + "," + discounId;
                                                orderDiscountValues = orderDiscountValues + "," + discountVal;
                                            }
                                            k5++;
                                        }
                                    }


                                    string OrderMasterData = "";
                                    OrderMasterData = orderDate + "&&" + orderTakenBy + "&&" + tempOrderHeaderId + "&&" + schemeId + "&&" +
                                    otherInstruction + "&&" + priority + "&&" + lastPrice + "&&" + shopName + "&&" + orderMobileReferenceNo + "&&" +
                                    deliveryDate + "&&" + orderDiscount + "&&" + paymentMode + "&&" + receiptNumber + "&&" + orderdistributorId + "&&" +
                                    productData + "&&" + quantityData + "&&" + unitSet + "&&" + schemeIdFromMobile + "&&" + rateSet + "&&" +
                                    amountData + "&&" + firstDiscount + "&&" + secondDiscount + "&&" + priceIdSet + "&&" + rateDiscountSet + "&&" +
                                    outletStock + "&&" + attributeIdSet + "&&" + orderDiscountIds + "&&" + orderDiscountValues + "&&" + unitDiscount + "&&" +
                                    freeQty + "&&" + mobileDiscountFlags + "&&" + totalDiscountData + "&&" + taxAmountData;

                                    if (OrderData != "")
                                    {
                                        OrderData = OrderData + "%%" + OrderMasterData;

                                    }
                                    else
                                    {
                                        OrderData = OrderMasterData;
                                    }

                                    k3++;

                                }



                                string NewCustomerMasterData = "";
                                NewCustomerMasterData = customerNames + "@@" + phones + "@@" + reportedBy + "@@" + reportedDates + "@@" +
                             tempMShopIds + "@@" + contactNames + "@@" + shortNames + "@@" + shopTypes + "@@" + shopClasses + "@@" +
                             locations + "@@" + addresses + "@@" + citys + "@@" + states + "@@" + countrys + "@@" + continents + "@@" +
                             beatPlanIds + "@@" + distributorIds + "@@" + emails + "@@" + pincodes + "@@" + tinNos + "@@" + cstNos + "@@" +
                             thirdPartyShopCodes + "@@" + customerGroups + "@@" + creditLimits + "@@" + doorNos + "@@" + streetNames + "@@" +
                             transporterNames + "@@" + districts + "@@" + specialitys + "@@" + mobileTransactionDates + "@@" + parentShopIds + "@@" +
                             mobileReferenceNos + "@@" + field1Set + "@@" + field2Set + "@@" + field3Set + "@@" + field4Set + "@@" + field5Set + "@@" + field6Set + "@@" +
                             field7Set + "@@" + field8Set + "@@" + field9Set + "@@" + field10Set + "@@" + field11Set + "@@" + field12Set + "@@" + field13Set + "@@" +
                             field14Set + "@@" + field15Set + "@@" + field16Set + "@@" + latitude + "@@" + longitude + "@@" +
                             processId + "@@" + userId + "@@" + source + "@@" + gpsmobileTransactionDate + "@@" + gpsMobileReferenceNo + "@@" + signalStrength + "@@" +
                             OrderData + "@@" + customerCategoryIds + "@@" + gstin;

                                if (newCustomerData != "")
                                {
                                    newCustomerData = newCustomerData + "##" + NewCustomerMasterData;

                                }
                                else
                                {
                                    newCustomerData = NewCustomerMasterData;
                                }

                                k++;

                            }

                            Data = newCustomerData;
                            dataArray[32] = Data;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }

                        /*********************stock entry **************************/
                        else if (tableSchema.TableName == "StockEntryHeader")
                        {
                            int k = 0;
                            int counter = 0;
                            int StockHeader = 0;


                            string StockData = "";

                            string userId = "", shopId = "", productData = "", quantitydata = "",
                           lattitude = "999", longitude = "999", processName = "",
                            UnitSet = "", mobileTransactionDate = "", mobileReferenceNo = "", source = "", SignalStrength = "", NetworkProvider = "";

                            DataTable dtrStockHeader = GetDataTable("select * from StockEntryHeader ");

                            foreach (DataRow row in dtrStockHeader.Rows)
                            {
                                productData = string.Empty;
                                quantitydata = string.Empty;
                                UnitSet = string.Empty;


                                shopId = dtrStockHeader.Rows[k]["ShopId"].ToString();
                                userId = dtrStockHeader.Rows[k]["UserId"].ToString();
                                mobileTransactionDate = dtrStockHeader.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = dtrStockHeader.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrStockHeader.Rows[k]["StockEntryHeaderId"].ToString(), out StockHeader);
                                int k1 = 0;

                                DataTable dtrStockDetails = GetDataTable("select * from StockEntryDetails WHERE  StockEntryHeaderId=" + StockHeader);
                                DataTable dtrGPSForStock = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + StockHeader + " and ProcessId =" + 1);
                                foreach (DataRow rows in dtrStockDetails.Rows)
                                {
                                    string productId = dtrStockDetails.Rows[k1]["ProductAttributeId"].ToString();
                                    string quantity = dtrStockDetails.Rows[k1]["Quantity"].ToString();
                                    string unit = dtrStockDetails.Rows[k1]["UnitId"].ToString();

                                    if (productData != "")
                                    {
                                        productData = productData + "," + productId; ;
                                    }
                                    else
                                    {
                                        productData = productId;
                                    }
                                    if (quantitydata != "")
                                    {
                                        quantitydata = quantitydata + "," + quantity;

                                    }
                                    else
                                    {
                                        quantitydata = quantity;
                                    }
                                    if (UnitSet != "")
                                    {
                                        UnitSet = UnitSet + "," + unit;
                                    }
                                    else
                                    {
                                        UnitSet = unit;
                                    }

                                    k1++;

                                }
                                if (dtrGPSForStock.Rows.Count > 0)
                                {
                                    lattitude = dtrGPSForStock.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForStock.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForStock.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForStock.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForStock.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForStock.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForStock.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForStock.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForStock.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForStock.Rows[0]["NetworkProviderId"].ToString();
                                }
                                else
                                {
                                    lattitude = "999";
                                    longitude = "999";
                                    source = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }
                                string SalesMasterData = "";
                                SalesMasterData = userId + "~" + shopId + "~" + productData + "~" + quantitydata + "~" + lattitude + "~" +
                                              longitude + "~" + "btnStockCaptureWithOrder" + "~" + UnitSet + "~" + mobileTransactionDate + "~" + mobileReferenceNo + "~" + source + "~" + mobileDate + "~" + SignalStrength + "~" + NetworkProvider;

                                if (StockData != "")
                                {
                                    StockData = StockData + "#" + SalesMasterData;

                                }
                                else
                                {
                                    StockData = SalesMasterData;
                                }

                                k++;

                            }

                            //   DataQuotation = QuotationData;
                            dataArray[3] = StockData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }
                        /***********************************/
                        /*********************collection **************************/
                        else if (tableSchema.TableName == "PaymentHeader")
                        {
                            //SqlCommand insert = BuildSQLServerInsert(tableSchema);
                            int counter = 0;
                            int k = 0;
                            int paymentId = 0;


                            string paymentData = "";

                            string userId = "", shopId = "", amount = "", instrumentNo = "", instrumentDate = ""
                            , bankId = "", paymentModeId = "", receiptNo = "", description = "", discountSet = "", latitude = ""
                            , longitude = "", processName = "", billNoSet = "", osBalanceSet = "", isRemitted = "", remittedAt = ""
                            , isRemittanceWithCollection = "", isMultipleDiscountCollection = "", discount1Set = "", discount2Set = "", amountSet = ""
                            , discount3Set = "", mobileTransactionDate = "", mobileReferenceNo = "", collectionDiscount = "", gpsSource = "", tempShopId = "", SignalStrength = "", NetworkProvider = "";

                            processName = "btnPayment";
                            DataTable dtrpaymentMaster = GetDataTable("select * from PaymentHeader ");


                            foreach (DataRow row in dtrpaymentMaster.Rows)
                            {
                                discountSet = string.Empty;
                                latitude = string.Empty;
                                longitude = string.Empty;
                                amountSet = string.Empty;
                                billNoSet = string.Empty;
                                osBalanceSet = string.Empty;
                                isRemittanceWithCollection = string.Empty;
                                isMultipleDiscountCollection = string.Empty;
                                discount1Set = string.Empty;
                                discount2Set = string.Empty;
                                discount3Set = string.Empty;

                                gpsSource = string.Empty;

                                bankId = dtrpaymentMaster.Rows[k]["BankId"].ToString();
                                shopId = dtrpaymentMaster.Rows[k]["ShopId"].ToString();
                                userId = dtrpaymentMaster.Rows[k]["CollectedBy"].ToString();
                                mobileReferenceNo = dtrpaymentMaster.Rows[k]["MobileReferenceNo"].ToString();
                                instrumentNo = dtrpaymentMaster.Rows[k]["InstrumentNo"].ToString();
                                instrumentDate = dtrpaymentMaster.Rows[k]["InstrumentDate"].ToString();
                                int.TryParse(dtrpaymentMaster.Rows[k]["PaymentId"].ToString(), out paymentId);
                                paymentModeId = dtrpaymentMaster.Rows[k]["PaymentModeId"].ToString();
                                receiptNo = dtrpaymentMaster.Rows[k]["ReceiptNo"].ToString();
                                description = dtrpaymentMaster.Rows[k]["Narration"].ToString();
                                amount = dtrpaymentMaster.Rows[k]["Amount"].ToString();
                                isRemitted = dtrpaymentMaster.Rows[k]["IsRemitted"].ToString();
                                remittedAt = dtrpaymentMaster.Rows[k]["RemittedAt"].ToString();
                                collectionDiscount = dtrpaymentMaster.Rows[k]["DiscountAmount"].ToString();
                                mobileTransactionDate = dtrpaymentMaster.Rows[k]["MobilePaymentDate"].ToString();
                                tempShopId = dtrpaymentMaster.Rows[k]["TempShopId"].ToString();
                                if (isRemitted.Equals("False"))
                                {
                                    isRemittanceWithCollection = "False";
                                }
                                else
                                {
                                    isRemittanceWithCollection = "True";
                                }

                                collectionDiscount = dtrpaymentMaster.Rows[k]["DiscountAmount"].ToString();

                                int k1 = 0;
                                int k2 = 0;


                                DataTable dtrPaymentDetails = GetDataTable("select * from PaymentDetails WHERE PaymentId=" + paymentId);
                                DataTable dtrGPSForCollection = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + paymentId + " and ProcessId =" + 14);

                                if (dtrGPSForCollection.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForCollection.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForCollection.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForCollection.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForCollection.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForCollection.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForCollection.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForCollection.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForCollection.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForCollection.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForCollection.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }


                                foreach (DataRow rows in dtrPaymentDetails.Rows)
                                {
                                    int paymentDetailsId = 0;
                                    int.TryParse(dtrPaymentDetails.Rows[k1]["PaymentDetailsId"].ToString(), out paymentDetailsId);
                                    string billNo = dtrPaymentDetails.Rows[k1]["BillNo"].ToString();
                                    string osBalance = dtrPaymentDetails.Rows[k1]["OSBalance"].ToString();
                                    string discount = dtrPaymentDetails.Rows[k1]["Discount"].ToString();
                                    string amountVal = dtrPaymentDetails.Rows[k1]["Amount"].ToString();

                                    if (billNoSet != "")
                                    {
                                        billNoSet = billNoSet + "," + billNo;
                                    }
                                    else
                                    {
                                        billNoSet = billNo;
                                    }

                                    if (osBalanceSet != "")
                                    {
                                        osBalanceSet = osBalanceSet + "," + osBalance;
                                    }
                                    else
                                    {
                                        osBalanceSet = osBalance;
                                    }

                                    if (discountSet != "")
                                    {
                                        discountSet = discountSet + "," + discount;
                                    }
                                    else
                                    {
                                        discountSet = discount;
                                    }

                                    if (amountSet != "")
                                    {
                                        amountSet = amountSet + "," + amountVal;
                                    }
                                    else
                                    {
                                        amountSet = amountVal;
                                    }
                                    DataTable dtrPaymentDiscountDetails = GetDataTable("select * from PaymentDiscountDetails WHERE PaymentDetailsId=" + paymentDetailsId);


                                    if (dtrPaymentDiscountDetails.Rows.Count > 0)
                                    {
                                        string rd = dtrPaymentDiscountDetails.Rows[0]["RD"].ToString();
                                        string sd = dtrPaymentDiscountDetails.Rows[0]["SD"].ToString();
                                        string cd = dtrPaymentDiscountDetails.Rows[0]["CD"].ToString();

                                        if (discount1Set != "")
                                        {
                                            discount1Set = discount1Set + "," + rd;
                                        }
                                        else
                                        {
                                            discount1Set = sd;
                                        }

                                        if (discount2Set != "")
                                        {
                                            discount2Set = discount2Set + "," + sd;
                                        }
                                        else
                                        {
                                            discount2Set = sd;
                                        }

                                        if (discount3Set != "")
                                        {
                                            discount3Set = discount3Set + "," + cd;
                                        }
                                        else
                                        {
                                            discount3Set = cd;
                                        }
                                        isMultipleDiscountCollection = "true";
                                    }
                                    else
                                    {
                                        isMultipleDiscountCollection = "false";
                                    }

                                    k1++;
                                }

                                string paymentSubData = "";
                                paymentSubData = userId + "@" + shopId + "@" + amount + "@" + instrumentNo + "@" + instrumentDate + "@" + bankId + "@" + paymentModeId + "@" + receiptNo + "@" + description + "@" + discountSet + "@" +
                                                latitude + "@" + longitude + "@" + processName + "@" + billNoSet + "@" + osBalanceSet + "@" + isRemitted + "@" + remittedAt + "@" + isRemittanceWithCollection
                                                + "@" + isMultipleDiscountCollection + "@" + discount1Set + "@" + discount2Set + "@" + amountSet + "@" +
                                                discount3Set + "@" + mobileTransactionDate + "@" + mobileReferenceNo + "@" + collectionDiscount + "@" + gpsSource + "@" + tempShopId + "@" + SignalStrength + "@" + NetworkProvider;

                                if (paymentData != "")
                                {
                                    paymentData = paymentData + "#" + paymentSubData;
                                }
                                else
                                {
                                    paymentData = paymentSubData;
                                }
                                k++;
                            }

                            dataArray[11] = paymentData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }
                        /***********************************/
                        /*********************Working with **************************/
                        else if (tableSchema.TableName == "MobileWorkingWithHeader")
                        {
                            //SqlCommand insert = BuildSQLServerInsert(tableSchema);
                            //int counter = 0;
                            int k = 0;
                            int MobileWorkingWithHeaderId = 0;

                            string workWithData = "";

                            string userId = "", shopId = "", userIdSet = "", latitude = ""
                            , longitude = "", processName = "", mobileTransactionDate = "", mobileReferenceNo = "", gpsSource = "", SignalStrength = "", NetworkProvider = ""
                            , Others ="" , DepartmentIdSet="" ;

                            processName = "btnWorkingWith";
                            DataTable dtrWorkingWithMatser = GetDataTable("select * from MobileWorkingWithHeader");


                            foreach (DataRow row in dtrWorkingWithMatser.Rows)
                            {
                                userIdSet = string.Empty;

                                userId = dtrWorkingWithMatser.Rows[k]["Userid"].ToString();
                                shopId = dtrWorkingWithMatser.Rows[k]["Shopid"].ToString();
                                mobileReferenceNo = dtrWorkingWithMatser.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrWorkingWithMatser.Rows[k]["MobileWorkingWithHeaderId"].ToString(), out MobileWorkingWithHeaderId);
                                mobileTransactionDate = dtrWorkingWithMatser.Rows[k]["MobileCapturedDate"].ToString();
                                Others = dtrWorkingWithMatser.Rows[k]["Others"].ToString();
                                DepartmentIdSet = dtrWorkingWithMatser.Rows[k]["DepartmentIdSet"].ToString();
                                int k1 = 0;

                                DataTable dtrMobileWorkingWithDetails = GetDataTable("select * from MobileWorkingWithDetails WHERE MobileWorkingWithHeaderId = " + MobileWorkingWithHeaderId);
                                DataTable dtrGPSForWorkiWith = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + MobileWorkingWithHeaderId + " and ProcessId =" + 30);

                                if (dtrGPSForWorkiWith.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForWorkiWith.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForWorkiWith.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForWorkiWith.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForWorkiWith.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForWorkiWith.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForWorkiWith.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForWorkiWith.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForWorkiWith.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForWorkiWith.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForWorkiWith.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }


                                foreach (DataRow rows in dtrMobileWorkingWithDetails.Rows)
                                {
                                    string userIdSetVal = dtrMobileWorkingWithDetails.Rows[k1]["WorkingWithUserId"].ToString();

                                    if (userIdSet != "")
                                    {
                                        userIdSet = userIdSet + "," + userIdSetVal;
                                    }
                                    else
                                    {
                                        userIdSet = userIdSetVal;
                                    }
                                    k1++;
                                }


                                string workWithSubData = "";
                                workWithSubData = userId + "@" + shopId + "@" + userIdSet + "@" + latitude + "@" + longitude + "@" + processName + "@" +
                                    mobileTransactionDate + "@" + mobileReferenceNo + "@" + gpsSource + "@" + SignalStrength + "@" + NetworkProvider + "@" + Others + "@" + DepartmentIdSet;

                                if (workWithData != "")
                                {
                                    workWithData = workWithData + "#" + workWithSubData;
                                }
                                else
                                {
                                    workWithData = workWithSubData;
                                }
                                k++;
                            }

                            dataArray[24] = workWithData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        /***********************************/


                        else if (tableSchema.TableName == "ExpenseEntryHead")
                        {
                            int k = 0;
                            int expenseEntryHeadId = 0;


                            string ExpenseData = "";

                            string userId = "", expenseIdSet = "", amountSet = "", latitude = ""
                            , longitude = "", processName = "", mobileTransactionDate = "", mobileReferenceNo = "", remarksSet = "", gpsSource = "", field1Set = "",
                            field2Set = "", field3Set = "", field4Set = "", field5Set = "", expenseDateSet = "", source = "", uniqueKeySet = "";

                            processName = "btnExpence";
                            DataTable dtrExpenseMaster = GetDataTable("select * from ExpenseEntryHead");

                            foreach (DataRow row in dtrExpenseMaster.Rows)
                            {
                                expenseIdSet = string.Empty;
                                amountSet = string.Empty;
                                field1Set = string.Empty;
                                field2Set = string.Empty;
                                field3Set = string.Empty;
                                field4Set = string.Empty;
                                field5Set = string.Empty;
                                remarksSet = string.Empty;
                                uniqueKeySet = string.Empty;


                                userId = dtrExpenseMaster.Rows[k]["UserId"].ToString();
                                mobileTransactionDate = dtrExpenseMaster.Rows[k]["MobileTransactionDate"].ToString();
                                int.TryParse(dtrExpenseMaster.Rows[k]["Id"].ToString(), out expenseEntryHeadId);
                                mobileReferenceNo = dtrExpenseMaster.Rows[k]["MobileReferenceNo"].ToString();


                                int k1 = 0;

                                DataTable dtrExpenseEntry = GetDataTable("select * from ExpenseEntry WHERE ExpenseEntryHeadId=" + expenseEntryHeadId);
                                DataTable dtrGPSForExpenseEntry = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + expenseEntryHeadId + " and ProcessId =5");

                                foreach (DataRow rows in dtrExpenseEntry.Rows)
                                {
                                    string expenseId = dtrExpenseEntry.Rows[k1]["ExpenseId"].ToString();
                                    string amount = dtrExpenseEntry.Rows[k1]["Amount"].ToString();
                                    string expenseDate = dtrExpenseEntry.Rows[k1]["ExpenseDate"].ToString();
                                    string remarks = dtrExpenseEntry.Rows[k1]["Remarks"].ToString();
                                    string expenseDetailsId = dtrExpenseEntry.Rows[k1]["ExpenseDetailsId"].ToString();
                                    string uniqueKey = dtrExpenseEntry.Rows[k1]["UniqueKey"].ToString();


                                    DataTable dtrExpenseEntryDetails = GetDataTable("select * from ExpenseEntryDetails WHERE ExpenseDetailsId=" + expenseDetailsId);
                                    foreach (DataRow dtrow in dtrExpenseEntryDetails.Rows)
                                    {
                                        string field1 = dtrExpenseEntryDetails.Rows[0]["Field1"].ToString();
                                        string field2 = dtrExpenseEntryDetails.Rows[0]["Field2"].ToString();
                                        string field3 = dtrExpenseEntryDetails.Rows[0]["Field3"].ToString();
                                        string field4 = dtrExpenseEntryDetails.Rows[0]["Field4"].ToString();
                                        string field5 = dtrExpenseEntryDetails.Rows[0]["Field5"].ToString();

                                        if (field1Set != "")
                                        {
                                            field1Set = field1Set + "," + field1;
                                        }
                                        else
                                        {
                                            field1Set = field1;
                                        }
                                        if (field2Set != "")
                                        {
                                            field2Set = field2Set + "," + field2;
                                        }
                                        else
                                        {
                                            field2Set = field2;
                                        }
                                        if (field3Set != "")
                                        {
                                            field3Set = field3Set + "," + field3;
                                        }
                                        else
                                        {
                                            field3Set = field3;
                                        }
                                        if (field4Set != "")
                                        {
                                            field4Set = field4Set + "," + field4;
                                        }
                                        else
                                        {
                                            field4Set = field4;
                                        }
                                        if (field5Set != "")
                                        {
                                            field5Set = field5Set + "," + field5;
                                        }
                                        else
                                        {
                                            field5Set = field5;
                                        }
                                    }

                                    if (expenseIdSet != "")
                                    {
                                        expenseIdSet = expenseIdSet + "," + expenseId; ;
                                    }
                                    else
                                    {
                                        expenseIdSet = expenseId;
                                    }
                                    if (amountSet != "")
                                    {
                                        amountSet = amountSet + "," + amount; ;
                                    }
                                    else
                                    {
                                        amountSet = amount;
                                    }
                                    if (expenseDateSet != "")
                                    {
                                        expenseDateSet = expenseDateSet + "," + expenseDate;

                                    }
                                    else
                                    {
                                        expenseDateSet = expenseDate;
                                    }
                                    if (remarksSet != "")
                                    {
                                        remarksSet = remarksSet + "," + remarks;
                                    }
                                    else
                                    {
                                        remarksSet = remarks;
                                    }
                                    if (uniqueKeySet != "")
                                    {
                                        uniqueKeySet = uniqueKeySet + "," + uniqueKey;
                                    }
                                    else
                                    {
                                        uniqueKeySet = uniqueKey;
                                    }
                                    k1++;

                                }
                                if (dtrGPSForExpenseEntry.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForExpenseEntry.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForExpenseEntry.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForExpenseEntry.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForExpenseEntry.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForExpenseEntry.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForExpenseEntry.Rows[0]["Source"].ToString();
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    source = "1";
                                }

                                //    userId = "", expenseIdSet = "", amountSet = "", latitude = ""
                                //, longitude = "", processName = "", mobileTransactionDate = "", mobileReferenceNo = "", remarksSet = "", gpsSource = "", field1Set = "",
                                //field2Set = "", field3Set = "", field4Set = "", field5Set = "", expenseDateSet = "", source=""

                                string ExpenseMasterData = "";
                                ExpenseMasterData = userId + "@" + expenseIdSet + "@" + amountSet + "@" + remarksSet + "@" + latitude + "@" + longitude
                                + "@" + "btnExpence" + "@" + mobileTransactionDate + "@" + mobileReferenceNo + "@" + source + "@" + expenseDateSet
                                + "@" + field1Set + "@" + field2Set
                                + "@" + field3Set + "@" + field4Set + "@" + field5Set + "@" + uniqueKeySet;

                                if (ExpenseData != "")
                                {
                                    ExpenseData = ExpenseData + "#" + ExpenseMasterData;

                                }
                                else
                                {
                                    ExpenseData = ExpenseMasterData;
                                }

                                k++;

                            }

                            DataQuotation = ExpenseData;
                            dataArray[16] = DataQuotation;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");


                        }

                        else if (tableSchema.TableName == "PromotionalResult")
                        {                            
                            int k = 0;
                            int promotionalResultId = 0;

                            string promoData = "";

                            string userId = "", shopId = "", qn = "", ans = ""
                            , freeText = "", dateSet = "", textSet = "", numberSet = "", imageDataSet = "", latitude = "",
                            longitude = "", processName = "", mobileTransactionDate = "", mobileReferenceNo = "", gpsSource = "", SignalStrength = "", NetworkProvider = "";

                            processName = "btnPromoActivity";
                            DataTable dtrPromoMaster = GetDataTable("select * from PromotionalResult");


                            foreach (DataRow row in dtrPromoMaster.Rows)
                            {
                                shopId = dtrPromoMaster.Rows[k]["ShopId"].ToString();
                                userId = dtrPromoMaster.Rows[k]["CreatedBy"].ToString();
                                qn = dtrPromoMaster.Rows[k]["QuestionId"].ToString();
                                ans = dtrPromoMaster.Rows[k]["AnswerId"].ToString();
                                freeText = dtrPromoMaster.Rows[k]["ResultDescription"].ToString();
                                dateSet = dtrPromoMaster.Rows[k]["AdditionalDate"].ToString();
                                textSet = dtrPromoMaster.Rows[k]["AdditionalText"].ToString();
                                numberSet = dtrPromoMaster.Rows[k]["AdditionalNumber"].ToString();
                                imageDataSet = dtrPromoMaster.Rows[k]["AdditionalImageData"].ToString();
                                mobileReferenceNo = dtrPromoMaster.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrPromoMaster.Rows[k]["ResultId"].ToString(), out promotionalResultId);
                                mobileTransactionDate = dtrPromoMaster.Rows[k]["MobileTransactionDate"].ToString();

                                DataTable dtrGPSForPromotional = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + promotionalResultId + " and ProcessId =" + 10);

                                if (dtrGPSForPromotional.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForPromotional.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForPromotional.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForPromotional.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForPromotional.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForPromotional.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForPromotional.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForPromotional.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForPromotional.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForPromotional.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForPromotional.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }



                                string promoSubData = "";
                                promoSubData = userId + "@" + shopId + "@" + qn + "@" + ans + "@" + freeText + "@" + dateSet + "@" + textSet + "@" + numberSet + "@" + imageDataSet + "@" + latitude + "@" + longitude
                                    + "@" + processName + "@" + mobileTransactionDate + "@" + mobileReferenceNo + "@" + gpsSource + "@" + SignalStrength + "@" + NetworkProvider;

                                if (promoData != "")
                                {
                                    promoData = promoData + "#" + promoSubData;
                                }
                                else
                                {
                                    promoData = promoSubData;
                                }
                                k++;
                            }

                            dataArray[19] = promoData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "TransactionResult")
                        {
                            int k = 0;
                            int transactionResultId = 0;

                            string transactionData = "";

                            string userId = "", shopId = "", qn = "", ans = ""
                            , freeText = "", dateSet = "", textSet = "", numberSet = "", imageDataSet = "", latitude = "",
                            longitude = "", processName = "", mobileTransactionDate = "", mobileReferenceNo = "", gpsSource = "", SignalStrength = "", NetworkProvider = "";

                            processName = "btnPlanogramActivity";
                            DataTable dtrPromoMaster = GetDataTable("select * from TransactionResult");


                            foreach (DataRow row in dtrPromoMaster.Rows)
                            {
                                shopId = dtrPromoMaster.Rows[k]["ShopId"].ToString();
                                userId = dtrPromoMaster.Rows[k]["CreatedBy"].ToString();
                                qn = dtrPromoMaster.Rows[k]["QuestionId"].ToString();
                                ans = dtrPromoMaster.Rows[k]["AnswerId"].ToString();
                                freeText = dtrPromoMaster.Rows[k]["ResultDescription"].ToString();
                                dateSet = dtrPromoMaster.Rows[k]["AdditionalDate"].ToString();
                                textSet = dtrPromoMaster.Rows[k]["AdditionalText"].ToString();
                                numberSet = dtrPromoMaster.Rows[k]["AdditionalNumber"].ToString();
                                imageDataSet = dtrPromoMaster.Rows[k]["AdditionalImageData"].ToString();
                                mobileReferenceNo = dtrPromoMaster.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrPromoMaster.Rows[k]["ResultId"].ToString(), out transactionResultId);
                                mobileTransactionDate = dtrPromoMaster.Rows[k]["MobileTransactionDate"].ToString();

                                DataTable dtrGPSForPromotional = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + transactionResultId + " and mobilereferenceNo ='" + mobileReferenceNo + "'");

                                if (dtrGPSForPromotional.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForPromotional.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForPromotional.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForPromotional.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForPromotional.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForPromotional.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForPromotional.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForPromotional.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForPromotional.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForPromotional.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForPromotional.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }



                                string transactionDataSubData = "";
                                transactionDataSubData = userId + "@" + shopId + "@" + qn + "@" + ans + "@" + freeText + "@" + dateSet + "@" + textSet + "@" + numberSet + "@" + imageDataSet + "@" + latitude + "@" + longitude
                                    + "@" + processName + "@" + mobileTransactionDate + "@" + mobileReferenceNo + "@" + gpsSource + "@" + SignalStrength + "@" + NetworkProvider;

                                if (transactionData != "")
                                {
                                    transactionData = transactionData + "#" + transactionDataSubData;
                                }
                                else
                                {
                                    transactionData = transactionDataSubData;
                                }
                                k++;
                            }

                            dataArray[40] = transactionData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "LeaveRequest")
                        {

                            int k = 0;
                            int leaveRequestId = 0;

                            string leaveData = "";

                            string userId = "", mobileCaptureTime = "", leaveReasonId = "", fromDate = ""
                            , toDate = "", latitude = "",
                            longitude = "", processName = "", mobileReferenceNo = "", gpsSource = "", remarks = ""
                            , leaveFromSessionId = "", leaveToSessionId = "";

                            processName = "btnLeave";
                            DataTable dtrLeaveMaster = GetDataTable("select * from LeaveRequest WHERE DailyTourPlanId IS NULL");


                            foreach (DataRow row in dtrLeaveMaster.Rows)
                            {
                                userId = dtrLeaveMaster.Rows[k]["RequestedBy"].ToString();
                                leaveReasonId = dtrLeaveMaster.Rows[k]["LeaveReasonId"].ToString();
                                fromDate = dtrLeaveMaster.Rows[k]["LeaveFrom"].ToString();
                                toDate = dtrLeaveMaster.Rows[k]["LeaveTo"].ToString();
                                mobileReferenceNo = dtrLeaveMaster.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrLeaveMaster.Rows[k]["LeaveRequestId"].ToString(), out leaveRequestId);
                                mobileCaptureTime = dtrLeaveMaster.Rows[k]["MobileTransactionDate"].ToString();
                                remarks = dtrLeaveMaster.Rows[k]["Remarks"].ToString();
                                leaveFromSessionId = dtrLeaveMaster.Rows[k]["LeaveFromSessionId"].ToString();
                                leaveToSessionId = dtrLeaveMaster.Rows[k]["LeaveToSessionId"].ToString();

                                DataTable dtrGPSForLeave = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + leaveRequestId + " and ProcessId =" + 2);

                                if (dtrGPSForLeave.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForLeave.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForLeave.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForLeave.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForLeave.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForLeave.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForLeave.Rows[0]["Source"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                }

                                string leaveSubData = "";
                                leaveSubData = userId + "@" + leaveReasonId + "@" + fromDate + "@" + toDate + "@" + latitude + "@" + longitude
                                    + "@" + processName + "@" + mobileCaptureTime + "@" + mobileReferenceNo + "@" + gpsSource + "@" + remarks
                                    + "@" + leaveFromSessionId + "@" + leaveToSessionId;

                                if (leaveData != "")
                                {
                                    leaveData = leaveData + "#" + leaveSubData;
                                }
                                else
                                {
                                    leaveData = leaveSubData;
                                }
                                k++;
                            }

                            dataArray[21] = leaveData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "MobileFeedback")
                        {                           

                            int k = 0;

                            int mobileFeedbackId = 0;

                            string feedbackData = "";

                            string userId = "", mobileCaptureTime = "", ansIdSet = "", ansSet = "", latitude = "",
                            longitude = "", processName = "", mobileReferenceNo = "", gpsSource = "";

                            processName = "btnFeedBack";
                            DataTable dtrFeadbackMaster = GetDataTable("select * from MobileFeedback");


                            foreach (DataRow row in dtrFeadbackMaster.Rows)
                            {
                                userId = dtrFeadbackMaster.Rows[k]["UserId"].ToString();
                                ansIdSet = dtrFeadbackMaster.Rows[k]["MobileFeedbackMasterId"].ToString();
                                ansSet = dtrFeadbackMaster.Rows[k]["Feedback"].ToString();
                                mobileReferenceNo = dtrFeadbackMaster.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrFeadbackMaster.Rows[k]["MobileFeedbackId"].ToString(), out mobileFeedbackId);
                                mobileCaptureTime = dtrFeadbackMaster.Rows[k]["MobileCapturedDate"].ToString();

                                DataTable dtrGPSForLeave = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + mobileFeedbackId + " and ProcessId =" + 19);

                                if (dtrGPSForLeave.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForLeave.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForLeave.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForLeave.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForLeave.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForLeave.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForLeave.Rows[0]["Source"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                }


                                string feedbackSubData = "";
                                feedbackSubData = userId + "@" + mobileCaptureTime + "@" + ansIdSet + "@" + ansSet + "@" + latitude + "@" + longitude
                                    + "@" + processName + "@" + mobileReferenceNo + "@" + gpsSource;

                                if (feedbackData != "")
                                {
                                    feedbackData = feedbackData + "#" + feedbackSubData;
                                }
                                else
                                {
                                    feedbackData = feedbackSubData;
                                }
                                k++;
                            }

                            dataArray[22] = feedbackData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "WorkingArea")
                        {                            
                            int k = 0;

                            int workingAreaId = 0;

                            string workingAreaData = "";

                            string userId = "", mobileCaptureTime = "", workingAreaMasterId = "", latitude = "",
                            longitude = "", processName = "", mobileReferenceNo = "", gpsSource = "", SignalStrength = "", NetworkProvider = "";

                            processName = "btnWorkingAt";
                            DataTable dtrWorkingAreaMaster = GetDataTable("select * from WorkingArea");


                            foreach (DataRow row in dtrWorkingAreaMaster.Rows)
                            {
                                workingAreaMasterId = dtrWorkingAreaMaster.Rows[k]["WorkingAreaMasterId"].ToString();
                                mobileReferenceNo = dtrWorkingAreaMaster.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrWorkingAreaMaster.Rows[k]["WorkingAreaId"].ToString(), out workingAreaId);
                                mobileCaptureTime = dtrWorkingAreaMaster.Rows[k]["MobileCapturedDate"].ToString();

                                DataTable dtrGPSForWorkingArea = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + workingAreaId + " and ProcessId =" + 18);

                                if (dtrGPSForWorkingArea.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForWorkingArea.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForWorkingArea.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForWorkingArea.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForWorkingArea.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForWorkingArea.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForWorkingArea.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForWorkingArea.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForWorkingArea.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForWorkingArea.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForWorkingArea.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }


                                string workingAreaSubData = "";
                                workingAreaSubData = mobileCaptureTime + "@" + workingAreaMasterId + "@" + latitude + "@" + longitude
                                    + "@" + processName + "@" + mobileReferenceNo + "@" + gpsSource + "@" + SignalStrength + "@" + NetworkProvider;

                                if (workingAreaData != "")
                                {
                                    workingAreaData = workingAreaData + "#" + workingAreaSubData;
                                }
                                else
                                {
                                    workingAreaData = workingAreaSubData;
                                }
                                k++;
                            }

                            dataArray[23] = workingAreaData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }
                        else if (tableSchema.TableName == "POPEntry")
                        {                            
                            //int counter = 0;
                            int k = 0;

                            int popId = 0;

                            string popData = "";

                            string userId = "", shopId = "", idSet = "", quantitySet = "", latitude = "",
                            longitude = "", processName = "", mobileTransactionDate = "", mobileReferenceNo = "", gpsSource = "", remarkSet = "";

                            processName = "btnPop";
                            DataTable dtrWorkingAreaMaster = GetDataTable("select * from POPEntry");


                            foreach (DataRow row in dtrWorkingAreaMaster.Rows)
                            {
                                userId = dtrWorkingAreaMaster.Rows[k]["DistributedBy"].ToString();
                                idSet = dtrWorkingAreaMaster.Rows[k]["POPId"].ToString();
                                shopId = dtrWorkingAreaMaster.Rows[k]["ShopId"].ToString();
                                remarkSet = dtrWorkingAreaMaster.Rows[k]["Remarks"].ToString();
                                quantitySet = dtrWorkingAreaMaster.Rows[k]["DistributedCount"].ToString();
                                userId = dtrWorkingAreaMaster.Rows[k]["DistributedBy"].ToString();
                                mobileReferenceNo = dtrWorkingAreaMaster.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrWorkingAreaMaster.Rows[k]["POPEntryId"].ToString(), out popId);
                                mobileTransactionDate = dtrWorkingAreaMaster.Rows[k]["MobileTransactionDate"].ToString();

                                DataTable dtrGPSForPop = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + popId + " and ProcessId =" + 44);

                                if (dtrGPSForPop.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForPop.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForPop.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForPop.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForPop.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForPop.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForPop.Rows[0]["Source"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                }


                                string popSubData = "";
                                popSubData = shopId + "@" + idSet + "@" + quantitySet + "@" + latitude + "@" + longitude
                                    + "@" + processName + "@" + mobileReferenceNo + "@" + mobileTransactionDate + "@" + gpsSource + "@" + remarkSet;

                                if (popData != "")
                                {
                                    popData = popData + "#" + popSubData;
                                }
                                else
                                {
                                    popData = popSubData;
                                }
                                k++;
                            }

                            dataArray[25] = popData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }


                        else if (tableSchema.TableName == "Signature")
                        {                            
                            int k = 0;
                            int signatureId = 0;

                            string signatureData = "";

                            string userId = "", shopId = "", data = "", shopkeeperName = "",
                                       latitude = "", longitude = "", processName = "", mobileReferenceNo = "", gpsSource = "", processId = "";

                            DataTable dtrSignatureMaster = GetDataTable("select * from Signature "); //dtr have all data from orderheader, this loop will exc. no. of rows times 

                            processName = "btnSignature";

                            foreach (DataRow row in dtrSignatureMaster.Rows)
                            {

                                userId = dtrSignatureMaster.Rows[k]["UserId"].ToString();
                                shopId = dtrSignatureMaster.Rows[k]["ShopId"].ToString();
                                data = dtrSignatureMaster.Rows[k]["Data"].ToString();
                                shopkeeperName = dtrSignatureMaster.Rows[k]["ShopkeeperName"].ToString();
                                mobileReferenceNo = dtrSignatureMaster.Rows[k]["MobileReferenceNo"].ToString();
                                processId = dtrSignatureMaster.Rows[k]["ProcessId"].ToString();
                                int.TryParse(dtrSignatureMaster.Rows[k]["SignatureId"].ToString(), out signatureId);

                                DataTable dtrGPSForOrder = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + signatureId + " and ProcessId =" + 22);

                                if (dtrGPSForOrder.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForOrder.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForOrder.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForOrder.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForOrder.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForOrder.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["Source"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                string signatureMasterData = "";
                                signatureMasterData = userId + "@" + shopId + "@" + data + "@" + shopkeeperName + "@" + latitude + "@" + longitude + "@" + processName + "@" +
                                               mobileReferenceNo + "@" + gpsSource + "@" + processId;


                                if (signatureData != "")
                                {
                                    signatureData = signatureData + "#" + signatureMasterData;

                                }
                                else
                                {
                                    signatureData = signatureMasterData;
                                }

                                k++;
                            }

                            dataArray[20] = signatureData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "Enquiry")
                        {                            
                            // int counter = 0;
                            int k = 0;
                            int enquiryId = 0;


                            string enquiryData = "";

                            DataTable dtrenquiryMaster = GetDataTable("select * from Enquiry "); //dtr have all data from orderheader, this loop will exc. no. of rows times 

                            foreach (DataRow row in dtrenquiryMaster.Rows)
                            {
                                string enquiredby = "", activityId = "", shopid = "", remarks = "",
                                       latitude = "", longitude = "", processName = "", mobileTransactionDate = ""
                                       , mobileReferenceNo = "", gpsSource = "", productId = "", tempShopId = "", SignalStrength = "", NetworkProvider = "";
                                processName = "btnEnquiry";
                                enquiredby = dtrenquiryMaster.Rows[k]["EnquiredBy"].ToString();
                                activityId = dtrenquiryMaster.Rows[k]["EnquiryActivityId"].ToString();
                                shopid = dtrenquiryMaster.Rows[k]["ShopId"].ToString();
                                mobileReferenceNo = dtrenquiryMaster.Rows[k]["MobileReferenceNo"].ToString();
                                remarks = dtrenquiryMaster.Rows[k]["Remarks"].ToString();
                                mobileTransactionDate = dtrenquiryMaster.Rows[k]["MobileTransactionDate"].ToString();
                                productId = dtrenquiryMaster.Rows[k]["ProductId"].ToString();
                                tempShopId = dtrenquiryMaster.Rows[k]["TempShopId"].ToString();
                                int.TryParse(dtrenquiryMaster.Rows[k]["EnquiryId"].ToString(), out enquiryId);


                                DataTable dtrGPSForOrder = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + enquiryId + " and ProcessId =" + 26);

                                if (dtrGPSForOrder.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForOrder.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForOrder.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForOrder.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForOrder.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForOrder.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForOrder.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForOrder.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                string enquiryMasterData = "";
                                enquiryMasterData = enquiredby + "@" + activityId + "@" + shopid + "@" + remarks + "@" + latitude + "@" + longitude + "@" + processName + "@" +
                                               mobileTransactionDate + "@" + mobileReferenceNo + "@" + gpsSource + "@" + productId + "@" + tempShopId + "@" + SignalStrength + "@" + NetworkProvider;


                                if (enquiryData != "")
                                {
                                    enquiryData = enquiryData + "#" + enquiryMasterData;

                                }
                                else
                                {
                                    enquiryData = enquiryMasterData;
                                }

                                k++;
                            }

                            dataArray[12] = enquiryData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "Appointment")
                        {                            
                            // int counter = 0;
                            int k = 0;
                            int enquiryId = 0;


                            string appointmentData = "";

                            DataTable dtrappintmentMaster = GetDataTable("select * from Appointment "); //dtr have all data from orderheader, this loop will exc. no. of rows times 

                            foreach (DataRow row in dtrappintmentMaster.Rows)
                            {
                                string userId = "", AppointmentId = "", latitude = "", longitude = "", gpsSource = ""
                                    , AppointmentDate = "", AppointmentDescription = "", processName = "", referenceNo = "";
                                processName = "btnappointment";
                                userId = dtrappintmentMaster.Rows[k]["userId"].ToString();
                                AppointmentId = dtrappintmentMaster.Rows[k]["AppointmentId"].ToString();
                                AppointmentDate = dtrappintmentMaster.Rows[k]["AppointmentDate"].ToString();
                                AppointmentDescription = dtrappintmentMaster.Rows[k]["AppointmentDescription"].ToString();
                                referenceNo = dtrappintmentMaster.Rows[k]["MobileReferenceNo"].ToString();


                                DataTable dtrGPSForOrder = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + AppointmentId + " and ProcessId =" + 43);

                                if (dtrGPSForOrder.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForOrder.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForOrder.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForOrder.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForOrder.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForOrder.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["Source"].ToString();

                                }
                                string appointmentMasterData = "";
                                appointmentMasterData = userId + "~" + AppointmentDate + "~" + AppointmentDescription + "~" + latitude + "~" + longitude + "~" + processName + "~"
                                               + gpsSource + "~" + referenceNo;


                                if (appointmentData != "")
                                {
                                    appointmentData = appointmentData + "#" + appointmentMasterData;

                                }
                                else
                                {
                                    appointmentData = appointmentMasterData;
                                }

                                k++;
                            }

                            dataArray[27] = appointmentData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }

                        else if (tableSchema.TableName == "ComplaintEntry")
                        {                            
                            int k = 0;

                            int complaintId = 0;


                            string complaintData = "";

                            DataTable dtrComplaintMaster = GetDataTable("select * from ComplaintEntry "); //dtr have all data from orderheader, this loop will exc. no. of rows times 

                            foreach (DataRow row in dtrComplaintMaster.Rows)
                            {
                                string userId = "", complaintid = "", shopid = "", remarks = "",
                                       latitude = "", longitude = "", processName = "", mobileTransactionDate = "", mobileReferenceNo = "", gpsSource = "", SignalStrength = "", NetworkProvider = "";
                                processName = "btnComplaint";
                                userId = dtrComplaintMaster.Rows[k]["ReportedBy"].ToString();
                                complaintid = dtrComplaintMaster.Rows[k]["ComplaintId"].ToString();
                                shopid = dtrComplaintMaster.Rows[k]["ShopId"].ToString();
                                remarks = dtrComplaintMaster.Rows[k]["Complaint"].ToString();
                                int.TryParse(dtrComplaintMaster.Rows[k]["ComplaintEntryId"].ToString(), out complaintId);
                                mobileTransactionDate = dtrComplaintMaster.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = dtrComplaintMaster.Rows[k]["MobileReferenceNo"].ToString();


                                DataTable dtrGPSForOrder = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + complaintId + " and ProcessId =" + 27);

                                if (dtrGPSForOrder.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForOrder.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForOrder.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForOrder.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForOrder.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForOrder.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForOrder.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForOrder.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForOrder.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                string complaintMasterData = "";
                                complaintMasterData = userId + "@" + complaintid + "@" + shopid + "@" + remarks + "@" + latitude + "@" + longitude + "@" + processName + "@" +
                                               mobileTransactionDate + "@" + mobileReferenceNo + "@" + gpsSource + "@" + SignalStrength + "@" + NetworkProvider;


                                if (complaintData != "")
                                {
                                    complaintData = complaintData + "#" + complaintMasterData;

                                }
                                else
                                {
                                    complaintData = complaintMasterData;
                                }

                                k++;
                            }

                            dataArray[13] = complaintData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "Remittance")
                        {
                            int k = 0;

                            int remittanceId = 0;
                            string remittanceData = "";

                            string userId = "", amount = "", bankId = "", latitude = "", longitude = "",
                                processName = "", mobileTransactionDate = "", mobileReferenceNo = "", gpsSource = "" ,remarks="",
                                 denominationIds = string.Empty, denominationCounts = string.Empty, approvedBy = "";
                            processName = "btnRemittance";

                            DataTable dtrRemittanceMaster = GetDataTable("select * from Remittance"); //dtr have all data from orderheader, this loop will exc. no. of rows times 

                            foreach (DataRow row in dtrRemittanceMaster.Rows)
                            {

                                userId = dtrRemittanceMaster.Rows[k]["RemittedBy"].ToString();
                                amount = dtrRemittanceMaster.Rows[k]["Amount"].ToString();
                                bankId = dtrRemittanceMaster.Rows[k]["BankId"].ToString();
                                int.TryParse(dtrRemittanceMaster.Rows[k]["RemittanceId"].ToString(), out remittanceId);
                                mobileTransactionDate = dtrRemittanceMaster.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = dtrRemittanceMaster.Rows[k]["MobileReferenceNo"].ToString();
                                approvedBy = dtrRemittanceMaster.Rows[k]["ApprovedBy"].ToString();
                                remarks = dtrRemittanceMaster.Rows[k]["Remarks"].ToString();

                                DataTable dtrGPSForRemittance = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + remittanceId + " and ProcessId =" + 4);

                                if (dtrGPSForRemittance.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForRemittance.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForRemittance.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForRemittance.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForRemittance.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForRemittance.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForRemittance.Rows[0]["Source"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                DataTable dtrRemittanceDenominations = GetDataTable("select * from RemittanceDenominations WHERE  RemittanceId=" + remittanceId);
                                foreach (DataRow drdenomrow in dtrRemittanceDenominations.Rows)
                                {
                                    string denominationId = string.Empty;
                                    string denomnationCount = string.Empty;

                                    denominationId = drdenomrow["DenominationId"].ToString();
                                    denomnationCount = drdenomrow["DenominationCount"].ToString();
                                    if (denominationIds != string.Empty)
                                    {
                                        denominationIds = denominationIds + "," + denominationId;
                                    }
                                    else
                                    {
                                        denominationIds = denominationId;
                                    }
                                    if (denominationCounts != string.Empty)
                                    {
                                        denominationCounts = denominationCounts + "," + denomnationCount;
                                    }
                                    else
                                    {
                                        denominationCounts = denomnationCount;
                                    }

                                }

                                string remittanceMasterData = "";
                                remittanceMasterData = userId + "@" + amount + "@" + bankId + "@" + latitude + "@" + longitude + "@" + processName + "@" +
                                               mobileTransactionDate + "@" + mobileReferenceNo + "@" + gpsSource + "@" + denominationIds + "@" + denominationCounts + "@" + approvedBy+ "@"+remarks;


                                if (remittanceData != "")
                                {
                                    remittanceData = remittanceData + "#" + remittanceMasterData;

                                }
                                else
                                {
                                    remittanceData = remittanceMasterData;
                                }

                                k++;
                            }

                            dataArray[26] = remittanceData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "StockReturn")
                        {                            
                            //int counter = 0;
                            int k = 0;

                            int stockReturnId = 0;


                            string stockReturnData = "";

                            string shopId = "", idSet = "", quantitySet = "", priceSet = ""
                            , batchNoSet = "", pkdDateSet = "", latitude = "", longitude = "", processName = "",
                            mobileTransactionDate = "", mobileReferenceNo = "", rate = "", reason = "", gpsSource = "", unitId = "", recieptNo = "", schemeIdSet = "", SignalStrength = "", NetworkProvider = "", remark = "";

                            processName = "btnSalesReturn";
                            DataTable dtrStockReturnMaster = GetDataTable("select * from StockReturn ");


                            foreach (DataRow row in dtrStockReturnMaster.Rows)
                            {
                                int.TryParse(dtrStockReturnMaster.Rows[k]["StockReturnId"].ToString(), out stockReturnId);
                                shopId = dtrStockReturnMaster.Rows[k]["ShopId"].ToString();
                                idSet = dtrStockReturnMaster.Rows[k]["ProductAttributeId"].ToString();
                                quantitySet = dtrStockReturnMaster.Rows[k]["Quantity"].ToString();
                                priceSet = dtrStockReturnMaster.Rows[k]["Amount"].ToString();
                                batchNoSet = dtrStockReturnMaster.Rows[k]["BatchNo"].ToString();
                                pkdDateSet = dtrStockReturnMaster.Rows[k]["PkdDate"].ToString();
                                mobileTransactionDate = dtrStockReturnMaster.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = dtrStockReturnMaster.Rows[k]["MobileReferenceNo"].ToString();
                                rate = dtrStockReturnMaster.Rows[k]["Rate"].ToString();
                                reason = dtrStockReturnMaster.Rows[k]["ReturnReasonId"].ToString();
                                unitId = dtrStockReturnMaster.Rows[k]["UnitId"].ToString();
                                recieptNo = dtrStockReturnMaster.Rows[k]["ReceiptNumber"].ToString();
                                schemeIdSet = dtrStockReturnMaster.Rows[k]["SchemeId"].ToString();
                                remark = dtrStockReturnMaster.Rows[k]["Remark"].ToString();
                                DataTable dtrGPSForCollection = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + stockReturnId + " and ProcessId =" + 28);

                                if (dtrGPSForCollection.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForCollection.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForCollection.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForCollection.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForCollection.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForCollection.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForCollection.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForCollection.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForCollection.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForCollection.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForCollection.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }

                                string stockReturnSubData = "";
                                stockReturnSubData = shopId + "@" + idSet + "@" + quantitySet + "@" + priceSet + "@" + batchNoSet + "@" + pkdDateSet
                                    + "@" + latitude + "@" + longitude + "@" + processName
                                    + "@" + mobileTransactionDate + "@" + mobileReferenceNo + "@" + rate + "@" + reason + "@" + gpsSource + "@" + unitId
                                    + "@" + recieptNo + "@" + schemeIdSet + "@" + SignalStrength + "@" + NetworkProvider + "@" + remark;

                                if (stockReturnData != "")
                                {
                                    stockReturnData = stockReturnData + "#" + stockReturnSubData;
                                }
                                else
                                {
                                    stockReturnData = stockReturnSubData;
                                }
                                k++;
                            }

                            dataArray[14] = stockReturnData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "ParameterCapture")
                        {                           
                            //int counter = 0;
                            int k = 0;

                            int parameterCaptureId = 0;

                            string shopId = "", productid = "", quantitydata = "", parameters = ""
                            , latitude = "", longitude = "", processName = "",
                            mobileTransactionDate = "", mobileReferenceNo = "", gpsSource = "", SignalStrength = "", NetworkProvider = "";

                            string parameterCaptureData = "";
                            processName = "btnCompetitor";
                            DataTable parameterCaptureMaster = GetDataTable("select * from ParameterCapture ");


                            foreach (DataRow row in parameterCaptureMaster.Rows)
                            {
                                int.TryParse(parameterCaptureMaster.Rows[k]["ParameterCaptureId"].ToString(), out parameterCaptureId);
                                shopId = parameterCaptureMaster.Rows[k]["ShopId"].ToString();
                                productid = parameterCaptureMaster.Rows[k]["CompetitorProductId"].ToString();
                                parameters = parameterCaptureMaster.Rows[k]["ParameterId"].ToString();
                                quantitydata = parameterCaptureMaster.Rows[k]["Data"].ToString();
                                mobileTransactionDate = parameterCaptureMaster.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = parameterCaptureMaster.Rows[k]["MobileReferenceNo"].ToString();

                                DataTable dtrGPSForCompetitor = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + parameterCaptureId + " and ProcessId =" + 15);

                                if (dtrGPSForCompetitor.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForCompetitor.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForCompetitor.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForCompetitor.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForCompetitor.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForCompetitor.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForCompetitor.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForCompetitor.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForCompetitor.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForCompetitor.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForCompetitor.Rows[0]["NetworkProviderId"].ToString();
                                    //_log.LogMessage("GPS Source", "Id : " + gpsSource, "L1");
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }

                                string parameterCaptureSubData = "";
                                parameterCaptureSubData = shopId + "@" + productid + "@" + quantitydata + "@" + parameters
                                + "@" + latitude + "@" + longitude + "@" + processName
                                + "@" + mobileTransactionDate + "@" + mobileReferenceNo + "@" + gpsSource + "@" + SignalStrength + "@" + NetworkProvider;

                                if (parameterCaptureData != "")
                                {
                                    parameterCaptureData = parameterCaptureData + "#" + parameterCaptureSubData;
                                }
                                else
                                {
                                    parameterCaptureData = parameterCaptureSubData;
                                }

                                k++;
                            }

                            dataArray[15] = parameterCaptureData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }

                        else if (tableSchema.TableName == "BeatPlanDeviation")
                        {

                            int k = 0;
                            int beatPlanDeviationIdId = 0;

                            string deviationData = "";

                            string userId = "", beatPlanId = "", mobileTransactionDate = "", deviationReasonId = "", latitude = "",
                            longitude = "", mobileReferenceNo = "", gpsSource = "", signalStrength = "", processName = "";

                            processName = "btnBeatPlanDeviation";
                            DataTable dtrBeatPlanDeviation = GetDataTable("select * from BeatPlanDeviation");


                            foreach (DataRow row in dtrBeatPlanDeviation.Rows)
                            {
                                userId = dtrBeatPlanDeviation.Rows[k]["UserId"].ToString();
                                beatPlanId = dtrBeatPlanDeviation.Rows[k]["BeatPlanId"].ToString();
                                deviationReasonId = dtrBeatPlanDeviation.Rows[k]["DeviationReasonId"].ToString();
                                mobileReferenceNo = dtrBeatPlanDeviation.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrBeatPlanDeviation.Rows[k]["Id"].ToString(), out beatPlanDeviationIdId);
                                mobileTransactionDate = dtrBeatPlanDeviation.Rows[k]["MobileTransactionDate"].ToString();

                                DataTable dtrGPSBeatPlanDeviation = GetDataTable("select latitude,longitude,Source,SignalStrength from GPSLocationInfo WHERE  ProcessDetailsId=" + beatPlanDeviationIdId + " and MobileReferenceNo ='" + mobileReferenceNo + "'");

                                if (dtrGPSBeatPlanDeviation.Rows.Count > 0)
                                {
                                    latitude = dtrGPSBeatPlanDeviation.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSBeatPlanDeviation.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSBeatPlanDeviation.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSBeatPlanDeviation.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSBeatPlanDeviation.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSBeatPlanDeviation.Rows[0]["Source"].ToString();
                                    signalStrength = dtrGPSBeatPlanDeviation.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSBeatPlanDeviation.Rows[0]["SignalStrength"].ToString();
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                }

                                string deviationSubData = "";
                                deviationSubData = userId + "@" + beatPlanId + "@" + deviationReasonId + "@" + mobileTransactionDate + "@" + latitude + "@" + longitude
                                    + "@" + processName + "@" + mobileReferenceNo + "@" + gpsSource + "@" + signalStrength;

                                if (deviationData != "")
                                {
                                    deviationData = deviationData + "#" + deviationSubData;
                                }
                                else
                                {
                                    deviationData = deviationSubData;
                                }
                                k++;
                            }

                            dataArray[35] = deviationData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "PjpHeader")
                        {
                            int k = 0;
                            int pjpHeaderId = 0;


                            string pjpData = "";
                            string userData = "";
                            string dateData = "";
                            string remarksData = "";
                            string statusData = "";
                            string mobileTransactionDates = "";
                            string beatplanData = "";
                            string commonRemarksData = "";

                            string userId = "", date = "", beatPlanIds = "", remarks = "", status = "", mobileTransactionDate = "", commonRemarks = "";


                            DataTable dtrpjpHeader = GetDataTable("select * from PjpHeader");

                            foreach (DataRow row in dtrpjpHeader.Rows)
                            {
                                userId = string.Empty;
                                date = string.Empty;
                                beatPlanIds = string.Empty;
                                remarks = string.Empty;
                                status = string.Empty;
                                commonRemarks = string.Empty;

                                userId = dtrpjpHeader.Rows[k]["UserId"].ToString();
                                date = dtrpjpHeader.Rows[k]["PjpDate"].ToString();
                                mobileTransactionDate = dtrpjpHeader.Rows[k]["MobileTransactionDate"].ToString();
                                remarks = dtrpjpHeader.Rows[k]["DateWiseRemarks"].ToString();
                                if (remarks == string.Empty)
                                {
                                    remarks = " ";
                                }
                                status = dtrpjpHeader.Rows[k]["Status"].ToString();
                                commonRemarks = dtrpjpHeader.Rows[k]["Remarks"].ToString();
                                if (commonRemarks == string.Empty)
                                {
                                    commonRemarks = " ";
                                }
                                int.TryParse(dtrpjpHeader.Rows[k]["Id"].ToString(), out pjpHeaderId);

                                int k1 = 0;

                                DataTable dtrpjpDetails = GetDataTable("select * from PjpDetails WHERE PjpHeaderId=" + pjpHeaderId);

                                foreach (DataRow rows in dtrpjpDetails.Rows)
                                {
                                    string beatPlanId = dtrpjpDetails.Rows[k1]["BeatPlanId"].ToString();

                                    if (beatPlanIds != "")
                                    {
                                        beatPlanIds = beatPlanIds + "," + beatPlanId;
                                    }
                                    else
                                    {
                                        beatPlanIds = beatPlanId;
                                    }

                                    k1++;

                                }

                                if (userData != string.Empty)
                                {
                                    userData = userData + ',' + userId;
                                }
                                else
                                {
                                    userData = userId;
                                }
                                if (dateData != string.Empty)
                                {
                                    dateData = dateData + ',' + date;
                                }
                                else
                                {
                                    dateData = date;
                                }
                                if (remarksData != string.Empty)
                                {
                                    remarksData = remarksData + ',' + remarks;
                                }
                                else
                                {
                                    remarksData = remarks;
                                }
                                if (statusData != string.Empty)
                                {
                                    statusData = statusData + ',' + status;
                                }
                                else
                                {
                                    statusData = status;
                                }
                                if (mobileTransactionDates != string.Empty)
                                {
                                    mobileTransactionDates = mobileTransactionDates + ',' + mobileTransactionDate;
                                }
                                else
                                {
                                    mobileTransactionDates = mobileTransactionDate;
                                }
                                if (commonRemarksData != string.Empty)
                                {
                                    commonRemarksData = commonRemarksData + ',' + commonRemarks;
                                }
                                else
                                {
                                    commonRemarksData = commonRemarks;
                                }
                                if (beatplanData != string.Empty)
                                {
                                    beatplanData = beatplanData + '#' + beatPlanIds;
                                }
                                else
                                {
                                    beatplanData = beatPlanIds;
                                }
                                k++;

                            }
                            pjpData = userData + "~" + dateData + "~" + remarksData + "~" + statusData + "~" + mobileTransactionDates + "~" + beatplanData
                                + "~" + commonRemarksData;

                            dataArray[37] = pjpData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");


                        }
                        else if (tableSchema.TableName == "ShopWiseDistributor")
                        {                                                        
                            string shopIds = "", distributorIds = "";
                            string shopDefaultDistributorData = "";

                            DataTable dtrShopWiseDefaultDistributor = GetDataTable("select * from ShopWiseDistributor ");
                            if (dtrShopWiseDefaultDistributor.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtrShopWiseDefaultDistributor.Rows)
                                {
                                    string shopId = row["ShopId"].ToString();
                                    string distributorId = row["DistributorId"].ToString();
                                    if (shopIds == "")
                                    {
                                        shopIds = shopId;
                                    }
                                    else
                                    {
                                        shopIds = shopIds + ',' + shopId;
                                    }
                                    if (distributorIds == "")
                                    {
                                        distributorIds = distributorId;
                                    }
                                    else
                                    {
                                        distributorIds = distributorIds + ',' + distributorId;
                                    }
                                }

                                shopDefaultDistributorData = shopIds + "@" + distributorIds;
                            }
                            dataArray[38] = shopDefaultDistributorData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "VW_StockTable")
                        {
                            int k = 0;
                            int stockHeaderId = 0;

                            string loadedStockData = "";

                            string userId = "", productSet = "", unitSet = "", stockSet = "";

                            DataTable dtrLoadedStock = GetDataTable("select Distinct StockHeaderId from VW_StockTable");

                            foreach (DataRow row in dtrLoadedStock.Rows)
                            {
                                int.TryParse(dtrLoadedStock.Rows[k]["StockHeaderId"].ToString(), out stockHeaderId);

                                int k1 = 0;

                                DataTable dtrStockDetails = GetDataTable("select * from VW_StockTable WHERE  StockHeaderId=" + stockHeaderId);

                                foreach (DataRow rows in dtrStockDetails.Rows)
                                {
                                    string productId = dtrStockDetails.Rows[k1]["ProductAttributeId"].ToString();
                                    string stock = dtrStockDetails.Rows[k1]["Stock"].ToString();
                                    string unit = dtrStockDetails.Rows[k1]["UnitId"].ToString();

                                    if (productSet != "")
                                    {
                                        productSet = productSet + "," + productId; ;
                                    }
                                    else
                                    {
                                        productSet = productId;
                                    }
                                    if (stockSet != "")
                                    {
                                        stockSet = stockSet + "," + stock;

                                    }
                                    else
                                    {
                                        stockSet = stock;
                                    }
                                    if (unitSet != "")
                                    {
                                        unitSet = unitSet + "," + unit;
                                    }
                                    else
                                    {
                                        unitSet = unit;
                                    }

                                    k1++;

                                }

                                string stockMasterData = "";
                                stockMasterData = userId + "~" + stockHeaderId + "~" + productSet + "~" + stockSet + "~" + unitSet;

                                if (loadedStockData != "")
                                {
                                    loadedStockData = loadedStockData + "#" + stockMasterData;

                                }
                                else
                                {
                                    loadedStockData = stockMasterData;
                                }

                                k++;

                            }

                            dataArray[41] = loadedStockData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "BTLActivityDetails")
                        {
                            int k = 0;
                            int btlActivityDetailsId = 0;

                            string btlActivityData = "";
                            string attendessData = "";                            
                            string configFieldIds = "";
                            string configFieldValues = "";
                            
                            string userId = "", organizationId = "", btlActivityId = "", mobileReferenceNo = "", mobileTransactionDate = "",latitude = "999"
                                    , longitude = "", source = "", SignalStrength = "", NetworkProvider = "";

                            DataTable dtrbtlActivityDetails = GetDataTable("select * from BTLActivityDetails");

                            foreach (DataRow row in dtrbtlActivityDetails.Rows)
                            {
                                userId = string.Empty;
                                organizationId = string.Empty;
                                btlActivityId = string.Empty;
                                mobileReferenceNo = string.Empty;
                                mobileTransactionDate = string.Empty;

                                userId = dtrbtlActivityDetails.Rows[k]["UserId"].ToString();
                                organizationId = dtrbtlActivityDetails.Rows[k]["OrganisationId"].ToString();
                                mobileTransactionDate = dtrbtlActivityDetails.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = dtrbtlActivityDetails.Rows[k]["MobileReferenceNo"].ToString();
                                btlActivityId = dtrbtlActivityDetails.Rows[k]["BTLActivityId"].ToString();

                                int.TryParse(dtrbtlActivityDetails.Rows[k]["Id"].ToString(), out btlActivityDetailsId);

                                int k1 = 0;

                                DataTable dtrBTLDetailsConfig = GetDataTable("select * from BTLDetailsConfig WHERE BTLActivityDetailsId=" + btlActivityDetailsId);

                                foreach (DataRow rows in dtrBTLDetailsConfig.Rows)
                                {
                                    string configFieldId = dtrBTLDetailsConfig.Rows[k1]["ConfigFieldId"].ToString();

                                    if (configFieldIds != "")
                                    {
                                        configFieldIds = configFieldIds + "," + configFieldId;
                                    }
                                    else
                                    {
                                        configFieldIds = configFieldId;
                                    }
                                    string configFieldValue = dtrBTLDetailsConfig.Rows[k1]["Value"].ToString();

                                    if (configFieldValues != "")
                                    {
                                        configFieldValues = configFieldValues + "," + configFieldValue;
                                    }
                                    else
                                    {
                                        configFieldValues = configFieldValue;
                                    }
                                    k1++;

                                }

                                int k2 = 0;

                                DataTable dtrBTLAttendees = GetDataTable("select * from BTLActivityAttendees WHERE BTLActivityDetailsId=" + btlActivityDetailsId);

                                foreach (DataRow rows in dtrBTLAttendees.Rows)
                                {
                                    string shopId = dtrBTLAttendees.Rows[k2]["ShopId"].ToString();

                                    if (attendessData != "")
                                    {
                                        attendessData = attendessData + "," + shopId;
                                    }
                                    else
                                    {
                                        attendessData = shopId;
                                    }
                                    
                                    k2++;

                                }

                                DataTable dtrGPSForBTL = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + btlActivityDetailsId + " and MobileReferenceNo ='" + mobileReferenceNo + "'");
                                if (dtrGPSForBTL.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForBTL.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForBTL.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForBTL.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForBTL.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForBTL.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForBTL.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForBTL.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForBTL.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForBTL.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForBTL.Rows[0]["NetworkProviderId"].ToString();                                    
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    source = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }
                                k++;


                                string btlData = userId + "@" + organizationId + "@" + btlActivityId + "@" + mobileReferenceNo + "@" + mobileTransactionDate + "@" +
                                                 configFieldIds + "@" + configFieldValues + "@" + attendessData + "@" + latitude + "@" + longitude + "@" + source + "@" + 
                                                 SignalStrength + "@" + NetworkProvider;

                                if (btlActivityData != "")
                                {
                                    btlActivityData = btlActivityData + "#" + btlData;

                                }
                                else
                                {
                                    btlActivityData = btlData;
                                }
                            }

                            dataArray[43] = btlActivityData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");


                        }
                        else if (tableSchema.TableName == "MonthlyTourPlans")
                        {
                            int k = 0;
                            int monthlyTourPlanId = 0;
                            string monthlyTourPlanData = "";
                            string userId = "", TourPlanDate = "", SubmissionRemark = "", SubmissionDate = "", SubmittedBy = "", mobileReferenceNo = "", mobileTransactionDate = "", latitude = "999"
                                    , longitude = "", source = "", SignalStrength = "", NetworkProvider = "";

                            DataTable dtMonthlyTourPlan = GetDataTable("select * from MonthlyTourPlans");

                            foreach (DataRow row in dtMonthlyTourPlan.Rows)
                            {
                                //Id#TourPlanDate#UserId#SubmissionRemark#SubmissionDate#SubmittedBy#MobileReferenceNo#MobileTransactionDate#MonthPosition
                                userId = string.Empty;
                                TourPlanDate = string.Empty;
                                SubmissionRemark = string.Empty;
                                SubmissionDate = string.Empty;
                                SubmittedBy = string.Empty;
                                mobileReferenceNo = string.Empty;
                                mobileTransactionDate = string.Empty;                                

                                userId = dtMonthlyTourPlan.Rows[k]["UserId"].ToString();
                                TourPlanDate = dtMonthlyTourPlan.Rows[k]["TourPlanDate"].ToString();
                                SubmissionRemark = dtMonthlyTourPlan.Rows[k]["SubmissionRemark"].ToString();
                                SubmissionDate = dtMonthlyTourPlan.Rows[k]["SubmissionDate"].ToString();
                                SubmittedBy = dtMonthlyTourPlan.Rows[k]["SubmittedBy"].ToString();                                
                                mobileTransactionDate = dtMonthlyTourPlan.Rows[k]["MobileTransactionDate"].ToString();
                                mobileReferenceNo = dtMonthlyTourPlan.Rows[k]["MobileReferenceNo"].ToString();                                

                                int.TryParse(dtMonthlyTourPlan.Rows[k]["Id"].ToString(), out monthlyTourPlanId);

                                int l = 0;
                                int dailyTourPlanId = 0;
                                string dailyTourPlanData = "";
                                string dailyUserId = "", Date = "", Status = "", ActionById = "", ActionDate = "", dailyMobileReferenceNo = "", dailyMobileTransactionDate = "";
                                //Id#MonthlyTourPlanId#Date#UserId#Status#ActionById#ActionDate#MobileReferenceNo#MobileTransactionDate
                                DataTable dtDailyTourPlans = GetDataTable("select * from DailyTourPlans WHERE MonthlyTourPlanId=" + monthlyTourPlanId);
                                
                                foreach (DataRow rows in dtDailyTourPlans.Rows)
                                {
                                    dailyUserId = string.Empty;
                                    Date = string.Empty;
                                    Status = string.Empty;
                                    ActionById = string.Empty;
                                    ActionDate = string.Empty;
                                    dailyMobileReferenceNo = string.Empty;
                                    dailyMobileTransactionDate = string.Empty;

                                    dailyUserId = dtDailyTourPlans.Rows[l]["UserId"].ToString();
                                    Date = dtDailyTourPlans.Rows[l]["Date"].ToString();
                                    Status = dtDailyTourPlans.Rows[l]["Status"].ToString();
                                    ActionById = dtDailyTourPlans.Rows[l]["ActionById"].ToString();
                                    ActionDate = dtDailyTourPlans.Rows[l]["ActionDate"].ToString();
                                    dailyMobileReferenceNo = dtDailyTourPlans.Rows[l]["MobileTransactionDate"].ToString();
                                    dailyMobileTransactionDate = dtDailyTourPlans.Rows[l]["MobileReferenceNo"].ToString();

                                    int.TryParse(dtDailyTourPlans.Rows[l]["Id"].ToString(), out dailyTourPlanId);

                                    //Id#BeatPlanId#DailyTourPlanId
                                    int m = 0;
                                    string routePlanData = "";
                                    string beatPlanId = "";                                
                                    DataTable dtRoutePlans = GetDataTable("select * from RoutePlans WHERE DailyTourPlanId=" + dailyTourPlanId);
                                    foreach(DataRow x in dtRoutePlans.Rows)
                                    {
                                        beatPlanId = string.Empty;

                                        beatPlanId = dtRoutePlans.Rows[m]["BeatPlanId"].ToString();

                                        if(routePlanData != "")
                                        {
                                            routePlanData = routePlanData + '#' + beatPlanId;
                                        }
                                        else
                                        {
                                            routePlanData = beatPlanId;
                                        }
                                        m++;
                                    }
                                    //Id#DailyTourPlanId#JointUserId
                                    int n = 0;
                                    string jointWorkPlanData = "";
                                    string jointUserId = "";
                                    DataTable dtJointWorkPlans = GetDataTable("select * from JointWorkPlans WHERE DailyTourPlanId=" + dailyTourPlanId);
                                    foreach (DataRow y in dtJointWorkPlans.Rows)
                                    {
                                        jointUserId = string.Empty;

                                        jointUserId = dtJointWorkPlans.Rows[n]["JointUserId"].ToString();

                                        if(jointWorkPlanData != "")
                                        {
                                            jointWorkPlanData = jointWorkPlanData + '#' + jointUserId;
                                        }
                                        else
                                        {
                                            jointWorkPlanData = jointUserId;
                                        }
                                        n++;
                                    }
                                    //Id#DailyTourPlanId#ActivityId#ConfigFieldId#Value
                                    int o = 0;
                                    string activityPlanData = "";
                                    string activityId = "", configFieldId = "", value = "";
                                    DataTable dtActivityPlans = GetDataTable("select * from ActivityPlans WHERE DailyTourPlanId=" + dailyTourPlanId);
                                    foreach (DataRow z in dtActivityPlans.Rows)
                                    {
                                        activityId = string.Empty;
                                        configFieldId = string.Empty;
                                        value = string.Empty;

                                        activityId = dtActivityPlans.Rows[o]["ActivityId"].ToString();
                                        configFieldId = dtActivityPlans.Rows[o]["ConfigFieldId"].ToString();
                                        value = dtActivityPlans.Rows[o]["Value"].ToString();
                                        string activityPlanSubData = "";
                                        activityPlanSubData = activityId + "@" + configFieldId + "@" + value;
                                        if (activityPlanData != "")
                                        {
                                            activityPlanData = activityPlanData + "#" + activityPlanSubData;
                                        }
                                        else
                                        {
                                            activityPlanData = activityPlanSubData;
                                        }
                                        o++;
                                    }
                                    int p = 0;
                                    int leaveRequestId = 0;
                                    string leaveData = "";
                                    string LeaveUserId = "", mobileCaptureTime = "", leaveReasonId = "", fromDate = ""
                                    , toDate = "", leaveLatitude = "",
                                    leaveLongitude = "", processName = "", leaveMobileReferenceNo = "", gpsSource = "", remarks = ""
                                    , leaveFromSessionId = "", leaveToSessionId = "";

                                    processName = "btnLeave";
                                    DataTable dtrLeaveMaster = GetDataTable("select * from LeaveRequest WHERE DailyTourPlanId=" + dailyTourPlanId);

                                    foreach (DataRow row2 in dtrLeaveMaster.Rows)
                                    {
                                        LeaveUserId = dtrLeaveMaster.Rows[p]["RequestedBy"].ToString();
                                        leaveReasonId = dtrLeaveMaster.Rows[p]["LeaveReasonId"].ToString();
                                        fromDate = dtrLeaveMaster.Rows[p]["LeaveFrom"].ToString();
                                        toDate = dtrLeaveMaster.Rows[p]["LeaveTo"].ToString();
                                        leaveMobileReferenceNo = dtrLeaveMaster.Rows[p]["MobileReferenceNo"].ToString();
                                        int.TryParse(dtrLeaveMaster.Rows[p]["LeaveRequestId"].ToString(), out leaveRequestId);
                                        mobileCaptureTime = dtrLeaveMaster.Rows[p]["MobileTransactionDate"].ToString();
                                        remarks = dtrLeaveMaster.Rows[p]["Remarks"].ToString();
                                        leaveFromSessionId = dtrLeaveMaster.Rows[p]["LeaveFromSessionId"].ToString();
                                        leaveToSessionId = dtrLeaveMaster.Rows[p]["LeaveToSessionId"].ToString();

                                        DataTable dtrGPSForLeave = GetDataTable("select latitude,longitude,Source from GPSLocationInfo WHERE  ProcessDetailsId=" + leaveRequestId + " and ProcessId =" + 2);

                                        if (dtrGPSForLeave.Rows.Count > 0)
                                        {
                                            leaveLatitude = dtrGPSForLeave.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForLeave.Rows[0]["latitude"].ToString();
                                            leaveLongitude = dtrGPSForLeave.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForLeave.Rows[0]["longitude"].ToString();
                                            gpsSource = dtrGPSForLeave.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForLeave.Rows[0]["Source"].ToString();                                            
                                        }
                                        else
                                        {
                                            leaveLatitude = "999";
                                            leaveLongitude = "999";
                                            gpsSource = "1";
                                        }
                                        string leaveSubData = "";
                                        leaveSubData = LeaveUserId + "@" + leaveReasonId + "@" + fromDate + "@" + toDate + "@" + leaveLatitude + "@" + leaveLongitude
                                            + "@" + processName + "@" + mobileCaptureTime + "@" + leaveMobileReferenceNo + "@" + gpsSource + "@" + remarks
                                            + "@" + leaveFromSessionId + "@" + leaveToSessionId;
                                        if (leaveData != "")
                                        {
                                            leaveData = leaveData + "#" + leaveSubData;
                                        }
                                        else
                                        {
                                            leaveData = leaveSubData;
                                        }
                                        p++;
                                    }
                                    string dailytourPlanSubData = "";
                                    dailytourPlanSubData = dailyUserId + '$' + Date + '$' + Status + '$' + ActionById + '$' + ActionDate + '$' + dailyMobileReferenceNo + '$' + dailyMobileTransactionDate
                                                           + '$' + routePlanData + '$' + jointWorkPlanData + '$' + activityPlanData + '$' + leaveData;
                                    if(dailyTourPlanData != "")
                                    {
                                        dailyTourPlanData = dailyTourPlanData + '%' + dailytourPlanSubData;
                                    }
                                    else
                                    {
                                        dailyTourPlanData = dailytourPlanSubData;
                                    }
                                    l++;
                                }
                                DataTable dtrGPSForMonthlyTourPlan = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + monthlyTourPlanId + " and MobileReferenceNo ='" + mobileReferenceNo + "'");
                                if (dtrGPSForMonthlyTourPlan.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForMonthlyTourPlan.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForMonthlyTourPlan.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForMonthlyTourPlan.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForMonthlyTourPlan.Rows[0]["longitude"].ToString();
                                    source = dtrGPSForMonthlyTourPlan.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForMonthlyTourPlan.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForMonthlyTourPlan.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForMonthlyTourPlan.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForMonthlyTourPlan.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForMonthlyTourPlan.Rows[0]["NetworkProviderId"].ToString();
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    source = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }
                                string monthlyTourPlanSubData = "";
                                monthlyTourPlanSubData = userId + '&' + TourPlanDate + '&' + SubmissionRemark + '&' + SubmissionDate + '&' + 
                                    SubmittedBy + '&' + mobileReferenceNo + '&' + mobileTransactionDate + '&' + latitude + '&' +
                                    longitude + '&' + source + '&' + SignalStrength + '&' + NetworkProvider + '&' + dailyTourPlanData;
                                if (monthlyTourPlanData != "")
                                {
                                    monthlyTourPlanData = monthlyTourPlanData + '*' + monthlyTourPlanSubData;
                                }
                                else
                                {
                                    monthlyTourPlanData = monthlyTourPlanSubData;
                                }
                                k++;
                            }

                            dataArray[44] = monthlyTourPlanData;
                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");


                        }
                        else if (tableSchema.TableName == "ActivityDeviations")
                        {

                            int k = 0;
                            int activityDeviationId = 0;

                            string deviationData = "";

                            string userId = "", activityId="", mobileTransactionDate = "", deviationReasonId = "", 
                             mobileReferenceNo = "";
                           
                            DataTable dtrActivityDeviation = GetDataTable("select * from ActivityDeviations");

                            foreach (DataRow row in dtrActivityDeviation.Rows)
                            {
                                userId = dtrActivityDeviation.Rows[k]["UserId"].ToString();
                                activityId = dtrActivityDeviation.Rows[k]["ActivityId"].ToString();
                                deviationReasonId = dtrActivityDeviation.Rows[k]["DeviationId"].ToString();
                                mobileReferenceNo = dtrActivityDeviation.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrActivityDeviation.Rows[k]["Id"].ToString(), out activityDeviationId);
                                mobileTransactionDate = dtrActivityDeviation.Rows[k]["MobileTransactionDate"].ToString();

                                string deviationSubData = "";
                                deviationSubData = userId + "@" + activityId + "@" + deviationReasonId + "@" + mobileTransactionDate 
                                    + "@" + mobileReferenceNo;

                                if (deviationData != "")
                                {
                                    deviationData = deviationData + "#" + deviationSubData;
                                }
                                else
                                {
                                    deviationData = deviationSubData;
                                }
                                k++;
                            }

                            dataArray[45] = deviationData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }
                        else if (tableSchema.TableName == "ActivityInandOutLog")
                        {                            
                            int k = 0;

                            int checkInId = 0;
                            string checkInData = "";

                            string userId = "", checkIn="",checkOut="",  activityId = ""
                            ,   mobileReferenceNo = "",  mobileTransactionDate = "", activityPlannedDate = "";

                            DataTable dtrCheckInMaster = GetDataTable("select * from ActivityInandOutLog");


                            foreach (DataRow row in dtrCheckInMaster.Rows)
                            {                               
                                checkIn = dtrCheckInMaster.Rows[k]["CheckIn"].ToString();
                                checkOut = dtrCheckInMaster.Rows[k]["CheckOut"].ToString();
                                userId = dtrCheckInMaster.Rows[k]["UserId"].ToString();
                                mobileReferenceNo = dtrCheckInMaster.Rows[k]["MobileReferenceNo"].ToString();
                                activityId = dtrCheckInMaster.Rows[k]["ActivityId"].ToString();
                                mobileTransactionDate = dtrCheckInMaster.Rows[k]["MobileTransactionDate"].ToString();
                                activityPlannedDate = dtrCheckInMaster.Rows[k]["ActivityPlannedDate"].ToString();
                                int.TryParse(dtrCheckInMaster.Rows[k]["Id"].ToString(), out checkInId);

                                if (String.Compare(checkIn, "1/1/1900 12:00:00 AM") == 0)
                                {
                                    checkIn = "";                                    
                                }
                                else
                                {
                                    checkOut = "";                                   
                                }

                                string checkInSubData = "";
                                checkInSubData = userId + "@" + activityId + "@" + checkIn + "@" + checkOut + "@" + mobileReferenceNo + "@" + mobileTransactionDate + "@" + activityPlannedDate;

                                if (checkInData != "")
                                {
                                    checkInData = checkInData + "#" + checkInSubData;
                                }
                                else
                                {
                                    checkInData = checkInSubData;
                                }
                                k++;
                            }

                            dataArray[46] = checkInData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");

                        }

                        else if (tableSchema.TableName == "ActivityHeader")
                        {                                   
                            int k = 0;
                            int activityHeaderId = 0;


                            string activityData = "";

                            string userId = "", activityId = "",   latitude = ""
                            , longitude = "", processName = "", mobileTransactionDate = "", mobileReferenceNo = "",  gpsSource = "",
                             SignalStrength = "", NetworkProvider = "", configFieldIds="",configFieldValues ="";

                            processName = "btnMyActivity";
                            DataTable dtrMyActivity = GetDataTable("select * from ActivityHeader");

                            foreach (DataRow row in dtrMyActivity.Rows)
                            {                                
                                latitude = string.Empty;
                                longitude = string.Empty;                               
                                gpsSource = string.Empty;
                                configFieldIds = string.Empty;
                                configFieldValues = string.Empty;


                                activityId = dtrMyActivity.Rows[k]["ActivityId"].ToString();
                                mobileTransactionDate = dtrMyActivity.Rows[k]["MobileTransactionDate"].ToString();
                                userId = dtrMyActivity.Rows[k]["UserId"].ToString();
                                mobileReferenceNo = dtrMyActivity.Rows[k]["MobileReferenceNo"].ToString();
                                int.TryParse(dtrMyActivity.Rows[k]["Id"].ToString(), out activityHeaderId);
                                                             
                               
                                int k1 = 0;
                                int k2 = 0;


                                DataTable dtrActivityDetails = GetDataTable("select * from ActivityDetails WHERE ActivityHeaderId=" + activityHeaderId );
                                DataTable dtrGPSForActivity = GetDataTable("select latitude,longitude,Source,SignalStrength,NetworkProviderId from GPSLocationInfo WHERE  ProcessDetailsId=" + activityHeaderId + " and MobileReferenceNo='" + mobileReferenceNo + "'");

                                if (dtrGPSForActivity.Rows.Count > 0)
                                {
                                    latitude = dtrGPSForActivity.Rows[0]["latitude"].ToString() == "" ? "999" : dtrGPSForActivity.Rows[0]["latitude"].ToString();
                                    longitude = dtrGPSForActivity.Rows[0]["longitude"].ToString() == "" ? "999" : dtrGPSForActivity.Rows[0]["longitude"].ToString();
                                    gpsSource = dtrGPSForActivity.Rows[0]["Source"].ToString() == "" ? "1" : dtrGPSForActivity.Rows[0]["Source"].ToString();
                                    SignalStrength = dtrGPSForActivity.Rows[0]["SignalStrength"].ToString() == "" ? "1" : dtrGPSForActivity.Rows[0]["SignalStrength"].ToString();
                                    NetworkProvider = dtrGPSForActivity.Rows[0]["NetworkProviderId"].ToString() == "" ? "1" : dtrGPSForActivity.Rows[0]["NetworkProviderId"].ToString();                                   
                                }
                                else
                                {
                                    latitude = "999";
                                    longitude = "999";
                                    gpsSource = "1";
                                    SignalStrength = "0";
                                    NetworkProvider = "0";
                                }

                                foreach (DataRow rows in dtrActivityDetails.Rows)
                                {
                                    int activityDetailsId = 0;
                                    int.TryParse(dtrActivityDetails.Rows[k1]["Id"].ToString(), out activityDetailsId);
                                    string configFieldId = dtrActivityDetails.Rows[k1]["ConfigFieldId"].ToString();
                                    string value = dtrActivityDetails.Rows[k1]["Value"].ToString();                                   

                                    if (configFieldIds != "")
                                    {
                                        configFieldIds = configFieldIds + "," + configFieldId;
                                    }
                                    else
                                    {
                                        configFieldIds = configFieldId;
                                    }

                                    if (configFieldValues != "")
                                    {
                                        configFieldValues = configFieldValues + "," + value;
                                    }
                                    else
                                    {
                                        configFieldValues = value;
                                    }                                                                       
                                    
                                    k1++;
                                }

                                string activitySubData = "";
                                activitySubData = userId + "@" + activityId + "@" +
                                                latitude + "@" + longitude + "@" +
                                                 mobileTransactionDate + "@" + mobileReferenceNo + "@" + gpsSource + "@" + configFieldIds + "@" + configFieldValues
                                                + "@" + SignalStrength + "@" + NetworkProvider;

                                if (activityData != "")
                                {
                                    activityData = activityData + "#" + activitySubData;
                                }
                                else
                                {
                                    activityData = activitySubData;
                                }
                                k++;
                            }

                            dataArray[47] = activityData;

                            trn.Commit();
                            //_log.LogMessage("CopySQLiteDBRowsToSqlServer", "finished inserting all rows for table [" + tableSchema.TableName + "]", "L1");
                        }
                        else if (tableSchema.TableName == "SalesPromotionDetails" || tableSchema.TableName == "VW_QuotationDetails"
                            || tableSchema.TableName == "VW_TempOrderDetails" || tableSchema.TableName == "NewCustomerDetails"
                            || tableSchema.TableName == "StockEntryDetails" || tableSchema.TableName == "PaymentDetails"
                            || tableSchema.TableName == "MobileWorkingWithDetails" || tableSchema.TableName == "Temp_OrderHeader"
                            || tableSchema.TableName == "OrderSpecialInstruction" || tableSchema.TableName == "BankForOrder"
                            || tableSchema.TableName == "sqlite_sequence" || tableSchema.TableName == "VWSL_OrderDetails"
                            || tableSchema.TableName == "android_metadata" || tableSchema.TableName == "GPSLocationInfo"
                            || tableSchema.TableName == "PaymentDiscountDetails" || tableSchema.TableName == "Stock"
                            || tableSchema.TableName == "ProductDetailedDetails" || tableSchema.TableName == "SampleRequestDetails"
                            || tableSchema.TableName == "DoctorsContributionDetails" || tableSchema.TableName == "OrderDiscount"
                            || tableSchema.TableName == "Temp_OrderDiscounts" || tableSchema.TableName == "StockReconcile"
                            || tableSchema.TableName == "LocalStore" || tableSchema.TableName == "ReceivedStockDetails" || tableSchema.TableName == "StockAgingDetails"
                            || tableSchema.TableName == "ExpenseEntryDetails" || tableSchema.TableName == "ExpenseEntry" || tableSchema.TableName == "PunchInDetails"
                            || tableSchema.TableName == "PjpDetails" || tableSchema.TableName == "VW_Delivery" || tableSchema.TableName == "BTLDetailsConfig"
                            || tableSchema.TableName == "BTLActivityAttendees"
                            || tableSchema.TableName == "DailyTourPlans"
                            || tableSchema.TableName == "RoutePlans"
                            || tableSchema.TableName == "JointWorkPlans"
                            || tableSchema.TableName == "ActivityPlans"
                            || tableSchema.TableName == "ActivityDetails"
                            || tableSchema.TableName == "RemittanceDenominations"
                            )
                        {

                            trn.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        _log.LogMessage("unexpected exception", ex.ToString(), "L2");
                        //  _log.Error("unexpected exception", ex);
                        // trn.Rollback();
                        throw;
                    }
                    // break;
                    //Read all rows from the table specified and insert their value to sqlserver db
                }

                ExecuteNonQuery(sb.ToString());
            }

            //return Data;
            return dataArray;
        }                                        
        
        private SqlCommand BuildSQLServerInsertforGPS(SQLiteTableSchema newCustomerGPsSchema)
        {
            SqlCommand res = new SqlCommand();
            StringBuilder sb = new StringBuilder();
            sb.Append("Insert INTO GpsLocationInfo(Latitude,Longitude,ShopId,ProcessId,ProcessDetailsId,CapturedDate,UserId,Source,MobileTransactionDate,MobileReferenceNo,SyncDate,MobileSyncDate) VALUES (");
            int k = 0;
            List<string> pnames = new List<string>();
            k = 0;
            foreach (SQLiteColumnSchema column in newCustomerGPsSchema.Columns)
            {
                if (column.ColumnName == "Latitude" || column.ColumnName == "Longitude" || column.ColumnName == "ProcessId" || column.ColumnName == "ProcessDetailsId" || column.ColumnName == "CapturedDate" 
                    || column.ColumnName == "UserId" || column.ColumnName == "SyncDate" || column.ColumnName == "ShopId"
                    || column.ColumnName == "Source" || column.ColumnName == "MobileSyncDate" || column.ColumnName == "MobileTransactionDate" || column.ColumnName == "MobileReferenceNo")
                {
                    if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                    {
                        string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                        sb.Append(pname);
                        if (k < newCustomerGPsSchema.Columns.Count -1)
                        {
                            sb.Append(",");
                        }
                        DbType dbType = GetDbTypeOfColumn(column);
                        SqlParameter prm = new SqlParameter();
                        prm.ParameterName = pname;
                        prm.DbType = dbType;
                        prm.SourceColumnNullMapping = true;
                        prm.IsNullable = true;
                        prm.SourceColumn = column.ColumnName;
                        res.Parameters.Add(prm);

                        // Remember the parameter name in order to avoid duplicates
                        pnames.Add(pname);
                    }
                }

                k++;
            }
            sb.Append(")SELECT SCOPE_IDENTITY()");
            res.CommandText = sb.ToString();
            res.CommandType = CommandType.Text;
            return res;
        }

        /********************************************/
        private SqlCommand BuildSQLServerInsertforGPSWithoutShop(SQLiteTableSchema newCustomerGPsSchema)
        {
            SqlCommand res = new SqlCommand();
            StringBuilder sb = new StringBuilder();
            sb.Append("Insert INTO GpsLocationInfo(Latitude,Longitude,ProcessId,ProcessDetailsId,CapturedDate,UserId,Source, MobileTransactionDate,MobileReferenceNo,SyncDate,MobileSyncDate) VALUES (");
            int k = 0;
            List<string> pnames = new List<string>();
            k = 0;
            foreach (SQLiteColumnSchema column in newCustomerGPsSchema.Columns)
            {
                if (column.ColumnName == "Latitude" || column.ColumnName == "Longitude" || column.ColumnName == "ProcessId" || column.ColumnName == "ProcessDetailsId"
                    || column.ColumnName == "CapturedDate" || column.ColumnName == "UserId"
                    || column.ColumnName == "SyncDate" || column.ColumnName == "Source" || column.ColumnName == "MobileSyncDate" || column.ColumnName == "MobileTransactionDate" || column.ColumnName == "MobileReferenceNo")
                {
                    if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                    {
                        string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                        sb.Append(pname);
                        if (k < newCustomerGPsSchema.Columns.Count - 1)
                        {
                            sb.Append(",");
                        }
                        DbType dbType = GetDbTypeOfColumn(column);
                        SqlParameter prm = new SqlParameter();
                        prm.ParameterName = pname;
                        prm.DbType = dbType;
                        prm.SourceColumnNullMapping = true;
                        prm.IsNullable = true;
                        prm.SourceColumn = column.ColumnName;
                        res.Parameters.Add(prm);

                        // Remember the parameter name in order to avoid duplicates
                        pnames.Add(pname);
                    }
                }

                k++;
            }
            sb.Append(")SELECT SCOPE_IDENTITY()");
            res.CommandText = sb.ToString();
            res.CommandType = CommandType.Text;
            return res;
        }
        /**********************************************/

        private SQLiteTableSchema getCurrentSchema(string p, List<SQLiteTableSchema> syncTables)
        {
            foreach (SQLiteTableSchema tableSchema1 in syncTables)
            {
                //tableSchema1 = 

                if (tableSchema1.TableName == p)
                {
                    return tableSchema1;
                }

            }
            return null;
        }
        
        private SqlCommand BuildSQLServerInsert(SQLiteTableSchema tableSchema)
        {
            SqlCommand res = new SqlCommand();
            res.CommandTimeout = 10;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO " + tableSchema.TableName + "(");
            int k = 0;
            foreach (SQLiteColumnSchema column in tableSchema.Columns)
            {
                if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                {

                    if (k < tableSchema.Columns.Count - 1 )
                    {
                        sb.Append(column.ColumnName + ",");
                    }
                    else
                    {
                        sb.Append(column.ColumnName);
                    }
                }
                k++;
            }
            sb.Append(") VALUES( ");
            List<string> pnames = new List<string>();
            k = 0;
            foreach (SQLiteColumnSchema column in tableSchema.Columns)
            {
                if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                {
                    string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                    sb.Append(pname);
                    if (k < tableSchema.Columns.Count - 1)
                    {
                        sb.Append(",");
                    }
                    DbType dbType = GetDbTypeOfColumn(column);
                    SqlParameter prm = new SqlParameter();
                    prm.ParameterName = pname;
                    prm.DbType = dbType;
                    prm.SourceColumnNullMapping = true;
                    prm.IsNullable = true;
                    prm.SourceColumn = column.ColumnName;
                    res.Parameters.Add(prm);

                    // Remember the parameter name in order to avoid duplicates
                    pnames.Add(pname);
                }
                k++;
            }
            sb.Append(")SELECT SCOPE_IDENTITY()");
            res.CommandText = sb.ToString();
            res.CommandType = CommandType.Text;
            return res;
        }                


        private SqlCommand BuildSQLServerInsertForFollowup(SQLiteTableSchema tableSchema)
        {
            SqlCommand res = new SqlCommand();
            StringBuilder sb = new StringBuilder();
            sb.Append("Insert INTO FollowUpEntry(ShopId,UserId,FollowUpId,Remarks,FollowUpDate,IsCalendarSync,SyncDate,MobileSyncDate) VALUES (");
            int k = 0;
            List<string> pnames = new List<string>();
            k = 0;
            foreach (SQLiteColumnSchema column in tableSchema.Columns)
            {
                if (column.ColumnName == "ShopId" || column.ColumnName == "UserId" || column.ColumnName == "FollowUpId" || column.ColumnName == "Remarks" || column.ColumnName == "FollowUpDate" || column.ColumnName == "IsCalendarSync" || column.ColumnName == "SyncDate" || column.ColumnName == "MobileSyncDate")
                {
                    if (column.IsPrimaryKey == false && column.ColumnName != "IsSync")
                    {
                        string pname = "@" + GetNormalizedName(column.ColumnName, pnames);
                        sb.Append(pname);
                        if (k < tableSchema.Columns.Count - 3)
                        {
                            sb.Append(",");
                        }
                        DbType dbType = GetDbTypeOfColumn(column);
                        SqlParameter prm = new SqlParameter();
                        prm.ParameterName = pname;
                        prm.DbType = dbType;
                        prm.SourceColumnNullMapping = true;
                        prm.IsNullable = true;
                        prm.SourceColumn = column.ColumnName;
                        res.Parameters.Add(prm);

                        // Remember the parameter name in order to avoid duplicates
                        pnames.Add(pname);
                    }
                }

                k++;
            }
            sb.Append(")");
            res.CommandText = sb.ToString();
            res.CommandType = CommandType.Text;
            return res;

        }

        /// <summary>
        /// Creates SQLite connection string from the specified DB file path.
        /// </summary>
        /// <param name="sqlitePath">The path to the SQLite database file.</param>
        /// <returns>SQLite connection string</returns>
        private static string CreateSQLiteConnectionString(string sqlitePath)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = sqlitePath;
            //if (password != null)
            //    builder.Password = password;
            builder.PageSize = 4096;
            builder.UseUTF16Encoding = true;
            string connstring = builder.ConnectionString;

            return connstring;
        }

        /// <summary>
        /// Used in order to avoid breaking naming rules (e.g., when a table has
        /// a name in SQL Server that cannot be used as a basis for a matching index
        /// name in SQLite).
        /// </summary>
        /// <param name="str">The name to change if necessary</param>
        /// <param name="names">Used to avoid duplicate names</param>
        /// <returns>A normalized name</returns>
        private static string GetNormalizedName(string str, List<string> names)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsLetterOrDigit(str[i]) || str[i] == '_')
                    sb.Append(str[i]);
                else
                    sb.Append("_");
            } // for

            // Avoid returning duplicate name
            if (names.Contains(sb.ToString()))
                return GetNormalizedName(sb.ToString() + "_", names);
            else
                return sb.ToString();
        }

        /// <summary>
        /// Used in order to adjust the value received from  SQLite database  for the SQL Servr.
        /// </summary>
        /// <param name="val">The value object</param>
        /// <param name="columnSchema">The corresponding column schema</param>
        /// <returns>SQL server adjusted value.</returns>
        private static object CastValueForColumn(object val, SQLiteColumnSchema columnSchema)
        {

            DbType dt = GetDbTypeOfColumn(columnSchema);

            switch (dt)
            {
                case DbType.Int32:
                    if (val is short)
                        return (int)(short)val;
                    if (val is byte)
                        return (int)(byte)val;
                    if (val is long)
                        return (int)(long)val;
                    if (val is decimal)
                        return (int)(decimal)val;
                    break;

                case DbType.Int16:
                    if (val is int)
                        return (short)(int)val;
                    if (val is byte)
                        return (short)(byte)val;
                    if (val is long)
                        return (short)(long)val;
                    if (val is decimal)
                        return (short)(decimal)val;
                    break;

                case DbType.Int64:
                    if (val is int)
                        return (long)(int)val;
                    if (val is short)
                        return (long)(short)val;
                    if (val is byte)
                        return (long)(byte)val;
                    if (val is decimal)
                        return (long)(decimal)val;
                    break;

                case DbType.Single:
                    if (val is double)
                        return (float)(double)val;
                    if (val is decimal)
                        return (float)(decimal)val;
                    break;

                case DbType.Double:
                    if (val is float)
                        return (double)(float)val;
                    if (val is double)
                        return (double)val;
                    if (val is decimal)
                        return (double)(decimal)val;
                    break;

                case DbType.String:
                    if (val is Guid)
                        return ((Guid)val).ToString();
                    break;

                case DbType.Guid:
                    if (val is string)
                        return ParseStringAsGuid((string)val);
                    if (val is byte[])
                        return ParseBlobAsGuid((byte[])val);
                    break;

                case DbType.Binary:
                case DbType.Boolean:
                case DbType.DateTime:

                    break;

                default:
                    _log.LogMessage("CastValueForColumn()", "argument exception - illegal database type", "L2");
                    throw new ArgumentException("Illegal database type [" + Enum.GetName(typeof(DbType), dt) + "]");
            } // switch
            //if (val is DBNull)
            //return null;

            return val;
        }

        /// <summary>
        /// Matches SQL Server types to general DB types
        /// </summary>
        /// <param name="cs">The column schema to use for the match</param>
        /// <returns>The matched DB type</returns>
        private static DbType GetDbTypeOfColumn(SQLiteColumnSchema cs)
        {
            if (cs.ColumnType == "tinyint")
                return DbType.Byte;
            if (cs.ColumnType == "int")
                return DbType.Int32;
            if (cs.ColumnType == "smallint")
                return DbType.Int16;
            if (cs.ColumnType == "bigint")
                return DbType.Int64;
            if (cs.ColumnType == "bit")
                return DbType.Boolean;
            if (cs.ColumnType == "nvarchar" || cs.ColumnType == "varchar" ||
                cs.ColumnType == "text" || cs.ColumnType == "ntext")
                return DbType.String;
            if (cs.ColumnType == "float")
                return DbType.Double;
            if (cs.ColumnType == "real")
                return DbType.Single;
            if (cs.ColumnType == "blob")
                return DbType.Binary;
            if (cs.ColumnType == "numeric")
                return DbType.Double;
            if (cs.ColumnType == "timestamp" || cs.ColumnType == "datetime")
                return DbType.DateTime;
            if (cs.ColumnType == "nchar" || cs.ColumnType == "char")
                return DbType.String;
            if (cs.ColumnType == "uniqueidentifier" || cs.ColumnType == "guid")
                return DbType.Guid;
            if (cs.ColumnType == "xml")
                return DbType.String;
            if (cs.ColumnType == "sql_variant")
                return DbType.Object;
            if (cs.ColumnType == "integer")
                return DbType.Int64;

            _log.LogMessage("GetDbTypeOfColumn()", "illegal db type found", "L2");
            throw new ApplicationException("Illegal DB type found (" + cs.ColumnType + ")");
        }

        private static Guid ParseBlobAsGuid(byte[] blob)
        {
            byte[] data = blob;
            if (blob.Length > 16)
            {
                data = new byte[16];
                for (int i = 0; i < 16; i++)
                    data[i] = blob[i];
            }
            else if (blob.Length < 16)
            {
                data = new byte[16];
                for (int i = 0; i < blob.Length; i++)
                    data[i] = blob[i];
            }

            return new Guid(data);
        }

        private static Guid ParseStringAsGuid(string str)
        {
            try
            {
                return new Guid(str);
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            } // catch
        }

        #endregion
    }
}
