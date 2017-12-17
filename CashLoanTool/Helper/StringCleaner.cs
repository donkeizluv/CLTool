﻿using CashLoanTool.EntityModels;
using System.Text.RegularExpressions;

namespace CashLoanTool.Helper
{
    public static class StringCleaner
    {
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

        public static CustomerInfo StripAccentsNSpecialChars(CustomerInfo customer)
        {
            //TODO: maybe missing some fields
            customer.FullName = StripAccentsNSpecialCharsNContinousSpaces(customer.FullName);
            customer.ContactAddress = StripAccentsNSpecialCharsNContinousSpaces(customer.ContactAddress);
            customer.HomeAddress = StripAccentsNSpecialCharsNContinousSpaces(customer.HomeAddress);
            customer.Professional = StripAccentsNSpecialCharsNContinousSpaces(customer.Professional);
            customer.Position = StripAccentsNSpecialCharsNContinousSpaces(customer.Position);
            customer.Pob = StripAccentsNSpecialCharsNContinousSpaces(customer.Pob);
            customer.Issuer = StripAccentsNSpecialCharsNContinousSpaces(customer.Issuer);
            customer.CompanyName = StripAccentsNSpecialCharsNContinousSpaces(customer.CompanyName);
            customer.CompanyAddress = StripAccentsNSpecialCharsNContinousSpaces(customer.CompanyAddress);
            return customer;
        }
        public static string ReplaceTwoContinousSpace(string text)
        {
            if (text == null) return null;
            return Regex.Replace(text, @"[ \t]{2,}", " "); //matches >= 2 continous space
        }
        public static string StripAccentsNSpecialCharsNContinousSpaces(string text)
        {
            if (text == null) return null;
            return ReplaceTwoContinousSpace(ReplaceSpecialCharsWithSpace(RemoveVietnameseAccents(text))).Trim();
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

       
    }
}
