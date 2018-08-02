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
using System.Security.Cryptography;

namespace FieldMax.MobileSyncService.DbAccess
{
    /// <summary>
    /// This class is resposible to take a single SQL Server database
    /// and convert it to an SQLite database file.
    /// </summary>
    /// <remarks>The class knows how to convert table and index structures only.</remarks>
    public class SqlServerToSQLite
    {

        #region Public Properties
        
        public static ErrorLogUtilities _log = new ErrorLogUtilities();

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        //public static bool IsActive
        //{
        //    get { return _isActive; }
        //}
        #endregion

        #region Public Methods
        //public static void CancelConversion()
        //{
        //    _cancelled = true;
        //}

        
        #endregion

        #region Private Methods
        /// <summary>
        /// Do the entire process of first reading the SQL Server schema, creating a corresponding
        /// SQLite schema, and copying all rows from the SQL Server database to the SQLite database.
        /// </summary>
        /// <param name="sqlConnString">The SQL Server connection string</param>
        /// <param name="sqlitePath">The path to the generated SQLite database file</param>
        /// <param name="password">The password to use or NULL if no password should be used to encrypt the DB</param>
        /// <param name="query">Dictionary of tablenames and columns in those tables  to be converted to SQLite database</param>
        /// <param name="viewNamesList">The List of viewnames and columns in views to be converted into tables</param>     
        public static Dictionary<string, string> ConvertSqlServerDatabaseToSQLiteFile(
            string sqlConnString, string sqlitePath, string password, Dictionary<string, string> query, List<string> viewNameList,
            bool createTriggers, string sqlitePath1, string userId, string modifiedDate)
        {
            try
            {
                DatabaseSchema ds = ReadSqlServerSchema(sqlConnString, query, viewNameList);
                string checksum = "";
                Dictionary<string, string> Response = new Dictionary<string, string>();
                if (!File.Exists(sqlitePath))
                {
                    CreateSQLiteDatabase(sqlitePath, ds, password, query, viewNameList);
                }
                else
                {
                    SQLiteDatabase sqliteDatabase = new SQLiteDatabase(sqlitePath);
                    sqliteDatabase.ClearDB(userId, modifiedDate);
                }
                // Copy all rows from SQL Server tables to the newly created SQLite database
                CopySqlServerRowsToSQLiteDB(sqlConnString, sqlitePath, ds.Tables, query, viewNameList, password, userId, modifiedDate);

                //_log.LogMessage("CopySqlServerRowsToSQLiteDBCompleted: User:" + userId + "Date: ", modifiedDate, "L2");              

                ///TO ZIP FILE 
                using (ZipFile zip = new ZipFile())
                {
                    
                    zip.AddFile(sqlitePath, "");
                    zip.Save(ReplaceFirst(sqlitePath, ".db", ".zip"));
                    
                }

                string data = Convert.ToBase64String(File.ReadAllBytes(sqlitePath));
                checksum = MD5Encryption(data);

                Response.Add("Result", "true");
                Response.Add("Value", checksum);

                return Response;
            }
            catch (Exception ex)
            {
                _log.LogMessage("ConvertSqlServerDatabaseToSQLiteFile()", ex.ToString(), "L2");
                DeleteSqliteFiles(sqlitePath);
                throw ex;

            }
        }


        /// <summary>
        /// MD5 Encryption
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string MD5Encryption(string plainText)
        {
            string newPlainText = plainText.Trim();
            MD5 enc = MD5.Create();
            byte[] rescBytes = Encoding.ASCII.GetBytes(newPlainText);
            byte[] hashBytes = enc.ComputeHash(rescBytes);
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                str.Append(hashBytes[i].ToString("x2"));
            }
            string productmenu = str.ToString();
            return productmenu;
        }
        /// <summary>
        /// To replace the first occurence of a string with another string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }


        public static void DeleteSqliteFiles(string sqlitePath)
        {
            try
            {
                if (File.Exists(sqlitePath))
                {
                    File.Delete(sqlitePath);
                }
                if (File.Exists(sqlitePath))
                {
                    throw new Exception("File Delete failed: " + sqlitePath);
                }
                string zipPath = string.Empty;
                zipPath = ReplaceFirst(sqlitePath, ".db", ".zip");
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
                if (File.Exists(zipPath))
                {
                    throw new Exception("File Delete failed: " + zipPath);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Copies table rows from the SQL Server database to the SQLite database.
        /// </summary>
        /// <param name="sqlConnString">The SQL Server connection string</param>
        /// <param name="sqlitePath">The path to the SQLite database file.</param>
        /// <param name="schema">The schema of the SQL Server database.</param>
        /// <param name="password">The password to use for encrypting the file</param>
        /// <param name="query">Dictionary of tablenames and columns in those tables  to be converted to SQLite database</param>
        /// <param name="viewNamesList">The List of viewnames and columns in views to be converted into tables</param>     
        private static void CopySqlServerRowsToSQLiteDB(
            string sqlConnString, string sqlitePath, List<TableSchema> schema, Dictionary<string, string> query, List<string> viewNamesList,
            string password, string userId, string modifiedDate)
        {
            //CheckCancelled();
            // _log.Debug("preparing to insert tables ...");
            SqlConnection ssconn = null;
            SQLiteConnection sqconn = null;
            // Connect to the SQL Server database
            try
            {
                using (ssconn = new SqlConnection(sqlConnString))
                {
                    ssconn.Open();

                    // Connect to the SQLite database next
                    string sqliteConnString = CreateSQLiteConnectionString(sqlitePath);

                    using (sqconn = new SQLiteConnection(sqliteConnString))
                    {
                        TableSchema tableSchema = new TableSchema();
                        sqconn.Open();

                        // Go over all tables in the schema and copy their rows                   
                        foreach (KeyValuePair<String, String> entry in query)
                        {
                            for (int i = 0; i < schema.Count; i++)
                            {
                                if (schema[i].TableName.ToLower().Trim() == entry.Key.Split('@')[0].ToLower().Trim())
                                {
                                    //_log.LogMessage("TableSchema: User: " + userId + " Date: " + modifiedDate, " TableSchemaTable :" + schema[i].TableName.ToLower().Trim() + ";EntryKey:" + entry.Key.Split('@')[0].ToLower().Trim(), "L2");                                
                                    using (SQLiteTransaction tx = sqconn.BeginTransaction())
                                    {
                                        try
                                        {
                                            string tableQuery = BuildSqlServerTableQuery(entry.Value);
                                            SqlCommand queryCmd = new SqlCommand(tableQuery, ssconn);
                                            queryCmd.CommandTimeout = 90;

                                            SqlDataAdapter dataAdaptQuery = new SqlDataAdapter();
                                            dataAdaptQuery.SelectCommand = queryCmd;
                                            DataTable dtQuery = new DataTable();
                                            dataAdaptQuery.Fill(dtQuery);
                                            int sqlRowCount = -1;
                                            if (dtQuery != null)
                                            {
                                                sqlRowCount = dtQuery.Rows.Count;
                                            }

                                            //_log.LogMessage("Query & RowCount(SQL): User: " + userId + " Date: " + modifiedDate, " Row Count for " + schema[i].TableName + "=" + sqlRowCount + ". Query:" + tableQuery, "L2");

                                            string pId = entry.Key.Split('@')[1].Split('#')[0].ToString();

                                            using (SQLiteCommand insert = BuildSQLiteInsert(schema[i], entry.Key.Split('@')[1]))
                                            {
                                                //int counter = 0;
                                                foreach (DataRow dtRow in dtQuery.Rows)
                                                {
                                                    insert.Connection = sqconn;
                                                    insert.Transaction = tx;
                                                    List<string> pnames = new List<string>();

                                                    string[] strArr1 = null;
                                                    strArr1 = entry.Key.Split('@')[1].Split('#');


                                                    for (int k = 0; k < strArr1.Length; k++)
                                                    {
                                                        for (int j = 0; j < schema[i].Columns.Count; j++)
                                                        {
                                                            if (schema[i].Columns[j].ColumnName.ToLower().Trim().Equals(strArr1[k].ToLower().Trim()))
                                                            {
                                                                string pname = "@" + GetNormalizedName(schema[i].Columns[j].ColumnName, pnames);
                                                                insert.Parameters[pname].Value = CastValueForColumn(dtRow[k], schema[i].Columns[j]);
                                                                pnames.Add(pname);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    insert.ExecuteNonQuery();

                                                } // foreach
                                            }

                                            // CheckCancelled();
                                            tx.Commit();


                                            using (SQLiteCommand cmdRowCount = new SQLiteCommand(sqconn))
                                            {
                                                cmdRowCount.CommandText = "select count(" + pId + ") from " + schema[i].TableName.ToLower().Trim() + ";";
                                                cmdRowCount.CommandType = CommandType.Text;
                                                int RowCount = -2;

                                                RowCount = Convert.ToInt32(cmdRowCount.ExecuteScalar());

                                                //_log.LogMessage("RowCount(SQLite):User: " + userId + " Date: " + modifiedDate, "Row Count for " + schema[i].TableName + " = " + RowCount, "L2");

                                                if (sqlRowCount != RowCount)
                                                {
                                                    //_log.LogMessage("RowCount(SQLite):User: " + userId + " Date: " + modifiedDate, "Row Count for " + schema[i].TableName + " =" + RowCount + " does not match with sql", "L2");
                                                    throw new Exception();
                                                }
                                            }

                                            //  _log.LogMessage("CopySqlServerRowsToSQLiteDB", "finished inserting all rows for table [" + ts.TableName + "]", "L1");
                                        }

                                        catch (Exception ex)
                                        {
                                            _log.LogMessage("unexpected exception in CopySqlServerRowsToSQLiteDB()", ex.ToString(), "L2");
                                            tx.Rollback();
                                            throw;
                                        } // catch
                                    }
                                }
                            }
                        }
                        string[] completeviewname = null;
                        foreach (string item in viewNamesList)
                        {
                            completeviewname = item.Split('#');
                            for (int i = 0; i < schema.Count; i++)
                            {
                                if (schema[i].TableName == completeviewname[0])
                                {
                                    using (SQLiteTransaction tx = sqconn.BeginTransaction())
                                    {
                                        try
                                        {

                                            string tableQuery = BuildSqlServerTableQueryForView(schema[i], completeviewname, userId);
                                            SqlCommand viewquery = new SqlCommand(tableQuery, ssconn);
                                            viewquery.CommandTimeout = 90;

                                            SqlDataAdapter dataAdaptView = new SqlDataAdapter();
                                            dataAdaptView.SelectCommand = viewquery;
                                            DataTable dtView = new DataTable();
                                            dataAdaptView.Fill(dtView);

                                            int sqlViewRowCount = 0;
                                            if (dtView != null && dtView.Rows.Count > 0)
                                            {
                                                sqlViewRowCount = dtView.Rows.Count;
                                            }

                                            //_log.LogMessage("Query & RowCount(SQL): User: " + userId + " Date: " + modifiedDate, " Row Count for " + schema[i].TableName + "=" + sqlViewRowCount + ". Query:" + tableQuery, "L2");


                                            //using (SqlDataReader reader = viewquery.ExecuteReader())
                                            //{
                                            using (SQLiteCommand insert = BuildSQLiteInsertForView(schema[i]))
                                            {

                                                foreach (DataRow dtRow in dtView.Rows)
                                                {
                                                    insert.Connection = sqconn;
                                                    insert.Transaction = tx;
                                                    List<string> pnames = new List<string>();
                                                    for (int j = 0; j < schema[i].Columns.Count; j++)
                                                    {
                                                        string pname = "@" + GetNormalizedName(schema[i].Columns[j].ColumnName, pnames);
                                                        insert.Parameters[pname].Value = CastValueForColumn(dtRow[j], schema[i].Columns[j]);
                                                        pnames.Add(pname);
                                                    }
                                                    insert.ExecuteNonQuery();
                                                } // for
                                                  //} // using
                                            }
                                            // CheckCancelled();
                                            tx.Commit();

                                            using (SQLiteCommand cmdRowCount = new SQLiteCommand(sqconn))
                                            {
                                                cmdRowCount.CommandText = "select count(*) from " + schema[i].TableName + ";";
                                                cmdRowCount.CommandType = CommandType.Text;
                                                int viewRowCount = 0;

                                                viewRowCount = Convert.ToInt32(cmdRowCount.ExecuteScalar());

                                                //_log.LogMessage("RowCount(SQLite):User: " + userId + " Date: " + modifiedDate, "Row Count for " + schema[i].TableName + " = " + viewRowCount, "L2");

                                                if (sqlViewRowCount != viewRowCount)
                                                {
                                                    //_log.LogMessage("RowCount(SQLite):User: " + userId + " Date: " + modifiedDate, "Row Count for " + schema[i].TableName + " =" + viewRowCount + " does not match with sql", "L2");
                                                    throw new Exception();
                                                }
                                            }

                                            //_log.LogMessage("CopySqlServerRowsToSQLiteDB()", "finished inserting all rows for view [" + schema[i].TableName + "]", "L2");
                                        }
                                        catch (Exception ex)
                                        {
                                            _log.LogMessage("unexpected exception", ex.ToString(), "L2");
                                            tx.Rollback();
                                            throw;
                                        } // catch
                                        break;
                                    }
                                }
                            }
                        }
                    } // using
                } // using
            }
            finally
            {
                if (sqconn != null)
                {
                    sqconn.Close();
                    sqconn.Dispose();
                }
                if (ssconn != null)
                {
                    ssconn.Close();
                }
            }
        }

        /// <summary>
        /// Used in order to adjust the value received from SQL Servr for the SQLite database.
        /// </summary>
        /// <param name="val">The value object</param>
        /// <param name="columnSchema">The corresponding column schema</param>
        /// <returns>SQLite adjusted value.</returns>
        private static object CastValueForColumn(object val, ColumnSchema columnSchema)
        {
            if (val is DBNull)
                return null;

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
                    //_log.LogMessage("CastValueForColumn()", "argument exception - illegal database type", "L2");
                    throw new ArgumentException("Illegal database type [" + Enum.GetName(typeof(DbType), dt) + "]");
            } // switch

            return val;
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

        /// <summary>
        /// Creates a command object needed to insert values into a specific SQLite table.
        /// </summary>
        /// <param name="ts">The table schema object for the table.</param>
        /// <returns>A command object with the required functionality.</returns>
        private static SQLiteCommand BuildSQLiteInsert(TableSchema ts, string query)
        {
            SQLiteCommand res = new SQLiteCommand();

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO [" + ts.TableName + "] (");
            string[] strArr1 = null;

            strArr1 = query.Split('#');
            for (int i = 0; i < ts.Columns.Count; i++)
            {
                for (int k = 0; k < strArr1.Length; k++)
                {
                    if (ts.Columns[i].ColumnName.ToLower().Trim().Equals(strArr1[k].ToLower().Trim()))
                    {

                        sb.Append("[" + ts.Columns[i].ColumnName + "]");
                        if (k < strArr1.Length - 1)
                            sb.Append(", ");
                    }
                }
            } // for
            sb.Append(") VALUES (");

            List<string> pnames = new List<string>();
            for (int i = 0; i < ts.Columns.Count; i++)
            {
                for (int k = 0; k < strArr1.Length; k++)
                {
                    if (ts.Columns[i].ColumnName.ToLower().Trim().Equals(strArr1[k].ToLower().Trim()))
                    {
                        string pname = "@" + GetNormalizedName(ts.Columns[i].ColumnName, pnames);
                        sb.Append(pname);
                        if (k < strArr1.Length - 1)
                            sb.Append(", ");

                        DbType dbType = GetDbTypeOfColumn(ts.Columns[i]);
                        SQLiteParameter prm = new SQLiteParameter(pname, dbType, ts.Columns[i].ColumnName);
                        res.Parameters.Add(prm);

                        // Remember the parameter name in order to avoid duplicates
                        pnames.Add(pname);
                    }
                }


            } // for
            sb.Append(")");
            res.CommandText = sb.ToString();
            res.CommandType = CommandType.Text;
            return res;
        }

        /// <summary>
        /// Builds a SELECT query for a specific view. Needed in the process of copying rows
        /// from the SQL Server database to the SQLite database.
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        private static SQLiteCommand BuildSQLiteInsertForView(TableSchema ts)
        {
            SQLiteCommand res = new SQLiteCommand();

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO [" + ts.TableName + "] (");
            for (int i = 0; i < ts.Columns.Count; i++)
            {
                sb.Append("[" + ts.Columns[i].ColumnName + "]");
                if (i < ts.Columns.Count - 1)
                    sb.Append(", ");
            } // for
            sb.Append(") VALUES (");

            List<string> pnames = new List<string>();
            for (int i = 0; i < ts.Columns.Count; i++)
            {
                string pname = "@" + GetNormalizedName(ts.Columns[i].ColumnName, pnames);
                sb.Append(pname);
                if (i < ts.Columns.Count - 1)
                    sb.Append(", ");

                DbType dbType = GetDbTypeOfColumn(ts.Columns[i]);
                SQLiteParameter prm = new SQLiteParameter(pname, dbType, ts.Columns[i].ColumnName);
                res.Parameters.Add(prm);

                // Remember the parameter name in order to avoid duplicates
                pnames.Add(pname);
            } // for
            sb.Append(")");
            res.CommandText = sb.ToString();
            res.CommandType = CommandType.Text;
            return res;
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
        /// Matches SQL Server types to general DB types
        /// </summary>
        /// <param name="cs">The column schema to use for the match</param>
        /// <returns>The matched DB type</returns>
        private static DbType GetDbTypeOfColumn(ColumnSchema cs)
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

            // _log.LogMessage("GetDbTypeOfColumn()", "illegal db type found", "L2");
            throw new ApplicationException("Illegal DB type found (" + cs.ColumnType + ")");
        }

        /// <summary>
        /// Builds a SELECT query for a specific table. Needed in the process of copying rows
        /// from the SQL Server database to the SQLite database.
        /// </summary>
        /// <param name="ts">The table schema of the table for which we need the query.</param>
        /// <returns>The SELECT query for the table.</returns>
        private static string BuildSqlServerTableQuery(string query)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(query);
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>  
        private static string BuildSqlServerTableQueryForView(TableSchema ts, string[] viewname, string userId)
        {
            StringBuilder sb = new StringBuilder();

            if (viewname[1] != "" && viewname[0] != "VWSL_ProductQty")
            {
                sb.Append("SELECT ");
                for (int i = 0; i < ts.Columns.Count; i++)
                {
                    sb.Append("[" + ts.Columns[i].ColumnName + "]");
                    if (i < ts.Columns.Count - 1)
                        sb.Append(", ");
                } // for
                sb.Append(" FROM " + ts.TableSchemaName + "." + "[" + ts.TableName + "]");
                sb.Append(" WHERE " + viewname[1]);
                return sb.ToString();
            }
            //else if (viewname[0] != "VWSL_OrderDetails")
            //{
            //    sb.Append("SELECT ");
            //    for (int i = 0; i < ts.Columns.Count; i++)
            //    {
            //        sb.Append("[" + ts.Columns[i].ColumnName + "]");
            //        if (i < ts.Columns.Count - 1)
            //            sb.Append(", ");
            //    } // for
            //    sb.Append(" FROM " + ts.TableSchemaName + "." + "[" + ts.TableName + "]");
            //    return sb.ToString();
            //}
            else if (viewname[0] != "VWSL_ProductQty" && viewname[0] != "VWSL_ProductList")
            {
                sb.Append("SELECT ");
                for (int i = 0; i < ts.Columns.Count; i++)
                {
                    sb.Append("[" + ts.Columns[i].ColumnName + "]");
                    if (i < ts.Columns.Count - 1)
                        sb.Append(", ");
                } // for
                sb.Append(" FROM " + ts.TableSchemaName + "." + "[" + ts.TableName + "]");
                //sb.Append(" WHERE OrderId<0"); 
                return sb.ToString();
            }
            else if (viewname[0] == "VWSL_ProductList")
            {
                sb.Append("SELECT ");
                for (int i = 0; i < ts.Columns.Count; i++)
                {
                    sb.Append("[" + ts.Columns[i].ColumnName + "]");
                    if (i < ts.Columns.Count - 1)
                        sb.Append(", ");
                }
                sb.Append(" FROM " + ts.TableSchemaName + "." + "[" + ts.TableName + "]");
                sb.Append(" WHERE AttributeId in (select attributeid from menuattribute where userid = ");
                sb.Append(userId);
                sb.Append(")");
                return sb.ToString();

            }
            else
            {
                sb.Append("SELECT     dbo.VWSL_ProductList.ProductAttributeId,dbo.VWSL_ProductList.SchemeId,dbo.VWSL_ProductList.SchemeName, dbo.VWSL_ProductList.AttributeId, dbo.VWSL_ProductAvgQty.ShopId, ISNULL(CAST(dbo.VWSL_ProductAvgQty.AvgQuantity AS INTEGER), 0) AS AvgQty, dbo.VWSL_ProductList.PRODUCTNAME, dbo.VWSL_ProductList.DEFAULTPRICE, dbo.VWSL_ProductList.Stock, dbo.VWSL_ProductList.PriceIds, dbo.VWSL_ProductList.PrimPrices,   dbo.VWSL_ProductList.SecPrices, dbo.VWSL_ProductList.PrimaryUnitId, dbo.VWSL_ProductList.SecondaryUnitId FROM  dbo.VWSL_ProductList LEFT OUTER JOIN dbo.VWSL_ProductAvgQty ON dbo.VWSL_ProductList.ProductAttributeId = dbo.VWSL_ProductAvgQty.ProductAttributeId ");
                sb.Append(" AND " + viewname[1]);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates the SQLite database from the schema read from the SQL Server.
        /// </summary>
        /// <param name="sqlitePath">The path to the generated DB file.</param>
        /// <param name="schema">The schema of the SQL server database.</param>
        /// <param name="password">The password to use for encrypting the DB or null if non is needed.</param>
        /// <param name="handler">A handle for progress notifications.</param>
        private static void CreateSQLiteDatabase(string sqlitePath, DatabaseSchema schema, string password, Dictionary<string, string> query, List<string> viewNameList)
        {
            //_log.LogMessage("CreateSQLiteDatabase()", "Creating SQLite database...", "");
            SQLiteConnection conn = null;
            // Create the SQLite database file
            SQLiteConnection.CreateFile(sqlitePath);

            //_log.LogMessage("CreateSQLiteDatabase()", "SQLite file was created successfully at [" + sqlitePath + "]", "");

            // Connect to the newly created database
            string sqliteConnString = CreateSQLiteConnectionString(sqlitePath);
            using (conn = new SQLiteConnection(sqliteConnString))
            {
                conn.Open();

                // Create all tables in the new database
                int count = 0;
                foreach (KeyValuePair<String, String> entry in query)
                {
                    try
                    {
                        foreach (TableSchema dt in schema.Tables)
                        {
                            string keyColumn = entry.Key.Split('@')[0];
                            if (keyColumn.ToLower().Trim().Equals(dt.TableName.ToLower().Trim()))
                            {
                                AddSQLiteTable(conn, dt, entry.Key);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.LogMessage("AddSQLiteTable failed in method CreateSQLiteDatabase()", ex.ToString(), "L2");
                        throw;
                    }
                    count++;
                    //CheckCancelled();


                    //  _log.LogMessage("CreateSQLiteDatabase()", "added schema for SQLite table [" + entry.Key.Split('@')[0] + "]", "L1");
                } // foreach

                string[] strview = null;

                foreach (TableSchema dt in schema.Tables)
                {
                    foreach (string item in viewNameList)
                    {
                        strview = item.Split('#');
                        if (dt.TableName == strview[0])
                        {
                            try
                            {
                                AddSQLiteTableFromView(conn, dt, strview);
                            }
                            catch (Exception ex)
                            {
                                _log.LogMessage("CreateSQLiteDatabase()", "AddSQLiteTable failed", "L2");
                                throw;
                            }
                        }
                    }
                }
                count++;
                //CheckCancelled();

            } // using

            // _log.Debug("finished adding all table/view schemas for SQLite database");
        }

        private static void AddSQLiteView(SQLiteConnection conn, ViewSchema vs)
        {
            // Prepare a CREATE VIEW DDL statement
            string stmt = vs.ViewSQL;
            //  _log.Info("\n\n" + stmt + "\n\n");

            // Execute the query in order to actually create the view.
            SQLiteTransaction tx = conn.BeginTransaction();
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(stmt, conn, tx);
                cmd.ExecuteNonQuery();

                tx.Commit();
            }
            catch (SQLiteException ex)
            {
                tx.Rollback();

                //if (handler != null)
                {
                    ViewSchema updated = new ViewSchema();
                    updated.ViewName = vs.ViewName;
                    updated.ViewSQL = vs.ViewSQL;

                    // Ask the user to supply the new view definition SQL statement
                    string sql = null;//= handler(updated);

                    if (sql == null)
                        return; // Discard the view
                    else
                    {
                        // Try to re-create the view with the user-supplied view definition SQL
                        updated.ViewSQL = sql;
                        AddSQLiteView(conn, updated);
                    }
                }
                //else
                throw ex;
            } // catch
        }

        /// <summary>
        /// Creates the CREATE TABLE DDL for SQLite and a specific table.
        /// </summary>
        /// <param name="conn">The SQLite connection</param>
        /// <param name="dt">The table schema object for the table to be generated.</param>
        private static void AddSQLiteTable(SQLiteConnection conn, TableSchema dt, string query)
        {
            // Prepare a CREATE TABLE DDL statement
            string stmt = BuildCreateTableQuery(dt, query);

            // _log.Info("\n\n" + stmt + "\n\n");

            // Execute the query in order to actually create the table.
            using (SQLiteCommand cmd = new SQLiteCommand(stmt, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        private static void AddSQLiteTableFromView(SQLiteConnection conn, TableSchema dt, string[] strview)
        {
            string stmt = BuildCreateTableQueryForView(dt, strview);

            // Execute the query in order to actually create the table from view.
            using (SQLiteCommand cmd = new SQLiteCommand(stmt, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private static string BuildCreateTableQueryForView(TableSchema ts, string[] strview)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("CREATE TABLE [" + ts.TableName + "] (\n");

            bool pkey = false;
            for (int i = 0; i < ts.Columns.Count; i++)
            {
                ColumnSchema col = ts.Columns[i];
                string cline = BuildColumnStatement(col, ts, ref pkey);
                sb.Append(cline);
                if (i < ts.Columns.Count - 1)
                    sb.Append(",\n");
            } // foreach

            if (strview[2] == "1" && ts.TableName != "VWSL_OrderDetails")
            {
                ColumnSchema newCol = new ColumnSchema();
                newCol.ColumnName = "IsSync";
                newCol.ColumnType = "bit";
                newCol.Length = 0;
                newCol.IsNullable = true;
                newCol.IsIdentity = false;
                newCol.DefaultValue = "0";
                sb.Append(",\n");
                string cline1 = BuildColumnStatement(newCol, ts, ref pkey);
                sb.Append(cline1);
            }
            else if (strview[2] == "1" && ts.TableName == "VWSL_OrderDetails")
            {
                ColumnSchema newCol = new ColumnSchema();
                newCol.ColumnName = "IsSync";
                newCol.ColumnType = "bit";
                newCol.Length = 0;
                newCol.IsNullable = true;
                newCol.IsIdentity = false;
                newCol.DefaultValue = "1";
                sb.Append(",\n");
                string cline1 = BuildColumnStatement(newCol, ts, ref pkey);
                sb.Append(cline1);
            }

            // add primary keys...
            if (ts.PrimaryKey != null && ts.PrimaryKey.Count > 0 & !pkey)
            {
                sb.Append(",\n");
                sb.Append("    PRIMARY KEY (");
                for (int i = 0; i < ts.PrimaryKey.Count; i++)
                {
                    sb.Append("[" + ts.PrimaryKey[i] + "]");
                    if (i < ts.PrimaryKey.Count - 1)
                        sb.Append(", ");
                } // for
                sb.Append(")\n");
            }
            else
                sb.Append("\n");

            // add foreign keys...
            //if (ts.ForeignKeys.Count > 0)
            //{
            //    sb.Append(",\n");
            //    for (int i = 0; i < ts.ForeignKeys.Count; i++)
            //    {
            //        ForeignKeySchema foreignKey = ts.ForeignKeys[i];
            //        string stmt = string.Format("    FOREIGN KEY ([{0}])\n        REFERENCES [{1}]([{2}])",
            //                    foreignKey.ColumnName, foreignKey.ForeignTableName, foreignKey.ForeignColumnName);

            //        sb.Append(stmt);
            //        if (i < ts.ForeignKeys.Count - 1)
            //            sb.Append(",\n");
            //    } // for
            //}

            sb.Append("\n");
            sb.Append(");\n");

            // Create any relevant indexes
            //if (ts.Indexes != null)
            //{
            //    for (int i = 0; i < ts.Indexes.Count; i++)
            //    {
            //        string stmt = BuildCreateIndex(ts.TableName, ts.Indexes[i]);
            //        sb.Append(stmt + ";\n");
            //    } // for
            //} // if

            string query = sb.ToString();
            return query;
        }

        /// <summary>
        /// returns the CREATE TABLE DDL for creating the SQLite table from the specified
        /// table schema object.
        /// </summary>
        /// <param name="ts">The table schema object from which to create the SQL statement.</param>
        /// <returns>CREATE TABLE DDL for the specified table.</returns>
        private static string BuildCreateTableQuery(TableSchema ts, string query)
        {
            StringBuilder sb = new StringBuilder();
            if (ts.TableName != "LocalStore" && ts.TableName != "OrderHeader")
            {
                sb.Append("CREATE TABLE [" + ts.TableName + "] (\n");

                string[] strArr1 = null;
                string[] strArr2 = null;
                string[] strArr3 = null;
                string[] strArr4 = null;
                strArr1 = query.Split('@')[1].Split('#');
                strArr2 = query.Split('@')[2].Split();
                strArr4 = query.Split('@')[3].Split();
                strArr3 = query.Split('@')[3].Split(':');
                bool pkey = false;
                for (int j = 0; j < strArr1.Length; j++)
                {
                    for (int i = 0; i < ts.Columns.Count; i++)
                    {
                        if (ts.Columns[i].ColumnName.ToLower().Trim().Equals(strArr1[j].ToLower().Trim()))
                        {
                            ColumnSchema col = ts.Columns[i];
                            string cline = BuildColumnStatement(col, ts, ref pkey);
                            sb.Append(cline);
                            if (j < strArr1.Length - 1)
                                sb.Append(",\n");
                            break;
                        }
                    }
                } // foreach

                if (strArr4[0] != "")
                {
                    ColumnSchema newCol = new ColumnSchema();
                    newCol.ColumnName = strArr3[0];
                    newCol.ColumnType = strArr3[1];
                    newCol.Length = Convert.ToInt32(strArr3[2]);
                    newCol.IsNullable = Convert.ToBoolean(strArr3[3]);
                    newCol.IsIdentity = Convert.ToBoolean(strArr3[4]);
                    newCol.DefaultValue = strArr3[5];
                    sb.Append(",\n");
                    string cline1 = BuildColumnStatement(newCol, ts, ref pkey);
                    sb.Append(cline1);
                }

                if (strArr2[0] == "1" && ts.TableName != "NewCustomer")
                {
                    ColumnSchema newCol = new ColumnSchema();
                    newCol.ColumnName = "IsSync";
                    newCol.ColumnType = "bit";
                    newCol.Length = 0;
                    newCol.IsNullable = true;
                    newCol.IsIdentity = false;
                    newCol.DefaultValue = "0";
                    sb.Append(",\n");
                    string cline1 = BuildColumnStatement(newCol, ts, ref pkey);
                    sb.Append(cline1);
                }
                else if (strArr2[0] == "1" && ts.TableName == "NewCustomer")
                {
                    ColumnSchema newCol = new ColumnSchema();
                    newCol.ColumnName = "IsSync";
                    newCol.ColumnType = "bit";
                    newCol.Length = 0;
                    newCol.IsNullable = true;
                    newCol.IsIdentity = false;
                    newCol.DefaultValue = "1";
                    sb.Append(",\n");
                    string cline1 = BuildColumnStatement(newCol, ts, ref pkey);
                    sb.Append(cline1);
                }
                // add primary keys...
                if (ts.PrimaryKey != null && ts.PrimaryKey.Count > 0 & !pkey)
                {
                    for (int j = 0; j < strArr1.Length; j++)
                    {

                        //sb.Append(",\n");
                        //sb.Append("    PRIMARY KEY (");
                        for (int i = 0; i < ts.PrimaryKey.Count; i++)
                        {
                            if (ts.PrimaryKey[i].ToLower().Trim().Equals(strArr1[j].ToLower().Trim()))
                            {
                                sb.Append(",\n");
                                sb.Append("    PRIMARY KEY (");
                                sb.Append("[" + ts.PrimaryKey[i] + "]");
                                if (i < ts.PrimaryKey.Count - 1)
                                    sb.Append(", ");
                                sb.Append(")\n");
                                sb.Append("\n");
                            }
                        }
                        // sb.Append(")\n");
                        //sb.Append("\n");
                        //sb.Append(");\n");
                    } // for
                    //sb.Append(")\n");
                }
                else
                    sb.Append("\n");

                // add foreign keys...
                //if (ts.ForeignKeys.Count > 0)
                //{
                //    sb.Append(",\n");
                //    for (int i = 0; i < ts.ForeignKeys.Count; i++)
                //    {
                //        ForeignKeySchema foreignKey = ts.ForeignKeys[i];
                //        string stmt = string.Format("    FOREIGN KEY ([{0}])\n        REFERENCES [{1}]([{2}])",
                //                    foreignKey.ColumnName, foreignKey.ForeignTableName, foreignKey.ForeignColumnName);

                //        sb.Append(stmt);
                //        if (i < ts.ForeignKeys.Count - 1)
                //            sb.Append(",\n");
                //    } // for
                //}

                //sb.Append("\n");
                sb.Append(");\n");


            }
            else if (ts.TableName == "LocalStore")
            {
                sb.Append("CREATE TABLE [" + ts.TableName + "] (\n");

                string[] strArr1 = null;
                string[] strArr2 = null;
                string[] strArr3 = null;
                string[] strArr4 = null;
                strArr1 = query.Split('@')[1].Split('#');
                strArr2 = query.Split('@')[2].Split();
                strArr4 = query.Split('@')[3].Split();
                strArr3 = query.Split('@')[3].Split(':');
                bool pkey = false;
                for (int j = 0; j < strArr1.Length; j++)
                {
                    for (int i = 0; i < ts.Columns.Count; i++)
                    {
                        if (ts.Columns[i].ColumnName.ToLower().Trim().Equals(strArr1[j].ToLower().Trim()))
                        {
                            ColumnSchema col = ts.Columns[i];
                            string cline = BuildColumnStatement(col, ts, ref pkey);
                            sb.Append(cline);
                            if (j < strArr1.Length - 1)
                                sb.Append(",\n");
                            break;
                        }
                    }
                } // foreach

                sb.Append(",\n");
                string str = "\"" + "ShopName" + "\"";
                sb.Append(str);
                sb.Append("		varchar(50) NOT NULL");

                if (strArr4[0] != "")
                {
                    ColumnSchema newCol = new ColumnSchema();
                    newCol.ColumnName = strArr3[0];
                    newCol.ColumnType = strArr3[1];
                    newCol.Length = Convert.ToInt32(strArr3[2]);
                    newCol.IsNullable = Convert.ToBoolean(strArr3[3]);
                    newCol.IsIdentity = Convert.ToBoolean(strArr3[4]);
                    newCol.DefaultValue = strArr3[5];
                    sb.Append(",\n");
                    string cline1 = BuildColumnStatement(newCol, ts, ref pkey);
                    sb.Append(cline1);
                }

                if (strArr2[0] == "1")
                {
                    ColumnSchema newCol = new ColumnSchema();
                    newCol.ColumnName = "IsSync";
                    newCol.ColumnType = "bit";
                    newCol.Length = 0;
                    newCol.IsNullable = true;
                    newCol.IsIdentity = false;
                    newCol.DefaultValue = "0";
                    sb.Append(",\n");
                    string cline1 = BuildColumnStatement(newCol, ts, ref pkey);
                    sb.Append(cline1);
                }

                // add primary keys...
                if (ts.PrimaryKey != null && ts.PrimaryKey.Count > 0 & !pkey)
                {
                    for (int j = 0; j < strArr1.Length; j++)
                    {


                        for (int i = 0; i < ts.PrimaryKey.Count; i++)
                        {
                            if (ts.PrimaryKey[i].ToLower().Trim().Equals(strArr1[j].ToLower().Trim()))
                            {
                                sb.Append(",\n");
                                sb.Append("    PRIMARY KEY (");
                                sb.Append("[" + ts.PrimaryKey[i] + "]");
                                if (i < ts.PrimaryKey.Count - 1)
                                    sb.Append(", ");
                                sb.Append(")\n");
                                sb.Append("\n");
                            }
                        }

                    } // for
                    //sb.Append(")\n");
                }
                else
                    //sb.Append("\n");
                    //sb.Append(",\n");
                    //string str =  "\""+"ShopName"+"\"";
                    //sb.Append(str);
                    //sb.Append("		varchar(50) NOT NULL");

                    sb.Append(");\n");
            }
            else if (ts.TableName == "OrderHeader")
            {
                sb.Append("CREATE TABLE [" + ts.TableName + "] (\n");

                string[] strArr1 = null;
                string[] strArr2 = null;
                string[] strArr3 = null;
                string[] strArr4 = null;
                strArr1 = query.Split('@')[1].Split('#');
                strArr2 = query.Split('@')[2].Split();
                strArr4 = query.Split('@')[3].Split();
                strArr3 = query.Split('@')[3].Split(':');
                bool pkey = false;
                for (int j = 0; j < strArr1.Length; j++)
                {
                    for (int i = 0; i < ts.Columns.Count; i++)
                    {
                        if (ts.Columns[i].ColumnName.ToLower().Trim().Equals(strArr1[j].ToLower().Trim()))
                        {
                            ColumnSchema col = ts.Columns[i];
                            string cline = BuildColumnStatement(col, ts, ref pkey);
                            sb.Append(cline);
                            if (j < strArr1.Length - 1)
                                sb.Append(",\n");
                            break;
                        }
                    }
                } // foreach

                if (strArr4[0] != "")
                {
                    ColumnSchema newCol = new ColumnSchema();
                    newCol.ColumnName = strArr3[0];
                    newCol.ColumnType = strArr3[1];
                    newCol.Length = Convert.ToInt32(strArr3[2]);
                    newCol.IsNullable = Convert.ToBoolean(strArr3[3]);
                    newCol.IsIdentity = Convert.ToBoolean(strArr3[4]);
                    newCol.DefaultValue = strArr3[5];
                    sb.Append(",\n");
                    string cline1 = BuildColumnStatement(newCol, ts, ref pkey);
                    sb.Append(cline1);
                }

                if (strArr2[0] == "1")
                {
                    ColumnSchema newCol = new ColumnSchema();
                    newCol.ColumnName = "IsSync";
                    newCol.ColumnType = "bit";
                    newCol.Length = 0;
                    newCol.IsNullable = true;
                    newCol.IsIdentity = false;
                    newCol.DefaultValue = "0";
                    sb.Append(",\n");
                    string cline1 = BuildColumnStatement(newCol, ts, ref pkey);
                    sb.Append(cline1);
                }

                // add primary keys...
                if (ts.PrimaryKey != null && ts.PrimaryKey.Count > 0 & !pkey)
                {
                    for (int j = 0; j < strArr1.Length; j++)
                    {
                        for (int i = 0; i < ts.PrimaryKey.Count; i++)
                        {
                            if (ts.PrimaryKey[i].ToLower().Trim().Equals(strArr1[j].ToLower().Trim()))
                            {
                                sb.Append(",\n");
                                sb.Append("    PRIMARY KEY (");
                                sb.Append("[" + ts.PrimaryKey[i] + "]");
                                if (i < ts.PrimaryKey.Count - 1)
                                    sb.Append(", ");
                                sb.Append(")\n");
                                sb.Append("\n");
                            }
                        }

                    } // for
                    //sb.Append(")\n");
                }
                else
                    //sb.Append("\n");
                    //sb.Append(",\n");
                    //string str =  "\""+"ShopName"+"\"";
                    //sb.Append(str);
                    //sb.Append("		varchar(50) NOT NULL");

                    sb.Append(");\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Creates a CREATE INDEX DDL for the specified table and index schema.
        /// </summary>
        /// <param name="tableName">The name of the indexed table.</param>
        /// <param name="indexSchema">The schema of the index object</param>
        /// <returns>A CREATE INDEX DDL (SQLite format).</returns>
        private static string BuildCreateIndex(string tableName, IndexSchema indexSchema, string[] tableColumnsArray)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE ");
            if (indexSchema.IsUnique)
                sb.Append("UNIQUE ");
            sb.Append("INDEX [" + tableName + "_" + indexSchema.IndexName + "]\n");
            sb.Append("ON [" + tableName + "]\n");
            sb.Append("(");

            for (int j = 0; j < tableColumnsArray.Length; j++)
            {
                for (int i = 0; i < indexSchema.Columns.Count; i++)
                {
                    if (indexSchema.Columns[i].ColumnName.ToLower().Trim().Equals(tableColumnsArray[j].ToLower().Trim()))
                    {
                        sb.Append("[" + indexSchema.Columns[i].ColumnName + "]");
                        if (!indexSchema.Columns[i].IsAscending)
                            sb.Append(" DESC");
                        if (i < indexSchema.Columns.Count - 1)
                            sb.Append(", ");
                        break;
                    }
                } // for
            }
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Used when creating the CREATE TABLE DDL. Creates a single row
        /// for the specified column.
        /// </summary>
        /// <param name="col">The column schema</param>
        /// <returns>A single column line to be inserted into the general CREATE TABLE DDL statement</returns>
        private static string BuildColumnStatement(ColumnSchema col, TableSchema ts, ref bool pkey)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\t\"" + col.ColumnName + "\"\t\t");

            // Special treatment for IDENTITY columns
            if (col.IsIdentity)
            {
                if (ts.PrimaryKey.Count == 1 && (col.ColumnType == "tinyint" || col.ColumnType == "int" || col.ColumnType == "smallint" ||
                    col.ColumnType == "bigint" || col.ColumnType == "integer"))
                {
                    sb.Append("integer PRIMARY KEY AUTOINCREMENT");
                    pkey = true;
                }
                else
                    sb.Append("integer");
            }
            else
            {
                if (col.ColumnType == "int")
                    sb.Append("integer");
                else
                {
                    sb.Append(col.ColumnType);
                }
                if (col.Length > 0)
                    sb.Append("(" + col.Length + ")");
            }
            if (!col.IsNullable)
                sb.Append(" NOT NULL");

            if (col.IsCaseSensitivite.HasValue && !col.IsCaseSensitivite.Value)
                sb.Append(" COLLATE NOCASE");

            string defval = StripParens(col.DefaultValue);
            defval = DiscardNational(defval);
            // _log.LogMessage("BuildColumnStatement()", "DEFAULT VALUE BEFORE [" + col.DefaultValue + "] AFTER [" + defval + "]", "");
            if (defval != string.Empty && defval.ToUpper().Contains("GETDATE"))
            {
                //_log.LogMessage("BuildColumnStatement()", "converted SQL Server GETDATE() to CURRENT_TIMESTAMP for column [" + col.ColumnName + "]", "");
                sb.Append(" DEFAULT (CURRENT_TIMESTAMP)");
            }
            else if (defval != string.Empty && IsValidDefaultValue(defval))
                sb.Append(" DEFAULT " + defval);
            return sb.ToString();
        }

        /// <summary>
        /// Discards the national prefix if exists (e.g., N'sometext') which is not
        /// supported in SQLite.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string DiscardNational(string value)
        {
            Regex rx = new Regex(@"N\'([^\']*)\'");
            Match m = rx.Match(value);
            if (m.Success)
                return m.Groups[1].Value;
            else
                return value;
        }

        /// <summary>
        /// Check if the DEFAULT clause is valid by SQLite standards
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsValidDefaultValue(string value)
        {
            if (IsSingleQuoted(value))
                return true;
            if (!IsSingleQuoted(value))
                return true;

            double testnum;
            if (!double.TryParse(value, out testnum))
                return false;
            return true;
        }

        private static bool IsSingleQuoted(string value)
        {
            value = value.Trim();
            if (value.StartsWith("'") && value.EndsWith("'"))
                return true;
            return false;
        }

        /// <summary>
        /// Strip any parentheses from the string.
        /// </summary>
        /// <param name="value">The string to strip</param>
        /// <returns>The stripped string</returns>
        private static string StripParens(string value)
        {
            Regex rx = new Regex(@"\(([^\)]*)\)");
            Match m = rx.Match(value);
            if (!m.Success)
                return value;
            else
                return StripParens(m.Groups[1].Value);
        }

        /// <summary>
        /// Reads the entire SQL Server DB schema using the specified connection string.
        /// </summary>
        /// <param name="connString">The connection string used for reading SQL Server schema.</param>
        /// <param name="handler">A handler for progress notifications.</param>
        /// <param name="selectionHandler">The selection handler which allows the user to select 
        /// which tables to convert.</param>
        /// <returns>database schema objects for every table/view in the SQL Server database.</returns>
        private static DatabaseSchema ReadSqlServerSchema(string connString, Dictionary<string, string> query, List<string> viewNameList)
        {
            try
            {
                // First step is to read the names of all tables in the database
                List<TableSchema> tables = new List<TableSchema>();
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    List<string> tableNames = new List<string>();
                    List<string> tblschema = new List<string>();

                    // This command will read the names of all tables in the database

                    string[] keys = new string[query.Keys.Count];
                    int index = 0;
                    foreach (KeyValuePair<String, String> entry in query)
                    {
                        keys[index++] = entry.Key.Split('@')[0];
                    }
                    string keysJoined = string.Join("','", keys);

                    //To get the names of all tables which are in the select list.
                    SqlCommand cmd = new SqlCommand(@"select * from INFORMATION_SCHEMA.TABLES  where TABLE_TYPE = 'BASE TABLE' and TABLE_NAME in ('" + keysJoined + "')", conn);

                    SqlDataAdapter dataAdaptSchema = new SqlDataAdapter();
                    dataAdaptSchema.SelectCommand = cmd;
                    DataTable dtSchema = new DataTable();
                    dataAdaptSchema.Fill(dtSchema);


                    foreach (DataRow drrow in dtSchema.Rows)
                    {
                        tableNames.Add((string)drrow["TABLE_NAME"]);
                        tblschema.Add((string)drrow["TABLE_SCHEMA"]);
                    } // foreach


                    // Next step is to use ADO APIs to query the schema of each table.
                    int count = 0;
                    for (int i = 0; i < tableNames.Count; i++)
                    {
                        string tname = tableNames[i];
                        string tschma = tblschema[i];
                        TableSchema ts = CreateTableSchema(conn, tname, tschma);

                        //CreateForeignKeySchema(conn, ts);
                        tables.Add(ts);
                        count++;
                        //CheckCancelled();
                        //handler(false, true, (int)(count * 50.0 / tableNames.Count), "Parsed table " + tname);

                        // _log.LogMessage("ReadSqlServerSchema()", "parsed table schema for [" + tname + "]", "");
                    } // foreach
                } // using

                // _log.LogMessage("ReadSqlServerSchema()", "finished parsing all tables in SQL Server schema", "");



                Regex removedbo = new Regex(@"dbo\.", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                // Continue and read all of the views in the database
                List<ViewSchema> views = new List<ViewSchema>();
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string viewnames = "";
                    string[] strArrvw = null;

                    foreach (string item in viewNameList)
                    {
                        strArrvw = item.Split('#');
                        if (string.IsNullOrEmpty(viewnames))
                        {
                            viewnames = strArrvw[0];
                        }
                        else
                        {
                            viewnames = viewnames + "','" + strArrvw[0];
                        }
                    }
                    SqlCommand cmd = new SqlCommand(@"SELECT TABLE_NAME, VIEW_DEFINITION  from INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME IN ('" + viewnames + "')", conn);
                    SqlDataAdapter dataAdaptSchema = new SqlDataAdapter();
                    dataAdaptSchema.SelectCommand = cmd;
                    DataTable dtSchema = new DataTable();
                    dataAdaptSchema.Fill(dtSchema);

                    int count = 0;
                    foreach (DataRow dr in dtSchema.Rows)
                    {
                        ViewSchema vs = new ViewSchema();
                        vs.ViewName = (string)dr["TABLE_NAME"];
                        vs.ViewSQL = (string)dr["VIEW_DEFINITION"];

                        // Remove all ".dbo" strings from the view definition
                        vs.ViewSQL = removedbo.Replace(vs.ViewSQL, string.Empty);

                        views.Add(vs);

                        count++;
                        //CheckCancelled();

                        //_log.LogMessage("ReadSqlServerSchema()", "parsed view schema for [" + vs.ViewName + "]", "L2");
                    } // foreach



                    int countView = 0;
                    for (int i = 0; i < views.Count; i++)
                    {
                        string tname = views[i].ViewName;
                        string tschma = "dbo";
                        TableSchema ts = CreateTableSchema(conn, tname, tschma);
                        //CreateForeignKeySchema(conn, ts);
                        tables.Add(ts);
                        countView++;
                        //CheckCancelled();

                        // _log.LogMessage("", "parsed New View schema for [" + tname + "]", "L2");
                    }

                } // using


                DatabaseSchema ds = new DatabaseSchema();
                ds.Tables = tables;
                //ds.Views = views;
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Convenience method for checking if the conversion progress needs to be cancelled.
        /// </summary>
        //private static void CheckCancelled()
        //{
        //    if (_cancelled)
        //        throw new ApplicationException("User cancelled the conversion");
        //}

        /// <summary>
        /// Creates a TableSchema object using the specified SQL Server connection
        /// and the name of the table for which we need to create the schema.
        /// </summary>
        /// <param name="conn">The SQL Server connection to use</param>
        /// <param name="tableName">The name of the table for which we wants to create the table schema.</param>
        /// <returns>A table schema object that represents our knowledge of the table schema</returns>
        private static TableSchema CreateTableSchema(SqlConnection conn, string tableName, string tschma)
        {
            TableSchema res = new TableSchema();
            res.TableName = tableName;
            res.TableSchemaName = tschma;
            res.Columns = new List<ColumnSchema>();
            SqlCommand cmd = new SqlCommand(@"SELECT COLUMN_NAME,COLUMN_DEFAULT,IS_NULLABLE,DATA_TYPE, " +
                @" (columnproperty(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity')) AS [IDENT], " +
                @"CHARACTER_MAXIMUM_LENGTH AS CSIZE " +
                "FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName + "' ORDER BY " +
                "ORDINAL_POSITION ASC", conn);

            SqlDataAdapter dataAdapt = new SqlDataAdapter();
            dataAdapt.SelectCommand = cmd;
            DataTable dtSchema = new DataTable();
            dataAdapt.Fill(dtSchema);

            foreach (DataRow dr in dtSchema.Rows)
            {
                object tmp = dr["COLUMN_NAME"];
                if (tmp is DBNull)
                    continue;
                string colName = (string)dr["COLUMN_NAME"];

                tmp = dr["COLUMN_DEFAULT"];
                string colDefault;
                if (tmp is DBNull)
                    colDefault = string.Empty;
                else
                    colDefault = (string)tmp;

                tmp = dr["IS_NULLABLE"];
                bool isNullable = ((string)tmp == "YES");
                string dataType = (string)dr["DATA_TYPE"];
                bool isIdentity = false;
                if (dr["IDENT"] != DBNull.Value)
                    isIdentity = ((int)dr["IDENT"]) == 1 ? true : false;
                int length = dr["CSIZE"] != DBNull.Value ? Convert.ToInt32(dr["CSIZE"]) : 0;

                ValidateDataType(dataType);

                // Note that not all data type names need to be converted because
                // SQLite establishes type affinity by searching certain strings
                // in the type name. For example - everything containing the string
                // 'int' in its type name will be assigned an INTEGER affinity
                if (dataType == "timestamp")
                    dataType = "blob";
                else if (dataType == "datetime" || dataType == "smalldatetime")
                    dataType = "datetime";
                else if (dataType == "decimal")
                    dataType = "numeric";
                else if (dataType == "money" || dataType == "smallmoney")
                    dataType = "numeric";
                else if (dataType == "binary" || dataType == "varbinary" ||
                    dataType == "image")
                    dataType = "blob";
                else if (dataType == "tinyint")
                    dataType = "smallint";
                else if (dataType == "bigint")
                    dataType = "integer";
                else if (dataType == "sql_variant")
                    dataType = "blob";
                else if (dataType == "xml")
                    dataType = "varchar";
                else if (dataType == "uniqueidentifier")
                    dataType = "guid";
                else if (dataType == "ntext")
                    dataType = "text";
                else if (dataType == "nchar")
                    dataType = "char";

                if (dataType == "bit" || dataType == "int")
                {
                    if (colDefault == "('False')")
                        colDefault = "(0)";
                    else if (colDefault == "('True')")
                        colDefault = "(1)";
                }

                colDefault = FixDefaultValueString(colDefault);

                ColumnSchema col = new ColumnSchema();
                col.ColumnName = colName;
                col.ColumnType = dataType;
                col.Length = length;
                col.IsNullable = isNullable;
                col.IsIdentity = isIdentity;
                col.DefaultValue = AdjustDefaultValue(colDefault);
                res.Columns.Add(col);
            } // while


            // Find PRIMARY KEY information
            SqlCommand cmd2 = new SqlCommand(@"EXEC sp_pkeys '" + tableName + "'", conn);

            SqlDataAdapter dtAdapt2 = new SqlDataAdapter();
            dtAdapt2.SelectCommand = cmd2;
            DataTable dtSchema2 = new DataTable();
            dtAdapt2.Fill(dtSchema2);


            res.PrimaryKey = new List<string>();
            foreach (DataRow dr in dtSchema2.Rows)
            {
                string colName = (string)dr["COLUMN_NAME"];
                res.PrimaryKey.Add(colName);
            } // foreach


            // Find COLLATE information for all columns in the table
            SqlCommand cmd4 = new SqlCommand(
                @"EXEC sp_tablecollations '" + tschma + "." + tableName + "'", conn);

            SqlDataAdapter dtAdapter = new SqlDataAdapter();
            dtAdapter.SelectCommand = cmd4;
            DataTable dttable = new DataTable();
            dtAdapter.Fill(dttable);

            foreach (DataRow dr in dttable.Rows)
            {
                bool? isCaseSensitive = null;
                string colName = (string)dr["name"];
                if (dr["tds_collation"] != DBNull.Value)
                {
                    byte[] mask = (byte[])dr["tds_collation"];
                    if ((mask[2] & 0x10) != 0)
                        isCaseSensitive = false;
                    else
                        isCaseSensitive = true;
                } // if

                if (isCaseSensitive.HasValue)
                {
                    // Update the corresponding column schema.
                    foreach (ColumnSchema csc in res.Columns)
                    {
                        if (csc.ColumnName == colName)
                        {
                            csc.IsCaseSensitivite = isCaseSensitive;
                            break;
                        }
                    } // foreach
                } // if
            } // while


            try
            {
                // Find index information
                SqlCommand cmd3 = new SqlCommand(
                    @"exec sp_helpindex '" + tschma + "." + tableName + "'", conn);
                SqlDataAdapter datatAdapter = new SqlDataAdapter();
                datatAdapter.SelectCommand = cmd3;
                DataTable dataTable = new DataTable();
                datatAdapter.Fill(dataTable);


                res.Indexes = new List<IndexSchema>();
                foreach (DataRow dr in dataTable.Rows)
                {
                    string indexName = (string)dr["index_name"];
                    string desc = (string)dr["index_description"];
                    string keys = (string)dr["index_keys"];

                    // Don't add the index if it is actually a primary key index
                    //if (desc.Contains("primary key"))
                    //    continue;



                    IndexSchema index = BuildIndexSchema(indexName, desc, keys);
                    res.Indexes.Add(index);
                } // while

            }
            catch (Exception ex)
            {
                _log.LogMessage("failed to read index information for table [" + tableName + "] in method CreateTableSchema()", ex.ToString(), "L2");
            } // catch

            return res;
        }

        /// <summary>
        /// Small validation method to make sure we don't miss anything without getting
        /// an exception.
        /// </summary>
        /// <param name="dataType">The datatype to validate.</param>
        private static void ValidateDataType(string dataType)
        {
            if (dataType == "int" || dataType == "smallint" ||
                dataType == "bit" || dataType == "float" ||
                dataType == "real" || dataType == "nvarchar" ||
                dataType == "varchar" || dataType == "timestamp" ||
                dataType == "varbinary" || dataType == "image" ||
                dataType == "text" || dataType == "ntext" ||
                dataType == "bigint" ||
                dataType == "char" || dataType == "numeric" ||
                dataType == "binary" || dataType == "smalldatetime" ||
                dataType == "smallmoney" || dataType == "money" ||
                dataType == "tinyint" || dataType == "uniqueidentifier" ||
                dataType == "xml" || dataType == "sql_variant" ||
                dataType == "decimal" || dataType == "nchar" || dataType == "datetime")
                return;
            throw new ApplicationException("Validation failed for data type [" + dataType + "]");
        }

        /// <summary>
        /// Does some necessary adjustments to a value string that appears in a column DEFAULT
        /// clause.
        /// </summary>
        /// <param name="colDefault">The original default value string (as read from SQL Server).</param>
        /// <returns>Adjusted DEFAULT value string (for SQLite)</returns>
        private static string FixDefaultValueString(string colDefault)
        {
            bool replaced = false;
            string res = colDefault.Trim();

            // Find first/last indexes in which to search
            int first = -1;
            int last = -1;
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] == '\'' && first == -1)
                    first = i;
                if (res[i] == '\'' && first != -1 && i > last)
                    last = i;
            } // for

            if (first != -1 && last > first)
                return res.Substring(first, last - first + 1);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] != '(' && res[i] != ')')
                {
                    sb.Append(res[i]);
                    replaced = true;
                }
            }
            if (replaced)
                return "(" + sb.ToString() + ")";
            else
                return sb.ToString();
        }



        /// <summary>
        /// Add foreign key schema object from the specified components (Read from SQL Server).
        /// </summary>
        /// <param name="conn">The SQL Server connection to use</param>
        /// <param name="ts">The table schema to whom foreign key schema should be added to</param>
        private static void CreateForeignKeySchema(SqlConnection conn, TableSchema ts)
        {
            ts.ForeignKeys = new List<ForeignKeySchema>();

            SqlCommand cmd = new SqlCommand(
                @"SELECT " +
                @"  ColumnName = CU.COLUMN_NAME, " +
                @"  ForeignTableName  = PK.TABLE_NAME, " +
                @"  ForeignColumnName = PT.COLUMN_NAME, " +
                @"  DeleteRule = C.DELETE_RULE, " +
                @"  IsNullable = COL.IS_NULLABLE " +
                @"FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C " +
                @"INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME " +
                @"INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME " +
                @"INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME " +
                @"INNER JOIN " +
                @"  ( " +
                @"    SELECT i1.TABLE_NAME, i2.COLUMN_NAME " +
                @"    FROM  INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 " +
                @"    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME " +
                @"    WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY' " +
                @"  ) " +
                @"PT ON PT.TABLE_NAME = PK.TABLE_NAME " +
                @"INNER JOIN INFORMATION_SCHEMA.COLUMNS AS COL ON CU.COLUMN_NAME = COL.COLUMN_NAME AND FK.TABLE_NAME = COL.TABLE_NAME " +
                @"WHERE FK.Table_NAME='" + ts.TableName + "'", conn);
            SqlDataAdapter datatAdapter = new SqlDataAdapter();
            datatAdapter.SelectCommand = cmd;
            DataTable dataTable = new DataTable();
            datatAdapter.Fill(dataTable);


            foreach (DataRow dr in dataTable.Rows)
            {
                ForeignKeySchema fkc = new ForeignKeySchema();
                fkc.ColumnName = (string)dr["ColumnName"];
                fkc.ForeignTableName = (string)dr["ForeignTableName"];
                fkc.ForeignColumnName = (string)dr["ForeignColumnName"];
                fkc.CascadeOnDelete = (string)dr["DeleteRule"] == "CASCADE";
                fkc.IsNullable = (string)dr["IsNullable"] == "YES";
                fkc.TableName = ts.TableName;
                ts.ForeignKeys.Add(fkc);
            }

        }

        /// <summary>
        /// Builds an index schema object from the specified components (Read from SQL Server).
        /// </summary>
        /// <param name="indexName">The name of the index</param>
        /// <param name="desc">The description of the index</param>
        /// <param name="keys">Key columns that are part of the index.</param>
        /// <returns>An index schema object that represents our knowledge of the index</returns>
        private static IndexSchema BuildIndexSchema(string indexName, string desc, string keys)
        {
            IndexSchema res = new IndexSchema();
            res.IndexName = indexName;

            // Determine if this is a unique index or not.
            string[] descParts = desc.Split(',');
            foreach (string p in descParts)
            {
                if (p.Trim().Contains("unique"))
                {
                    res.IsUnique = true;
                    break;
                }
            } // foreach

            // Get all key names and check if they are ASCENDING or DESCENDING
            res.Columns = new List<IndexColumn>();
            string[] keysParts = keys.Split(',');
            foreach (string p in keysParts)
            {
                Match m = _keyRx.Match(p);
                if (!m.Success)
                {
                    throw new ApplicationException("Illegal key name [" + p + "] in index [" +
                        indexName + "]");
                }

                string key = m.Groups[1].Value;
                IndexColumn ic = new IndexColumn();
                ic.ColumnName = key;
                if (m.Groups[2].Success)
                    ic.IsAscending = false;
                else
                    ic.IsAscending = true;

                res.Columns.Add(ic);
            } // foreach

            return res;
        }

        /// <summary>
        /// More adjustments for the DEFAULT value clause.
        /// </summary>
        /// <param name="val">The value to adjust</param>
        /// <returns>Adjusted DEFAULT value string</returns>
        private static string AdjustDefaultValue(string val)
        {
            if (val == null || val == string.Empty)
                return val;

            Match m = _defaultValueRx.Match(val);
            if (m.Success)
                return m.Groups[1].Value;
            return val;
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
        #endregion

        #region Trigger related
        private static void AddTriggersForForeignKeys(string sqlitePath, IEnumerable<TableSchema> schema,
            string password)
        {
            // Connect to the newly created database
            string sqliteConnString = CreateSQLiteConnectionString(sqlitePath);
            using (SQLiteConnection conn = new SQLiteConnection(sqliteConnString))
            {
                conn.Open();
                // foreach
                foreach (TableSchema dt in schema)
                {
                    try
                    {
                        AddTableTriggers(conn, dt);
                    }
                    catch (Exception ex)
                    {
                        _log.LogMessage("AddTableTriggers failed in AddTriggersForForeignKeys()", ex.ToString(), "L2");
                        //  _log.Error("AddTableTriggers failed", ex);
                        throw;
                    }
                }

            } // using

            // _log.Debug("finished adding triggers to schema");
        }

        private static void AddTableTriggers(SQLiteConnection conn, TableSchema dt)
        {
            IList<TriggerSchema> triggers = TriggerBuilder.GetForeignKeyTriggers(dt);
            foreach (TriggerSchema trigger in triggers)
            {
                SQLiteCommand cmd = new SQLiteCommand(WriteTriggerSchema(trigger), conn);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        /// <summary>
        /// Gets a create script for the triggerSchema in sqlite syntax
        /// </summary>
        /// <param name="ts">Trigger to script</param>
        /// <returns>Executable script</returns>
        public static string WriteTriggerSchema(TriggerSchema ts)
        {
            return @"CREATE TRIGGER [" + ts.Name + "] " +
                   ts.Type + " " + ts.Event +
                   " ON [" + ts.Table + "] " +
                   "BEGIN " + ts.Body + " END;";
        }

        #region Private Variables
        //private static bool _isActive = false;
        //private static bool _cancelled = false;
        private static Regex _keyRx = new Regex(@"([a-zA-Z_0-9]+)(\(\-\))?");
        private static Regex _defaultValueRx = new Regex(@"\(N(\'.*\')\)");
        //private static ILog _log = LogManager.GetLogger(typeof(SqlServerToSQLite));
        #endregion
    }

    /// <summary>
    /// This handler is called whenever a progress is made in the conversion process.
    /// </summary>
    /// <param name="done">TRUE indicates that the entire conversion process is finished.</param>
    /// <param name="success">TRUE indicates that the current step finished successfully.</param>
    /// <param name="percent">Progress percent (0-100)</param>
    /// <param name="msg">A message that accompanies the progress.</param>
    public delegate void SqlConversionHandler(bool done, bool success, int percent, string msg);

    /// <summary>
    /// This handler allows the user to change which tables get converted from SQL Server
    /// to SQLite.
    /// </summary>
    /// <param name="schema">The original SQL Server DB schema</param>
    /// <returns>The same schema minus any table we don't want to convert.</returns>
    public delegate List<TableSchema> SqlTableSelectionHandler(List<TableSchema> schema);

    /// <summary>
    /// This handler is called in order to handle the case when copying the SQL Server view SQL
    /// statement is not enough and the user needs to either update the view definition himself
    /// or discard the view definition from the generated SQLite database.
    /// </summary>
    /// <param name="vs">The problematic view definition</param>
    /// <returns>The updated view definition, or NULL in case the view should be discarded</returns>
    public delegate string FailedViewDefinitionHandler(ViewSchema vs);
}
