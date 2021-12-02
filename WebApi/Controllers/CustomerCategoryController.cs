using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Filters;
using WebApi.Models;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("[Controller]s")]
    [TokenAuthenticationFilter]
    public class CustomerCategoryController : ControllerBase
    {

        private static List<CustomerCategory> CustomerCategoryList = new List<CustomerCategory>()
        {
            new CustomerCategory
            {
                Id=1,
                Title="Free",
            },

            new CustomerCategory
            {
                Id=2,
                Title="Paid",
            },

            new CustomerCategory
            {
                Id=3,
                Title="Enterprise",
            },
        };


        private readonly ILogger<CustomerCategoryController> _logger;

        public CustomerCategoryController(ILogger<CustomerCategoryController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<CustomerCategory> GetCustomerCategories()
        {
            var customerCategoryList = CustomerCategoryList.OrderBy(x=> x.Id).ToList<CustomerCategory>();
            return customerCategoryList;
        }

        [HttpGet("{id}")]
        public CustomerCategory GetById(int id)
        {
            var customerCategory = CustomerCategoryList.Where(x => x.Id == id).SingleOrDefault();
            return customerCategory;
        }

        [HttpPost]
        public IActionResult AddCustomerCategory([FromBody] CustomerCategory newCustomerCategory)
        {
            var customerCategory = CustomerCategoryList.SingleOrDefault(x => x.Title == newCustomerCategory.Title);

            if (customerCategory is not null)
                return BadRequest();

            newCustomerCategory.Id = CustomerCategoryList.Max(x => x.Id) + 1;
            CustomerCategoryList.Add(newCustomerCategory);

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCustomerCategory(int id, [FromBody] CustomerCategory updatedCustomerCategory)
        {
            var customerCategory = CustomerCategoryList.SingleOrDefault(x => x.Id == id);

            if (customerCategory is null)
                return BadRequest();

            customerCategory.Title = updatedCustomerCategory.Title != default ? updatedCustomerCategory.Title : updatedCustomerCategory.Title;

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomerCategory(int id)
        {
            var customerCategory = CustomerCategoryList.SingleOrDefault(x => x.Id == id);

            if (customerCategory is null)
                return BadRequest();

            CustomerCategoryList.Remove(customerCategory);

            return Ok();
        }
    }
}
