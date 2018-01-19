using Aspose.Words;
using CashLoanTool.EntityModels;
using GemBox.Document;
using QRCoder;
using System;
using System.IO;

namespace CashLoanTool.DocumentUltility
{
    public static class ArgreementMaker
    {
        public const double QRSize = 30;
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
        public static void GemboxPdfStream(DocumentModel model, Stream outputStream)
        {
            model.Save(outputStream, SaveOptions.DocxDefault);
        }

        public static DocumentModel FillTemplate(CustomerInfo customer, string loanNo, string acctNo, string templatePath)
        {
            var document = DocumentModel.Load(templatePath, DocxLoadOptions.DocxDefault);

            //InsertQRCode(document, loanNo);
            TickGender(document, customer.Gender);

            // Find and replace info place holder
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
            return document;
        }
        private static void TickGender(DocumentModel model, string gender)
        {
            if (GenderStringToBool(gender))
            {
                foreach (ContentRange item in model.Content.Find("{ma}"))
                {
                    item.LoadText("S", new CharacterFormat() { FontName = "Wingdings 2" });
                }

                foreach (ContentRange item in model.Content.Find("{fe}"))
                    item.LoadText("o", new CharacterFormat() { FontName = "Wingdings" });
            }
            else
            {
                foreach (ContentRange item in model.Content.Find("{fe}"))
                    item.LoadText("T", new CharacterFormat() { FontName = "Wingdings 2" });
                foreach (ContentRange item in model.Content.Find("{ma}"))
                    item.LoadText("o", new CharacterFormat() { FontName = "Wingdings" });
            }
        }
        private static bool InsertQRCode(DocumentModel model, string qr)
        {
            //Insert QR
            foreach (var element in model.GetChildElements(true, ElementType.Picture))
            {
                if (element.ElementType != ElementType.Picture)
                    continue;
                var pic = (Picture)element;
                //Find picture place holder
                if (string.Compare(pic.Metadata.Description, "{qr}") != 0) continue;
                var qrStream = CreateQR(qr);
                var para = (GemBox.Document.Paragraph)pic.Parent;
                //Get place holder layout
                var layout = pic.Layout;
                //Delete place holder
                pic.Content.Delete();
                //Set QR
                para.Inlines.Add(new Picture(model, qrStream, PictureFormat.Png, layout));
                //Done
                return true;
            }
            return false;
        }

        private static MemoryStream CreateQR(string text)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.L);
            var qrCode = new QRCode(qrCodeData);
            var memoryStream = new MemoryStream();
            qrCode.GetGraphic(20).Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            return memoryStream;
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
