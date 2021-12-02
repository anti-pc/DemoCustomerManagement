using System;
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
    public class CustomerController : ControllerBase
    {

        private static List<Customer> CustomerList = new List<Customer>()
        {
            new Customer
            {
                Id=1,
                FirstName="Allison",
                LastName="Dark",
                CategoryId=100,
                Updated=new DateTime(2021,11,30)
            },

            new Customer{
                Id=2,
                FirstName="David",
                LastName="Dark",
                CategoryId=100,
                Updated=new DateTime(2021,12,1)
            },

            new Customer{
                Id=3,
                FirstName="Peter",
                LastName="Crown",
                CategoryId=100,
                Updated=new DateTime(2021,11,30)
            }
        };


        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<Customer> GetCustomers()
        {
            var customerList = CustomerList.OrderBy(x => x.Id).ToList<Customer>();
            return customerList;
        }

        [HttpGet("{id}")]
        public Customer GetById(int id)
        {
            var customer = CustomerList.SingleOrDefault(x => x.Id == id);
            return customer;
        }

        [HttpPost]
        public IActionResult AddCustomer([FromBody] Customer newCustomer)
        {
            var customer = CustomerList.SingleOrDefault(x => x.FirstName == newCustomer.FirstName && x.LastName == newCustomer.LastName);

            if (customer is not null)
                return BadRequest();

            newCustomer.Id = CustomerList.Max(x => x.Id) + 1;
            CustomerList.Add(newCustomer);

            return Ok(); 
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            var customer = CustomerList.SingleOrDefault(x => x.Id == id);

            if(customer is null)
                return BadRequest();

            customer.FirstName = updatedCustomer.FirstName ?? customer.FirstName;
            customer.LastName = updatedCustomer.LastName ?? customer.LastName;
            customer.CategoryId = updatedCustomer.CategoryId != default ? updatedCustomer.CategoryId : customer.CategoryId;

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = CustomerList.SingleOrDefault(x => x.Id == id);

            if (customer is null)
                return BadRequest();

            CustomerList.Remove(customer);
            return Ok();
        }
    }
}
