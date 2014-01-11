using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HsImport
{
    class HansoftXmlWriter
    {
        internal static void MapToHansoftXml(ExcelReader sourceReader, Mapping mapping)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement root = doc.CreateElement("HansoftProjectManagerXML");
            doc.AppendChild(root);
            XmlElement activities = doc.CreateElement("Activities");
            root.AppendChild(activities);
            if (sourceReader != null)
            {
                int startRowIndex = mapping.FirstSourceRowIsHeader ? 1 : 0;
                for (int iRow = startRowIndex; iRow < sourceReader.Rows.Count; iRow += 1)
                {
                    XmlElement task = doc.CreateElement("Task");
                    activities.AppendChild(task);
                    XmlElement customColumns = doc.CreateElement("CustomColumnDatas");
                    for (int iCol = 0; iCol < mapping.MappedColumns.Count; iCol += 1)
                    {
                        Mapping.ColumnDefinition colDef = mapping.MappedColumns[iCol].HansoftColumnDefinition;
                        if (colDef != null)
                        {
                            XmlElement el = doc.CreateElement(colDef.CodeName);
                            string excelText = sourceReader.Rows[iRow].Cells[iCol].Value;
                            if (colDef.Type == Mapping.ColumnDefinition.ColumnType.EnumSingle && !colDef.IsCustomColumn)
                            {
                                int num = 0;
                                int cNum = 0;
                                foreach (string optionText in colDef.Options)
                                {
                                    cNum += 1;
                                    if (excelText != null && optionText == excelText.Trim())
                                    {
                                        num = cNum;
                                        break;
                                    }
                                }
                                el.InnerText = num.ToString();
                            }
                            else if (colDef.Type == Mapping.ColumnDefinition.ColumnType.Resources)
                            {
                                if (excelText != null && excelText.Trim() != string.Empty)
                                {
                                    string[] resources = excelText.Split(new char[]{';'});
                                    foreach (string resource in resources)
                                    {
                                        XmlElement rEl = doc.CreateElement("Resource");
                                        el.AppendChild(rEl);
                                        XmlElement nEl = doc.CreateElement("ResourceName");
                                        nEl.InnerText = resource.Trim();
                                        rEl.AppendChild(nEl);
                                        XmlElement aEl = doc.CreateElement("Allocation");
                                        aEl.InnerText = "100";
                                        rEl.AppendChild(aEl);
                                    }
                                }
                            }
                            else
                                el.InnerText = System.Security.SecurityElement.Escape(excelText);
                            if (mapping.BuiltInHansoftColumns.Contains(colDef))
                            {
                                task.AppendChild(el);
                                if (colDef.DisplayName == "User story")
                                {
                                    el = doc.CreateElement("FlaggedAsUserStory");
                                    el.InnerText = "1";
                                    task.AppendChild(el);
                                }
                            }
                            else
                                customColumns.AppendChild(el);
                        }
                    }
                    task.AppendChild(customColumns);
                }
            }
            doc.Save(mapping.Target);
        }
    }
}
