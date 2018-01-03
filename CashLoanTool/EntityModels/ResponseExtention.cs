using CashLoanTool.Helper;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CashLoanTool.EntityModels
{
    public partial class Response
    {
        [NotMapped]
        [JsonIgnore]
        public string SignatureComposition
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append(ResponseCode ?? string.Empty).Append(AcctNo ?? string.Empty);
                builder.Append(AcctName ?? string.Empty).Append(RSAHelper.Salt);
                return builder.ToString();
            }
        }

        [NotMapped]
        [JsonIgnore]
        public string VerificationHash
        {
            get
            {
                return RSAHelper.Hash(SignatureComposition);
            }
        }
    }
}
