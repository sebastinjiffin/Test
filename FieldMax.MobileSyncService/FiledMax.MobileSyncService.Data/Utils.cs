using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Globalization;

namespace FieldMax.MobileSyncService.Data
{

    public static class Utils
    {
        /// <summary>
        /// Function encrypts the text passed. 
        /// </summary>
        /// <param name="theTextToEncrypt">Content to encrypt</param>
        /// <returns>Encrypted contents</returns>
        public static string EncryptText(string theTextToEncrypt)
        {
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] key = Encoding.UTF8.GetBytes(Constants.SECRET_KEY);
            byte[] InputByteArray = System.Text.Encoding.UTF8.GetBytes(theTextToEncrypt);

            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateEncryptor(key, IV), CryptoStreamMode.Write);
            cryptoStream.Write(InputByteArray, 0, InputByteArray.Length);
            cryptoStream.FlushFinalBlock();
            return Convert.ToBase64String(memStream.ToArray());
        }

        /// <summary>
        /// Decrypted a previously encrypted value.
        /// </summary>
        /// <param name="theTextToDecrypt">encrypted byte array</param>
        /// <returns>Decrypted contents</returns>
        public static string DecryptText(string theTextToDecrypt)
        {
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] key = Encoding.UTF8.GetBytes(Constants.SECRET_KEY);
            byte[] InputByteArray = Convert.FromBase64String(theTextToDecrypt);

            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateDecryptor(key, IV), CryptoStreamMode.Write);
            cryptoStream.Write(InputByteArray, 0, InputByteArray.Length);
            cryptoStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(memStream.ToArray(), 0, (int)memStream.Length);
        }

        /// <summary>
        /// Convert Date String to Dateime Format.
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static DateTime ConvertStringToDate(string strDate)
        {
            return Convert.ToDateTime(strDate);
        }

        /// <summary>
        /// Convert Date time to Given string format.
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ConvertDateToString(DateTime Date, string format)
        {
            return Date.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert Date time to string format (dd MMM yyyy Ex: 01 Mar 2011).
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static string ConvertDateToString(DateTime Date)
        {
            return Date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert Date time to string format for Mobile (MMM dd yyyy Ex: Mar 01 2011).
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static string ConvertDateToStringForMobile(DateTime Date)
        {
            return Date.ToString("MMM dd yyyy");
        }

    }
}
