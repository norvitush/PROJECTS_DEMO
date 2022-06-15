using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using UnityEngine;


namespace VOrb
{
    public static class DataKeeper
    {       
        private static readonly int paprika = 951753465;

        public static string SaveCryptedTo(string path, string data, bool needSaveToFile = true)
        {
            string CryptedStr = string.Empty;
            foreach (char j in data)
            {
                CryptedStr += (char)((int)j ^ 49);
            }
            if (needSaveToFile)
            {
                File.WriteAllText(path, CryptedStr);
            }            
            return CryptedStr;
        }
        public static string  UncryptStr(string data)
        {
            string UnCryptedStr = string.Empty;
            foreach (char j in data)
            {
                UnCryptedStr += (char)((int)j ^ 49);
            }
            return UnCryptedStr;
        }

        //УНИВЕРСАЛЬНЫЕ МЕТОДЫ
        public static void SaveParam<T>(string key, T param)
        {
           
            if ((param.GetType() == typeof(int)) || (param.GetType() == typeof(SafeInt)))
            {
                int inP = int.Parse((param.ToString()));
                SetInt(key, inP);
            }
            else if ((param.GetType() == typeof(float)) || (param.GetType() == typeof(SafeFloat)))
            {
                float pf = float.Parse(param.ToString());
                SetFloat(key, pf);
            }
            else if (param.GetType() == typeof(String))
            {
                SetString(key, param.ToString());
            }
            else if (param.GetType() == typeof(Vector2))
            {
                Vector2 parsed = param.ToString().ParseToVector2();
                SetFloat(key + "_x", parsed.x);
                SetFloat(key + "_y", parsed.y);
            }
            PlayerPrefs.Save();
        }



        public static object LoadParam<T>(string key, T retIfNONE)
        {

            if (retIfNONE.GetType() == typeof(int))
            {
                return GetInt(key, int.Parse(retIfNONE.ToString()));
            }
            else if (retIfNONE.GetType() == typeof(float))
            {
                return GetFloat(key, float.Parse(retIfNONE.ToString()));
            }
            else if (retIfNONE.GetType() == typeof(String))
            {
                return GetString(key, retIfNONE.ToString());
            }
            else if (retIfNONE.GetType() == typeof(Vector2))
            {
                float ix, iy;
                ix = GetFloat(key + "_x", 0f);
                iy = GetFloat(key + "_y", 0f);
                Debug.Log(ix + " load|load " + iy);
                return new Vector2(ix, iy);
            }
            return retIfNONE;
        }

        //SET методы  ----------
        private static void SetInt(string key, int value)
        {
            int salted = value ^ paprika;
            PlayerPrefs.SetInt(StringHash(key), salted);
            PlayerPrefs.SetInt(StringHash("_" + key), IntHash(value));
        }

        private static void SetFloat(string key, float value)
        {
            int intValue = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);

            int salted = intValue ^ paprika;
            PlayerPrefs.SetInt(StringHash(key), salted);
            PlayerPrefs.SetInt(StringHash("_" + key), IntHash(intValue));
        }
        private static void SetString(string key, string value)
        {
            string CryptedStr = string.Empty;
            foreach (char j in value)
            {
                CryptedStr += (char)((int)j ^ 49);
            }          
            PlayerPrefs.SetString(StringHash(key), CryptedStr);
        }
        //GET методы -----------
        private static float GetFloat(string key, float defaultVal)
        {
            string hashedKey = StringHash(key);
            if (!PlayerPrefs.HasKey(hashedKey)) return defaultVal;

            int salted = PlayerPrefs.GetInt(hashedKey);
            int value = salted ^ paprika;

            int loadedHash = PlayerPrefs.GetInt(StringHash("_" + key));
            if (loadedHash != IntHash(value)) return defaultVal; //хакнули
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        private static int GetInt(string key)
        {
            return GetInt(key, 0);
        }
        private static int GetInt(string key, int defaultVal)
        {
            string hashedKey = StringHash(key);
            if (!PlayerPrefs.HasKey(hashedKey)) return defaultVal;

            int salted = PlayerPrefs.GetInt(hashedKey);
            int value = salted ^ paprika;

            int loadedHash = PlayerPrefs.GetInt(StringHash("_" + key));
            if (loadedHash != IntHash(value)) return defaultVal; //хакнули
            return value;
        }

        private static string GetString(string key, string DefaultVal)
        {
            string hashedKey = StringHash(key);
            if (!PlayerPrefs.HasKey(hashedKey)) return DefaultVal;
            string UnCryptedStr = string.Empty;
            string solted = PlayerPrefs.GetString(hashedKey);
            foreach (char j in solted)
            {
                UnCryptedStr += (char)((int)j ^ 49);
            }
            return UnCryptedStr;
        }

        //HASH
        private static int IntHash(int x)
        {
            x = ((x >> 16) ^ x) * 0x27d9a1b;
            x = ((x >> 16) ^ x) * 0x27d9a1b;
            x = (x >> 16) ^ x;
            return x;
        }

        private static string StringHash(string s)
        {
            HashAlgorithm algo = SHA256.Create();
            StringBuilder sb = new StringBuilder();

            var bytes = algo.ComputeHash(Encoding.UTF8.GetBytes(s));
            foreach (byte b in bytes) sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        //удаление и проверки
        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(StringHash(key));
            PlayerPrefs.DeleteKey(StringHash("_" + key));
        }
        public static bool HasKey(string key, bool notString = true)
        {
            if (!PlayerPrefs.HasKey(StringHash(key)))
                return false;
            else if (notString)
            {

                int salted = PlayerPrefs.GetInt(StringHash(key));
                int value = salted ^ paprika;

                int loadedHash = PlayerPrefs.GetInt(StringHash("_" + key));
                if (loadedHash != IntHash(value)) return false; //хакнули
                return true;
            }
            else
                return true;


        }
        public static void DeleteAllKeys()
        {
            PlayerPrefs.DeleteAll();
        }

        
    }

}
