using AdvancedClipboard.Server.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdvancedClipboard.Tests.ServicesTest
{
  [TestClass]
  public class UserServiceTest
  {
    #region Methods

    [TestMethod]
    public void CheckPasswordCorrect()
    {
      //Arrange
      var userService = new UserSevice();
      var testPassword = "123Password123!";

      // Act
      (string password, string salt) = userService.GeneratePasswordHash(testPassword);

      // Asset
      Assert.IsTrue(userService.CheckPassword(testPassword, salt, password));
    }

    [TestMethod]
    public void CheckPasswordWrongSalt()
    {
      //Arrange
      var userService = new UserSevice();
      var testPassword = "123Password123!";
      var wrongSalt = CryptographyService.CreateHexStringFromByteArray(CryptographyService.GenerateSalt());

      // Act
      (string password, string salt) = userService.GeneratePasswordHash(testPassword);

      // Asset
      Assert.IsFalse(userService.CheckPassword(testPassword, wrongSalt, password));
    }

    [TestMethod]
    public void CheckPasswordCorrectWrongPassword()
    {
      //Arrange
      var userService = new UserSevice();
      var testPassword = "123Password123!";
      var wrongPassword = "NOT_123Password123!";

      // Act
      (string password, string salt) = userService.GeneratePasswordHash(testPassword);

      // Asset
      Assert.IsFalse(userService.CheckPassword(testPassword, salt, wrongPassword));
    }

    #endregion Methods
  }
}