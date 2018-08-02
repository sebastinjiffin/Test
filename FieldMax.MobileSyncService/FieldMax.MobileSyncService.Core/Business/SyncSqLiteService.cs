using FieldMax.AsyncService.Core;
using FieldMax.MobileSyncService.Data;
using FieldMax.MobileSyncService.Data.BO;
using FieldMax.MobileSyncService.DbAccess;
using Infocean.DataAccessHelper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FieldMax.MobileSyncService.Core.Business
{
    public class SyncSqLiteService
    {
        #region property
        private Dictionary<string, string> query { get; set; }

        private string connectionString { get; set; }

        private string sqlConnString { get; set; }
        private string conString { get; set; }

        private Hashtable sigHashTable { get; set; }

        #endregion

        #region Public Method

        public void Process(SyncDbDetail detail) ////UploadSQLiteDBWithSyncDate
        {
            string data = detail.Data;
            string fileName = detail.FileName;
            string userCode = detail.UserCode;
            string mobileDate = detail.MobileDate;
            string IsPjpEnabled = detail.IsPjpEnabled;
            string mobileSyncDate = Convert.ToString(DateTime.Now);
            string statusCode = string.Empty;

            try
            {
                query = new Dictionary<string, string>();
                data = data.Replace(" ", "+");
                int len = data.Length;

                byte[] fileByteArray = Convert.FromBase64String(data);

                MemoryStream ms = new MemoryStream(fileByteArray);

                //create a folder in hosting environment
                string path = "";
                ////////string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Zip/" + userCode + "/");
                ////if (!Directory.Exists(path))
                ////{
                ////    Directory.CreateDirectory(path);
                ////}

                string str1 = userCode.Split('@')[0];
                string customerCode = str1;
                string userId = userCode.Split('@')[1]; ;
                var connectionString = GetConnectionString(customerCode);
                int sqliteCount = getSqliteCount();
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                DirectoryInfo[] directoryInfoInner = directoryInfo.GetDirectories().OrderBy(x => x.CreationTime).ToArray();
                if (directoryInfoInner.Length >= sqliteCount)
                {
                    ////string pathToDelete = System.Web.Hosting.HostingEnvironment.MapPath("~/Zip/" + userCode + "/" + directoryInfoInner[0].ToString() + "/");
                    ////Directory.Delete(pathToDelete, true);
                }
                String dateTime = "DB_" + DateTime.Now.ToString();
                dateTime = dateTime.Replace(" ", "_");
                dateTime = dateTime.Replace("/", "_");
                dateTime = dateTime.Replace(":", "_");
                ////path = System.Web.Hosting.HostingEnvironment.MapPath("~/Zip/" + userCode + "/" + dateTime + "/");
                ////Directory.CreateDirectory(path);
                FileStream fs = new FileStream(path + fileName, FileMode.Create);
                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                fs.Dispose();
                SQLiteToDB sQLiteToDB = new SQLiteToDB();
                sQLiteToDB.UnzipFile(path, fileName);
                string ConnectionString = GetConnectionString(customerCode);//"";
                query = GetQueries(customerCode);
                fileName = "FMTransaction.db";


                //********************************oder sync only*******************//
                string[] sqliteData = new string[10];
                string tableQuery = "select name from sqlite_master  where type='table' and sql LIKE '%IsSync%'";
                sqliteData = sQLiteToDB.CopySQLiteDBRowsToSqlServer(ConnectionString, path + fileName, query, mobileSyncDate, tableQuery, mobileDate);
                string OrderData = sqliteData[0];
                string quotationData = sqliteData[1];
                string salesPromotionData = sqliteData[2];
                string stockData = sqliteData[3];
                string stockRequestData = sqliteData[4];
                string photoCaptureData = sqliteData[5];
                string ShoplocationData = sqliteData[9];
                string StockReconcileData = sqliteData[10];
                string paymentData = sqliteData[11];
                string enquiryData = sqliteData[12];
                string complaintData = sqliteData[13];
                string salesReturnData = sqliteData[14];
                string parameterCaptureData = sqliteData[15];
                string expenseData = sqliteData[16];
                string punchData = sqliteData[17];
                string shopInData = sqliteData[18];
                string promoData = sqliteData[19];
                string sigData = sqliteData[20];
                string leaveData = sqliteData[21];
                string feedbackData = sqliteData[22];
                string workingAreaData = sqliteData[23];
                string workingWithData = sqliteData[24];
                string popData = sqliteData[25];
                string remittanceData = sqliteData[26];
                string receiptNo = sqliteData[27];
                string newCustomerData = sqliteData[32];
                string stockAgingData = sqliteData[29];
                string todaysPlanData = sqliteData[34];
                string deviationData = sqliteData[35];

                string AssetRequestData = sqliteData[36];
                string pjpData = sqliteData[37];
                string shopwiseDefaultDistributorData = sqliteData[38];
                string editCustomerData = sqliteData[39];
                string transactionResultData = sqliteData[40];
                string loadedStockData = sqliteData[41];
                string deliveryData = sqliteData[42];
                string btlActivityData = sqliteData[43];
                string TourPlanData = sqliteData[44];



                string activityDeviationData = sqliteData[45];
                string activityLogData = sqliteData[46];
                string myActivityData = sqliteData[47];

                string JointWorkingLoginData = sqliteData[48];
                string JointWorkingData = sqliteData[49];
                string CollectionSettlementData = sqliteData[50];

                if (!string.IsNullOrEmpty(workingWithData))
                {
                    saveWorkingWithData(workingWithData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(newCustomerData))
                {
                    NewCustomerSave(newCustomerData, userCode, mobileSyncDate);
                }
                if (!string.IsNullOrEmpty(todaysPlanData))
                {
                    TodaysPlanSave(todaysPlanData, userCode, mobileSyncDate);
                }
                if (!string.IsNullOrEmpty(editCustomerData))
                {
                    EditCustomerDataSave(editCustomerData, userCode, mobileSyncDate);
                }
                ///* asset request */
                if (!string.IsNullOrEmpty(AssetRequestData))
                {
                    AssetRequestSave(AssetRequestData, userCode, mobileSyncDate);
                }
                ///* asset request */
                if (!string.IsNullOrEmpty(stockAgingData))
                {
                    StockAgingSave(stockAgingData, userCode, mobileSyncDate);
                }
                if (!string.IsNullOrEmpty(OrderData))
                {
                    OrderSave(OrderData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(paymentData))
                {
                    savePaymentData(paymentData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(enquiryData))
                {
                    saveEnquiryData(enquiryData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(quotationData))
                {
                    quotationSave(quotationData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(salesPromotionData))
                {
                    salesPromotionSave(salesPromotionData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(stockData))
                {
                    saveStock(stockData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(stockRequestData))
                {
                    saveStockRequest(stockRequestData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(photoCaptureData))
                {
                    savePhotoCapture(photoCaptureData, userCode, mobileSyncDate);
                }
                if (!string.IsNullOrEmpty(ShoplocationData))
                {
                    saveShoplocationData(ShoplocationData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(StockReconcileData))
                {
                    saveStockReconcileData(StockReconcileData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(complaintData))
                {
                    saveComplaintData(complaintData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(salesReturnData))
                {
                    saveSalesReturnData(salesReturnData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(parameterCaptureData))
                {
                    saveParameterCaptureData(parameterCaptureData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(expenseData))
                {
                    saveExpenseData(expenseData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(punchData))
                {
                    savePunchData(punchData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(shopInData))
                {
                    saveShopInData(shopInData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(promoData))
                {
                    savePromoData(promoData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(sigData))
                {
                    saveSignatureData(sigData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(leaveData))
                {
                    saveLeaveData(leaveData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(feedbackData))
                {
                    saveFeedbackData(feedbackData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(workingAreaData))
                {
                    saveWorkingAreaData(workingAreaData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(popData))
                {
                    savePopData(popData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(remittanceData))
                {
                    saveRemittanceData(remittanceData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(receiptNo))
                {
                    UpdateReceiptNo(receiptNo, userCode);
                }
                if (!string.IsNullOrEmpty(deviationData))
                {
                    saveDeviationData(deviationData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(pjpData))
                {
                    savePjpdata(pjpData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(shopwiseDefaultDistributorData))
                {
                    saveShopDefaultDistributorData(shopwiseDefaultDistributorData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(transactionResultData))
                {
                    savetransactionResultData(transactionResultData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(loadedStockData))
                {
                    saveStockData(loadedStockData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(deliveryData))
                {
                    SaveDeliveryData(deliveryData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(btlActivityData))
                {
                    SaveBTLActivityData(btlActivityData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(TourPlanData))
                {
                    saveTourPlanData(TourPlanData, userCode, mobileSyncDate, mobileDate);
                }

                if (!string.IsNullOrEmpty(JointWorkingLoginData))
                {
                    saveJointWorkingLoginData(JointWorkingLoginData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(JointWorkingData))
                {
                    saveJointWorkingData(JointWorkingData, userCode, mobileSyncDate, mobileDate);
                }


                if (!string.IsNullOrEmpty(activityDeviationData))
                {
                    saveActivityDeviationData(activityDeviationData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(activityLogData))
                {
                    saveActivityLogData(activityLogData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(myActivityData))
                {
                    saveMyActivityData(myActivityData, userCode, mobileSyncDate, mobileDate);
                }
                if (!string.IsNullOrEmpty(CollectionSettlementData))
                {
                    saveCollectionSettlementData(CollectionSettlementData, userCode, mobileSyncDate, mobileDate);
                }

                GetSqliteDatabaseSplitAllDatabases(userCode, customerCode, "PRODUCT_TRANSACTION_DB", true, mobileDate, "0");
                GetSqliteDatabaseSplitAllDatabases(userCode, customerCode, "CHEQUE_REMITTANCE_DB", true, mobileDate, "0");
                if (IsPjpEnabled == "1")
                {
                    GetSqliteDatabaseSplitAllDatabases(userCode, customerCode, "PJPMASTER_DB", true, mobileDate, "0");
                }


                MobileControlsBO mobileControlsBO = new MobileControlsBO();
                mobileControlsBO.ConString = ConnectionString;
                int status = mobileControlsBO.isExpenseTransactionNeeded();

                if (status == 1)
                {
                    ExpenseBO expenseBO = new ExpenseBO();
                    expenseBO.ConString = ConnectionString;
                    expenseBO.UserId = Convert.ToInt32(userId);
                    expenseBO.InsertTransactionData();
                    GetSqliteDatabaseSplitAllDatabases(userCode, customerCode, "EXPENSE_TRANSACTION_DB", true, mobileDate, "0");
                }

                statusCode = Status.Success.ToString();
            }
            catch (IndexOutOfRangeException ex)
            {
                statusCode = Status.IndexOutOfRange.ToString();
                ////////////_log.LogMessage("UploadSQLiteDB()", ex.ToString(), "L2");
            }
            catch (NullReferenceException ex)
            {
                statusCode = Status.NullReferenceException.ToString();
                ////////////_log.LogMessage("UploadSQLiteDB()", ex.ToString(), "L2");
            }
            catch (Exception ex)
            {
                statusCode = ex.ToString();
                ////////////_log.LogMessage("UploadSQLiteDB()", ex.ToString(), "L2");
            }
            finally
            {
                ////////////Context.Response.Write(statusCode);
                ////////////Context.Response.End();
            }
        }

        #endregion

        #region Private Method

        private string GetConnectionString(string customerCode)
        {
            try
            {
                string conString = Status.CustomerDBNotAvailable.ToString();

                //if (ConfigurationManager.ConnectionStrings["Connectionstring_" + customerCode] != null)
                //{
                //    conString = ConfigurationManager.ConnectionStrings["Connectionstring_" + customerCode].ToString();
                //}
                return conString;

            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("GetConnectionString(string constrVal)", ex.ToString(), "L2");
                ////////Context.Response.Write(Status.Error.ToString());
                throw ex;
            }
        }

        private int getSqliteCount()
        {
            ParameterSettingsBO dBCredentialBO = new ParameterSettingsBO();
            dBCredentialBO.ConString = connectionString;
            //dBCredentialBO.UserID = userIdVal;
            return dBCredentialBO.getSqliteCount();
        }

        private Dictionary<string, string> GetQueries(string CustomerCode)
        {
            ////XDocument xmlDoc = XDocument.Load(Server.MapPath("~/Resources/InlineQuery.xml"));
            ////foreach (XElement node in xmlDoc.Descendants("Root").Elements())
            ////{
            ////    string tablename = node.Attribute("tablename").Value;
            ////    string columnname = node.Attribute("columnname").Value;
            ////    string IsSync = node.Attribute("IsSync").Value;
            ////    string newcol = node.Attribute("newcol").Value;
            ////    string Query = node.Attribute("Query").Value;

            ////    string[] strArr = null;
            ////    strArr = newcol.Split(':');
            ////    if (newcol != "" && strArr[5] == "custCodeValue")
            ////    {
            ////        strArr[5] = strArr[5].Replace("custCodeValue", CustomerCode);
            ////        newcol = strArr[0] + ':' + strArr[1] + ':' + strArr[2] + ':' + strArr[3] + ':' + strArr[4] + ':' + strArr[5];
            ////    }
            ////    {
            ////        query.Add(tablename + '@' + columnname + '@' + IsSync + '@' + newcol, Query);
            ////    }
            ////}
            return query;
        }

        #region Upload SQLite DB

        private void saveCollectionSettlementData(string CollectionSettlementData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string PaymentId = "", InstrumentNo = "", CashAmount = "", RemittedBy = "", MobilereferenceNo = "",
                                MobileTransactionDate = "", latitude = "", longitude = "",
                                source = "";
                string[] MasterData = CollectionSettlementData.Split('#');

                //StockBO stockBO = new StockBO();
                //connectionString = GetConnectionString(userCode.Split('@')[0]);
                //stockBO.ConString = connectionString;
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');
                    PaymentId = SubData[0];
                    InstrumentNo = SubData[1];
                    CashAmount = SubData[2];
                    RemittedBy = SubData[3];
                    MobilereferenceNo = SubData[4];
                    MobileTransactionDate = SubData[5];
                    latitude = SubData[6];
                    longitude = SubData[7];
                    source = SubData[8];


                    UpdateCollectionSettlementSync(PaymentId, InstrumentNo, CashAmount, RemittedBy, MobilereferenceNo, MobileTransactionDate,
                        userCode, latitude, longitude, source, mobileSyncDate, mobileDate);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateCollectionSettlementSync(string PaymentId, string InstrumentNo, string CashAmount,
            string RemittedBy, string MobilereferenceNo, string MobileTransactionDate, string userCode
            , string latitude, string longitude, string source, string serverSyncDate, string mobileSyncDate)
        {
            try
            {

                CashSettlementBO cashSettlementBO = new CashSettlementBO();
                cashSettlementBO.ConString = GetConnectionString(userCode.Split('@')[0]);

                cashSettlementBO.UserId = userCode.Split('@')[1];
                cashSettlementBO.PaymentHeaderId = PaymentId;
                cashSettlementBO.Mode = "I";
                cashSettlementBO.InstrumentNo = InstrumentNo;
                cashSettlementBO.Cash = CashAmount;
                cashSettlementBO.RemittedBy = RemittedBy; // DLO USERID
                cashSettlementBO.MobileReferenceNo = MobilereferenceNo;
                cashSettlementBO.MobileTransactionDate = MobileTransactionDate;
                cashSettlementBO.Latitude = latitude;
                cashSettlementBO.Longitude = longitude;
                cashSettlementBO.gpsSource = source;
                cashSettlementBO.ServerSyncDate = serverSyncDate;
                cashSettlementBO.MobileSyncDate = mobileSyncDate;
                cashSettlementBO.UpdateCashSettlementUser();


            }
            catch (IndexOutOfRangeException ex)
            {
                ////////_log.LogMessage("UpdateCollectionSettlementDataSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.IndexOutOfRange.ToString());
            }
            catch (NullReferenceException ex)
            {
                ////////_log.LogMessage("UpdateCollectionSettlementDataSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.NullReferenceException.ToString());
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("UpdateCollectionSettlementDataSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.Error.ToString());
            }
        }

        private void saveStockData(string loadedStockData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string userId = "", shopId = "", unitSet = "", productData = "", stockdata = "",
                                stockHeaderId = "", mode = "";
                string[] MasterData = loadedStockData.Split('#');

                StockBO stockBO = new StockBO();
                connectionString = GetConnectionString(userCode.Split('@')[0]);
                stockBO.ConString = connectionString;
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');
                    userId = userCode;
                    stockHeaderId = SubData[1];
                    productData = SubData[2];
                    stockdata = SubData[3];
                    unitSet = SubData[4];
                    UpdateLoadedStockSync(userId, stockHeaderId, productData, stockdata, unitSet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void UpdateLoadedStockSync(string userId, string stockHeaderId, string productData, string stockdata, string unitSet)
        {
            try
            {
                if (userId != "" && stockHeaderId != "") // Must fields are checked here.
                {
                    StockBO stockBO = new StockBO();
                    stockBO.ConString = GetConnectionString(userId.Split('@')[0]);
                    string userIdInt = userId.Split('@')[1];
                    stockBO.UserId = Convert.ToInt32(userIdInt);
                    stockBO.StockHeaderId = Convert.ToInt32(stockHeaderId);
                    stockBO.Mode = "4";
                    stockBO.ProductData = productData;
                    stockBO.UnitData = unitSet;
                    stockBO.StockData = stockdata;
                    stockBO.UpdateLoadedStock();
                }
                else
                {
                    ////////////Context.Response.Output.Write(Status.AttributesNull.ToString());
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                ////////_log.LogMessage("UpdateLoadedStockSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.IndexOutOfRange.ToString());
            }
            catch (NullReferenceException ex)
            {
                ////////_log.LogMessage("UpdateLoadedStockSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.NullReferenceException.ToString());
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("UpdateLoadedStockSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.Error.ToString());
            }
        }

        private void saveShopDefaultDistributorData(string shopwiseDefaultDistributorData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] shopdistributorData = shopwiseDefaultDistributorData.Split('@');

                string shopIds = shopdistributorData[0];
                string distributorIds = shopdistributorData[1];

                if (!string.IsNullOrEmpty(shopIds) && !string.IsNullOrEmpty(distributorIds))
                {
                    ShopBO shopBO = new ShopBO();
                    shopBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                    shopBO.ShopIds = shopIds;
                    shopBO.DistributorIds = distributorIds;
                    shopBO.UserId = userCode.Split('@')[1];
                    shopBO.UpdateShopDefaultDistributor();

                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveShopDefaultDistributorData()", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void savePjpdata(string pjpData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] pjpSubData = pjpData.Split('~');

                string userId = pjpSubData[0];
                string dates = pjpSubData[1];
                string remarks = pjpSubData[2];
                string status = pjpSubData[3];
                string mobileTransactionDate = pjpSubData[4];
                string beatPlanIds = pjpSubData[5];
                string commonRemarks = pjpSubData[6];


                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(beatPlanIds) && !string.IsNullOrEmpty(dates)) // Must fields are checked here.
                {
                    PjpBO pjpBO = new PjpBO();
                    pjpBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                    pjpBO.UserIdList = userId;
                    pjpBO.Routes = beatPlanIds;
                    pjpBO.Date = dates;
                    pjpBO.StatusList = status;
                    pjpBO.MobileTransactionDate = mobileTransactionDate;
                    pjpBO.Remarks = remarks;
                    pjpBO.Mode = "2";
                    pjpBO.MobileSyncDate = mobileSyncDate;
                    pjpBO.CommonRemarks = commonRemarks;
                    pjpBO.updatePjp();


                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("savePjpdata()", ex.ToString(), "L2");
                throw ex;
            }
            //UpdatePjpSync(userCode, userId, dates, remarks, status, mobiletransactionDate,beatPlanIds, mobileSyncDate, mobileDate);

        }

        private void saveDeviationData(string deviationData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = deviationData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');
                string userId = SubData[0];
                string beatPlanId = SubData[1];
                string deviationReasonId = SubData[2];
                string mobileReferenceNo = SubData[7];
                string mobileTransactionDate = SubData[3];
                string latitude = SubData[4];
                string longitude = SubData[5];
                string processName = SubData[6];
                string gpsSource = SubData[8];
                string signalStrength = SubData[9];
                UpdateBeatPlanDeviationSync(userCode, userId, beatPlanId, deviationReasonId, latitude, longitude, processName,
                        mobileTransactionDate, mobileReferenceNo, gpsSource, signalStrength, mobileSyncDate, mobileDate);
            }
        }

        private void UpdateBeatPlanDeviationSync(string userCode, string userId, string beatPlanId, string deviationReasonId, string latitude, string longitude
            , string processName, string mobileTransactionDate, string mobileReferenceNo, string gpsSource, string signalStrength, string mobileSyncDate
            , string mobileDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(beatPlanId)) // Must fields are checked here.
                {
                    BeatPlanDeviationBO beatPlanDeviationBO = new BeatPlanDeviationBO();

                    beatPlanDeviationBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                    beatPlanDeviationBO.UserId = Convert.ToInt32(userId);
                    beatPlanDeviationBO.BeatPlanId = Convert.ToInt32(beatPlanId);
                    beatPlanDeviationBO.MobileTransactionDate = mobileTransactionDate;
                    beatPlanDeviationBO.Latitude = latitude;
                    beatPlanDeviationBO.Longitude = longitude;
                    beatPlanDeviationBO.source = gpsSource;
                    beatPlanDeviationBO.MobileReferenceNo = mobileReferenceNo;
                    beatPlanDeviationBO.DeviationReasonId = Convert.ToInt32(deviationReasonId);
                    beatPlanDeviationBO.signalStrength = signalStrength;
                    beatPlanDeviationBO.SyncDate = Convert.ToString(DateTime.Now);
                    beatPlanDeviationBO.UpdateBeatPlanDeviation();
                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("UpdateBeatPlanDeviationSync()", ex.ToString(), "L2");
                throw ex;
            }

        }

        private void saveRemittanceData(string remittanceData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = remittanceData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');
                string amount = SubData[1];
                string bankId = SubData[2];
                string latitude = SubData[3];
                string longitude = SubData[4];
                string processName = SubData[5];
                string mobileTransactionDate = SubData[6];
                string mobileReferenceNo = SubData[7];
                string gpsSource = SubData[8];
                string denominationIds = SubData[9];
                string denominationCounts = SubData[10];
                string approvedBy = SubData[11];
                string remarks = SubData[12];

                UpdateRemittanceSync(userCode, amount, bankId, latitude, longitude, processName,
                        mobileTransactionDate, mobileReferenceNo, gpsSource, mobileSyncDate, mobileDate, denominationIds, denominationCounts, approvedBy, remarks);
            }
        }

        private void UpdateReceiptNo(string ReceiptNo, string userCode)
        {

            string[] receiptno = ReceiptNo.Split(',');
            if (receiptno.Length > 1)
            {
                int userId = Convert.ToInt32(receiptno[0].ToString().Trim());
                string receiptNo = (receiptno[1].ToString().Trim());
                ReceiptNoBO receiptNoBO = new ReceiptNoBO();
                receiptNoBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                receiptNoBO.UserId = userId;
                receiptNoBO.ReceiptNo = receiptNo;
                receiptNoBO.Flag = 1;
                receiptNoBO.UpdateReceiptNo();

            }
        }

        private void savePopData(string popData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = popData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');
                string shopId = SubData[0];
                string idSet = SubData[1];
                string quantitySet = SubData[2];
                string latitude = SubData[3];
                string longitude = SubData[4];
                string processName = SubData[5];
                string mobileReferenceNo = SubData[6];
                string mobileTransactionDate = SubData[7];
                string gpsSource = SubData[8];
                string remarksSet = SubData[9];

                updatePOPSync(userCode, idSet, quantitySet, processName, latitude, longitude, shopId,
                                     mobileTransactionDate, mobileReferenceNo, remarksSet, mobileSyncDate, mobileDate);
            }
        }

        private void saveWorkingWithData(string workingWithData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = workingWithData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');
                string shopId = SubData[1];
                string userIdSet = SubData[2];
                string latitude = SubData[3];
                string longitude = SubData[4];
                string processName = SubData[5];
                string mobileTransactionDate = SubData[6];
                string mobileReferenceNo = SubData[7];
                string gpsSource = SubData[8];
                string signalStrength = SubData[9];
                string networkProvider = SubData[10];
                string Others = SubData[11];
                string DepartmentIdSet = SubData[12];

                updateWorkingWithSync(userCode, shopId, userIdSet, processName, latitude, longitude,
                                     mobileTransactionDate, mobileReferenceNo, gpsSource, mobileSyncDate, mobileDate, signalStrength, networkProvider, Others, DepartmentIdSet);
            }
        }

        private void saveWorkingAreaData(string workingAreaData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = workingAreaData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');

                string mobileCaptureTime = SubData[0];
                string workingAreaMasterId = SubData[1];
                string latitude = SubData[2];
                string longitude = SubData[3];
                string processName = SubData[4];
                string mobileReferenceNo = SubData[5];
                string gpsSource = SubData[6];
                string signalStrength = SubData[7];
                string networkProvider = SubData[8];



                updateWorkingAreaSync(userCode, mobileCaptureTime, workingAreaMasterId, latitude, longitude, processName,
                                     mobileReferenceNo, gpsSource, mobileSyncDate, mobileDate, signalStrength, networkProvider);
            }
        }

        private void saveFeedbackData(string feedbackData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = feedbackData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');

                string mobileCaptureTime = SubData[1];
                string ansIdSet = SubData[2];
                string ansSet = SubData[3];
                string latitude = SubData[4];
                string longitude = SubData[5];
                string processName = SubData[6];
                string mobileReferenceNo = SubData[7];
                string gpsSource = SubData[8];

                updateMobileFeedbackSync(userCode, mobileCaptureTime, ansIdSet, ansSet, latitude, longitude, processName,
                                     mobileReferenceNo, gpsSource, mobileSyncDate, mobileDate);
            }
        }

        private void saveLeaveData(string leaveData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = leaveData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');

                string leaveReasonId = SubData[1];
                string fromDate = SubData[2];
                string toDate = SubData[3];
                string latitude = SubData[4];
                string longitude = SubData[5];
                string processName = SubData[6];
                string mobileCaptureTime = SubData[7];
                string mobileReferenceNo = SubData[8];
                string gpsSource = SubData[9];
                string remarks = SubData[10];
                string LeaveFromSessionID = SubData[11];
                string LeaveToSessionID = SubData[12];

                updateLeaveSync(userCode, leaveReasonId, fromDate, toDate, latitude, longitude, processName,
                                     mobileCaptureTime, mobileReferenceNo, gpsSource, mobileSyncDate, mobileDate, remarks, LeaveFromSessionID, LeaveToSessionID);
            }
        }

        private void saveShopInData(string shopInData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = shopInData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');

                string shopIn = SubData[0];
                string shopOut = SubData[1];
                string latitude = SubData[2];
                string longitude = SubData[3];
                string processName = SubData[4];
                string mobileReferenceNo = SubData[5];
                string gpsSource = SubData[6];
                string shopId = SubData[7];
                string mobileTransactionDate = SubData[8];
                string signalStrength = SubData[9];
                string networkProvider = SubData[10];
                string IsGpsForciblyEnabled = SubData[11];

                UpdateShopInAndOutSync(userCode, shopId, shopIn, shopOut, latitude, longitude, processName,
                                    gpsSource, mobileReferenceNo, mobileTransactionDate, mobileSyncDate, mobileDate, signalStrength, networkProvider, IsGpsForciblyEnabled);
            }
        }

        private void saveSignatureData(string sigData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = sigData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');

                string shopId = SubData[1];
                string data = SubData[2];
                string shopKeeperName = SubData[3];
                string latitude = SubData[4];
                string longitude = SubData[5];
                string processName = SubData[6];
                string mobileReferenceNo = SubData[7];
                string gpsSource = SubData[8];
                string processId = SubData[9];

                UpdateSignatureSync(userCode, shopId, data, shopKeeperName, latitude, longitude, processName,
                                    gpsSource, mobileReferenceNo, processId, mobileSyncDate, mobileDate);
            }
        }

        private void savePromoData(string promoData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = promoData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');

                string shopId = SubData[1];
                string qn = SubData[2];
                string ans = SubData[3];
                string freeText = SubData[4];
                string dateSet = SubData[5];
                string textSet = SubData[6];
                string numberSet = SubData[7];
                string imageSet = SubData[8];
                string latitude = SubData[9];
                string longitude = SubData[10];
                string processName = SubData[11];
                string mobileTransationDate = SubData[12];
                string mobileReferenceNo = SubData[13];
                string gpsSource = SubData[14];
                string signalStrength = SubData[15];
                string networkProvider = SubData[16];

                UpdatePromotionalActivitiesSync(userCode, shopId, qn, ans, freeText, dateSet, textSet, numberSet, imageSet, latitude, longitude, processName, mobileTransationDate, mobileReferenceNo, gpsSource, mobileSyncDate, mobileDate, signalStrength, networkProvider);
            }
        }

        private void savetransactionResultData(string transactionResultData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = transactionResultData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');

                string shopId = SubData[1];
                string qn = SubData[2];
                string ans = SubData[3];
                string freeText = SubData[4];
                string dateSet = SubData[5];
                string textSet = SubData[6];
                string numberSet = SubData[7];
                string imageSet = SubData[8];
                string latitude = SubData[9];
                string longitude = SubData[10];
                string processName = SubData[11];
                string mobileTransationDate = SubData[12];
                string mobileReferenceNo = SubData[13];
                string gpsSource = SubData[14];
                string signalStrength = SubData[15];
                string networkProvider = SubData[16];

                UpdateTransactionResultsSync(userCode, shopId, qn, ans, freeText, dateSet, textSet, numberSet, imageSet, latitude, longitude, processName, mobileTransationDate, mobileReferenceNo, gpsSource, mobileSyncDate, mobileDate, signalStrength, networkProvider);
            }
        }

        private void savePunchData(string punchData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = punchData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');
                string punchInTime = SubData[0];
                string punchOutTime = SubData[1];
                string startReading = SubData[2];
                string endReading = SubData[3];
                string latitude = SubData[4];
                string longitude = SubData[5];
                string processName = SubData[6];
                string mobileTransactionDate = SubData[7];
                string mobileReferenceNo = SubData[8];
                string gpsSource = SubData[9];
                string travelMode = SubData[10];
                string travelModeAnswer = SubData[11];

                PunchSync(userCode, punchInTime, punchOutTime, startReading, endReading,
                                    latitude, longitude, processName, mobileReferenceNo,
                                    gpsSource, mobileSyncDate, mobileDate, travelMode, travelModeAnswer);
            }
        }

        private void saveExpenseData(string expenseData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = expenseData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');
                string expenseIdSet = SubData[1];
                string amountSet = SubData[2];
                string remarks = SubData[3];
                string latitude = SubData[4];
                string longitude = SubData[5];
                string processName = SubData[6];
                string mobileTransactionDate = SubData[7];
                string mobileReferenceNo = SubData[8];
                string gpsSource = SubData[9];
                string expenseDate = SubData[10];
                string field1 = SubData[11];
                string field2 = SubData[12];
                string field3 = SubData[13];
                string field4 = SubData[14];
                string field5 = SubData[15];
                string uniqueKey = SubData[16];


                UpdateExpenseSync(userCode, expenseIdSet, amountSet, remarks,
                                    latitude, longitude, processName, mobileTransactionDate, mobileReferenceNo,
                                    gpsSource, mobileSyncDate, mobileDate, expenseDate, field1, field2, field3, field4, field5, uniqueKey);
            }
        }

        private void saveParameterCaptureData(string parameterCaptureData, string userCode, string mobileSyncDate, string mobileDate)
        {

            string[] MasterData = parameterCaptureData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');
                string shopId = SubData[0];
                string productId = SubData[1];
                string quantity = SubData[2];
                string parameters = SubData[3];
                string latitude = SubData[4];
                string longitude = SubData[5];
                string processName = SubData[6];
                string mobileTransactionDate = SubData[7];
                string mobileReferenceNo = SubData[8];
                string gpsSource = SubData[9];
                string signalStrength = SubData[10];
                string networkProvider = SubData[11];

                UpdateParametersSync(userCode, shopId, productId, quantity, parameters,
                                    latitude, longitude, processName, mobileTransactionDate, mobileReferenceNo,
                                    gpsSource, mobileSyncDate, mobileDate, signalStrength, networkProvider);
            }

        }

        private void saveSalesReturnData(string salesReturnData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] MasterData = salesReturnData.Split('#');
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('@');
                    string shopId = SubData[0];
                    string productAttributeId = SubData[1];
                    string quantity = SubData[2];
                    string price = SubData[3];
                    string batchNo = SubData[4];
                    string packedDate = SubData[5];
                    string latitude = SubData[6];
                    string longitude = SubData[7];
                    string processName = SubData[8];
                    string mobileTransactionDate = SubData[9];
                    string mobileReferenceNo = SubData[10];
                    string rate = SubData[11];
                    string reason = SubData[12];
                    string gpsSource = SubData[13];
                    string unitData = SubData[14];
                    string recieptNo = SubData[15];
                    string schemeId = SubData[16];
                    string signalStrength = SubData[17];
                    string networkProvider = SubData[18];
                    string remark = SubData[19];

                    UpdateSalesReturnDataSync(userCode, shopId, productAttributeId, quantity, price, batchNo, packedDate,
                                        latitude, longitude, processName, mobileTransactionDate, mobileReferenceNo,
                                        rate, reason, gpsSource, mobileSyncDate, mobileDate, unitData, recieptNo, schemeId, signalStrength, networkProvider, remark);
                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveEnquiryData()", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void saveComplaintData(string complaintData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] MasterData = complaintData.Split('#');
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('@');
                    string reportedBy = SubData[0];
                    string complaintId = SubData[1];
                    string shopid = SubData[2];
                    string remarks = SubData[3];
                    string latitude = SubData[4];
                    string longitude = SubData[5];
                    string processName = SubData[6];
                    string mobileTransactionDate = SubData[7];
                    string mobileReferenceNo = SubData[8];
                    string gpsSource = SubData[9];
                    string signalStrength = SubData[10];
                    string networkProvider = SubData[11];

                    UpdateComplaintSync(userCode, shopid, remarks, complaintId, latitude, longitude, processName,
                                         mobileTransactionDate, mobileReferenceNo, gpsSource, mobileSyncDate, mobileDate, signalStrength, networkProvider);
                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveEnquiryData()", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void saveJointWorkingLoginData(string JointWorkingLoginData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string[] MasterData = JointWorkingLoginData.Split('#');
            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('~');

                string UserID = SubData[0];
                string ShopInId = SubData[1];
                string ShopInTime = SubData[2];
                string ShopOutId = SubData[3];
                string ShopoutTime = SubData[4];
                string JointWorkUserId = SubData[5];
                string MobileTransactionDate = SubData[6];
                string MobileReferenceNo = SubData[7];
                string BeatPlanId = SubData[8];
                string ShopName = SubData[9];
                string BeatPlanDeviationReasonId = SubData[10];

                UpdateJointWorkingLoginDataSync(userCode, ShopInId, ShopInTime, ShopOutId, ShopoutTime, JointWorkUserId,
                    MobileTransactionDate, MobileReferenceNo, BeatPlanId, ShopName, mobileSyncDate, BeatPlanDeviationReasonId);
            }
        }

        private void saveJointWorkingData(string JointWorkingData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] MasterData = JointWorkingData.Split('#');
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');

                    string UserID = SubData[0];
                    string BeatPlanActivityConfigsId = SubData[1];
                    string SurveyGroupId = SubData[2];
                    string ShopId = SubData[3];
                    string ShopName = SubData[4];
                    string AnswerId = SubData[5];
                    string QuestionId = SubData[6];
                    string Remarks = SubData[7];
                    string MobileTransactionDate = SubData[8];
                    string MobileReferenceNo = SubData[9];
                    string BeatPlanId = SubData[10];
                    string latitude = SubData[11];
                    string longitude = SubData[12];
                    string source = SubData[13];
                    string JointWorkUserId = SubData[14];


                    UpdateJointWorkingDataSync(userCode, BeatPlanActivityConfigsId, SurveyGroupId, ShopId,
                        ShopName, QuestionId, AnswerId, Remarks, MobileTransactionDate, MobileReferenceNo, BeatPlanId, latitude
                        , longitude, source, mobileSyncDate, JointWorkUserId);
                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveTourPlanData()", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void saveTourPlanData(string TourPlanData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] MonthlyMasterData = TourPlanData.Split('*');

                for (int i = 0; i < MonthlyMasterData.Length; i++)
                {
                    string[] MonthlySubData = MonthlyMasterData[i].Split('&');
                    //userId + '&' + TourPlanDate + '&' + SubmissionRemark + '&' + SubmissionDate + '&' +
                    //                SubmittedBy + '&' + mobileReferenceNo + '&' + mobileTransactionDate + '&' + latitude + '&' +
                    //                longitude + '&' + source + '&' + SignalStrength + '&' + NetworkProvider + '&' + dailyTourPlanData;
                    string userId = MonthlySubData[0];
                    string TourPlanDate = MonthlySubData[1];
                    string SubmissionRemark = MonthlySubData[2];
                    string SubmissionDate = MonthlySubData[3];
                    string SubmittedBy = MonthlySubData[4];
                    string mobileReferenceNo = MonthlySubData[5];
                    string mobileTransactionDate = MonthlySubData[6];
                    string latitude = MonthlySubData[7];
                    string longitude = MonthlySubData[8];
                    string source = MonthlySubData[9];
                    string SignalStrength = MonthlySubData[10];
                    string NetworkProvider = MonthlySubData[11];
                    string dailyTourPlanData = MonthlySubData[12];

                    int monthlyTourPlanId = updateMonthlyTourPlanSync(userCode, userId, TourPlanDate, SubmissionRemark, SubmissionDate, SubmittedBy, mobileReferenceNo, mobileTransactionDate,
                                                latitude, longitude, source, SignalStrength, NetworkProvider, mobileSyncDate, mobileDate);

                    string[] DailyMasterData = MonthlySubData[12].Split('%');


                    for (int j = 0; j < DailyMasterData.Length; j++)
                    {
                        string[] DailySubData = DailyMasterData[j].Split('$');
                        //dailyUserId + '$' + Date + '$' + Status + '$' + ActionById + '$' + ActionDate + '$' + dailyMobileReferenceNo + '$' + dailyMobileTransactionDate
                        //                                   + '$' + routePlanData + '$' + jointWorkPlanData + '$' + activityPlanData + '$' + leaveData;
                        string dailyUserId = DailySubData[0];
                        string Date = DailySubData[1];
                        string Status = DailySubData[2];
                        string ActionById = DailySubData[3];
                        string ActionDate = DailySubData[4];
                        string dailyMobileReferenceNo = DailySubData[6];
                        string dailyMobileTransactionDate = DailySubData[5];
                        string RoutePlanData = DailySubData[7];
                        string JointWorkData = DailySubData[8];
                        string ActivityPlanData = DailySubData[9];
                        string LeaveData = DailySubData[10];
                        string leaveReasonId = string.Empty;
                        string fromDate = string.Empty;
                        string toDate = string.Empty;
                        string leavelatitude = string.Empty;
                        string leavelongitude = string.Empty;
                        string processName = string.Empty;
                        string mobileCaptureTime = string.Empty;
                        string leavemobileReferenceNo = string.Empty;
                        string gpsSource = string.Empty;
                        string remarks = string.Empty;
                        string LeaveFromSessionID = string.Empty;
                        string LeaveToSessionID = string.Empty;

                        //string[] RoutePlanMasterData = DailySubData[7].Split('#');
                        //string[] JointWorkMasterData = DailySubData[8].Split('#');
                        //string[] ActivityPlanMasterData = DailySubData[9].Split('#');
                        if (DailySubData[10] != "")
                        {
                            //string[] LeaveMasterData = DailySubData[10].Split('#');

                            //for (int n = 0; n < LeaveMasterData.Length; n++)
                            //{
                            string[] LeaveSubData = LeaveData.Split('@');
                            leaveReasonId = LeaveSubData[1];
                            fromDate = LeaveSubData[2];
                            toDate = LeaveSubData[3];
                            leavelatitude = LeaveSubData[4];
                            leavelongitude = LeaveSubData[5];
                            processName = LeaveSubData[6];
                            mobileCaptureTime = LeaveSubData[7];
                            leavemobileReferenceNo = LeaveSubData[8];
                            gpsSource = LeaveSubData[9];
                            remarks = LeaveSubData[10];
                            LeaveFromSessionID = LeaveSubData[11];
                            LeaveToSessionID = LeaveSubData[12];
                            //}
                        }
                        updateDailyTourPlanSync(userCode, mobileSyncDate, mobileDate, monthlyTourPlanId, dailyUserId, Date, Status, ActionById, ActionDate, dailyMobileReferenceNo, dailyMobileTransactionDate, RoutePlanData, JointWorkData, ActivityPlanData, LeaveData,
                            leaveReasonId, fromDate, toDate, leavelatitude, leavelongitude, processName, mobileCaptureTime, leavemobileReferenceNo, gpsSource, remarks, LeaveFromSessionID, LeaveToSessionID);
                        //for (int k = 0; k < RoutePlanMasterData.Length; k++)
                        //{
                        //    string beatPlanId = RoutePlanMasterData[k];

                        //}
                        //for (int l = 0; l < JointWorkMasterData.Length; l++)
                        //{
                        //    string jointUserId = JointWorkMasterData[l];                          
                        //}
                        //for (int m = 0; m < ActivityPlanMasterData.Length; m++)
                        //{
                        //    string[] ActivityPlanSubData = ActivityPlanMasterData[m].Split('@');

                        //    string activityId = ActivityPlanSubData[0];
                        //    string configFieldId = ActivityPlanSubData[1];
                        //    string value = ActivityPlanSubData[2];                           
                        //}                                                                       
                    }
                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveTourPlanData()", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void saveActivityDeviationData(string activityDeviationData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] DeviationData = activityDeviationData.Split('#');

                for (int i = 0; i < DeviationData.Length; i++)
                {
                    string[] deviationSubData = DeviationData[i].Split('@');
                    string userId = deviationSubData[0];
                    string activityId = deviationSubData[1];
                    string deviationReasonId = deviationSubData[2];
                    string mobileTransactionDate = deviationSubData[3];
                    string mobileReferenceNo = deviationSubData[4];

                    UpdateActivyDeviationSync(userCode, userId, activityId, deviationReasonId, mobileTransactionDate, mobileReferenceNo, mobileSyncDate);


                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveActivityDeviationData() :", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void UpdateActivyDeviationSync(string userCode, string userId, string activityId, string deviationReasonId, string mobileTransactionDate, string mobileReferenceNo, string mobileSyncDate)
        {
            try
            {
                MyActivityBO myactivityBO = new MyActivityBO();
                myactivityBO.Mode = "1";
                myactivityBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                myactivityBO.UserId = Convert.ToInt32(userId);
                myactivityBO.ActivityId = Convert.ToInt32(activityId);
                myactivityBO.ActivityDeviationId = Convert.ToInt32(deviationReasonId);
                myactivityBO.MobileReferenceNo = mobileReferenceNo;
                myactivityBO.MobileTransactionDate = mobileTransactionDate;
                myactivityBO.MobileSyncDate = mobileSyncDate;
                myactivityBO.UpdateActivity();

            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("UpdateActivyDeviationSync()", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void saveActivityLogData(string activityLogData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] ActivityLogData = activityLogData.Split('#');

                for (int i = 0; i < ActivityLogData.Length; i++)
                {
                    string[] logSubData = ActivityLogData[i].Split('@');
                    string userId = logSubData[0];
                    string activityId = logSubData[1];
                    string checkIn = logSubData[2];
                    string checkOut = logSubData[3];
                    string mobileReferenceNo = logSubData[4];
                    string mobileTransactionDate = logSubData[5];
                    string activityPlannedDate = logSubData[6];

                    UpdateActivyCheckInOutSync(userCode, userId, activityId, checkIn, checkOut, mobileTransactionDate, mobileReferenceNo, mobileSyncDate, activityPlannedDate);


                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveActivityLogData() :", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void UpdateActivyCheckInOutSync(string userCode, string userId, string activityId, string checkIn, string checkOut, string mobileTransactionDate, string mobileReferenceNo, string mobileSyncDate, string activityPlannedDate)
        {
            try
            {
                MyActivityBO myactivityBO = new MyActivityBO();
                myactivityBO.Mode = "2";
                myactivityBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                myactivityBO.UserId = Convert.ToInt32(userId);
                myactivityBO.ActivityId = Convert.ToInt32(activityId);
                myactivityBO.CheckIn = checkIn;
                myactivityBO.CheckOut = checkOut;
                myactivityBO.MobileTransactionDate = mobileTransactionDate;
                myactivityBO.MobileSyncDate = mobileSyncDate;
                myactivityBO.MobileReferenceNo = mobileReferenceNo;
                myactivityBO.ActivityPlannedDate = activityPlannedDate;
                myactivityBO.UpdateActivity();
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("UpdateActivyCheckInOutSync()", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void saveMyActivityData(string myActivityData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] ActivityData = myActivityData.Split('#');

                for (int i = 0; i < ActivityData.Length; i++)
                {
                    string[] activitySubData = ActivityData[i].Split('@');
                    string userId = activitySubData[0];
                    string activityId = activitySubData[1];
                    string latitude = activitySubData[2];
                    string longitude = activitySubData[3];
                    string mobileTransactionDate = activitySubData[4];
                    string mobileReferenceNo = activitySubData[5];
                    string gpsSource = activitySubData[6];
                    string configFieldIds = activitySubData[7];
                    string values = activitySubData[8];
                    string signalStrength = activitySubData[9];
                    string networkProvider = activitySubData[10];

                    UpdateMyActivySync(userCode, userId, activityId, mobileTransactionDate, mobileReferenceNo, configFieldIds, values, latitude, longitude
                       , signalStrength, networkProvider, mobileSyncDate, gpsSource);

                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveMyActivityData() :", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void UpdateMyActivySync(string userCode, string userId, string activityId, string mobileTransactionDate, string mobileReferenceNo, string configFieldIds, string values, string latitude, string longitude, string signalStrength, string networkProvider
            , string mobileSyncDate, string gpsSource)
        {
            try
            {
                MyActivityBO myactivityBO = new MyActivityBO();
                myactivityBO.Mode = "3";
                myactivityBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                myactivityBO.UserId = Convert.ToInt32(userId);
                myactivityBO.ActivityId = Convert.ToInt32(activityId);
                myactivityBO.ConfigFieldIds = configFieldIds;
                myactivityBO.ConfigFieldValues = values;
                myactivityBO.Latitude = latitude;
                myactivityBO.Longitude = longitude;
                myactivityBO.MobileTransactionDate = mobileTransactionDate;
                myactivityBO.MobileSyncDate = mobileSyncDate;
                myactivityBO.MobileReferenceNo = mobileReferenceNo;
                myactivityBO.signalStrength = signalStrength;
                myactivityBO.NetworkProvider = networkProvider;
                myactivityBO.gpsSource = Convert.ToInt32(gpsSource);
                myactivityBO.UpdateActivity();
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("UpdateMyActivySync()", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void saveEnquiryData(string EnquiryData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] MasterData = EnquiryData.Split('#');

                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('@');
                    string enquiredby = SubData[0];
                    string activityId = SubData[1];
                    string shopid = SubData[2];
                    string remarks = SubData[3];
                    string latitude = SubData[4];
                    string longitude = SubData[5];
                    string processName = SubData[6];
                    string mobileTransactionDate = SubData[7];
                    string mobileReferenceNo = SubData[8];
                    string gpsSource = SubData[9];
                    string productId = SubData[10];
                    string tempShopId = SubData[11];
                    string signalStrength = SubData[12];
                    string networkProvider = SubData[13];

                    UpdateEnquirySync(userCode, enquiredby, activityId, shopid, remarks, latitude, longitude, processName,
                                        mobileTransactionDate, mobileReferenceNo, gpsSource, mobileSyncDate, mobileDate, productId, tempShopId, signalStrength, networkProvider);

                }
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveEnquiryData()", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void savePaymentData(string paymentData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] MasterData = paymentData.Split('#');

                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('@');

                    string userId = SubData[0];
                    string shopId = SubData[1];
                    string amount = SubData[2];
                    string instrumentNo = SubData[3];
                    string instrumentDate = SubData[4];
                    string bankId = SubData[5];
                    string paymentModeId = SubData[6];
                    string receiptNo = SubData[7];
                    string description = SubData[8];
                    string discount = SubData[9];
                    string latitude = SubData[10];
                    string longitude = SubData[11];
                    string processName = SubData[12];
                    string billNo = SubData[13];
                    string osBalance = SubData[14];
                    string isRemitted = SubData[15];
                    string remittedAt = SubData[16];
                    string isRemittanceWithCollection = SubData[17];
                    string isMultipleDiscountCollection = SubData[18];
                    string discount1Set = SubData[19];
                    string discount2Set = SubData[20];
                    string amountSet = SubData[21];
                    string discount3Set = SubData[22];
                    string mobileTransactionDate = SubData[23];
                    string mobileReferenceNo = SubData[24];
                    string collectionDiscount = SubData[25];
                    string gpsSource = SubData[26];
                    string tempShopId = SubData[27];
                    string signalStrength = SubData[28];
                    string networkProvider = SubData[29];

                    UpdateCollectionSync(userCode, shopId, amount, instrumentNo, instrumentDate, bankId, paymentModeId, receiptNo, description, discount,
                        latitude, longitude, processName, billNo, osBalance, isRemitted, remittedAt, isRemittanceWithCollection,
                        isMultipleDiscountCollection, discount1Set, discount2Set, discount3Set, amountSet, mobileTransactionDate, mobileReferenceNo
                        , collectionDiscount, gpsSource, mobileSyncDate, mobileDate, tempShopId, signalStrength, networkProvider);
                }

            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("savePaymentData()", ex.ToString(), "L2");
                throw ex;
            }
        }

        private void SaveBTLActivityData(string btlActivityData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string userId = "", organizationId = "", btlActivityId = "", mobileReferenceNo = "", mobileTransactionDate = "",
                                                 configFieldIds = "", configFieldValues = "", attendessData = "", latitude = "", longitude = "", source = "",
                                                 SignalStrength = "", NetworkProvider = "";
            string[] MasterData = btlActivityData.Split('#');
            BTLActivityBO btlActivityBO = new BTLActivityBO();
            connectionString = GetConnectionString(userCode.Split('@')[0]);
            btlActivityBO.ConString = connectionString;

            for (int i = 0; i < MasterData.Length; i++)
            {
                string[] SubData = MasterData[i].Split('@');
                userId = SubData[0];
                organizationId = SubData[1];
                btlActivityId = SubData[2];
                mobileReferenceNo = SubData[3];
                mobileTransactionDate = SubData[4];
                configFieldIds = SubData[5];
                configFieldValues = SubData[6];
                attendessData = SubData[7];
                latitude = SubData[8];
                longitude = SubData[9];
                source = SubData[10];
                SignalStrength = SubData[11];
                NetworkProvider = SubData[12];

                UpdateBTLActivitySync(userId, organizationId, btlActivityId, mobileReferenceNo, mobileTransactionDate,
                                             configFieldIds, configFieldValues, attendessData, latitude, longitude, source,
                                             SignalStrength, NetworkProvider, mobileDate, userCode);
            }
        }

        private void UpdateBTLActivitySync(string userId, string organizationId, string btlActivityId, string mobileReferenceNo, string mobileTransactionDate
            , string configFieldIds, string configFieldValues, string attendessData, string latitude, string longitude, string source, string SignalStrength
            , string NetworkProvider, string mobileDate, string userCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(btlActivityId)) // Must fields are checked here.
                {
                    BTLActivityBO btlActivityBO = new BTLActivityBO();
                    btlActivityBO.ConString = GetConnectionString(userCode.Split('@')[0]);

                    btlActivityBO.UserId = Convert.ToInt32(userId);
                    btlActivityBO.Mode = "1";
                    btlActivityBO.OrganizationId = organizationId;
                    btlActivityBO.BTLActivityId = btlActivityId;
                    btlActivityBO.ConfigFieldIds = configFieldIds;
                    btlActivityBO.ConfigFieldValues = configFieldValues;
                    btlActivityBO.Latitude = latitude;
                    btlActivityBO.Longitude = longitude;
                    btlActivityBO.MobileTransactionDate = mobileTransactionDate;
                    btlActivityBO.MobileReferenceNo = mobileReferenceNo;
                    btlActivityBO.GpsSource = source;
                    btlActivityBO.Attendees = attendessData;
                    btlActivityBO.MobileSyncDate = DateTime.Now.ToString();
                    btlActivityBO.UpdateBTLDetails();

                }

            }
            catch (IndexOutOfRangeException ex)
            {
                ////////_log.LogMessage("UpdateBTLActivitySync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.IndexOutOfRange.ToString());
            }
            catch (NullReferenceException ex)
            {
                ////////_log.LogMessage("UpdateBTLActivitySync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.NullReferenceException.ToString());
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("UpdateBTLActivitySync()", ex.ToString(), "L2");
                ////////Context.Response.Write(Status.Error.ToString());
            }
        }


        /// <summary>
        /// SendCustomerApprovalMail
        /// </summary>
        /// <param name="editCustomerBO"></param>
        /// <returns></returns>
        public int SendCustomerApprovalMail(EditCustomerBO editCustomerBO)
        {
            ////try
            ////{
            ////////    DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(editCustomerBO.ConString);
            ////    SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            ////    command.CommandText = "uspEmailSettings";
            ////    sqlHelper.AddParameter(command, "@Mode", "5", ParameterDirection.Input);
            ////    DataSet dsEmailSettings = sqlHelper.ExecuteDataSet(command);

            ////    MailMessage Msg = new MailMessage();
            ////    Attachment inlineLogo = new Attachment(dsEmailSettings.Tables[0].Rows[0]["EmailLogoPath"].ToString());
            ////    Msg.Attachments.Add(inlineLogo);
            ////    string contentID = "Image";
            ////    inlineLogo.ContentId = contentID;
            ////    //To make the image display as inline and not as attachment
            ////    inlineLogo.ContentDisposition.Inline = true;
            ////    inlineLogo.ContentDisposition.DispositionType = DispositionTypeNames.Inline;

            ////    string[] toArray = editCustomerBO.UserEmail.Split(';');
            ////    foreach (string id in toArray)
            ////    {
            ////        if (string.Compare(id, "") != 0)
            ////        {
            ////            Msg.To.Add(id);
            ////        }
            ////    }

            ////    string body = this.PopulateBodyCustomerApproval(editCustomerBO, contentID);

            ////    var fromMail = new MailAddress(dsEmailSettings.Tables[0].Rows[0]["EmailFrom"].ToString(), "");
            ////    // Sender e-mail address.
            ////    Msg.From = fromMail;
            ////    // Recipient e-mail address.               
            ////    ///////// Msg.To.Add(new MailAddress(to, ""));
            ////    string fromPassword = dsEmailSettings.Tables[0].Rows[0]["Password"].ToString();
            ////    // attachment of e-mail
            ////    SmtpClient smtp = new SmtpClient("localhost", 25);
            ////    smtp.Host = dsEmailSettings.Tables[0].Rows[0]["SMTP"].ToString();
            ////    smtp.Port = Convert.ToInt32(dsEmailSettings.Tables[0].Rows[0]["Port"].ToString());
            ////    smtp.EnableSsl = Convert.ToBoolean(dsEmailSettings.Tables[0].Rows[0]["EnableSSL"].ToString());
            ////    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            ////    smtp.UseDefaultCredentials = Convert.ToBoolean(dsEmailSettings.Tables[0].Rows[0]["DefaultCredentials"].ToString());
            ////    smtp.Credentials = new NetworkCredential(fromMail.Address, fromPassword);
            ////    Msg.Subject = editCustomerBO.RequestedUser + " has submitted approval request on customer edit";
            ////    Msg.IsBodyHtml = true;
            ////    //To embed image in email
            ////    Msg.Body = body;
            ////    //   Msg.Body = "<htm><body> <img src=\"cid:" + contentID + "\"> </body></html>";

            ////    smtp.Send(Msg);
            ////}
            ////catch (Exception ex)
            ////{ ////////Context.Response.Write(ex.Message); throw ex; 
            ////}
            return 0;
        }

        /// <summary>
        /// PopulateBodyCustomerApproval
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="title"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public string PopulateBodyCustomerApproval(EditCustomerBO editCustomerBO, string image)
        {
            ////try
            ////{
            ////    string body = string.Empty;
            ////    using (StreamReader reader = new StreamReader(Http////////Context.Current.Server.MapPath("~/emailTemplateCustomerApproval.html")))
            ////    {
            ////        body = reader.ReadToEnd();
            ////    }

            ////    body = body.Replace("[contentID]", image);
            ////    body = body.Replace("[ReportingUser]", editCustomerBO.ReportingUser);
            ////    body = body.Replace("[UserName]", editCustomerBO.RequestedUser);
            ////    body = body.Replace("[ShopName]", editCustomerBO.ShopName);
            ////    body = body.Replace("[NewContact]", editCustomerBO.EditedContactName);
            ////    body = body.Replace("[OldContact]", editCustomerBO.ContactName);
            ////    body = body.Replace("[NewMobile]", editCustomerBO.EditedMobile);
            ////    body = body.Replace("[OldMobile]", editCustomerBO.MobileNo);
            ////    body = body.Replace("[NewPincode]", editCustomerBO.EditedPinCode);
            ////    body = body.Replace("[OldPincode]", editCustomerBO.PinCode);
            ////    body = body.Replace("[NewAddress]", editCustomerBO.EditedAddress);
            ////    body = body.Replace("[OldAddress]", editCustomerBO.Address);
            ////    body = body.Replace("[NewLandmark]", editCustomerBO.Landmark);
            ////    body = body.Replace("[OldLandmark]", editCustomerBO.StreetName);
            ////    return body;
            ////}
            ////catch (Exception ex)
            ////{
            ////    string content = "exception : " + ex.Message + " ## " + DateTime.Now;
            ////    throw ex;
            ////}
            return "";
        }



        public void updateCustomerDetailsSync(string userId, string ShopId, string ContactPersonName, string MobileNumber, string PinCode, string Address
            , string Landmark, string latitude, string longitude, string processName, string mobileTransactionDate
            , string MobileReferenceNo, string gpsSource, string mobileSyncDate, string shopPotential, string gstNumber)
        {
            if (ShopId != "" && userId != "")
            {
                EditCustomerBO editCustomerBO = new EditCustomerBO();

                editCustomerBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                editCustomerBO.UserId = Convert.ToInt32(userIdInt);
                editCustomerBO.Mode = "0";
                editCustomerBO.ShopId = ShopId;
                editCustomerBO.ContactName = ContactPersonName;
                editCustomerBO.MobileNo = MobileNumber;
                editCustomerBO.PinCode = PinCode;
                editCustomerBO.Address = Address;
                editCustomerBO.Landmark = Landmark;
                editCustomerBO.Latitude = latitude;
                editCustomerBO.Longitude = longitude;
                editCustomerBO.ProcessName = processName;
                editCustomerBO.MobileTransactionDate = mobileTransactionDate;
                editCustomerBO.MobileReferenceNo = MobileReferenceNo;
                editCustomerBO.MobileSyncDate = mobileSyncDate;
                editCustomerBO.GpsSource = gpsSource;
                editCustomerBO.ShopPotential = Convert.ToDecimal(shopPotential);
                editCustomerBO.GstNumber = gstNumber;
                editCustomerBO.updateCustomerDetails();

            }

        }
        public void UpdateTodaysPlanSync(string userId, string planIdSet
            , string latitude, string longitude, string processName, string mobileTransactionDate
            , string mobileReferenceNo, string gpsSource, string todaysmobileSyncDate)
        {

            if (userId != "")
            {

                TodaysPlanBO todaysPlanBO = new TodaysPlanBO();
                todaysPlanBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                todaysPlanBO.UserId = Convert.ToInt32(userIdInt ?? "0");
                todaysPlanBO.planIdSet = planIdSet;
                todaysPlanBO.Latitude = latitude;
                todaysPlanBO.Longitude = longitude;
                todaysPlanBO.ProcessName = processName;
                todaysPlanBO.MobileTransactionDate = mobileTransactionDate;
                todaysPlanBO.MobileRefNo = mobileReferenceNo;
                todaysPlanBO.GpsSource = Convert.ToInt32(gpsSource);
                todaysPlanBO.MobileSyncDate = todaysmobileSyncDate;
                todaysPlanBO.MobileTransactionDate = mobileTransactionDate;
                todaysPlanBO.UpdateTodaysPlan();
            }
        }
        /** asset Request **/
        public void UpdateAssetRequestSync(string userId, string AssetId, string ShopId,
            string Field1, string Field2, string Field3, string Field4, string Field5,
             string latitude, string longitude, string processName, string mobileTransactionDate
            , string mobileReferenceNo, string gpsSource, string RequestType, string AssetRequestmobileSyncDate, string AssetNo)
        {

            if (userId != "")
            {

                AssetRequestBO assetRequestBO = new AssetRequestBO();
                assetRequestBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                assetRequestBO.UserId = Convert.ToInt32(userIdInt ?? "0");
                assetRequestBO.ShopId = Convert.ToInt32(ShopId);
                assetRequestBO.AssetNameId = Convert.ToInt32(AssetId);
                assetRequestBO.Field1 = Field1;
                assetRequestBO.Field2 = Field2;
                assetRequestBO.Field3 = Field3;
                assetRequestBO.Field4 = Field4;
                assetRequestBO.Field5 = Field5;
                assetRequestBO.Latitude = latitude;
                assetRequestBO.Longitude = longitude;
                assetRequestBO.ProcessName = processName;
                assetRequestBO.MobileTransactionDate = mobileTransactionDate;
                assetRequestBO.MobileReferenceNo = mobileReferenceNo;
                assetRequestBO.GpsSource = Convert.ToInt32(gpsSource);
                assetRequestBO.Requesttype = RequestType;
                assetRequestBO.MobileDate = AssetRequestmobileSyncDate; // no null
                assetRequestBO.AssetNo = AssetNo;
                assetRequestBO.UpdateAssetRequest();

            }
        }
        /** asset Request **/


        private void saveStockReconcileData(string StockReconcileData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string[] MasterData = StockReconcileData.Split('#');

                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');

                    string userId = SubData[0];
                    string storeId = SubData[1];
                    string productAttributeData = SubData[2];
                    string quantityData = SubData[3];
                    string reasonIdData = SubData[4];
                    string rateData = SubData[5];
                    string currentLatitude = SubData[6];
                    string currentlongitude = SubData[7];
                    string updatedDate = SubData[8];
                    string unloadStatus = SubData[9];
                    string source = SubData[10];
                    string mobileRef = SubData[11];
                    string unitData = SubData[12];
                    string remarkData = SubData[13];
                    updateStockReconcilationSync(userCode, userId, storeId, updatedDate, productAttributeData, quantityData, reasonIdData, rateData
                        , currentLatitude, currentlongitude, unloadStatus, source, mobileDate, mobileRef, unitData, remarkData);

                }

            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveStockReconcileData()", ex.ToString(), "L2");
                throw ex;
            }
        }
        /** asset Request **/

        public void updateStockReconcilationSync(string userCode, string userId, string storeId
            , string reconcileDate, string productIdSet, string quantitySet, string reasonIdSet
            , string rateSet, string latitude, string longitude, string unloadStatus
            , string gpsSource, string mobileDate, string mobileRefNo, string unitData, string remarksData)
        {


            if (userId != null && storeId != null) // Must fields are checked here.
            {
                StockReconcilationBO stockReconcilationBO = new StockReconcilationBO();

                stockReconcilationBO.ConString = GetConnectionString(userCode.Split('@')[0]);

                stockReconcilationBO.UserId = Convert.ToInt32(userId);
                stockReconcilationBO.StoreId = Convert.ToInt32(storeId);
                stockReconcilationBO.ProductIdData = productIdSet;
                stockReconcilationBO.ReasonIdData = reasonIdSet;
                stockReconcilationBO.QuantityData = quantitySet;
                stockReconcilationBO.RateData = rateSet;
                stockReconcilationBO.Latitude = latitude;
                stockReconcilationBO.Longitude = longitude;
                stockReconcilationBO.ProcessName = "btnVanStockReconciliation";
                stockReconcilationBO.ReconcileDate = reconcileDate;
                stockReconcilationBO.GpsSource = Convert.ToInt32(gpsSource);
                stockReconcilationBO.SyncDate = DateTime.Now.ToString();
                stockReconcilationBO.mobileDate = mobileDate;
                stockReconcilationBO.MobileRefNo = mobileRefNo;
                stockReconcilationBO.UnitData = unitData;
                stockReconcilationBO.Remarks = remarksData;
                if (unloadStatus.Equals("true"))
                {
                    stockReconcilationBO.UnloadStatus = true;
                }
                else
                {
                    stockReconcilationBO.UnloadStatus = false;
                }
                stockReconcilationBO.UpdateStockReconcilation();
            }

        }

        private void saveShoplocationData(string ShoplocationData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {

                string[] MasterData = ShoplocationData.Split('~');
                string[] shopIds = MasterData[1].Split(',');
                string[] userIds = MasterData[0].Split(',');
                string[] Latitude = MasterData[2].Split(',');
                string[] longitude = MasterData[3].Split(',');
                string[] currentLatitude = MasterData[4].Split(',');
                string[] currentlongitude = MasterData[5].Split(',');
                string[] source = MasterData[6].Split(',');
                string[] mobileReferenceNo = MasterData[8].Split(',');
                string[] signalStrength = MasterData[9].Split(',');
                string[] networkProvider = MasterData[10].Split(',');
                string[] tempShopId = MasterData[11].Split(',');
                for (int i = 0; i < shopIds.Length; i++)
                {
                    UpdateShopLocationSync(userCode, shopIds[i], userIds[i], Latitude[i], longitude[i], currentLatitude[i], currentlongitude[i], source[i], mobileDate, mobileReferenceNo[i], signalStrength[i], networkProvider[i], tempShopId[i]);
                }

            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("saveShoplocationData()", ex.ToString(), "L2");
                throw ex;
            }
        }

        //private void updateLogForMaterialdelivery(string materialdata, string userCode, string mobileSyncDate, string mobileDate)
        //{
        //    try
        //    {
        //        MaterialTransactionBO materialBO = new MaterialTransactionBO();
        //        materialBO.ConString = GetConnectionString(userCode.Split('@')[0]);
        //        materialBO.UpdateLogsForMaterialDelivery();
        //    }
        //    catch (Exception ex)
        //    {
        //        ////////_log.LogMessage("updateLogForMaterialdelivery()", ex.ToString(), "L2");
        //        throw ex;
        //    }
        //}


        private void UpdateShopLocationSync(string userCode, string shopId, string userId, string Latitude
            , string longitude, string currentLatitude, string currentlongitude, string source, string mobileDate, string mobileReferenceNo, string signalStrength, string networkProvider, string tempShopId)
        {
            string[] user = userCode.Split('@');
            ShopLocationBO shopLocationBO = new ShopLocationBO();
            shopLocationBO.ConString = GetConnectionString(user[0]);
            shopLocationBO.UserId = Convert.ToInt32(userId);
            if (shopId != null && shopId != string.Empty)
            {
                shopLocationBO.ShopId = Convert.ToInt32(shopId);
            }

            shopLocationBO.Latitude = Latitude;
            shopLocationBO.longitude = longitude;
            shopLocationBO.currentLatitude = currentLatitude;
            shopLocationBO.currentlongitude = currentlongitude;
            shopLocationBO.GPSSource = Convert.ToInt32(source);
            shopLocationBO.mobileDate = mobileDate;
            shopLocationBO.MobileReferenceNo = mobileReferenceNo;
            shopLocationBO.signalStrength = signalStrength;
            shopLocationBO.networkProvider = networkProvider;
            shopLocationBO.TempShopId = tempShopId;
            shopLocationBO.SaveShopLocation();
        }


        private void savePhotoCapture(string photoCaptureData, string userCode, string mobileSyncDate)
        {
            try
            {
                string userId = "", shopId = "", imageName = "", captureDate = "", imageDescription = "",
                           lattitude = "", longitude = "", mobRefNo = "", imageData = "", photoDescTypeId = "", processId = "", source = "", processDetailsId = "",
                           tempShopId = "", signalStrength = "", networkProvider = "";

                string[] MasterData = photoCaptureData.Split('#');
                PhotoCaptureBO photoCaptureBO = new PhotoCaptureBO();
                string[] user = userCode.Split('@');
                photoCaptureBO.ConString = GetConnectionString(user[0]);
                DataSet ds = photoCaptureBO.GetImagePath();

                string imagePath = "";
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ////BuildNoClass BuildNoClass = new BuildNoClass();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        imagePath = dr["PhotoPath"].ToString().Trim();
                    }
                }
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');

                    userId = userCode;
                    imageData = SubData[0];
                    imageName = SubData[1];
                    lattitude = SubData[2];
                    longitude = SubData[3];
                    imageDescription = SubData[4];
                    mobRefNo = SubData[5];
                    shopId = SubData[6];
                    photoDescTypeId = SubData[7];
                    captureDate = SubData[8];
                    processId = SubData[9];
                    source = SubData[10];
                    processDetailsId = SubData[12];
                    tempShopId = SubData[13];
                    signalStrength = SubData[14];
                    networkProvider = SubData[15];
                    UpdatePhotoCaptureSync(imageData, imageName, userId, lattitude, longitude, processId
                        , imageDescription, mobRefNo, shopId, photoDescTypeId, mobileSyncDate, captureDate
                        , source, imagePath, processDetailsId, tempShopId, signalStrength, networkProvider);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        private void StockAgingSave(string stockAgingData, string userCode, string mobileSyncDate)
        {
            try
            {

                string userId = "", shopId = "", mobileTransactionDate = "", unitSet = "", productData = "", quantitydata = "",
                  batchNoData = "", manufactureddate = "", expiryDate = "", remarkdata = "", mobileReferenceNo = "", ageData = "", processName = "", latitude = "999", longitude = "999", source = "", signalStrength = "", networkProvider = "";

                string[] MasterData = stockAgingData.Split('#');
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');
                    userId = userCode;
                    shopId = SubData[1];
                    productData = SubData[2];
                    unitSet = SubData[4];
                    quantitydata = SubData[3];
                    batchNoData = SubData[5];
                    manufactureddate = SubData[6];
                    expiryDate = SubData[7];
                    ageData = SubData[8];

                    latitude = SubData[9];
                    longitude = SubData[10];
                    processName = SubData[11];
                    remarkdata = SubData[12];
                    mobileReferenceNo = SubData[13];
                    source = SubData[14];
                    mobileTransactionDate = SubData[15];
                    signalStrength = SubData[16];
                    networkProvider = SubData[17];




                    updateStockAgingSync(userId, shopId, productData, unitSet, quantitydata, batchNoData, manufactureddate, expiryDate, ageData, latitude
                        , longitude, processName, remarkdata, mobileReferenceNo, source, mobileTransactionDate, mobileSyncDate, signalStrength, networkProvider);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void TodaysPlanSave(string todaysPlanData, string userCode, string mobileSyncDate)
        {
            try
            {

                string userId = "", todaysmobileSyncDate = "", mobileTransactionDate = "", Planset = "", syncDate = "", mobileReferenceNo = "", processName = "", latitude = "999", longitude = "999", source = "";

                string[] MasterData = todaysPlanData.Split('#');
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');
                    userId = userCode;
                    Planset = SubData[1];

                    latitude = SubData[3];
                    longitude = SubData[4];
                    processName = SubData[5];
                    mobileReferenceNo = SubData[6];
                    source = SubData[7];
                    mobileTransactionDate = SubData[8];
                    todaysmobileSyncDate = mobileSyncDate;


                    UpdateTodaysPlanSync(userId, Planset, latitude, longitude, processName, mobileTransactionDate
             , mobileReferenceNo, source, todaysmobileSyncDate);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private void EditCustomerDataSave(string EditCustomerData, string userCode, string mobileSyncDate)
        {
            try
            {
                string userId = "", ShopId = "", ContactPersonName = "", MobileNumber = "", PinCode = "", Address = ""
                , Landmark = "", latitude = "", longitude = "", processName = "", mobileTransactionDate = ""
                , MobileReferenceNo = "", gpsSource = "", syncDate = "", shopPotential = "", gstNumber = "";
                string[] MasterData = EditCustomerData.Split('#');

                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');
                    userId = userCode;
                    ShopId = SubData[1];
                    ContactPersonName = SubData[2];
                    MobileNumber = SubData[3];
                    PinCode = SubData[4];
                    Address = SubData[5];
                    Landmark = SubData[6];
                    latitude = SubData[7];
                    longitude = SubData[8];
                    processName = SubData[9];
                    mobileTransactionDate = SubData[10];
                    MobileReferenceNo = SubData[11];
                    gpsSource = SubData[12];
                    mobileSyncDate = SubData[13];
                    shopPotential = SubData[14];
                    gstNumber = SubData[15];
                    updateCustomerDetailsSync(userId, ShopId, ContactPersonName, MobileNumber, PinCode, Address
              , Landmark, latitude, longitude, processName, mobileTransactionDate
              , MobileReferenceNo, gpsSource, syncDate, shopPotential, gstNumber);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /* asset request save */
        private void AssetRequestSave(string AssetRequestData, string userCode, string mobileSyncDate)
        {
            try
            {

                string userId = "", AssetId = "", ShopId = "", AssetRequestmobileSyncDate = "", Field1 = "",
                    Field2 = "", Field3 = "", Field4 = "", Field5 = "", mobileTransactionDate = ""
                , mobileReferenceNo = "", RequestType = "", processName = "", latitude = "999", longitude = "999", source = "", assetNo = "";

                string[] MasterData = AssetRequestData.Split('#');
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');
                    userId = userCode;
                    AssetId = SubData[1];
                    ShopId = SubData[2];
                    Field1 = SubData[3];
                    Field2 = SubData[4];
                    Field3 = SubData[5];
                    Field4 = SubData[6];
                    Field5 = SubData[7];
                    latitude = SubData[8];
                    longitude = SubData[9];
                    processName = SubData[10];
                    mobileTransactionDate = SubData[11];
                    mobileReferenceNo = SubData[12];
                    source = SubData[13];
                    RequestType = SubData[14];
                    assetNo = SubData[15];
                    AssetRequestmobileSyncDate = mobileSyncDate;

                    UpdateAssetRequestSync(userId, AssetId, ShopId, Field1, Field2, Field3, Field4, Field5,
                        latitude, longitude, processName, mobileTransactionDate
             , mobileReferenceNo, source, RequestType, AssetRequestmobileSyncDate, assetNo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private void saveStock(string stockData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string userId = "", shopId = "", unitData = "", productData = "", quantitydata = "", mobilereferenceNO = "",
               lattitude = "", longitude = "", processName = "",
                mobileTransactionDate = "", source = "", signalStrength = "", networkProvider = "";
                string[] MasterData = stockData.Split('#');
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');
                    userId = userCode;
                    shopId = SubData[1];
                    productData = SubData[2];
                    unitData = SubData[7];
                    quantitydata = SubData[3];
                    lattitude = SubData[4];
                    longitude = SubData[5];
                    processName = SubData[6];
                    mobilereferenceNO = SubData[9];
                    source = SubData[10];
                    mobileTransactionDate = SubData[8];
                    signalStrength = SubData[12];
                    networkProvider = SubData[13];
                    updateStockSync(userId, shopId, unitData, productData, quantitydata, lattitude
                        , longitude, processName, mobileTransactionDate, mobileSyncDate, mobilereferenceNO, source, mobileDate, signalStrength, networkProvider);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void updateStockSync(string userId, string shopId, string unitData, string productData, string quantitydata
            , string lattitude, string longitude, string processName, string mobileTransactionDate
            , string mobileSyncDate, string mobileReferenceNo, string source, string mobileDate, string signalStrength, string networkProvider)
        {
            if (userId != "" && shopId != null) // Must fields are checked here.
            {
                StockBO stockBO = new StockBO();
                String prodAndStock = SplitString(productData, quantitydata, unitData);

                String[] dataArr = prodAndStock.Split(':');

                stockBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                stockBO.UserId = Convert.ToInt32(userIdInt);
                stockBO.ShopId = Convert.ToInt32(shopId);
                stockBO.UnitData = dataArr[2];
                stockBO.ProductData = dataArr[0];
                stockBO.StockData = dataArr[1];
                stockBO.Latitude = lattitude;
                stockBO.Longitude = longitude;
                stockBO.ProcessName = processName;
                stockBO.SyncDate = mobileSyncDate;
                stockBO.MobileReferenceNo = mobileReferenceNo;
                stockBO.mobileTransactionDate = mobileTransactionDate;
                stockBO.GpsSource = Convert.ToInt32(source);
                stockBO.mobileDate = mobileDate;
                stockBO.signalStrength = signalStrength;
                stockBO.networkProvider = networkProvider;
                stockBO.UpdateStock();
                //////////////Context.Response.Output.Write(Status.Success.ToString());
            }
        }
        private void updateStockAgingSync(string userId, string shopId, string productData, string unitSet, string quantitydata, string batchNoData, string manufactureddate, string expiryDate, string ageData, string latitude
                        , string longitude, string processName, string remarkdata, string mobileReferenceNo, string source, string mobileTransactionDate, string mobileSyncDate, string signalStrength, string networkProvider)
        {

            if (userId != "" && shopId != null) // Must fields are checked here.
            {
                StockAgingBO stockAgingBO = new StockAgingBO();

                stockAgingBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                stockAgingBO.UserId = Convert.ToInt32(userIdInt);
                stockAgingBO.ShopId = Convert.ToInt32(shopId);
                stockAgingBO.unitSet = unitSet;
                stockAgingBO.productData = productData;
                stockAgingBO.quantitydata = quantitydata;

                stockAgingBO.batchNoData = batchNoData;
                stockAgingBO.manufactureddate = manufactureddate;
                stockAgingBO.expiryDate = expiryDate;
                stockAgingBO.age = ageData;
                stockAgingBO.Latitude = latitude;
                stockAgingBO.Longitude = longitude;
                stockAgingBO.remarks = remarkdata;

                stockAgingBO.ProcessName = processName;
                stockAgingBO.SyncDate = mobileSyncDate;
                stockAgingBO.mobileReferenceNo = mobileReferenceNo;
                stockAgingBO.mobileTransactionDate = mobileTransactionDate;
                stockAgingBO.GpsSource = Convert.ToInt32(source);
                stockAgingBO.signalStrength = signalStrength;
                stockAgingBO.networkProvider = networkProvider;
                stockAgingBO.UpdateStockAging();
            }
            else
            {
                ////////Context.Response.Write(Status.AttributesNull.ToString());
            }

        }

        private void saveStockRequest(string stockRequestData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string userId = "", storeId = "", productIdData = "", stockData = "", unitData = "", lattitude = "",
                longitude = "", stockStatus = "", processName = "",
                createdDate = "", mobileReferenceNo = "", source = "";
                string[] MasterData = stockRequestData.Split('#');
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');
                    userId = userCode;
                    storeId = SubData[1];
                    productIdData = SubData[2];
                    stockData = SubData[3];
                    unitData = SubData[4];
                    lattitude = SubData[5];
                    longitude = SubData[6];
                    processName = SubData[7];
                    stockStatus = SubData[8];
                    createdDate = SubData[9];
                    mobileReferenceNo = SubData[10];
                    source = SubData[11];
                    updateVanStockRequestSync(userId, storeId, unitData, productIdData, stockData, lattitude, longitude, processName, createdDate, mobileSyncDate
                        , mobileReferenceNo, source, mobileDate);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void updateVanStockRequestSync(string userId, string storeId, string unitSet, string productData, string stockData
            , string latitude, string longitude, string processName, string mobileTransactionDate
            , string mobileSyncDate, string mobilereferenceNo, string source, string mobileDate)
        {



            if (userId != "" && storeId != null) // Must fields are checked here.
            {
                StockBO stockBO = new StockBO();

                stockBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                stockBO.UserId = Convert.ToInt32(userIdInt);
                stockBO.ShopId = Convert.ToInt32(storeId);
                stockBO.UnitData = unitSet;
                stockBO.ProductData = productData;
                stockBO.StockData = stockData;
                stockBO.Latitude = latitude;
                stockBO.Longitude = longitude;
                stockBO.ProcessName = processName;
                stockBO.SyncDate = mobileSyncDate;
                stockBO.MobileReferenceNo = mobilereferenceNo;
                stockBO.mobileTransactionDate = mobileTransactionDate;
                stockBO.GpsSource = Convert.ToInt32(source);
                stockBO.mobileDate = mobileDate;
                stockBO.UpdateVanStockRequest();
            }
            else
            {
                ////////Context.Response.Write(Status.AttributesNull.ToString());
            }

        }
        private void salesPromotionSave(string salesPromotionData, string userCode, string mobileSyncDate, string mobileDate)
        {
            try
            {
                string userId = "", shopId = "", unitData = "", productData = "", quantitydata = "",
               lattitude = "", longitude = "", processName = "", narration = "",
                mobileTransactionDate = "", mobileReferenceNO = "", source = "", signalStrength = "", networkProvider = "";
                string[] MasterData = salesPromotionData.Split('#');
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');
                    userId = userCode;
                    shopId = SubData[1];
                    productData = SubData[2];
                    unitData = SubData[8];
                    quantitydata = SubData[4];
                    lattitude = SubData[5];
                    longitude = SubData[6];
                    narration = SubData[3];
                    processName = SubData[7];
                    mobileTransactionDate = SubData[9];
                    mobileReferenceNO = SubData[10];
                    source = SubData[11];
                    signalStrength = SubData[13];
                    networkProvider = SubData[14];
                    updateSalesPromotionSync(userCode, shopId, productData, quantitydata, "", lattitude, longitude
                        , processName, narration, mobileTransactionDate, unitData, mobileSyncDate, mobileReferenceNO, source, mobileDate, signalStrength, networkProvider);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void OrderSave(string OrderData, string userCode, string mobileSyncDate, string mobileDate)
        {
            var sigHashTable = new Hashtable();
            try
            {
                string userId = "", shopId = "", unitSet = "", productData = "", quantitydata = "",
                                scheme = "", mode = "", specialInstruction = "", otherInstns = "", priority = "",
                                siteAddress = "", contactPerson = "", phone = "", price = "", mobileNo = "", latitude = "", longitude = "", processName = "", totalAmount = "",
                                firstDiscount = "", secondDiscount = "", orderDate = "", bankName = "", unitDiscount = "",
                                freeQuantity = "", mobileDiscFlag = "", orderDisc = "",
                                distributorId = "", mobileReferenceNo = "",
                                orderDateMobile = "", orderId = "", schemIdFromMobile,
                                orderDiscountId = "", orderDiscountValues = "", deliveryDate = "", paymentmode = ""
                                , source = "", signalStrength = "", requestedQuantityData = "", tempShopId = "", totalDiscountData = "", taxAmountData = ""
                                , networkProvider = "", invoiceNo = "";
                string[] MasterData = OrderData.Split('#');
                ParameterSettingsBO parameterSettingsBO = new ParameterSettingsBO();
                connectionString = GetConnectionString(userCode.Split('@')[0]);
                parameterSettingsBO.ConString = connectionString;
                int requireERB = parameterSettingsBO.getRequireErb();
                int requreSMSForOrder = parameterSettingsBO.RequireSMSForOrder();
                int length = 0;
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('@');
                    userId = userCode;
                    shopId = SubData[1];
                    unitSet = SubData[2];
                    productData = SubData[3];
                    quantitydata = SubData[4];
                    scheme = SubData[5];
                    mode = SubData[6];
                    specialInstruction = SubData[7];
                    otherInstns = SubData[8];
                    priority = SubData[9];
                    siteAddress = SubData[10];
                    contactPerson = SubData[11];
                    phone = SubData[12];
                    mobileNo = SubData[14];
                    price = SubData[13];
                    latitude = SubData[15];
                    longitude = SubData[16];
                    processName = SubData[17];
                    totalAmount = SubData[18];
                    firstDiscount = SubData[19];
                    secondDiscount = SubData[20];
                    orderDateMobile = SubData[21];
                    orderDate = mobileSyncDate;
                    bankName = SubData[22];
                    orderDisc = SubData[23];
                    unitDiscount = SubData[24];
                    freeQuantity = SubData[25];
                    mobileDiscFlag = SubData[26];
                    distributorId = SubData[27];
                    mobileReferenceNo = SubData[28];
                    orderId = SubData[29];
                    orderDiscountId = SubData[30];
                    orderDiscountValues = SubData[31];
                    deliveryDate = SubData[32];
                    paymentmode = SubData[33];
                    source = SubData[34];
                    schemIdFromMobile = SubData[35];
                    signalStrength = SubData[36];
                    requestedQuantityData = SubData[37];
                    tempShopId = SubData[38];
                    totalDiscountData = SubData[39];
                    taxAmountData = SubData[40];
                    networkProvider = SubData[41];
                    invoiceNo = SubData[42];
                    if (i == MasterData.Length - 1)
                    {
                        length = 1;
                    }
                    UpdateOrderSync(userId, shopId, unitSet, productData, quantitydata,
                                 scheme, mode, specialInstruction, otherInstns, priority,
                                 siteAddress,
                                 contactPerson, phone, price, mobileNo,
                                 latitude, longitude, processName,
                                 totalAmount, firstDiscount, secondDiscount,
                                 i, orderDate, bankName, unitDiscount, freeQuantity,
                                 mobileDiscFlag, orderDisc, distributorId,
                                 mobileReferenceNo, orderDateMobile, orderId, orderDiscountId,
                                 orderDiscountValues, deliveryDate, paymentmode, source
                                 , schemIdFromMobile, mobileDate, requireERB, requreSMSForOrder
                                 , signalStrength, requestedQuantityData, tempShopId, totalDiscountData, taxAmountData, networkProvider, invoiceNo, length);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void SaveDeliveryData(string deliveryData, string userCode, string mobileSyncDate, string mobileDate)
        {
            string OrderId = "", productIdSet = "", deliveredQtySet = "", invoiceQtySet = ""
               , billNo = "", isClosed = "", mobileTransactionDate = "", userId = "", shopId = ""
               , mobileReferenceNo = "", unitIdSet = "", schemeIdSet = "", latitude = "", longitude = "", processName = "",
                gpsSource = "", signalStrength = "", networkProvider = "", returnreasonId = "";



            string[] MasterData = deliveryData.Split('#');
            connectionString = GetConnectionString(userCode.Split('@')[0]);
            for (int i = 0; i < MasterData.Length; i++)
            {

                string[] SubData = MasterData[i].Split('@');
                userId = userCode;
                shopId = SubData[1];
                mobileTransactionDate = SubData[2];
                mobileReferenceNo = SubData[3];
                billNo = SubData[4];
                productIdSet = SubData[5];
                deliveredQtySet = SubData[6];
                unitIdSet = SubData[7];
                schemeIdSet = SubData[8];
                invoiceQtySet = SubData[9];
                OrderId = SubData[10];
                isClosed = SubData[11];
                latitude = SubData[12];
                longitude = SubData[13];
                processName = SubData[14];
                gpsSource = SubData[15];
                signalStrength = SubData[16];
                networkProvider = SubData[17];
                returnreasonId = SubData[18];
                updateDeliveryDetailsSync(OrderId, productIdSet, deliveredQtySet, invoiceQtySet, billNo, isClosed, mobileTransactionDate, userId, shopId
           , mobileReferenceNo, unitIdSet, schemeIdSet, latitude, longitude, processName, gpsSource, signalStrength, networkProvider, returnreasonId);
            }

        }

        /**method to save quotation in offline mode**/
        private void quotationSave(string quotationData, string str1, string mobileSyncDate, string mobileDate)
        {

            try
            {
                string userId = "", shopId = "", rateData = "", productData = "", quantitydata = "",
                to = "", lattitude = "", longitude = "", processName = "", totalAmount = "",
                productIdData = "", mobileTransactionDate = "", mobRefNo = "", tempShopId = "", source = "", signalStrength = "", networkProvider = "";
                string[] MasterData = quotationData.Split('#');
                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = MasterData[i].Split('~');
                    userId = str1;
                    to = SubData[1];
                    shopId = SubData[2];
                    productData = SubData[3];
                    rateData = SubData[4];
                    quantitydata = SubData[5];
                    lattitude = SubData[6];
                    longitude = SubData[7];
                    processName = SubData[8];
                    totalAmount = SubData[9];
                    productIdData = SubData[10];
                    mobileTransactionDate = SubData[11];
                    mobRefNo = SubData[12];
                    tempShopId = SubData[13];
                    source = SubData[14];
                    signalStrength = SubData[16];
                    networkProvider = SubData[17];
                    sendEmailQuotationSync(userId, to, shopId, productData, rateData,
                                 quantitydata, lattitude, longitude, processName, totalAmount, productIdData, mobileTransactionDate
                                 , mobileSyncDate, mobRefNo, tempShopId, source, mobileDate, signalStrength, networkProvider);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void sendEmailQuotationSync(string userId, string to, string shopId, string product, string rate, string quantity, string latitude, string longitude, string processName, string totalAmount, string productId
          , string mobileTransactionDate, string mobileSyncDate, string mobRefNo, string tempShopId, string source, string mobileDate, string signalStrength, string networkProvider)
        {

            QuatationDetailsBO quatationDetailsBO = new QuatationDetailsBO();
            quatationDetailsBO.ConString = GetConnectionString(userId.Split('@')[0]);
            quatationDetailsBO.userId = Convert.ToInt32(userId.Split('@')[1]);
            if (shopId != null && shopId != string.Empty)
            {
                quatationDetailsBO.shopId = Convert.ToInt32(shopId);
            }
            else
            {
                quatationDetailsBO.shopId = 0;
            }

            quatationDetailsBO.processName = processName;
            quatationDetailsBO.productData = productId;
            quatationDetailsBO.quantityData = quantity;
            quatationDetailsBO.rateData = rate;
            quatationDetailsBO.lattitude = latitude;
            quatationDetailsBO.longitude = longitude;
            quatationDetailsBO.totalAmount = totalAmount;
            quatationDetailsBO.emailAddress = to;
            quatationDetailsBO.mobileTransactionDate = mobileTransactionDate;
            quatationDetailsBO.SyncDate = mobileSyncDate;
            quatationDetailsBO.mobRefNo = mobRefNo;
            quatationDetailsBO.TempShopId = tempShopId;
            quatationDetailsBO.GpsSource = Convert.ToInt32(source);
            quatationDetailsBO.MobileSyncDate = mobileDate;
            quatationDetailsBO.signalStrength = signalStrength;
            quatationDetailsBO.networkProvider = networkProvider;
            string quatationNo = quatationDetailsBO.saveQuatation();

            QuatationDetailsBO quatationDetailsBoObject;
            if (quatationDetailsBO.shopId == 0)
            {
                quatationDetailsBoObject = quatationDetailsBO.getQuatationDetails(0);
            }
            else
            {
                quatationDetailsBoObject = quatationDetailsBO.getQuatationDetails(1);
            }
            string customerTableDetails = "Customer Name and Address : \n" + quatationDetailsBoObject.shopName + "\n" + quatationDetailsBO.shopAddress;
            string userTableDetails = "Reference :\n" + quatationDetailsBoObject.userName + "\n" + quatationDetailsBoObject.userRole + "\n" + quatationDetailsBoObject.userAddress;
            string headerName = "";
            string footerName = "";


            try
            {
                int isCustomizedImageNeeded = quatationDetailsBO.isCustomizedImageNeeded();
                if (isCustomizedImageNeeded == 1)
                {
                    string[] prodIdVal = productId.Split(',');
                    ////headerName = getHeaderName(prodIdVal[0], quatationDetailsBO) + "_header";
                    ////footerName = getHeaderName(prodIdVal[0], quatationDetailsBO) + "_footer";
                }
                else
                {
                    headerName = "header";
                    footerName = "footer";
                }
            }
            catch (Exception ex)
            {
                headerName = "header";
                footerName = "footer";
            }

            generatePDFQuatation(product, rate, quantity, quatationNo, customerTableDetails, userTableDetails, totalAmount, headerName, footerName);
            EmailBO emailBO = new EmailBO();
            emailBO.ConString = GetConnectionString(userId.Split('@')[0]);
            EmailBO emailBOObject = emailBO.getEmailSettings();

            var fromAddress = new MailAddress(emailBOObject.emailFrom, "");
            //*****************Splitting the to address*****************************************//
            string[] toArray = to.Split(',');
            var toAddress = new MailAddress(toArray[0], "");

            string fromPassword = emailBOObject.password;
            Attachment attachment;
            ////attachment = new System.Net.Mail.Attachment(System.Web.Hosting.HostingEnvironment.MapPath("~/Quotation" + quatationNo + ".pdf"));
            var smtp = new SmtpClient
            {
                Host = emailBOObject.smtp,
                Port = emailBOObject.port,
                EnableSsl = emailBOObject.enableSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = emailBOObject.defaultCredentials,
                ////Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = emailBOObject.subject,
                Body = emailBOObject.emailData,
            })
            {
                ////message.Attachments.Add(attachment);
                for (int i = 1; i < toArray.Length; i++)
                {
                    var bccAddress = new MailAddress(toArray[i], "");
                    message.Bcc.Add(bccAddress);
                }
                smtp.Send(message);
            }
        }


        private void NewCustomerSave(string newCustomerData, string userCode, string mobileSyncDate)
        {
            try
            {
                string userId = "", customerNames = "", phones = "", reportedBy = "", reportedDates = "",
                             tempMShopIds = "", contactNames = "", shortNames = "", shopTypes = "", shopClasses = "",
                             locations = "", addresses = "", citys = "", states = "", countrys = "", continents = "",
                             beatPlanIds = "", distributorIds = "", emails = "", pincodes = "", tinNos = "", cstNos = "",
                             thirdPartyShopCodes = "", customerGroups = "", creditLimits = "", doorNos = "", streetNames = "",
                             transporterNames = "", districts = "", specialitys = "", mobileTransactionDates = "", parentShopIds = "",
                             mobileReferenceNos = "", field1Set = "", field2Set = "", field3Set = "", field4Set = "", field5Set = "", field6Set = "",
                             field7Set = "", field8Set = "", field9Set = "", field10Set = "", field11Set = "", field12Set = "", field13Set = ""
                             , field14Set = "", field15Set = "", field16Set = "", latitude = "", longitude = ""
                             , processId = "", source = "", gpsmobileTransactionDate = "", gpsMobileReferenceNo = "", signalStrength = "", orderData = "",
                             customerCategoryId = "", gstin = "";
                int length = 0;
                string[] MasterData = Regex.Split(newCustomerData, "##");

                for (int i = 0; i < MasterData.Length; i++)
                {
                    string[] SubData = Regex.Split(MasterData[i], "@@");
                    userId = userCode;
                    customerNames = SubData[0];
                    phones = SubData[1];
                    reportedBy = SubData[2];
                    reportedDates = SubData[3];
                    tempMShopIds = SubData[4];
                    contactNames = SubData[5];
                    shortNames = SubData[6];
                    shopTypes = SubData[7];
                    shopClasses = SubData[8];
                    locations = SubData[9];
                    addresses = SubData[10];
                    citys = SubData[11];
                    states = SubData[12];
                    countrys = SubData[13];
                    continents = SubData[14];
                    beatPlanIds = SubData[15];
                    distributorIds = SubData[16];
                    emails = SubData[17];
                    pincodes = SubData[18];
                    tinNos = SubData[19];
                    cstNos = SubData[20];
                    thirdPartyShopCodes = SubData[21];
                    customerGroups = SubData[22];
                    creditLimits = SubData[23];
                    doorNos = SubData[24];
                    streetNames = SubData[25];
                    transporterNames = SubData[26];
                    districts = SubData[27];
                    specialitys = SubData[28];
                    mobileTransactionDates = SubData[29];
                    parentShopIds = SubData[30];
                    mobileReferenceNos = SubData[31];
                    field1Set = SubData[32];
                    field2Set = SubData[33];
                    field3Set = SubData[34];
                    field4Set = SubData[35];
                    field5Set = SubData[36];
                    field6Set = SubData[37];
                    field7Set = SubData[38];
                    field8Set = SubData[39];
                    field9Set = SubData[40];
                    field10Set = SubData[41];
                    field11Set = SubData[42];
                    field12Set = SubData[43];
                    field13Set = SubData[44];
                    field14Set = SubData[45];
                    field15Set = SubData[46];
                    field16Set = SubData[47];
                    latitude = SubData[48];
                    longitude = SubData[49];
                    processId = SubData[50];
                    source = SubData[52];
                    gpsmobileTransactionDate = SubData[53];
                    gpsMobileReferenceNo = SubData[54];
                    signalStrength = SubData[55];
                    orderData = SubData[56];
                    customerCategoryId = SubData[57];
                    gstin = SubData[58];
                    if (i == MasterData.Length - 1)
                    {
                        length = 1;
                    }
                    UpdateNewCustomerSync(userId, customerNames, phones, reportedBy, reportedDates,
                             tempMShopIds, contactNames, shortNames, shopTypes, shopClasses,
                             locations, addresses, citys, states, countrys, continents,
                             beatPlanIds, distributorIds, emails, pincodes, tinNos, cstNos,
                             thirdPartyShopCodes, customerGroups, creditLimits, doorNos, streetNames,
                             transporterNames, districts, specialitys, mobileTransactionDates, parentShopIds,
                             mobileReferenceNos, field1Set, field2Set, field3Set, field4Set, field5Set, field6Set,
                             field7Set, field8Set, field9Set, field10Set, field11Set, field12Set, field13Set
                             , field14Set, field15Set, field16Set, latitude, longitude
                             , processId, source, gpsmobileTransactionDate, gpsMobileReferenceNo, signalStrength, orderData, mobileSyncDate, customerCategoryId, length
                             , gstin);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        public void UpdateNewCustomerSync(string userId, string shopName, string phone, string reportedBy, string reportedDate, string tempShopId, string ContactName, string shortName,
            string shopType, string shopClass, string Location, string Address, string City, string State, string Country, string continent, string RouteId, string DistributorId
            , string Email, string pincode, string TinNo, string CstNo, string ThirdpartyName, string CustomerGroup, string CreditLimit, string DoorNo, string StreetName, string Transporter
            , string District, string speciality, string mobileTransactionDates, string ParentShopId, string mobRefNo,
            string field1, string field2, string field3, string field4, string field5, string field6, string field7, string field8, string field9, string field10,
            string field11, string field12, string field13, string field14, string field15, string field16, string latitude, string longitude, string processId
            , string source, string gpsmobileTransactionDate, string gpsMobileReferenceNo, string signalStrength, string orderData, string mobileSyncDate
            , string customerCategoryId, int length, string gstin)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(shopName))
                {

                    //////////_log.LogMessage("UpdateNewCustomerSync()", "start new customer", "L2");
                    int newCusomerId = 0;
                    DataSet dsNewCustomerEmailAlertData = new DataSet();
                    NewCustomerBO newCustomerBO = new NewCustomerBO();


                    newCustomerBO.ConString = GetConnectionString(userId.Split('@')[0]);

                    newCustomerBO.reportedBy = Convert.ToInt32(reportedBy);
                    newCustomerBO.ShopName = shopName;
                    newCustomerBO.Phone = phone;
                    newCustomerBO.ProcesName = "btnNewShop";
                    newCustomerBO.Location = Location;
                    newCustomerBO.District = District;
                    newCustomerBO.reportedDate = reportedDate;
                    newCustomerBO.TempShopId = tempShopId;
                    newCustomerBO.contactName = ContactName;
                    newCustomerBO.shortName = shortName;
                    newCustomerBO.ShopType = shopType;
                    newCustomerBO.ShopClass = shopClass;
                    newCustomerBO.Location = Location;
                    newCustomerBO.Address = Address;
                    newCustomerBO.City = City;
                    newCustomerBO.State = State;
                    newCustomerBO.Country = Country;
                    newCustomerBO.Continent = continent;
                    newCustomerBO.reportedDate = reportedDate;
                    newCustomerBO.gstin = gstin;
                    if (!string.IsNullOrEmpty(RouteId))
                    {
                        newCustomerBO.RouteId = Convert.ToInt32(RouteId);
                    }
                    if (!string.IsNullOrEmpty(DistributorId))
                    {
                        newCustomerBO.DistributorId = Convert.ToInt32(DistributorId);
                    }
                    newCustomerBO.District = District;
                    newCustomerBO.Email = Email;
                    newCustomerBO.pinCode = pincode;
                    newCustomerBO.TinNo = TinNo;
                    newCustomerBO.CSTNo = CstNo;
                    newCustomerBO.ThirdParty = ThirdpartyName;
                    newCustomerBO.CustomerGroup = CustomerGroup;
                    newCustomerBO.CreditLimit = CreditLimit;
                    newCustomerBO.DoorNo = DoorNo;
                    newCustomerBO.StreetName = StreetName;
                    newCustomerBO.Transporter = Transporter;
                    newCustomerBO.ParentShopId = ParentShopId;
                    newCustomerBO.Field1 = field1;
                    newCustomerBO.Field2 = field2;
                    newCustomerBO.Field3 = field3;
                    newCustomerBO.Field4 = field4;
                    newCustomerBO.Field5 = field5;
                    newCustomerBO.Field6 = field6;
                    newCustomerBO.Field7 = field7;
                    newCustomerBO.Field8 = field8;
                    newCustomerBO.Field9 = field9;
                    newCustomerBO.Field10 = field10;
                    newCustomerBO.Field11 = field11;
                    newCustomerBO.Field12 = field12;
                    newCustomerBO.Field13 = field13;
                    newCustomerBO.Field14 = field14;
                    newCustomerBO.Field15 = field15;
                    newCustomerBO.Field16 = field16;
                    newCustomerBO.MobRefNo = mobRefNo;
                    newCustomerBO.Latitude = latitude;
                    newCustomerBO.Longitude = longitude;
                    newCustomerBO.Length = length;
                    if (!string.IsNullOrEmpty(source))
                    {
                        newCustomerBO.GpsSource = Convert.ToInt32(source);
                    }
                    newCustomerBO.MobileSyncDate = mobileSyncDate;
                    if (!string.IsNullOrEmpty(customerCategoryId))
                    {
                        newCustomerBO.CustomerCategoryid = Convert.ToInt32(customerCategoryId);
                    }
                    dsNewCustomerEmailAlertData = newCustomerBO.UpdateNewCustomerSync();
                    newCusomerId = Convert.ToInt32(dsNewCustomerEmailAlertData.Tables[0].Rows[0][0].ToString());
                    int IsEmailAlertRequiredNewcustomer = newCustomerBO.IsEmailAlertRequiredNewCustomer();
                    if (IsEmailAlertRequiredNewcustomer == 1)
                    {
                        if (dsNewCustomerEmailAlertData.Tables.Contains("Table1"))
                        {
                            string usernameNewCust = dsNewCustomerEmailAlertData.Tables[1].Rows[0][0].ToString();
                            string reportingToNewCust = dsNewCustomerEmailAlertData.Tables[1].Rows[0][1].ToString();
                            string ToMailNewCust = dsNewCustomerEmailAlertData.Tables[1].Rows[0]["email"].ToString();

                            string ShopNameNewCust = dsNewCustomerEmailAlertData.Tables[2].Rows[0]["ShopName"].ToString();
                            string ShopTypeNewCust = dsNewCustomerEmailAlertData.Tables[2].Rows[0]["shopType"].ToString();
                            string AddressNewCust = dsNewCustomerEmailAlertData.Tables[2].Rows[0]["address"].ToString();
                            string ContactNameNewCust = dsNewCustomerEmailAlertData.Tables[2].Rows[0]["ContactName"].ToString();
                            string MobileNewCust = dsNewCustomerEmailAlertData.Tables[2].Rows[0]["mobileNo"].ToString();
                            string RouteNewCust = dsNewCustomerEmailAlertData.Tables[2].Rows[0]["planName"].ToString();
                            string EmailNewCust = dsNewCustomerEmailAlertData.Tables[2].Rows[0]["email"].ToString();
                            string ShopclassNewCust = dsNewCustomerEmailAlertData.Tables[2].Rows[0]["shopClass"].ToString();

                            string LandMarkNewCust = "";
                            if (dsNewCustomerEmailAlertData.Tables[3] != null)
                            {
                                LandMarkNewCust = dsNewCustomerEmailAlertData.Tables[3].Rows[0][0].ToString();
                            }

                            string LandLineNewCust = "";
                            if (dsNewCustomerEmailAlertData.Tables[4] != null)
                            {
                                LandLineNewCust = dsNewCustomerEmailAlertData.Tables[4].Rows[0][0].ToString();
                            }

                            sendEmailAlertNewCustomer(usernameNewCust, reportingToNewCust,
                                ToMailNewCust, ShopNameNewCust, ShopTypeNewCust, AddressNewCust, ContactNameNewCust,
                                MobileNewCust, RouteNewCust, EmailNewCust, ShopclassNewCust, LandMarkNewCust, LandLineNewCust
                                , newCustomerBO.ConString);
                        }
                    }


                    if (newCusomerId > 0)
                    {
                        NewCustomerOrderSave(orderData, userId, newCusomerId, mobileSyncDate);
                    }
                }
                else
                {
                    ////////////Context.Response.Output.Write(Status.AttributesNull.ToString());
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                ////////_log.LogMessage("UpdateNewCustomerSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.IndexOutOfRange.ToString());
            }
            catch (NullReferenceException ex)
            {
                ////////_log.LogMessage("UpdateNewCustomerSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.NullReferenceException.ToString());
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("UpdateNewCustomerSync()", ex.ToString(), "L2");
                ////////Context.Response.Write(Status.Error.ToString());
            }
        }


        public void NewCustomerOrderSave(string orderData, string userCode, int newCustomerId, string MobileSyncDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(orderData))
                {
                    string[] orderValue = Regex.Split(orderData, "%%");

                    string orderTakenBy = "", schemeId = "", otherInstruction = "", priority = "", lastPrice = "", shopName = ""
                                , orderMobileReferenceNo = "", deliveryDate = "", orderDiscount = "", paymentMode = "", receiptNumber = "", orderDate = "",
                                productData = "", quantityData = "", amountData = "", schemeIdFromMobile = "", unitSet = "", priceIdSet = "", rateSet = ""
                                , rateDiscountSet = "", attributeIdSet = "", firstDiscount = "", secondDiscount = "", outletStock = "", orderdistributorId = ""
                                , orderDiscountIds = "", orderDiscountValues = "", unitDiscount = "", freeQty = "", mobileDiscountFlag = "", totalDiscountData = "", taxAmountData = "";
                    int length = 0;
                    for (int i = 0; i < orderValue.Length; i++)
                    {

                        string[] SubData = Regex.Split(orderValue[i], "&&");

                        orderDate = SubData[0];
                        orderTakenBy = SubData[1];
                        schemeId = SubData[3];
                        otherInstruction = SubData[4];
                        priority = SubData[5];
                        lastPrice = SubData[6];
                        shopName = SubData[7];
                        orderMobileReferenceNo = SubData[8];
                        deliveryDate = SubData[9];
                        orderDiscount = SubData[10];
                        paymentMode = SubData[11];
                        receiptNumber = SubData[12];
                        productData = SubData[14];
                        quantityData = SubData[15];
                        amountData = SubData[19];
                        schemeIdFromMobile = SubData[17];
                        unitSet = SubData[16];
                        priceIdSet = SubData[22];
                        rateSet = SubData[18];
                        rateDiscountSet = SubData[23];
                        attributeIdSet = SubData[25];
                        firstDiscount = SubData[20];
                        secondDiscount = SubData[21];
                        outletStock = SubData[24];
                        orderdistributorId = SubData[13];
                        orderDiscountIds = SubData[26];
                        orderDiscountValues = SubData[27];
                        unitDiscount = SubData[28];
                        freeQty = SubData[29];
                        mobileDiscountFlag = SubData[30];
                        totalDiscountData = SubData[31];
                        taxAmountData = SubData[32];
                        if (i == orderValue.Length - 1)
                        {
                            length = 1;
                        }
                        UpdateNewCustomerOrderSync(orderTakenBy, schemeId, otherInstruction, priority, lastPrice, shopName
                                , orderMobileReferenceNo, deliveryDate, orderDiscount, paymentMode, receiptNumber, orderDate,
                                productData, quantityData, amountData, schemeIdFromMobile, unitSet, priceIdSet, rateSet
                                , rateDiscountSet, attributeIdSet, firstDiscount, secondDiscount, outletStock, orderdistributorId, newCustomerId, userCode
                                , orderDiscountIds, orderDiscountValues, MobileSyncDate, unitDiscount, freeQty, mobileDiscountFlag, totalDiscountData, taxAmountData, length);
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                ////////_log.LogMessage("NewCustomerOrderSave()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.IndexOutOfRange.ToString());
            }
            catch (NullReferenceException ex)
            {
                ////////_log.LogMessage("NewCustomerOrderSave()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.NullReferenceException.ToString());
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("NewCustomerOrderSave()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.Error.ToString());
            }
        }



        public void UpdateNewCustomerOrderSync(string orderTakenBy, string schemeId, string otherInstruction, string priority, string lastPrice, string shopName
                           , string orderMobileReferenceNo, string deliveryDate, string orderDiscount, string paymentMode, string receiptNumber, string orderDate, string
                           productData, string quantityData, string amountData, string schemeIdFromMobile, string unitSet, string priceIdSet, string rateSet
                           , string rateDiscountSet, string attributeIdSet, string firstDiscount, string secondDiscount, string outletStock, string orderdistributorId, int newCustomerId, string userCode
                           , string orderDiscountIds, string orderDiscountValues, string mobileSyncDate, string unitDiscount, string freeQty, string mobileDiscountFlag
            , string totalDiscountData, string taxAmountData, int length)
        {
            try
            {
                if (orderTakenBy != "" && newCustomerId != null && productData != null) // Must fields are checked here.
                {

                    NewCustomerOrderBO orderBO = new NewCustomerOrderBO();

                    orderBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                    orderBO.UserId = Convert.ToInt32(orderTakenBy);

                    orderBO.ShopId = Convert.ToString(newCustomerId);
                    orderBO.Priority = priority;
                    orderBO.UnitData = unitSet;
                    orderBO.ProductData = productData;
                    orderBO.QuantityData = quantityData;
                    orderBO.TotalAmount = amountData;
                    orderBO.FirstDiscount = firstDiscount;
                    orderBO.SecondDiscount = secondDiscount;
                    orderBO.SchemeId = schemeIdFromMobile;
                    orderBO.Mode = "2";
                    orderBO.OtherInstn = otherInstruction;
                    orderBO.Rate = rateSet;
                    orderBO.OutletStock = outletStock;
                    orderBO.OrderDate = orderDate;
                    orderBO.mobileReferenceNo = orderMobileReferenceNo;
                    orderBO.orderDiscountVals = orderDiscountValues;
                    orderBO.orderDiscountIds = orderDiscountIds;
                    orderBO.deliveryDate = deliveryDate;
                    orderBO.ReceiptNumber = receiptNumber;
                    orderBO.UnitDiscount = unitDiscount;
                    orderBO.FreeQuantity = freeQty;
                    orderBO.MobileDiscountFlag = mobileDiscountFlag;
                    orderBO.Length = length;
                    if (!string.IsNullOrEmpty(paymentMode))
                    {
                        orderBO.paymentMode = Convert.ToInt32(paymentMode);
                    }

                    orderBO.orderDisc = orderDiscount;

                    orderBO.MobileSyncDate = mobileSyncDate;
                    orderBO.TotalDiscount = totalDiscountData;
                    orderBO.TaxAmount = taxAmountData;
                    if (!string.IsNullOrEmpty(orderdistributorId))
                    {
                        orderBO.distributorId = Convert.ToInt32(orderdistributorId);
                    }

                    string newcustvalues = orderBO.UserId + " # " + orderBO.ShopId + " # " + orderBO.Priority + " # " + orderBO.UnitData + " # " + orderBO.ProductData + " # " + orderBO.QuantityData + " # " + orderBO.TotalAmount + " # " + orderBO.FirstDiscount + " # " + orderBO.SecondDiscount + " # " + orderBO.SchemeId + " # " + orderBO.Mode + " # " + orderBO.SpecialInstnSet + " # " + orderBO.OtherInstn + " # " + orderBO.Priority + " # " + orderBO.SiteAddress + " # " + orderBO.ContactPerson + " # " + orderBO.Phone + " # " + orderBO.Rate + " # " + orderBO.MobileNo + " # " + orderBO.OutletStock + " # " + orderBO.OrderDate + " # " + orderBO.mobileReferenceNo + " # " + orderBO.orderDiscountVals + " # " + orderBO.orderDiscountIds + " # " + orderBO.deliveryDate + " # " + orderBO.paymentMode + " # " + orderBO.orderDisc + " # " + orderBO.distributorId;
                    orderBO.UpdateNewCustomerOrder();

                }
                else
                {
                    ////////////Context.Response.Output.Write(Status.AttributesNull.ToString());
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                ////////_log.LogMessage("UpdateNewCustomerOrderSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.IndexOutOfRange.ToString());
            }
            catch (NullReferenceException ex)
            {
                ////////_log.LogMessage("UpdateNewCustomerOrderSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.NullReferenceException.ToString());
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("UpdateNewCustomerOrderSync()", ex.ToString(), "L2");
                ////////////Context.Response.Output.Write(Status.Error.ToString());
            }
        }




        public void UpdateCollectionSync(string userId, string shopId, string amount, string instrumentNo, string instrumentDate
            , string bankId, string paymentModeId, string receiptNo, string description, string discount, string latitude
            , string longitude, string processName, string billNo, string osBalance, string isRemitted, string remittedAt
            , string isRemittanceWithCollection, string isMultipleDiscountCollection, string discount1Set, string discount2Set
            , string discount3Set, string amountSet, string mobileTransactionDate, string mobileReferenceNo, string collectionDiscount, string gpsSource
            , string mobileSyncDate, string mobileDate, string tempShopId, string signalStrength, string networkProvider)
        {

            try
            {
                if (userId != "" && amount != null)
                {
                    CollectionBO collectionBO = new CollectionBO();
                    collectionBO.ConString = GetConnectionString(userId.Split('@')[0]);
                    string userIdVal = userId.Split('@')[1];
                    collectionBO.Mode = "I";
                    collectionBO.CollectedBy = Convert.ToInt32(userIdVal);
                    collectionBO.Amount = amountSet;

                    collectionBO.TotalAmount = Convert.ToDouble(amount);
                    collectionBO.Latitude = latitude;
                    collectionBO.Longitude = longitude;
                    collectionBO.ProcessName = processName;
                    if (!instrumentDate.Equals(""))
                    {
                        collectionBO.InstrumentDate = Utils.ConvertStringToDate(instrumentDate);
                    }
                    collectionBO.InstrumentNo = instrumentNo;
                    collectionBO.ReceiptNo = receiptNo;
                    if (!string.IsNullOrEmpty(bankId) && !bankId.Equals("0"))
                    {
                        collectionBO.BankId = Convert.ToInt32(bankId);
                    }
                    else
                    {
                        collectionBO.BankId = 0;
                    }

                    collectionBO.PaymentModeId = paymentModeId;
                    collectionBO.Narration = description;
                    collectionBO.ShopId = Convert.ToInt32(shopId);
                    collectionBO.BillNo = billNo;
                    collectionBO.OsBalance = osBalance;
                    collectionBO.Discount = discount;
                    collectionBO.GpsSource = Convert.ToInt32(gpsSource); ;
                    if (isRemitted.Equals("True"))
                    {
                        collectionBO.IsRemitted = "1";
                    }
                    else
                    {
                        collectionBO.IsRemitted = "0";
                    }
                    if (!remittedAt.Equals(""))
                    {
                        collectionBO.RemittedAt = Convert.ToInt32(remittedAt);
                    }

                    collectionBO.IsRemittanceWithCollection = isRemittanceWithCollection;
                    collectionBO.IsMultipleDiscountCollection = isMultipleDiscountCollection;
                    collectionBO.Discount1 = discount1Set;
                    collectionBO.Discount2 = discount2Set;
                    collectionBO.Discount3 = discount3Set;
                    collectionBO.MobilePaymentDate = mobileTransactionDate;
                    collectionBO.MobileReferenceNo = mobileReferenceNo;
                    collectionBO.CollectionDiscount = collectionDiscount;
                    collectionBO.MobileSyncDate = mobileSyncDate;
                    collectionBO.MobileDate = mobileDate;
                    collectionBO.TempShopId = tempShopId;
                    collectionBO.signalStrength = signalStrength;
                    collectionBO.networkProvider = networkProvider;
                    collectionBO.UpdateCollection();
                    ParameterSettingsBO parameterSettingsBO = new ParameterSettingsBO();
                    parameterSettingsBO.ConString = collectionBO.ConString;

                    int requreSMSForCollection = parameterSettingsBO.RequireSMSForcollection();

                    if (requreSMSForCollection == 1)
                    {
                        Thread threadSMs = new Thread(() => { sendSMSCollection(Convert.ToDouble(amount), shopId, collectionBO.ConString, paymentModeId, processName); });
                        threadSMs.Start();

                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                ////////_log.LogMessage("UpdateCollection()", ex.ToString(), "L2");
                throw ex;
            }
            catch (NullReferenceException ex)
            {
                ////////_log.LogMessage("UpdateCollection()", ex.ToString(), "L2");
                throw ex;
            }
            catch (Exception ex)
            {
                ////////_log.LogMessage("UpdateCollection()", ex.ToString(), "L2");
                throw ex;
            }

        }


        private Dictionary<string, string> GetSqliteDatabaseSplitAllDatabases(string UserID, string customerCode, string value, Boolean isFMDb
            , string lastModifiedDate, string userWiseProductRequired)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Dictionary<string, string> queryDBSplit = new Dictionary<string, string>();
            List<string> ViewNamesDBSplit = new List<string>();
            try
            {

                ////queryDBSplit = GetQueriesSplitAllDatabases(UserID, value, userWiseProductRequired);

                ////if (value.ToUpper().Equals("TRANSACTION_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewsForTransactionTables.xml"));
                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {

                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);
                ////        //////////_log.LogMessage("TransactionDB", viewname, "L2");


                ////    }
                ////}
                ////else if (value.ToUpper().Equals("BASEMASTER_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewsFMMaster.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);


                ////    }
                ////}
                ////else if (value.ToUpper().Equals("APPLICATIONMASTER_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewsFMMaster1.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);


                ////    }
                ////}
                ////else if (value.ToUpper().Equals("SHOPMASTER_DB"))
                ////{
                ////    ParameterSettingsBO parameterSettingsBO = new ParameterSettingsBO();
                ////    parameterSettingsBO.ConString = GetConnectionString(customerCode);
                ////    int isDailyBeat = parameterSettingsBO.getIsDailyBeat();
                ////    int isDateWiseBeat = parameterSettingsBO.getIsDateWiseBeat();
                ////    XDocument xmlDoc1;
                ////    xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewsFMMasterShop.xml"));
                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);


                ////    }
                ////}
                /////**asset requests **/
                ////else if (value.ToUpper().Equals("ASSETMASTER_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewsAssetRequests.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);


                ////    }

                ////}

                /////**asset requests db end**/
                ////else if (value.ToUpper().Equals("PRODUCTMASTER_DB"))
                ////{

                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewsFMMasterProduct.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);


                ////    }
                ////}
                ////else if (value.ToUpper().Equals("USERMASTER_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewsFMMasterUser.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);


                ////    }

                ////}

                ////else if (value.ToUpper().Equals("CHEQUE_REMITTANCE_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewForChequeRemittance.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);
                ////    }
                ////}
                /////**milma db.*/
                ////else if (value.ToUpper().Equals("MILMA_MASTER_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/Milma/InlineViewsMaster.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);


                ////    }

                ////}
                ////else if (value.ToUpper().Equals("MILMA_SHOPMASTER_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/Milma/InlineViewsShopMaster.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);


                ////    }

                ////}
                ////else if (value.ToUpper().Equals("MILMA_PRODUCTMASTER_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/Milma/InlineViewsProductMaster.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);


                ////    }

                ////}
                ////else if (value.ToUpper().Equals("MILMA_USERMASTER_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/Milma/InlineViewsUserMaster.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);


                ////    }

                ////}
                /////**milma db end**/
                ////else if (value.ToUpper().Equals("PRODUCT_TRANSACTION_DB"))
                ////{

                ////    ProductTransactionBO productTransactionBO = new ProductTransactionBO();
                ////    connectionString = GetConnectionString(customerCode);
                ////    productTransactionBO.ConString = connectionString;
                ////    productTransactionBO.UserId = Convert.ToInt32(UserID.Split('@')[1]);
                ////    DataSet dataSetQty = productTransactionBO.GetOrderedQty();
                ////    productTransactionBO.GetReturnedQty();
                ////    productTransactionBO.GetDeliveredQty();
                ////    // productTransactionBO.GetReturnedShopDetails();
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewCollectedAmount.xml"));

                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);
                ////    }
                ////}
                ////else if (value.ToUpper().Equals("PLANOGRAM_MASTER_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewsPlanogramMaster.xml"));
                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);
                ////    }
                ////}

                ////else if (value.ToUpper().Equals("STOCKMASTER_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewsFMStock.xml"));
                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);
                ////    }
                ////}
                ////else if (value.ToUpper().Equals("TOURPLAN_DB"))
                ////{
                ////    XDocument xmlDoc1 = XDocument.Load(Server.MapPath("~/Resources/InlineViewsFMTourPlan.xml"));
                ////    foreach (XElement node in xmlDoc1.Descendants("Root").Elements())
                ////    {
                ////        string viewname = node.Attribute("viewname").Value;
                ////        string condition = node.Attribute("condition").Value;
                ////        string IsSync = node.Attribute("IsSync").Value;
                ////        ViewNamesDBSplit.Add(viewname + '#' + condition + '#' + IsSync);
                ////    }
                ////}

                ////string path = "";
                ////if (isFMDb)
                ////    path = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID);
                ////else
                ////    path = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/Milma/" + UserID);
                ////if (!Directory.Exists(path))
                ////{
                ////    Directory.CreateDirectory(path);
                ////}

                ////string sqlConnString;
                string[] strArr = null;
                ////strArr = UserID.Split('@');
                ////sqlConnString = GetConnectionString(customerCode);// connectionString;
                string sqlitePath = "";
                ////if (value.ToUpper().Equals("TRANSACTION_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMTransaction.db");
                ////}
                ////else if (value.ToUpper().Equals("BASEMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMBaseMaster.db");
                ////}
                ////else if (value.ToUpper().Equals("APPLICATIONMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMApplicationMaster.db");
                ////}
                ////else if (value.ToUpper().Equals("SHOPMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMMasterShop.db");
                ////}
                ////else if (value.ToUpper().Equals("PRODUCTMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMMasterProduct.db");
                ////}
                ////else if (value.ToUpper().Equals("USERMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMMasterUser.db");
                ////}
                ////else if (value.ToUpper().Equals("MASTERLOG_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMMasterLog.db");
                ////}
                ////else if (value.ToUpper().Equals("PRODUCT_TRANSACTION_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMProductTransaction.db");
                ////}

                ////else if (value.ToUpper().Equals("CHEQUE_REMITTANCE_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMChequeRemittance.db");
                ////}
                ////else if (value.ToUpper().Equals("EXPENSE_TRANSACTION_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMExpenseTransaction.db");
                ////}
                /////**milma db path.*/
                ////else if (value.ToUpper().Equals("MILMA_MASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/Milma/" + UserID + "/FMMaster.db");
                ////}
                ////else if (value.ToUpper().Equals("MILMA_SHOPMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/Milma/" + UserID + "/FMMasterShop.db");
                ////}

                ////else if (value.ToUpper().Equals("MILMA_PRODUCTMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/Milma/" + UserID + "/FMMasterProduct.db");
                ////}
                ////else if (value.ToUpper().Equals("MILMA_USERMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/Milma/" + UserID + "/FMMasterUser.db");
                ////}
                ////else if (value.ToUpper().Equals("MILMA_MASTERLOG_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/Milma/" + UserID + "/FMMasterLog.db");
                ////}
                ////else if (value.ToUpper().Equals("PLANOGRAM_MASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMMasterPlanogram.db");
                ////}
                ////else if (value.ToUpper().Equals("STOCKMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMMasterStock.db");
                ////}

                ////else if (value.ToUpper().Equals("PJPMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMMasterPJP.db");
                ////}

                /////*asset master */
                ////else if (value.ToUpper().Equals("ASSETMASTER_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FmAssetMaster.db");
                ////}
                /////*asset master */
                ////else if (value.ToUpper().Equals("PLANNEDPJP_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMPlannedPJP.db");
                ////}

                ////else if (value.ToUpper().Equals("PRODUCTSETTINGS_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMProductSettings.db");
                ////}
                ////else if (value.ToUpper().Equals("SHIPMENT_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMShipment.db");
                ////}
                ////else if (value.ToUpper().Equals("TOURPLAN_DB"))
                ////{
                ////    sqlitePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Sqlite/" + UserID + "/FMTourPlan.db");
                ////}
                string password = "dbpwd";// txtPassword.Text.Trim();

                foreach (var key in queryDBSplit.Keys.ToArray())
                {
                    queryDBSplit[key] = queryDBSplit[key].Replace("@UserId", strArr[1]);//.ToString());               
                }

                for (int i = 0; i < ViewNamesDBSplit.Count; i++)
                {
                    string view = ViewNamesDBSplit[i];
                    ViewNamesDBSplit[i] = view.Replace("@UserId", strArr[1]);//.ToString());
                }
                if (!CheckGenerateDb(UserID, value))
                {
                    lastModifiedDate = DateTime.Now.ToString();
                    result = SqlServerToSQLite.ConvertSqlServerDatabaseToSQLiteFile(sqlConnString, sqlitePath, password, queryDBSplit, ViewNamesDBSplit, false, sqlitePath, strArr[1], lastModifiedDate);
                    UpdateGenerateDB(UserID, value);
                }
                else
                {
                    if (!File.Exists(sqlitePath))
                    {
                        result = SqlServerToSQLite.ConvertSqlServerDatabaseToSQLiteFile(sqlConnString, sqlitePath, password, queryDBSplit, ViewNamesDBSplit, false, sqlitePath, strArr[1], lastModifiedDate);
                        UpdateGenerateDB(UserID, value);
                    }
                    else
                    {
                        string data = Convert.ToBase64String(File.ReadAllBytes(sqlitePath));
                        string checksum = SqlServerToSQLite.MD5Encryption(data);
                        result.Add("Result", "true");
                        result.Add("Value", checksum);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Add("Result", "false");

            }
            return result;
        }


        #endregion

        private void UpdateGenerateDB(string UserID, string value)
        {
            string connectionString = GetConnectionString(UserID.Split('@')[0]);
            GenarateDbBO genarateDbBO = new GenarateDbBO();
            genarateDbBO.ConString = connectionString;
            genarateDbBO.DbName = value;
            genarateDbBO.UserId = Convert.ToInt32(UserID.Split('@')[1]);
            genarateDbBO.UpdateGenerateDB();
        }

        private Boolean CheckGenerateDb(string UserID, string value)
        {
            if (!value.ToUpper().Equals("MASTERLOG_DB"))
            {

                string connectionString = GetConnectionString(UserID.Split('@')[0]);
                //////_log.LogMessage("connectionString : ", connectionString, "L2");
                GenarateDbBO genarateDbBO = new GenarateDbBO();
                genarateDbBO.ConString = connectionString;
                genarateDbBO.DbName = value;
                genarateDbBO.UserId = Convert.ToInt32(UserID.Split('@')[1]);
                //////_log.LogMessage("CheckGenerateDb : ", genarateDbBO.CheckGenerateDb().ToString(), "L2");
                return genarateDbBO.CheckGenerateDb();
            }
            else
            {
                return false;
            }
        }


        private void sendSMSCollection(double amountValue, string shopId, string conn, string paymentModeId, string processName)
        {
            ////String phoneNo = GetMobileNumber(shopId, conn);
            ////SMS sms = new SMS();
            ////if (paymentModeId.Equals("1"))
            ////{
            ////    string data = "notuse:" + amountValue + ":" + DateTime.Now.ToString("dd/MM/yyyy");
            ////    SendSMS(shopId, "btnPayment", conn, data);
            ////}
            ////else
            ////{
            ////    string data = "notuse:" + amountValue + ":" + DateTime.Now.ToString("dd/MM/yyyy");
            ////    SendSMS(shopId, "btnChequeCollection", conn, data);
            ////}
        }


        private void UpdateExpenseSync(string userId, string expenseIdSet, string amountSet, string remarks, string latitude
           , string longitude, string processName, string mobileTransactionDate, string mobileReferenceNo
           , string gpsSource, string serverSyncDate, string mobileSyncDate, string expenseDate, string field1
            , string field2, string field3, string field4, string field5, string uniqueKey)
        {

            if (userId != "" && expenseIdSet != null && amountSet != "")
            {
                ExpenseBO expenseBO = new ExpenseBO();
                expenseBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                expenseBO.UserId = Convert.ToInt32(userIdInt ?? "0");
                expenseBO.Parameters = expenseIdSet ?? "";
                expenseBO.Quantity = amountSet ?? "";
                expenseBO.Mode = "2";
                expenseBO.Latitude = latitude;
                expenseBO.Longitude = longitude;
                expenseBO.ProcessName = processName;
                expenseBO.MobileTransactionDate = mobileTransactionDate;
                expenseBO.mobilereferenceNo = mobileReferenceNo;
                expenseBO.Remarks = remarks;
                expenseBO.GpsSource = Convert.ToInt32(gpsSource);
                expenseBO.MobileSyncDate = mobileSyncDate;
                expenseBO.ServerSyncDate = serverSyncDate;
                expenseBO.ExpenseDate = expenseDate;
                expenseBO.Field1 = field1;
                expenseBO.Field2 = field2;
                expenseBO.Field3 = field3;
                expenseBO.Field4 = field4;
                expenseBO.Field5 = field5;
                expenseBO.UniqueKey = uniqueKey;
                expenseBO.UpdateExpense();
            }
        }

        private void SendSMSVisitNote(string shopId, string ProcessName, string connectionstring, string returnData)
        {

            ////string[] returnData1 = returnData.Split(':');
            ////string phoneNo = shopId;
            ////List<string> lstSmsArguments = new List<string>();
            ////try
            ////{
            ////    SMSConfigBO sMSConfigBO = new SMSConfigBO();
            ////    sMSConfigBO.ProcessName = ProcessName;
            ////    sMSConfigBO.ActivityId = returnData1[1];
            ////    sMSConfigBO.ConString = connectionstring;
            ////    DataSet ds = sMSConfigBO.getSmsConfiguration();
            ////    foreach (DataRow dr in ds.Tables[0].Rows)
            ////    {
            ////        string smsFormat = dr["Message"].ToString();
            ////        string msg = "";
            ////        if (String.Compare(ProcessName, "btnEnquiry") == 0)
            ////        {
            ////            msg = string.Format(smsFormat ?? "", returnData1[0], returnData1[2], returnData1[3]);
            ////        }
            ////        lstSmsArguments.Add(msg);
            ////        lstSmsArguments.Add(phoneNo);
            ////        lstSmsArguments.Add(dr["SMSUrl"].ToString());
            ////        lstSmsArguments.Add(dr["username"].ToString());
            ////        lstSmsArguments.Add(dr["password"].ToString());
            ////        lstSmsArguments.Add(dr["sender"].ToString());
            ////        lstSmsArguments.Add(dr["cdmasender"].ToString());
            ////        lstSmsArguments.Add(dr["key"].ToString());

            ////    }
            ////}
            ////catch (Exception ex)
            ////{
            ////    ////////_log.LogMessage("SMS Sending()", ex.ToString(), "L2");
            ////}
            ////SMS.SendSMS(lstSmsArguments);
        }

        private void UpdateParametersSync(string userid, string shopid, string productid, string quantitydata
            , string parameters, string latitude, string longitude, string processName
            , string mobileTransactionDate, string mobRefNo, string gpsSource, string serverSyncDate, string mobileSyncDate, string signalStrength, string networkProvider)
        {

            if (shopid != "" && productid != "" && productid != null && quantitydata != "" && parameters != "")
            {
                CompetitorBO competitorBO = new CompetitorBO();
                competitorBO.ConString = GetConnectionString(userid.Split('@')[0]);
                string useridInt = userid.Split('@')[1];
                competitorBO.ShopId = Convert.ToInt32(shopid ?? "0");
                competitorBO.UserId = Convert.ToInt32(useridInt ?? "0");
                competitorBO.ProductId = Convert.ToInt32(productid ?? "0");
                competitorBO.ParameterList = parameters ?? "";
                competitorBO.QuantityData = quantitydata ?? "";
                competitorBO.Latitude = latitude;
                competitorBO.Longitude = longitude;
                competitorBO.GpsSource = Convert.ToInt32(gpsSource);
                competitorBO.ProcessName = processName;
                competitorBO.MobileTransactionDate = mobileTransactionDate;
                competitorBO.ServerSyncDate = serverSyncDate;
                competitorBO.MobileSyncDate = mobileSyncDate;
                competitorBO.MobRefNo = mobRefNo;
                competitorBO.signalStrength = signalStrength;
                competitorBO.networkProvider = networkProvider;
                competitorBO.UpdateParameters();
            }
        }


        private void UpdateSalesReturnDataSync(string userCode, string shopId, string productAttributeId, string quantity, string price
            , string batchNo, string pkdDate, string latitude, string longitude, string processName
            , string mobileTransactionDate, string mobileReferenceNo, string rate, string reason, string gpsSource
            , string serverSyncDate, string mobileSyncDate, string unitData, string recieptNo, string schemeId, string signalStrength, string networkProvider, string remark)
        {

            if (userCode != "" && shopId != null && price != null && quantity != "") // Must fields are checked here.
            {
                SalesReturnBO salesReturnBO = new SalesReturnBO();
                salesReturnBO.ConString = GetConnectionString(userCode.Split('@')[0]);

                string userId = userCode.Split('@')[1];
                salesReturnBO.Mode = "1";
                salesReturnBO.UserId = Convert.ToInt32(userId);
                salesReturnBO.ShopId = Convert.ToInt32(shopId);
                salesReturnBO.Quantity = Convert.ToInt32(quantity);
                salesReturnBO.Amount = Convert.ToSingle(price);
                salesReturnBO.UnitId = Convert.ToInt32(unitData);
                salesReturnBO.ProductAttributeId = Convert.ToInt32(productAttributeId);
                salesReturnBO.ReceiptNo = recieptNo;
                salesReturnBO.signalStrength = signalStrength;
                salesReturnBO.networkProvider = networkProvider;
                if (remark == "")
                {
                    salesReturnBO.Remark = string.Empty;
                }
                else
                {
                    salesReturnBO.Remark = remark;
                }
                if (reason == "")
                {
                    salesReturnBO.returnReason = string.Empty;
                }
                else
                {
                    salesReturnBO.returnReason = reason;
                }
                try
                {
                    salesReturnBO.Rate = Convert.ToDouble(rate);
                }
                catch (Exception e)
                {
                    salesReturnBO.Rate = 0;
                }
                if (!batchNo.Equals("0"))
                {
                    salesReturnBO.BatchNo = batchNo;
                }
                else
                {
                    salesReturnBO.BatchNo = "";
                }
                if (pkdDate != "")
                {
                    salesReturnBO.PkdDate = pkdDate;
                }
                else
                {
                    salesReturnBO.PkdDate = string.Empty;
                }
                if (schemeId == "")
                {
                    salesReturnBO.SchemeId = 0;
                }
                else
                {
                    salesReturnBO.SchemeId = Convert.ToInt32(schemeId);
                }
                salesReturnBO.Latitude = latitude;
                salesReturnBO.Longitude = longitude;
                salesReturnBO.ProcessName = processName;
                salesReturnBO.MobileTransactionDate = mobileTransactionDate;
                salesReturnBO.mobileReferenceNo = mobileReferenceNo;
                salesReturnBO.GpsSouce = Convert.ToInt32(gpsSource);
                salesReturnBO.ServerSyncDate = serverSyncDate;
                salesReturnBO.MobileSyncDate = mobileSyncDate;
                salesReturnBO.UpdateSalesReturn();
            }

        }


        private void UpdateComplaintSync(string userId, string shopid, string remarks, string complaintid
            , string latitude, string longitude, string processName, string mobileTransactionDate, string mobRefNo, string gpsSource, string mobileSyncDate, string mobileDate, string signalStrength, string networkProvider)
        {



            if (shopid != "" && remarks != null && complaintid != "")
            {
                ComplaintEntryBO complaintBO = new ComplaintEntryBO();
                complaintBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                complaintBO.ReportedBy = Convert.ToInt32(userIdInt ?? "0");
                complaintBO.ShopId = Convert.ToInt32(shopid ?? "0");
                complaintBO.Complaint = remarks ?? "";
                complaintBO.ComplaintId = Convert.ToInt32(complaintid ?? "0");
                complaintBO.Latitude = latitude;
                complaintBO.Longitude = longitude;
                complaintBO.ProcessName = processName;
                complaintBO.mobileTransactionDate = mobileTransactionDate;
                complaintBO.mobileReferenceNo = mobRefNo;
                complaintBO.signalStrength = signalStrength;
                complaintBO.networkProvider = networkProvider;
                try
                {
                    complaintBO.GpsSource = Convert.ToInt32(gpsSource);
                }
                catch (Exception ex)
                {
                    complaintBO.GpsSource = 1;
                }
                complaintBO.mobileSyncDate = mobileSyncDate;
                complaintBO.mobileDate = mobileDate;
                complaintBO.UpdateComplaint();
            }
        }



        public void UpdateJointWorkingLoginDataSync(string userCode, string ShopInId, string ShopInTime, string ShopOutId,
            string ShopOutTime, string JointWorkUserId, string MobileTransactionDate, string MobileReferenceNo,
            string BeatPlanId, string ShopName, string mobileSyncDate, string BeatPlanDeviationReasonId)
        {
            try
            {
                if (userCode != "")
                {
                    JointWorkShopInAndOutBO jointWorkshopInAndOutBO = new JointWorkShopInAndOutBO();
                    jointWorkshopInAndOutBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                    string userIdVal = userCode.Split('@')[1];
                    jointWorkshopInAndOutBO.UserId = Convert.ToInt32(userIdVal ?? "0");
                    jointWorkshopInAndOutBO.ShopInId = ShopInId;
                    jointWorkshopInAndOutBO.ShopInTime = ShopInTime;
                    jointWorkshopInAndOutBO.ShopOutId = ShopOutId;
                    jointWorkshopInAndOutBO.ShopOutTime = ShopOutTime;
                    jointWorkshopInAndOutBO.JointWorkUserId = JointWorkUserId;
                    jointWorkshopInAndOutBO.MobileReferenceNo = MobileReferenceNo;
                    jointWorkshopInAndOutBO.MobileTransactionDate = MobileTransactionDate;
                    jointWorkshopInAndOutBO.BeatPlanId = BeatPlanId;
                    jointWorkshopInAndOutBO.ShopName = ShopName;
                    jointWorkshopInAndOutBO.MobileSyncDate = mobileSyncDate;
                    jointWorkshopInAndOutBO.BeatPlanDeviationReasionId = BeatPlanDeviationReasonId;

                    jointWorkshopInAndOutBO.updateJointWorkingShopInDetails();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void UpdateJointWorkingDataSync(string userCode, string BeatPlanActivityConfigsId,
           string SurveyGroupId, string ShopId, string ShopName, string QuestionId, string AnswerId,
           string Remarks, string MobileTransactionDate, string MobileReferenceNo, string BeatPlanId, string Latitude,
            string Longitude, string Source, string mobileSyncDate, string JointWorkUserId)
        {
            if (userCode != "")
            {
                JointWorkShopInAndOutBO jointWorkshopInAndOutBO = new JointWorkShopInAndOutBO();
                jointWorkshopInAndOutBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                string userIdVal = userCode.Split('@')[1];
                jointWorkshopInAndOutBO.UserId = Convert.ToInt32(userIdVal ?? "0");
                jointWorkshopInAndOutBO.BeatPlanActivityConfigsId = BeatPlanActivityConfigsId;
                jointWorkshopInAndOutBO.SurveyGroupId = SurveyGroupId;
                jointWorkshopInAndOutBO.ShopId = ShopId;
                jointWorkshopInAndOutBO.QuestionId = QuestionId;
                jointWorkshopInAndOutBO.AnswerId = AnswerId;
                jointWorkshopInAndOutBO.Remarks = Remarks;
                jointWorkshopInAndOutBO.MobileTransactionDate = MobileTransactionDate;
                jointWorkshopInAndOutBO.MobileReferenceNo = MobileReferenceNo;
                jointWorkshopInAndOutBO.BeatPlanId = BeatPlanId;
                jointWorkshopInAndOutBO.ShopName = ShopName;
                jointWorkshopInAndOutBO.Latitude = Latitude;
                jointWorkshopInAndOutBO.Longitude = Longitude;
                jointWorkshopInAndOutBO.GpsSource = Source;
                jointWorkshopInAndOutBO.MobileSyncDate = mobileSyncDate;
                jointWorkshopInAndOutBO.JointWorkUserId = JointWorkUserId;

                jointWorkshopInAndOutBO.updateJointWorkingShopInDetailsOffline();

            }
        }



        public void UpdateRemittanceSync(string userId, string amount, string bankId, string latitude, string longitude
             , string processName, string mobileTransactionDate, string mobRefNo, string gpsSource, string serverSyncDate, string mobileSyncDate
             , string denominationIds, string denominationCounts, string approvedBy, string remarks)
        {

            RemittanceBO remittanceBO = new RemittanceBO();
            remittanceBO.ConString = GetConnectionString(userId.Split('@')[0]);
            string userIdInt = userId.Split('@')[1];
            remittanceBO.UserId = Convert.ToInt32(userIdInt ?? "0");

            remittanceBO.Amount = amount ?? "";
            remittanceBO.BankId = Convert.ToInt32(bankId ?? "0");
            remittanceBO.Latitude = latitude;
            remittanceBO.Longitude = longitude;
            remittanceBO.ProcessName = processName;
            remittanceBO.MobileTransactionDate = mobileTransactionDate;
            remittanceBO.MobRefNo = mobRefNo;
            remittanceBO.GpsSource = Convert.ToInt32(gpsSource);
            remittanceBO.ServerSyncDate = serverSyncDate;
            remittanceBO.MobileSyncDate = mobileSyncDate;
            remittanceBO.Remarks = remarks;
            if (!string.IsNullOrEmpty(denominationIds) && !string.IsNullOrEmpty(denominationCounts))
            {
                remittanceBO.DenominationIds = denominationIds;
                remittanceBO.Denominations = denominationCounts;
            }
            remittanceBO.ApprovedBy = approvedBy;
            remittanceBO.UpdateRemittance();
        }


        public void updatePOPSync(string userId, string idSet, string quantitySet, string processName, string latitude, string longitude, string shopId, string mobileTransactionDate, string mobRefNo, string remarkSet, string serverSyncDate, string mobileSyncDate)
        {
            PopBO popBO = new PopBO();
            popBO.ConString = GetConnectionString(userId.Split('@')[0]);
            string userIdInt = userId.Split('@')[1];
            popBO.userId = Convert.ToInt32(userIdInt);
            popBO.popIdSet = idSet;
            popBO.processName = processName;
            popBO.quantitySet = quantitySet;
            popBO.lattitude = latitude;
            popBO.longitude = longitude;
            popBO.shopId = shopId;
            popBO.mobileTransactionDate = mobileTransactionDate;
            popBO.mobRefNo = mobRefNo;
            popBO.remarkSet = remarkSet;
            popBO.ServerSyncDate = serverSyncDate;
            popBO.MobileSyncDate = mobileSyncDate;
            popBO.Mode = 2;
            popBO.updatePop();
        }


        public void updateWorkingAreaSync(string userId, string mobileCaptureTime, string workingAreaMasterId, string latitude
            , string longitude, string processName, string mobRefNo, string gpsSource, string serverSyncDate, string mobileSyncDate, string signalStrength, string networkProvider)
        {

            WorkingAreaBO workingAreaBO = new WorkingAreaBO();
            workingAreaBO.ConString = GetConnectionString(userId.Split('@')[0]);
            string userIdInt = userId.Split('@')[1];
            workingAreaBO.userId = Convert.ToInt32(userIdInt ?? "0");

            workingAreaBO.workingAreaMasterId = Convert.ToInt32(workingAreaMasterId);
            workingAreaBO.mobileCaptureDate = Convert.ToDateTime(mobileCaptureTime);
            workingAreaBO.GpsSource = Convert.ToInt32(gpsSource);
            workingAreaBO.Latitude = latitude;
            workingAreaBO.Longitude = longitude;
            workingAreaBO.ProcessName = processName;
            workingAreaBO.MobRefNo = mobRefNo;
            workingAreaBO.ServerSyncDate = serverSyncDate;
            workingAreaBO.MobileSyncDate = mobileSyncDate;
            workingAreaBO.signalStrength = signalStrength;
            workingAreaBO.networkProvider = networkProvider;
            workingAreaBO.UpdateWorkArea();
        }


        public void updateMobileFeedbackSync(string userId, string mobileCaptureTime, string ansIdSet
           , string ansSet, string latitude, string longitude, string processName, string mobRefNo, string gpsSource, string serverSyncDate, string mobileSyncDate)
        {

            MobileFeedbackBO mobileFeedbackBO = new MobileFeedbackBO();
            mobileFeedbackBO.ConString = GetConnectionString(userId.Split('@')[0]);
            string userIdInt = userId.Split('@')[1];
            mobileFeedbackBO.userId = Convert.ToInt32(userIdInt ?? "0");

            mobileFeedbackBO.selectedAnsIdSet = ansIdSet;
            mobileFeedbackBO.selectedAnsSet = ansSet;
            mobileFeedbackBO.mobileCaptureDate = Convert.ToDateTime(mobileCaptureTime);

            mobileFeedbackBO.Latitude = latitude;
            mobileFeedbackBO.Longitude = longitude;
            mobileFeedbackBO.ProcessName = processName;
            mobileFeedbackBO.MobRefNo = mobRefNo;
            mobileFeedbackBO.GpsSource = Convert.ToInt32(gpsSource);
            mobileFeedbackBO.ServerSyncDate = serverSyncDate;
            mobileFeedbackBO.MobileSyncDate = mobileSyncDate;
            mobileFeedbackBO.UpdateMobileFeedback();
        }


        public void updateLeaveSync(string userId, string leaveReasonId, string fromDate, string toDate, string latitude
            , string longitude, string processName, string mobileCaptureTime, string mobRefNo, string gpsSource, string serverSyncDate, string mobileSyncDate
            , string remarks, string leaveSessionFromId, string leaveSessionToId)
        {

            LeaveBO leaveBO = new LeaveBO();
            leaveBO.ConString = GetConnectionString(userId.Split('@')[0]);
            string userIdInt = userId.Split('@')[1];
            leaveBO.userId = Convert.ToInt32(userIdInt ?? "0");
            leaveBO.resonId = Convert.ToInt32(leaveReasonId ?? "0");
            leaveBO.leaveFrom = Convert.ToDateTime(fromDate);
            leaveBO.leaveTo = Convert.ToDateTime(toDate);
            leaveBO.mobileCaptureDate = mobileCaptureTime;
            leaveBO.Latitude = latitude;
            leaveBO.Longitude = longitude;
            leaveBO.ProcessName = processName;
            leaveBO.MobRefNo = mobRefNo;
            leaveBO.GpsSource = Convert.ToInt16(gpsSource);
            leaveBO.ServerSyncDate = serverSyncDate;
            leaveBO.MobileSyncDate = mobileSyncDate;
            leaveBO.Remarks = remarks;
            leaveBO.LeaveSessionIdFrom = leaveSessionFromId;
            leaveBO.LeaveSessionIdTo = leaveSessionToId;
            leaveBO.UpdateLeaveRequest();
        }


        public void UpdateShopInAndOutSync(string userId, string shopId, string shopIn, string shopOut, string latitude, string longitude, string processName, string gpsSource, string mobileReferenceNo, string mobileTransactionDate, string serverSyncDate, string mobileSyncDate, string signalStrength, string networkProvider
            , string isGpsForciblyEnabled)
        {
            if (shopId != "" && userId != "")
            {
                ShopInAndOutBO shopInAndOutBO = new ShopInAndOutBO();
                shopInAndOutBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdVal = userId.Split('@')[1];
                shopInAndOutBO.ShopId = Convert.ToInt32(shopId ?? "0");
                shopInAndOutBO.UserId = Convert.ToInt32(userIdVal ?? "0");
                shopInAndOutBO.ShopIn = shopIn;
                shopInAndOutBO.ShopOut = shopOut;
                shopInAndOutBO.Latitude = latitude;
                shopInAndOutBO.Longitude = longitude;
                shopInAndOutBO.ProcessName = processName;
                shopInAndOutBO.gpsSource = Convert.ToInt32(gpsSource);
                shopInAndOutBO.MobileReferenceNo = mobileReferenceNo;
                shopInAndOutBO.ServerSyncDate = serverSyncDate;
                shopInAndOutBO.MobileSyncDate = mobileSyncDate;
                shopInAndOutBO.MobileTransactionDate = mobileTransactionDate;
                shopInAndOutBO.signalStrength = signalStrength;
                shopInAndOutBO.networkProvider = networkProvider;
                shopInAndOutBO.IsGpsForciblyEnabled = isGpsForciblyEnabled;
                shopInAndOutBO.UpdateShopInAndOut();
            }
        }

        public void UpdateSignatureSync(string userId, string shopId, string data, string shopkeeperName
            , string latitude, string longitude, string processName, string gpsSource, string mobilereferenceNo, string processId, string serverSyncDate, string mobileSyncDate)
        {
            if (userId != "" && shopId != "" && data != "" && shopkeeperName != "")
            {
                SignatureBO signatureBO = new SignatureBO();
                signatureBO.ConString = GetConnectionString(userId.Split('@')[0]);
                signatureBO.ProcessId = processId;
                signatureBO.ProcessName = signatureBO.GetProcessName();
                userId = userId.Split('@')[1];
                signatureBO.UserId = Convert.ToInt32(userId);
                signatureBO.ShopId = Convert.ToInt32(shopId);
                signatureBO.Data = data;
                signatureBO.Shopkeepername = shopkeeperName;
                signatureBO.Latitude = latitude;
                signatureBO.Longitude = longitude;
                //signatureBO.ProcessName = processName;
                signatureBO.MobileReferenceNo = mobilereferenceNo;
                //signatureBO.GpsSource = Convert.ToInt32(gpsSource);
                signatureBO.ServerSyncDate = serverSyncDate;
                signatureBO.MobileSyncDate = mobileSyncDate;
                signatureBO.UpdateSignature();

            }
        }


        public void UpdatePromotionalActivitiesSync(string userId, string shopId, string qn, string ans,
           string freeText, string dateSet, string textSet, string numberSet, string imageDataSet, string latitude
           , string longitude, string processName, string mobileTransactionDate, string mobileReferenceNo, string gpsSource, string serverSyncDate, string mobileSyncDate, string signalStrength, string networkProvider)
        {

            if (userId != "" && shopId != null && qn != "" && ans != "")
            {
                PromotionalActivitiesBO prmBO = new PromotionalActivitiesBO();
                prmBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                prmBO.UserId = Convert.ToInt32(userIdInt);
                prmBO.ShopId = Convert.ToInt32(shopId);
                prmBO.Mode = 2;
                prmBO.QuestionId = qn;
                prmBO.AnswerId = ans;
                prmBO.ResultDescription = freeText;
                prmBO.Latitude = latitude;
                prmBO.Longitude = longitude;
                prmBO.ProcessName = processName;
                prmBO.DateSet = dateSet;
                prmBO.TextSet = textSet;
                prmBO.NumberSet = numberSet;
                prmBO.ImageDataSet = imageDataSet;
                prmBO.MobileTransactionDate = mobileTransactionDate;
                prmBO.MobileReferenceNo = mobileReferenceNo;
                prmBO.GpsSource = Convert.ToInt32(gpsSource);
                prmBO.ServerSyncDate = serverSyncDate;
                prmBO.MobileSyncDate = mobileSyncDate;
                prmBO.signalStrength = signalStrength;
                prmBO.networkProvider = networkProvider;
                prmBO.UpdatePromotionalActivities();
            }
        }


        public void UpdateTransactionResultsSync(string userId, string shopId, string qn, string ans,
           string freeText, string dateSet, string textSet, string numberSet, string imageDataSet, string latitude
           , string longitude, string processName, string mobileTransactionDate, string mobileReferenceNo, string gpsSource, string serverSyncDate, string mobileSyncDate, string signalStrength, string networkProvider)
        {

            if (userId != "" && shopId != null && qn != "" && ans != "")
            {
                TransactionResultBO transactionBO = new TransactionResultBO();
                transactionBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                transactionBO.UserId = Convert.ToInt32(userIdInt);
                transactionBO.ShopId = Convert.ToInt32(shopId);
                transactionBO.Mode = 2;
                transactionBO.QuestionId = qn;
                transactionBO.AnswerId = ans;
                transactionBO.ResultDescription = freeText;
                transactionBO.Latitude = latitude;
                transactionBO.Longitude = longitude;
                transactionBO.ProcessName = processName;
                transactionBO.DateSet = dateSet;
                transactionBO.TextSet = textSet;
                transactionBO.NumberSet = numberSet;
                transactionBO.ImageDataSet = imageDataSet;
                transactionBO.MobileTransactionDate = mobileTransactionDate;
                transactionBO.MobileReferenceNo = mobileReferenceNo;
                transactionBO.GpsSource = Convert.ToInt32(gpsSource);
                transactionBO.ServerSyncDate = serverSyncDate;
                transactionBO.MobileSyncDate = mobileSyncDate;
                transactionBO.signalStrength = signalStrength;
                transactionBO.networkProvider = networkProvider;
                transactionBO.UpdateTransactionResults();
            }
        }


        public void PunchSync(string userId, string punchInTime, string punchOutTime, string startReading, string endReading, string latitude
            , string longitude, string processName, string mobileRefNo, string gpsSource, string serverSyncDate, string mobileSyncDate, string travelMode, string travelModeAnswer)
        {
            MobilePunchingBO punchingBO = new MobilePunchingBO();
            punchingBO.ConString = GetConnectionString(userId.Split('@')[0]);
            string userIdInt = userId.Split('@')[1];
            punchingBO.userId = Convert.ToInt32(userIdInt ?? "0");

            if (startReading != "")
            {
                punchingBO.OdometerReading = startReading;
            }
            else
            {
                punchingBO.OdometerReading = endReading;
            }

            punchingBO.punchInTime = punchInTime;
            punchingBO.punchOutTime = punchOutTime;
            punchingBO.MobileRefNo = mobileRefNo;
            punchingBO.Latitude = latitude;
            punchingBO.Longitude = longitude;
            punchingBO.ProcessName = processName;
            punchingBO.GpsSource = Convert.ToInt32(gpsSource);
            punchingBO.ServerSyncDate = serverSyncDate;
            punchingBO.MobileSyncDate = mobileSyncDate;
            punchingBO.TravelModeId = travelMode;
            punchingBO.TravelModeAnswer = travelModeAnswer;
            punchingBO.UpdatePunch();
        }


        public int updateMonthlyTourPlanSync(string userCode, string userId, string TourPlanDate, string SubmissionRemark, string SubmissionDate, string SubmittedBy, string mobileReferenceNo, string mobileTransactionDate,
                                         string latitude, string longitude, string source, string SignalStrength, string NetworkProvider, string mobileSyncDate, string mobileDate)
        {
            //int productIdInt;
            try
            {
                TourPlanBO TourPlanBO = new TourPlanBO();
                TourPlanBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                TourPlanBO.UserId = Convert.ToInt32(userId);
                TourPlanBO.TourPlanDate = Convert.ToDateTime(TourPlanDate);
                TourPlanBO.SubmissionRemark = SubmissionRemark;
                TourPlanBO.SubmissionDate = Convert.ToDateTime(SubmissionDate);
                TourPlanBO.SubmittedBy = SubmittedBy;
                //TourPlanBO.Remarks = remarks ?? "";
                TourPlanBO.Latitude = latitude;
                TourPlanBO.Longitude = longitude;
                //TourPlanBO.ProcessName = processName;
                TourPlanBO.MobileTransactionDate = Convert.ToDateTime(mobileTransactionDate);
                TourPlanBO.MobileReferenceNo = mobileReferenceNo;
                TourPlanBO.GpsSource = Convert.ToInt32(source);
                TourPlanBO.MobileDate = mobileDate;
                TourPlanBO.MobileSyncDate = Convert.ToDateTime(mobileSyncDate);
                TourPlanBO.signalStrength = SignalStrength;
                TourPlanBO.networkProvider = NetworkProvider;
                int monthlyTourPlanId = TourPlanBO.UpdateMonthlyTourPlan();
                return monthlyTourPlanId;

            }
            catch (IndexOutOfRangeException ex)
            {
                ////_log.LogMessage("UpdateMonthlyTourPlan()", ex.ToString(), "L2");
                throw ex;
            }
            catch (NullReferenceException ex)
            {
                ////_log.LogMessage("UpdateMonthlyTourPlan()", ex.ToString(), "L2");
                throw ex;
            }
            catch (Exception ex)
            {
                ////_log.LogMessage("UpdateMonthlyTourPlan()", ex.ToString(), "L2");
                throw ex;
            }
        }
        public void updateDailyTourPlanSync(string userCode, string mobileSyncDate, string mobileDate, int monthlyTourPlanId, string dailyUserId, string Date, string Status, string ActionById, string ActionDate, string dailyMobileReferenceNo, string dailyMobileTransactionDate, string RoutePlanData, string JointWorkData, string ActivityPlanData, string LeaveData,
           string leaveReasonId, string fromDate, string toDate, string leavelatitude, string leavelongitude, string processName, string mobileCaptureTime, string leavemobileReferenceNo, string gpsSource, string remarks, string LeaveFromSessionID, string LeaveToSessionID)
        {
            TourPlanBO TourPlanBO = new TourPlanBO();
            TourPlanBO.ConString = GetConnectionString(userCode.Split('@')[0]);
            TourPlanBO.monthlyTourPlanId = Convert.ToInt32(monthlyTourPlanId);
            TourPlanBO.dailyUserId = Convert.ToInt32(dailyUserId);
            TourPlanBO.Date = Convert.ToDateTime(Date);
            TourPlanBO.Status = Convert.ToInt32(Status);
            TourPlanBO.ActionById = Convert.ToInt32(ActionById);
            TourPlanBO.ActionDate = Convert.ToDateTime(ActionDate);
            TourPlanBO.dailyMobileReferenceNo = dailyMobileReferenceNo;
            TourPlanBO.dailyMobileTransactionDate = Convert.ToDateTime(dailyMobileTransactionDate);
            TourPlanBO.RoutePlanData = RoutePlanData;
            TourPlanBO.JointWorkData = JointWorkData;
            TourPlanBO.ActivityPlanData = ActivityPlanData;
            TourPlanBO.LeaveData = LeaveData;

            //TourPlanBO.userId = Convert.ToInt32(userIdInt ?? "0");
            TourPlanBO.resonId = 0;
            if (leaveReasonId != "")
            {
                TourPlanBO.resonId = Convert.ToInt32(leaveReasonId);
            }
            if (fromDate != "")
                TourPlanBO.leaveFrom = Convert.ToDateTime(fromDate);
            if (toDate != "")
                TourPlanBO.leaveTo = Convert.ToDateTime(toDate);
            if (mobileCaptureTime != "")
                TourPlanBO.mobileCaptureDate = Convert.ToDateTime(mobileCaptureTime);
            if (leavelatitude != "")
                TourPlanBO.LeaveLatitude = leavelatitude;
            if (leavelongitude != "")
                TourPlanBO.LeaveLongitude = leavelongitude;
            if (processName != "")
                TourPlanBO.LeaveProcessName = processName;
            if (leavemobileReferenceNo != "")
                TourPlanBO.MobRefNo = leavemobileReferenceNo;
            if (gpsSource != "")
                TourPlanBO.LeaveGpsSource = Convert.ToInt16(gpsSource);
            //TourPlanBO.ServerSyncDate = serverSyncDate;
            if (mobileSyncDate != "")
                TourPlanBO.LeaveMobileSyncDate = mobileSyncDate;
            if (remarks != "")
                TourPlanBO.Remarks = remarks;
            if (LeaveFromSessionID != "-1")
                TourPlanBO.LeaveSessionIdFrom = LeaveFromSessionID;
            if (LeaveToSessionID != "-1")
                TourPlanBO.LeaveSessionIdTo = LeaveToSessionID;

            TourPlanBO.UpdateDailyTourPlan();
        }

        public void UpdatePhotoCaptureSync(string data, string fileName, string userId, string latitude
         , string longitude, string processId, string imageDescription, string mobileReferenceNumber
         , string shopIdValue, string photoDescTypeId, string syncDate, string captureDate
         , string source, string imagePath, string processDetailsId, string tempShopId, string signalStrength, string networkProvider)
        {

            string[] user = userId.Split('@');
            int userIdVal = Convert.ToInt32(user[1]);
            PhotoCaptureBO photoCaptureBO = new PhotoCaptureBO();
            photoCaptureBO.ConString = GetConnectionString(user[0]);
            photoCaptureBO.UserId = userIdVal;
            photoCaptureBO.ShopId = shopIdValue;
            photoCaptureBO.ImageName = fileName;
            photoCaptureBO.Latitude = latitude;
            photoCaptureBO.Longitude = longitude;
            photoCaptureBO.ProcessId = Convert.ToInt32(processId);
            photoCaptureBO.ImageDescription = imageDescription;
            photoCaptureBO.DateTime = captureDate;
            photoCaptureBO.mobReferenceNumber = mobileReferenceNumber;
            photoCaptureBO.mode = 5;
            photoCaptureBO.SyncDate = syncDate;
            photoCaptureBO.GpsSource = Convert.ToInt32(source);
            photoCaptureBO.TempShopId = tempShopId;
            photoCaptureBO.signalStrength = signalStrength;
            photoCaptureBO.networkProvider = networkProvider;
            if (!string.IsNullOrEmpty(processDetailsId))
            {
                photoCaptureBO.ProcessDetailsId = Convert.ToInt32(processDetailsId);
            }

            try
            {
                photoCaptureBO.descriptionTypeId = Convert.ToInt32(photoDescTypeId);
            }
            catch (Exception e)
            {
                photoCaptureBO.descriptionTypeId = 0;
            }
            SaveImageToFolder(data, fileName, imagePath);
            photoCaptureBO.UpdatePhotoCapture();

        }
        private string SplitString(string productData, string stockData, string unit)
        {

            String[] ArrPid = productData.Split(',');
            String[] arrStock = stockData.Split(',');
            String[] arrUnit = unit.Split(',');
            String strpid = "", stockSet = "", unitSet = "";
            Boolean isAllFirst = true;
            for (int i = 0; i < ArrPid.Length; ++i)
            {
                if (isAllFirst)
                {
                    isAllFirst = false;
                    strpid = ArrPid[i];
                    stockSet = arrStock[i];
                    unitSet = arrUnit[i];
                }
                else
                {
                    if (!stockSet.Equals(""))
                    {
                        strpid += "," + ArrPid[i];
                        stockSet += "," + arrStock[i];
                        unitSet += "," + arrUnit[i];
                    }
                }
            }
            return strpid + ":" + stockSet + ":" + unitSet;
        }
        public void updateSalesPromotionSync(string userId, string shopId, string productData, string quantity
            , string product, string latitude, string longitude, string processName, string narration
            , string mobileTransactionDate, string unitId, string mobileSyncDate, string mobileReferenceNo
            , string source, string mobileDate, string signalStrength, string networkProvider)
        {
            if (userId != "" && shopId != null) // Must fields are checked here.
            {
                SalesPromotionBO salesPromotionBO = new SalesPromotionBO();
                salesPromotionBO.ConString = GetConnectionString(userId.Split('@')[0]);
                string userIdInt = userId.Split('@')[1];
                salesPromotionBO.UserId = Convert.ToInt32(userIdInt);
                salesPromotionBO.ShopId = Convert.ToInt32(shopId);
                salesPromotionBO.ProductData = productData;
                salesPromotionBO.quantity = quantity;
                salesPromotionBO.Latitude = latitude;
                salesPromotionBO.Longitude = longitude;
                salesPromotionBO.ProcessName = processName;
                salesPromotionBO.Narration = narration;
                salesPromotionBO.mobileTransactionDate = mobileTransactionDate;
                salesPromotionBO.UnitId = unitId;
                salesPromotionBO.SyncDate = mobileSyncDate;
                salesPromotionBO.mobileReferenceNo = mobileReferenceNo;
                salesPromotionBO.GpsSource = Convert.ToInt32(source);
                salesPromotionBO.mobileDate = mobileDate;
                salesPromotionBO.signalStrength = signalStrength;
                salesPromotionBO.networkProvider = networkProvider;
                salesPromotionBO.UpdateSalesPromotion();
                // ////Context.Response.Output.Write(Status.Success.ToString());
            }
        }

        public void UpdateOrderSync(string userId, string shopId, string unitSet, string productData, string quantitydata,
          string scheme, string mode, string specialInstruction, string otherInstns, string priority, string siteAddress
            , string contactPerson, string phone, string price, string mobileNo, string latitude, string longitude, string processName
            , string totalAmount, string firstDiscount, string secondDiscount, int count, string orderDate1, string bankName
            , string unitDiscount, string freeQty, string mobileDiscFlag, string orderDisc, string distributorId, string mobileReferenceNo
            , string mobileOrderDate, string orderIdVal, string orderDiscId, string orderDiscVal, string deliveryDate, string paymentMode
            , string source, string schemeFromMobile, string mobileDate, int requireERB, int requreSMSForOrder, string signalStrength
            , string requestedQuantityData, string tempShopId, string totalDiscountData, string taxAmountData, string networkProvider, string invoiceNo, int length)
        {
            try
            {

                string values = userId + "#" + shopId + "#" + unitSet + "#" + productData + "#" + quantitydata + "#" + scheme + "#" + mode + "#" + specialInstruction + "#" + otherInstns + "#" + priority + "#" + siteAddress + "#" + contactPerson + "#" + phone + "#" + price + "#" + mobileNo + "#" + latitude + "#" + longitude + "#" + processName + "#" + totalAmount + "#" + firstDiscount + "#" + secondDiscount + "#" + count + "#" + orderDate1 + "#" + bankName + "#" + unitDiscount + "#" + freeQty + "#" + mobileDiscFlag + "#" + orderDisc + "#" + distributorId + "#" + mobileReferenceNo + "#" + mobileOrderDate + "#" + orderIdVal + "#" + orderDiscId + "#" + orderDiscVal + "#" + deliveryDate + "#" + paymentMode + "#" + source + "#" + schemeFromMobile + "#" + mobileDate + "#" + requireERB + "#" + requreSMSForOrder + "#" + signalStrength + "#" + requestedQuantityData;
                ////_log.LogMessage("UpdateOrderSyncValues()", values, "L2");
                //////_log.LogMessage("UpdateOrderSync()", "Start : " + source, "L2");
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(shopId) && !string.IsNullOrEmpty(price)) // Must fields are checked here.
                {
                    OrderBO orderBO = new OrderBO();
                    String[] dataArr = null;
                    connectionString = GetConnectionString(userId.Split('@')[0]);


                    orderBO.ConString = connectionString;
                    userId = userId.Split('@')[1];
                    orderBO.UserId = Convert.ToInt32(userId);
                    orderBO.ShopId = Convert.ToInt32(shopId);
                    orderBO.Priority = priority;

                    orderBO.UnitData = unitSet;
                    orderBO.ProductData = productData;
                    orderBO.QuantityData = quantitydata;
                    orderBO.Rate = price;
                    orderBO.TotalAmount = totalAmount;
                    orderBO.FirstDiscount = firstDiscount;
                    orderBO.SecondDiscount = secondDiscount;
                    orderBO.schemeFromMobile = schemeFromMobile;
                    orderBO.SchemeId = Convert.ToInt32("0");
                    ////orderBO.CommandTimeout = commandTimeout;
                    orderBO.signalStrength = signalStrength;
                    orderBO.networkProvider = networkProvider;
                    if (mode.Equals("1"))
                        orderBO.Mode = "5";
                    else
                        orderBO.Mode = mode;
                    if (specialInstruction == "")
                    {
                        orderBO.SpecialInstnSet = "0";
                    }
                    else
                    {
                        orderBO.SpecialInstnSet = specialInstruction;
                    }
                    orderBO.OtherInstn = otherInstns;
                    orderBO.Priority = priority;
                    orderBO.SiteAddress = siteAddress;
                    orderBO.ContactPerson = contactPerson;
                    orderBO.Phone = phone;

                    orderBO.MobileNo = mobileNo;
                    orderBO.Latitude = latitude;
                    orderBO.Longitude = longitude;
                    orderBO.ProcessName = "btnOrderOnly";
                    orderBO.OrderDate = orderDate1;
                    orderBO.BankName = bankName;
                    orderBO.unitDiscount = unitDiscount;
                    orderBO.freeQuantity = freeQty;
                    orderBO.mobileDiscFlag = mobileDiscFlag;
                    orderBO.orderDisc = orderDisc;
                    orderBO.syncDate = orderDate1;
                    orderBO.mobileTransactionDate = mobileOrderDate;
                    orderBO.distributorId = distributorId;
                    orderBO.mobileReferenceNo = mobileReferenceNo;
                    orderBO.paymentReferenceNo = 0;
                    orderBO.userRef = string.Empty;
                    int orderId = 0;
                    string returnData = "";
                    orderBO.orderDiscountIds = orderDiscId;
                    orderBO.orderDiscountVals = orderDiscVal;
                    orderBO.mobileDate = mobileDate;
                    orderBO.RequestedQuantityData = requestedQuantityData;
                    orderBO.TempShopId = tempShopId;
                    orderBO.TotalDiscount = totalDiscountData;
                    orderBO.length = length;
                    orderBO.TaxAmount = taxAmountData;
                    try
                    {
                        orderBO.paymentMode = Convert.ToInt32(paymentMode);
                    }
                    catch (Exception ex)
                    {
                        orderBO.paymentMode = 0;
                        ////_log.LogMessage("UpdateOrder()", "orderBO.paymentMode ", "L2");
                    }
                    orderBO.deliveryDate = deliveryDate;
                    orderBO.source = source;
                    orderBO.InvoiceNo = invoiceNo;
                    returnData = orderBO.UpdateOrder();
                    string id = returnData.Split(':')[0];
                    try
                    {
                        orderId = Convert.ToInt32(id);
                    }
                    catch
                    {
                        orderId = 0;
                    }

                    sigHashTable.Add(orderIdVal, orderId);
                    if (requireERB == 1)
                    {
                        Thread thred = new Thread(() => { callSAP(orderBO.ConString, orderId); });
                        thred.Start();
                    }
                    if (requreSMSForOrder == 1)
                    {
                        double totAmt = 0;
                        returnData += ":" + DateTime.Now.ToString("dd/MM/yyyy");
                        Thread threadSMs = new Thread(() => { sendSMSOrder(returnData, shopId, orderBO.ConString, "btnOrderOnly"); });
                        threadSMs.Start();
                        string[] amtArray = totalAmount.Split(',');
                        foreach (string amt in amtArray)
                        {
                            double tmpAmt = 0;
                            try
                            {
                                tmpAmt = Convert.ToDouble(amt);

                            }
                            catch (Exception ex)
                            {
                                tmpAmt = 0;
                            }
                            totAmt += tmpAmt;
                        }
                        string mailContent = returnData.Split(':')[1] + ":" + DateTime.Now.ToString("dd/MM/yyyy") + ":" + totAmt + ":" + returnData.Split(':')[0];
                        Thread threadEmail = new Thread(() => { sendEmail("1", mailContent, shopId, orderBO.ConString, distributorId); });
                        threadEmail.Start();

                    }
                }
                else
                {
                    ////Context.Response.Output.Write(Status.AttributesNull.ToString());
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                ////_log.LogMessage("UpdateOrder()", ex.ToString(), "L2");
                ////Context.Response.Output.Write(Status.IndexOutOfRange.ToString());
            }
            catch (NullReferenceException ex)
            {
                ////_log.LogMessage("UpdateOrder()", ex.ToString(), "L2");
                ////Context.Response.Output.Write(Status.NullReferenceException.ToString());
            }
            catch (Exception ex)
            {
                ////_log.LogMessage("UpdateOrder()", ex.ToString(), "L2");
                ////Context.Response.Output.Write(Status.Error.ToString());
            }
        }

        public void updateDeliveryDetailsSync(string OrderId, string productIdSet, string deliveredQtySet, string invoiceQtySet
         , string billNo, string isClosed, string mobileTransactionDate, string userId, string shopId
         , string mobileReferenceNo, string unitIdSet, string schemeIdSet, string latitude, string longitude, string processName, string gpsSource, string signalStrength, string networkProvider
          , string ReturnReasonId)
        {
            DeliveryBO deliveryBO = new DeliveryBO();
            string[] user = userId.Split('@');
            deliveryBO.ConString = GetConnectionString(user[0]);
            deliveryBO.UserId = Convert.ToInt32(user[1]);
            deliveryBO.shopId = Convert.ToInt32(shopId);
            deliveryBO.mode = 2;
            deliveryBO.billNo = billNo;
            deliveryBO.productIdSet = productIdSet;
            deliveryBO.deliveryQuantitySet = deliveredQtySet;
            deliveryBO.invoiceQtySet = invoiceQtySet;
            deliveryBO.orderId = Convert.ToInt32(OrderId);
            deliveryBO.isClosed = Convert.ToInt32(isClosed);
            deliveryBO.productUnitSet = unitIdSet;
            deliveryBO.productSchemeSet = schemeIdSet;
            deliveryBO.mobileTransactionDate = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
            deliveryBO.mobileReferenceNo = mobileReferenceNo;
            deliveryBO.latitude = latitude;
            deliveryBO.longitude = longitude;
            deliveryBO.processName = processName;
            deliveryBO.gpsSource = Convert.ToInt32(gpsSource); ;
            deliveryBO.signalStrength = signalStrength;
            deliveryBO.networkProvider = networkProvider;
            deliveryBO.returnReasonId = ReturnReasonId;

            deliveryBO.UpdateDeliveryData();

        }


        public void updateWorkingWithSync(string userId, string shopId, string userIdSet, string processName, string latitude, string longitude
            , string mobileTransactionDate, string mobRefNo, string gpsSource, string serverSyncDate, string mobileSyncDate, string signalStrength, string networkProvider, string Others, string DepartmentIdSet)
        {
            WorkWithBO workingWithBO = new WorkWithBO();
            workingWithBO.ConString = GetConnectionString(userId.Split('@')[0]);
            string userIdInt = userId.Split('@')[1];
            workingWithBO.userId = Convert.ToInt32(userIdInt ?? "0");

            workingWithBO.userIdSet = userIdSet;
            workingWithBO.processName = processName;
            workingWithBO.mobileCaptureDate = Convert.ToDateTime(mobileTransactionDate);
            workingWithBO.lattitude = latitude;
            workingWithBO.longitude = longitude;
            workingWithBO.shopId = shopId;
            workingWithBO.mobRefNo = mobRefNo;
            workingWithBO.gpsSource = Convert.ToInt16(gpsSource);
            workingWithBO.ServerSyncDate = serverSyncDate;
            workingWithBO.MobileSyncDate = mobileSyncDate;
            workingWithBO.signalStrength = signalStrength;
            workingWithBO.networkProvider = networkProvider;
            workingWithBO.Others = Others;
            workingWithBO.DepartmentIdSet = DepartmentIdSet;
            workingWithBO.updateWorkWith();
        }



        public void UpdateEnquirySync(string userCode, string enquiredby, string activityId, string shopid, string remarks
            , string latitude, string longitude, string processName, string mobileTransactionDate
            , string mobileReferenceNo, string gpsSource, string mobileSyncDate, string mobileDate
            , string productId, string tempShopId, string signalStrength, string networkProvider)
        {
            int productIdInt;
            try
            {
                if (shopid != "" && enquiredby != "" && activityId != null)
                {
                    EnquiryBO enquiryBO = new EnquiryBO();
                    enquiryBO.ConString = GetConnectionString(userCode.Split('@')[0]);
                    string enquiredbyInt = enquiredby;
                    enquiryBO.ShopId = Convert.ToInt32(shopid ?? "0");
                    enquiryBO.UserId = Convert.ToInt32(enquiredbyInt ?? "0");
                    enquiryBO.ActivityId = Convert.ToInt32(activityId ?? "0");
                    enquiryBO.Remarks = remarks ?? "";
                    enquiryBO.Latitude = latitude;
                    enquiryBO.Longitude = longitude;
                    enquiryBO.ProcessName = processName;
                    enquiryBO.MobileTransactionDate = mobileTransactionDate;
                    enquiryBO.MobileReferenceNo = mobileReferenceNo;
                    enquiryBO.GpsSource = Convert.ToInt32(gpsSource);
                    enquiryBO.MobileDate = mobileDate;
                    enquiryBO.MobileSyncDate = mobileSyncDate;
                    enquiryBO.signalStrength = signalStrength;
                    enquiryBO.networkProvider = networkProvider;
                    try
                    {
                        productIdInt = Convert.ToInt32(productId);
                    }
                    catch (Exception e)
                    {
                        productIdInt = 0;
                    }
                    enquiryBO.ProductId = productIdInt;
                    enquiryBO.TempShopId = tempShopId;
                    enquiryBO.UpdateEnquiry();
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                ////_log.LogMessage("UpdateEnquirySync()", ex.ToString(), "L2");
                throw ex;
            }
            catch (NullReferenceException ex)
            {
                ////_log.LogMessage("UpdateEnquirySync()", ex.ToString(), "L2");
                throw ex;
            }
            catch (Exception ex)
            {
                ////_log.LogMessage("UpdateEnquirySync()", ex.ToString(), "L2");
                throw ex;
            }

        }

        public static bool SendEmail(string actionId, string mailContent, string shopMailId, string connectionString)
        {
            ErrorLogUtilities logfile = new ErrorLogUtilities();
            ////GridView grdSummary = new GridView();
            bool status = false;
            var conString = connectionString;
            string messageId = string.Empty;
            string emailSubject = string.Empty;
            string emailMessage = string.Empty;
            string emailTo = string.Empty;
            string emailCc = string.Empty;
            string emailBcc = string.Empty;
            string OrderId = mailContent.Split(':')[3];
            DataSet dsMessageDetails = GetMessageDetails(actionId);
            try
            {

                if (dsMessageDetails != null && dsMessageDetails.Tables[0].Rows.Count > 0)
                {
                    messageId = dsMessageDetails.Tables[0].Rows[0]["MessageId"].ToString();
                    emailSubject = dsMessageDetails.Tables[0].Rows[0]["Subject"].ToString();
                    emailMessage = dsMessageDetails.Tables[0].Rows[0]["Message"].ToString();
                    emailTo = dsMessageDetails.Tables[0].Rows[0]["EmailTo"].ToString();
                    emailCc = dsMessageDetails.Tables[0].Rows[0]["EmailCc"].ToString();
                    emailBcc = dsMessageDetails.Tables[0].Rows[0]["EmailBcc"].ToString();
                }
                StringBuilder orginalSubject = PrepareMessage(emailSubject, mailContent);
                StringBuilder orginalMessage = PrepareMessage(emailMessage, mailContent);

                String fromAddress = ConfigurationSettings.AppSettings["Email"].ToString();
                String password = ConfigurationSettings.AppSettings["Emailpassword"].ToString();
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                MailAddress mailFrom = new MailAddress(ConfigurationSettings.AppSettings["Email"].ToString());
                message.From = mailFrom;
                if (shopMailId.Trim().Length > 0)
                {
                    string[] toList = shopMailId.Split(';');
                    foreach (string strTo in toList)
                    {
                        if (strTo.Trim().Length > 0)
                        {
                            message.To.Add(strTo);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(emailTo))
                    {
                        string[] toList = emailTo.Split(',');
                        foreach (string strTo in toList)
                        {
                            message.To.Add(strTo);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(emailCc))
                {
                    string[] ccList = emailCc.Split(',');
                    foreach (string strCc in ccList)
                    {
                        message.CC.Add(strCc);
                    }
                }
                if (!string.IsNullOrEmpty(emailBcc))
                {
                    string[] bccList = emailBcc.Split(',');
                    foreach (string strBcc in bccList)
                    {
                        message.Bcc.Add(strBcc);
                    }
                }

                string user = string.Empty;
                string Distributor = string.Empty;
                string Value = string.Empty;
                string ShopName = string.Empty;
                string organization = string.Empty;
                DataTable dt = new DataTable();
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(connectionString);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "[uspOrderMailSending]";
                sqlHelper.AddParameter(command, "@OrderId", OrderId, ParameterDirection.Input);
                DataSet ds = sqlHelper.ExecuteDataSet(command);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    user = ds.Tables[0].Rows[0]["Name"].ToString();
                    Distributor = ds.Tables[0].Rows[0]["DistributorName"].ToString();
                    Value = ds.Tables[0].Rows[0]["OrderValue"].ToString();
                    ShopName = ds.Tables[0].Rows[0]["ShopName"].ToString();
                    organization = ds.Tables[2].Rows[0]["Organization"].ToString();
                    dt = ds.Tables[1];
                    ////grdSummary.Columns.Clear();
                    ////grdSummary.AutoGenerateColumns = false;
                    foreach (DataColumn c in dt.Columns)
                    {
                        ////BoundField boundField = new BoundField();

                        ////boundField.DataField = c.ColumnName;
                        ////boundField.HeaderText = c.ColumnName;

                        ////boundField.ItemStyle.BorderColor = System.Drawing.Color.LightGray;
                        ////boundField.HeaderStyle.BorderColor = System.Drawing.Color.Gray;
                        ////boundField.HeaderStyle.Width = 750;
                        ////// boundField.ItemStyle.Font.Size = 12;
                        ////grdSummary.Columns.Add(boundField);
                    }

                    ////grdSummary.Style.Add("font-family", "Verdana");
                    ////grdSummary.Style.Add("font-Size", "12");

                    ////grdSummary.HeaderStyle.BackColor = System.Drawing.Color.LightGray;
                    ////grdSummary.AlternatingRowStyle.BackColor = System.Drawing.Color.AliceBlue;
                    ////grdSummary.RowStyle.BackColor = System.Drawing.Color.White;
                    ////grdSummary.DataSource = dt;
                    ////grdSummary.DataBind();

                    ////message.IsBodyHtml = true;
                    ////string body = Email.PopulateBody(user, Distributor, Value, ShopName, organization, GetData(grdSummary));
                    ////ErrorLogUtilities log = new ErrorLogUtilities();
                    ////log.LogMessage("SendEmail()", body, "L2");
                    ////message.Subject = orginalSubject.ToString();
                    ////message.Body = body;


                    ////Configuration config = WebConfigurationManager.OpenWebConfiguration("~/web.config");
                    ////MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
                    ////smtpClient.Host = settings.Smtp.Network.Host;
                    ////smtpClient.Port = settings.Smtp.Network.Port;
                    ////smtpClient.UseDefaultCredentials = settings.Smtp.Network.DefaultCredentials;
                    ////smtpClient.Credentials = new System.Net.NetworkCredential(settings.Smtp.Network.UserName, settings.Smtp.Network.Password);
                    ////smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationSettings.AppSettings["EnableSslForSMTP"]);
                    ////smtpClient.Send(message);
                    status = true;
                }
                if (status)
                {
                    status = InsertEmailTransaction(Convert.ToInt32(messageId), orginalMessage.ToString(), emailTo, emailCc, emailBcc);
                }
            }
            catch (Exception ex)
            {
                logfile.LogMessage("Send Email", "" + "Exception : " + ex.Message + ":" + ex.Source, "L2");
                status = false;
            }
            return status;
        }

        public static StringBuilder PrepareMessage(string message, string Values)
        {
            string[] ValuesList = Values.Split(':');
            StringBuilder orginalMessage = new StringBuilder();
            string msg = string.Format(message ?? "", " ", ValuesList[0], ValuesList[1], ValuesList[2]);
            orginalMessage.Append(msg);
            return orginalMessage;
        }
        /// <summary>
        /// pass the message template this function will return Orginal Message (No changes in this class)
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Orginal Message</returns>
        public static StringBuilder GetMessage(string message, string orderId)
        {
            StringBuilder orginalMessage = new StringBuilder();
            string verable;
            int bracketOpeningIndex = 0;
            int bracketClosingIndex = 0;
            do
            {
                bracketOpeningIndex = message.IndexOf("{", bracketClosingIndex);
                if (bracketOpeningIndex > 0)
                {
                    if (orginalMessage.Length == 0)
                    {
                        orginalMessage.Append(message.Substring(bracketClosingIndex, (bracketOpeningIndex - bracketClosingIndex)));
                    }
                    else
                    {
                        orginalMessage.Append(message.Substring(bracketClosingIndex + 1, ((bracketOpeningIndex - 1) - bracketClosingIndex)));
                    }
                }
                if (bracketOpeningIndex < 0)
                {
                    orginalMessage.Append(message.Substring((bracketClosingIndex + 1), (message.Length - (bracketClosingIndex + 1))));
                    break;
                }
                bracketClosingIndex = message.IndexOf("}", bracketOpeningIndex);
                verable = message.Substring(bracketOpeningIndex, bracketClosingIndex - bracketOpeningIndex + 1);
                orginalMessage.Append(VeriableValue(verable, orderId));
            } while (true);
            return orginalMessage;//will return the orginal message;
        }

        /// <summary>
        /// This function will return parameter value EX:- is we pass "{UserName}" this function will return "Shinu"
        /// </summary>
        /// <param name="veriable"></param>
        /// <returns></returns>
        public static string VeriableValue(string veriable, string orderId)
        {
            var conString = "";
            string value = veriable.Substring(1, veriable.Length - 2);
            DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(conString);
            SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
            string mode = "2";
            command.CommandText = "uspEmailSending";
            sqlHelper.AddParameter(command, "@Mode", mode, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@Parameter", value, ParameterDirection.Input);
            sqlHelper.AddParameter(command, "@TransactionId", orderId.ToString(), ParameterDirection.Input);
            DataSet rdrParameterValue = sqlHelper.ExecuteDataSet(command);

            if (rdrParameterValue.Tables.Count > 0 && rdrParameterValue.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow dr in rdrParameterValue.Tables[0].Rows)
                {
                    value = Convert.ToString(dr["ParameterValue"].ToString().Trim());
                }
            }
            return value;
        }

        /// <summary>
        /// Get email message sending details
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns>Dataset</returns>
        public static DataSet GetMessageDetails(string actionId)
        {
            try
            {
                var conString = "";
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(conString);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                string mode = "1";
                command.CommandText = "uspEmailSending";
                sqlHelper.AddParameter(command, "@Mode", mode, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@ActionId", actionId, ParameterDirection.Input);
                DataSet dsMessageDetails = sqlHelper.ExecuteDataSet(command);
                return dsMessageDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Insert transaction after sending email
        /// </summary>
        /// <returns> status</returns>
        public static bool InsertEmailTransaction(int messageId, string message, string emailTo, string emailCc, string emailBcc)
        {
            try
            {
                var conString = "";
                DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(conString);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "uspEmailSending";

                sqlHelper.AddParameter(command, "@Mode", "I", ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@MessageId", messageId, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@Message", message, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@EmailTo", emailTo, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@EmailCc", emailCc, ParameterDirection.Input);
                sqlHelper.AddParameter(command, "@EmailBcc", emailBcc, ParameterDirection.Input);

                int rec = sqlHelper.ExecuteNonQuery(command);
                if (rec >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string PopulateBody(string user, string Distributor, string Value, string shopName, string Organisation, string title)
        {
            string body = string.Empty;
            ////using (StreamReader reader = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/email-template.html")))
            ////{
            ////    body = reader.ReadToEnd();
            ////}
            body = body.Replace("{ShopName}", shopName);
            body = body.Replace("{organization}", Organisation);
            body = body.Replace("{User}", user);
            body = body.Replace("{Distributor}", Distributor);
            body = body.Replace("{Value}", Value);

            body = body.Replace("{Content}", title);

            // body = body.Replace("{Description}", description);
            return body;
        }
        ////public static string GetData(GridView grid)
        ////{

        ////    StringBuilder strBuilder = new StringBuilder();
        ////    StringWriter strWriter = new StringWriter(strBuilder);
        ////    System.Web.UI.HtmlTextWriter htWriter = new System.Web.UI.HtmlTextWriter(strWriter);


        ////    grid.RenderControl(htWriter);
        ////    return strBuilder.ToString();
        ////}
        private void sendEmail(string Action, string mailContent, string shopId, string connectionstring, string distributorId)
        {
            ////string ShopMailId = GetShopMailId(shopId, connectionstring);
            ////string DistributorMailId = GetDistributorMailId(distributorId.Split(';')[0].Trim(), connectionstring);

            ////ShopMailId = (ShopMailId.Trim().Length > 0 ? ShopMailId += ";" + DistributorMailId : DistributorMailId);
            //////_log.LogMessage("sendEmail()", ShopMailId, "L2");
            ////Email.SendEmail(Action, mailContent, ShopMailId, connectionstring);
        }

        private void sendSMSOrder(string returnData, string shopId, string connectionstring, string ProcessName)
        {
            ////SendSMS(shopId, ProcessName, connectionstring, returnData);
        }

        //method for get mobile number of shop owner
        public string GetMobileNumber(string strShopId, string con)
        {
            string mobileNo = "";
            SqlConnection conn = new SqlConnection(con);
            try
            {
                // for read mobile number
                conn.Open();
                Infocean.DataAccessHelper.DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(con);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "GetMailId";
                command.Parameters.Add(new SqlParameter("@shopId", Convert.ToInt32(strShopId)));
                command.Parameters.Add(new SqlParameter("@Mode", 4));
                DataSet ds = sqlHelper.ExecuteDataSet(command);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    ////BuildNoClass BuildNoClass = new BuildNoClass();
                    ////foreach (DataRow dr in ds.Tables[0].Rows)
                    ////{
                    ////    mobileNo = dr["MobileNo"].ToString().Trim();

                    ////}
                }
                return mobileNo;
            }
            catch (Exception ex)
            {

                return mobileNo;
            }

        }
        //method for get mobile number of shop owner
        public string GetShopMailId(string strShopId, string con)
        {
            string mailId = "";
            SqlConnection conn = new SqlConnection(con);
            try
            {
                // for read mobile number
                conn.Open();
                Infocean.DataAccessHelper.DataAccessSqlHelper sqlHelper = new DataAccessSqlHelper(con);
                SqlCommand command = sqlHelper.CreateCommand(CommandType.StoredProcedure);
                command.CommandText = "GetMailId";
                command.Parameters.Add(new SqlParameter("@shopId", Convert.ToInt32(strShopId)));
                command.Parameters.Add(new SqlParameter("@Mode", 5));
                DataSet ds = sqlHelper.ExecuteDataSet(command);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        mailId = dr["Email"].ToString().Trim();

                    }
                }
                return mailId;
            }
            catch (Exception ex)
            {
                ////_log.LogMessage("GetShopMailId +Catch ", ex.ToString(), "L2");
                ////_log.LogMessage("GetShopMailId +mailId ", mailId, "L2");
                return mailId;
            }

        }
        private void callSAP(string userId, int orderId1)
        {
            ////SAPService.Service sapService = new SAPService.Service();
            ////sapService.callSAP(userId, orderId1);

        }

        public void SaveImageToFolder(string imageData, string imageName, string imagePath)
        {
            try
            {
                ////System.Drawing.Bitmap bmp = null;
                ////byte[] byteResponse = Convert.FromBase64String(imageData);

                //////MemoryStream memoryStream = new MemoryStream(byteResponse);

                ////using (FileStream fs = new FileStream(imagePath + imageName, FileMode.Create))
                ////{
                ////    MemoryStream memoryStream = new MemoryStream(byteResponse);
                ////    memoryStream.WriteTo(fs);
                ////    memoryStream.Close();
                ////}

                //////FileStream fs = new FileStream(imagePath + imageName, FileMode.Create);
                //////memoryStream.WriteTo(fs);

                //////memoryStream.Close();
                //////fs.Close();
                //////fs.Dispose();
            }
            catch (Exception ex)
            {

            }
        }


        private void sendEmailAlertNewCustomer(string usernameNewCust, string reportingToNewCust,
                           string ToMailNewCust, string ShopNameNewCust, string ShopTypeNewCust, string AddressNewCust
                          , string ContactNameNewCust, string MobileNewCust, string RouteNewCust, string EmailNewCust,
            string ShopclassNewCust, string LandMarkNewCust, string LandLineNewCust, string ConString)
        {
            if (ToMailNewCust != "")
            {
                try
                {

                    DataAccessSqlHelper sqlHelper1 = new DataAccessSqlHelper(ConString);
                    SqlCommand command1 = sqlHelper1.CreateCommand(CommandType.StoredProcedure);
                    command1.CommandText = "uspEmailSettings";
                    sqlHelper1.AddParameter(command1, "@Mode", "5", ParameterDirection.Input);
                    DataSet dsEmailSettings = sqlHelper1.ExecuteDataSet(command1);
                    MailMessage Msg = new MailMessage();
                    Attachment inlineLogo = new Attachment(dsEmailSettings.Tables[0].Rows[0]["EmailLogoPath"].ToString());
                    Msg.Attachments.Add(inlineLogo);
                    string contentID = "Image";
                    inlineLogo.ContentId = contentID;
                    inlineLogo.ContentDisposition.Inline = true;
                    ////inlineLogo.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                    string body = PopulateBodyNewCustomerEmail(usernameNewCust, reportingToNewCust,
                            ShopNameNewCust, ShopTypeNewCust, AddressNewCust, ContactNameNewCust,
                            MobileNewCust, RouteNewCust, EmailNewCust, ShopclassNewCust,
                            LandMarkNewCust, LandLineNewCust, contentID);
                    var fromMail = new MailAddress(dsEmailSettings.Tables[0].Rows[0]["EmailFrom"].ToString(), "");
                    Msg.From = fromMail;
                    string[] emailIds = ToMailNewCust.Split(';');
                    foreach (string id in emailIds)
                    {
                        if (string.Compare(id, "") != 0)
                        {
                            Msg.To.Add(id);
                        }
                    }
                    // Msg.To.Add(new MailAddress(ToMailNewCust, ""));
                    string fromPassword = dsEmailSettings.Tables[0].Rows[0]["Password"].ToString();
                    SmtpClient smtp = new SmtpClient("localhost", 25);
                    smtp.Host = dsEmailSettings.Tables[0].Rows[0]["SMTP"].ToString();
                    smtp.Port = Convert.ToInt32(dsEmailSettings.Tables[0].Rows[0]["Port"].ToString());
                    smtp.EnableSsl = Convert.ToBoolean(dsEmailSettings.Tables[0].Rows[0]["EnableSSL"].ToString());
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = Convert.ToBoolean(dsEmailSettings.Tables[0].Rows[0]["DefaultCredentials"].ToString());
                    ////smtp.Credentials = new NetworkCredential(fromMail.Address, fromPassword);
                    Msg.Subject = "New Customer Alert";
                    Msg.IsBodyHtml = true;
                    Msg.Body = body;

                    smtp.Send(Msg);
                }
                catch (Exception ex)
                {
                    string content = "exception : " + ex.Message + " ## " + DateTime.Now;
                    //lg.LogWrite(Convert.ToString(ex));
                    throw ex;
                }

            }
            //else
            //{
            //    //return 0;
            //}
        }

        private string PopulateBodyNewCustomerEmail(string usernameNewCust, string reportingToNewCust,
                            string ShopNameNewCust, string ShopTypeNewCust, string AddressNewCust
                          , string ContactNameNewCust, string MobileNewCust, string RouteNewCust,
            string EmailNewCust,
            string ShopclassNewCust, string LandMarkNewCust, string LandLineNewCust, string image)
        {
            try
            {
                string body = string.Empty;
                ////using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/New_Customer_Email_alert.html")))
                ////{
                ////    body = reader.ReadToEnd();
                ////}

                body = body.Replace("{contentID}", image);
                body = body.Replace("{Manager}", reportingToNewCust);
                body = body.Replace("{UserName}", usernameNewCust);
                body = body.Replace("{Address}", AddressNewCust);
                body = body.Replace("{ShopName}", ShopNameNewCust);
                body = body.Replace("{Email}", EmailNewCust);
                body = body.Replace("{ContactName}", ContactNameNewCust);
                body = body.Replace("{Mobile}", MobileNewCust);
                body = body.Replace("{ShopType}", ShopTypeNewCust);
                body = body.Replace("{Route}", RouteNewCust);
                body = body.Replace("{ShopClass}", ShopclassNewCust);
                body = body.Replace("{Landmark}", LandMarkNewCust);
                body = body.Replace("{Landline}", LandLineNewCust);
                return body;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private Boolean generatePDFQuatation(string product, string rate, string quantity, string quatationNo, string customer, string salesman, string total, string headerName, string footerName)
        {
            try
            {
                //specifying the Document size.
                var doc = new Document(PageSize.A4, 0, 0, 0, 0);

                ////PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/Quotation" + quatationNo + ".pdf"), FileMode.Create));
                ////string strHeaderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Resources/" + headerName + ".png");
                ////string strFooterPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Resources/" + footerName + ".png");
                doc.Open();
                Rectangle page = doc.PageSize;
                /******/
                /**The table for logo**/
                PdfPTable headerTable = new PdfPTable(1);
                headerTable.TotalWidth = page.Width;
                PdfPCell headerCell = new PdfPCell();
                headerCell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                ////iTextSharp.text.Image imageHeader = iTextSharp.text.Image.GetInstance(strHeaderPath);
                ////AddImageInCell(headerCell, imageHeader, page.Width, 200f, 0);
                headerCell.Border = 0;
                headerTable.AddCell(headerCell);

                ////writer.PageEvent = new Footer(strHeaderPath, strFooterPath, page.Width, page.Height, headerTable);

                /**Setting the font**/
                Font Arial = FontFactory.GetFont("Arial", 10F, Font.NORMAL, BaseColor.BLACK);
                Font ArialBold = FontFactory.GetFont("Arial", 10F, Font.BOLD, BaseColor.BLACK);



                /**the table for customer name reference etc....**/
                PdfPTable table2 = new PdfPTable(2);
                table2.TotalWidth = page.Width;
                PdfPCell cell2 = new PdfPCell(new Phrase(customer));
                cell2.FixedHeight = 50f;
                cell2.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                table2.AddCell(cell2);

                /**inner table for quatation no and reference**/
                PdfPTable innerTable2 = new PdfPTable(1);
                innerTable2.TotalWidth = page.Width;
                PdfPCell cell4 = new PdfPCell(new Phrase(quatationNo + " dated " + System.DateTime.Now.ToString("dd MMM yyyy"), Arial));
                cell4.FixedHeight = 20f;
                cell4.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
                innerTable2.AddCell(cell4);
                PdfPCell cell5 = new PdfPCell(new Phrase(salesman));
                //cell5.MinimumHeight = 50f;
                cell5.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER;
                innerTable2.AddCell(cell5);
                PdfPCell nesthousing = new PdfPCell(innerTable2);
                nesthousing.Padding = 0f;
                table2.AddCell(nesthousing);
                /**giving space.**/
                table2.SpacingAfter = 15f;
                table2.SpacingBefore = headerTable.TotalHeight + 5f;
                /**spacing table**/
                PdfPTable spacer = new PdfPTable(1);
                PdfPCell spaceCell = new PdfPCell(new Phrase("\n")); spaceCell.Border = 0;
                spaceCell.Border = 0;
                spacer.AddCell(spaceCell);



                PdfPTable table3 = new PdfPTable(2);
                table3.TotalWidth = page.Width;
                /**Table for the dynamic content**/
                table3.AddCell(new Phrase("Sl No     Description of goods          ", Arial));
                PdfPTable innerTableDynamic = new PdfPTable(3);
                innerTableDynamic.AddCell(new Phrase(" Quantity", Arial));
                innerTableDynamic.AddCell(new Phrase(" Rate", Arial));
                innerTableDynamic.AddCell(new Phrase(" Amount", Arial));
                PdfPCell dynamicHousing = new PdfPCell(innerTableDynamic);
                dynamicHousing.Padding = 0f;
                table3.AddCell(dynamicHousing);

                /**Iterating  the dynamic contents**/
                string[] productsArrayTemp = product.Split(',');
                string[] productsArray = new string[productsArrayTemp.Length];
                for (int i = 0; i < productsArrayTemp.Length; i++)
                {
                    if (productsArrayTemp[i].Length < 19)
                    {
                        productsArray[i] = productsArrayTemp[i].Trim();
                    }
                    else
                    {
                        productsArray[i] = productsArrayTemp[i].Substring(0, 17).Trim();
                    }
                }

                string[] rateArray = rate.Split(',');
                string[] quantityArray = quantity.Split(',');
                int spacingCount = 0;
                int spacingGain = 0;
                Boolean insertlistHeader = false;
                for (int i = 0; i < productsArray.Length; i++)
                {
                    if (insertlistHeader)
                    {
                        table3.AddCell(new Phrase("Sl No     Description of goods          ", Arial));
                        PdfPTable innerTableDynamicNew = new PdfPTable(3);
                        innerTableDynamicNew.AddCell(new Phrase(" Quantity", Arial));
                        innerTableDynamicNew.AddCell(new Phrase(" Rate", Arial));
                        innerTableDynamicNew.AddCell(new Phrase(" Amount", Arial));
                        PdfPCell dynamicHousingNew = new PdfPCell(innerTableDynamicNew);
                        dynamicHousingNew.Padding = 0f;
                        table3.AddCell(dynamicHousingNew);
                        insertlistHeader = false;
                    }
                    int slNo = i + 1;
                    PdfPCell productCell = new PdfPCell(new Phrase(Convert.ToString(slNo).PadLeft(2, '0') + "          " + productsArray[i], Arial));
                    if (i == (productsArray.Length - 1))
                    {
                        productCell.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER;
                    }
                    else
                    {
                        productCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    }

                    table3.AddCell(productCell);
                    innerTableDynamic = new PdfPTable(3);
                    PdfPCell innerCell = new PdfPCell(new Phrase(string.Format("{0:N2}", Convert.ToDouble(quantityArray[i])), Arial));
                    innerCell.Border = Rectangle.RIGHT_BORDER;
                    innerCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    innerTableDynamic.AddCell(innerCell);
                    innerCell = new PdfPCell(new Phrase(string.Format("{0:N2}", Convert.ToDouble(rateArray[i])), Arial));
                    innerCell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                    innerCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    innerTableDynamic.AddCell(innerCell);
                    innerCell = new PdfPCell(new Phrase(string.Format("{0:N2}", Convert.ToDouble(Convert.ToDouble(rateArray[i]) * Convert.ToDouble(quantityArray[i]))), Arial));
                    innerCell.Border = Rectangle.RIGHT_BORDER;
                    innerCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    innerTableDynamic.AddCell(innerCell);
                    dynamicHousing = new PdfPCell(innerTableDynamic);

                    if (i == (productsArray.Length - 1))
                    {
                        dynamicHousing.Border = Rectangle.BOTTOM_BORDER;
                    }
                    else
                    {
                        dynamicHousing.Border = 0;
                    }
                    dynamicHousing.Padding = 0f;
                    table3.AddCell(dynamicHousing);
                    if (spacingCount == 29 + spacingGain)
                    {
                        for (int c = 0; c < 15; c++)
                        {
                            PdfPCell emptyCell = new PdfPCell(new Phrase("\n"));
                            if (c == 0)
                            {
                                emptyCell.Border = Rectangle.TOP_BORDER;
                            }
                            else
                            {
                                emptyCell.Border = 0;
                            }

                            table3.AddCell(emptyCell);
                            table3.AddCell(emptyCell);

                        }
                        spacingCount = 0;
                        spacingGain = 10;
                        insertlistHeader = true;
                    }
                    spacingCount++;

                }
                PdfPTable totalTable = new PdfPTable(2);
                PdfPCell totalTitle = new PdfPCell(new Phrase("Total Amount", ArialBold));
                PdfPCell totalAmountValue = new PdfPCell(new Phrase(string.Format("{0:N2}", Convert.ToDouble(total)), ArialBold));
                totalAmountValue.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalTable.AddCell(totalTitle);
                totalTable.AddCell(totalAmountValue);
                //*******************************Adding tables to documents**********************************************//
                doc.Add(spacer);
                doc.Add(table2);
                doc.Add(table3);
                doc.Add(totalTable);
                doc.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

            //**********************


        }



        #endregion

    }

    public enum Status
    {
        Error,
        InvalidUser,
        LicenceKeyDoesntExist,
        CustomerDBNotAvailable,
        Success,
        AttributesNull,
        IndexOutOfRange,
        NullReferenceException,
        NoUpdation,
        LicenceServiceError,
        SettingsError

    }

}