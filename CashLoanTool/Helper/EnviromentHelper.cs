using CashLoanTool.EntityModels;
using NLog;
using System;
using System.Text.RegularExpressions;

namespace CashLoanTool.Helper
{
    public static class EnviromentHelper
    {
        public static string ConnectionStringKey = "DbConnectionString";
        public static string ApiUrlKey = "HDB_API";
        public static string RootPath { get; set; }
        public static string GetDocumentFullPath(string fileName, string docFolder)
        {
            return $"{RootPath}\\{docFolder}\\{fileName}";
        }
        public static CustomerInfo StripCustomerAccentsNSpecialChars(CustomerInfo customer)
        {
            //TODO: maybe missing some fields
            customer.FullName = RemoveAccentsNSpecialCharsButSpace(customer.FullName);
            customer.ContactAddress = RemoveAccentsNSpecialCharsButSpace(customer.ContactAddress);
            customer.HomeAddress = RemoveAccentsNSpecialCharsButSpace(customer.HomeAddress);
            customer.Professional = RemoveAccentsNSpecialCharsButSpace(customer.Professional);
            customer.Position = RemoveAccentsNSpecialCharsButSpace(customer.Position);
            customer.Pob = RemoveAccentsNSpecialCharsButSpace(customer.Pob);
            customer.Issuer = RemoveAccentsNSpecialCharsButSpace(customer.Issuer);
            customer.CompanyName = RemoveAccentsNSpecialCharsButSpace(customer.CompanyName);
            customer.CompanyAddress = RemoveAccentsNSpecialCharsButSpace(customer.CompanyAddress);
            return customer;
        }
        public static string RemoveAccentsNSpecialCharsButSpace(string text)
        {
            if (text == null) return null;
            return ReplaceSpecialCharsWithSpace(RemoveVietnameseAccents(text));
        }
        public static string ReplaceSpecialCharsWithSpace(string text)
        {
            if (text == null) return null;
            return Regex.Replace(text, @"[^0-9a-zA-Z ]+", " ");
        }
        public static string RemoveSpecialChars(string text)
        {
            if (text == null) return null;
            return Regex.Replace(text, @"[^0-9a-zA-Z]+", string.Empty);
        }
        public static string RemoveVietnameseAccents(string text)
        {
            if (text == null) return null;
            for (int i = 0; i < _accents.Length; i++)
            {
                text = text.Replace(_accents[i], _replacement[i]);
                text = text.Replace(_accents[i].ToUpper(), _replacement[i].ToUpper());
            }
            return text;
        }

        private static string[] _accents = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ",
                "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                "í","ì","ỉ","ĩ","ị",
                "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                "ý","ỳ","ỷ","ỹ","ỵ",};
        private static string[] _replacement = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d",
                "e","e","e","e","e","e","e","e","e","e","e",
                "i","i","i","i","i",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                "u","u","u","u","u","u","u","u","u","u","u",
                "y","y","y","y","y",};
        public static void LogException(Exception ex, Logger logger)
        {
            logger.Error(ex.GetType().ToString());
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
            if (ex.InnerException != null)
            {
                logger.Error("Inner Ex:");
                LogException(ex.InnerException, logger);
            }
        }
    }
}
