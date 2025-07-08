using CrystalDecisions.CrystalReports.Engine;
using Etickets.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using static Etickets.GlobalClass;

namespace Etickets
{
    public class GlobalFunctions
    {
        public static string GenerateReport(ReportRequest request)
        {
            var report = new ReportDocument();
            report.Load(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~"), "Report/" + request.rptFile + ".rpt"));
            if (string.IsNullOrEmpty(request.rptQuery) && request.rptDataSource != null)
                report.SetDataSource(request.rptDataSource);
            else if (!string.IsNullOrEmpty(request.rptQuery))
            {
                DataTable dtRpt = new ClassConnection().GetDataTable(request.rptQuery, request.rptFile);
                report.SetDataSource(dtRpt);
            }
            if (request.rptParam != null && request.rptParam.Count > 0)
                foreach (var item in request.rptParam)
                    report.SetParameterValue(item.Key, item.Value);
            if (string.IsNullOrEmpty(request.rptQuery) && request.rptDataSource == null)
                ClassProcedure.SetDBLogonForReport(report);
            if (request.rptPaperSizeEnum != 0)
                report.PrintOptions.PaperSize = (CrystalDecisions.Shared.PaperSize)request.rptPaperSizeEnum;
            if (request.rptPaperOrientationEnum != 0)
                report.PrintOptions.PaperOrientation = (CrystalDecisions.Shared.PaperOrientation)request.rptPaperOrientationEnum;
            var reportId = Guid.NewGuid();
            var saveAs = reportId.ToString().AsTempPath(StringExt.FileExt_Report);
            report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.CrystalReport, saveAs);
            report.Dispose(); report.Close();

            var sdir = System.IO.Path.GetTempPath();
            if (Directory.Exists(sdir))
            {
                foreach (var file in Directory.GetFiles(sdir))
                {
                    var updtime = File.GetLastWriteTime(file);
                    if (updtime < DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00:00")) && file.EndsWith(".exported-rpt"))
                    {
                        File.Delete(file);
                    }
                }
            }

            return reportId.ToString();
        }
    }
}