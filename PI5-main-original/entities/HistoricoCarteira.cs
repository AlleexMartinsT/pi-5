using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace pi5.entities;

[Table ("historico_carteira")]
public class HistoricoCarteira{
    public int Id{ get ; set;}
    public int Carteira_id{ get ; set;}
    public int Acao_id{ get ; set;}
    public DateTime Data_historico{ get ; set;}
    public decimal Valor_Fechamento_Acao{ get ; set;}


}