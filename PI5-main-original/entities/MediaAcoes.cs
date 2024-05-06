using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace pi5.entities;


public class MediaAcoes{
    public int AcaoId{ get ; set;}
    public decimal MediaMovel {get;set;}
    public float Desvio {get; set;}

}