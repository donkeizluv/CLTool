using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashLoanTool.EntityModels
{
    public partial class CustomerInfo
    {
        [NotMapped]
        [JsonIgnore]
        //only for checking valid purposes,
        public string Status { get; set; }
    }
}
