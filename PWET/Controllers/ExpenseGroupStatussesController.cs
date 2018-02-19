using AutoMapper;
using PWET.Domain;
using PWET.DTO;
using PWET.Mappings;
using PWET.Repo;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PWET.Controllers
{
    public class ExpenseGroupStatussesController : ApiController
    {
        private readonly IExpenseTrackerRepository _repo;

        public ExpenseGroupStatussesController(IExpenseTrackerRepository repo)
        {
            _repo = repo;
        }

        public IHttpActionResult Get()
        {
            try
            {
                var result = _repo.GetExpenseGroupStatusses();

                var config = new AutoMapperConfiguration().Configure();
                IMapper mapper = config.CreateMapper();
                var dest = mapper.Map<IEnumerable<ExpenseGroupStatus>, IEnumerable<ExpenseGroupStatusDTO>> (result);

                return Ok(dest);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}