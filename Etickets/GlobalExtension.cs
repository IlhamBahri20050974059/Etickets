using System;
using System.Linq;
using System.Linq.Expressions;

namespace Etickets
{
    public class GlobalExtension
    {
    }
        public static class StringExt
        {
            public const string FileExt_Report = ".exported-rpt";

            public static string AsTempPath(this string filenameNoExt, string ext = "")
            {
                return System.IO.Path.GetTempPath() + filenameNoExt + ext;
            }
        }
}