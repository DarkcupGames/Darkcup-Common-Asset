using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NETFX_CORE
using System.Security.Cryptography;

#else
using UnityEngine.Windows;
#endif


namespace DarkcupGames{
public class Encryption
{
//#if !NETFX_CORE
//    public static MD5 md5 = MD5.Create();

//    public static SymmetricAlgorithm GetAlgorithm(String secretKey)
//	{
//		var keyBytes = new byte[16];
//		var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
//		Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
//        return new RijndaelManaged
//		{
//			Mode = CipherMode.CBC,
//			Padding = PaddingMode.PKCS7,
//			KeySize = 128,
//			BlockSize = 128,
//			Key = keyBytes,
//			IV = keyBytes
//		};
//    }

//    public static byte[] Encrypt(byte[] plainBytes, SymmetricAlgorithm algorithm)
//	{
//		return algorithm.CreateEncryptor()
//			.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
//	}

//	public static string Decrypt(byte[] encryptedData, SymmetricAlgorithm algorithm)
//	{
//		return Encoding.UTF8.GetString(algorithm.CreateDecryptor()
//			.TransformFinalBlock(encryptedData, 0, encryptedData.Length));
//	}
//#else

//#endif

    /// <summary>
    /// Encrypts plaintext using AES 128bit key and a Chain Block Cipher and returns a base64 encoded string
    /// </summary>
    /// <param name="plainText">Plain text to encrypt</param>
    /// <param name="key">Secret key</param>
    /// <returns>Base64 encoded string</returns>
 //   public static String Encrypt(String plainText, String key,System.Action<bool> result=null)
	//{
	//	//key += SystemHelper.deviceUniqueID;
	//	//var plainBytes = Encoding.UTF8.GetBytes(plainText);
	//	//return Convert.ToBase64String(Encrypt(plainBytes, GetAlgorithm(key)));
	//}

 //   public static byte[] Encrypt(byte[] plainBytes, String key, System.Action<bool> result = null) {
 //       //return Encrypt(plainBytes, GetAlgorithm(key));
 //   }

    /// <summary>
    /// Decrypts a base64 encoded string using the given key (AES 128bit key and a Chain Block Cipher)
    /// </summary>
    /// <param name="encryptedText">Base64 Encoded String</param>
    /// <param name="key">Secret Key</param>
    /// <returns>Decrypted String</returns>
 //   public static String Decrypt(String encryptedText, String key,System.Action<bool> result=null)
	//{
	//	//key += SystemHelper.deviceUniqueID;
	//	try
	//	{
	//		var encryptedBytes = Convert.FromBase64String(encryptedText);
	//		if(result!=null)
	//		{
	//			result(true);
	//		}
	//		return (Decrypt(encryptedBytes, GetAlgorithm(key)));

	//	}
	//	catch(Exception ex)
	//	{
	//		UnityEngine.Debug.LogWarning("Decode Exception: "+ex.Message);
	//		if(result!=null)
	//		{
	//			result(false);
	//		}
	//		return "";
	//	}
	//}


	//public static String EncryptNetworkMessage(String plainText, String key)
	//{
	//	var plainBytes = Encoding.UTF8.GetBytes(plainText);
	//	return Convert.ToBase64String(Encrypt(plainBytes, GetAlgorithm(key)));
	//}
	//public static String DecryptNetworkMessage(String encryptedText, String key)
	//{
	//	try
	//	{
	//		var encryptedBytes = Convert.FromBase64String(encryptedText);
	//		return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetAlgorithm(key)));
	//	}
	//	catch(Exception ex)
	//	{
	//		UnityEngine.Debug.LogError("Decode Exception:"+ex.Message);
	//		return "";
	//	}
	//}

//	public static string GetMD5Hash(string plainText)
//	{
//#if !NETFX_CORE
//        return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(plainText))).Replace("-", String.Empty);
//#else
//        return BitConverter.ToString(UnityEngine.Windows.Crypto.ComputeMD5Hash(Encoding.UTF8.GetBytes(plainText))).Replace("-", String.Empty);
//#endif
//    }

//    public static string GetMD5Hash(byte[] plainBytes)
//	{
//#if !NETFX_CORE
//        return BitConverter.ToString(md5.ComputeHash(plainBytes)).Replace("-", String.Empty);
//#else
//        return BitConverter.ToString(UnityEngine.Windows.Crypto.ComputeMD5Hash(plainBytes)).Replace("-", String.Empty);
//#endif

//    }

    /// <summary>
    /// Encrypts plaintext using AES 128bit key and a Chain Block Cipher and returns a base64 encoded string
    /// Include checksum in the encryption data to prevent data modification
    /// </summary>
    /// <param name="plainText">Plain text to encrypt</param>
    /// <param name="key">Secret key</param>
    /// <returns>Base64 encoded string</returns>
 //   public static String EncryptWithChecksum(String plainText, String key)
	//{
	//	string hash = GetMD5Hash(plainText);
	//	string plainTextWithHash = hash + "," + plainText;
	//	return Encrypt(plainTextWithHash, key);
	//}

	/// <summary>
	/// Decrypts a base64 encoded string using the given key (AES 128bit key and a Chain Block Cipher)
	/// Extract hash value from the data, if hash value does not match with the hash calculation from the string the value is rejected
	/// </summary>
	/// <param name="encryptedText">Base64 Encoded String</param>
	/// <param name="key">Secret Key</param>
	/// <returns>Decrypted String</returns>
	//public static bool DecryptWithChecksum(String encryptedText, String key, out String result)
	//{
	//	result = null;

	//	// Decrypt plain text with include hash
	//	String plainTextWithHash = Decrypt(encryptedText, key);
	//	int pos = plainTextWithHash.IndexOf(',');
	//	if (pos <= 0)
	//		return false;

	//	// extract the hash and plain text
	//	string hash = plainTextWithHash.Substring(0, pos);
	//	string plainText = plainTextWithHash.Substring(pos + 1);

	//	// Compare the hash
	//	if (!hash.Equals(GetMD5Hash(plainText)))
	//		return false;

	//	// return result
	//	result = plainText;
	//	return true;
	//}
}
}