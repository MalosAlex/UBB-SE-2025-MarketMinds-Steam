using System;

namespace BusinessLayer.Exceptions
{
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException(string email) 
            : base($"An account with the email '{email}' already exists.") { }
    }

    public class UsernameAlreadyTakenException : Exception
    {
        public UsernameAlreadyTakenException(string username) 
            : base($"The username '{username}' is already taken.") { }
    }

    public class UserValidationException : Exception
    {
        public UserValidationException(string message) : base(message) { }
    }
} 