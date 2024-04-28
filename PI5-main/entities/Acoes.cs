using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace pi5.entities;

[Table ("acoes")]
public class Acoes{
    public int Id{ get ; set;}
    public string Nome{ get ; set;}
    public string Sigla{ get ; set;}
    public string Logo{ get ; set;}

    // Propriedades para armazenar os resultados da análise
        public decimal MediaMovel { get; set; }
        public decimal DesvioPadrao { get; set; }

}