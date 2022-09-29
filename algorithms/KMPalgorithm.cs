using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD_lab14
{
    public class Substrings
    {
        /// <summary>
        /// Zadanie pierwsze, w którym musimy znaleźć najdłuższy fragment tekstu powtarzający się przynajmniej dwukrotnie
        /// </summary>
        /// <param name="text">Pierwszy string</param>
        /// <returns>
        /// length: długość najdłuższego fragmentu powtarzającego się przynajmniej 2 razy <br />
        /// longestCommonSubstring: najdłuższy fragment powtarzający się przynajmniej 2 razy
        /// </returns>
        public (int length, string longestSubstring) StageOne(string text)
        {
            int ret = -1, j = -1;

            for (int i = 0; i < text.Length; i++)
            {
                var temp = text.Substring(i);
                var d = text.Substring(0, i);
                var max = KMP(temp, d);
                if (max > ret)
                {
                    ret = max;
                    j = i;
                }
            }

            string r = j < 0 ? "" : text.Substring(j, ret);
            return (ret, r);
        }

        static int[] ComputeP(string x)
        {
            var P = new int[x.Length + 1];
            if (x.Length == 0)
                return P;

            int t = P[0] = 0;
            if (x.Length > 1)
                P[1] = 0;

            for (int j = 2; j < x.Length + 1; j++)
            {
                while (t > 0 && x[t] != x[j - 1])
                    t = P[t];
                if (x[t] == x[j - 1])
                    t++;
                P[j] = t;
            }

            return P;
        }


        /// <summary>
        /// Zadanie drugie, w którym musimy znaleźć dwa najdłuższe powtarzające się fragmenty w dwóch stringach
        /// </summary>
        /// <param name="x">Pierwszy string</param>
        /// <param name="y">Drugi string</param>
        /// <returns>
        /// length: długość najdłuższego wspólnego fragmentu <br />
        /// longestCommonSubstring: najdłuższy wspólny fragment
        /// </returns>
        public (int length, string longestCommonSubstring) StageTwo(string x, string y)
        {
            int ret = -1, j = -1;
            if (x.Length > y.Length)
            {
                var temp = x;
                x = y;
                y = temp;
            }

            for (int i = 0; i < x.Length; i++)
            {
                var temp = x.Substring(i);
                var max = KMP(temp, y);
                if (max > ret)
                {
                    ret = max;
                    j = i;
                    if (max == temp.Length)
                        break;
                }
                if (ret > temp.Length)
                    break;
            }

            string r = j < 0 ? "" : x.Substring(j, ret);
            return (ret, r);
        }

        public int KMP(string x, string y)
        {
            var result = -1;

            var P = ComputeP(x);
            int n = y.Length, m = x.Length;
            for (int i = 0, j = 0; i < n; i += Math.Max(j - P[j], 1))
            {
                j = P[j];
                while (i + j < n && j < m && y[i + j] == x[j])
                    j++;
                if (j > result)
                    result = j;
            }

            return result;
        }
    }
}
