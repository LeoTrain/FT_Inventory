using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace FT_Inventory.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when there is an error connecting to the database.
    /// </summary>
    internal class DbConnectionException : Exception
    {
        private const string DefaultMessage = "Database connection error. Please check your connection string.";

        /// <summary>
        /// Constructor for the DbConnectionException class.
        /// </summary>
        public DbConnectionException() : base(DefaultMessage) { }
        /// <summary>
        /// Constructor for the DbConnectionException class.
        /// </summary>
        /// <param name="message"></param>
        public DbConnectionException(string message) : base(message) { }
        /// <summary>
        /// Constructor for the DbConnectionException class.
        /// </summary>
        /// <param name="exception"></param>
        public DbConnectionException(SqlException exception)
            : base(GenerateMessageForError(exception.Number)) { }

        private static string GenerateMessageForError(int number)
        {
            switch (number)
            {
                case 4060:
                    return "Invalid database.";
                case 233:
                    return "SQL Connection Error: Invalid credentials.";
                default:
                    return DefaultMessage;
            }
        }
    }
}
