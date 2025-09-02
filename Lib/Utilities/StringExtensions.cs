using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Jworkz.ResonitePowerShellModule.Core.Utilities;

public static class StringExtensions
{
    /// <summary>
    /// Regex that matches query param strings as found in URLs
    /// </summary>
    public static readonly Regex QueryParamRegex = new Regex(@"(?<key>[\w_-]+)=(?<value>[\w_-]+)", RegexOptions.Compiled);

    /// <summary>
    /// Checks if the given string is a user id.
    /// </summary>
    /// <param name="str">String to check</param>
    /// <returns>true if the provided string is a user id; otherwise, false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUserId(this string str) => str.StartsWith("U-");

    /// <summary>
    /// Checks if the given string is a group id.
    /// </summary>
    /// <param name="str">String to check</param>
    /// <returns>true if the provided string is a group id; otherwise, false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGroupId(this string str) => str.StartsWith("G-");

    /// <summary>
    /// Checks if the given string is a machine id.
    /// </summary>
    /// <param name="str">String to check</param>
    /// <returns>true if the provided string is a machine id; otherwise, false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMachineId(this string str) => str.StartsWith("M-");

    /// <summary>
    /// Converts string to a SecureString object
    /// </summary>
    /// <param name="str">String to convert</param>
    /// <returns>String as a SecureString object</returns>
    public static SecureString ToSecureString(this string str)
    {
        var secureStr = new SecureString();
        var chars = str.ToCharArray();

        foreach (char c in chars)
        {
            secureStr.AppendChar(c);
        }

        return secureStr;
    }

    /// <summary>
    /// Converts string to a byte[]
    /// </summary>
    /// <param name="str">String to convert</param>
    /// <returns>String as a byte[]</returns>
    public static byte[]? ToBytes(this string str) => str == null ? null : Encoding.Unicode.GetBytes(str);

    /// <summary>
    /// Converts SecureString object to a byte[]
    /// </summary>
    /// <param name="secureStr">SecureString to convert</param>
    /// <returns>SecureString as a byte[]</returns>
    public static byte[]? ToBytes(this SecureString secureStr) => secureStr.ToPlainText().ToBytes();

    /// <summary>
    /// Converts SecureString object to plain text that is readable
    /// </summary>
    /// <param name="secureStr">SecureString object to convert to plain text</param>
    /// <returns>SecureString as plain text</returns>
    public static string ToPlainText(this SecureString secureStr) =>
        Marshal.PtrToStringAuto(Marshal.SecureStringToBSTR(secureStr)) ?? string.Empty;

    /// <summary>
    /// Converts a byte array into a hash string
    /// </summary>
    /// <param name="bytes">Byte array to generate the hash string from</param>
    /// <param name="upperCase">To use uppercase letters or not</param>
    /// <returns>byte[] as a hash string</returns>
    public static string ToHex(this byte[] bytes, bool upperCase = false)
    {
        var sb = new StringBuilder(bytes.Length * 2);

        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Throws an exception if the provided string is null or empty.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <param name="msg">The exception message.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void ThrowOnNullOrEmpty(this string? str, string msg)
    {
        if (string.IsNullOrEmpty(str))
        {
            throw new ArgumentException(msg);
        }
    }

    /// <summary>
    /// Throws an exception if the provided uri is null.
    /// </summary>
    /// <param name="uri">The uri to check.</param>
    /// <param name="msg">The exception message.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void ThrowOnNullOrEmpty(this Uri? uri, string msg)
    {
        if (uri == null)
        {
            throw new ArgumentException(msg);
        }
    }

    /// <summary>
    /// Throws an exception if the provided string does not match the pattern.
    /// </summary>
    /// <param name="str">The string to match.</param>
    /// <param name="pattern">The regex pattern that the string needs to follow.</param>
    /// <param name="msg">The exception message.</param>
    /// <param name="allowNullOrEmpty">Whether or not to allow null or empty strings.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void ThrowOnInvalidPattern(this string? str, Regex pattern, string msg, bool allowNullOrEmpty = false)
    {
        if (!allowNullOrEmpty && !pattern.IsMatch(str ?? string.Empty))
        {
            throw new ArgumentException(msg);
        }
    }

    /// <summary>
    /// Throws an exception if the provided Uri does not match the pattern.
    /// </summary>
    /// <param name="str">The string to match.</param>
    /// <param name="pattern">The regex pattern that the string needs to follow.</param>
    /// <param name="msg">The exception message.</param>
    /// <param name="allowNullOrEmpty">Whether or not to allow null or empty strings.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void ThrowOnInvalidPattern(this Uri? uri, Regex pattern, string msg, bool allowNullOrEmpty = false)
    {
        (uri?.ToString() ?? string.Empty).ThrowOnInvalidPattern(pattern, msg, allowNullOrEmpty);
    }
}
