using System;
using System.Text.RegularExpressions;

namespace BusinessLayer.Validators
{
    public static class PaymentValidator
    {
        // === General Constants ===
        private const int CardNumberLength = 16;
        private const int CvvLength = 3;
        private const int MinimumPasswordLength = 8;
        private const int MinimumCardNameParts = 2;
        private const int MaxMonetaryAmount = 500;
        private const int MinMonetaryAmount = 0;
        private const int MonthMin = 1;
        private const int MonthMax = 12;
        private const int YearDigits = 2;
        private const int ExpirationDatePartsCount = 2;

        private const char NameSplitChar = ' ';
        private const char ExpirationDateSeparator = '/';

        // === Derived Constants ===
        private static readonly int MaxYearModulo = (int)Math.Pow(10, YearDigits);
        private const int ExpirationMonthIndex = 0;
        private const int ExpirationYearIndex = 1;

        // === Regex Patterns ===
        private static readonly string EmailRegexPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
        private static readonly string EmailInvalidSequencePattern = @"(^\.)|(\.\.)|(\.$)";
        private static readonly string PasswordRegexPattern = $@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{{{MinimumPasswordLength},}}$";
        private static readonly string CardNumberRegexPattern = $@"^\d{{{CardNumberLength}}}$";
        private static readonly string CvvRegexPattern = $@"^\d{{{CvvLength}}}$";
        private static readonly string ExpirationDateRegexPattern = $@"^(0[1-9]|1[0-2])\/\d{{{YearDigits}}}$";
        private static readonly string NumericRegexPattern = @"^\d+$";

        // === Validation Methods ===
        public static bool IsEmailValid(string emailAddress)
        {
            return !string.IsNullOrEmpty(emailAddress) &&
                   Regex.IsMatch(emailAddress, EmailRegexPattern) &&
                   !Regex.IsMatch(emailAddress, EmailInvalidSequencePattern);
        }

        public static bool IsPasswordValid(string password)
        {
            return !string.IsNullOrEmpty(password) &&
                   Regex.IsMatch(password, PasswordRegexPattern);
        }

        public static bool IsCardNameValid(string name)
        {
            return !string.IsNullOrEmpty(name) &&
                   name.Split(NameSplitChar).Length >= MinimumCardNameParts;
        }

        public static bool IsCardNumberValid(string cardNumber)
        {
            return !string.IsNullOrEmpty(cardNumber) &&
                   Regex.IsMatch(cardNumber, CardNumberRegexPattern);
        }

        public static bool IsCvvValid(string cvv)
        {
            return !string.IsNullOrEmpty(cvv) &&
                   Regex.IsMatch(cvv, CvvRegexPattern);
        }

        public static bool IsExpirationDateValid(string expirationDate)
        {
            if (string.IsNullOrEmpty(expirationDate) ||
                !Regex.IsMatch(expirationDate, ExpirationDateRegexPattern))
            {
                return false;
            }

            string[] dateParts = expirationDate.Split(ExpirationDateSeparator);
            if (dateParts.Length != ExpirationDatePartsCount ||
                !int.TryParse(dateParts[ExpirationMonthIndex], out int month) ||
                !int.TryParse(dateParts[ExpirationYearIndex], out int year))
            {
                return false;
            }

            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year % MaxYearModulo;

            return (year > currentYear) || (year == currentYear && month >= currentMonth);
        }

        public static bool IsMonetaryAmountValid(string input, int maxAmount = MaxMonetaryAmount)
        {
            if (string.IsNullOrEmpty(input) || !Regex.IsMatch(input, NumericRegexPattern))
            {
                return false;
            }

            return int.TryParse(input, out int amount) &&
                   amount > MinMonetaryAmount &&
                   amount <= maxAmount;
        }
    }
}
