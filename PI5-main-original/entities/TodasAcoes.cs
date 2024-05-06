using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace pi5.entities;

public class TodasAcoes{
    [JsonPropertyName("stocks")]
    public List<string> Stocks{ get ; set;}


}