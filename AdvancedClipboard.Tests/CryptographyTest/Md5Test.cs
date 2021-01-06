using AdvancedClipboard.Server.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdvancedClipboard.Tests.CryptographyTest
{
  [TestClass]
  public class Md5Test
  {
    #region Methods

    [TestMethod]
    public void SimpleTest()
    {
      // Arrange
      var testString = "Das ist ein Test für den MD5 hash Generator";

      // Act
      var md5 = CryptographyService.CreateMd5(testString);
      var hexMd5 = CryptographyService.CreateHexStringFromByteArray(md5);

      // Assert
      Assert.AreEqual("1149A17992E8BA470887322F98CCA885", hexMd5);
    }

    [TestMethod]
    public void TestWithSalt()
    {
      // Arrange
      var testString = "Das ist ein Test für den MD5 hash Generator";
      var salt = new byte[] { 154, 228, 143, 209, 192, 28, 199, 85, 81, 163, 143, 106, 198, 91, 229, 193 };

      // Act
      var md5 = CryptographyService.CreateMd5(testString);
      var salted = CryptographyService.BitwiseXOr(md5, salt);
      var saltedMd5 = CryptographyService.CreateMd5(salted);
      var saltedMd5Hex = CryptographyService.CreateHexStringFromByteArray(saltedMd5);

      // Assert
      Assert.AreEqual("61D239E7D679F92B9DECC1F4095BC545", saltedMd5Hex);
    }

    [TestMethod]
    public void ToByteArrayAndBackToHex()
    {
      // Arrange
      var originalHex = "61D239E7D679F92B9DECC1F4095BC545";

      // Act
      var byteArray = CryptographyService.CreateByteArrayFromHexString(originalHex);
      var resultingHex = CryptographyService.CreateHexStringFromByteArray(byteArray);

      // Assert
      Assert.AreEqual(originalHex, resultingHex);
    }

    #endregion Methods
  }
}