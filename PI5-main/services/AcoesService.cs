using System;
using Microsoft.EntityFrameworkCore;
using pi5.database;
using pi5.entities;
using pi5.Interfaces.Services;
using PI5.entities;

namespace pi5.services;

public class AcoesService:IAcoesService{
        //Criando uma variável para a classe PI5Context
    private readonly PI5Context _context;
    private readonly IIntegracaoService _integracaoService; 

    //Construtor da classe de services
    public AcoesService(PI5Context context, IIntegracaoService integracaoService){
        _context = context;
        _integracaoService = integracaoService;
    }


    //Task: void do async
    public async Task AtualizaDados(){
        //Validações para adicionar as ações no banco. 
        RetornoAPI retornoDados = await _integracaoService.GetDados("quote", new() { "BBAS3", "ITUB4", "TSLA34", "MGLU3", "RENT3"});
        
        //Verificando se o dado encontrado na API já está inserido na coluna NOE no banco. 
        foreach (var acao in retornoDados.Results)
        {
            var acaoBanco = _context.Acoes.FirstOrDefault(x=>x.Nome==acao.LongName);
            if (acaoBanco == null){
                //Se o nome da ação na API ainda nao existir no banco(for nulo), irá instanciar a ação e adicionar no banco. 
                acaoBanco = new Acoes (){
                    Nome = acao.LongName,
                    Logo = acao.Logourl,
                    Sigla = acao.Symbol,

                };
                _context.Acoes.Add(acaoBanco);
                await _context.SaveChangesAsync();

                acaoBanco = _context.Acoes.FirstOrDefault(x=>x.Nome==acao.LongName);
            }

            //pegando todo o conteúdo da determinada ação existente no banco. 
            List<Valores>? valoresBanco = await _context.Valores.Where(x => x.Acao_id == acaoBanco.Id).ToListAsync();
            foreach (var valoresAPI in acao.HistoricalDataPrice){
                DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(valoresAPI.Data);
                if (valoresBanco.Count == 0){
                    Valores valor = new Valores(){
                        Acao_id = acaoBanco.Id,
                        Valor_Fechamento = valoresAPI.Close,
                        Valor_Abertura = valoresAPI.Open,
                        Valor_Alta = valoresAPI.High,
                        Valor_Baixa = valoresAPI.Low,
                        Data = dateTime.LocalDateTime

                    };

                    _context.Valores.Add(valor);

                    await _context.SaveChangesAsync();


                }
                else{
                    if (valoresBanco.Where(x=>x.Data==dateTime.LocalDateTime).FirstOrDefault() == null){
                        Valores valor = new Valores(){
                            Acao_id = acaoBanco.Id,
                            Valor_Fechamento = valoresAPI.Close,
                            Valor_Abertura = valoresAPI.Open,
                            Valor_Alta = valoresAPI.High,
                            Valor_Baixa = valoresAPI.Low,
                            Data = dateTime.LocalDateTime

                        };
                    _context.Valores.Add(valor);
                    await _context.SaveChangesAsync();

                    }
                }

            }
        }

    }
    public async Task MelhorAcao() {
        
    }

}