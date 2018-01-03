using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CashLoanTool.EntityModels
{
    public partial class Request
    {
        [NotMapped]
        public string IdentityCardName
        {
            get
            {
                return CustomerInfo.Single().FullName;
            }
        }
        [NotMapped]
        public string RequestCreateTimeString
        {
            get
            {
                return RequestCreateTime.ToString("dd/MM/yyyy");
            }
        }
        //This kind of check is unreliable, Response, info.count varies when call Include...
        //Fix this
        //This only works when 
        [NotMapped]
        public bool HasValidAcctNo
        {
            //00: success
            //01: có tài khoản cũ rồi nhưng tên sai so với tên đã lưu tại hdb What this???
            //09: trả về tài khoản cũ
            //03: tạo tài khoản thất bại, tham số input truyền qua không hợp lệ
            get
            {
                if (Response.Count < 1) return false;
                
                var responseOK = Response.Where(r => string.Compare(r.ResponseCode, "00") == 0 || string.Compare(r.ResponseCode, "09") == 0);
                if (responseOK.Count() > 0) return true;
                return false;
            }
        }
        [NotMapped]
        public string AcctNo
        {
            get
            {
                var responseOK = Response.Where(r => string.Compare(r.ResponseCode, "00") == 0 || string.Compare(r.ResponseCode, "09") == 0);
                if (responseOK.SingleOrDefault() == null) return string.Empty;
                return responseOK.Single().AcctNo;
            }
        }
        //Expose these for easier usage
        [NotMapped]
        public string IdentityCard => CustomerInfo.Single().IdentityCard;
        [NotMapped]
        public string Phone => CustomerInfo.Single().Phone;
        //Buggy
        //public bool HasResponse
        //{
        //    get
        //    {
        //        return Response.Count > 0;
        //    }
        //}
    }
}
