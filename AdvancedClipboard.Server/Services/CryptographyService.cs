using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AdvancedClipboard.Server.Services
{
  public static class CryptographyService
  {
    #region Methods

    public static byte[] BitwiseXOr(byte[] a, byte[] b)
    {
      byte[] result = new byte[16];
      for (int i = 0; i < 16; i++)
      {
        result[i] = (byte)(a[i] ^ b[i]);
      }
      return result;
    }

    public static byte[] CreateByteArrayFromHexString(string input)
    {
      return Enumerable.Range(0, input.Length)
                       .Where(x => x % 2 == 0)
                       .Select(x => Convert.ToByte(input.Substring(x, 2), 16))
                       .ToArray();
    }

    public static string CreateHexStringFromByteArray(byte[] input)
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < input.Length; i++)
      {
        sb.Append(input[i].ToString("X2"));
      }
      return sb.ToString();
    }

    public static byte[] CreateMd5(string input)
    {
      byte[] inputBytes = Encoding.ASCII.GetBytes(input);
      return CreateMd5(inputBytes);
    }

    public static byte[] CreateMd5(byte[] input)
    {
      using (MD5 md5 = MD5.Create())
      {
        return md5.ComputeHash(input);
      }
    }

    public static byte[] GenerateSalt()
    {
      RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
      byte[] saltBytes = new byte[16];
      provider.GetBytes(saltBytes);

      return saltBytes;
    }

    #endregion Methods
  }
}