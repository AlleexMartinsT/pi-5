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
        var todasAcoes = await _integracaoService.TodasAcoes();
        List<string> vinteAcoes = new();

        int contador = 0;

        for(int i = 0; i<todasAcoes.Count; i++){
            if (contador == 20){
                await AtualizaAcoesBanco(vinteAcoes);
                contador = 0;
                vinteAcoes.Clear();
                
            }
            vinteAcoes.Add(todasAcoes[i]);
            contador++;
        }
    
        if(contador > 0){
            await AtualizaAcoesBanco(vinteAcoes);

        }
        
    }

    private async Task AtualizaAcoesBanco(List<string> acoesNome){

        RetornoAPI retornoDados = await _integracaoService.GetDados("quote", acoesNome);
        
        //Verificando se o dado encontrado na API já está inserido na coluna no banco. 
        foreach (var acao in retornoDados.Results)
        {

            //verifica se existe uma ação no banco com o nome da que está sendo inserida
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


    //Decidir as 3 melhores ações das 5 para investir o dinheiro com base na análise da ultima semana
    public async Task HistoricoFechamento() {
        
        string dataInicial = "2024-04-01 00:00:00";
        string dataFinal = "2024-04-05 23:59:59";
        //Convertendo a data para o padrão aceito pelo .net
        DateTime dataInicio;
        DateTime.TryParseExact(dataInicial, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataInicio);
        DateTime dataFim;
        DateTime.TryParseExact(dataFinal, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataFim);
        
        // Camando os métodos e passando os parametros
        var mediaMovel = await GetMediaMovel(dataInicio, dataFim);
        var melhoresAcoes = await MelhorAcao(mediaMovel, 5);
        int carteiraId = await CadastroCarteira(melhoresAcoes);

        dataInicial = "2024-04-08 00:00:00";
        dataFinal = "2024-05-12 23:59:59";
        DateTime.TryParseExact(dataInicial, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataInicio);
        DateTime.TryParseExact(dataFinal, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataFim);

        //Chamando o metodo
        await InsercaoHistorico(carteiraId, dataInicio, dataFim);

    }
    public async Task<List<MediaAcoes>> GetMediaMovel(DateTime dataInicio, DateTime dataFim){
        List<Valores> historicoAcoes = await _context.Valores.Where(x=> x.Data >= dataInicio && x.Data <= dataFim).ToListAsync();
        List<Acoes> acoes = await _context.Acoes.ToListAsync();
        List<MediaAcoes> mediaAcoes = new ();


        //Foreach para calcular a soma dos valores de fechamento de cada uma das 5 ações
        foreach(var acao in acoes){
            //Se o id da ação da tabela acoes for igual ao id da tabela Valores
            var historicoLista = historicoAcoes.Where(x=> x.Acao_id == acao.Id).ToList();
            //contador da quantidade de dias da análise
            int contador = 0;
            decimal somaFechamento = 0;

            foreach(var historico in historicoLista){
                somaFechamento+= historico.Valor_Fechamento;
                contador ++;

            }

            if(contador > 0){
                //adicionando o id da ação e a média movel no periodo selecionado na lista de mediaAcoes

                mediaAcoes.Add(new MediaAcoes{
                AcaoId = acao.Id, 
                MediaMovel = somaFechamento / contador,
                Desvio = 0
            });
            }
           

        }
        return mediaAcoes;
        
    } 
//Escolher a melhor 3 ações das 5 que tem na carteira
    public async Task<List<MediaAcoes>> MelhorAcao(List<MediaAcoes> mediaAcoes, int qtdAcoesEscolhidas){
    string dataInicioInvestimento = "2024-04-08 10:00:00";
    DateTime dataInicio;
    DateTime.TryParseExact(dataInicioInvestimento, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataInicio);   
    List<Valores> historicoAcoes = await _context.Valores.Where(x => x.Data == dataInicio).ToListAsync();

    // Calculando o desvio padrão referente a cada ação da minha lista de mediaAcoes com a data atual
    for (int i = 0; i < mediaAcoes.Count; i++){
        mediaAcoes[i].Desvio = (float)Math.Round(((100 * historicoAcoes.FirstOrDefault(x => x.Acao_id == mediaAcoes[i].AcaoId).Valor_Abertura) / mediaAcoes[i].MediaMovel) - 100, 2);
    }

    // Ordenando as ações pelo desvio padrão em ordem crescente
    mediaAcoes = mediaAcoes.OrderBy(x => x.Desvio).ToList();

    // Pegando as 5 melhores ações e a sexta melhor ação
    var melhoresAcoes = mediaAcoes.Take(qtdAcoesEscolhidas).ToList();
    var sextaMelhorAcao = mediaAcoes[qtdAcoesEscolhidas];

    // Verificando se alguma das 5 melhores ações possui desvio padrão inferior a 5%
    foreach (var acao in melhoresAcoes){
        if (acao.Desvio < 5){
            // Substituir a ação com desvio padrão inferior a 5% pela sexta melhor ação
            var index = mediaAcoes.IndexOf(acao);
            mediaAcoes[index] = sextaMelhorAcao;
            break;
        }
    }

    // Retornando as 5 melhores ações após a possível substituição
    return mediaAcoes.Take(qtdAcoesEscolhidas).ToList();
}
    //Tabela carteira_acao
    public async Task<int> CadastroCarteira(List<MediaAcoes> melhoresAcoes){

        Carteira carteira = new();
        _context.Carteira.Add(carteira);
        await _context.SaveChangesAsync();

        foreach(var acao in melhoresAcoes){
            CarteiraAcao carteiraAcao = new(){
                Carteira_id = carteira.Id,
                Acao_id = acao.AcaoId,
                Qtd_cotas = 3
            };

            _context.CarteiraAcao.Add(carteiraAcao);
            await _context.SaveChangesAsync();
        }

        return carteira.Id;
    }

    //Tabela historico_carteira
    public async Task InsercaoHistorico(int carteiraId, DateTime dataInicio, DateTime dataFim){
        
        List<CarteiraAcao> carteirasAcoes = await _context.CarteiraAcao.Where(x=> x.Carteira_id == carteiraId).ToListAsync();
        
        //rodando foreach nas melhores ações a serem investidas
        foreach(var acao in carteirasAcoes){
            List<Valores> historicoAcoes = await _context.Valores.Where(x=> x.Data >= dataInicio && x.Data <= dataFim && x.Acao_id==acao.Acao_id).ToListAsync();
            historicoAcoes = historicoAcoes.OrderBy(x => x.Data).ToList();
            for(int i = 0; i < historicoAcoes.Count; i++){
                if(i == 0){
                    HistoricoCarteira historicoCarteira = new(){
                        Carteira_id = carteiraId,
                        Acao_id = acao.Acao_id,
                        Data_historico = historicoAcoes[i].Data,
                        Valor_Fechamento_Acao = historicoAcoes[i].Valor_Abertura*acao.Qtd_cotas
    

                    };

                    _context.HistoricoCarteira.Add(historicoCarteira);
                    await _context.SaveChangesAsync();
                }
                else{
                    HistoricoCarteira historicoCarteira = new(){
                        Carteira_id = carteiraId,
                        Acao_id = acao.Acao_id,
                        Data_historico = historicoAcoes[i].Data,
                        Valor_Fechamento_Acao = historicoAcoes[i].Valor_Fechamento*acao.Qtd_cotas
    

                    };

                    _context.HistoricoCarteira.Add(historicoCarteira);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    
    // public Task<RetornoDashboardAPI> RetornoDadosDashboard(int carteiraId){
    //     List<HistoricoCarteira> historicoCarteira = await _context.HistoricoCarteira.Where(x=>x.Carteira_id == carteiraId).ToListAsync();
    //     List<DateTime> Datas = await _context.
    
    // }



}