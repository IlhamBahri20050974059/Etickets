using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Etickets.Report
{
    public partial class ReportBooking : System.Web.UI.Page
    {
        ReportDocument rd = new ReportDocument();

        protected void Page_Load(object sender, EventArgs e)
        {

            string reportId = Request.QueryString["report"];
            string rpttype = Request.QueryString["rpttype"];
            string rptname = Request.QueryString["rptname"];
            if (reportId != null)
            {
                string filePath = reportId.ToString().AsTempPath(StringExt.FileExt_Report);
                if (File.Exists(filePath))
                {
                    rd.Load(filePath);
                    if (rpttype == "PDF")
                    {
                        Response.Buffer = false; Response.ClearContent(); Response.ClearHeaders();
                        rd.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Response, true, rptname);
                        rd.Close(); rd.Dispose();
                    }
                    else if (rpttype == "XLS")
                    {
                        Response.Buffer = false; Response.ClearContent(); Response.ClearHeaders();
                        rd.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.Excel, Response, true, rptname);
                        rd.Close(); rd.Dispose();
                    }
                    else if (rpttype == "CSV")
                    {
                        Response.Buffer = false; Response.ClearContent(); Response.ClearHeaders();
                        rd.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.Excel, Response, true, rptname);
                        rd.Close(); rd.Dispose();
                    }
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CRViewer.ReportSource = rd;
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            try
            {
                if (rd != null)
                {
                    if (rd.IsLoaded)
                    {
                        rd.Dispose();
                        rd.Close();
                    }
                }
            }
            catch
            {
                rd.Dispose();
                rd.Close();
            }
        }
    }
}