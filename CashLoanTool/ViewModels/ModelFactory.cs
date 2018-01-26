using CashLoanTool.EntityModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashLoanTool.ViewModels
{
    public static class ModelFactory
    {
        public static async Task<AdmViewModel> CreateAdmViewModel(CLToolContext context, int pageNum)
        {
            int getPage = pageNum < 1 ? 1 : pageNum;
            int excludedRows = (getPage - 1) * RequestListingViewModel.ItemPerPage;
            var query = context.User.Include(u => u.UserAbility);
            var model = new AdmViewModel
            {
                Users = await query.OrderBy(u => u.Username)
                             .Skip(excludedRows)
                             .Take(RequestListingViewModel.ItemPerPage).ToListAsync(),
                OnPage = pageNum,
                Divisions = await context.Division.Select(a => a.DivisionName).ToListAsync()
            };
            model.UpdatePagination(await context.User.CountAsync());
            return model;
        }
        public static async Task<RequestListingViewModel> CreateRequestListingModel(IQueryable<Request> query, int pageNum, string orderBy, bool asc)
        {
            int getPage = pageNum < 1 ? 1 : pageNum;
            int excludedRows = (getPage - 1) * RequestListingViewModel.ItemPerPage;
            //User can only see rq from same Division
            var totalRows = await query.CountAsync();
            var ordered = RequestOrderTranslater(query, orderBy, asc);
            var model = new RequestListingViewModel
            {
                Requests = await ordered
                                .Skip(excludedRows)
                                .Take(RequestListingViewModel.ItemPerPage)
                                .Include(r => r.CustomerInfo)
                                .Include(r => r.Response)
                                .ToListAsync(),
                OnPage = pageNum,
                OrderAsc = asc,
                OrderBy = orderBy
            };
            model.UpdatePagination(totalRows);
            return model;
        }
        private static IOrderedQueryable<Request> RequestOrderTranslater(IQueryable<Request> query, string orderBy, bool asc)
        {
            switch (orderBy)
            {
                case "RequestCreateTime":
                    if (!asc)
                        return query.OrderByDescending(r => r.RequestCreateTime);
                    return query.OrderBy(r => r.RequestCreateTime);
                case "RequestId":
                    if (!asc)
                        return query.OrderByDescending(r => r.RequestId);
                    return query.OrderBy(r => r.RequestId);
                //Others are not NYI
                default:
                    return query.OrderBy(r => r.RequestId);
            }
        }
    }
}
