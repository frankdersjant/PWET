using AutoMapper;
using PWET.Domain;
using PWET.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWET.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Expense, ExpenseDTO>();
            CreateMap<ExpenseGroup, ExpenseGroupDTO>();
            CreateMap<ExpenseGroupStatus, ExpenseGroupStatusDTO>();
        }
    }
}