using PagedList;
using PWET.Domain;
using PWET.Web.Helpers;
using System.Collections.Generic;

namespace PWET.Web.ViewModels
{
    public class ExpenseGroupsViewModel
    {
        public IPagedList<ExpenseGroup> ExpenseGroup { get; set; }
        public IEnumerable<ExpenseGroupStatus> ExpenseGroupStatus { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}