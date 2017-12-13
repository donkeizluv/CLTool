using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        //This kind of check is unreliable, Response, info.count varies when call Include...
        //Fix this
        public bool HasValidAcctNo
        {
            get
            {
                if (Response.Count < 1) return false;
                var responseOK = Response.Where(r => string.Compare(r.ResponseCode, "00") == 0);
                //00 response only sent once
                if (responseOK.Count() == 1) return true;
                return false;
            }
        }
        public string AcctNo
        {
            get
            {
                if (!HasValidAcctNo) return string.Empty;
                var responseOK = Response.Where(r => string.Compare(r.ResponseCode, "00") == 0);
                return responseOK.First().AcctNo;
            }
        }
        //Expose these for easier usage
        public string IdentityCard => CustomerInfo.First().IdentityCard;
        public string Phone => CustomerInfo.First().Phone;
        public bool HasResponse
        {
            get
            {
                return Response.Count > 0;
            }
        }
        public bool HasCustomerInfo => CustomerInfo.Count == 1;
    }
}
