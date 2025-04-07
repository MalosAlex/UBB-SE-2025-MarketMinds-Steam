using System;
using NUnit.Framework;
using BusinessLayer.Validators;

namespace Tests
{
    [TestFixture]
    public class PaymentValidatorTests
    {
        private const int MAX_AMOUNT = 500;

        [Test]
        public void IsMonetaryAmountValid_ValidAmount_ReturnsTrue()
        {
            // Arrange
            string amount = "250";

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amount, MAX_AMOUNT);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMonetaryAmountValid_MaximumAmount_ReturnsTrue()
        {
            // Arrange
            string amount = MAX_AMOUNT.ToString();

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amount, MAX_AMOUNT);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMonetaryAmountValid_ExceedsMaximum_ReturnsFalse()
        {
            // Arrange
            string amount = (MAX_AMOUNT + 1).ToString();

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amount, MAX_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_ZeroAmount_ReturnsFalse()
        {
            // Arrange
            string amount = "0";

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amount, MAX_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_NegativeAmount_ReturnsFalse()
        {
            // Arrange
            string amount = "-10";

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amount, MAX_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_NonNumericString_ReturnsFalse()
        {
            // Arrange
            string amount = "abc";

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amount, MAX_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_EmptyString_ReturnsFalse()
        {
            // Arrange
            string amount = "";

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amount, MAX_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_NullString_ReturnsFalse()
        {
            // Arrange
            string amount = null;

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amount, MAX_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNameValid_ValidName_ReturnsTrue()
        {
            // Arrange
            string name = "John Smith";

            // Act
            bool result = PaymentValidator.IsCardNameValid(name);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardNameValid_EmptyName_ReturnsFalse()
        {
            // Arrange
            string name = "";

            // Act
            bool result = PaymentValidator.IsCardNameValid(name);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNameValid_NullName_ReturnsFalse()
        {
            // Arrange
            string name = null;

            // Act
            bool result = PaymentValidator.IsCardNameValid(name);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_ValidNumber_ReturnsTrue()
        {
            // Arrange
            string[] validNumbers = {
                "4111111111111111",
                "5555555555554444",
                "3782822463100051"
            };

            // Act & Assert
            foreach (var number in validNumbers)
            {
                bool result = PaymentValidator.IsCardNumberValid(number);
                Assert.That(result, Is.True, $"Card number {number} should be valid");
            }
        }

        [Test]
        public void IsCardNumberValid_InvalidNumber_ReturnsFalse()
        {
            // Arrange
            string invalidNumber = "12345678901234562";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(invalidNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_TooShortNumber_ReturnsFalse()
        {
            // Arrange
            string tooShortNumber = "1234";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(tooShortNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_EmptyNumber_ReturnsFalse()
        {
            // Arrange
            string emptyNumber = "";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(emptyNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_NullNumber_ReturnsFalse()
        {
            // Arrange
            string nullNumber = null;

            // Act
            bool result = PaymentValidator.IsCardNumberValid(nullNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_NonNumericInput_ReturnsFalse()
        {
            // Arrange
            string nonNumericInput = "123abc456def";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(nonNumericInput);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCvvValid_ValidCVV_ReturnsTrue()
        {
            // Arrange
            string[] validCVVs = { "123", "999", "000" };

            // Act & Assert
            foreach (var cvv in validCVVs)
            {
                bool result = PaymentValidator.IsCvvValid(cvv);
                Assert.That(result, Is.True, $"CVV {cvv} should be valid");
            }
        }

        [Test]
        public void IsCvvValid_FourDigitCVV_ReturnsTrue()
        {
            // Arrange
            string fourDigitCVV = "123";

            // Act
            bool result = PaymentValidator.IsCvvValid(fourDigitCVV);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCvvValid_TooShortCVV_ReturnsFalse()
        {
            // Arrange
            string tooShortCVV = "12";

            // Act
            bool result = PaymentValidator.IsCvvValid(tooShortCVV);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCvvValid_TooLongCVV_ReturnsFalse()
        {
            // Arrange
            string tooLongCVV = "12345";

            // Act
            bool result = PaymentValidator.IsCvvValid(tooLongCVV);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCvvValid_EmptyCVV_ReturnsFalse()
        {
            // Arrange
            string emptyCVV = "";

            // Act
            bool result = PaymentValidator.IsCvvValid(emptyCVV);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCvvValid_NullCVV_ReturnsFalse()
        {
            // Arrange
            string nullCVV = null;

            // Act
            bool result = PaymentValidator.IsCvvValid(nullCVV);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCvvValid_NonNumericCVV_ReturnsFalse()
        {
            // Arrange
            string nonNumericCVV = "12A";

            // Act
            bool result = PaymentValidator.IsCvvValid(nonNumericCVV);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_ValidDate_ReturnsTrue()
        {
            // Arrange
            string validDate = "12/28";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(validDate);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsExpirationDateValid_CurrentMonthAndYear_ReturnsTrue()
        {
            // Arrange
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year % 100;
            string currentDate = $"{currentMonth:D2}/{currentYear:D2}";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(currentDate);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsExpirationDateValid_PastDate_ReturnsFalse()
        {
            // Arrange
            int lastYear = (DateTime.Now.Year - 1) % 100;
            string pastDate = $"12/{lastYear:D2}";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(pastDate);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_InvalidMonth_ReturnsFalse()
        {
            // Arrange
            string invalidMonth = "13/30";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(invalidMonth);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_InvalidFormat_ReturnsFalse()
        {
            // Arrange
            string[] invalidFormats = { "1/23", "1234", "12-23", "12.23" };

            // Act & Assert
            foreach (var format in invalidFormats)
            {
                bool result = PaymentValidator.IsExpirationDateValid(format);
                Assert.That(result, Is.False, $"Format {format} should be invalid");
            }
        }

        [Test]
        public void IsExpirationDateValid_EmptyDate_ReturnsFalse()
        {
            // Arrange
            string emptyDate = "";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(emptyDate);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_NullDate_ReturnsFalse()
        {
            // Arrange
            string nullDate = null;

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(nullDate);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_ValidEmail_ReturnsTrue()
        {
            // Arrange
            string[] validEmails = {
                "user@example.com",
                "name.surname@domain.co.uk",
                "user123@gmail.com",
                "user+tag@domain.org"
            };

            // Act & Assert
            foreach (var email in validEmails)
            {
                bool result = PaymentValidator.IsEmailValid(email);
                Assert.That(result, Is.True, $"Email {email} should be valid");
            }
        }

        [Test]
        public void IsEmailValid_InvalidEmail_ReturnsFalse()
        {
            // Arrange
            string[] invalidEmails = {
                "plainaddress",
                "@missingusername.com",
                "user@.com",
                "user@domain..com",
                "user@domain@domain.com",
                "user@domain.com.",
                ".user@domain.com"
            };

            // Act & Assert
            foreach (var email in invalidEmails)
            {
                bool result = PaymentValidator.IsEmailValid(email);
                Assert.That(result, Is.False, $"Email {email} should be invalid");
            }
        }

        [Test]
        public void IsEmailValid_EmptyEmail_ReturnsFalse()
        {
            // Arrange
            string emptyEmail = "";

            // Act
            bool result = PaymentValidator.IsEmailValid(emptyEmail);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_NullEmail_ReturnsFalse()
        {
            // Arrange
            string nullEmail = null;

            // Act
            bool result = PaymentValidator.IsEmailValid(nullEmail);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_ValidPassword_ReturnsTrue()
        {
            // Arrange
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
            string emptyPassword = "";

            // Act
            bool result = PaymentValidator.IsPasswordValid(emptyPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NullPassword_ReturnsFalse()
        {
            // Arrange
            string nullPassword = null;

            // Act
            bool result = PaymentValidator.IsPasswordValid(nullPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_ValidDigitsButCannotParse_ReturnsFalse()
        {
            // Arrange
            // A number that's too large to be parsed as an int but contains only digits
            string amount = "9999999999999999999"; // Exceeds int.MaxValue

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amount, MAX_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}