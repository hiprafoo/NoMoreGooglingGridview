using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class NoMoreGooglingGridView : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        List<p1> lst = new List<p1>();
        lst.Add(new p1 { Name = "hi1" });
        lst.Add(new p1 { Name = "hi2" });
        lst.Add(new p1 { Name = "hi3" });
        lst.Add(new p1 { Name = "hi4" });
        gg1.DataSource = lst;
        gg1.DataBind();
    }
    protected void btn1_Click(object sender, EventArgs e)
    {
        ControlClass d = new ControlClass(txtAspContent.Text);
        List<p1> lst = new List<p1>();
        lst.Add(new p1 { Name = "hi1" });
        lst.Add(new p1 { Name = "hi2" });
        lst.Add(new p1 { Name = "hi3" });
        lst.Add(new p1 { Name = "hi4" });
        GridView ss = new GridView();
        ss = ((GridView)d.ControlGenerated);
        ss.DataSource = lst;
        ss.DataBind();

        foreach (GridViewRow row in ss.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                Label myHyperLink = row.FindControl("lbl1") as Label;
                Label myHyperLnk1 = row.FindControl("Name2") as Label;
            }
        }

        foreach (GridViewRow row in gg1.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                Label myHyperLink = row.FindControl("lbl1") as Label;
            }
        }

        plcHolder1.Controls.Add(ss);
    }
}

public class p1
{
    public string Name { get; set; }
}