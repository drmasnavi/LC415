using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YasnaSoftwareGroup
{
    public class LC415Wrapper
    {
//تنها>وسط>اول>آخر

        #region کاراکتر های فارسی یونیکد

        private static readonly CharInfo[] A =
        {
            new CharInfo(0x630, 0xfeac, 0xfeab, 0xfeac, 0xfeab),
            new CharInfo(0x62f, 0xfeaa, 0xfea9, 0xfeaa, 0xfea9),
            new CharInfo(0x62c, 0xfe9e, 0xfe9f, 0xfea0, 0xfe9d),
            new CharInfo(0x62d, 0xfea2, 0xfea3, 0xfea4, 0xfea1),
            new CharInfo(0x62e, 0xfea6, 0xfea7, 0xfea8, 0xfea5),
            new CharInfo(0x647, 0xfeea, 0xfeeb, 0xfeec, 0xfee9),
            new CharInfo(0x639, 0xfeca, 0xfecb, 0xfecc, 0xfec9),
            new CharInfo(0x63a, 0xfece, 0xfecf, 0xfed0, 0xfecd),
            new CharInfo(0x641, 0xfed2, 0xfed3, 0xfed4, 0xfed1),
            new CharInfo(0x642, 0xfed6, 0xfed7, 0xfed8, 0xfed5),
            new CharInfo(0x62b, 0xfe9a, 0xfe9b, 0xfe9c, 0xfe99),
            new CharInfo(0x635, 0xfeba, 0xfebb, 0xfebc, 0xfeb9),
            new CharInfo(0x636, 0xfebe, 0xfebf, 0xfec0, 0xfebd),
            new CharInfo(0x637, 0xfec2, 0xfec3, 0xfec4, 0xfec1),
            new CharInfo(0x643, 0xfeda, 0xfedb, 0xfedc, 0xfed9),
            new CharInfo(0x645, 0xfee2, 0xfee3, 0xfee4, 0xfee1),
            new CharInfo(0x646, 0xfee6, 0xfee7, 0xfee8, 0xfee5),
            new CharInfo(0x62a, 0xfe96, 0xfe97, 0xfe98, 0xfe95),
            new CharInfo(0x627, 0xfe8e, 0xfe8d, 0xfe8e, 0xfe8d),
            new CharInfo(0x644, 0xfede, 0xfedf, 0xfee0, 0xfedd),
            new CharInfo(0x628, 0xfe90, 0xfe91, 0xfe92, 0xfe8f),
            new CharInfo(0x64a, 0xfef2, 0xfef3, 0xfef4, 0xfef1),
            new CharInfo(0x633, 0xfeb2, 0xfeb3, 0xfeb4, 0xfeb1),
            new CharInfo(0x634, 0xfeb6, 0xfeb7, 0xfeb8, 0xfeb5),
            new CharInfo(0x638, 0xfec6, 0xfec7, 0xfec8, 0xfec5),
            new CharInfo(0x632, 0xfeb0, 0xfeaf, 0xfeb0, 0xfeaf),
            new CharInfo(0x648, 0xfeee, 0xfeed, 0xfeee, 0xfeed),
            new CharInfo(0x629, 0xfe94, 0xfe93, 0xfe93, 0xfe93),
            new CharInfo(0x649, 0xfef0, 0xfeef, 0xfef0, 0xfeef),
            new CharInfo(0x631, 0xfeae, 0xfead, 0xfeae, 0xfead),
            new CharInfo(0x624, 0xfe86, 0xfe85, 0xfe86, 0xfe85),
            new CharInfo(0x621, 0xfe80, 0xfe80, 0xfe80, 0xfe80),
            new CharInfo(0x626, 0xfe8a, 0xfe8b, 0xfe8c, 0xfe89),
            new CharInfo(0x623, 0xfe84, 0xfe83, 0xfe84, 0xfe83),
            new CharInfo(0x622, 0xfe82, 0xfe81, 0xfe82, 0xfe81),
            new CharInfo(0x625, 0xfe88, 0xfe87, 0xfe88, 0xfe87),
            new CharInfo(0x6a9, 0xfb8f, 0xfb90, 0xfb91, 0xfb8e),
            new CharInfo(0x6af, 0xfb93, 0xfb94, 0xfb95, 0xfb92),
            new CharInfo(0x686, 0xfb7b, 0xfb7c, 0xfb7d, 0xfb7a),
            new CharInfo(0x67e, 0xfb57, 0xfb58, 0xfb59, 0xfb56),
            new CharInfo(0x698, 0xfb8b, 0xfb8a, 0xfb8b, 0xfb8a),
            new CharInfo(0x6cc, 0xfef0, 0xfeef, 0xfef0, 0xfeef)
        };

        #endregion

        #region کاراکتر های فارسی هایسنس

        private static readonly ByteInfo[] B =
        {
            new ByteInfo(100, 101, 100, 101, 100), //د
            new ByteInfo(98, 99, 98, 99, 98), //ذ
            new ByteInfo(86, 89, 87, 88, 86), //ج
            new ByteInfo(90, 93, 91, 92, 90), //ح
            new ByteInfo(94, 97, 95, 96, 94), //خ
            new ByteInfo(162, 165, 163, 164, 162), //ه
            new ByteInfo(130, 133, 131, 132, 130), //ع
            new ByteInfo(134, 137, 135, 136, 134), //غ
            new ByteInfo(138, 141, 139, 140, 138), //ف
            new ByteInfo(142, 145, 143, 144, 142), //ق
            new ByteInfo(82, 85, 83, 84, 82), //ث
            new ByteInfo(114, 117, 115, 116, 114), //ص
            new ByteInfo(118, 121, 119, 120, 118), //ض
            new ByteInfo(122, 125, 123, 124, 122), //ط
            new ByteInfo(146, 149, 147, 148, 146), //ک
            new ByteInfo(154, 157, 155, 156, 154), //م
            new ByteInfo(158, 161, 159, 160, 158), //ن
            new ByteInfo(78, 81, 79, 80, 78), //ت
            new ByteInfo(72, 73, 72, 73, 72), //ا
            new ByteInfo(150, 153, 151, 152, 150), //ل
            new ByteInfo(74, 77, 75, 76, 74), //ب
            new ByteInfo(168, 171, 169, 170, 168), //ی
            new ByteInfo(106, 109, 107, 108, 106), //س
            new ByteInfo(110, 113, 111, 112, 110), //ش
            new ByteInfo(126, 129, 127, 128, 126), //ظ
            new ByteInfo(104, 105, 104, 105, 104), //ز
            new ByteInfo(166, 167, 166, 167, 166), //و
            new ByteInfo(78, 81, 79, 80, 78), //ت
            new ByteInfo(168, 171, 169, 170, 168), //ی
            new ByteInfo(102, 103, 102, 103, 102), //ر
            new ByteInfo(166, 167, 166, 167, 166), //و
            new ByteInfo(67, 67, 67, 67, 67), //ء
            new ByteInfo(67, 70, 68, 69, 67), //ئ
            new ByteInfo(72, 73, 72, 73, 72), //ا
            new ByteInfo(71, 73, 71, 73, 71), //آ
            new ByteInfo(72, 73, 72, 73, 72), //ا
            new ByteInfo(146, 149, 147, 148, 146), //ک
            new ByteInfo(48, 51, 49, 50, 48), //گ
            new ByteInfo(42, 45, 43, 44, 42), //چ
            new ByteInfo(38, 41, 39, 40, 38), //پ
            new ByteInfo(46, 47, 46, 47, 46), //ژ
            new ByteInfo(168, 171, 169, 170, 168) //ی
        };

        #endregion

        #region دسته بندی ها

        private static readonly int[] TheSet1 =
        {
            0x62c, 0x62d, 0x62e, 0x647, 0x639, 0x63a, 0x641, 0x642, 0x62b, 0x635,
            0x636, 0x637, 0x643, 0x645, 0x646, 0x62a,
            0x644, 0x628, 0x64a, 0x633, 0x634, 0x638, 0x6af, 0x686, 0x67e, 0x6a9,
            0x6cc, 0x626
        };

        private static readonly int[] TheSet2 =
        {
            0x627, 0x623, 0x625, 0x622, 0x62f, 0x630, 0x631, 0x632, 0x648, 0x624, 0x629,
            0x698, 0x649
        };

        private const string ConstToken = "39c488c1d6aee649";

        #endregion

        /// <summary>
        /// علائم قابل نمایش در این نسخه
        /// </summary>
        /// <remarks></remarks>
        public enum LCDSymbol
        {
            None = 0,
            RLS = 1,
            Kg = 2,
            Gr = 3,
            KgRLS,
            GrRLS
        }

        /// <summary>
        /// آماده سازی عدد جهت نمایش
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] WrapNumberToBeReady(string number, LCDSymbol symbol = LCDSymbol.None)
        {
            try
            {
                if (!Assembly.GetCallingAssembly().FullName.Contains("PublicKeyToken=" + ConstToken))
                {
                    return null;
                }
                if (number.Trim().Length > 15 && symbol == LCDSymbol.None)
                {

                    return ReverseArray(new byte[]
                    {
                        48, 102, 105, 75, 102, 73, 170, 108,
                        75, 98, 99, 131
                    });
                }
                if (number.Trim().Length > 11 && (symbol == LCDSymbol.RLS || symbol == LCDSymbol.Kg))
                {

                    return ReverseArray(new byte[]
                    {
                        48, 102, 105, 75, 102, 73, 170, 108,
                        75, 98, 99, 131
                    });
                }
                if (number.Trim().Length > 12 && symbol == LCDSymbol.Gr)
                {

                    return ReverseArray(new byte[]
                    {
                        48, 102, 105, 75, 102, 73, 170, 108,
                        75, 98, 99, 131
                    });
                }
                var arrayOfPrice = number.Trim().ToCharArray();
                var resultValue = new byte[number.Trim().Length];
                for (int i = 0; i < number.Trim().Length; i++)
                {
                    resultValue[number.Trim().Length - 1 - i] = FindSubsVal((byte) arrayOfPrice[i]);
                    //xresultValue[i] = FindSubsVal((byte) arrayOfPrice[i]);
                }
                if (symbol == LCDSymbol.None) return resultValue;
                if (symbol == LCDSymbol.RLS)
                {
                    var finalvalue = new byte[resultValue.Length + 4];
                    Array.Copy(resultValue, 0, finalvalue, 0, resultValue.Length);
                    finalvalue[resultValue.Length] = 102;
                    finalvalue[resultValue.Length + 1] = 169;
                    finalvalue[resultValue.Length + 2] = 73;
                    finalvalue[resultValue.Length + 3] = 150;
                    return finalvalue;
                }
                if (symbol == LCDSymbol.Kg)
                {
                    var finalvalue = new byte[resultValue.Length + 4];
                    Array.Copy(resultValue, 0, finalvalue, 0, resultValue.Length);
                    finalvalue[resultValue.Length] = 147;
                    finalvalue[resultValue.Length + 1] = 170;
                    finalvalue[resultValue.Length + 2] = 152;
                    finalvalue[resultValue.Length + 3] = 167;
                    return finalvalue;
                }
                if (symbol == LCDSymbol.Gr)
                {
                    var finalvalue = new byte[resultValue.Length + 3];
                    Array.Copy(resultValue, 0, finalvalue, 0, resultValue.Length);
                    finalvalue[resultValue.Length] = 49;
                    finalvalue[resultValue.Length + 1] = 103;
                    finalvalue[resultValue.Length + 2] = 154;
                    return finalvalue;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// آماده سازی قیمت واحد 
        /// </summary>
        /// <param name="price">قیمت واحد</param>
        /// <param name="symbol">علامت</param>
        /// <param name="lineNo">خط نمایشگر</param>
        /// <param name="shouldbeClear">پیش از ارسال پاکسازی کند؟</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] WrapUnitPrice(string price, LCDSymbol symbol = LCDSymbol.RLS, byte lineNo = 1,
            bool shouldbeClear = false)
        {
            try
            {
                if (!Assembly.GetCallingAssembly().FullName.Contains("PublicKeyToken=" + ConstToken))
                {
                    return null;
                }
                if (price.Trim().Length > 13 && symbol == LCDSymbol.None)
                {

                    return WrapArrayToBeShown(ReverseArray(new byte[]
                    {
                        48, 102, 105, 75, 102, 73, 170, 108,
                        75, 98, 99, 131
                    }), lineNo, shouldbeClear);
                }
                if (price.Trim().Length > 9 && (symbol == LCDSymbol.RLS))
                {
                    if (price.Trim().Length > 13)
                    {
                        return WrapArrayToBeShown(ReverseArray(new byte[]
                        {
                            48, 102, 105, 75, 102, 73, 170, 108,
                            75, 98, 99, 131
                        }), lineNo, shouldbeClear);
                    }
                    symbol = LCDSymbol.None;
                }
                var arrayOfPrice = price.Trim().ToCharArray();
                var resultValue = new byte[price.Trim().Length];
                //ساخت واژه فی از سمت راست
                var rightsidearray = WrapTextToBeReady("فی");
                var rightofresult = WrapArrayToBeShown(rightsidearray, lineNo, shouldbeClear);
                //ساخت رشته آماده ارسال
                for (int i = 0; i < price.Trim().Length; i++)
                {
                    //resultValue[price.Trim().Length - 1 - i] = FindSubsVal((byte)arrayOfPrice[i]);
                    resultValue[i] = FindSubsVal((byte) arrayOfPrice[i]);
                }
                var leftofresult = new byte[] {};
                if (symbol == LCDSymbol.None)
                {
                    leftofresult = WrapArrayToBeShown(resultValue, lineNo, false, false);
                }
                if (symbol == LCDSymbol.RLS)
                {
                    var finalvalue = new byte[resultValue.Length + 4];
                    Array.Copy(resultValue, 0, finalvalue, 4, resultValue.Length);
                    finalvalue[3] = 102;
                    finalvalue[2] = 169;
                    finalvalue[1] = 73;
                    finalvalue[0] = 150;
                    leftofresult = WrapArrayToBeShown(finalvalue, lineNo, false, false);
                }
                var finalresult = new byte[leftofresult.Length + rightofresult.Length];
                Array.Copy(rightofresult, 0, finalresult, 0, rightofresult.Length);
                Array.Copy(leftofresult, 0, finalresult, rightofresult.Length, leftofresult.Length);
                return finalresult;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// آماده سازی عدد جهت نمایش
        /// </summary>
        /// <param name="number">عدد جهت نمایش</param>
        /// <param name="symbol">نوع عدد</param>
        /// <param name="showpoint">نمایش ممیز</param>
        /// <returns>آرایه خروجی</returns>
        /// <remarks></remarks>
        public static byte[] WrapNumberToBeReady(decimal number, LCDSymbol symbol = LCDSymbol.None,
            bool showpoint = true)
        {
            try
            {
                if (!Assembly.GetCallingAssembly().FullName.Contains("PublicKeyToken=" + ConstToken))
                {
                    return null;
                }
                string strPrice = !showpoint ? String.Format("{0:0}", number) : Convert.ToString(number);
                if (strPrice.Length > 15 && symbol == LCDSymbol.None)
                {

                    return ReverseArray(new byte[]
                    {
                        48, 102, 105, 75, 102, 73, 170, 108,
                        75, 98, 99, 131
                    });
                }
                if (strPrice.Length > 11 && (symbol == LCDSymbol.RLS || symbol == LCDSymbol.Kg))
                {

                    return ReverseArray(new byte[]
                    {
                        48, 102, 105, 75, 102, 73, 170, 108,
                        75, 98, 99, 131
                    });
                }
                if (strPrice.Length > 12 && symbol == LCDSymbol.Gr)
                {

                    return ReverseArray(new byte[]
                    {
                        48, 102, 105, 75, 102, 73, 170, 108,
                        75, 98, 99, 131
                    });
                }
                var arrayOfPrice = strPrice.ToCharArray();
                var resultValue = new byte[strPrice.Length];
                for (int i = 0; i < strPrice.Length; i++)
                {
                    resultValue[strPrice.Length - 1 - i] = FindSubsVal((byte) arrayOfPrice[i]);
                    //xresultValue[i] = FindSubsVal((byte) arrayOfPrice[i]);
                }
                if (symbol == LCDSymbol.None) return resultValue;
                if (symbol == LCDSymbol.RLS)
                {
                    var finalvalue = new byte[resultValue.Length + 4];
                    Array.Copy(resultValue, 0, finalvalue, 0, resultValue.Length);
                    finalvalue[resultValue.Length] = 102;
                    finalvalue[resultValue.Length + 1] = 169;
                    finalvalue[resultValue.Length + 2] = 73;
                    finalvalue[resultValue.Length + 3] = 150;
                    return finalvalue;
                }
                if (symbol == LCDSymbol.Kg)
                {
                    var finalvalue = new byte[resultValue.Length + 4];
                    Array.Copy(resultValue, 0, finalvalue, 0, resultValue.Length);
                    finalvalue[resultValue.Length] = 147;
                    finalvalue[resultValue.Length + 1] = 170;
                    finalvalue[resultValue.Length + 2] = 152;
                    finalvalue[resultValue.Length + 3] = 167;
                    return finalvalue;
                }
                if (symbol == LCDSymbol.Gr)
                {
                    var finalvalue = new byte[resultValue.Length + 3];
                    Array.Copy(resultValue, 0, finalvalue, 0, resultValue.Length);
                    finalvalue[resultValue.Length] = 49;
                    finalvalue[resultValue.Length + 1] = 103;
                    finalvalue[resultValue.Length + 2] = 154;
                    return finalvalue;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// آماده سازی جمع کل
        /// </summary>
        /// <param name="price">مبلغ</param>
        /// <param name="symbol">علامت</param>
        /// <param name="lineNo">خط نمایشگر</param>
        /// <param name="shouldbeClear">پیش از ارسال پاکسازی کند؟</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] WrapTotalPrice(string price, LCDSymbol symbol = LCDSymbol.RLS, byte lineNo = 1,
            bool shouldbeClear = false)
        {
            try
            {
                if (!Assembly.GetCallingAssembly().FullName.Contains("PublicKeyToken=" + ConstToken))
                {
                    return null;
                }
                if (price.Trim().Length > 10 && symbol == LCDSymbol.None)
                {

                    return WrapArrayToBeShown(ReverseArray(new byte[]
                    {
                        48, 102, 105, 75, 102, 73, 170, 108,
                        75, 98, 99, 131
                    }), lineNo, shouldbeClear);
                }
                if (price.Trim().Length > 6 && (symbol == LCDSymbol.RLS))
                {
                    if (price.Trim().Length > 10)
                    {
                        return WrapArrayToBeShown(ReverseArray(new byte[]
                        {
                            48, 102, 105, 75, 102, 73, 170, 108,
                            75, 98, 99, 131
                        }), lineNo, shouldbeClear);
                    }
                    symbol = LCDSymbol.None;
                }
                var arrayOfPrice = price.Trim().ToCharArray();
                var resultValue = new byte[price.Trim().Length];
                //ساخت واژه فی از سمت راست
                var lsidearray = WrapTextToBeReady("جمع کل", false);
                var lofresult = WrapArrayToBeShown(lsidearray, lineNo, shouldbeClear, false);
                //ساخت رشته آماده ارسال
                for (int i = 0; i < price.Trim().Length; i++)
                {
                    resultValue[price.Trim().Length - 1 - i] = FindSubsVal((byte) arrayOfPrice[i]);
                    //resultValue[i] = FindSubsVal((byte)arrayOfPrice[i]);
                }
                var rofresult = new byte[] {};
                if (symbol == LCDSymbol.None)
                {
                    rofresult = WrapArrayToBeShown(resultValue, lineNo, false);
                }
                if (symbol == LCDSymbol.RLS)
                {
                    var finalvalue = new byte[resultValue.Length + 4];
                    Array.Copy(resultValue, 0, finalvalue, 0, resultValue.Length);
                    finalvalue[resultValue.Length] = 102;
                    finalvalue[resultValue.Length + 1] = 169;
                    finalvalue[resultValue.Length + 2] = 73;
                    finalvalue[resultValue.Length + 3] = 150;
                    rofresult = WrapArrayToBeShown(finalvalue, lineNo, false);
                }
                var finalresult = new byte[rofresult.Length + lofresult.Length];
                Array.Copy(lofresult, 0, finalresult, 0, lofresult.Length);
                Array.Copy(rofresult, 0, finalresult, lofresult.Length, rofresult.Length);
                return finalresult;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private static byte[] ReverseArray(IEnumerable<byte> bytes)
        {
            return bytes.Reverse().ToArray();
        }

        private static byte FindSubsVal(byte c, bool isquestionmark = false)
        {
            if (isquestionmark) return 66;
            if (c == '0') return 0x37;
            if (c == '1') return 0x38;
            if (c == '2') return 0x39;
            if (c == '3') return 0x3A;
            if (c == '4') return 0x3B;
            if (c == '5') return 0x3C;
            if (c == '6') return 0x3D;
            if (c == '7') return 0x3E;
            if (c == '8') return 0x3F;
            if (c == '9') return 0x40;
            if (c == '.') return 0x36;
            if (c == ',') return 54;
            return 0x42;
        }

        /// <summary>
        /// آماده سازی آرایه جهت نمایش بر روی صفحه
        /// </summary>
        /// <param name="argarray">آرایه ورودی</param>
        /// <param name="lineno">شماره خط</param>
        /// <param name="shouldbeclear">آیا صفحه نمایش پاک شود؟</param>
        /// <param name="isRTL">آیا راست به چپ می باشد؟ </param>
        /// <returns>آرایه خروجی</returns>
        /// <remarks></remarks>
        public static byte[] WrapArrayToBeShown(byte[] argarray, byte lineno = 1, bool shouldbeclear = true,
            bool isRTL = true)
        {
            try
            {
                if (!Assembly.GetCallingAssembly().FullName.Contains("PublicKeyToken=" + ConstToken))
                {
                    return null;
                }
                var finalarray = new byte[argarray.Length + 5];
                Array.Copy(argarray, 0, finalarray, 4, argarray.Length);
                if (shouldbeclear) finalarray[0] = 0x0C;
                finalarray[1] = 0x1B;
                finalarray[2] = 0x62;
                if (!isRTL) finalarray[2] = 0x61;
                if (lineno == 0 || lineno > 4)
                {
                    lineno = 1;
                }
                switch (lineno)
                {
                    case 1:
                        finalarray[3] = 0x40;
                        break;
                    case 2:
                        finalarray[3] = 0x41;
                        break;
                    case 3:
                        finalarray[3] = 0x42;
                        break;
                    case 4:
                        finalarray[3] = 0x43;
                        break;
                    default:
                        finalarray[3] = 0x40;
                        break;
                }

                finalarray[finalarray.Length - 1] = 0x0D;
                return finalarray;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// آماده سازی متن جهت نمایش
        /// </summary>
        /// <param name="text">متن موردنظر</param>
        /// <param name="isRTL">آیا راست به چپ می باشد؟ </param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] WrapTextToBeReady(string text, bool isRTL = true)
        {
            try
            {
                if (!Assembly.GetCallingAssembly().FullName.Contains("PublicKeyToken=" + ConstToken))
                {
                    return null;
                }
                if (isRTL)
                    return Hisense(Masnavi(text.Replace("ی", "ي")));
                return ReverseArray(Hisense(Masnavi(text.Replace("ی", "ي"))));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private static byte[] Hisense(IEnumerable<char> arabic)
        {
            try
            {
                if (!Assembly.GetCallingAssembly().FullName.Contains("PublicKeyToken=" + ConstToken))
                {
                    return null;
                }
                var arabize = arabic.Where(c => c != 32).ToArray();
                var returnVal = new byte[arabize.Length];
                for (int i = 0; i < arabize.Length; i++)
                {
                    if (arabize[i] <= 57 && arabize[i] >= 46)
                    {
                        returnVal[i] = FindSubsVal((byte) arabize[i], true);
                        continue;
                    }
                    Indexandtype it = Findindexofinfo(arabize[i]);
                    switch (it.Typeofc)
                    {
                        case TypeOfC.IsoGlyph:
                            returnVal[i] = B[it.Index].IsoWord;
                            break;
                        case TypeOfC.IniGlyph:
                            returnVal[i] = B[it.Index].IniWord;
                            break;
                        case TypeOfC.EndGlyph:
                            returnVal[i] = B[it.Index].EndOfWord;
                            break;
                        case TypeOfC.Character:
                            returnVal[i] = B[it.Index].Character;
                            break;
                        case TypeOfC.MidGlyph:
                            returnVal[i] = B[it.Index].MidOfWord;
                            break;
                    }
                }
                return returnVal;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private static Indexandtype Findindexofinfo(char c)
        {
            int index = 0;
            while (index < A.Length)
            {
                if (A[index].Character == c)
                {
                    return new Indexandtype(index, TypeOfC.Character);
                }
                if (A[index].EndOfWord == c)
                {
                    return new Indexandtype(index, TypeOfC.EndGlyph);
                }
                if (A[index].IniWord == c)
                {
                    return new Indexandtype(index, TypeOfC.IniGlyph);
                }
                if (A[index].IsoWord == c)
                {
                    return new Indexandtype(index, TypeOfC.IsoGlyph);
                }
                if (A[index].MidOfWord == c)
                {
                    return new Indexandtype(index, TypeOfC.MidGlyph);
                }
                index++;
            }
            return new Indexandtype(0, TypeOfC.Character);
        }

        internal static IEnumerable<char> Masnavi(string inputString)
        {
            char[] chArray = inputString.ToCharArray();
            for (int i = 0; i < inputString.Length; i++)
            {
                char ch = inputString[i];
                if (((((ch >= 'ء') && (ch <= 'ي')) || ((ch == 'ی') || (ch == 'ک'))) ||
                     (((ch == 'گ') || (ch == 'چ')) || (ch == 'پ'))) || (ch == 'ژ'))
                {
                    bool flag2;
                    int index = 0;
                    while (index < A.Length)
                    {
                        if (A[index].Character == inputString[i])
                        {
                            break;
                        }
                        index++;
                    }
                    if (i == (inputString.Length - 1))
                    {
                        flag2 = false;
                    }
                    else
                    {
                        flag2 = IsFromTheSet1(inputString[i + 1]) || IsFromTheSet2(inputString[i + 1]);
                    }
                    bool flag = i != 0 && IsFromTheSet1(inputString[i - 1]);
                    if (flag && flag2)
                    {
                        chArray[i] = A[index].MidOfWord;
                    }
                    if (!(!flag || flag2))
                    {
                        chArray[i] = A[index].EndOfWord;
                    }
                    if (!(flag || !flag2))
                    {
                        chArray[i] = A[index].IniWord;
                    }
                    if (!(flag || flag2))
                    {
                        chArray[i] = A[index].IsoWord;
                    }
                }
            }

            return chArray;

        }

        private static bool IsFromTheSet1(char ch)
        {
            for (int i = 0; i < 0x1c; i++)
            {
                if (ch == TheSet1[i])
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsFromTheSet2(char ch)
        {
            for (var i = 0; i < 13; i++)
            {
                if (ch == TheSet2[i])
                {
                    return true;
                }
            }
            return false;
        }

        private struct CharInfo
        {
            public readonly char Character;
            public readonly char EndOfWord;
            public readonly char IniWord;
            public readonly char MidOfWord;
            public readonly char IsoWord;

            public CharInfo(int character, int endGlyph, int iniGlyph, int midGlyph, int isoGlyph)
            {
                Character = Convert.ToChar(character);
                EndOfWord = Convert.ToChar(endGlyph);
                IniWord = Convert.ToChar(iniGlyph);
                MidOfWord = Convert.ToChar(midGlyph);
                IsoWord = Convert.ToChar(isoGlyph);
            }
        }

        private struct ByteInfo
        {
            public readonly byte Character;
            public readonly byte EndOfWord;
            public readonly byte IniWord;
            public readonly byte MidOfWord;
            public readonly byte IsoWord;

            public ByteInfo(int character, int endGlyph, int iniGlyph, int midGlyph, int isoGlyph)
            {
                Character = Convert.ToByte(character);
                EndOfWord = Convert.ToByte(endGlyph);
                IniWord = Convert.ToByte(iniGlyph);
                MidOfWord = Convert.ToByte(midGlyph);
                IsoWord = Convert.ToByte(isoGlyph);
            }
        }

        private struct Indexandtype
        {
            public readonly int Index;
            public readonly TypeOfC Typeofc;

            public Indexandtype(int index, TypeOfC typeofc)
            {
                Index = index;
                Typeofc = typeofc;

            }
        }

        private enum TypeOfC
        {
            Character = 0,
            EndGlyph,
            IniGlyph,
            MidGlyph,
            IsoGlyph
        }

        public static byte[] ConcatArrays(params byte[][] inputarray)
        {
            try
            {
                var totalLength = inputarray.Sum(b => b.Length);
                var retArray = new byte[totalLength];
                var fromwhere = 0;
                foreach (var bytearray in inputarray.Where(bytearray => bytearray != null && bytearray.Length != 0))
                {
                    Array.Copy(bytearray, 0, retArray, fromwhere, bytearray.Length);
                    fromwhere += bytearray.Length;
                }
                return retArray;

            }
            catch (Exception)
            {

                return null;
            }

            #region نمونه

            /*var sendingArray = new byte[
                                finalarray1.Length + finalarray2.Length + finalarray3.Length + finalarray4.Length
                                ];
            Array.Copy(finalarray1, 0, sendingArray, 0, finalarray1.Length);
            Array.Copy(finalarray2, 0, sendingArray, finalarray1.Length, finalarray2.Length);
            Array.Copy(finalarray3, 0, sendingArray, finalarray1.Length + finalarray2.Length, finalarray3.Length);
            Array.Copy(finalarray4, 0, sendingArray,
                finalarray1.Length + finalarray2.Length + finalarray3.Length,
                finalarray4.Length); */

            #endregion
        }
    }
}
