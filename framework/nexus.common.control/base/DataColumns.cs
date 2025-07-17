using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nexus.common.control
{
    public class FieldDefinition
    {
        public bool   PreferAutoSize      { get; set; } = false;
        public string Field               { get; set; } = string.Empty;
        public string Header              { get; set; } = string.Empty;
        public string Width               { get; set; } = string.Empty;
        public string Format              { get; set; } = string.Empty;
        public string Type                { get; set; } = string.Empty;
        public string TextAlign           { get; set; } = string.Empty;
        public string FilterType          { get; set; } = string.Empty;
        public string FilterOperator      { get; set; } = string.Empty;
        public string FilterValue         { get; set; } = string.Empty;
        public bool   Visible             { get; set; } = true;
        public bool   ReadOnly            { get; set; }
        public bool   AllowEditing        { get; set; }
        public bool   AllowFiltering      { get; set; }
        public bool   AllowSorting        { get; set; }
        public bool   AllowResizing       { get; set; }
        public bool   AllowReordering     { get; set; }
        public bool   ShowInColumnChooser { get; set; }
        public bool   ShowInRowChooser    { get; set; }
    }
}
