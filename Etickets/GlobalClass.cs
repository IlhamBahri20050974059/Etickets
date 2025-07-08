using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Etickets
{
    public class GlobalClass
    {
        public class ReportRequest
        {
            public string rptQuery { get; set; }
            public string rptFile { get; set; }
            /// <summary>
            /// Paper Size; 0-Default, 1-Letter, 4-Ledger, 5-Legal, 8-A3, 9-A4, 11-A5, 14-Folio
            /// </summary>
            public int rptPaperSizeEnum { get; set; }
            /// <summary>
            /// Paper Orientation: 1-Portrait, 2-Landscape
            /// </summary>
            public int rptPaperOrientationEnum { get; set; }
            public string rptExportType { get; set; }
            public Dictionary<string, string> rptParam { get; set; }
            public System.Data.DataTable rptDataSource { get; set; }
        }
    }
}