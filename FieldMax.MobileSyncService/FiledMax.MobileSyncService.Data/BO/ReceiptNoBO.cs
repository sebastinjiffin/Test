﻿using System.Data;
using Infocean.DataAccessHelper;
using System.Data.SqlClient;
using FieldMax.MobileSyncService.Data.DAO;
using System;

namespace FieldMax.MobileSyncService.Data.BO
{
    public class ReceiptNoBO : BOBase
    {
        #region Properties
        ReceiptNoDAO receiptNoDAO = new ReceiptNoDAO();

        public int UserId { get; set; }
        public string ReceiptNo { get; set; }
        public int Flag { get; set; }
        #endregion



        #region Methods

        public DataSet GetReceiptNoDetails()
        {
            return receiptNoDAO.GetReceiptNoDetails(this);
        }
        public int UpdateReceiptNo()
        {
            return receiptNoDAO.UpdateReceiptNo(this);
        }
        #endregion
    }
}

