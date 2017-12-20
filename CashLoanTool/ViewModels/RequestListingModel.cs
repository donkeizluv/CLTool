using System.Collections.Generic;
using CashLoanTool.EntityModels;
namespace CashLoanTool.ViewModels
{
    public class RequestListingModel
    {
        //public List<string> IssuersList
        //{
        //    get
        //    {
        //        return IssuerList.Issuers;
        //    }
        //}

        public List<Request> Requests { get; set; }
        public static int ItemPerPage { get; set; } = 10;
        //update these every time add record
        public int TotalPages { get; private set; }
        public int TotalRows { get; private set; }
        public string OrderBy { get; set; }
        public bool OrderAsc { get; set; }
        public int OnPage { get; set; }

        public RequestListingModel()
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
