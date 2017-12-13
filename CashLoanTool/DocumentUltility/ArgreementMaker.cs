using CashLoanTool.EntityModels;
using CashLoanTool.Helper;
using GemBox.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.DocumentUltility
{
    public static class ArgreementMaker
    {
        public const string DateFormat = "dd/MM/yyyy";
        public static DocumentModel FillTemplate(CustomerInfo customer, string acctNo, string templatePath)
        {
            var response = customer.Request.Response.First();

            var document = DocumentModel.Load(templatePath);
            // Find and replace text.
            document.Content.Replace("{name}", customer.FullName);
            document.Content.Replace("{pob}", customer.Pob);
            document.Content.Replace("{id}", customer.IdentityCard);
            document.Content.Replace("{id_date}", customer.IssueDate.ToString(DateFormat));
            document.Content.Replace("{id_issuer}", customer.Issuer);
            document.Content.Replace("{dob}", customer.Dob.ToString(DateFormat));
            document.Content.Replace("{addr}", customer.ContactAddress);
            document.Content.Replace("{occu}", customer.Professional);
            document.Content.Replace("{pos}", customer.Position);
            document.Content.Replace("{nat}", customer.Nationality);
            document.Content.Replace("{phone}", customer.Phone);
            document.Content.Replace("{acct_no}", acctNo);

            // incase of changing font...
            //foreach (ContentRange item in document.Content.Find("{full_name}"))
            //    item.LoadText("Luu Nhat Hong", new CharacterFormat());
            //foreach (ContentRange item in document.Content.Find("{birth_place}"))
            //    item.LoadText("Viet Nam", new CharacterFormat() { FontName = "Times New Roman" });

            
            if (GenderStringToBool(customer.Gender))
            {
                foreach (ContentRange item in document.Content.Find("{m}"))
                    item.LoadText("S", new CharacterFormat() { FontName = "Wingdings 2" });
                foreach (ContentRange item in document.Content.Find("{f}").Reverse())
                    item.LoadText("o", new CharacterFormat() { FontName = "Wingdings" });
            }
            else
            {
                foreach (ContentRange item in document.Content.Find("{f}"))
                    item.LoadText("T", new CharacterFormat() { FontName = "Wingdings 2" });
                foreach (ContentRange item in document.Content.Find("{m}"))
                    item.LoadText("o", new CharacterFormat() { FontName = "Wingdings" });
            }
            return document;
        }
        private static bool GenderStringToBool(string gender)
        {
            switch (gender.ToUpper())
            {
                case "M":
                    return true;
                case "F":
                    return false;
                default:
                    throw new ArgumentException("Unregconizable string: " + gender);
            }
        }
    }
}
