﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamyShop.Common.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Remove all white and space of text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveAllWhiteSpace(this string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return text.Replace(" ", String.Empty);
        }
    }
}
