using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RoverCore.Datatables.DTOs
{
    public class DtResponseData
    {
	    [JsonPropertyName("draw")]
        public int Draw { get; set; }
        [JsonPropertyName("recordsFiltered")]
        public int RecordsFiltered { get; set; }
        [JsonPropertyName("recordsTotal")]
        public int RecordsTotal { get; set; }

        [JsonPropertyName("data")]
        public ICollection Data { get; set; } = new List<object>();
    }
}
