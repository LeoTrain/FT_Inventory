using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Inventory.MVVM.Models
{
    public class Customer
    {
        private string _firstName;
        private string _lastName;
        private string _email;
        public int Id { get; set; }
        /// <summary>
        /// The first name of the customer cannot be null or empty.
        /// </summary>
        public string FirstName { get => _firstName; set { if (!string.IsNullOrEmpty(value)) _firstName = value; }}
        /// <summary>
        /// The last name of the customer cannot be null or empty.
        /// </summary>
        public string LastName { get => _lastName; set { if (!string.IsNullOrEmpty(value)) _lastName = value; }}
        public string FullName { get; set; }
        /// <summary>
        /// The email of the customer cannot be null or empty.
        /// </summary>
        public string Email { get => _email; set { if (!string.IsNullOrEmpty(value)) _email = value; }}
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ImageUrl { get; set; }
        public Customer()
        {
            Id = 0;
            _firstName = string.Empty;
            _lastName = string.Empty;
            _email = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            City = string.Empty;
            State = string.Empty;
            PostalCode = string.Empty;
            Country = string.Empty;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            ImageUrl = string.Empty;
            FullName = string.Empty;
        }
        public Customer(int customerId, string first_name, string last_name, string email, string phone, DateTime createdAt, DateTime updatedAt, string address = "", string city = "", string state = "", string postal_code = "", string country = "", string image_url = "")
        {
            Id = customerId;
            _firstName = first_name;
            _lastName = last_name;
            _email = email;
            Phone = phone;
            Address = address;
            City = city;
            State = state;
            PostalCode = postal_code;
            Country = country;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            ImageUrl = image_url;
            FullName = $"{FirstName} {LastName}";
        }
    }
}
