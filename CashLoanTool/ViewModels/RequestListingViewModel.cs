using System.Collections.Generic;
using CashLoanTool.EntityModels;
using CashLoanTool.Helper;

namespace CashLoanTool.ViewModels
{
    public class RequestListingViewModel
    {
        //public string Division { get; set; }

        public List<Request> Requests { get; set; }
        public static int ItemPerPage { get; set; } = 10;
        //update these every time add record
        public int TotalPages { get; private set; }
        public int TotalRows { get; private set; }
        public string OrderBy { get; set; }
        public bool OrderAsc { get; set; }
        public int OnPage { get; set; }

        public bool IsSchedulerDown
        {
            get
            {
                return EnviromentHelper.IsSchedulerDown;
            }
        }

        public RequestListingViewModel()
        {
            Requests = new List<Request>();
        }
        public void UpdatePagination(int totalRows)
        {
            TotalRows = totalRows;
            TotalPages = (TotalRows + ItemPerPage - 1) / ItemPerPage;
            if (TotalPages < 1)
                TotalPages = 1;
        }
    }
}
