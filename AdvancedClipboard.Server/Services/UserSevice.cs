using AdvancedClipboard.Server.Data;
using AdvancedClipboard.Server.DataBase;
using System.Threading.Tasks;

namespace AdvancedClipboard.Server.Services
{
  public class UserSevice
  {
    #region Methods

    public bool CheckPassword(string clearPassword, string salt, string savedPaswordHash)
    {
      byte[] saltBytes = CryptographyService.CreateByteArrayFromHexString(salt);
      string passwordHashHex = HashSaltHashPassword(clearPassword, saltBytes);
      return string.Compare(passwordHashHex, savedPaswordHash, ignoreCase: true) == 0;
    }

    public async Task CreateUserAsync(SignupPostData signupPostData, DatabaseContext context)
    {
      (string password, string salt) = this.GeneratePasswordHash(signupPostData.Password);

      UserEntity user = new UserEntity()
      {
        Login = signupPostData.Username,
        Password = password,
        Salt = salt
      };
      await context.AddAsync(user);
      await context.SaveChangesAsync();
    }

    public (string password, string salt) GeneratePasswordHash(string clearPassword)
    {
      byte[] salt = CryptographyService.GenerateSalt();
      string saltHex = CryptographyService.CreateHexStringFromByteArray(salt);

      string passwordHashHex = HashSaltHashPassword(clearPassword, salt);

      return (passwordHashHex, saltHex);
    }

    private static string HashSaltHashPassword(string clearPassword, byte[] salt)
    {
      byte[] passwordMd5 = CryptographyService.CreateMd5(clearPassword);
      byte[] saltedPassword = CryptographyService.BitwiseXOr(passwordMd5, salt);
      byte[] saltedPasswordMd5 = CryptographyService.CreateMd5(saltedPassword);
      return CryptographyService.CreateHexStringFromByteArray(saltedPasswordMd5);
    }

    #endregion Methods
  }
}