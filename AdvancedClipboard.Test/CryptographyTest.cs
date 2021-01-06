using AdvancedClipboard.Server.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdvancedClipboard.Test
{
  [TestClass]
  public class CryptographyTest
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

    #endregion Methods
  }
}
