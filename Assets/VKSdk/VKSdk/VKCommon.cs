using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
#if VK_TMP_ACTIVE
using TMPro;
#endif
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = System.Random;

namespace VKSdk
{
    public class VKCommon
    {
        public static DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static Random random = new Random();
        private static List<string> badMessages = null;
        private static List<char> specialCharNames = new List<char>{' ', '.',  '*', '!', ',', ';', ':', '`'};

        #region Color Hex
        public static string HEX_WHITE = "#FFFFFF";
        public static string HEX_BLACK = "#000000";
        public static string HEX_RED = "#FF090D";
        public static string HEX_RED_LIGHT = "#E65D47";
        public static string HEX_GREEN = "#0F922B";
        public static string HEX_GREEN_LIGHT = "#CFFF4C";
        public static string HEX_BLUE = "#3A3BF7";
        public static string HEX_BLUE_LIGHT = "#00D9F5";
        public static string HEX_VIOLET = "#9B15EF";
        public static string HEX_VIOLET_LIGHT = "#CD7DFF";
        public static string HEX_YELLOW = "#F1DE50";
        public static string HEX_YELLOW_DARK = "#DDC47A";
        public static string HEX_YELLOW_LIGHT = "#FFEFA2";
        public static string HEX_ORANGE = "#D65C2C";
        public static string HEX_ORANGE_LIGHT = "#FA6C32";
        public static string HEX_GRAY = "#7C7C7C";
        public static string HEX_GRAY_LOW = "#707070";
        public static string HEX_GRAY_LIGHT = "#868686";
        #endregion

        #region LUA METHOD
        public static void PrintType(object o)
        {
            Debug.Log("type:" + o.GetType() + ", value:" + o);
        }

        public static DateTime ConvertLongToDateTime(long ticks)
        {
            return dtStart.AddMilliseconds(ticks).ToLocalTime();
        }

        public static string ConvertDictionaryToString(System.Collections.Generic.IDictionary<string, object> dict)
        {
            string s = "";
            foreach (string a in dict.Keys)
            {
                s += "KEY: " + a + " - Type: " + dict[a].GetType().ToString() + " - value: " + dict[a] + "\n";
            }
            return s;
        }

        public static string ConvertDictionaryToString(System.Collections.Generic.IDictionary<string, string> dict)
        {
            string s = "";
            foreach (string a in dict.Keys)
            {
                s += "KEY: " + a + " - Type: " + dict[a].GetType().ToString() + " - value: " + dict[a] + "\n";
            }
            return s;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string FormatStringToStringDateTime(string formatType, string jsonDate)
        {
            string s = jsonDate.Replace("/Date(", "").Replace(")/", "");
            DateTime dt = ConvertLongToDateTime(long.Parse(s));

            return dt.ToString(formatType);
        }

        public static string FormatOtherStringToStringDateTime(string formatType, string jsonDate)
        {
            DateTime dt = Convert.ToDateTime(jsonDate);
            return dt.ToString(formatType);
        }

        public static string FormatExactStringToStringDateTime(string date, string exactType, string formatType)
        {
            DateTime time = DateTime.ParseExact(date, exactType, System.Globalization.CultureInfo.InvariantCulture);
            return time.ToString(formatType);
        }

        public static string FormatDateTimeToString(DateTime time, string formatType)
        {
            return time.ToString(formatType);
        }

        public static string StringFormatNumber(string format, long number)
        {
            return String.Format(format, number);
        }

        public static string[] SplitString(string strBase, string key)
        {
            return Regex.Split(strBase, key);
        }

        public static bool IsStringContains(string str, string value, bool isUpper)
        {
            if (isUpper) return str.ToUpper().Contains(value);
            return str.Contains(value);
        }

        public static string CurrentPlatform()
        {
#if UNITY_EDITOR
            return "EDITOR";
#elif UNITY_ANDROID
        return "ANDROID";
#elif UNITY_IOS
        return "IOS";
#elif UNITY_STANDALONE_WIN
        return "WINDOW";
#elif UNITY_STANDALONE_OSX
        return "MACOSX";
#elif UNITY_WEBGL
        return "WEBGL";
#else
            return "UNKNOW";
#endif
        }

        public static string TrimString(string strBase)
        {
            return strBase.Trim();
        }

        public static double TruncateNumber(double number)
        {
            return Math.Truncate(number);
        }

        public static int RandomIntNumber(int from, int to)
        {
            return UnityEngine.Random.Range(from, to);
        }

        public static double RandomDoubleNumber(float from, float to)
        {
            return UnityEngine.Random.Range(from, to);
        }

        public static List<int> RandomListIntNumber(int from, int to, int take)
        {
            var rnd = new System.Random();
            return Enumerable.Range(from, to).OrderBy(x => rnd.Next()).Take(take).ToList();
        }

        public static double FloorNumber(double number)
        {
            return Math.Floor(number);
        }

        public static Vector3 GetRandomVector3ByRange(Vector3 vStart, float range)
        {
            return new Vector3(vStart.x + UnityEngine.Random.Range(-range, range), vStart.y + UnityEngine.Random.Range(-range, range), 0);
        }
        #endregion

        #region Generate Random String
        public static string GenerateRandomString(int length)
        {
            if (length <= 0)
            {
                throw new Exception("Expected nonce to have positive length");
            }

            const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
            var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
            var result = string.Empty;
            var remainingLength = length;

            var randomNumberHolder = new byte[1];
            while (remainingLength > 0)
            {
                var randomNumbers = new List<int>(16);
                for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
                {
                    cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                    randomNumbers.Add(randomNumberHolder[0]);
                }

                for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
                {
                    if (remainingLength == 0)
                    {
                        break;
                    }

                    var randomNumber = randomNumbers[randomNumberIndex];
                    if (randomNumber < charset.Length)
                    {
                        result += charset[randomNumber];
                        remainingLength--;
                    }
                }
            }

            return result;
        }

        public static string GenerateSHA256NonceFromRawNonce(string rawNonce)
        {
            var sha = new SHA256Managed();
            var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
            var hash = sha.ComputeHash(utf8RawNonce);

            var result = string.Empty;
            for (var i = 0; i < hash.Length; i++)
            {
                result += hash[i].ToString("x2");
            }

            return result;
        }
        #endregion
        #region Clipboard
        public static void CopyToClipboard(string data)
        {
#if UNITY_ANDROID || UNITY_IOS
            // UniClipboard.SetText(data);
#else
            GUIUtility.systemCopyBuffer = data;
#endif
        }

        //         public static string GetFromClipboard()
        //         {
        // #if UNITY_ANDROID || UNITY_IOS
        //             // return UniClipboard.GetText();
        //             return "";
        // #else
        //             GUIUtility.systemCopyBuffer = data;
        // #endif
        //         }
        #endregion

        #region Crypt
        public static string MD5Encode(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string MD5OTPDecode(string input, int length)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            int _parsedReqNo = BitConverter.ToInt32(hash, 0);
            string _strParsedReqId = Math.Abs(_parsedReqNo).ToString();
            if (_strParsedReqId.Length < length)
            {
                StringBuilder sb = new StringBuilder(_strParsedReqId);
                for (int k = 0; k < (length - _strParsedReqId.Length); k++)
                {
                    sb.Insert(0, '0');
                }
                _strParsedReqId = sb.ToString();
            }
            return _strParsedReqId.Substring(0, length);
        }

        public static string Base64Encode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                return "";
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(32).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(32).Take(32).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((32) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((32) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, 1000))
            {
                var keyBytes = password.GetBytes(32);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region UNITY SAVE METHOD
        public static void PlayerPrefsString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public static void PlayerPrefsNumber(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static string PlayerPrefsGetString(string key, string value)
        {
            return PlayerPrefs.GetString(key, value);
        }

        public static float PlayerPrefsGetNumber(string key, float value)
        {
            return PlayerPrefs.GetFloat(key, value);
        }

        public static void PlayerPrefsDeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        #endregion

        #region MONEY METHOD
        public static string ConvertSubMoneyString(double money, int maxLength = 3, bool space = false)
        {
            string str = "";
            double result = 0;
            if (money < 1000)
            {
                result = money;
            }
            else if (money < 1000000)
            {
                result = money / 1000;
                str = "K";
            }
            else if (money < 1000000000)
            {
                result = money / 1000000;
                str = "M";
            }
            else if (money < 1000000000000)
            {
                result = money / 1000000000;
                str = "B";
            }

            string temp = (Math.Truncate(result * 100) / 100).ToString();
            if (temp.Replace(".", "").Length > maxLength)
            {
                temp = (Math.Truncate(result * 10) / 10).ToString();
                if (temp.Replace(".", "").Length > maxLength)
                    temp = Math.Truncate(result).ToString();
            }

            return temp + (space ? " " : "") + str;
        }

        public static string ConvertStringMoneyToNormalString(string money)
        {
            return Regex.Replace(money, "[^0-9]", "");
        }

        public static int ConvertStringMoneyToNumber(string money)
        {
            try
            {
                return int.Parse(ConvertStringMoneyToNormalString(money));
            }
            catch
            {
                return 0;
            }
        }

        public static double ConvertStringMoneyToDouble(string money)
        {
            try
            {
                return double.Parse(ConvertStringMoneyToNormalString(money));
            }
            catch
            {
                return 0;
            }
        }

        public static string ConvertStringMoney(double money, string comma = ",")
        {
            if ((int)money == 0)
                return "0";

            money = Math.Truncate(money);
            bool isAm = false;
            if (money < 0f)
            {
                money = Math.Abs(money);
                isAm = true;
            }

            if (money < 1000)
                return (isAm ? "-" : "") + money.ToString("F0");

            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            string s = (isAm ? "-" : "") + Math.Truncate(money).ToString("0,0", elGR);

            if (comma.Equals(",")) return s.Replace(".", ",");
            return s;
        }

        public static string ConvertStringMoney(string money)
        {
            return ConvertStringMoney(double.Parse(money));
        }
        #endregion

        #region STRING BAD MESAGE
        public static string ReplaceBadMessage(string msg)
        {
            try
            {
                if (badMessages == null)
                {
                    var data = Resources.Load("Config/badMessage") as TextAsset;
                    // Debug.Log("data.text: " + data.text);
                    badMessages = LitJson.JsonMapper.ToObject<List<string>>(data.text);
                    // Debug.Log("badMessages: " + badMessages.Count);
                }

                // foreach()
                var re = new Regex(
                    @"\b("
                    + string.Join("|", badMessages.Select(word =>
                        string.Join(@"\s*", word.ToCharArray())))
                    + @")\b", RegexOptions.IgnoreCase);
                return re.Replace(msg, match =>
                {
                    return new string('*', match.Length);
                });
            }
            catch { }
            return msg;
            // void Main()
            // {
            //     string[] badWords = new[] { "bad", "word", "words" };

            //     string input = "This is a bad string containing some of the words in the"
            //         + " list, even one w o r d that has whitespace";
            //     string output = Filter(input, badWords);
            //     Debug.WriteLine(output);
            // }

            // public static string Filter(string input, string[] badWords)
            // {
            //     var re = new Regex(
            //         @"\b("
            //         + string.Join("|", badWords.Select(word =>
            //             string.Join(@"\s*", word.ToCharArray())))
            //         + @")\b", RegexOptions.IgnoreCase);
            //     return re.Replace(input, match =>
            //     {
            //         return new string('*', match.Length);
            //     });
            // }
        }
        #endregion

        #region STRING METHOD
        public static string GetSystemArg(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
            return null;
        }

        public static string UnescapeString(string data)
        {
            return Regex.Unescape(data);
        } 

        public static bool IsMatch(string str)
        {
            return Regex.IsMatch(str, @"\s");
        }

        public static bool HasSpecialChar(string input, string[] ignores = null)
        {
            string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>*^@><~`+,_";
            if (ignores != null)
            {
                foreach (var item in ignores)
                {
                    specialChar = specialChar.Replace(item, "");
                }
            }

            foreach (var item in specialChar)
            {
                if (input.Contains(item)) return true;
            }
            if (input.Contains("\"")) return true;

            return false;
        }

        public static bool CheckResponseError(string data)
        {
            return string.IsNullOrEmpty(data) || data.Contains("<!DOCTYPE ") || data.Contains("<?xml version=") || data.Contains("<Error><Code>") || data.Contains("<div") || data.Contains("<head>");
        }

        public static Dictionary<string, string> GetParamsFromURL(string uri)
        {
            var matches = Regex.Matches(uri, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.Compiled);
            return matches.Cast<Match>().ToDictionary(
                m => Uri.UnescapeDataString(m.Groups[2].Value),
                m => Uri.UnescapeDataString(m.Groups[3].Value)
            );
        }

        public static string GetStraightString(string text)
        {
            return text.Replace(" ", "\n");
        }

        public static string ConvertStringHTML(string HTMLText)
        {
            var reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            return reg.Replace(HTMLText, "");
        }

        public static string ConvertEllipseText(string txtInfo, int contentLength)
        {
            if (txtInfo.Length > contentLength)
                return txtInfo.Remove(contentLength) + "...";

            return txtInfo;
        }

        public static double GetInputFieldNumber(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;

            s = Regex.Replace(s, "[^0-9]", "");

            if (string.IsNullOrEmpty(s))
                return 0;

            try
            {
                return double.Parse(s);
            }
            catch (Exception e)
            {
                VKDebug.LogError(e.Message);
                return 0;
            }
        }

        public static string CustomString(string parent, string key, params string[] childs)
        {
            var regex = new Regex(Regex.Escape(key));
            var newText = parent;
            for (int i = 0; i < childs.Length; i++)
            {
                newText = regex.Replace(newText, childs[i], 1);
            }

            return newText;
        }

        public static string FillColorString(string text, string codeColor)
        {
            return "<color=" + codeColor + ">" + text + "</color>";
        }

        public static string SetSizeItalicsString(string text, float percent)
        {
            return SetItalicsString(SetSizeString(text, percent));
        }

        public static string SetSizeString(string text, float percent)
        {
            return "<size=" + percent + "%>" + text + "</size>";
        }

        public static string SetItalicsString(string text)
        {
            return "<i>" + text + "</i>";
        }

        public static string SetBoldString(string text)
        {
            return "<b>" + text + "</b>";
        }

        public static string StringReplate(string str)
        {
            return str.Replace("<br/>", "\n");
        }

        public static string SetAlign(string text, TextAlignment align)
        {
            if (align == TextAlignment.Center)
            {
                return "<align=\"center\">" + text + "</align>";
            }
            else if (align == TextAlignment.Right)
            {
                return "<align=\"right\">" + text + "</align>";
            }
            else if (align == TextAlignment.Left)
            {
                return "<align=\"left\">" + text + "</align>";
            }
            return text;
        }

        public static bool IsEmail(string inputEmail)
        {
            inputEmail = inputEmail ?? string.Empty;
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
        
        public static int CountSpecialCharacter(string data, string special = "[~!@#$%^&*()_+{}:\"<>?]")
        {
            return data.Count(f => specialCharNames.Contains(f));
        }
        #endregion

        #region TIME METHOD
        public static long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        public static string ConvertIntegerToTime(int time)
        {
            string s = "";
            if (time / 60 < 10)
                s += "0" + ((int)(time / 60)) + ":";
            else
                s += ((int)(time / 60)) + ":";

            if (time % 60 < 10)
                s += "0" + time % 60;
            else
                s += ((int)(time % 60));

            return s;
        }

        public static string ConvertIntegerToTimeString(int seconds, bool onlySeconds = false)
        {
            if(onlySeconds)
            {
                return seconds + " " + (seconds > 1 ? "SECONDS" : "SECOND");
            }

            string str = "";
            if (seconds >= 3600)
            {
                // style hour
                int h = seconds / 3600;
                str = h + " " + (h > 1 ? "HOURS" : "HOUR");

                if (seconds % 3600 == 0)
                {
                    // ko co du
                    return str;
                }
                else
                {
                    str += " ";
                    seconds = seconds % 3600;

                    if (seconds < 60)
                    {
                        str += "0 MINUTE " ;
                    }
                }
            }
            
            if(seconds >= 60)
            {
                int m = seconds / 60;
                str += m + " " + (m > 1 ? "MINUTES" : "MINUTE");

                // style minute
                if (seconds % 60 == 0)
                {
                    // ko co du
                    return str;
                }
                else
                {
                    str += " ";
                    seconds = seconds % 60;
                }
            }

            str += seconds + " " + (seconds > 1 ? "SECONDS" : "SECOND");
            return str;
        }

        #endregion

        #region SUPPORT METHOD
        public static void Shuffle(List<int> ids)
        {
            List<int> ints = new List<int>();

            for (int i = ids.Count - 1; i >= 0; i--)
            {
                int index = UnityEngine.Random.Range(0, ids.Count);
                ints.Add(ids[index]);

                ids.RemoveAt(index);
            }
            ids.AddRange(ints);
        }
        public static int GetNumberFromString(string str)
        {
            char[] arr = str.ToArray();
            var result = "";
            foreach(var ch in arr)
            {
                if(ch>='0' && ch <= '9')
                {
                    result += ch;
                }
            }
            if (result != "")
                return int.Parse(result);
            else return 1;
        }
        
        public static Vector2 GetSize(GameObject gContent, float xMin, float yMin, Vector2 padding)
        {
            Vector3 vector3Min = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 vector3Max = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3[] minMax = new Vector3[] {
                vector3Max,
                vector3Min
            };
            
            Transform t = gContent.transform;
            GetRendererBoundsInChildren(t.worldToLocalMatrix, minMax, t);
            if (minMax[0] != vector3Max && minMax[1] != vector3Min)
            {
                minMax[0] = Vector3.Min(minMax[0], Vector3.zero);
                minMax[1] = Vector3.Max(minMax[1], Vector3.zero);

                Vector2 v2 = new Vector2((minMax[1].x - minMax[0].x) + padding.x, (minMax[1].y - minMax[0].y) + padding.y);
                if (v2.x < xMin)
                    v2.x = xMin;
                if (v2.y < yMin)
                    v2.y = yMin;

                Debug.Log(v2);
                return v2;
            }
            else
            {
                Debug.Log(new Vector2(xMin, yMin));
                return new Vector2(xMin, yMin);
            }
        }

        private static readonly Vector3[] boxExtents = new Vector3[]
        {
            new Vector3(-1, -1, -1), new Vector3( 1, -1, -1), new Vector3(-1,  1, -1), new Vector3( 1,  1, -1), new Vector3(-1, -1,  1), new Vector3( 1, -1,  1), new Vector3(-1,  1,  1), new Vector3( 1,  1,  1)
        };

        private static void GetRendererBoundsInChildren(Matrix4x4 rootWorldToLocal, Vector3[] minMax, Transform t)
        {
            MeshFilter mf = t.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                Bounds b = mf.sharedMesh.bounds;
                Matrix4x4 relativeMatrix = rootWorldToLocal * t.localToWorldMatrix;
                for (int j = 0; j < 8; ++j)
                {
                    Vector3 localPoint = b.center + Vector3.Scale(b.extents, boxExtents[j]);
                    Vector3 pointRelativeToRoot = relativeMatrix.MultiplyPoint(localPoint);
                    minMax[0] = Vector3.Min(minMax[0], pointRelativeToRoot);
                    minMax[1] = Vector3.Max(minMax[1], pointRelativeToRoot);
                }
            }
            int childCount = t.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform child = t.GetChild(i);
#if UNITY_3_5
            if (t.gameObject.active) {
#else
                if (t.gameObject.activeSelf)
                {
#endif
                    GetRendererBoundsInChildren(rootWorldToLocal, minMax, child);
                }
            }
        }

        public static bool EqualsToggle(Toggle t1, Toggle t2)
        {
            return t1.Equals(t2);
        }

        public static void ChangeLayers(GameObject go, string name)
        {
            ChangeLayers(go, LayerMask.NameToLayer(name));
        }

        public static void ChangeLayers(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                ChangeLayers(child.gameObject, layer);
            }
        }

        public static string ToStringF(double str)
        {
            return Math.Truncate(str).ToString("F0");
        }

        public static Color ParseColor(string hexstring)
        {
            if (hexstring.StartsWith("#"))
            {
                hexstring = hexstring.Substring(1);
            }

            if (hexstring.StartsWith("0x"))
            {
                hexstring = hexstring.Substring(2);
            }

            if (hexstring.Length != 6 && hexstring.Length != 8)
            {
                throw new Exception(string.Format("{0} is not a valid color string.", hexstring));
            }

            byte r = byte.Parse(hexstring.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hexstring.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hexstring.Substring(4, 2), NumberStyles.HexNumber);

            byte a = 255;

            if (hexstring.Length == 8)
            {
                a = byte.Parse(hexstring.Substring(6, 2), NumberStyles.HexNumber); ;
            }

            return new Color32(r, g, b, a);
        }

        public static string ConvertStringStar(string str, int idxStart, int count)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            if (str.Length < idxStart + count)
                return str;

            string s = str.Substring(idxStart, count);
            return str.Replace(s, "xxxx");
        }

        public static GameObject CreateGameObject(GameObject gPrefab, GameObject gContent, bool autoActive = true)
        {
            GameObject gObj = GameObject.Instantiate(gPrefab);

            gObj.transform.SetParent(gContent.transform);
            gObj.transform.localPosition = Vector3.zero;
            gObj.transform.localScale = Vector3.one;

            if(autoActive) gObj.SetActive(true);

            return gObj;
        }

        public static T CreateGameObject<T>(GameObject gPrefab, GameObject gContent, bool autoActive = true)
        {
            GameObject gObj = CreateGameObject(gPrefab, gContent, autoActive);
            return gObj.GetComponent<T>();
        }

        public static void SetParent(Transform tranChild, Transform tranParent)
        {
            tranChild.SetParent(tranParent);
            tranChild.localPosition = Vector3.zero;
            tranChild.localScale = Vector3.one;

            tranChild.gameObject.SetActive(true);
        }

        public static void LoadDropDownMenu(List<string> obj, Dropdown dMenu, int idxDefault, bool isBottomToTop = false)
        {
            dMenu.ClearOptions();

            if (isBottomToTop)
            {
                for (int i = obj.Count - 1; i >= 0; i--)
                {
                    dMenu.options.Add(new Dropdown.OptionData(obj[i]));
                }
            }
            else
            {
                for (int i = 0; i < obj.Count; i++)
                {
                    dMenu.options.Add(new Dropdown.OptionData(obj[i]));
                }
            }

            dMenu.value = idxDefault;
            dMenu.Select();
            dMenu.RefreshShownValue();
            //        dMenu.captionText.text = dMenu.options[idxDefault].text;
        }

        public static void LoadDropDownMenu(List<string> obj, TMPro.TMP_Dropdown dMenu, int idxDefault, bool isBottomToTop = false)
        {
            dMenu.ClearOptions();

            if (isBottomToTop)
            {
                for (int i = obj.Count - 1; i >= 0; i--)
                {
                    dMenu.options.Add(new TMPro.TMP_Dropdown.OptionData(obj[i]));
                }
            }
            else
            {
                for (int i = 0; i < obj.Count; i++)
                {
                    dMenu.options.Add(new TMPro.TMP_Dropdown.OptionData(obj[i]));
                }
            }

            dMenu.value = idxDefault;
            dMenu.Select();
            dMenu.RefreshShownValue();
            //        dMenu.captionText.text = dMenu.options[idxDefault].text;
        }

        public static double GetSystemVersion()
        {
            string sVersion = SystemInfo.operatingSystem;
            double iVersion = 0;

            //        LMainGame layer = UILayerController.Instance.GetLayer<LMainGame>();
            //        layer.txtName.text = sVersion;
            //        layer.txtId.text = "";

#if UNITY_ANDROID
        int indexStart = sVersion.IndexOf("API-");
        try
        {
            sVersion = sVersion.Substring(indexStart + 4);
            indexStart = sVersion.IndexOf(" ");
            if(indexStart > 0)
                iVersion = int.Parse(sVersion.Substring(0, indexStart));
            else
                iVersion = int.Parse(sVersion);
        }
        catch (Exception)
        {
            iVersion = 0;   
        }
#elif UNITY_IOS
        int indexStart = sVersion.IndexOf("OS ");
        try
        {
			string[] v = sVersion.Substring(indexStart + 3).Split('.');
			iVersion = double.Parse(v[0]);
        }
        catch (Exception)
        {
            iVersion = 0;
        }
#endif

            return iVersion;
        }

        public static void OpenLink(string url)
        {
#if UNITY_WEBGL
        Application.ExternalCall("CallOpenLink", url);
#else
            Application.OpenURL(url);
#endif
        }
        
        public static string DeviceUniqueIdentifier()
        {
            var deviceId = "";

#if UNITY_EDITOR
            deviceId = SystemInfo.deviceUniqueIdentifier + "-editor"; //GenerateRandomString(8);//
#elif UNITY_WEBGL
            if (!PlayerPrefs.HasKey("UniqueIdentifier"))
                PlayerPrefs.SetString("UniqueIdentifier", Guid.NewGuid().ToString());
            deviceId = PlayerPrefs.GetString("UniqueIdentifier");
#else
            deviceId = SystemInfo.deviceUniqueIdentifier;
#endif
            return deviceId;
        }
        #endregion

        #region WEB SUPPORT
        //         public static void ShowWebView(string url, string title)
        //         {
        //             ShowWebView(url, title, false);
        //         }

        //         public static void ShowWebView(string url, string title, bool isRotate)
        //         {
        // #if UNITY_WEBGL || IOS_STORE || ANDROID_STORE
        //         OpenLink(url);
        // #elif UNITY_ANDROID || UNITY_IOS
        //         LWebView.ShowWebView(url, title, isRotate);
        // #elif UNITY_STANDALONE_WIN
        //             ((LWebViewPC)UILayerController.Instance.ShowLayer("LWebViewPC")).Init(url, title);
        // #endif
        //         }

        //         public static void ShowWebViewGameSlot(string url, string title, bool isFree)
        //         {
        // #if UNITY_WEBGL || IOS_STORE || ANDROID_STORE
        //         OpenLink(url);
        // #elif UNITY_ANDROID || UNITY_IOS
        //         LWebView.ShowGameSlot(url, title, isFree);
        // #elif UNITY_STANDALONE_WIN
        //             ((LWebViewPC)UILayerController.Instance.ShowLayer("LWebViewPC")).InitGameSlot(url, title, isFree);
        // #endif
        //         }

        //         #region Open Web
        //         public static IEnumerator WaitingToOpenSlot(string url, string nameGame, bool isFree)
        //         {
        // #if UNITY_ANDROID || UNITY_IOS
        //         yield return new WaitForEndOfFrame();
        //         Common.ShowWebViewGameSlot(url, nameGame, isFree);
        // #else
        //             UILayerController.Instance.ShowLoading();
        //             yield return new WaitUntil(() => !UILayerController.Instance.IsLayerExisted("LWebViewPC"));
        //             UILayerController.Instance.HideLoading();
        //             Common.ShowWebViewGameSlot(url, nameGame, isFree);
        // #endif
        //         }

        //         public static IEnumerator WaitingToOpenOther(string url, string nameGame, bool isRotate)
        //         {
        // #if UNITY_ANDROID || UNITY_IOS
        //         yield return new WaitForEndOfFrame();
        //         Common.ShowWebView(url, nameGame, isRotate);
        // #else
        //             UILayerController.Instance.ShowLoading();
        //             yield return new WaitUntil(() => !UILayerController.Instance.IsLayerExisted("LWebViewPC"));
        //             UILayerController.Instance.HideLoading();
        //             Common.ShowWebView(url, nameGame);
        // #endif
        //         }

        //         public static IEnumerator WaitingToOpenOther(string url, string nameGame)
        //         {
        // #if UNITY_ANDROID || UNITY_IOS
        //         yield return new WaitForEndOfFrame();
        //         Common.ShowWebView(url, nameGame);
        // #else
        //             UILayerController.Instance.ShowLoading();
        //             yield return new WaitUntil(() => !UILayerController.Instance.IsLayerExisted("LWebViewPC"));
        //             UILayerController.Instance.HideLoading();
        //             Common.ShowWebView(url, nameGame);
        // #endif
        //         }
        //         #endregion
// #if UNITY_EDITOR
//         public static void DoGetHostEntry(string hostname)
//         {
//             System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(hostname);

//             Debug.LogError($"GetHostEntry({hostname}) returns:");

//             foreach (System.Net.IPAddress address in host.AddressList)
//             {
//                 Debug.LogError($"    {address}");
//             }
//         }  
// #endif
        #endregion

        #region UNIWEBVIEW SUPPORT
        //         public static void UniWebViewSetAllowInlinePlay(bool allow)
        //         {
        // #if IOS_STORE || ANDROID_STORE
        // #elif UNITY_IOS || UNITY_ANDROID
        //         UniWebView.SetAllowInlinePlay(allow);
        // #endif
        //         }

        //         public static void UniWebViewSetAllowAutoPlay(bool allow)
        //         {
        // #if IOS_STORE || ANDROID_STORE
        // #elif UNITY_IOS || UNITY_ANDROID
        //         UniWebView.SetAllowAutoPlay(allow);
        // #endif
        //         }
        #endregion

        #region IMAGE SUPPORT
        public static bool IsAvatarDefault(string url)
        {
            string[] items = url.Split('/');
            return items[items.Length - 1].Contains("default");
        }

        public static Sprite ConvertTexture2DSprite(Texture2D texture, string sprName = "")
        {
            Sprite sprite = Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height),
                        Vector2.one / 2);
            if (!string.IsNullOrEmpty(sprName)) sprite.name = sprName;
            return sprite;
        }

        public static string ConvertTexture2DBase64(Texture2D texture, bool isJPG)
        {
            if(isJPG) return System.Convert.ToBase64String(texture.EncodeToJPG());
            return System.Convert.ToBase64String(texture.EncodeToPNG());
        }

        public static Texture2D ResizeTexture2D(Texture2D texture, int width, int height)
        {
            RenderTexture rt = new RenderTexture(width, height, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture, rt);
            Texture2D result = new Texture2D(width, height);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();
            return result;
        }

        public static IEnumerator LoadImageFromURL(Image image, string url, Action action = null)
        {
            VKDebug.LogWarning("Link image: " + url);
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();
            try
            {

                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                if (texture != null)
                {
                    Sprite sprite = Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height),
                        Vector2.one / 2);

                    image.sprite = sprite;
                    image.color = new Color(1f, 1f, 1f, 1f);
                }
            }
            catch (Exception e)
            {
                VKDebug.Log("Download failed: " + e.Message);
            }

            if (action != null)
                action.Invoke();
        }

        public static IEnumerator LoadImageFromURL(Image img, string url, int idx, int id, Dictionary<int, Sprite[]> dicSprRank)
        {
            if (dicSprRank[id][idx] != null)
            {
                img.sprite = dicSprRank[id][idx];
                img.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                yield return new WaitForSeconds(0.4f);

                UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
                yield return www.SendWebRequest();

                try
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    if (texture != null)
                    {
                        Sprite sprite = Sprite.Create(texture,
                            new Rect(0, 0, texture.width, texture.height),
                            Vector2.one / 2);

                        img.sprite = sprite;
                        img.color = new Color(1f, 1f, 1f, 1f);

                        dicSprRank[id][idx] = sprite;
                    }
                }
                catch (Exception e)
                {
                    VKDebug.Log("Download failed: " + e.Message);
                }
            }
        }

        public static IEnumerator LoadImageFromURL(Image image, string url, float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return LoadImageFromURL(image, url);
        }

        public static IEnumerator DownloadImageFromURL(string url, Action<Texture2D> action)
        {
            VKDebug.LogWarning("Link image: " + url);
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();
            try
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Debug.Log("texture != null " + (texture != null));
                if (texture != null)
                {
// #if UNITY_WEBGL
//                     try
//                     {
//                         Texture2D tex = null;
//                         string filenamelower = url.ToLower();
//                         if(filenamelower.EndsWith(".jpg") || filenamelower.EndsWith(".jpeg"))
//                         {
//                             tex = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, true);
//                             tex.SetPixels(texture.GetPixels());
//                             tex.filterMode = FilterMode.Bilinear;
//                             tex.Apply();
//                         }
//                         else
//                         {
//                             tex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, true);
//                             tex.SetPixels(texture.GetPixels());
//                             tex.filterMode = FilterMode.Bilinear;
//                             tex.Apply();
//                         }
//                         if (action != null) action.Invoke(tex);
//                     }
//                     catch
//                     {
//                         if (action != null) action.Invoke(texture);
//                     }
// #else
                    if (action != null) action.Invoke(texture);
// #endif
                }
                else
                {
                    if (action != null) action.Invoke(null);
                }
            }
            catch (Exception e)
            {
                VKDebug.Log("Download failed: " + e.Message);
                if (action != null) action.Invoke(null);
            }
        }

        public static IEnumerator LoadImageFromBase64(Image image, string base64, bool setAlpha = false, float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            Texture2D newPhoto = new Texture2D(1, 1);
            newPhoto.LoadImage(Convert.FromBase64String(base64));
            newPhoto.Apply();

            Sprite sprite = Sprite.Create(newPhoto,
                new Rect(0, 0, newPhoto.width, newPhoto.height),
                Vector2.one / 2);

            image.sprite = sprite;

            if (setAlpha) image.color = Color.white;
        }
        #endregion
    
#if VK_TMP_ACTIVE
        #region TEXTMESHPRO
        public static void TMPSetFace(TMPro.TMP_Text text, VKTMPFaceSetting faceSetting)
        {
            Material materialInstance = text.fontSharedMaterial;

            materialInstance.SetColor(ShaderUtilities.ID_FaceColor, faceSetting.color);
            materialInstance.SetFloat(ShaderUtilities.ID_FaceDilate, faceSetting.dilate);
        }

        public static void TMPSetOutline(TMPro.TMP_Text text, VKTMPOutlineSetting outlineSetting)
        {
            Material materialInstance = text.fontSharedMaterial;

            if (outlineSetting.outlineActive)
            {
                materialInstance.EnableKeyword(ShaderUtilities.Keyword_Outline);
                materialInstance.SetColor(ShaderUtilities.ID_OutlineColor, outlineSetting.color); //"_OutlineColor"
                materialInstance.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineSetting.thickness); //"_OutlineWidth"
            }
            else
            {
                materialInstance.DisableKeyword(ShaderUtilities.Keyword_Outline);
            }
        }

        public static void TMPSetUnderlay(TMPro.TMP_Text text, VKTMPUnderlaySetting underlaySetting)
        {
            Material materialInstance = text.fontSharedMaterial;

            if (underlaySetting.underlayActive)
            {
                materialInstance.EnableKeyword(ShaderUtilities.Keyword_Underlay);

                materialInstance.SetColor(ShaderUtilities.ID_UnderlayColor, underlaySetting.color); //"_UnderlayColor"
                materialInstance.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, underlaySetting.offsetX); //"_UnderlayOffsetX"
                materialInstance.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, underlaySetting.offsetY); //"_UnderlayOffsetY"
                materialInstance.SetFloat(ShaderUtilities.ID_UnderlayDilate, underlaySetting.dilate); //"_UnderlayDilate"
                materialInstance.SetFloat(ShaderUtilities.ID_UnderlaySoftness, underlaySetting.softness); //"_UnderlaySoftness"
            }
            else
            {
                materialInstance.DisableKeyword(ShaderUtilities.Keyword_Underlay);
            }
        }
        #endregion
#endif
    }
}