using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RoverCore.Datatables.Converters;

public class DatatablesDateConverter<T> : JsonConverter<T>
{
    public override void Write(Utf8JsonWriter writer, T date, JsonSerializerOptions options)
    {
        writer.WriteStringValue((date as dynamic).ToString("yyyy-MM-dd HH:mm")); 
    }
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        //Don't need to implement this unless you're using this to deserialize too
        throw new NotImplementedException();
    }
}

