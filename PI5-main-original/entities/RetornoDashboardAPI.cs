using System;

namespace pi5.entities;

public class RetornoDashboardAPI{

    public List<HistoricoAcoesRetornoAPI> HistoricoAcoes {get ; set ;}
}

public class HistoricoAcoesRetornoAPI{
    public DateTime Data {get; set;}
    public decimal ValorCarteira {get; set;}
    public decimal RetornoDiario {get; set;}
    public List<AcoesRetornoAPI> Acoes {get; set;}

}

public class AcoesRetornoAPI{
    public string Acao {get; set;}
    public string Sigla {get; set;}
    public decimal ValorCota{get; set;}
    public decimal ValotalInvestido {get; set;}
    public decimal RetornoDiario {get; set;}

}
