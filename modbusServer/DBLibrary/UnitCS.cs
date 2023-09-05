using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace lunar
{
    public static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
        public static byte[] FromHex2ByteArray(this string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }
        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }

    public class cs
    {
        public static string _APP = "";
        public const string _KEY_DM = "0DB6D824F30555BDADE6A576CD1BDAF8";
        public static string _URLHOME = "http://202.30.46.174/hb/attach/files/";


        public static string saltkey = "0DB6D824F30555BDADE6A576CD1BDAF8";

        [DllImport("User32", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern void SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_SHOWNORMAL = 1;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static String _KEY_DUPE = "Cannot insert duplicate key in object";

        public static Color COLOR_TITLE_BACK = System.Drawing.ColorTranslator.FromHtml("#474747");
        public static Color COLOR_WALL_BACK = System.Drawing.ColorTranslator.FromHtml("#636363");

        public static Color COLOR_ON_BACK = Color.MediumSeaGreen;
        public static Color COLOR_OFF_BACK = Color.Gray;

        public static Color COLOR_TAB_NORMAL_BACK = System.Drawing.ColorTranslator.FromHtml("#1a2127");
        public static Color COLOR_TAB_PRESSED_BACK = System.Drawing.ColorTranslator.FromHtml("#5a7083");

        public static Boolean _ISDEBUG = false;

        public static Boolean _WO_DB = false; // without db

        public static UInt64 LoginIdx = 0;

        public static Boolean IsNeedUserSync = false; // 로그인 이후 사용자 권한(deny)를 row한 형태의 테이블로 동기화

        public static Boolean empdev = false;
        public static string empid = "admin";
        public static string empname = "관리자";
        public static string empno = "";
        public static string emppermission = "";
        public static string empkind = "관리자";

        public static DateTime emplogintime = DateTime.Now;

        public static string empcpno = "";
        public static string empcpname = "";

        public static string menuKey = "";

        public static DataRow empdr = null;


        public static Boolean isLogShow = false;

        // design

        public static Color ColorTabButtonOnBack = ColorTranslator.FromHtml("#95999b");// Color.FromArgb(149, 153, 155);

        //public Color ColorTabButtonOverBack = Color.FromArgb(149, 153, 155);

        public static Color ColorTabButtonDownBack = ColorTranslator.FromHtml("#95999b");// Color.FromArgb(189, 193, 195);

        public static Color ColorTabButtonOffBack = ColorTranslator.FromHtml("#c4c6c9");// Color.FromArgb(196, 198, 201);

        public static Color ColorTabButtonBorder = Color.FromArgb(187, 191, 193);

        public static Color ColorPanelBorder = ColorTranslator.FromHtml("#dadada"); //Color.FromArgb(207, 207, 212);

        public static Color ColorLabelFont = Color.FromArgb(149, 153, 155);

        public static Color ColorButtonBorder = ColorTranslator.FromHtml("#bbbfc1");  //Color.FromArgb(149, 153, 155);

        public static Color ColorGraphCalcData = ColorTranslator.FromHtml("#494b4c");  //Color.FromArgb(149, 153, 155);

        public static Color ColorGridCellFont = ColorTranslator.FromHtml("#494b4c");  //Color.FromArgb(149, 153, 155);

        public static PrivateFontCollection privateFonts;

        public static Font fontMd10; // = new Font(privateFonts.Families[0], 10, GraphicsUnit.Pixel);
        public static Font fontMd12; // = new Font(privateFonts.Families[0], 12, GraphicsUnit.Pixel);
        public static Font fontMd14; // = new Font(privateFonts.Families[0], 14, GraphicsUnit.Pixel);
        public static Font fontMd16; // = new Font(privateFonts.Families[0], 16, GraphicsUnit.Pixel);
        public static Font fontMdCn20; // = new Font(privateFonts.Families[1], 20, GraphicsUnit.Pixel);
        public static Font fontMdCn22; // = new Font(privateFonts.Families[1], 20, GraphicsUnit.Pixel);
        public static Font fontMdCn40; // = new Font(privateFonts.Families[1], 20, GraphicsUnit.Pixel);
        public static Font fontMdCn14; // = new Font(privateFonts.Families[1], 14, GraphicsUnit.Pixel);
        public static Font fontMdCn62; // = new Font(privateFonts.Families[1], 62, GraphicsUnit.Pixel);

        public static List<string> lPacket = new List<string>();
        public static string keyBuf = "";
        public static Boolean isBarcodeDetected = false;

        public static event EventHandler KeyboardPacketNew;

        public static Boolean isDebug()
        {
            Boolean b = false;
            try
            {
                string[] args = Environment.GetCommandLineArgs();

                foreach (string arg in args)
                {
                    if (arg.Contains("-debug"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return b;
        }

        public static Boolean IsDev
        {
            get
            {
                if (cs.LocalParamGet("isdev") == "1")
                    return true;

                return empdev;
            }
        }
        public static string fileTemp(string fileOrg)
        {
            string fileDupe = Application.StartupPath + "\\temp\\temp_" + DateTime.Now.ToString("yyMMdd_HHmmss") + "_" + Path.GetFileName(fileOrg);
            try
            {
                File.Copy(fileOrg, fileDupe);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }

            return fileDupe;
        }


        /// <summary>
        /// Encrypt
        /// </summary>

        public static string Decrypt(string textToDecrypt, string key)
        {
            string rr = "";

            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 128;
                rijndaelCipher.BlockSize = 128;
                byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
                byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
                byte[] keyBytes = new byte[16];
                int len = pwdBytes.Length;
                if (len > keyBytes.Length)
                    len = keyBytes.Length;
                Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                rijndaelCipher.IV = keyBytes;
                byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                rr = Encoding.UTF8.GetString(plainText);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return rr;
        }

        public static string Encrypt(string textToEncrypt, string key)
        {

            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;

            rijndaelCipher.Padding = PaddingMode.PKCS7;



            rijndaelCipher.KeySize = 128;

            rijndaelCipher.BlockSize = 128;

            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;

            if (len > keyBytes.Length)

            {

                len = keyBytes.Length;

            }

            Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;

            rijndaelCipher.IV = keyBytes;

            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

            byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);

            return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));

        }

        private static byte[] mkey(string skey)
        {

            byte[] key = Encoding.UTF8.GetBytes(skey);
            byte[] k = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < key.Length; i++)
            {
                k[i % 16] = (byte)(k[i % 16] ^ key[i]);
            }

            return k;
        }
        /// <summary>
        /// Encrypt
        /// </summary>


        public static String AES_encrypt(String Input, string key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(Input);
                    cs.Write(xXml, 0, xXml.Length);
                    cs.FlushFinalBlock();
                }

                xBuff = ms.ToArray();
            }

            String output = Convert.ToBase64String(xBuff);
            return output;
        }

        public static String AES_decrypt(String Input, string key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var decrypt = aes.CreateDecryptor();
            byte[] encryptedStr = Convert.FromBase64String(Input);

            byte[] xBuff = new byte[encryptedStr.Length];
            using (var ms = new MemoryStream(encryptedStr))
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Read))
                {
                    cs.Read(xBuff, 0, xBuff.Length);
                }
            }

            String output = Encoding.UTF8.GetString(xBuff);
            return output;
        }

        public static string Decrypt(string Input)
        {
            string key = saltkey;

            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = mkey(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var decrypt = aes.CreateDecryptor();
            byte[] encryptedStr = Input.FromHex2ByteArray();

            string Plain_Text;

            using (var ms = new MemoryStream(encryptedStr))
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        Plain_Text = reader.ReadToEnd();
                    }
                }
            }

            return Plain_Text;
        }

        public static string Encrypt(string Input)
        {
            string key = saltkey;

            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = mkey(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(Input);
                    cs.Write(xXml, 0, xXml.Length);
                    cs.FlushFinalBlock();
                }

                xBuff = ms.ToArray();
            }

            return xBuff.ToHexString();
        }



        public static string Client_IP
        {
            get
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                string ClientIP = string.Empty;
                for (int i = 0; i < host.AddressList.Length; i++)
                {
                    if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        ClientIP = host.AddressList[i].ToString();
                    }
                }
                return ClientIP;
            }
        }

        public static String MyIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static int iIsServer = -1;
        public static int iIsMap = -1;
        public static string UserDCPFilter = "";
        public static string UserPermission = "";

        public static String UserAuth = ""; // 주기적으로 user테이블의 phone필드 저장

        public static Dictionary<String, String> DicUser = new Dictionary<string, string>();

        public static Dictionary<String, String> DicUserAction = new Dictionary<string, string>();

        public static Thread threadLog = null;

        public static Boolean IsApplicationTerminated = false;

        public static Dictionary<String, StringBuilder> DicLog = new Dictionary<string, StringBuilder>();

        public static void LogInit()
        {
            privateFonts = new PrivateFontCollection();

            //폰트명이 아닌 폰트의 파일명을 적음
            try
            {
                privateFonts.AddFontFile("fonts\\AppleSDGothicNeoM.ttf");

                fontMd10 = new Font(privateFonts.Families[0], 12, GraphicsUnit.Point);
                fontMd12 = new Font(privateFonts.Families[0], 12, GraphicsUnit.Pixel);
                fontMd14 = new Font(privateFonts.Families[0], 14, GraphicsUnit.Pixel);
                fontMd16 = new Font(privateFonts.Families[0], 16, GraphicsUnit.Pixel);
            }
            catch (Exception E)
            {

            }

            try
            {
                string sDirPath;

                sDirPath = Application.StartupPath + "\\temp";
                DirectoryInfo di = new DirectoryInfo(sDirPath);
                if (di.Exists == false)
                {
                    di.Create();
                }
                sDirPath = Application.StartupPath + "\\log";
                di = new DirectoryInfo(sDirPath);
                if (di.Exists == false)
                {
                    di.Create();
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }

            if (cs.LocalParamGet("isdev") == "1")
            {
                if (cs.LocalParamGet("dbname") == "hb")
                {
                    _URLHOME = "http://10.8.0.10/hb/attach/files/";
                }
                else
                {
                    _URLHOME = "http://dair.co.kr/dair/files/";
                }
            }
            else
            {
                _URLHOME = "http://202.30.46.174/hb/attach/files/";
            }

            if (cs.LocalParamGet("dbname") == "dair_srt")
            {
                _URLHOME = "http://dair.co.kr/dair/srt/files/";
            }
            else if (cs.LocalParamGet("dbname") == "dair_koreaplate")
            {
                _URLHOME = "http://dair.co.kr/dair/koreaplate/files/";
            }
            else if (cs.LocalParamGet("dbname") == "dms_ksf")
            {
                _URLHOME = "http://dair.co.kr/dair/dms_ksf/files/";
            }
            else
            {
                _URLHOME = "http://dair.co.kr/dair/"+cs.LocalParamGet("dbname")+"/files/";                
            }
            //Application.ApplicationExit += new EventHandler(OnApplicationExit);

            threadLog = new Thread(() => DoThreadLog());
            threadLog.Start();
            if (IsDev)
            {
            }
        }
        public static int strToInt(string v, int ADefault = 0)
        {
            int d = 0;
            if (Int32.TryParse(v, out d) == false)
                d = ADefault;
            return d;
        }

        public static double strToDouble(string v)
        {
            double d = 0;
            double.TryParse(v, out d);
            return d;
        }
        public static string strToDoubleString(string v)
        {
            double d = 0;
            double.TryParse(v, out d);

            return d.ToString();
        }

        public static string getDateStr(DataRow dr, string field)
        {
            string r = "";
            if (dr.Table.Columns.Contains(field))
            {
                r = strTodt(dr[field].ToString()).ToString("yyyy-MM-dd");
            }
            return r;
        }
        public static string getDateTimeStr(DataRow dr, string field)
        {
            string r = "";
            if (dr.Table.Columns.Contains(field))
            {
                r = strTodt(dr[field].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return r;
        }
        public static string dtTostr(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static DateTime strTodt(string sdt)
        {
            DateTime dt;
            if (DateTime.TryParse(sdt, out dt))
            {
                return dt;
            }
            else
            {
                return new DateTime(1900, 1, 1);
            }
        }


        private void OnApplicationExit(object sender, EventArgs e)
        {
            IsApplicationTerminated = true;
        }

        public static Boolean isVaildDate(string sdt, out DateTime dt)
        {
            DateTime dtt = DateTime.MinValue;
            Boolean r = false;
            try
            {
                if (DateTime.TryParse(sdt, out dtt))
                {
                    if (dtt.Year > 1900)
                        r = true;
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            dt = dtt;
            return r;
        }

        public static void DoThreadLog()
        {
            while (IsApplicationTerminated == false)
            {
                Thread.Sleep(500);

                lock (DicLog)
                {
                    foreach (KeyValuePair<String, StringBuilder> a in DicLog)
                    {
                        String sFile = a.Key;
                        StringBuilder sb = a.Value;

                        try
                        {
                            if (sb.Length > 0)
                            {
                                File.AppendAllText(sFile, sb.ToString());
                                lock (sb)
                                {
                                    sb.Clear();
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        public static void UserActionInit()
        {
            DicUserAction.Add("overview", "시스템요약");
            DicUserAction.Add("eqoverview", "설비요약");
            DicUserAction.Add("matrix", "매트릭스");
            DicUserAction.Add("graph", "그래프");
            DicUserAction.Add("table", "테이블");
            DicUserAction.Add("map", "맵");
            DicUserAction.Add("setting", "설정");
            DicUserAction.Add("event", "이벤트");
            DicUserAction.Add("eventsystem", "이벤트(시스템)");
            DicUserAction.Add("alarmhistory", "알람목록");
            DicUserAction.Add("raw", "파티클데이터");
            DicUserAction.Add("report", "레포트");
            DicUserAction.Add("mcshistory", "MCS목록");
            DicUserAction.Add("tehistory", "동보알람");
            DicUserAction.Add("vehicle", "VEHICLE");
        }

        public static String GetPathSQL()
        {
            return Application.StartupPath + "\\sql";
        }

        public static String GetPathResource()
        {
            return Application.StartupPath + "\\resource";
        }

        public static String GetPathForm()
        {
            return Application.StartupPath + "\\form";
        }

        public static void RunExe(String AFile)
        {
            try
            {
                Process.Start(AFile);
            }
            catch (Exception E)
            {
                logError(E);
            }
        }
        /// <summary>
        /// 외부 프로그램 실행하기
        /// </summary>
        /// <param name="program">프로그램명</param>
        /// <param name="param">파라메터</param>
        public static void RunProcess(string program, string param)
        {
            ProcessStartInfo info = new ProcessStartInfo(program, param);
            Process.Start(info);
        }



        /// <summary>
        /// 엑셀 프로그램 실행하기
        /// </summary>
        /// <param name="path">파일명</param>
        public static void RunExcel(string path)
        {
            RunProcess("excel.exe", path);
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static String UserIDToUser(String AID)
        {
            String sName = "";

            if (DicUser.TryGetValue(AID, out sName) == false)
            {
                sName = "";
            }

            return sName;
        }

        public static String ValidateEMail(String AFrom, List<String> AError)
        {
            List<String> l = null;
            List<String> l2 = new List<string>();

            AFrom = AFrom.Replace("\r", "");
            l = AFrom.Split('\n').ToList();

            foreach (String s in l)
            {
                if ((s != "") && (s != " "))
                {
                    if (IsValidEmail(s))
                    {
                        if (l2.Contains(s) == false)
                            l2.Add(s);
                    }
                    else
                    {
                        AError.Add(s);
                    }
                }
            }

            return String.Join("\r\n", l2);
        }

        public static bool IsValidEmail(string email)
        {
            bool valid = Regex.IsMatch(email, "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

            if (email == "")
                valid = true;

            return valid;
        }

        public static String StringOrNull(Object AValue, String ADefault = "null")
        {
            if (AValue == null)
                return ADefault;
            else if (AValue.ToString() == "")
                return ADefault;
            else
                return AValue.ToString();
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static DateTime DateTimeSplitAfter10Minute(DateTime ADT)
        {
            int iMin = ADT.Minute;          // 04 11:58:24
            int iMinUnder10 = iMin % 10;    // 58 -> 8

            if (iMin >= 50)
            {
                // 무조건 시간 변동이 생기므로 Add함수 이용
                // 04 11:58:24 -> 05 00:00:24    
                ADT = ADT.AddMinutes(10 - iMinUnder10); // 2 = 10 - 8
                TimeSpan tsBodyFrom = new TimeSpan(ADT.Hour, ADT.Minute, 0);
                //  05 00:00:24 -> 05 00:00:00     
                ADT = ADT.Date + tsBodyFrom;
            }
            else
            {
                // 04 11:58:24 -> 05 00:00:24  
                TimeSpan tsBodyFrom = new TimeSpan(ADT.Hour, iMin + (10 - iMinUnder10), 0);
                //  05 00:00:24 -> 05 00:00:00    
                ADT = ADT.Date + tsBodyFrom;
            }

            return ADT;
        }

        public static DateTime DateTimeSplitBefore10Minute(DateTime ADT)
        {
            int iMin = ADT.Minute;          // 04 11:58:24
            int iMinUnder10 = iMin % 10;    // 58 -> 8

            TimeSpan tsBodyFrom = new TimeSpan(ADT.Hour, ADT.Minute - iMinUnder10, 0);
            ADT = ADT.Date + tsBodyFrom;

            return ADT;
        }

        public static DateTime DateTimeSplitAfter1Hour(DateTime ADT)
        {
            ADT = ADT.AddHours(1);

            TimeSpan tsBodyFrom = new TimeSpan(ADT.Hour, 0, 0);
            ADT = ADT.Date + tsBodyFrom;

            return ADT;
        }

        public static DateTime DateTimeSplitBefore1Hour(DateTime ADT)
        {
            TimeSpan tsBodyFrom = new TimeSpan(ADT.Hour, 0, 0);
            ADT = ADT.Date + tsBodyFrom;

            return ADT;
        }

        public static Boolean IsRAWSTDEV()
        {
            String sf = Application.StartupPath + "\\_CALCSTDEV.ini";
            return !File.Exists(sf);
        }

        public static String SetParam(String ABody, String AParam, String AValue, Char ADelimiter = '|')
        {
            if (ABody == null)
                ABody = "";

            String[] sa = ABody.Split(ADelimiter);

            int i = 0;
            while (i < sa.Length)
            {
                if (sa[i] == AParam)
                {
                    if (i + 1 < sa.Length)
                    {
                        sa[i + 1] = AValue;
                        return String.Join("|", sa);
                    }
                }
                i = i + 1;
            }

            //if (ABody.Length == 0)
            //    ABody = AParam + ADelimiter + AValue;
            //else
            ABody = ABody + ADelimiter + AParam + ADelimiter + AValue;

            return ABody;
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            int ir = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            if (ir == 52)
                ir = 0;
            return ir + 1;
        }

        public static String GetParam(String ABody, String AParam, String ADefault = "", Char ADelimiter = '|')
        {
            if (ABody == null)
                return ADefault;
            String[] sa = ABody.Split(ADelimiter);

            int i = 0;
            while (i < sa.Length)
            {
                if (sa[i] == AParam)
                {
                    if (i + 1 < sa.Length)
                    {
                        return sa[i + 1];
                    }
                }
                i = i + 1;
            }
            return ADefault;
        }

        public static int GetParamInt(String ABody, String AParam, int ADefault = 0, Char ADelimiter = '|')
        {
            if (ABody == null)
                return ADefault;
            String[] sa = ABody.Split(ADelimiter);

            int i = 0;
            while (i < sa.Length)
            {
                if (sa[i] == AParam)
                {
                    if (i + 1 < sa.Length)
                    {
                        string s = sa[i + 1];
                        int iResult = 0;

                        if (Int32.TryParse(s, out iResult))
                        {
                            return iResult;
                        }
                        else
                        {
                            return ADefault;
                        }
                    }
                }
                i = i + 1;
            }
            return ADefault;
        }

        public static Boolean IsPermission(String AType)
        {
            Boolean b = false;

            if (IsSetup1())
                return true;

            // 2차 = 사용자권한
            if (GetParam(UserPermission, AType) == "1")
            {
                b = true;
            }
            else if (GetParam(UserPermission, AType) == "0")
            {
                // 사용자권한으로 override한다. 
                b = false;
            }
            else
            {
                // 사용자권한이 비어 있으면 상위 권한 사용
                String sFile = Application.StartupPath + "\\setting_permission.csv";
                String sValue = GetValueFromCSV(sFile, "이름", AType, "기본값");

                if (sValue == "1")
                    b = true;
            }
            return b;
        }

        public static String GetValueFromCSV(String AFile, String AFindCol, String AFindValue, String AResultCol, String ADefault = "")
        {
            String[] saRows = File.ReadAllLines(AFile);

            if (saRows.Count() > 0)
            {
                String[] saHeader = saRows[0].Split(',');

                int iFindCol = -1;
                int iResultCol = -1;
                int i = 0;
                foreach (string sHeader in saHeader)
                {
                    if (sHeader == AFindCol)
                    {
                        iFindCol = i;
                    }
                    if (sHeader == AResultCol)
                    {
                        iResultCol = i;
                    }

                    if ((iResultCol >= 0) && (iFindCol >= 0))
                        break;
                    i++;
                }

                if ((iResultCol >= 0) && (iFindCol >= 0))
                {
                    foreach (string sCols in saRows)
                    {
                        String[] saCols = sCols.Split(',');

                        if (iFindCol < saCols.Count())
                        {
                            if (saCols[iFindCol] == AFindValue)
                            {
                                if (iResultCol < saCols.Count())
                                    return saCols[iResultCol];
                                else
                                    return ADefault;
                            }
                        }
                    }
                }
            }
            return ADefault;
        }

        public static Boolean IsLogin()
        {
            if (empid == "")
                return false;
            else
                return true;
        }

        public static Boolean IsSetup1()
        {
            if (IsLogin())
            {
                //int i = Program.FormMainRef.FormDBManager.GetValueToInt("[user]", "id", LoginIdx.ToString(), "phone", 0);
                int i = 3; Int32.TryParse(UserAuth, out i);
                if (i == 1)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static Boolean IsAdmin()
        {
            return false;
        }
        public static Boolean IsAdmin2()
        {
            if (IsLogin())
            {
                //int i = Program.FormMainRef.FormDBManager.GetValueToInt("[user]", "id", LoginIdx.ToString(), "phone", 0);
                int i = 3; Int32.TryParse(UserAuth, out i);
                if (i <= 2)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static Boolean IsPowerUser3()
        {
            if (IsLogin())
            {
                //int i = Program.FormMainRef.FormDBManager.GetValueToInt("[user]", "id", LoginIdx.ToString(), "phone", 0);
                int i = 3; Int32.TryParse(UserAuth, out i);
                if (i <= 3)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        static Dictionary<String, String> DicParamCache = new Dictionary<string, string>();

        public static String PlatformParamGet(String AID, String ADefault = "", Boolean AWithCache = false)
        {
            String AValue;
            if (AWithCache)
            {
                if (DicParamCache.TryGetValue(AID, out AValue) == false)
                {
                    String sf = Application.StartupPath + "\\platform.ini";
                    StringBuilder temp = new StringBuilder(65535);

                    int ret = GetPrivateProfileString("setting", AID, ADefault, temp, 65535, sf);
                    AValue = temp.ToString();

                    DicParamCache.Add(AID, AValue);
                }
            }
            else
            {
                String sf = Application.StartupPath + "\\platform.ini";
                StringBuilder temp = new StringBuilder(65535);

                int ret = GetPrivateProfileString("setting", AID, ADefault, temp, 65535, sf);
                AValue = temp.ToString();
            }

            return AValue;
        }

        public static String LocalParamGet(String AID, String ADefault = "", Boolean AWithCache = false, string settingname = "setting.ini")
        {
            String AValue;
            if (AWithCache)
            {
                if (DicParamCache.TryGetValue(AID, out AValue) == false)
                {
                    String sf = Application.StartupPath + "\\" + settingname;
                    StringBuilder temp = new StringBuilder(65535);

                    int ret = GetPrivateProfileString("setting", AID, ADefault, temp, 65535, sf);
                    AValue = temp.ToString();

                    DicParamCache.Add(AID, AValue);
                }
            }
            else
            {
                String sf = Application.StartupPath + "\\" + settingname;
                StringBuilder temp = new StringBuilder(65535);

                int ret = GetPrivateProfileString("setting", AID, ADefault, temp, 65535, sf);
                AValue = temp.ToString();
            }

            return AValue;
        }

        public static int LocalParamGetToInt(String AID, int ADefault = 0, string settingname = "setting.ini")
        {
            String s = "";
            int ir = ADefault;

            String sf = Application.StartupPath + "\\" + settingname;
            StringBuilder temp = new StringBuilder(65535);
            int ret = GetPrivateProfileString("setting", AID, ADefault.ToString(), temp, 65535, sf);

            try
            {
                ir = Convert.ToInt32(temp.ToString());
            }
            catch
            {

            }

            return ir;
        }

        public static decimal LocalParamGetToDecimal(String AID, decimal ADefault = 0, string settingname = "setting.ini")
        {
            String s = "";
            decimal ir = ADefault;

            String sf = Application.StartupPath + "\\" + settingname;
            StringBuilder temp = new StringBuilder(65535);
            int ret = GetPrivateProfileString("setting", AID, ADefault.ToString(), temp, 65535, sf);

            try
            {
                ir = Convert.ToDecimal(temp.ToString());
            }
            catch
            {

            }

            return ir;
        }

        public static double LocalParamGetToDouble(String AID, double ADefault = 0, string settingname = "setting.ini")
        {
            String s = "";
            double ir = ADefault;

            String sf = Application.StartupPath + "\\" + settingname;
            StringBuilder temp = new StringBuilder(65535);
            int ret = GetPrivateProfileString("setting", AID, ADefault.ToString(), temp, 65535, sf);

            try
            {
                ir = Convert.ToDouble(temp.ToString());
            }
            catch
            {

            }

            return ir;
        }

        public static void LocalParamSet(String AID, String AValue, Boolean AInsert = false, string settingname = "setting.ini")
        {
            String sf = Application.StartupPath + "\\" + settingname;
            WritePrivateProfileString("setting", AID, AValue, sf);
        }

        public static String DBParamGet(String AID, String ADefault = "")
        {
            return "";
        }

        public static void DBParamSet(String AID, String AValue, Boolean AInsert = false)
        {
            //Program.FormMainRef.FormDBManager.SetValue("CS_Param", "cs_param", AID, "cs_param_value", AValue);

        }

        public static void ShowMsg(String AMsg)
        {
            MessageBox.Show(AMsg, "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static String DebugMsg = "";
        public static void Debug(String AMsg)
        {
            if (DebugMsg.Length > 65535)
            {
                DebugMsg = "";
            }

            if (_ISDEBUG == false)
                return;

            String m_exePath = Application.StartupPath;
            String sFile = m_exePath + "\\log\\logdebug@" + DateTime.Now.ToString("yyMMdd") + ".txt";
            Log(sFile, "DEBUG", AMsg);


            DebugMsg = DebugMsg + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + AMsg + "\r\n";
        }

        public static string getParamValueInBracket(string org, string from, string to)
        {
            string r = "";
            int i1 = org.IndexOf(from);
            if(i1>=0)
            {
                i1 += from.Length;
                int i2 = org.IndexOf(to, i1);
                if(i2 > i1)
                {
                    i2--;
                    r = org.Substring(i1, i2 - i1 + 1);
                }
            }
            return r;
        }

        public static void Log(String AFile, String ATag = "", String AMsg = "", Boolean AInfo = true, Boolean AShowTime = true)
        {
            try
            {
                if (ATag == "")
                    ATag = "LOG";

                ATag = String.Format("[{0,-10}] ", ATag);

                //AFile = Application.StartupPath + "\\log\\log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

                StringBuilder sb = null;
                if (DicLog.TryGetValue(AFile, out sb) == false)
                {
                    sb = new StringBuilder();

                    lock (DicLog)
                    {
                        DicLog.Add(AFile, sb);
                    }
                }

                if (AInfo)
                {
                    //if (AShowTime)
                    lock (sb)
                    {
                        //sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + ATag + AMsg + "\r\n");
                        sb.Append(ATag + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + AMsg + "\r\n");
                        if ((AShowTime == false) || (AMsg.Contains("\r")))
                            sb.Append("\r\n");
                    }
                    //else
                    //    sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + AMsg + "\r\n");
                }
                else
                {
                    lock (sb)
                    {
                        sb.Append(AMsg + "\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void logmsg(String AMsg, String ATag = "", Boolean AShowTime = true)
        {
            String m_exePath = Application.StartupPath;

            String sFile = m_exePath + "\\log\\log@" + DateTime.Now.ToString("yyMMdd") + ".txt";
            Log(sFile, "MESSAGE", AMsg, AShowTime);

        }

        public static void logSQL(String ASQL, String ATag = "[SQL     ]")
        {
            if (IsDev==false)
                return;
            String m_exePath = Application.StartupPath;

            String sFile = m_exePath + "\\log\\logsql@" + DateTime.Now.ToString("yyMMdd") + ".txt";
            Log(sFile, "SQL", "--------------------------------------------------");
            Log(sFile, "SQL", ASQL);

        }

        public static void LogDebug(String ASQL, String ATag = "[DEBUG   ]")
        {
            String m_exePath = Application.StartupPath;

            String sFile = m_exePath + "\\log\\logdebug@" + DateTime.Now.ToString("yyMMdd") + ".txt";
            Log(sFile, "DEBUG", ASQL);

            Debug(ASQL);

        }

        public static void logError(Exception AError, String ATag = "[ERROR   ]")
        {
            String m_exePath = Application.StartupPath;
            String sFile = "";
            try
            {
                sFile = m_exePath + "\\log\\logerror@" + DateTime.Now.ToString("yyMMdd") + ".txt";
                Log(sFile, "ERROR", AError.Message);
                Log(sFile, "", AError.StackTrace);
            }
            catch (Exception ex)
            {
            }

            if(isDebug())
            {
                if(isLogShow==false)
                {
                    isLogShow = true;

                    System.Diagnostics.Process.Start(sFile);
                }
            }
        }

        public static void LogErrorMsg(String AMsg, String ATag = "[ERROR   ]")
        {
            String m_exePath = Application.StartupPath;

            String sFile = m_exePath + "\\log\\logerror@" + DateTime.Now.ToString("yyMMdd") + ".txt";
            Log(sFile, "ERROR", AMsg);
        }
        public static void setBorder(Control ctl, Color col, int width, BorderStyle style)
        {
            //TEA.setBorder(comboY, TEA.ColorButtonBorder, 1, BorderStyle.FixedSingle);
            if (col == Color.Transparent)
            {
                Panel pan = ctl.Parent as Panel;
                if (pan == null) { throw new Exception("control not in border panel!"); }
                ctl.Location = new Point(pan.Left + width, pan.Top + width);
                ctl.Parent = pan.Parent;
                pan.Dispose();

            }
            else
            {
                Panel pan = new Panel();
                pan.BorderStyle = style;
                pan.Size = new Size(ctl.Width + width * 2, ctl.Height + width * 2);
                pan.Location = new Point(ctl.Left - width, ctl.Top - width);
                pan.BackColor = col;
                pan.Parent = ctl.Parent;
                ctl.Parent = pan;
                ctl.Location = new Point(width, width);
            }
        }

        public class HashSalt
        {
            public static Dictionary<string, int> dicLoginFail = new Dictionary<string, int>();

            public string Hash { get; set; }
            public string Salt { get; set; }

            public static HashSalt GenerateSaltedHash(int size, string password)
            {
                var saltBytes = new byte[size];
                var provider = new RNGCryptoServiceProvider();
                provider.GetNonZeroBytes(saltBytes);
                var salt = Convert.ToBase64String(saltBytes);

                var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
                var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

                HashSalt hashSalt = new HashSalt { Hash = hashPassword, Salt = salt };
                //hashSalt.Hash = AESEncrypt256(hashSalt.Hash, hashSalt.Salt);
                return hashSalt;
            }

            public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
            {
                var saltBytes = Convert.FromBase64String(storedSalt);
                var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000);
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
            }

            public static String AESEncrypt256(String Input, String key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Encoding.UTF8.GetBytes(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                String Output = Convert.ToBase64String(xBuff);
                return Output;
            }


            //AES_256 복호화
            public static String AESDecrypt256(String Input, String key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var decrypt = aes.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                String Output = Encoding.UTF8.GetString(xBuff);
                return Output;
            }
        }
    }

    public static class ControlHelper
    {
        #region Redraw Suspend/Resume
        [DllImport("user32.dll", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private const int WM_SETREDRAW = 0xB;

        public static void SuspendDrawing(this Control target)
        {
            SendMessage(target.Handle, WM_SETREDRAW, 0, 0);
        }

        public static void ResumeDrawing(this Control target) { ResumeDrawing(target, true); }
        public static void ResumeDrawing(this Control target, bool redraw)
        {
            SendMessage(target.Handle, WM_SETREDRAW, 1, 0);

            if (redraw)
            {
                target.Refresh();
            }
        }
        #endregion
    }

    public static class GenericCopier<T>
    {
        public static T DeepCopy(object objectToCopy)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, objectToCopy);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
    public class HSLColor
    {
        // Private data members below are on scale 0-1
        // They are scaled for use externally based on scale
        private double hue = 1.0;
        private double saturation = 1.0;
        private double luminosity = 1.0;

        private const double scale = 240.0;

        public double Hue
        {
            get { return hue * scale; }
            set { hue = CheckRange(value / scale); }
        }
        public double Saturation
        {
            get { return saturation * scale; }
            set { saturation = CheckRange(value / scale); }
        }
        public double Luminosity
        {
            get { return luminosity * scale; }
            set { luminosity = CheckRange(value / scale); }
        }

        private double CheckRange(double value)
        {
            if (value < 0.0)
                value = 0.0;
            else if (value > 1.0)
                value = 1.0;
            return value;
        }

        public override string ToString()
        {
            return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
        }

        public string ToRGBString()
        {
            Color color = (Color)this;
            return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
        }

        #region Casts to/from System.Drawing.Color
        public static implicit operator Color(HSLColor hslColor)
        {
            double r = 0, g = 0, b = 0;
            if (hslColor.luminosity != 0)
            {
                if (hslColor.saturation == 0)
                    r = g = b = hslColor.luminosity;
                else
                {
                    double temp2 = GetTemp2(hslColor);
                    double temp1 = 2.0 * hslColor.luminosity - temp2;

                    r = GetColorComponent(temp1, temp2, hslColor.hue + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, hslColor.hue);
                    b = GetColorComponent(temp1, temp2, hslColor.hue - 1.0 / 3.0);
                }
            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            temp3 = MoveIntoRange(temp3);
            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
            else
                return temp1;
        }
        private static double MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;
            return temp3;
        }
        private static double GetTemp2(HSLColor hslColor)
        {
            double temp2;
            if (hslColor.luminosity < 0.5)  //<=??
                temp2 = hslColor.luminosity * (1.0 + hslColor.saturation);
            else
                temp2 = hslColor.luminosity + hslColor.saturation - (hslColor.luminosity * hslColor.saturation);
            return temp2;
        }

        public static implicit operator HSLColor(Color color)
        {
            HSLColor hslColor = new HSLColor();
            hslColor.hue = color.GetHue() / 360.0; // we store hue as 0-1 as opposed to 0-360 
            hslColor.luminosity = color.GetBrightness();
            hslColor.saturation = color.GetSaturation();
            return hslColor;
        }
        #endregion

        public void SetRGB(int red, int green, int blue)
        {
            HSLColor hslColor = (HSLColor)Color.FromArgb(red, green, blue);
            this.hue = hslColor.hue;
            this.saturation = hslColor.saturation;
            this.luminosity = hslColor.luminosity;
        }

        public HSLColor() { }
        public HSLColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }
        public HSLColor(int red, int green, int blue)
        {
            SetRGB(red, green, blue);
        }
        public HSLColor(double hue, double saturation, double luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
        }

    }

    public class ColourGenerator
    {
        public Color GetColor(int index)
        {
            int i = index / colors.Length;
            double d = (5 - (double)i) / 5 * 255;

            Color c = colors[index % colors.Length];

            HSLColor hc = new HSLColor(c);

            hc.Saturation = d;

            return hc;
        }

        public void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        /// <summary>
        /// The list of hardcoded colors.
        /// </summary>
        /// <remarks>
        /// Can be extended manually by the developer or the class can be modified so that the property can be modified during runtime.
        /// </remarks>
        private static readonly Color[] colors =
        {
            Color.Orange,
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Purple,
            Color.DarkRed,
            Color.Violet,
            Color.NavajoWhite,
            Color.MediumSeaGreen,
            Color.HotPink
        };
    }

    public class PatternGenerator
    {
        public static string NextPattern(int index)
        {
            switch (index % 7)
            {
                case 0: return "{0}0000";
                case 1: return "00{0}00";
                case 2: return "0000{0}";
                case 3: return "{0}{0}00";
                case 4: return "{0}00{0}";
                case 5: return "00{0}{0}";
                case 6: return "{0}{0}{0}";
                default: throw new Exception("Math error");
            }
        }
    }

    public class IntensityGenerator
    {
        private IntensityValueWalker walker;
        private int current;

        public string NextIntensity(int index)
        {
            if (index == 0)
            {
                current = 255;
            }
            else if (index % 7 == 0)
            {
                if (walker == null)
                {
                    walker = new IntensityValueWalker();
                }
                else
                {
                    walker.MoveNext();
                }
                current = walker.Current.Value;
            }
            string currentText = current.ToString("X");
            if (currentText.Length == 1) currentText = "0" + currentText;
            return currentText;
        }
    }

    public class IntensityValue
    {

        private IntensityValue mChildA;
        private IntensityValue mChildB;

        public IntensityValue(IntensityValue parent, int value, int level)
        {
            if (level > 7)
            {
                level = 7;
            }
            //throw new Exception("There are no more colours left");
            Value = value;
            Parent = parent;
            Level = level;
        }

        public int Level { get; set; }
        public int Value { get; set; }
        public IntensityValue Parent { get; set; }

        public IntensityValue ChildA
        {
            get
            {
                return mChildA ?? (mChildA = new IntensityValue(this, this.Value - (1 << (7 - Level)), Level + 1));
            }
        }

        public IntensityValue ChildB
        {
            get
            {
                return mChildB ?? (mChildB = new IntensityValue(this, Value + (1 << (7 - Level)), Level + 1));
            }
        }
    }

    public class IntensityValueWalker
    {

        public IntensityValueWalker()
        {
            Current = new IntensityValue(null, 1 << 7, 1);
        }

        public IntensityValue Current { get; set; }

        public void MoveNext()
        {
            if (Current.Parent == null)
            {
                Current = Current.ChildA;
            }
            else if (Current.Parent.ChildA == Current)
            {
                Current = Current.Parent.ChildB;
            }
            else
            {
                int levelsUp = 1;
                Current = Current.Parent;
                while (Current.Parent != null && Current == Current.Parent.ChildB)
                {
                    Current = Current.Parent;
                    levelsUp++;
                }
                if (Current.Parent != null)
                {
                    Current = Current.Parent.ChildB;
                }
                else
                {
                    levelsUp++;
                }
                for (int i = 0; i < levelsUp; i++)
                {
                    Current = Current.ChildA;
                }
            }
        }
    }

    static class GraphicsExtension
    {
        private static GraphicsPath GenerateRoundedRectangle(
            this Graphics graphics,
            RectangleF rectangle,
            float radius)
        {
            float diameter;
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0.0F)
            {
                path.AddRectangle(rectangle);
                path.CloseFigure();
                return path;
            }
            else
            {
                if (radius >= (Math.Min(rectangle.Width, rectangle.Height)) / 2.0)
                    return graphics.GenerateCapsule(rectangle);
                diameter = radius * 2.0F;
                SizeF sizeF = new SizeF(diameter, diameter);
                RectangleF arc = new RectangleF(rectangle.Location, sizeF);
                path.AddArc(arc, 180, 90);
                arc.X = rectangle.Right - diameter;
                path.AddArc(arc, 270, 90);
                arc.Y = rectangle.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                arc.X = rectangle.Left;
                path.AddArc(arc, 90, 90);
                path.CloseFigure();
            }
            return path;
        }
        private static GraphicsPath GenerateCapsule(
            this Graphics graphics,
            RectangleF baseRect)
        {
            float diameter;
            RectangleF arc;
            GraphicsPath path = new GraphicsPath();
            try
            {
                if (baseRect.Width > baseRect.Height)
                {
                    diameter = baseRect.Height;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(baseRect.Location, sizeF);
                    path.AddArc(arc, 90, 180);
                    arc.X = baseRect.Right - diameter;
                    path.AddArc(arc, 270, 180);
                }
                else if (baseRect.Width < baseRect.Height)
                {
                    diameter = baseRect.Width;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(baseRect.Location, sizeF);
                    path.AddArc(arc, 180, 180);
                    arc.Y = baseRect.Bottom - diameter;
                    path.AddArc(arc, 0, 180);
                }
                else path.AddEllipse(baseRect);
            }
            catch { path.AddEllipse(baseRect); }
            finally { path.CloseFigure(); }
            return path;
        }

        /// <summary>
        /// Draws a rounded rectangle specified by a pair of coordinates, a width, a height and the radius 
        /// for the arcs that make the rounded edges.
        /// </summary>
        /// <param name="brush">System.Drawing.Pen that determines the color, width and style of the rectangle.</param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to draw.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to draw.</param>
        /// <param name="width">Width of the rectangle to draw.</param>
        /// <param name="height">Height of the rectangle to draw.</param>
        /// <param name="radius">The radius of the arc used for the rounded edges.</param>

        public static void DrawRoundedRectangle(
            this Graphics graphics,
            Pen pen,
            float x,
            float y,
            float width,
            float height,
            float radius)
        {
            RectangleF rectangle = new RectangleF(x, y, width, height);
            GraphicsPath path = graphics.GenerateRoundedRectangle(rectangle, radius);
            SmoothingMode old = graphics.SmoothingMode;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawPath(pen, path);
            graphics.SmoothingMode = old;
        }

        /// <summary>
        /// Draws a rounded rectangle specified by a pair of coordinates, a width, a height and the radius 
        /// for the arcs that make the rounded edges.
        /// </summary>
        /// <param name="brush">System.Drawing.Pen that determines the color, width and style of the rectangle.</param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to draw.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to draw.</param>
        /// <param name="width">Width of the rectangle to draw.</param>
        /// <param name="height">Height of the rectangle to draw.</param>
        /// <param name="radius">The radius of the arc used for the rounded edges.</param>

        public static void DrawRoundedRectangle(
            this Graphics graphics,
            Pen pen,
            int x,
            int y,
            int width,
            int height,
            int radius)
        {
            graphics.DrawRoundedRectangle(
                pen,
                Convert.ToSingle(x),
                Convert.ToSingle(y),
                Convert.ToSingle(width),
                Convert.ToSingle(height),
                Convert.ToSingle(radius));
        }

        /// <summary>
        /// Fills the interior of a rounded rectangle specified by a pair of coordinates, a width, a height
        /// and the radius for the arcs that make the rounded edges.
        /// </summary>
        /// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.</param>
        /// <param name="width">Width of the rectangle to fill.</param>
        /// <param name="height">Height of the rectangle to fill.</param>
        /// <param name="radius">The radius of the arc used for the rounded edges.</param>

        public static void FillRoundedRectangle(
            this Graphics graphics,
            Brush brush,
            float x,
            float y,
            float width,
            float height,
            float radius)
        {
            RectangleF rectangle = new RectangleF(x, y, width, height);
            GraphicsPath path = graphics.GenerateRoundedRectangle(rectangle, radius);
            SmoothingMode old = graphics.SmoothingMode;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillPath(brush, path);
            graphics.SmoothingMode = old;
        }

        /// <summary>
        /// Fills the interior of a rounded rectangle specified by a pair of coordinates, a width, a height
        /// and the radius for the arcs that make the rounded edges.
        /// </summary>
        /// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.</param>
        /// <param name="width">Width of the rectangle to fill.</param>
        /// <param name="height">Height of the rectangle to fill.</param>
        /// <param name="radius">The radius of the arc used for the rounded edges.</param>

        public static void FillRoundedRectangle(
            this Graphics graphics,
            Brush brush,
            int x,
            int y,
            int width,
            int height,
            int radius)
        {
            graphics.FillRoundedRectangle(
                brush,
                Convert.ToSingle(x),
                Convert.ToSingle(y),
                Convert.ToSingle(width),
                Convert.ToSingle(height),
                Convert.ToSingle(radius));
        }
    }

    public static class AESCipherMysql
    {
        public static String AES_encrypt(String Input, string key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(Input);
                    cs.Write(xXml, 0, xXml.Length);
                    cs.FlushFinalBlock();
                }

                xBuff = ms.ToArray();
            }

            String output = Convert.ToBase64String(xBuff);
            return output;
        }

        public static String AES_decrypt(String Input, string key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var decrypt = aes.CreateDecryptor();
            byte[] encryptedStr = Convert.FromBase64String(Input);

            byte[] xBuff = new byte[encryptedStr.Length];
            using (var ms = new MemoryStream(encryptedStr))
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Read))
                {
                    cs.Read(xBuff, 0, xBuff.Length);
                }
            }

            String output = Encoding.UTF8.GetString(xBuff);
            return output;
        }
    }
}
