using Aspose.Words;
using CashLoanTool.EntityModels;
using GemBox.Document;
using System;
using System.IO;

namespace CashLoanTool.DocumentUltility
{
    public static class ArgreementMaker
    {
        public const string DateFormat = "dd/MM/yyyy";
        //Use Aspose to save Pdf to workaround gembox's word wrap bug
        public static void AsposePdfStream(DocumentModel model, Stream outputStream)
        {
            var stream = new MemoryStream();
            model.Save(stream, SaveOptions.DocxDefault);
            stream.Position = 0;
            var asposeDoc = new Document(stream, new Aspose.Words.LoadOptions() { LoadFormat = LoadFormat.Docx});
            asposeDoc.Save(outputStream, SaveFormat.Pdf);
            outputStream.Position = 0;
        }

        //Its a pain in the ass to replace with custom style using Aspose
        //public static Document FillTemplateAspose(CustomerInfo customer, string acctNo, string issuer, string pob, string templatePath)
        //{
        //    var document = new Document();
        //    // Find and replace text.
        //    document.Range.Replace("{name}", customer.FullName, false, false);
        //    //document.Content.Replace("{pob}", customer.Pob); //Indus cant supply this
        //    document.Range.Replace("{pob}", pob, false, false);
        //    document.Range.Replace("{id}", customer.IdentityCard, false, false);
        //    document.Range.Replace("{id_date}", customer.IssueDate.ToString(DateFormat), false, false);
        //    //document.Content.Replace("{id_issuer}", customer.Issuer); //Indus cant supply this
        //    document.Range.Replace("{id_issuer}", issuer, false, false);
        //    document.Range.Replace("{dob}", customer.Dob.ToString(DateFormat), false, false);
        //    document.Range.Replace("{addr}", (customer.HomeAddress ?? string.Empty), false, false);
        //    //IF no CT info then use RS instead
        //    string ctInfo = customer.ContactAddress ?? string.Empty;
        //    if (string.IsNullOrEmpty(customer.ContactAddress))
        //        ctInfo = customer.HomeAddress;
        //    document.Range.Replace("{ct_addr}", ctInfo, false, false);
        //    document.Range.Replace("{occu}", customer.Professional ?? string.Empty, false, false);
        //    document.Range.Replace("{pos}", customer.Position ?? string.Empty, false, false);
        //    document.Range.Replace("{nat}", customer.Nationality, false, false);
        //    document.Range.Replace("{phone}", customer.Phone, false, false);
        //    document.Range.Replace("{acct_no}", acctNo, false, false);
        //    if (GenderStringToBool(customer.Gender))
        //    {
        //        document.Range.Replace("{ma}", customer.Phone, false, false);
        //        document.Range.Replace("{fe}", customer.Phone, false, false);
        //        //ReplaceWithCustomStyle(document, "@fe", "o", "Wingdings");
        //        //ReplaceWithCustomStyle(document, "@ma", "S", "Wingdings 2");
        //    }
        //    else
        //    {
        //        ReplaceWithCustomStyle(document, "@fe", "T", "Wingdings 2");
        //        ReplaceWithCustomStyle(document, "@ma", "o", "Wingdings");
        //    }
        //    return document;
        //}

    //doesnt work
    //TextRun is shit

    private static void ReplaceWithCustomStyle(Document doc, string text, string replace, string fontName)
        {
            foreach (Aspose.Words.Run runNode in doc.GetChildNodes(NodeType.Run, true))
            {
                if (!runNode.Text.Contains(text)) continue;
                var builder = new DocumentBuilder(doc);
                builder.MoveTo(runNode);
                builder.Font.Name = fontName;
                builder.Write(replace);
                runNode.Remove();
            }
        }
        public static DocumentModel FillTemplate(CustomerInfo customer, string acctNo, string templatePath)
        {
            var document = DocumentModel.Load(templatePath, DocxLoadOptions.DocxDefault);
            document.ViewOptions.ViewType = ViewType.FullScreen;
            // Find and replace text.
            document.Content.Replace("{name}", customer.FullName);
            //document.Content.Replace("{pob}", customer.Pob); //Indus cant supply this
            document.Content.Replace("{pob}", customer.Pob);
            document.Content.Replace("{id}", customer.IdentityCard);
            document.Content.Replace("{id_date}", customer.IssueDate.ToString(DateFormat));
            //document.Content.Replace("{id_issuer}", customer.Issuer); //Indus cant supply this
            document.Content.Replace("{id_issuer}", customer.Issuer);
            document.Content.Replace("{dob}", customer.Dob.ToString(DateFormat));
            document.Content.Replace("{addr}", (customer.HomeAddress??string.Empty));
            //IF no CT info then use RS instead
            string ctInfo = customer.ContactAddress ?? string.Empty;
            if (string.IsNullOrEmpty(customer.ContactAddress))
                ctInfo = customer.HomeAddress;
            document.Content.Replace("{ct_addr}", ctInfo);
            document.Content.Replace("{occu}", (customer.Professional??string.Empty));
            document.Content.Replace("{pos}", (customer.Position??string.Empty));
            document.Content.Replace("{nat}", customer.Nationality);
            document.Content.Replace("{phone}", customer.Phone);
            document.Content.Replace("{acct_no}", acctNo);
            
            if (GenderStringToBool(customer.Gender))
            {
                foreach (ContentRange item in document.Content.Find("{ma}"))
                    item.LoadText("S", new CharacterFormat() { FontName = "Wingdings 2" });
                foreach (ContentRange item in document.Content.Find("{fe}"))
                    item.LoadText("o", new CharacterFormat() { FontName = "Wingdings" });
            }
            else
            {
                foreach (ContentRange item in document.Content.Find("{fe}"))
                    item.LoadText("T", new CharacterFormat() { FontName = "Wingdings 2" });
                foreach (ContentRange item in document.Content.Find("{ma}"))
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
