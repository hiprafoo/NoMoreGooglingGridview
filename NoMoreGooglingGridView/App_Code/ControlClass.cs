using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Drawing;

/// <summary>
/// Takes a aspx code as input and returns the conrol as output
/// If the control has child controls, then this will be refer to it
/// </summary>
public class ControlClass
{
    public System.Web.UI.Control ControlGenerated { get; set; }

    public ControlClass(string aspxCode)
    {
        string headerProperty;
        string headerProperties = "";
        string whatsNext;
        string property;
        string propertyValue;
        object gvObject = null;
        int colCount = 0;
        int lastCol = 1;
        TemplateField tc = new TemplateField();
        GridView gv;
        Dictionary<string, string> MappingPropNameValue = new Dictionary<string, string>();
        List<Dictionary<string, string>> ListCols = new List<Dictionary<string, string>>();
        
        string cntrlCode = Regex.Unescape(aspxCode)
            .Replace("\n", "")
            .Replace("\r", "");

        while (cntrlCode != "")
        {
            whatsNext = WhatsNext(ref cntrlCode);
            if (whatsNext == "HeaderProperty")
            {
                headerProperty = FindHeaderProperty(ref cntrlCode);
                if (headerProperty == "TemplateField" || headerProperty == "BoundField")
                {
                    colCount++;
                    headerProperty += colCount.ToString();
                }

                if (headerProperties == "")
                    headerProperties = headerProperty;
                else
                    headerProperties += "." + headerProperty;

                if (headerProperty == "ItemTemplate")
                {
                    propertyValue = FindControlsInTemplate(ref cntrlCode);
                    MappingPropNameValue.Add(headerProperties, propertyValue);
                    removeLastHeaderProperty(ref headerProperties);
                }
            }
            else if (whatsNext == "Property")
            {
                property = FindProperty(ref cntrlCode).Replace("-", ".");
                propertyValue = FindPropertyValue(ref cntrlCode);

                MappingPropNameValue.Add(headerProperties == "" ? property : headerProperties + "." + property, propertyValue);
            }
            else if (whatsNext == "Terminate")
            {
                RemoveTerminate(ref cntrlCode, ref headerProperties);
            }
            else
            {
            }
        }

        string controlName = string.Empty;
        foreach (var dic in MappingPropNameValue)
        {
            controlName = dic.Key.Substring(0, dic.Key.IndexOf('.'));
            break;
        }

        dynamic cn = GenerateControlInstance(controlName);

        if (controlName == "GridView")
        {
            gv = (GridView)cn;
        }
        else
        {
            gv = null;
        }

        foreach (var dic in MappingPropNameValue)
        {
            //If propName is column, then it should call a different private method

            string propName = dic.Key.ToString().Remove(0, dic.Key.ToString().IndexOf('.') + 1);
            string propValue = dic.Value;
            if (IsProperty(propName))
            {
                if (propName.Substring(0, propName.IndexOf('.') == -1 ? propName.Length - 1 : propName.IndexOf('.')).ToLower() != "columns")
                {
                    PropertyInfo propInfo = FindPropertyInfo(ref cn, out gvObject, propName);
                    AssignPropertyInfo(ref gvObject, propInfo, propValue);
                }
                else
                {
                    AddCols(ref gv, ref tc, ref lastCol, propName, propValue);
                }
            }
        }
        ControlGenerated = cn;
    }

    private string FindControlsInTemplate(ref string cntrlCode)
    {
        string propValue = cntrlCode.Substring(0, cntrlCode.IndexOf("</ItemTemplate>")).Trim();
        cntrlCode = cntrlCode.Remove(0, cntrlCode.IndexOf("</ItemTemplate>") + 15).Trim();
        return propValue;
    }

    private void AddCols<T>(ref T gv, ref TemplateField tc, ref int lastCol, string propName, string propValue) where T : GridView
    {
        int startPos = propName.IndexOf('.');
        int endPos = propName.IndexOf('.', startPos + 1);

        if (propName.Substring(startPos + 1, endPos - startPos - 1) == "BoundField")
        {
            BoundField bc = new BoundField();
            object bcObject = null;
            PropertyInfo propInfo = FindPropertyInfo(ref bc, out bcObject, propName.Remove(0, endPos + 1));
            AssignPropertyInfo(ref bcObject, propInfo, propValue);
            gv.Columns.Add(bc);
        }
        else if (propName.Substring(startPos + 1, 13) == "TemplateField")
        {
            object tcObject;
            int LenOfColInt = endPos - startPos - 13 - 1;
            int currCol = Convert.ToInt16(propName.Substring(startPos + 1 + 13, LenOfColInt));
            if(currCol != lastCol)
            {
                lastCol = currCol;
                //gv.Columns.Add(tc);   //Add the previous columns
                tc = new TemplateField();   //Refresh for the next column
            }

            if (propName.Remove(0, endPos + 1) == "ItemTemplate")
            {
                //http://www.codeproject.com/Tips/594549/Dynamic-columns-addition-to-GridView
                ControlClass cn = new ControlClass(propValue);
                TemplateHandler.control = cn.ControlGenerated;
                gv.Columns.Add(tc);   
                tc.ItemTemplate = new TemplateHandler();
            }
            else
            {
                PropertyInfo propInfo = FindPropertyInfo(ref tc, out tcObject, propName.Remove(0, endPos + 1));
                AssignPropertyInfo(ref tcObject, propInfo, propValue);
            }
        }
    }

    private PropertyInfo FindPropertyInfo<T>(ref T gv, out object gvObject, string propertyName)
    {
        string[] propertyNames = propertyName.Split('.');
        PropertyInfo propInfo = null;
        gvObject = null;
        for (int i = 0; i < propertyNames.Length; i++)
        {
            if (i == 0 && propertyNames.Length == 1)
            {
                propInfo = gv.GetType().GetProperty(propertyNames[i]);
                gvObject = gv;
            }
            else if (i == 0)
                gvObject = gv.GetType().GetProperty(propertyNames[i]).GetValue(gv, null);
            else if (i < propertyNames.Length - 1)
                gvObject = gvObject.GetType().GetProperty(propertyNames[i]).GetValue(gvObject, null);
            else
                propInfo = gvObject.GetType().GetProperty(propertyNames[i]);
        }
        return propInfo;
    }

    /// <summary>
    /// Not all the words in the aspx are properties, so filtering it out
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    private bool IsProperty(string prop)
    {
        //<br /> too needs to be skipped
        List<string> lstString = new List<string>();
        lstString.Add("runat");
        lstString.Add("DataSourceID");
        return !lstString.Contains(prop);
    }

    private void AssignPropertyInfo(ref object cntrlObject, PropertyInfo propertyInfo, string propertyValue)
    {
        switch (propertyInfo.PropertyType.FullName)
        {
            case "System.Boolean":
                propertyInfo.SetValue(cntrlObject, propertyValue.ToLower() == "true", null);
                break;
            case "System.Drawing.Color":
                if (propertyValue.Substring(0, 1) == "#")
                    propertyInfo.SetValue(cntrlObject, System.Drawing.ColorTranslator.FromHtml(propertyValue), null);
                else
                    propertyInfo.SetValue(cntrlObject, Color.FromName(propertyValue), null);
                break;
            case "System.Web.UI.WebControls.Unit":
                propertyInfo.SetValue(cntrlObject, new System.Web.UI.WebControls.Unit(propertyValue), null);
                break;
            case "System.Int32":
                propertyInfo.SetValue(cntrlObject, Convert.ToInt16(propertyValue), null);
                break;
            case "System.Web.UI.WebControls.BorderStyle":
                propertyInfo.SetValue(cntrlObject, (BorderStyle)Enum.Parse(typeof(BorderStyle),
                              propertyValue), null);
                break;
            case "System.Web.UI.WebControls.HorizontalAlign":
                propertyInfo.SetValue(cntrlObject, (HorizontalAlign)Enum.Parse(typeof(HorizontalAlign),
                              propertyValue), null);
                break;
            default:
                propertyInfo.SetValue(cntrlObject, propertyValue, null);
                break;
        }

    }

    private string WhatsNext(ref string grdCode)
    {
        string returnType = string.Empty;
        if (grdCode.Substring(0, 2) == "/>")
            returnType = "Terminate";
        else if (grdCode.Substring(0, 2) == "</")
            returnType = "Terminate";
        else if (grdCode.Substring(0, 1) == "<")
            returnType = "HeaderProperty";
        else if (grdCode.Substring(0, 1) == ">")
        {
            grdCode = grdCode.Remove(0, 1).Trim();
            if (grdCode == "</asp:GridView>")
                return grdCode;
        }
        else
            returnType = "Property";
        return returnType;
    }

    private string FindHeaderProperty(ref string grdCode)
    {
        int index = grdCode.IndexOf(' ') < grdCode.IndexOf('>') ? grdCode.IndexOf(' ') : grdCode.IndexOf('>') - 1;
        string returnString = grdCode.Substring(1, index).Replace("asp:", "").Trim();
        grdCode = grdCode.Remove(0, grdCode.IndexOf(' ')).Trim();
        return returnString;
    }

    private string FindProperty(ref string grdCode)
    {
        string returnString = grdCode.Substring(0, grdCode.IndexOf('=')).Trim();
        grdCode = grdCode.Remove(0, grdCode.IndexOf('=')).Trim();
        return returnString;
    }

    private string FindPropertyValue(ref string grdCode)
    {
        string returnString;
        if (grdCode.IndexOf("'") == -1 || grdCode.IndexOf("\"") < grdCode.IndexOf("'"))
        {
            returnString = grdCode.Substring(grdCode.IndexOf("\"") + 1, grdCode.IndexOf("\"", grdCode.IndexOf("\"") + 1) - 2).Trim();
            grdCode = grdCode.Remove(0, grdCode.IndexOf("\"", 2) + 2 - grdCode.IndexOf("\"")).Trim();
        }
        else
        {
            returnString = grdCode.Substring(grdCode.IndexOf("'") + 1, grdCode.IndexOf("'", grdCode.IndexOf("'") + 1) - 2).Trim();
            grdCode = grdCode.Remove(0, grdCode.IndexOf("'", 2) + 2 - grdCode.IndexOf("'")).Trim();
        }
        
        return returnString;
    }

    private void RemoveTerminate(ref string grdCode, ref string headerProperties)
    {
        grdCode = grdCode.Remove(0, grdCode.IndexOf('>') + 1).Trim();
        removeLastHeaderProperty(ref headerProperties);
    }

    private void removeLastHeaderProperty(ref string headerProperties)
    {
        //Remove the last property
        if (headerProperties.LastIndexOf('.') != -1)
            headerProperties = headerProperties.Remove(headerProperties.LastIndexOf("."), headerProperties.Length - headerProperties.LastIndexOf("."));
        else
            headerProperties = "";
    }

    private dynamic GenerateControlInstance(string controlName)
    {
        Dictionary<string, dynamic> controlInstance = new Dictionary<string, dynamic>();
        controlInstance.Add("Label", (Label)Activator.CreateInstance(typeof(Label)));
        controlInstance.Add("TextBox", (TextBox)Activator.CreateInstance(typeof(TextBox)));
        controlInstance.Add("GridView", (GridView)Activator.CreateInstance(typeof(GridView)));
        return controlInstance[controlName];
    }
}