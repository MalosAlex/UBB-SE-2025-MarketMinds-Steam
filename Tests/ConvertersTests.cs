using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using NUnit.Framework;
using SteamProfile.Converters;

namespace Tests
{
    public class ConvertersTests
    {
        // ---------- BooleanToVisibilityConverter (first one) ----------
        [Test]
        public void BooleanToVisibilityConverter_Convert_TrueWithoutInverse_ReturnsVisible()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();
            // Act
            var result = converter.Convert(true, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void BooleanToVisibilityConverter_Convert_FalseWithoutInverse_ReturnsCollapsed()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();
            // Act
            var result = converter.Convert(false, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void BooleanToVisibilityConverter_Convert_TrueWithInverse_ReturnsCollapsed()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();
            // Act
            var result = converter.Convert(true, null, "inverse", null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void BooleanToVisibilityConverter_Convert_FalseWithInverse_ReturnsVisible()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();
            // Act
            var result = converter.Convert(false, null, "inverse", null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void BooleanToVisibilityConverter_Convert_InvalidValue_ReturnsCollapsed()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();
            // Act
            var result = converter.Convert("not a bool", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void BooleanToVisibilityConverter_ConvertBack_VisibleWithoutInverse_ReturnsTrue()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();
            // Act
            var result = converter.ConvertBack(Visibility.Visible, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void BooleanToVisibilityConverter_ConvertBack_CollapsedWithoutInverse_ReturnsFalse()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();
            // Act
            var result = converter.ConvertBack(Visibility.Collapsed, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void BooleanToVisibilityConverter_ConvertBack_VisibleWithInverse_ReturnsFalse()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();
            // Act
            var result = converter.ConvertBack(Visibility.Visible, null, "inverse", null);
            // Assert
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void BooleanToVisibilityConverter_ConvertBack_CollapsedWithInverse_ReturnsTrue()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();
            // Act
            var result = converter.ConvertBack(Visibility.Collapsed, null, "inverse", null);
            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void BooleanToVisibilityConverter_ConvertBack_InvalidValue_ReturnsFalse()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();
            // Act
            var result = converter.ConvertBack("not a visibility", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(false));
        }

        // ---------- BoolToGridColumnConverter ----------
        [Test]
        public void BoolToGridColumnConverter_Convert_True_Returns1()
        {
            // Arrange
            var converter = new BoolToGridColumnConverter();
            // Act
            var result = converter.Convert(true, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void BoolToGridColumnConverter_Convert_False_Returns0()
        {
            // Arrange
            var converter = new BoolToGridColumnConverter();
            // Act
            var result = converter.Convert(false, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void BoolToGridColumnConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new BoolToGridColumnConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- BoolToNextGridColumnConverter ----------
        [Test]
        public void BoolToNextGridColumnConverter_Convert_True_Returns2()
        {
            // Arrange
            var converter = new BoolToNextGridColumnConverter();
            // Act
            var result = converter.Convert(true, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void BoolToNextGridColumnConverter_Convert_False_Returns1()
        {
            // Arrange
            var converter = new BoolToNextGridColumnConverter();
            // Act
            var result = converter.Convert(false, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void BoolToNextGridColumnConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new BoolToNextGridColumnConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- BoolToLastGridColumnConverter ----------
        [Test]
        public void BoolToLastGridColumnConverter_Convert_True_Returns3()
        {
            // Arrange
            var converter = new BoolToLastGridColumnConverter();
            // Act
            var result = converter.Convert(true, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void BoolToLastGridColumnConverter_Convert_False_Returns2()
        {
            // Arrange
            var converter = new BoolToLastGridColumnConverter();
            // Act
            var result = converter.Convert(false, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void BoolToLastGridColumnConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new BoolToLastGridColumnConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- BoolToInverseVisibilityConverter ----------
        [Test]
        public void BoolToInverseVisibilityConverter_Convert_True_ReturnsCollapsed()
        {
            // Arrange
            var converter = new BoolToInverseVisibilityConverter();
            // Act
            var result = converter.Convert(true, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void BoolToInverseVisibilityConverter_Convert_False_ReturnsVisible()
        {
            // Arrange
            var converter = new BoolToInverseVisibilityConverter();
            // Act
            var result = converter.Convert(false, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void BoolToInverseVisibilityConverter_Convert_InvalidValue_ReturnsVisible()
        {
            // Arrange
            var converter = new BoolToInverseVisibilityConverter();
            // Act
            var result = converter.Convert("not a bool", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void BoolToInverseVisibilityConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new BoolToInverseVisibilityConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- BoolToOpacityConverter ----------
        [Test]
        public void BoolToOpacityConverter_Convert_True_Returns1Point0()
        {
            // Arrange
            var converter = new BoolToOpacityConverter();
            // Act
            var result = converter.Convert(true, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(1.0));
        }

        [Test]
        public void BoolToOpacityConverter_Convert_False_Returns0Point3()
        {
            // Arrange
            var converter = new BoolToOpacityConverter();
            // Act
            var result = converter.Convert(false, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(0.3));
        }

        [Test]
        public void BoolToOpacityConverter_Convert_InvalidValue_Returns0Point3()
        {
            // Arrange
            var converter = new BoolToOpacityConverter();
            // Act
            var result = converter.Convert("not a bool", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(0.3));
        }

        [Test]
        public void BoolToOpacityConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new BoolToOpacityConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- Second BoolToVisibilityConverter (with ConvertBack implemented) ----------
        [Test]
        public void SecondBoolToVisibilityConverter_Convert_True_ReturnsVisible()
        {
            // Arrange
            var converter = new SteamProfile.Converters.BoolToVisibilityConverter();
            // Act
            var result = converter.Convert(true, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void SecondBoolToVisibilityConverter_Convert_False_ReturnsCollapsed()
        {
            // Arrange
            var converter = new SteamProfile.Converters.BoolToVisibilityConverter();
            // Act
            var result = converter.Convert(false, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void SecondBoolToVisibilityConverter_Convert_InvalidValue_ReturnsCollapsed()
        {
            // Arrange
            var converter = new SteamProfile.Converters.BoolToVisibilityConverter();
            // Act
            var result = converter.Convert("not a bool", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void SecondBoolToVisibilityConverter_ConvertBack_Visible_ReturnsTrue()
        {
            // Arrange
            var converter = new SteamProfile.Converters.BoolToVisibilityConverter();
            // Act
            var result = converter.ConvertBack(Visibility.Visible, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void SecondBoolToVisibilityConverter_ConvertBack_Collapsed_ReturnsFalse()
        {
            // Arrange
            var converter = new SteamProfile.Converters.BoolToVisibilityConverter();
            // Act
            var result = converter.ConvertBack(Visibility.Collapsed, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(false));
        }

        // ---------- InvertedBoolToVisibilityConverter ----------
        [Test]
        public void InvertedBoolToVisibilityConverter_Convert_True_ReturnsCollapsed()
        {
            // Arrange
            var converter = new InvertedBoolToVisibilityConverter();
            // Act
            var result = converter.Convert(true, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void InvertedBoolToVisibilityConverter_Convert_False_ReturnsVisible()
        {
            // Arrange
            var converter = new InvertedBoolToVisibilityConverter();
            // Act
            var result = converter.Convert(false, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void InvertedBoolToVisibilityConverter_Convert_InvalidValue_ReturnsVisible()
        {
            // Arrange
            var converter = new InvertedBoolToVisibilityConverter();
            // Act
            var result = converter.Convert("invalid", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void InvertedBoolToVisibilityConverter_ConvertBack_Visible_ReturnsFalse()
        {
            // Arrange
            var converter = new InvertedBoolToVisibilityConverter();
            // Act
            var result = converter.ConvertBack(Visibility.Visible, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void InvertedBoolToVisibilityConverter_ConvertBack_Collapsed_ReturnsTrue()
        {
            // Arrange
            var converter = new InvertedBoolToVisibilityConverter();
            // Act
            var result = converter.ConvertBack(Visibility.Collapsed, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        // ---------- DateOnlyToStringConverter ----------
        [Test]
        public void DateOnlyToStringConverter_Convert_ValidDate_ReturnsFormattedString()
        {
            // Arrange
            var converter = new DateOnlyToStringConverter();
            var date = new DateOnly(2025, 4, 6);
            // Act
            var result = converter.Convert(date, null, null, null);
            // Assert (uses the short date format "d")
            Assert.That(result, Is.EqualTo(date.ToString("d")));
        }

        [Test]
        public void DateOnlyToStringConverter_Convert_InvalidValue_ReturnsEmptyString()
        {
            // Arrange
            var converter = new DateOnlyToStringConverter();
            // Act
            var result = converter.Convert("not a date", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void DateOnlyToStringConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new DateOnlyToStringConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- GameCountToStringConverter ----------
        [Test]
        public void GameCountToStringConverter_Convert_IntValue_ReturnsFormattedString()
        {
            // Arrange
            var converter = new GameCountToStringConverter();
            // Act
            var result = converter.Convert(5, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo("5 games"));
        }

        [Test]
        public void GameCountToStringConverter_Convert_NonInt_ReturnsZeroGames()
        {
            // Arrange
            var converter = new GameCountToStringConverter();
            // Act
            var result = converter.Convert("not an int", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo("0 games"));
        }

        [Test]
        public void GameCountToStringConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new GameCountToStringConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- ImagePathToImageSourceConverter ----------
        [Test]
        public void ImagePathToImageSourceConverter_Convert_ValidPath_ReturnsBitmapImage()
        {
            // Arrange
            var converter = new ImagePathToImageSourceConverter();
            var validPath = "http://example.com/image.jpg";
            // Act
            var result = converter.Convert(validPath, null, null, null);
            // Assert
            Assert.That(result, Is.TypeOf<BitmapImage>());
        }

        [Test]
        public void ImagePathToImageSourceConverter_Convert_InvalidUri_ReturnsNull()
        {
            // Arrange
            var converter = new ImagePathToImageSourceConverter();
            var invalidPath = "not a uri";
            // Act
            var result = converter.Convert(invalidPath, null, null, null);
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ImagePathToImageSourceConverter_Convert_NullValue_ReturnsNull()
        {
            // Arrange
            var converter = new ImagePathToImageSourceConverter();
            // Act
            var result = converter.Convert(null, null, null, null);
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ImagePathToImageSourceConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new ImagePathToImageSourceConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- InverseBooleanToVisibilityConverter ----------
        [Test]
        public void InverseBooleanToVisibilityConverter_Convert_True_ReturnsCollapsed()
        {
            // Arrange
            var converter = new InverseBooleanToVisibilityConverter();
            // Act
            var result = converter.Convert(true, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void InverseBooleanToVisibilityConverter_Convert_False_ReturnsVisible()
        {
            // Arrange
            var converter = new InverseBooleanToVisibilityConverter();
            // Act
            var result = converter.Convert(false, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void InverseBooleanToVisibilityConverter_Convert_InvalidValue_ReturnsVisible()
        {
            // Arrange
            var converter = new InverseBooleanToVisibilityConverter();
            // Act
            var result = converter.Convert("invalid", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void InverseBooleanToVisibilityConverter_ConvertBack_Visible_ReturnsFalse()
        {
            // Arrange
            var converter = new InverseBooleanToVisibilityConverter();
            // Act
            var result = converter.ConvertBack(Visibility.Visible, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void InverseBooleanToVisibilityConverter_ConvertBack_Collapsed_ReturnsTrue()
        {
            // Arrange
            var converter = new InverseBooleanToVisibilityConverter();
            // Act
            var result = converter.ConvertBack(Visibility.Collapsed, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        // ---------- NullToCollapsedConverter ----------
        [Test]
        public void NullToCollapsedConverter_Convert_NullString_ReturnsCollapsed()
        {
            // Arrange
            var converter = new NullToCollapsedConverter();
            // Act
            var result = converter.Convert(null, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void NullToCollapsedConverter_Convert_EmptyString_ReturnsCollapsed()
        {
            // Arrange
            var converter = new NullToCollapsedConverter();
            // Act
            var result = converter.Convert("", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void NullToCollapsedConverter_Convert_NonEmptyString_ReturnsVisible()
        {
            // Arrange
            var converter = new NullToCollapsedConverter();
            // Act
            var result = converter.Convert("text", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void NullToCollapsedConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new NullToCollapsedConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- NullToVisibilityConverter ----------
        [Test]
        public void NullToVisibilityConverter_Convert_Null_ReturnsCollapsed()
        {
            // Arrange
            var converter = new NullToVisibilityConverter();
            // Act
            var result = converter.Convert(null, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void NullToVisibilityConverter_Convert_NonNull_ReturnsVisible()
        {
            // Arrange
            var converter = new NullToVisibilityConverter();
            // Act
            var result = converter.Convert("not null", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void NullToVisibilityConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new NullToVisibilityConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- NumberToVisibilityConverter ----------
        [Test]
        public void NumberToVisibilityConverter_Convert_NullValue_ReturnsCollapsed()
        {
            // Arrange
            var converter = new NumberToVisibilityConverter();
            // Act
            var result = converter.Convert(null, null, "any", null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void NumberToVisibilityConverter_Convert_NullParameter_ReturnsCollapsed()
        {
            // Arrange
            var converter = new NumberToVisibilityConverter();
            // Act
            var result = converter.Convert(0, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void NumberToVisibilityConverter_Convert_NonInverse_NumberZero_ReturnsVisible()
        {
            // Arrange
            var converter = new NumberToVisibilityConverter();
            // Act
            var result = converter.Convert(0, null, "non-inverse", null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void NumberToVisibilityConverter_Convert_NonInverse_NumberNonZero_ReturnsCollapsed()
        {
            // Arrange
            var converter = new NumberToVisibilityConverter();
            // Act
            var result = converter.Convert(5, null, "non-inverse", null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void NumberToVisibilityConverter_Convert_Inverse_NumberGreaterThanZero_ReturnsVisible()
        {
            // Arrange
            var converter = new NumberToVisibilityConverter();
            // Act
            var result = converter.Convert(5, null, "inverse", null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void NumberToVisibilityConverter_Convert_Inverse_NumberZero_ReturnsCollapsed()
        {
            // Arrange
            var converter = new NumberToVisibilityConverter();
            // Act
            var result = converter.Convert(0, null, "inverse", null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void NumberToVisibilityConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new NumberToVisibilityConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- StringFormatConverter ----------
        [Test]
        public void StringFormatConverter_Convert_WithFormat_ReturnsFormattedString()
        {
            // Arrange
            var converter = new StringFormatConverter();
            // Act
            var result = converter.Convert("World", null, "Hello {0}", null);
            // Assert
            Assert.That(result, Is.EqualTo("Hello World"));
        }

        [Test]
        public void StringFormatConverter_Convert_NoFormat_ReturnsValueToString()
        {
            // Arrange
            var converter = new StringFormatConverter();
            // Act
            var result = converter.Convert(123, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo("123"));
        }

        [Test]
        public void StringFormatConverter_Convert_NullValue_ReturnsEmptyString()
        {
            // Arrange
            var converter = new StringFormatConverter();
            // Act
            var result = converter.Convert(null, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void StringFormatConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new StringFormatConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- StringLengthToVisibilityConverter ----------
        [Test]
        public void StringLengthToVisibilityConverter_Convert_NonEmptyString_ReturnsVisible()
        {
            // Arrange
            var converter = new StringLengthToVisibilityConverter();
            // Act
            var result = converter.Convert("text", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void StringLengthToVisibilityConverter_Convert_EmptyString_ReturnsCollapsed()
        {
            // Arrange
            var converter = new StringLengthToVisibilityConverter();
            // Act
            var result = converter.Convert("", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void StringLengthToVisibilityConverter_Convert_NonString_ReturnsCollapsed()
        {
            // Arrange
            var converter = new StringLengthToVisibilityConverter();
            // Act
            var result = converter.Convert(123, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void StringLengthToVisibilityConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new StringLengthToVisibilityConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- StringToImageSourceConverter ----------
        [Test]
        public void StringToImageSourceConverter_Convert_ValidPath_ReturnsBitmapImage()
        {
            // Arrange
            var converter = new StringToImageSourceConverter();
            var validPath = "http://example.com/image.jpg";
            // Act
            var result = converter.Convert(validPath, null, null, null);
            // Assert
            Assert.That(result, Is.TypeOf<BitmapImage>());
        }

        [Test]
        public void StringToImageSourceConverter_Convert_InvalidPath_ReturnsNull()
        {
            // Arrange
            var converter = new StringToImageSourceConverter();
            var invalidPath = "not a uri";
            // Act
            var result = converter.Convert(invalidPath, null, null, null);
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void StringToImageSourceConverter_Convert_Null_ReturnsNull()
        {
            // Arrange
            var converter = new StringToImageSourceConverter();
            // Act
            var result = converter.Convert(null, null, null, null);
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void StringToImageSourceConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new StringToImageSourceConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }

        // ---------- StringToVisibilityConverter ----------
        [Test]
        public void StringToVisibilityConverter_Convert_NonEmptyString_ReturnsVisible()
        {
            // Arrange
            var converter = new StringToVisibilityConverter();
            // Act
            var result = converter.Convert("text", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        public void StringToVisibilityConverter_Convert_EmptyString_ReturnsCollapsed()
        {
            // Arrange
            var converter = new StringToVisibilityConverter();
            // Act
            var result = converter.Convert("", null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void StringToVisibilityConverter_Convert_NonString_ReturnsCollapsed()
        {
            // Arrange
            var converter = new StringToVisibilityConverter();
            // Act
            var result = converter.Convert(123, null, null, null);
            // Assert
            Assert.That(result, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        public void StringToVisibilityConverter_ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new StringToVisibilityConverter();
            // Act & Assert
            Assert.That(() => converter.ConvertBack(null, null, null, null), Throws.TypeOf<NotImplementedException>());
        }
    }
}
