using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Inventory.MVVM.Models
{
    public class Customer
    {
        private int _id { get; set; }
        private string _firstName { get; set; }
        private string _lastName { get; set; }
        private string _email { get; set; }
        private string _phone { get; set; }
        private string _address { get; set; }
        private string _city { get; set; }
        private string _state { get; set; }
        private string _postalCode { get; set; }
        private string _country { get; set; }
        private DateTime _createdAt { get; set; }
        private DateTime _updatedAt { get; set; }
        private string _imageUrl { get; set; }

        public int Id
        {
            get => _id;
            set => _id = value;
        }
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("First name cannot be empty");
                };
                _firstName = value;
            }
        }
        public string LastName
        {
            get => _lastName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Last name cannot be empty");
                };
                _lastName = value;
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Email cannot be empty");
                };
                _email = value;
            }
        }
        public string Phone
        {
            get => _phone;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Phone cannot be empty");
                };
                _phone = value;
            }
        }
        public string Address
        {
            get => _address;
            set => _address = value;
        }
        public string City
        {
            get => _city;
            set => _city = value;
        }
        public string State
        {
            get => _state;
            set => _state = value;
        }
        public string PostalCode
        {
            get => _postalCode;
            set => _postalCode = value;
        }
        public string Country
        {
            get => _country;
            set => _country = value;
        }
        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Created at cannot be empty");
                };
                if (value > DateTime.Now)
                {
                    throw new ArgumentException("Created at cannot be in the future");
                };
                _createdAt = value;
            }
        }
        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Updated at cannot be empty");
                };
                if (value > DateTime.Now)
                {
                    throw new ArgumentException("Updated at cannot be in the future");
                };
                _updatedAt = value;
            }
        }
        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                if (value.Length > 255)
                    throw new ArgumentException("Image URL cannot be longer than 255 characters");
                if (string.IsNullOrEmpty(value))
                    _imageUrl = "";
                else
                    _imageUrl = value;
            }
        }

        public Customer()
        {
            _id = 0;
            _firstName = "NewFirstName";
            _lastName = "NewLastName";
            _email = "NewEmail";
            _phone = "NewPhone";
            _address = "NewAddress";
            _city = "NewCity";
            _state = "NewState";
            _postalCode = "NewPostalCode";
            _country = "NewCountry";
            _createdAt = DateTime.Now;
            _updatedAt = DateTime.Now;
            _imageUrl = "new/image/url";
        }
        public Customer(int customerId, string first_name, string last_name, string email, string phone, DateTime createdAt, DateTime updatedAt, string address = "", string city = "", string state = "", string postal_code = "", string country = "", string image_url = "")
        {
            _id = customerId;
            _firstName = first_name;
            _lastName = last_name;
            _email = email;
            _phone = phone;
            _address = address;
            _city = city;
            _state = state;
            _postalCode = postal_code;
            _country = country;
            _createdAt = createdAt;
            _updatedAt = updatedAt;
            _imageUrl = image_url;
        }
    }
}
