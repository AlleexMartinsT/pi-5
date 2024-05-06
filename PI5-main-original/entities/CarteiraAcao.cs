using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace pi5.entities;

[Table ("carteira_acao")]
public class CarteiraAcao{
    public int Id{ get ; set;}
    public int Carteira_id{ get ; set;}
    public int Acao_id{ get ; set;}
    public int Qtd_cotas{ get ; set;}


}