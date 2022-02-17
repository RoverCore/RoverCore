using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RoverCore.Datatables.DTOs;

namespace RoverCore.Datatables.Models
{
    public class DtMetadata
    {
        public List<DtMetaColumn> Columns { get; set; } = new();
        /// <summary>
        /// Index of primary key in Columns
        /// </summary>
        public int KeyIndex { get; set; } = -1;

        public string DatatableColumns
        {
            get
            {
                var serialized = Columns.Select(dc => JsonSerializer.Serialize(dc));
                return String.Join(",\n\t\t\t\t\t\t", serialized);
            }
        }

        public string DatatableColumnDefinitions
        {
            get
            {
                return string.Empty;
            }
        }
    }

    public class DtMetaColumn 
    {
        [JsonPropertyName("data")]
        public string Data { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public string DisplayName { get; set; } = string.Empty;
        [JsonPropertyName("searchable")]
        public bool Searchable { get; set; }
        [JsonPropertyName("orderable")]
        public bool Orderable { get; set; }
        [JsonPropertyName("autoWidth")]
        public bool Autowidth { get; set; } = true;
        [JsonPropertyName("visible")]
        public bool Visible { get; set; } = true;
        [JsonIgnore]
        public bool IsPrimaryKey { get; set; }
        [JsonIgnore]
        public bool IsDate { get; set; }
    }
}
