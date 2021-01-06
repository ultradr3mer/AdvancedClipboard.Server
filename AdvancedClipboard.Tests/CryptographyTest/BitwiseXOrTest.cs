using AdvancedClipboard.Server.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdvancedClipboard.Tests.CryptographyTest
{
  [TestClass]
  public class BitwiseXOrTest
  {
    #region Methods

    [TestMethod]
    public void BitwiseOrOnly1And0()
    {
      // Arrange
      byte[] a = new byte[]
      {
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111
      };

      byte[] b = new byte[]
      {
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000
      };
      // Act

      byte[] result = CryptographyService.BitwiseXOr(a, b);

      // Assert
      byte[] expected = new byte[]
      {
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111
      };

      CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void BitwiseOrOnly1And1()
    {
      // Arrange
      byte[] a = new byte[]
      {
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111
      };

      byte[] b = new byte[]
      {
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111
      };
      // Act

      byte[] result = CryptographyService.BitwiseXOr(a, b);

      // Assert
      byte[] expected = new byte[]
      {
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000,
        0b00000000
      };

      CollectionAssert.AreEqual(expected, result);
    }

    [TestMethod]
    public void BitwisePatternAnd1()
    {
      // Arrange
      byte[] a = new byte[]
      {
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
      };

      byte[] b = new byte[]
      {
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111,
        0b11111111
      };
      // Act

      byte[] result = CryptographyService.BitwiseXOr(a, b);

      // Assert
      byte[] expected = new byte[]
      {
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010,
        0b01010101,
        0b10101010
      };

      CollectionAssert.AreEqual(expected, result);
    }

    #endregion Methods
  }
}
