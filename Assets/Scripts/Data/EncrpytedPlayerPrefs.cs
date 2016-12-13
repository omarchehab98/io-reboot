using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System;

public class EncryptedPlayerPrefs
{
    private static string STRING = "s";
    private static string INT = "i";
    private static string FLOAT = "f";

    public static string SetString(string key, string value)
    {
        string encrypted = Encryption.Encrypt(value);
        PlayerPrefs.SetString(STRING + key, encrypted);
        return encrypted;
    }

    public static void SetInt(string key, int value)
    {
        string encrypted = Encryption.Encrypt(value.ToString());
        PlayerPrefs.SetString(INT + key, encrypted);
    }

    public static void SetFloat(string key, float value)
    {
        string encrypted = Encryption.Encrypt(value.ToString());
        PlayerPrefs.SetString(FLOAT + key, encrypted);
    }

    public static string GetString(string key, string strDefault)
    {
        string encrypted = PlayerPrefs.GetString(STRING + key, strDefault);
        try
        {
            if (encrypted != strDefault)
                return Encryption.Decrypt(encrypted);
        }
        catch (Exception) { }
        return strDefault;
    }

    public static int GetInt(string key, int intDefault)
    {
        string strDefault = intDefault.ToString();
        string encrypted = PlayerPrefs.GetString(INT + key, strDefault);
        try
        {
            if (encrypted != strDefault)
                return int.Parse(Encryption.Decrypt(encrypted));
        }
        catch (Exception) { }
        return intDefault;
    }

    public static float GetFloat(string key, float fltDefault)
    {
        string strDefault = fltDefault.ToString();
        string encrypted = PlayerPrefs.GetString(FLOAT + key, strDefault);
        try
        {
            if (encrypted != strDefault)
                return float.Parse(Encryption.Decrypt(encrypted));
        }
        catch (Exception) { }
        return fltDefault;
    }
}