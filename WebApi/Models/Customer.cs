using System;

namespace WebApi.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Updated { get; set; } = DateTime.Now;
    }
}
