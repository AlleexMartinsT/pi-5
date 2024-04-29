using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace pi5.entities;

[Table ("acoes")]
public class Acoes{
    public int Id{ get ; set;}
    public string Nome{ get ; set;}
    public string Sigla{ get ; set;}
    public string Logo{ get ; set;}

}