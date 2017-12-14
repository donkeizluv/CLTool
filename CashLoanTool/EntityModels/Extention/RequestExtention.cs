using System.Linq;

namespace CashLoanTool.EntityModels
{
    public partial class Request
    {
        public string IdentityCardName
        {
            get
            {
                return CustomerInfo.First().FullName;
            }
        }
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
        public string AcctNo
        {
            get
            {
                var responseOK = Response.Where(r => string.Compare(r.ResponseCode, "00") == 0 || string.Compare(r.ResponseCode, "09") == 0);
                if (responseOK.FirstOrDefault() == null) return string.Empty;
                return responseOK.First().AcctNo;
            }
        }
        //Expose these for easier usage
        public string IdentityCard => CustomerInfo.First().IdentityCard;
        public string Phone => CustomerInfo.First().Phone;
        //Buggy
        //public bool HasResponse
        //{
        //    get
        //    {
        //        return Response.Count > 0;
        //    }
        //}
        public bool HasCustomerInfo => CustomerInfo.Count == 1;
    }
}
