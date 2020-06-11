using System;
using System.Collections.Generic;

namespace StationaryZCurve
{
    public class ZCurveUtility
    {
        public const int MIN_LAT = -90;
        public const int MAX_LAT = 90;
        public const int MIN_LNG = 0;
        public const int MAX_LNG = 180;

        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <param name="level">细分等级</param>
        public static string Encode (float latitude, float longitude, ushort level = 0)
        {
            long result = 0L;
            double maxLat = MAX_LAT;
            double minLat = MIN_LAT;
            double maxLng = MAX_LNG;
            double minLng = MIN_LNG;

            for (uint i = 0; i < 36; i++)
            {
                // 是不是纬度
                bool isLat = i % 2 == 0;
                if (isLat)
                {
                    double mid = (maxLat + minLat) / 2d;
                    if (latitude > mid)
                    {
                        result = (result << 1) + 1;
                        minLat = mid;
                    }
                    else
                    {
                        result <<= 1;
                        maxLat = mid;
                    }
                }
                else
                {
                    double mid = (maxLng + minLng) / 2d;
                    if (longitude > mid)
                    {
                        result = (result << 1) + 1;
                        minLng = mid;
                    }
                    else
                    {
                        result <<= 1;
                        maxLng = mid;
                    }
                }

            }

            Console.WriteLine ("long : " + Convert.ToString (result, 2));

            if (level == 0)
            {
                return result.ToString ();
            }

            double result2 = 0d;
            for (uint i = 1; i <= level * 2; i++)
            {
                bool isLat = i % 2 == 0;
                if (isLat)
                {
                    double mid = (maxLat + minLat) / 2d;
                    if (latitude > mid)
                    {
                        result2 += Math.Pow (2d, -i);
                        minLat = mid;
                    }
                    else
                    {
                        maxLat = mid;
                    }
                }
                else
                {
                    double mid = (maxLng + minLng) / 2d;
                    if (longitude > mid)
                    {
                        result2 += Math.Pow (2d, -i);
                        minLng = mid;
                    }
                    else
                    {
                        maxLng = mid;
                    }
                }
            }

            string bit = Convert.ToString (level, 2).PadLeft (3, '0');

            for (int i = 15; i < 18; i += 1)
            {
                if (bit[i - 15] == '1')
                {
                    result2 += Math.Pow (2d, -i);
                }
            }

            if (result == 0)
            {
                return result2.ToString ();
            }
            return result + result2.ToString ().Replace ("0.", ".");
        }

        public static double[] Decode (string z)
        {
            double maxLat = MAX_LAT;
            double minLat = MIN_LAT;
            double maxLng = MAX_LNG;
            double minLng = MIN_LNG;

            double binary = double.Parse (z);

            Console.WriteLine ("double : " + binary);

            for (int i = 35; i >= 0; i -= 1)
            {
                double bit = binary / (1L << i);
                //Console.WriteLine (bit + "   " + (1L << i));
                if (bit >= 1L)
                {
                    binary -= 1L << i;
                }
                //Console.WriteLine (bit + "   " + (1L << i));
                bool isLat = i % 2 == 1;

                if (isLat)
                {
                    double mid = (minLat + maxLat) / 2;
                    if (bit >= 1)
                    {
                        minLat = mid;
                    }
                    else
                    {
                        maxLat = mid;
                    }
                }
                else
                {
                    double mid = (minLng + maxLng) / 2;
                    if (bit >= 1)
                    {
                        minLng = mid;
                    }
                    else
                    {
                        maxLng = mid;
                    }
                }
            }

            double latitude = (minLat + maxLat) / 2;
            double longitude = (minLng + maxLng) / 2;

            Console.WriteLine (latitude);
            Console.WriteLine (longitude);

            return new double[2] { latitude, longitude };
        }

        public static void Main ()
        {
            string z = Encode (32.057f, 118.78098f, 0);
            Console.WriteLine (z);
            Decode (z);
            //11001100111000101101011001111110110100000000000000000
            //110011001101000111101001101111011110
        }
    }
}