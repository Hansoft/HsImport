using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HsImport
{
    class Mapping
    {
        ColumnDefinition.ColumnType ColumnTypeFromString(string aString)
        {
            return (ColumnDefinition.ColumnType)Enum.Parse(typeof(ColumnDefinition.ColumnType), aString);
        }

        internal class MappedColumn
        {
            private int excelColumnIndex;
            private ColumnDefinition hansoftColumnDefinition;
            private XmlElement xmlElement;

            internal int ExcelColumnIndex
            {
                get { return excelColumnIndex; }
            }

            internal ColumnDefinition HansoftColumnDefinition
            {
                get { return hansoftColumnDefinition; }
            }

            internal MappedColumn(XmlNode node, Mapping mapping)
            {
                if (node.Name == "MappedColumn")
                {
                    if (node.Attributes["ExcelColumnIndex"] != null)
                        excelColumnIndex = Int32.Parse(node.Attributes["ExcelColumnIndex"].Value);
                    else
                        throw new FormatException("Missing attribute: ExcelColumnIndex");
                    if (node.Attributes["HansoftColumn"] != null)
                    {
                        string hansoftColumnCodeName = node.Attributes["HansoftColumn"].Value;
                        hansoftColumnDefinition = mapping.FindColumnWithCodeName(hansoftColumnCodeName);
                        xmlElement =  (XmlElement)node;
                    }
                    else
                        throw new FormatException("Missing attribute: HansoftColumn");
                }
                else
                {
                    throw new FormatException("Expected element of type MappedColumn, got " + node.Name);
                }
            }

            internal MappedColumn(int excelColumnIndex, ColumnDefinition hansoftColumnDefinition, XmlDocument xmlDocument)
            {
                this.excelColumnIndex = excelColumnIndex;
                this.hansoftColumnDefinition = hansoftColumnDefinition;
                xmlElement = xmlDocument.CreateElement("MappedColumn");
                XmlAttribute attr = xmlDocument.CreateAttribute("ExcelColumnIndex");
                attr.Value = excelColumnIndex.ToString();
                xmlElement.Attributes.Append(attr);
                attr = xmlDocument.CreateAttribute("HansoftColumn");
                attr.Value = hansoftColumnDefinition == null ? "" : hansoftColumnDefinition.CodeName;
                xmlElement.Attributes.Append(attr);
            }

            internal void UpdateMappedHansoftColumn(ColumnDefinition colDef)
            {
                this.hansoftColumnDefinition = colDef;
                xmlElement.SetAttribute("HansoftColumn", colDef == null ? "" : colDef.CodeName);
            }

            internal XmlElement XmlElement
            {
                get { return this.xmlElement; } 
            }
        }

        internal class ColumnDefinition
        {
            internal enum ColumnType
            { String, Integer, Float, Date, DateTime, EnumSingle, EnumMulti, Hyperlink, MultilineText, People, Resources }

            string displayName;
            string codeName;
            ColumnType columnType;
            string[] options;

            bool isCustomColumn;

            internal ColumnDefinition(string displayName, string codeName, ColumnType columnType, string[] options, bool isCustomColumn)
            {
                this.displayName = displayName;
                this.codeName = codeName;
                this.columnType = columnType;
                this.options = options;
                this.isCustomColumn = isCustomColumn;
            }

            internal void Update(string displayName, string codeName, ColumnType columnType, string[] options)
            {
                this.displayName = displayName;
                this.codeName = codeName;
                this.columnType = columnType;
                this.options = options;
            }

            public override string ToString()
            {
                return displayName;
            }

            internal ColumnType Type
            {
                get { return columnType; }
            }

            internal string[] Options
            {
                get { return options; }
            }
            
            internal string DisplayName
            {
                get { return displayName; }
            }

            internal string CodeName
            {
                get { return codeName; }
            }

            internal bool IsCustomColumn
            {
                get { return isCustomColumn; }
            }
        }


        internal event EventHandler TargetChanged;
        internal event EventHandler SourceChanged;
        internal event EventHandler MappedColumnsChanged;
        internal event EventHandler ColumnAdded;
        internal event EventHandler ColumnDeleted;
        internal event EventHandler ColumnUpdated;
        internal event EventHandler FileNameChanged;
        internal event EventHandler DirtyChanged;

        private string fileName;
        private bool dirty = false;
        private List<ColumnDefinition> builtInColumns;
        private List<ColumnDefinition> customColumns;
        private List<MappedColumn> mappedColumns;
        private XmlDocument xmlDocument;
        private XmlElement documentElement;
        private XmlElement customColumnsElement;
        private XmlElement mappedColumnsElement;

        protected virtual void OnTargetChanged(EventArgs e)
        {
            if (TargetChanged != null)
                TargetChanged(this, e);
        }

        protected virtual void OnSourceChanged(EventArgs e)
        {
            if (SourceChanged != null)
                SourceChanged(this, e);
        }

        protected virtual void OnColumnAdded(object sender, EventArgs e)
        {
            if (ColumnAdded != null)
                ColumnAdded(sender, e);
        }

        protected virtual void OnColumnDeleted(object sender, EventArgs e)
        {
            if (ColumnDeleted != null)
                ColumnDeleted(sender, e);
        }

        protected virtual void OnColumnUpdated(object sender, EventArgs e)
        {
            if (ColumnUpdated != null)
                ColumnUpdated(sender, e);
        }

        protected virtual void OnMappedColumnsChanged(object sender, EventArgs e)
        {
            if (MappedColumnsChanged != null)
                MappedColumnsChanged(sender, e);
        }

        protected virtual void OnFileNameChanged(EventArgs e)
        {
            if (FileNameChanged != null)
                FileNameChanged(this, e);
        }

        protected virtual void OnDirtyChanged(EventArgs e)
        {
            if (DirtyChanged != null)
                DirtyChanged(this, e);
        }

        internal Mapping(string fileName, bool isNewFile)
        {
            xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            documentElement = xmlDocument.DocumentElement;
            if (documentElement.Name != "Mapping")
                throw new FormatException("The root element of the input file must be of type Mapping, got " + documentElement.Name);

            XmlNodeList topNodes = documentElement.ChildNodes;

            foreach (XmlNode node in topNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    XmlElement el = (XmlElement)node;
                    switch (el.Name)
                    {
                        case ("BuiltInColumns"):
                            builtInColumns = ParseColumns(el.ChildNodes, false);
                            break;
                        case ("CustomColumns"):
                            customColumnsElement = el;
                            customColumns = ParseColumns(el.ChildNodes, true);
                            break;
                        case ("ExcelToHansoftMapping"):
                            mappedColumnsElement = el;
                            mappedColumns = ParseMappedColumns(el.ChildNodes);
                            break;
                        default:
                            throw new FormatException("Expected element of type BuiltInColumns, CustomColumns, or ExcelToHansoftMapping, got " + el.Name);
                    }
                }
            }
            if (!isNewFile)
                this.FileName = fileName;
            SetDirty(false);
        }

        internal bool Save()
        {
            xmlDocument.Save(fileName);
            SetDirty(false);
            return true;
        }

        internal bool Save(string fileName)
        {
            this.fileName = fileName;
            return Save();
        }

        internal List<ColumnDefinition> BuiltInHansoftColumns
        {
            get { return builtInColumns; }
        }

        internal List<ColumnDefinition> CustomHansoftColumns
        {
            get { return customColumns; }
        }

        internal List<ColumnDefinition> AllHansoftColumns
        {
            get
            {
                List<ColumnDefinition> allColumns = new List<ColumnDefinition>();
                allColumns.AddRange(builtInColumns);
                allColumns.AddRange(customColumns);
                return allColumns; 
            }
        }

        internal List<MappedColumn> MappedColumns
        {
            get { return mappedColumns; }
        }

        internal ColumnDefinition FindColumnWithCodeName(string codeName)
        {
            // KSL: For practice rewrite with LINQ
            foreach (ColumnDefinition cd in AllHansoftColumns)
                if (cd.CodeName == codeName)
                    return cd;
            return null;
        }

        internal string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; OnFileNameChanged(EventArgs.Empty); }
        }

        internal void SetDirty(bool val)
        {
            dirty = val;
            OnDirtyChanged(EventArgs.Empty);
        }

        internal bool Dirty
        {
            get { return dirty; }
        }

        private ColumnDefinition AddColumn(string displayName, string codeName, ColumnDefinition.ColumnType columnType, string[] options, bool isCustomColumn, List<ColumnDefinition> cols)
        {
            ColumnDefinition col = new ColumnDefinition(displayName, codeName, columnType, options, isCustomColumn);
            cols.Add(col);
            return col;
        }

        internal void AddMappedColumn(int excelColumnIndex, ColumnDefinition hansoftColumnDefinition, bool makeMappingDirty)
        {
            MappedColumn mCol = new MappedColumn(excelColumnIndex, hansoftColumnDefinition, xmlDocument);
            mappedColumns.Add(mCol);
            mappedColumnsElement.AppendChild(mCol.XmlElement);
            if (makeMappingDirty)
                SetDirty(true);
            OnMappedColumnsChanged(mCol, EventArgs.Empty);
        }

        internal void ClearMappedColumns(bool makeMappingDirty)
        {
            mappedColumnsElement.RemoveAll();
            mappedColumns.Clear();
            OnMappedColumnsChanged(this, EventArgs.Empty);
            if (makeMappingDirty)
                SetDirty(true);
                
        }

        internal void DeleteCustomColumn(ColumnDefinition colDef)
        {
            
            customColumns.Remove(colDef);
            foreach (XmlElement el in customColumnsElement.ChildNodes)
            {
                if (el.GetAttribute("CodeName") == colDef.CodeName)
                {
                    customColumnsElement.RemoveChild(el);
                    break;
                }
            }
            SetDirty(true);
            OnColumnDeleted(colDef, EventArgs.Empty);
        }

        internal void UpdateCustomColumn(ColumnDefinition colDef, string displayName, string codeName, ColumnDefinition.ColumnType type, string[] options)
        {

            foreach (XmlElement el in customColumnsElement.ChildNodes)
            {
                if (el.GetAttribute("CodeName") == colDef.CodeName)
                {
                    el.RemoveAllAttributes();
                    AddColumnAttributes(el, displayName, codeName, type, options);
                    break;
                }
            }
            colDef.Update(displayName, codeName, type, options);
            SetDirty(true);
            OnColumnUpdated(colDef, EventArgs.Empty);
        }

        internal void AddCustomColumn(string displayName, string codeName, ColumnDefinition.ColumnType columnType, string[] options)
        {
            ColumnDefinition col = AddColumn(displayName, codeName, columnType, options, true, customColumns);
            XmlElement el = xmlDocument.CreateElement("Column");
            AddColumnAttributes(el, displayName, codeName, columnType, options);
            customColumnsElement.AppendChild(el);
            SetDirty(true);
            OnColumnAdded(col, EventArgs.Empty);
        }

        void AddColumnAttributes(XmlElement el, string displayName, string codeName, ColumnDefinition.ColumnType columnType, string[] options)
        {
            XmlAttribute attr;
            attr = xmlDocument.CreateAttribute("DisplayName");
            attr.Value = displayName;
            el.Attributes.Append(attr);
            attr = xmlDocument.CreateAttribute("CodeName");
            attr.Value = codeName;
            el.Attributes.Append(attr);
            attr = xmlDocument.CreateAttribute("Type");
            attr.Value = columnType.ToString();
            el.Attributes.Append(attr);
            foreach (string oString in options)
            {
                XmlElement oEl = xmlDocument.CreateElement("Option");
                attr = xmlDocument.CreateAttribute("Name");
                attr.Value = oString;
                oEl.Attributes.Append(attr);
                el.AppendChild(oEl);
            }
        }

        private string GetAttributeOrNull(XmlElement el, string attr)
        {
            return el.Attributes[attr] != null ? el.Attributes[attr].Value : null;
        }

        private bool GetAttributeOrFalse(XmlElement el, string attr)
        {
            return el.Attributes[attr] != null ? bool.Parse(el.Attributes[attr].Value) : false;
        }

        private void SetAttribute(XmlElement el, string attrName, string val)
        {
            if (el.Attributes[attrName] == null)
            {
                XmlAttribute attr = xmlDocument.CreateAttribute(attrName);
                attr.Value = val;
                documentElement.Attributes.Append(attr);
            }
            else
                el.Attributes[attrName].Value = val;
        }

        internal string Source
        {
            get
            {
                return GetAttributeOrNull(documentElement, "source");
            }
            set
            {
                SetAttribute(documentElement, "source", value);
                SetDirty(true);
                OnSourceChanged(EventArgs.Empty);
            }
        }

        internal bool FirstSourceRowIsHeader
        {
            get
            {
                return GetAttributeOrFalse(documentElement, "firstsourcerowisheader");
            }
            set
            {
                SetAttribute(documentElement, "firstsourcerowisheader", value.ToString());
            }
        }

        internal string Target
        {
            get
            {
                return GetAttributeOrNull(documentElement, "target");
            }
            set
            {
                SetAttribute(documentElement, "target", value);
                SetDirty(true);
                OnTargetChanged(EventArgs.Empty);
            }
        }

        List<MappedColumn> ParseMappedColumns(XmlNodeList nodes)
        {
            List<MappedColumn> mappedCols = new List<MappedColumn>();
            foreach (XmlNode node in nodes)
            {
                mappedCols.Add(new MappedColumn(node, this));
            }
            return mappedCols;
        }

        internal List<ColumnDefinition> ParseColumns(XmlNodeList nodes, bool customColumns)
        {
            List<ColumnDefinition> cols = new List<ColumnDefinition>();
            string displayName;
            string codeName;
            string type;
            string[] options;

            foreach (XmlNode node in nodes)
            {
                if (node is XmlElement)
                {
                    if (node.Name == "Column")
                    {
                        if (node.Attributes["DisplayName"] != null)
                            displayName = node.Attributes["DisplayName"].Value;
                        else
                            throw new FormatException("Missing attribute: DisplayName");
                        if (node.Attributes["CodeName"] != null)
                            codeName = node.Attributes["CodeName"].Value;
                        else
                            throw new FormatException("Missing attribute: CodeName");
                        if (node.Attributes["Type"] != null)
                        {
                            type = node.Attributes["Type"].Value;
                            if (type == "EnumSingle" || type == "EnumMulti")
                            {
                                List<string> optionsColl = new List<string>();
                                foreach (XmlNode el in node.ChildNodes)
                                {
                                    if (el is XmlElement)
                                    {
                                        if (el.Name == "Option")
                                        {
                                            if (el.Attributes["Name"] != null)
                                                optionsColl.Add(el.Attributes["Name"].Value);
                                            else
                                                throw new FormatException("Missing attribute: Name");
                                        }
                                        else
                                            throw new FormatException("Expected element of type Option, got " + el.Name);
                                    }
                                }
                                options = new string[optionsColl.Count];
                                for (int i = 0; i < options.Length; i += 1)
                                    options[i] = optionsColl[i];
                            }
                            else
                                options = new string[0];
                        }
                        else
                            throw new FormatException("Missing attribute: CodeName");
                    }
                    else
                    {
                        throw new FormatException("Expected element of type Column, got " + node.Name);
                    }
                    AddColumn(displayName, codeName, ColumnTypeFromString(type), options, customColumns, cols);
                }
            }
            return cols;
        }
    }
}
