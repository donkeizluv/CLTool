using System.ComponentModel.DataAnnotations.Schema;

namespace CashLoanTool.EntityModels
{
    public partial class CustomerInfo
    {
        [NotMapped]
        public string Status { get; set; }
    }
}
