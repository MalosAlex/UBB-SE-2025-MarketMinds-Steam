using System;
using NUnit.Framework;
using BusinessLayer.Validators; // Assuming this namespace contains PaymentValidator

namespace Tests.Validators
{
    [TestFixture]
    public class PaymentValidatorTests
    {
        // Changed MAX_MONEY_AMOUNT to MAXIMUM_MONEY_AMOUNT for clarity
        private const int MAXIMUM_MONEY_AMOUNT = 500;

        #region IsMonetaryAmountValid Tests

        [Test]
        public void IsMonetaryAmountValid_ValidAmount_ReturnsTrue()
        {
            // Arrange
            // Changed 'amount' to 'amountString' for clarity (input is string)
            string amountString = "250";

            // Act
            // Updated second argument to use the renamed constant
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMonetaryAmountValid_MaximumAmount_ReturnsTrue()
        {
            // Arrange
            // Changed 'amount' to 'amountString'
            // Updated constant name
            string amountString = MAXIMUM_MONEY_AMOUNT.ToString();

            // Act
            // Updated constant name
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMonetaryAmountValid_ExceedsMaximum_ReturnsFalse()
        {
            // Arrange
            // Changed 'amount' to 'amountString'
            // Updated constant name
            string amountString = (MAXIMUM_MONEY_AMOUNT + 1).ToString();

            // Act
            // Updated constant name
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_ZeroAmount_ReturnsFalse()
        {
            // Arrange
            // Changed 'amount' to 'amountString'
            string amountString = "0";

            // Act
            // Updated constant name
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_NegativeAmount_ReturnsFalse()
        {
            // Arrange
            // Changed 'amount' to 'amountString'
            string amountString = "-10";

            // Act
            // Updated constant name
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_NonNumericString_ReturnsFalse()
        {
            // Arrange
            // Changed 'amount' to 'amountString'
            string amountString = "abc";

            // Act
            // Updated constant name
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_EmptyString_ReturnsFalse()
        {
            // Arrange
            // Changed 'amount' to 'amountString'
            string amountString = string.Empty;

            // Act
            // Updated constant name
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_NullString_ReturnsFalse()
        {
            // Arrange
            // Changed 'amount' to 'amountString'
            string amountString = null;

            // Act
            // Updated constant name
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_ValidDigitsButCannotParse_ReturnsFalse()
        {
            // Arrange
            // Changed 'amount' to 'amountString'
            // A number that's too large to be parsed as an int but contains only digits
            string amountString = "9999999999999999999"; // Exceeds int.MaxValue

            // Act
            // Updated constant name
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsCardNameValid Tests

        [Test]
        public void IsCardNameValid_ValidName_ReturnsTrue()
        {
            // Arrange
            // Changed 'name' to 'cardholderName' for specificity
            string cardholderName = "John Smith";

            // Act
            bool result = PaymentValidator.IsCardNameValid(cardholderName);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardNameValid_EmptyName_ReturnsFalse()
        {
            // Arrange
            // Changed 'name' to 'cardholderName'
            string cardholderName = string.Empty;

            // Act
            bool result = PaymentValidator.IsCardNameValid(cardholderName);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNameValid_NullName_ReturnsFalse()
        {
            // Arrange
            // Changed 'name' to 'cardholderName'
            string cardholderName = null;

            // Act
            bool result = PaymentValidator.IsCardNameValid(cardholderName);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNameValid_SingleName_ReturnsFalse()
        {
            // Arrange
            // Changed 'name' to 'cardholderName'
            string cardholderName = "John";

            // Act
            bool result = PaymentValidator.IsCardNameValid(cardholderName);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsCardNumberValid Tests

        [Test]
        public void IsCardNumberValid_VisaNumber_ReturnsTrue()
        {
            // Arrange
            // 'cardNumber' is already quite clear
            string cardNumber = "4111111111111111";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(cardNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardNumberValid_MastercardNumber_ReturnsTrue()
        {
            // Arrange
            string cardNumber = "5555555555554444";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(cardNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardNumberValid_AmexNumber_ReturnsTrue()
        {
            // Arrange
            // Changed 'cardNumber' to 'americanExpressCardNumber' for this specific test case clarity
            string americanExpressCardNumber = "3782822463100051";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(americanExpressCardNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardNumberValid_TooLongNumber_ReturnsFalse()
        {
            // Arrange
            // 'invalidNumber' changed to 'tooLongCardNumber' for consistency
            string tooLongCardNumber = "12345678901234562";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(tooLongCardNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_TooShortNumber_ReturnsFalse()
        {
            // Arrange
            // 'tooShortNumber' changed to 'tooShortCardNumber'
            string tooShortCardNumber = "1234";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(tooShortCardNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_EmptyNumber_ReturnsFalse()
        {
            // Arrange
            // 'emptyNumber' changed to 'emptyCardNumber'
            string emptyCardNumber = string.Empty;

            // Act
            bool result = PaymentValidator.IsCardNumberValid(emptyCardNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_NullNumber_ReturnsFalse()
        {
            // Arrange
            // 'nullNumber' changed to 'nullCardNumber'
            string nullCardNumber = null;

            // Act
            bool result = PaymentValidator.IsCardNumberValid(nullCardNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_NonNumericInput_ReturnsFalse()
        {
            // Arrange
            // 'nonNumericInput' changed to 'nonNumericCardNumber'
            string nonNumericCardNumber = "123abc456def";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(nonNumericCardNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        // Renamed region and test methods from Cvv to CardVerificationValue
        #region IsCardVerificationValueValid Tests

        [Test]
        public void IsCardVerificationValueValid_ThreeDigitCVV_ReturnsTrue()
        {
            // Arrange
            // Changed 'cvv' to 'cardVerificationValueString'
            string cardVerificationValueString = "123";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(cardVerificationValueString);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardVerificationValueValid_AllNinesCVV_ReturnsTrue()
        {
            // Arrange
            // Changed 'cvv' to 'cardVerificationValueString'
            string cardVerificationValueString = "999";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(cardVerificationValueString);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardVerificationValueValid_AllZerosCVV_ReturnsTrue()
        {
            // Arrange
            // Changed 'cvv' to 'cardVerificationValueString'
            string cardVerificationValueString = "000";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(cardVerificationValueString);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardVerificationValueValid_TooShortCVV_ReturnsFalse()
        {
            // Arrange
            // Renamed 'tooShortCVV'
            string tooShortCardVerificationValue = "12";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(tooShortCardVerificationValue);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardVerificationValueValid_TooLongCVV_ReturnsFalse()
        {
            // Arrange
            // Renamed 'tooLongCVV'
            string tooLongCardVerificationValue = "12345";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(tooLongCardVerificationValue);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardVerificationValueValid_EmptyCVV_ReturnsFalse()
        {
            // Arrange
            // Renamed 'emptyCVV'
            string emptyCardVerificationValue = string.Empty;

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(emptyCardVerificationValue);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardVerificationValueValid_NullCVV_ReturnsFalse()
        {
            // Arrange
            // Renamed 'nullCVV'
            string nullCardVerificationValue = null;

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(nullCardVerificationValue);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardVerificationValueValid_NonNumericCVV_ReturnsFalse()
        {
            // Arrange
            // Renamed 'nonNumericCVV'
            string nonNumericCardVerificationValue = "12A";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(nonNumericCardVerificationValue);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsExpirationDateValid Tests

        [Test]
        public void IsExpirationDateValid_FutureDate_ReturnsTrue()
        {
            // Arrange
            // Changed 'validDate' to 'futureExpirationDateString'
            string futureExpirationDateString = "12/28"; // Assuming test runs before Dec 2028

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(futureExpirationDateString);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsExpirationDateValid_CurrentMonthAndYear_ReturnsTrue()
        {
            // Arrange
            int currentMonth = DateTime.Now.Month;
            int currentYearTwoDigits = DateTime.Now.Year % 100;
            // Changed 'currentDate' to 'currentExpirationDateString'
            string currentExpirationDateString = $"{currentMonth:D2}/{currentYearTwoDigits:D2}";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(currentExpirationDateString);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsExpirationDateValid_PastDate_ReturnsFalse()
        {
            // Arrange
            int lastYearTwoDigits = (DateTime.Now.Year - 1) % 100;
            // Changed 'pastDate' to 'pastExpirationDateString'
            string pastExpirationDateString = $"12/{lastYearTwoDigits:D2}";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(pastExpirationDateString);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_InvalidMonth_ReturnsFalse()
        {
            // Arrange
            // Changed 'invalidMonth' to 'expirationDateWithInvalidMonth'
            string expirationDateWithInvalidMonth = "13/30";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(expirationDateWithInvalidMonth);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_SingleDigitMonthFormat_ReturnsFalse()
        {
            // Arrange
            // Changed 'invalidFormat' to 'expirationDateWithSingleDigitMonth'
            string expirationDateWithSingleDigitMonth = "1/23";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(expirationDateWithSingleDigitMonth);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_NoSlashFormat_ReturnsFalse()
        {
            // Arrange
            // Changed 'invalidFormat' to 'expirationDateWithoutSlash'
            string expirationDateWithoutSlash = "1234";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(expirationDateWithoutSlash);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_DashSeparatorFormat_ReturnsFalse()
        {
            // Arrange
            // Changed 'invalidFormat' to 'expirationDateWithDashSeparator'
            string expirationDateWithDashSeparator = "12-23";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(expirationDateWithDashSeparator);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_DotSeparatorFormat_ReturnsFalse()
        {
            // Arrange
            // Changed 'invalidFormat' to 'expirationDateWithDotSeparator'
            string expirationDateWithDotSeparator = "12.23";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(expirationDateWithDotSeparator);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_EmptyDate_ReturnsFalse()
        {
            // Arrange
            // Changed 'emptyDate' to 'emptyExpirationDateString'
            string emptyExpirationDateString = string.Empty;

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(emptyExpirationDateString);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_NullDate_ReturnsFalse()
        {
            // Arrange
            // Changed 'nullDate' to 'nullExpirationDateString'
            string nullExpirationDateString = null;

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(nullExpirationDateString);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsEmailValid Tests

        [Test]
        public void IsEmailValid_SimpleValidEmail_ReturnsTrue()
        {
            // Arrange
            // 'email' is generally clear in this context
            string email = "user@example.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmailValid_ComplexValidEmail_ReturnsTrue()
        {
            // Arrange
            string email = "name.surname@domain.co.uk";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmailValid_ValidEmailWithNumbers_ReturnsTrue()
        {
            // Arrange
            string email = "user123@gmail.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmailValid_ValidEmailWithPlusTag_ReturnsTrue()
        {
            // Arrange
            string email = "user+tag@domain.org";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmailValid_MissingAtSymbol_ReturnsFalse()
        {
            // Arrange
            string email = "plainaddress";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_MissingUsername_ReturnsFalse()
        {
            // Arrange
            string email = "@missingusername.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_MissingDomainName_ReturnsFalse()
        {
            // Arrange
            string email = "user@.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_DoubleDotInDomain_ReturnsFalse()
        {
            // Arrange
            string email = "user@domain..com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_DoubleAtSymbol_ReturnsFalse()
        {
            // Arrange
            string email = "user@domain@domain.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_DotAtEnd_ReturnsFalse()
        {
            // Arrange
            string email = "user@domain.com.";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_DotAtStart_ReturnsFalse()
        {
            // Arrange
            string email = ".user@domain.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_EmptyEmail_ReturnsFalse()
        {
            // Arrange
            // 'emptyEmail' is clear
            string emptyEmail = string.Empty;

            // Act
            bool result = PaymentValidator.IsEmailValid(emptyEmail);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_NullEmail_ReturnsFalse()
        {
            // Arrange
            // 'nullEmail' is clear
            string nullEmail = null;

            // Act
            bool result = PaymentValidator.IsEmailValid(nullEmail);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsPasswordValid Tests

        [Test]
        public void IsPasswordValid_ValidPassword_ReturnsTrue()
        {
            // Arrange
            // 'validPassword' is clear
            string validPassword = "Pass123!";

            // Act
            bool result = PaymentValidator.IsPasswordValid(validPassword);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsPasswordValid_EmptyPassword_ReturnsFalse()
        {
            // Arrange
            // 'emptyPassword' is clear
            string emptyPassword = string.Empty;

            // Act
            bool result = PaymentValidator.IsPasswordValid(emptyPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NullPassword_ReturnsFalse()
        {
            // Arrange
            // 'nullPassword' is clear
            string nullPassword = null;

            // Act
            bool result = PaymentValidator.IsPasswordValid(nullPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoUppercase_ReturnsFalse()
        {
            // Arrange
            // 'password' is generally clear in this context
            string password = "pass123!";

            // Act
            bool result = PaymentValidator.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoLowercase_ReturnsFalse()
        {
            // Arrange
            string password = "PASS123!";

            // Act
            bool result = PaymentValidator.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoDigit_ReturnsFalse()
        {
            // Arrange
            string password = "Password!";

            // Act
            bool result = PaymentValidator.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoSpecialCharacter_ReturnsFalse()
        {
            // Arrange
            string password = "Password123";

            // Act
            bool result = PaymentValidator.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_TooShort_ReturnsFalse()
        {
            // Arrange
            string password = "Pa1!";

            // Act
            bool result = PaymentValidator.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion
    }
}