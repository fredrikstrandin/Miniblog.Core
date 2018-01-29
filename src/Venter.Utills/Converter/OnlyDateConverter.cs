using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Venter.Utilities.Converter
{
    public class OnlyDateConverter : IsoDateTimeConverter
    {
        public OnlyDateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null && value is DateTime)
            {
                DateTime? date = value as DateTime?;

                value = date.Value.AddHours(4);
            }

            base.WriteJson(writer, value, serializer);
        }
    }
}
