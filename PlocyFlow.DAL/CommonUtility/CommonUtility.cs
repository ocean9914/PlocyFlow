using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PlocyFlow.DAL.CommonUtility
{
    public class CommonUtility
    {
        public const string defaultCode = "0x80101";//默认用户具有的权限代码
        public static DateTime PasreStrToDateTime(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException();
            if (s.Length != 8)
                throw new FormatException();
            s = s.Substring(0, 4) + "-" + s.Substring(4, 2) + "-" + s.Substring(6, 2);
            return DateTime.Parse(s);
        }

        /// <summary>
        /// 判断指定字符串是否是整数格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool StringIsNumber(string s)
        {
            var p_out = 0;
            if (s == null || s.Trim().Length == 0)
                return false;
            return int.TryParse(s, out p_out);
        }

        public static int TryParseStringToInt32(string s)
        {
            if (StringIsNumber(s))
                return int.Parse(s);
            return 0;
        }

        public static bool StringIsDateTime(string s)
        {
            var datetime = DateTime.MinValue;
            if (s == null || s.Trim().Length == 0)
                return false;
            return DateTime.TryParse(s, out datetime);
        }

        public static DateTime? TryParseStringToDateTime(string s)
        {
            if (StringIsDateTime(s))
                return DateTime.Parse(s);
            return null;
        }

        public static string ConvertListStrToString(List<string> list)
        {
            if (list == null || list.Count == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.AppendFormat("{0};", item);
            }
            var result = sb.ToString();

            return result.Substring(0, result.LastIndexOf(";"));
        }

        /// <summary>
        /// 文件名加时间后缀
        ///   如：insert.txt返回insert_20121001120000.txt
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FileNameExtWithDateTime(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "";
            var array = fileName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != 2)
                return "";
            return string.Format("{0}_{1}.{2}", array[0], DateTime.Now.ToString("yyyyMMddHHmmss"), array[1]);
        }
        private static string chang(UInt64 d)
        {
            string x = "";
            switch (d)
            {
                case 10:
                    x = "A";
                    break;
                case 11:
                    x = "B";
                    break;
                case 12:
                    x = "C";
                    break;
                case 13:
                    x = "D";
                    break;
                case 14:
                    x = "E";
                    break;
                case 15:
                    x = "F";
                    break;
                default:
                    x = d.ToString();
                    break;
            }
            return x;
        }
        public static string DtoX(ulong v)
        {
            string x = "";
            if (v < 16)
                x = chang(v);
            else
            {
                ulong c;
                int s = 0;
                ulong n = v;
                ulong temp = v;
                while (n >= 16)
                {
                    s++;
                    n = n / 16;
                }
                string[] m = new string[s];
                int i = 0;
                do
                {
                    c = temp / 16;
                    m[i++] = chang(temp % 16);
                    temp = c;
                } while (c >= 16);
                x = chang(temp);
                for (int j = m.Length - 1; j >= 0; j--)
                {
                    x += m[j];
                }
            }
            return x;
        }
        public static int Comp(UInt64 v, UInt64 M)
        {
            int r = 0;
            if ((v & M) != 0)
                r = 1;
            return r;
        }
        public static string ReplaceNullString(object o)
        {
            string result = "";
            if (o != null && o != DBNull.Value)
                result = o.ToString();
            return result;
        }
        public static string DateTimeToString(object o, string format)
        {
            string r = "";
            if (o != null)
            {
                DateTime d;
                if (DateTime.TryParse(o.ToString(), out d))
                {
                    r = d.ToString(format);
                }
            }
            return r;
        }
        /// <summary>
        /// 将字符串按指定分隔符分隔，返回数组
        /// </summary>
        /// <param name="s">要处理的字符串</param>
        /// <param name="split">分隔符,默认英文分号（;）</param>
        /// <param name="isTrimEmpty">是否过滤分隔后的空字符串</param>
        /// <returns></returns>
        public static string[] SplitStringArray(string s, char[] split, bool isTrimEmpty)
        {
            string[] result = new string[0];
            if (s != null && s.Trim().Length > 0)
            {
                if (split == null || split.Length == 0)
                {
                    split = new[] { ';' };
                }
                if (isTrimEmpty)
                {
                    result = s.Split(split, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    result = s.Split(split);
                }
            }

            return result;
        }

        public static bool IsNullOrTrimEmpty(string s)
        {
            var result = true;
            if (s == null)
            {
                result = true;
            }
            else
            {
                result = (s.Trim().Length == 0);
            }

            return result;
        }
        public static string HasBoolValue(bool? v,string falseV,string trueV,string defaultV)
        {
            if (v.HasValue)
            {
                if (v.Value)
                    return trueV;
                else
                    return falseV;
            }
            else
                return defaultV;
        }
        public static string GetFileName(string filepath)
        {
            string r = filepath;
            if (filepath != null && filepath.Length > 0)
            {
                int idx = filepath.LastIndexOf('\\');
                if (idx != -1)
                    r = filepath.Substring(idx + 1);
                else
                { 
                    idx = filepath.LastIndexOf('/');
                    if(idx!=-1)
                        r=filepath.Substring(idx + 1);
                }
            }
            return r;
        }

    }
    public static class BaseId
    {
        private static UInt64 fid;
        private static object myLock = new object();
        private static string ymd;
        public static string getID()
        {
            string result = "";
            string currentYMD = DateTime.Now.ToString("yyyyMMdd");
            lock (myLock)
            {
                if (ymd == null || ymd != currentYMD)
                {
                    ymd = currentYMD;
                    fid = 0;
                }
                fid++;
                result = DateTime.Now.ToString("yyMMddHHmmss") + fid.ToString();
            }
            return result;
        }
        private static char getCharRadom()
        {
            int seed = unchecked((int)DateTime.Now.Ticks);
            seed += new Random(seed).Next();
            Random rm = new Random(seed);
            char r;
            if (rm.Next() % 2 == 0)
            {
                r = (char)(rm.Next(10) + 48);
            }
            else
            {
                int m = rm.Next(26) + 65;
                r = (char)m;
            }
            return r;
        }
        public static string getRadom(int length)
        {
            string r = "";
            StringBuilder sbd = new StringBuilder();
            List<char> cl = new List<char>();
            for (int i = 0; i < length; i++)
            {
                char c = getCharRadom();
                while (cl.Contains(c))
                {
                    c = getCharRadom();
                }
                cl.Add(c);
                sbd.Append(c);
            }
            r = sbd.ToString();
            return r;
        }
        public static string getDeclareId()
        {
            return "1" + DateTime.Now.ToString("yyMMddHHmmss") + getRadom(5);
        }
        public static string getDeclareFlowId()
        {
            return "2" + DateTime.Now.ToString("yyMMddHHmmss") + getRadom(5);
        }
        public static string getCompOrderId()
        {
            return "3" + DateTime.Now.ToString("yyMMddHHmmss") + getRadom(5);
        }
        public static string getCompFlowId()
        {
            return "4" + DateTime.Now.ToString("yyMMddHHmmss") + getRadom(5);
        }
    }
    public class Oper_Type
    {
        public string Oper_Key;
        public string Oper_Value;
    }
    public class OperList
    {
        public static Oper_Type NewSave = new Oper_Type() { Oper_Key = "新申报", Oper_Value = "NS" };
        public static Oper_Type Handle = new Oper_Type() { Oper_Key = "提交", Oper_Value = "HL" };
        public static Oper_Type Reject = new Oper_Type() { Oper_Key = "驳回", Oper_Value = "RJ" };
        public static Oper_Type Close = new Oper_Type() { Oper_Key = "关闭", Oper_Value = "CS" };
        public static Oper_Type To = new Oper_Type() { Oper_Key = "转交", Oper_Value = "TO" };
        public static Oper_Type getOT_ByValue(string value)
        {
            Oper_Type r = null;
            switch(value)
            {
                case "NS":
                    r = NewSave;
                    break;
                case "HL":
                    r = Handle;
                    break;
                case "RJ":
                    r = Reject;
                    break;
                case "CS":
                    r = Close;
                    break;
                case "TO":
                    r = To;
                    break;
            }
            return r;
        }
        public static Oper_Type getOT_ByKey(string key)
        {
            Oper_Type r = null;
            switch (key)
            {
                case "新申报":
                    r = NewSave;
                    break;
                case "提交":
                    r = Handle;
                    break;
                case "驳回":
                    r = Reject;
                    break;
                case "转交":
                    r = To;
                    break;
                case "关闭":
                    r = Close;
                    break;
            }
            return r;
        }
    }
}
