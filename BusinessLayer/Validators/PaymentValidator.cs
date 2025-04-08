using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLayer.Validators
{
    public static class PaymentValidator
    {
        public static bool IsEmailValid(string emailAddress)
        {
            return !string.IsNullOrEmpty(emailAddress) &&
                   Regex.IsMatch(emailAddress, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$") &&
                   !Regex.IsMatch(emailAddress, @"(^\.)|(\.\.)|(\.$)");
        }

        public static bool IsPasswordValid(string password)
        {
            return !string.IsNullOrEmpty(password) &&
                   Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        }
        public static bool IsCardNameValid(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            return name.Split(' ').Length > 1;
        }

        public static bool IsCardNumberValid(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
            {
                return false;
            }
            return Regex.IsMatch(cardNumber, @"^\d{16}$");
        }

        public static bool IsCardVerificationValueValid(string cardVerificationValue)
        {
            if (string.IsNullOrEmpty(cardVerificationValue))
            {
                return false;
            }
            return Regex.IsMatch(cardVerificationValue, @"^\d{3}$");
        }

        public static bool IsExpirationDateValid(string expirationDate)
        {
            if (string.IsNullOrEmpty(expirationDate))
            {
                return false;
            }
            bool isValidDateFormat = Regex.IsMatch(expirationDate, @"^(0[1-9]|1[0-2])\/\d{2}$");

            if (!isValidDateFormat)
            {
                return false;
            }

            string[] dateParts = expirationDate.Split('/');
            int month = int.Parse(dateParts[0]);
            int year = int.Parse(dateParts[1]);
            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year % 100;

            return (year > currentYear) || (year == currentYear && month >= currentMonth);
        }

        public static bool IsMonetaryAmountValid(string input, int maxAmount = 500)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            if (!Regex.IsMatch(input, @"^\d+$"))
            {
                return false;
            }

            if (int.TryParse(input, out int amount))
            {
                if (amount > maxAmount || amount <= 0)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}