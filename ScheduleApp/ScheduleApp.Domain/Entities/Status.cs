using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ScheduleApp.Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        Activa, 
        Inactiva

    }
}
