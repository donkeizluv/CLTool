using System.Collections.Generic;
using CashLoanTool.EntityModels;

namespace CashLoanTool.ViewModels
{
    public class AdmViewModel
    {
        public List<User> Users { get; set; }
        public List<string> Divisions { get; set; }
        public static int ItemPerPage { get; set; } = 10;
        //update these every time add record
        public int TotalPages { get; private set; }
        public int TotalRows { get; private set; }
        public int OnPage { get; set; }

        public AdmViewModel()
        {
            Users = new List<User>();
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
