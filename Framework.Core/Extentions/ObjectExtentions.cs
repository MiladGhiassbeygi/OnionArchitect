using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Core.Extentions
{
    public static class ObjectExtentions
    {
        public static string ToPrice(this object dec)
        {

            if (dec == "" || dec == null)
            {
                dec = 0;
            }

            string Src = dec.ToString();

            bool manfi = false;
            if (Src.Contains("-"))
            {
                Src = Src.Replace("-", "");
                manfi = true;

            }

            Src = Src.Replace(".0000", "");
            if (!Src.Contains("."))
            {
                Src = Src + ".00";
            }
            string[] price = Src.Split('.');
            if (price[1].Length >= 2)
            {
                price[1] = price[1].Substring(0, 2);
                price[1] = price[1].Replace("00", "");
            }

            string Temp = null;
            int i = 0;
            if ((price[0].Length % 3) >= 1)
            {
                Temp = price[0].Substring(0, (price[0].Length % 3));
                for (i = 0; i <= (price[0].Length / 3) - 1; i++)
                {
                    Temp += "," + price[0].Substring((price[0].Length % 3) + (i * 3), 3);
                }
            }
            else
            {
                for (i = 0; i <= (price[0].Length / 3) - 1; i++)
                {
                    Temp += price[0].Substring((price[0].Length % 3) + (i * 3), 3) + ",";
                }
                Temp = Temp.Substring(0, Temp.Length - 1);
                // Temp = price(0)
                //If price(1).Length > 0 Then
                //    Return price(0) + "." + price(1)
                //End If
            }
            if (price[1].Length > 0)
            {
                if (manfi == true)
                {
                    return (Temp + "." + price[1]) + "-";
                }
                return Temp + "." + price[1];
            }
            else
            {
                if (manfi == true)
                {
                    return (Temp) + "-";
                }
                return Temp;
            }
        }
    }
}
