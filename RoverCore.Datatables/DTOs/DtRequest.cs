using System.Text.Json.Serialization;

namespace RoverCore.Datatables.DTOs;

public class DtRequest
{
    [JsonPropertyName("draw")]
    public int Draw { get; set; }
    [JsonPropertyName("start")]
    public int Start { get; set; }
    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("columns")]
    public List<DtColumn> Columns { get; set; } = new();

    [JsonPropertyName("search")]
    public DtSearch Search { get; set; } = new();
    [JsonPropertyName("order")]
    public List<DtOrder> Order { get; set; } = new ();
}

public class DtColumn
{
	[JsonPropertyName("data")]
	public string Data { get; set; } = string.Empty;
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("searchable")]
    public bool Searchable { get; set; }
    [JsonPropertyName("orderable")]
    public bool Orderable { get; set; }
    [JsonPropertyName("search")]
    public DtSearch Search { get; set; } = new();
}

public class DtSearch
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
    [JsonPropertyName("regex")]
    public string Regex { get; set; } = string.Empty;
}

public class DtOrder
{
    [JsonPropertyName("column")]
    public int Column { get; set; }

    [JsonPropertyName("dir")]
    public string Dir { get; set; } = "asc";
}
