﻿namespace RO.Common3
{
    using System;
    using System.IO;
    using System.Text;
    using System.Data;
    using System.Data.OleDb;
    using System.Collections.Generic;
    using System.Threading;
    using System.Diagnostics;
    using RO.SystemFramewk;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.DirectoryServices;
    using System.Management;
    using System.Security.Cryptography.X509Certificates;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string Left(this string s, int left)
        {
            return s.Length == 0 ? "" : s.Substring(0, s.Length < left ? s.Length : left);
        }

        public static string StartFrom(this string s, int idx)
        {
            return s.Length <= idx ? "" : s.Substring(idx);
        }
    }

    public class DataStructure
    {
        public string ColumnName { get; set; }
        public string ColumnTitle { get; set; }
        public string ColumnType { get; set; }
        public int ColumnWidth { get; set; }
        public bool hasEmpty { get; set; }
        public double maxValue { get; set; }
        public int maxDecimal { get; set; }
        public bool lastIsEmpty { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
    }

    public class Utils
    {
        // Return ~/ added if needed:
        public static string AddTilde(string instr)
        {
            instr = instr.Trim();
            string str = instr.ToLower();
            if (str.StartsWith("searchlink")) { return instr; }
            else if (str.StartsWith("file:")) { return instr; }
            else if (str.StartsWith("tel:")) { return instr; }
            else if (str.StartsWith("mailto:")) { return instr; }
            else if (str.StartsWith("http://")) { return instr; }
            else if (str.StartsWith("https://")) { return instr; }
            else if (str.StartsWith("javascript:")) { return instr; }
            else if (str.StartsWith("../")) { return instr; }
            else if (str.StartsWith("~/")) { return instr; }
            else if (str.IndexOf("@") >= 0 && !str.StartsWith("mailto:")) { return "mailto:" + instr; }
            else if (IsPhone(str) && !str.StartsWith("tel:")) { return "tel:" + instr; }
            else { return "~/" + instr; }
        }

        // Remove ~/ if present:
        public static string StripTilde(string instr, bool bEmptyOnly)
        {
            if (bEmptyOnly && instr.Trim() == "~/") { return string.Empty; }
            else if (instr.StartsWith("~/")) { return instr.Substring(2); } else { return instr; }
        }

        // Return true if this is an integer.
        public static bool IsInt(string value)
        {
            int result;
            return int.TryParse(value, out result);
        }

        // Return true if this is a phone number.
        public static bool IsPhone(string value)
        {
            bool bPhone = false;
            value = value.Replace(" ", string.Empty).Replace(".", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("-", string.Empty);
            if (Regex.IsMatch(value, @"\d") && value.Length >= 6) { bPhone = true; }
            return bPhone;
        }

        public static string PopFirstWord(StringBuilder sourceString, char delimitor)
        {
            string oldWord = sourceString.ToString();
            string firstWord = "";
            if (oldWord != null && oldWord.Length > 0)
            {
                int delimitorIndex = oldWord.IndexOf(delimitor);
                if (delimitorIndex <= 0)
                {
                    firstWord = oldWord;
                    sourceString.Remove(0, oldWord.Length);
                }
                else
                {
                    firstWord = oldWord.Substring(0, delimitorIndex);
                    sourceString.Remove(0, delimitorIndex + 1);
                }
            }
            return firstWord;
        }

        public static string SetBool(bool bb)
        {
            if (bb) return "Y"; else return "N";
        }

        public static string Num2ExcelCol(int nn)
        {
            if (nn < 1) return "";
            const string az = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string ret = "";
            if (nn > 26) { ret = ret + az.Substring(((nn - 1) / 26) - 1, 1); }
            ret = ret + az.Substring((nn - 1) % 26, 1);
            return ret;
        }

        public static string fmLongDateTime(string ss, string culture)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            //else { return DateTime.Parse(ss).ToString("F", new System.Globalization.CultureInfo(culture)); }
            else { return fmLongDate(ss, culture) + " " + DateTime.Parse(ss).ToString("t", new System.Globalization.CultureInfo(culture)); }
        }

        public static string fmShortDateTime(string ss, string culture)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            //else { return DateTime.Parse(ss).ToString("f", new System.Globalization.CultureInfo(culture)); }
            else { return fmShortDate(ss, culture) + " " + DateTime.Parse(ss).ToString("t", new System.Globalization.CultureInfo(culture)); }
        }

        public static string fmDateTime(string ss, string culture)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else { return fmDate(ss, culture) + " " + DateTime.Parse(ss).ToString("t", new System.Globalization.CultureInfo(culture)); }
        }

        public static string fmLongDate(string ss, string culture)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else { return DateTime.Parse(ss).ToString("D", new System.Globalization.CultureInfo(culture)); }
        }

        // patLongDate:	d:		Single-digit day no leading zero
        //				dd:		Single-digit day with leading zero
        //				ddd:	Day of the week (abbr)
        //				dddd:	Day of the week (full)
        //				M:		Single-digit month no leading zero
        //				MM:		Single-digit month with leading zero
        //				MMM:	Month name (abbr)
        //				MMMM:	Month name (full)
        //				y:		Single-digit year no leading zero
        //				yy:		Single-digit year with leading zero
        //				yyyy:	Year (full)
        public static string fmLongDate(string ss, string culture, string patLongDate)
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(culture);
            ci.DateTimeFormat.LongDatePattern = patLongDate;
            if (ss.Equals(string.Empty)) { return ss; }
            else { return DateTime.Parse(ss).ToString("D", ci); }
        }

        // Format in CalendarExtender must match the output from Utils.fmDate... or TextChange event would be triggered:
        public static string fmShortDate(string ss, string culture)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else
            {
                return DateTime.Parse(ss).ToString("d", new System.Globalization.CultureInfo(culture));
            }
        }

        // Zero-filled date format for grid only:
        public static string fmShortDateZf(string ss, string culture)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else
            {
                string sd = DateTime.Parse(ss).ToString("d", new System.Globalization.CultureInfo(culture));
                string sep = new System.Globalization.CultureInfo(culture).DateTimeFormat.DateSeparator;
                string[] adt = sd.Split(sep[0]);
                for (int ii = 0; ii < adt.Length; ii++) { if (adt[ii].Length == 1) { adt[ii] = "0" + adt[ii]; } }
                return string.Join(sep, adt);
            }
        }

        public static string fmDate(string ss, string culture)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else { return DateTime.Parse(ss).ToString("d-MMM-yyyy", new System.Globalization.CultureInfo(culture)); }
        }
        public static string DateTimeUtcToTz(string ss, TimeZoneInfo tzInfo)
        {
            DateTime d = Convert.ToDateTime(ss);
            if (d.Hour == 0 && d.Minute == 0 && d.Second == 0 && d.Millisecond == 0) return ss;
            else return TimeZoneInfo.ConvertTimeFromUtc(d, tzInfo).ToString();
        }
        public static string fmLongDateTimeUTC(string ss, string culture, TimeZoneInfo tzInfo)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            //else { return DateTime.Parse(ss).ToString("F", new System.Globalization.CultureInfo(culture)); }
            else { return fmLongDateTime(DateTimeUtcToTz(ss, tzInfo), culture); }
        }

        public static string fmShortDateTimeUTC(string ss, string culture, TimeZoneInfo tzInfo)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            //else { return DateTime.Parse(ss).ToString("f", new System.Globalization.CultureInfo(culture)); }
            else { return fmShortDateTime(DateTimeUtcToTz(ss, tzInfo), culture); }
        }

        public static string fmDateTimeUTC(string ss, string culture, TimeZoneInfo tzInfo)
        {

            if (ss.Equals(string.Empty)) { return ss; }
            else { return fmDateTime(DateTimeUtcToTz(ss, tzInfo), culture); }
        }
        public static string fmLongDateUTC(string ss, string culture, TimeZoneInfo tzInfo)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else { return fmLongDate(DateTimeUtcToTz(ss, tzInfo), culture); }
        }
        public static string fmLongDateUTC(string ss, string culture, string patLongDate, TimeZoneInfo tzInfo)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else
            {
                return fmLongDate(DateTimeUtcToTz(ss, tzInfo), culture, patLongDate);
            }
        }
        public static string fmShortDateUTC(string ss, string culture, TimeZoneInfo tzInfo)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else
            {
                return fmShortDate(DateTimeUtcToTz(ss, tzInfo), culture);
            }
        }
        public static string fmShortDateZfUTC(string ss, string culture, TimeZoneInfo tzInfo)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else
            {
                return fmShortDateZf(DateTimeUtcToTz(ss, tzInfo), culture);
            }
        }
        public static string fmDateUTC(string ss, string culture, TimeZoneInfo tzInfo)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else { return fmDate(DateTimeUtcToTz(ss, tzInfo), culture); }
        }

        public static string fmCurrency(string ss, string culture)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else { return double.Parse(ss).ToString("c", new System.Globalization.CultureInfo(culture)); }
        }

        // patNegative:	0: ($n)
        //				1: -$n
        //				2: $-n
        //				3: $n-
        //				4: (n$)
        //				5: -n$
        //				6: n-$
        //				7: n$-
        // patPositive:	0: $n
        //				1: n$
        //				2: $ n
        //				3: n $
        public static string fmCurrency(string ss, string culture, int numDecimal, int patNegative, int patPositive)
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(culture);
            ci.NumberFormat.CurrencyDecimalDigits = numDecimal;
            ci.NumberFormat.CurrencyNegativePattern = patNegative;
            ci.NumberFormat.CurrencyPositivePattern = patPositive;
            if (ss.Equals(string.Empty)) { return ss; }
            else { return double.Parse(ss).ToString("c", ci); }
        }

        public static string fmMoney(string ss, string culture)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else { return double.Parse(ss).ToString("n", new System.Globalization.CultureInfo(culture)); }
        }

        // patNegative:	0: (n)
        //				1: -n
        //				2: - n
        //				3: n-
        //				4: n -
        public static string fmMoney(string ss, string culture, int numDecimal, int patNegative)
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(culture);
            ci.NumberFormat.NumberDecimalDigits = numDecimal;
            ci.NumberFormat.NumberNegativePattern = patNegative;
            if (ss.Equals(string.Empty)) { return ss; }
            else { return double.Parse(ss).ToString("n", ci); }
        }

        //patNegative:	0:	-n %	
        //				1:	-n%
        //				2:	-%n
        //patPositive:	0:	n %	
        //				1:	n%
        //				2:	%n
        public static string fmPercent(string ss, string culture, int numDecimal, int patNegative, int patPositive)
        {
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(culture);
            ci.NumberFormat.PercentDecimalDigits = numDecimal;
            ci.NumberFormat.PercentNegativePattern = patNegative;
            ci.NumberFormat.PercentPositivePattern = patPositive;
            if (ss.Equals(string.Empty)) { return ss; }
            else { return double.Parse(ss).ToString("p", ci); }
        }

        public static string fmNumeric(string ss, string culture)       // Backward compatible.
        {
            return fmNumeric(string.Empty, ss, culture);
        }

        public static string fmNumeric(string ColumnScale, string ss, string culture)
        {
            if (ss.Equals(string.Empty)) { return ss; }
            else if (string.IsNullOrEmpty(ColumnScale))
            { return Decimal.Parse(ss, System.Globalization.NumberStyles.Any).ToString("g", new System.Globalization.CultureInfo(culture)); }
            else { return Decimal.Parse(ss, System.Globalization.NumberStyles.Any).ToString("#############0." + string.Empty.PadRight(int.Parse(ColumnScale), Convert.ToChar(48))); }
        }

        public static string evalExpr(string expr)
        {
            double val = evalExprDbl(expr);
            if (val != double.NaN && val != double.PositiveInfinity && val != double.NegativeInfinity)
                return val.ToString();
            else
                return "0.00";
        }

        private static double evalExprDbl(string expr)
        {
            const int NONE = 11;
            const int POWER = 9;
            const int TIMES = 8;
            const int INT_DIV = 6;
            const int MOD = 5;
            const int PLUS = 4;

            string expression = expr;
            bool is_unary = false;
            bool next_unary = false;
            string ch = "";
            string lexpr = "";
            string rexpr = "";
            int best_pos = 0;
            int best_prec = 0;
            int parens = 0;
            int expr_len = 0;

            expr.Replace(" ", "");
            expr_len = expr.Length;

            if (expr_len == 0) return 0;
            is_unary = true;
            best_prec = NONE;
            for (int pos = 0; pos < expr_len; pos++)
            {
                ch = expr.Substring(pos, 1);
                next_unary = false;

                if (ch == "(")
                {
                    parens += 1;
                    next_unary = true;
                }
                else if (ch == ")")
                {
                    parens -= 1;
                    next_unary = false;

                    if (parens < 0)
                    {
                        throw new InvalidExpressionException("Too many close parentheses in '" + expression + "'");
                    }
                }
                else if (ch != " " && parens == 0)
                {
                    if (ch == "^" || ch == "*" ||
                        ch == "/" || ch == "\\" ||
                        ch == "%" || ch == "+" ||
                        ch == "-")
                    {
                        next_unary = true;

                        switch (ch.ToCharArray()[0])
                        {
                            case '^':
                                if (best_prec >= POWER)
                                {
                                    best_prec = POWER;
                                    best_pos = pos;
                                }
                                break;
                            case '*':
                            case '/':
                                if (best_prec >= TIMES)
                                {
                                    best_prec = TIMES;
                                    best_pos = pos;
                                }
                                break;
                            case '\\':
                                if (best_prec >= INT_DIV)
                                {
                                    best_prec = INT_DIV;
                                    best_pos = pos;
                                }
                                break;
                            case '%':
                                if (best_prec >= MOD)
                                {
                                    best_prec = MOD;
                                    best_pos = pos;
                                }
                                break;
                            case '+':
                            case '-':
                                if (!is_unary && best_prec >= PLUS)
                                {
                                    best_prec = PLUS;
                                    best_pos = pos;
                                }
                                break;
                        }
                    }
                }
                is_unary = next_unary;
            }
            if (parens != 0)
            {
                throw new InvalidExpressionException("Missing close parenthesis in '" + expression + "'");
            }
            if (best_prec < NONE)
            {
                lexpr = expr.Substring(0, best_pos);
                rexpr = expr.Substring(best_pos + 1);
                switch (expr.Substring(best_pos, 1).ToCharArray()[0])
                {
                    case '^': return Math.Pow(evalExprDbl(lexpr), evalExprDbl(rexpr));
                    case '*': return evalExprDbl(lexpr) * evalExprDbl(rexpr);
                    case '/': return evalExprDbl(lexpr) / evalExprDbl(rexpr);
                    case '\\': return (long)evalExprDbl(lexpr) / (long)evalExprDbl(rexpr);
                    case '%': return evalExprDbl(lexpr) % evalExprDbl(rexpr);
                    case '+': return evalExprDbl(lexpr) + evalExprDbl(rexpr);
                    case '-': return evalExprDbl(lexpr) - evalExprDbl(rexpr);
                }
            }

            if (expr.StartsWith("(") && expr.EndsWith(")"))
            {
                return evalExprDbl(expr.Substring(1, expr_len - 2));
            }

            if (expr.StartsWith("-"))
            {
                return -evalExprDbl(expr.Substring(1));
            }

            if (expr.StartsWith("+"))
            {
                return evalExprDbl(expr.Substring(1));
            }

            if (expr_len > 5 && expr.EndsWith(")"))
            {
                int parens_pos = expr.IndexOf("(");
                if (parens_pos > 0)
                {
                    lexpr = expr.Substring(0, parens_pos);
                    rexpr = expr.Substring(parens_pos + 1, expr_len - parens_pos - 2);
                    string fun = lexpr.ToLower();
                    if (fun == "sin") return Math.Sin(evalExprDbl(rexpr));
                    else if (fun == "cos") return Math.Cos(evalExprDbl(rexpr));
                    else if (fun == "tan") return Math.Tan(evalExprDbl(rexpr));
                    else if (fun == "sqrt") return Math.Sqrt(evalExprDbl(rexpr));
                }
            }
            return double.Parse(expr);
        }
        private static void JFileZip(FileInfo fi, Ionic.Zip.ZipFile zos, string baseName)
        {
            string fn = fi.FullName;
            bool has_slash = baseName.EndsWith("/") || baseName.EndsWith("\\") || true;
            string inDir = fn.Substring(baseName.Length + (has_slash ? 0 : 1)).Replace(fi.Name, "");
            zos.AddFile(fi.FullName, inDir);
        }

        private static void JFileZip(DirectoryInfo di, Ionic.Zip.ZipFile zos, string baseName, string match)
        {
            foreach (FileInfo fi in di.GetFiles())
            {
                if (match == null || fi.Name == match) { JFileZip(fi, zos, baseName); }
            }
            foreach (DirectoryInfo dii in di.GetDirectories())
            {
                JFileZip(dii, zos, baseName, match);
            }
        }
        private static void JFileZip(DirectoryInfo di, Ionic.Zip.ZipFile zos, string baseName, Func<string, string, bool> fIsIncluded, Func<string, string, bool> fIsExempted, string aspExt2Replace, string oldNS, string newNS)
        {
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fIsIncluded(fi.Name.ToLower(), fi.FullName.ToLower()) && !fIsExempted(fi.Name.ToLower(), fi.FullName.ToLower())) { JFileZip(fi, zos, baseName); }
            }
            foreach (DirectoryInfo dii in di.GetDirectories())
            {
                if (!fIsExempted(dii.Name.ToLower(), dii.FullName.ToLower())) JFileZip(dii, zos, baseName, fIsIncluded, fIsExempted, aspExt2Replace, oldNS, newNS);
            }
        }

        public static void JFileZip(string zipFr, string zipTo, bool bRecursive, string includedFiles, string exemptFiles, string aspExt2Replace, string oldNS, string newNS)
        {
            zipFr = zipFr.Replace(@"\\", @"\").Replace("//", "/");
            zipTo = zipTo.Replace(@"\\", @"\").Replace("//", "/");
            string zipFrNoSlash = zipFr.EndsWith(@"\") ? zipFr.Substring(0, zipFr.Length - 1) : zipFr;
            List<System.Text.RegularExpressions.Regex> exemptRules =
                (from o in exemptFiles.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>()
                 where !string.IsNullOrEmpty(o.Trim())
                 select new System.Text.RegularExpressions.Regex("^" + zipFrNoSlash.Replace("\\", "\\\\").Replace(".", "\\.") + (o.Contains(".") && !o.Contains("\\") ? ".*" : "") + (o.StartsWith("\\") ? @"(\\)?" : @"\\") + o.Trim().ToLower().Replace("\\", "\\\\").Replace("*.*", "*").Replace(".", "\\.").Replace("*", o.Contains("\\*.*") ? ".*" : "[^\\\\]*") + "$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)).ToList();
            Func<string, string, bool> fIsExempted = (f, p) => { foreach (var re in exemptRules) { if (re.IsMatch(p)) return true; } return false; };

            List<System.Text.RegularExpressions.Regex> includeRules =
                (from o in includedFiles.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>()
                 where !string.IsNullOrEmpty(o.Trim())
                 select new System.Text.RegularExpressions.Regex("^" + zipFrNoSlash.Replace("\\", "\\\\").Replace(".", "\\.") + (o.Contains(".") && !o.Contains("\\") ? ".*" : "") + (o.StartsWith("\\") ? @"(\\)?" : @"\\") + o.Trim().ToLower().Replace("\\", "\\\\").Replace("*.*", "*").Replace(".", "\\.").Replace("*", o.Contains("\\*.*") ? ".*" : "[^\\\\]*") + "$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)).ToList();
            Func<string, string, bool> fIsIncluded = (f, p) => { foreach (var re in includeRules) { if (re.IsMatch(p)) return true; } return false; };

            //var files = (from x in Directory.GetFiles(zipFr, "*.*", SearchOption.AllDirectories)
            //             where !fIsExempted(x,x)
            //             select x).ToList();
            using (Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile(zipTo))
            {
                if (!zipTo.StartsWith("\\\\")) zipFile.TempFileFolder = Path.GetDirectoryName(zipTo);
                zipFile.UseZip64WhenSaving = Ionic.Zip.Zip64Option.Always;
                zipFile.ParallelDeflateThreshold = -1;

                DirectoryInfo di = new DirectoryInfo(zipFr);
                if (di.Exists)
                {
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        if (fIsIncluded(fi.Name.ToLower(), fi.FullName.ToLower()) && !fIsExempted(fi.Name.ToLower(), fi.FullName.ToLower())) JFileZip(fi, zipFile, zipFr);
                    }
                    if (bRecursive)
                    {
                        foreach (DirectoryInfo dii in di.GetDirectories())
                        {
                            if (!fIsExempted(dii.Name.ToLower(), dii.FullName.ToLower()) && dii.FullName != zipTo) { JFileZip(dii, zipFile, zipFr, fIsIncluded, fIsExempted, aspExt2Replace, oldNS, newNS); }
                        }
                    }
                }
                else
                {
                    FileInfo fi = new FileInfo(zipFr);
                    if (fi.Exists)
                    {
                        if (fIsIncluded(fi.Name.ToLower(), fi.FullName.ToLower()) && !fIsExempted(fi.Name.ToLower(), fi.FullName.ToLower())) JFileZip(fi, zipFile, fi.DirectoryName);
                    }
                    if (bRecursive)
                    {
                        di = new DirectoryInfo(fi.DirectoryName);
                        foreach (DirectoryInfo dii in di.GetDirectories())
                        {
                            if (!fIsExempted(dii.Name.ToLower(), dii.FullName.ToLower()) && dii.FullName != zipTo) { JFileZip(dii, zipFile, fi.DirectoryName + "\\", fIsIncluded, fIsExempted, aspExt2Replace, oldNS, newNS); }
                        }
                    }
                }
                zipFile.Save();
            }
        }

        public static void JFileZip(string zipFr, string zipTo, bool bRecursive, string exemptFiles, string aspExt2Replace, string oldNS, string newNS)
        {
            zipFr = zipFr.Replace(@"\\", @"\").Replace("//", "/");
            zipTo = zipTo.Replace(@"\\", @"\").Replace("//", "/");
            string zipFrNoSlash = zipFr.EndsWith(@"\") ? zipFr.Substring(0, zipFr.Length - 1) : zipFr;
            List<System.Text.RegularExpressions.Regex> exemptRules =
                (from o in exemptFiles.Split('|').ToList<string>()
                 where !string.IsNullOrEmpty(o.Trim())
                 select new System.Text.RegularExpressions.Regex("^" + zipFrNoSlash.Replace("\\", "\\\\").Replace(".", "\\.") + (o.Contains(".") && !o.Contains("\\") ? ".*" : "") + (o.StartsWith("\\") ? @"(\\)?" : @"\\") + o.Trim().ToLower().Replace("\\", "\\\\").Replace("*.*", "*").Replace(".", "\\.").Replace("*", o.Contains("\\*.*") ? ".*" : "[^\\\\]*") + "$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)).ToList();
            Func<string, string, bool> fIsExempted = (f, p) => { foreach (var re in exemptRules) { if (re.IsMatch(p)) return true; } return false; };
            Func<string, string, bool> fIsIncluded = (f, p) => true;
            //var files = (from x in Directory.GetFiles(zipFr, "*.*", SearchOption.AllDirectories)
            //             where !fIsExempted(x,x)
            //             select x).ToList();
            using (Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile(zipTo))
            {
                if (!zipTo.StartsWith("\\\\")) zipFile.TempFileFolder = Path.GetDirectoryName(zipTo);
                zipFile.UseZip64WhenSaving = Ionic.Zip.Zip64Option.Always;
                zipFile.ParallelDeflateThreshold = -1;

                DirectoryInfo di = new DirectoryInfo(zipFr);
                if (di.Exists)
                {
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        if (!fIsExempted(fi.Name.ToLower(), fi.FullName.ToLower())) JFileZip(fi, zipFile, zipFr);
                    }
                    if (bRecursive)
                    {
                        foreach (DirectoryInfo dii in di.GetDirectories())
                        {
                            if (!fIsExempted(dii.Name.ToLower(), dii.FullName.ToLower()) && dii.FullName != zipTo) { JFileZip(dii, zipFile, zipFr, fIsIncluded, fIsExempted, aspExt2Replace, oldNS, newNS); }
                        }
                    }
                }
                else
                {
                    FileInfo fi = new FileInfo(zipFr);
                    if (fi.Exists)
                    {
                        if (!fIsExempted(fi.Name.ToLower(), fi.FullName.ToLower())) JFileZip(fi, zipFile, fi.DirectoryName);
                    }
                    if (bRecursive)
                    {
                        di = new DirectoryInfo(fi.DirectoryName);
                        foreach (DirectoryInfo dii in di.GetDirectories())
                        {
                            if (!fIsExempted(dii.Name.ToLower(), di.FullName) && dii.FullName != zipTo) { JFileZip(dii, zipFile, fi.DirectoryName + "\\", fIsIncluded, fIsExempted, aspExt2Replace, oldNS, newNS); }
                        }
                    }
                }
                zipFile.Save();
            }
        }
        public static void JFileZip(string zipFr, string zipTo, bool bRecursive, bool bRmFr)
        {
            using (Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile(zipTo, System.Text.Encoding.UTF8))
            {
                if (!zipTo.StartsWith("\\\\")) zipFile.TempFileFolder = Path.GetDirectoryName(zipTo);
                zipFile.UseZip64WhenSaving = Ionic.Zip.Zip64Option.Always;
                zipFile.ParallelDeflateThreshold = -1;
                zipFile.AddDirectory(zipFr, "");
                zipFile.Save();
            }

        }

        public static void JFileUnzip(string zipFileName, string destinationPath)
        {
            using (Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile(zipFileName, System.Text.Encoding.UTF8))
            {
                zipFile.ExtractAll(destinationPath);
            }
        }
        public static DateTime NextTargetDate(DateTime now, int? year, int? month, int? day, byte? dow)
        {
            DateTime today = new DateTime(now.Year, now.Month, now.Day);
            DateTime eow = new DateTime(9999, 12, 31);
            Func<DateTime, bool> fValidDow = d => dow == null || d.DayOfWeek == (DayOfWeek)dow.Value;
            Func<DateTime, bool> fValidYear = d => year == null || year.Value == d.Year;
            Func<DateTime, bool> fValidMonth = d => month == null || month.Value == d.Month;
            Func<DateTime, bool> fValidDay = d => day == null || day.Value == d.Day;
            Func<DateTime, DateTime> fNextDoW = d =>
            {
                if (d.DayOfWeek < (DayOfWeek)dow.Value)
                {
                    return d.AddDays((DayOfWeek)dow.Value - d.DayOfWeek);
                }
                else
                {
                    return d.AddDays(7 - (d.DayOfWeek - (DayOfWeek)dow.Value));
                }
            };

            Func<DateTime, DateTime> fNext = d =>
            {
                if (year != null)
                {
                    if (month != null)
                    {
                        if (day != null)
                        {
                            return fValidDow(d) && d == new DateTime(year.Value, month.Value, day.Value) ? d : eow;
                        }
                        else
                        {
                            if (dow == null) d = d.AddDays(1);
                            else d = fNextDoW(d);
                            return fValidMonth(d) ? d : eow;
                        }
                    }
                    else if (day != null)
                    {
                        while (fValidYear(d))
                        {
                            d = d.AddMonths(1);
                            if (fValidDow(d)) break;
                        }
                        return fValidYear(d) && fValidDow(d) ? d : eow;
                    }
                    else
                    {
                        if (dow == null) d = d.AddDays(1);
                        else d = fNextDoW(d);
                        return fValidYear(d) ? d : eow;
                    }
                }
                else if (month != null)
                {
                    if (day != null)
                    {
                        do
                        {
                            d = d.AddYears(1);
                        } while (!fValidDow(d));
                        return d;
                    }
                    else
                    {
                        do
                        {
                            if (dow == null) d = d.AddDays(1);
                            else d = fNextDoW(d);

                            if (fValidMonth(d) && fValidDow(d)) return d;
                            if (!fValidMonth(d)) d = new DateTime(d.Year + 1, month.Value, 1);
                        } while (true);
                    }
                }
                else if (day != null)
                {
                    do
                    {
                        d = d.AddMonths(1);
                    } while (!fValidDow(d));
                    return d;
                }
                else
                {
                    if (dow == null) d = d.AddDays(1);
                    else d = fNextDoW(d);
                    return d;
                }
            };
            return fNext(day != null ? new DateTime(today.Year, today.Month, day.Value) : today);
        }
        public static DateTime GetNextRun(DateTime now, int? year, int? month, int? day, byte? dow, int? hour, int? min)
        {
            now = DateTime.Parse(now.ToString("g")); // strip to minute for comparison.
            DateTime today = new DateTime(now.Year, now.Month, now.Day);
            DateTime nextDate = new DateTime(year ?? today.Year, month ?? today.Month, day ?? today.Day, 0, 0, 0, 0);
            DateTime nextTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, hour ?? (nextDate == today ? now.Hour : 0), min ?? (nextDate == today ? now.Minute : 0), 0);
            if (nextTime <= now || (dow != null && (DayOfWeek)dow.Value != nextTime.DayOfWeek))
            {
                if ((dow != null && (DayOfWeek)dow.Value != nextTime.DayOfWeek) || nextDate < today)
                {
                    nextDate = NextTargetDate(today, year, month, day, dow);
                    nextTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, hour ?? 0, min ?? 0, 0);
                }
                else
                {
                    if (hour != null)
                    {
                        if (nextTime.Hour != now.Hour)
                        {
                            nextDate = NextTargetDate(today, year, month, day, dow);
                            nextTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, hour ?? 0, min ?? 0, 0);
                        }
                        else
                        {
                            nextTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, now.Hour, min ?? now.Minute, 0);
                        }
                        if (nextTime < now && min == null) nextTime = nextTime.Add(now.Subtract(nextTime)).AddMinutes(1);
                        if (new DateTime(nextTime.Year, nextTime.Month, nextTime.Day) != today)
                        {
                            nextDate = NextTargetDate(today, year, month, day, dow);
                            nextTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, hour ?? 0, min ?? 0, 0);
                        }
                        else if (nextTime < now && min != null)
                        {
                            nextDate = NextTargetDate(today, year, month, day, dow);
                            nextTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, hour ?? 0, min ?? 0, 0);
                        }
                    }
                    else if (min != null)
                    {
                        if (nextTime.Minute != min.Value
                            ||
                            (nextDate == today && 
                            (year == null || year == today.Year) && 
                            (month == null || month == today.Month) &&
                            (day == null || day == today.Day) &&
                            (dow == null || (DayOfWeek) dow == today.DayOfWeek) &&
                            (hour == null) &&
                            nextTime < now)
                            )
                        {
                            nextTime = nextTime.AddHours(1);
                            if (new DateTime(nextTime.Year, nextTime.Month, nextTime.Day) != today)
                            {
                                nextDate = NextTargetDate(today, year, month, day, dow);
                                nextTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, hour ?? 0, min ?? 0, 0);
                            }
                        }
                    }
                    else
                    {
                        nextTime = now.AddMinutes(1);

                        if (new DateTime(nextTime.Year, nextTime.Month, nextTime.Day) != today)
                        {
                            nextDate = NextTargetDate(today, year, month, day, dow);
                            nextTime = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, hour ?? 0, min ?? 0, 0);
                        }

                    }
                }
            }
            return nextTime;
        }

        public static string WinProc(string cmd_path, string cmd_arg, bool bErrFirst)
        {
            string er = "";
            string ss = "";
            //Prevent more than one compilation for the same project at the same time (DotNet2.0):
            Semaphore sem = new Semaphore(1, 1);
            sem.WaitOne();
            ProcessStartInfo psi = new ProcessStartInfo(cmd_path, cmd_arg);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            System.Diagnostics.Process proc = new Process();
            proc.StartInfo = psi;
            proc.Start();
            proc.ErrorDataReceived += (o, e) => er = er + (e.Data != null ? e.Data.ToString() + Environment.NewLine : string.Empty);
            proc.OutputDataReceived += (o, e) => ss = ss + (e.Data != null ? e.Data.ToString() + Environment.NewLine : string.Empty);
            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();
            //if (bErrFirst)     // Compile requires StandardError to run first or it would hang on error:
            //{
            //    er = proc.StandardError.r;
            //    ss = proc.StandardOutput.ReadToEnd();    // Must ReadToEnd if RedirectStandardOutput is set to true;
            //}
            //else // bcp requires StandardOutput to run first or it would hang all the time:
            //{
            //    ss = proc.StandardOutput.ReadToEnd();    // Must ReadToEnd if RedirectStandardOutput is set to true;
            //    er = proc.StandardError.ReadToEnd();
            //}
            proc.WaitForExit();
            proc.CancelErrorRead();
            proc.CancelOutputRead();
            sem.Release();
            if (er != string.Empty) { throw new ApplicationException(er); }
            return ss;
        }
        public static string ROEncryptString(string inStr, string inKey)
        {
            string outStr = string.Empty;
            RandomNumberGenerator rng = new RNGCryptoServiceProvider();
            // general format
            // base64(version byte + byte[] of IV + encrypted content) + '-' + visible tail portion
            // version 1 3DES CBC with 8 byte IV

            byte[] ver = new byte[] { 1 };
            byte[] iv = new byte[8];
            rng.GetBytes(iv);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Mode = CipherMode.CBC;
            des.IV = iv;
            des.Key = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(inKey));
            byte[] encryptedBlock = des.CreateEncryptor().TransformFinalBlock(UTF8Encoding.UTF8.GetBytes(inStr), 0, UTF8Encoding.UTF8.GetBytes(inStr).Length);
            outStr = Convert.ToBase64String(ver.Concat(iv).Concat(encryptedBlock).ToArray());
            return outStr;
        }

        public static string RODecryptString(string inStr, string inKey)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            byte[] encryptedData = Convert.FromBase64String(inStr);
            byte ver = encryptedData[0];
            int ivSize = 0;
            if (ver == 1) ivSize = 8;
            else throw new Exception("unsupported encryption version");

            des.IV = encryptedData.Skip(1).Take(ivSize).ToArray();
            des.Mode = CipherMode.CBC;
            des.Key = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(inKey));
            string outStr = UTF8Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(encryptedData.Skip(1 + ivSize).ToArray(), 0, encryptedData.Length - (1 + ivSize)));
            return outStr;
        }
        public static SecurityIdentifier GetComputerSid()
        {
            return new SecurityIdentifier((byte[])new DirectoryEntry(string.Format("WinNT://{0},Computer", Environment.MachineName)).Children.Cast<DirectoryEntry>().First().InvokeGet("objectSID"), 0).AccountDomainSid;
        }

        public static string SignData(byte[] data, string keyPath)
        {
            // Hash the data
            X509Certificate2 cert = new X509Certificate2(keyPath);
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PrivateKey;
            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] hash = sha1.ComputeHash(data);

            // Sign the hash
            return Convert.ToBase64String(csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")));
        }

        public static string SignData(byte[] data, byte[] raw_key)
        {
            // Hash the data
            X509Certificate2 cert = new X509Certificate2(raw_key);
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PrivateKey;
            SHA1Managed sha1 = new SHA1Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] hash = sha1.ComputeHash(data);

            // Sign the hash
            return Convert.ToBase64String(csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")));
        }
        public static KeyValuePair<string, bool> CheckValidLicense()
        {
            RO.Common3.Encryption e = new RO.Common3.Encryption();
            return new KeyValuePair<string, bool>(e.GetInstallID(), e.CheckValidLicense());
        }

        public static List<DataStructure> AnalyseExcelData(DataTable dtImp, int rowsToExamine)
        {
            Func<string, bool> isDateTime = x => { try { DateTime.Parse(x); return true; } catch { return false; } };
            Func<string, bool> isDate = x => { try { return DateTime.Parse(x).TimeOfDay.TotalSeconds == 0.0; } catch { return false; } };
            Func<string, bool> isFloat = x => { try { double.Parse(x); return !(x.EndsWith("-") || x.EndsWith("+")); } catch { return false; } };
            Func<string, bool> isPct = x => { try { double.Parse(x); return x.EndsWith("-"); } catch { return false; } };
            Func<string, bool> isInt = x => { try { int.Parse(x); return true; } catch { return false; } };
            List<DataStructure> columns = new List<DataStructure>();
            int ii = 0; int colCount = dtImp.Columns.Count;
            foreach (DataRow dr in dtImp.Rows)
            {
                bool hasSomeData = false;
                for (int jj = 0; jj < colCount; jj = jj + 1)
                {
                    if (ii == 0)
                    {
                        columns.Add(new DataStructure { ColumnName = dr[jj].ToString(), ColumnTitle = dr[jj].ToString(), ColumnType = "Unknown", ColumnWidth = 0, MaxLength = 0, MinLength = 0, hasEmpty = false });
                    }
                    else
                    {
                        DataStructure column = columns[jj];
                        string val = dr[jj].ToString();
                        bool isEmpty = string.IsNullOrEmpty(val);
                        hasSomeData = hasSomeData || !isEmpty;
                        column.MinLength = val.Length > 0 && (val.Length < column.MinLength || column.MinLength == 0) ? val.Length : column.MinLength;
                        column.MaxLength = val.Length > column.MaxLength ? val.Length : column.MaxLength;
                        if (!isEmpty && "NVarChar,Date,DateTime".IndexOf(column.ColumnType) < 0 && val.EndsWith("%"))
                        {
                            column.ColumnType = "Float";
                            column.maxDecimal = 6;
                        }
                        else if (!isEmpty && isInt(val) && "Int,Unknown".IndexOf(column.ColumnType) >= 0) column.ColumnType = "Int";
                        else if (!isEmpty && isDate(val) && "Date,Unknown".IndexOf(column.ColumnType) >= 0) column.ColumnType = "Date";
                        else if (!isEmpty && isDateTime(val) && "Date,DateTime,Unknown".IndexOf(column.ColumnType) >= 0) column.ColumnType = "DateTime";
                        else if (!isEmpty && (isFloat(val) || val == "-") && "Int,Float,Unknown".IndexOf(column.ColumnType) >= 0)
                        {
                            column.ColumnType = "Float";
                            if (val != "-")
                            {
                                double n = double.Parse(val);
                                column.maxValue = n;
                                int dec = val.LastIndexOf('.') > 0 ? val.Length - val.LastIndexOf('.') - 1 : 0;
                                column.maxDecimal = column.maxDecimal > dec ? column.maxDecimal : dec;
                            }
                        }
                        else if (!isEmpty) column.ColumnType = "NVarChar";
                        column.lastIsEmpty = isEmpty;
                    }
                }
                if (hasSomeData)
                {
                    foreach (var x in columns)
                    {
                        x.hasEmpty = x.hasEmpty || x.lastIsEmpty;
                    }
                }
                if ((!hasSomeData && ii > 10) || ii > rowsToExamine) break;
                ii = ii + 1;
            }

            foreach (DataStructure x in columns)
            {
                if (x.ColumnType == "Float")
                {
                    if (x.maxDecimal <= 2) x.ColumnType = "Money";
                }
                x.ColumnType = x.ColumnType == "Unknown" ? "NVarChar" : x.ColumnType;
                x.ColumnWidth = x.ColumnType == "NVarChar" ? (x.MinLength == x.MaxLength && x.MaxLength > 0 ? x.MaxLength : x.MaxLength * 2)
                        : x.ColumnWidth;
                x.ColumnTitle = x.ColumnName;
            }

            return (from x in columns where !string.IsNullOrEmpty(x.ColumnName) select x).ToList();
        }

        // Should only execute this on the client tier:
        //public static System.Collections.ArrayList GetSheetNames(string fileFullName)
        //{
        //    OleDbConnection conn = new OleDbConnection();
        //    System.Collections.ArrayList al = new System.Collections.ArrayList();
        //    try
        //    {
        //        conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileFullName + ";Extended Properties=\"Excel 8.0; HDR=NO; IMEX=1;\"";
        //        conn.Open();
        //        // Get original sheet order:
        //        DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
        //        DataRow[] drs = dt.Select(dt.Columns[2].ColumnName + " not like '*$Print_Area' AND " + dt.Columns[2].ColumnName + " not like '*$''Print_Area'");
        //        foreach (DataRow dr in drs) { al.Add(dr["TABLE_NAME"].ToString().Replace("'", string.Empty).Replace("$", string.Empty)); }
        //    }
        //    catch (Exception e)
        //    {
        //        ApplicationAssert.CheckCondition(false, "", "", e.Message);
        //    }
        //    finally
        //    {
        //        conn.Close(); conn = null;
        //    }
        //    return al;
        //}
    }
}