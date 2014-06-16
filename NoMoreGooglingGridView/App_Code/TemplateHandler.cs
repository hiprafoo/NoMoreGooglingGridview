using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

/// <summary>
/// Descrizione di riepilogo per TemplateHandler
/// </summary>
public class TemplateHandler : ITemplate
{
    public static Control control { get; set; }
    void ITemplate.InstantiateIn(Control container)
    {
        container.Controls.Add(control);
    }
}