using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Remora.Extensions
{
    public static class StringExtensions
    {
        public static string MakeValidFileName(this string name)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);
            return Regex.Replace(name, invalidReStr, "_");
        }
    }
}
