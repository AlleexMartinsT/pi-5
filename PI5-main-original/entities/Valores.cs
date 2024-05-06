using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace pi5.entities;

[Table ("valores")]
public class Valores{
    public int Id{ get ; set;}
    public int Acao_id{ get ; set;}
    public DateTime Data{ get ; set;}
    public decimal Valor_Fechamento{ get ; set;}
    public decimal Valor_Abertura{ get ; set;}
    public decimal Valor_Alta{ get ; set;}
    public decimal Valor_Baixa{ get ; set;}

}