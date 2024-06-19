using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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

        if (todasAcoes.Count > 61) {
            todasAcoes = todasAcoes.Take(61).ToList();
        }

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
            if(acao.LongName != null)
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
                    if(valoresAPI.Open != null)
                    {
                        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(valoresAPI.Data);
                        if (valoresBanco.Count == 0){
                            Valores valor = new Valores(){
                                Acao_id = acaoBanco.Id,
                                Valor_Fechamento = (decimal)valoresAPI.Close,
                                Valor_Abertura = (decimal)valoresAPI.Open,
                                Valor_Alta = (decimal)valoresAPI.High,
                                Valor_Baixa = (decimal)valoresAPI.Low,
                                Data = dateTime.LocalDateTime

                            };

                            _context.Valores.Add(valor);

                            await _context.SaveChangesAsync();


                        }
                        else{
                            if (valoresBanco.Where(x=>x.Data==dateTime.LocalDateTime).FirstOrDefault() == null){
                                Valores valor = new Valores(){
                                    Acao_id = acaoBanco.Id,
                                    Valor_Fechamento = (decimal)valoresAPI.Close,
                                    Valor_Abertura = (decimal)valoresAPI.Open,
                                    Valor_Alta = (decimal)valoresAPI.High,
                                    Valor_Baixa = (decimal)valoresAPI.Low,
                                    Data = dateTime.LocalDateTime

                                };
                            _context.Valores.Add(valor);
                            await _context.SaveChangesAsync();

                            }
                        }
                    }
                }
            }
        }
    }


    //Decidir as 5 melhores ações investir o dinheiro com base na análise da ultima semana
    public async Task HistoricoFechamento() {
        
       
        //Pegando os ultimos 30 dias para realizar a média móvel
        DateTime dataInicio = DateTime.Today.AddDays(-30);
        DateTime dataFim = DateTime.Today.AddDays(-1); 
        
        if (DateTime.Today.DayOfWeek != DayOfWeek.Sunday && DateTime.Today.DayOfWeek != DayOfWeek.Saturday)
        {
            // Camando os métodos e passando os parametros
            var mediaMovel = await GetMediaMovel(dataInicio, dataFim);
            var melhoresAcoes = await MelhorAcao(mediaMovel, 5);
            await AtualizaCarteira(melhoresAcoes);
        }

        // dataInicial = "2024-04-22 00:00:00";
        // dataFinal = "2024-04-26 23:59:59";
        // DateTime.TryParseExact(dataInicial, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataInicio);
        // DateTime.TryParseExact(dataFinal, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataFim);

        // //Chamando o metodod
        // await InsercaoHistorico(carteiraId, dataInicio, dataFim);

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
    //Escolher as 5 melhores ações de todas
    public async Task<List<MediaAcoes>> MelhorAcao(List<MediaAcoes> mediaAcoes, int qtdAcoesEscolhidas){
        DateTime dataInicio = DateTime.Today;
        List<Valores> historicoAcoes = await _context.Valores.Where(x=> x.Data.Date == dataInicio.Date).ToListAsync();

        //Calculando o desvio padrão referente a cada ação da minha lista de mediaAcoes com a data atual. 
        for(int i = 0; i < mediaAcoes.Count; i++){

            mediaAcoes[i].Desvio = (float) Math.Round(((100*historicoAcoes.FirstOrDefault(x=>x.Acao_id == mediaAcoes[i].AcaoId).Valor_Abertura) / mediaAcoes[i].MediaMovel)-100, 2);
            
            Console.WriteLine($"acaoID: {mediaAcoes[i].AcaoId} | Media Movel: {mediaAcoes[i].MediaMovel} | Valor Abertura: R${historicoAcoes.FirstOrDefault(x=>x.Acao_id == mediaAcoes[i].AcaoId).Valor_Abertura} | Desvio: {mediaAcoes[i].Desvio}%");
        }

        return mediaAcoes.OrderByDescending(x=>x.Desvio).Take(qtdAcoesEscolhidas).ToList();
    }

    //Tabela carteira_acao
    //Método para atualizar a carteira todos os dias, irá manter as ações resultado da MediaAcao que ja possui hoje na carteira e irá trocar aquelas que não saíram no resultado da MediaAcao de hoje
    public async Task AtualizaCarteira(List<MediaAcoes> melhoresAcoes){

        //INDO NO BANCO, VERIFICANDO SE TEM CARTEIRA CADASTRADA E TRAZENDO ELA
        var carteira = await _context.Carteira.FirstOrDefaultAsync();
        var hoje = DateTime.Now;
        //Se a carteira estiver com dados, cria uma lista para armazenar os Id's das ações existentes
        if (carteira != null){
           List<int> acoesId = new();

            //Adicionando o ID das melhores ações na lista acoesId
            foreach(var melhorAcao in melhoresAcoes){
                acoesId.Add(melhorAcao.AcaoId);
            }
            //Comparando com as ações que ja tenho na carteira para nao inserir duplicado
            var acoesCompra = melhoresAcoes.ExceptBy(_context.CarteiraAcao.Where(x=> x.Qtd_cotas > 0).Select(x=> x.Acao_id).ToList(), x=> x.AcaoId);
            //Guardando numa variável as ações resultado de MelhorAcao que são iguais as ações que ja tenho na minha carteira atual
            var acoesIguais = await _context.CarteiraAcao.Where(x=> acoesId.Contains(x.Acao_id)).ToListAsync();
            //Identificando as ações em que a QtdCotas é maior que 0, ou seja, as que poderão ser vendidas
            var acoesVender =  await _context.CarteiraAcao.Where(x=> x.Qtd_cotas > 0).ToListAsync();
            //Exceto as ações da interseção(acoesIguais). 
            acoesVender = acoesVender.ExceptBy(acoesIguais.Select(x => x.Acao_id), x=> x.Acao_id).ToList();

            //Venda das ações 
            foreach (var acaoVender in acoesVender){
                acaoVender.Qtd_cotas = 0;
                _context.CarteiraAcao.Update(acaoVender);
                await _context.SaveChangesAsync();
                //PEGANDO TODO MUNDO QUE TEM O VALOR VENDA NULO. 
                var historicoCarteira = await _context.HistoricoCarteira.Where(x => x.Acao_id == acaoVender.Acao_id && x.Valor_Venda == null).FirstOrDefaultAsync();
                var valorVenda = (await _context.Valores.Where(x=> x.Data.Date == DateTime.Today.Date && x.Acao_id == acaoVender.Acao_id).FirstOrDefaultAsync()).Valor_Abertura;
                //vendendo todas as ações cujo o valor de venda anterior era Null
                historicoCarteira.Valor_Venda = valorVenda;
                _context.HistoricoCarteira.Update(historicoCarteira);
                await _context.SaveChangesAsync();


            }
            //Compra das ações (Atribuição  na tabela de Carteira Acao)
            foreach(var acaoCompra in acoesCompra){

                var valorCompra = (await _context.Valores.Where(x=> x.Data.Date == DateTime.Today.Date && x.Acao_id == acaoCompra.AcaoId).FirstOrDefaultAsync()).Valor_Abertura;
               
                var carteiraAcao = await _context.CarteiraAcao.Where(x=> x.Acao_id == acaoCompra.AcaoId).FirstOrDefaultAsync();
                
                if(carteiraAcao != null){
                    carteiraAcao.Qtd_cotas = 3;
                    _context.CarteiraAcao.Update(carteiraAcao);
                    await _context.SaveChangesAsync();

                }
                else{

                    carteiraAcao = new CarteiraAcao(){
                        Carteira_id = carteira.Id,
                        Acao_id = acaoCompra.AcaoId,
                        Qtd_cotas = 3

                    };
                    _context.CarteiraAcao.Add(carteiraAcao);
                    await _context.SaveChangesAsync();

                }

                HistoricoCarteira historicoCarteira = new HistoricoCarteira(){
                    Carteira_id = carteira.Id,
                    Acao_id = acaoCompra.AcaoId,
                    Data_historico = hoje,
                    Valor_Compra = valorCompra,
                };

                _context.HistoricoCarteira.Add(historicoCarteira);

                await _context.SaveChangesAsync();
            }

            foreach (var acaoIgual in acoesIguais)
            {
                var historicoCarteira = await _context.HistoricoCarteira.Where(x => x.Acao_id == acaoIgual.Acao_id && x.Valor_Venda == null).FirstOrDefaultAsync();
                historicoCarteira.Valor_Venda = 0;
                _context.HistoricoCarteira.Update(historicoCarteira);
                await _context.SaveChangesAsync();

                _context.HistoricoCarteira.Add(new HistoricoCarteira() {
                    Carteira_id = historicoCarteira.Carteira_id,
                    Acao_id = historicoCarteira.Acao_id,
                    Data_historico = hoje,
                    Valor_Compra = historicoCarteira.Valor_Compra
                });

                await _context.SaveChangesAsync();
            }
        }

        else{
            //Criando uma nova carteira
            carteira = new Carteira();
            _context.Carteira.Add(carteira);
            await _context.SaveChangesAsync();

            //Pegando as melhores ações da lista de Melhores ações
            foreach(var acao in melhoresAcoes)
            {
                //Inserindo aa 5 melhores açoes na carteira
                CarteiraAcao carteiraAcao = new(){
                    Carteira_id = carteira.Id,
                    Acao_id = acao.AcaoId,
                    Qtd_cotas = 3
                };

                _context.CarteiraAcao.Add(carteiraAcao);
                await _context.SaveChangesAsync();
            }
            await InsercaoHistorico(carteira.Id);
        }

    }
    //Tabela historico_carteira
    public async Task InsercaoHistorico(int carteiraId){
        
        List<CarteiraAcao> carteirasAcoes = await _context.CarteiraAcao.Where(x=> x.Carteira_id == carteiraId).ToListAsync();
        
        //rodando foreach nas melhores ações a serem investidas
        foreach(var acao in carteirasAcoes){
            Valores historicoAcoes = await _context.Valores.Where(x => x.Data.Date == DateTime.Today.Date && x.Acao_id == acao.Acao_id).FirstOrDefaultAsync();
            
            HistoricoCarteira historicoCarteira = new(){
                Carteira_id = carteiraId,
                Acao_id = acao.Acao_id,
                Data_historico = historicoAcoes.Data,
                Valor_Compra = historicoAcoes.Valor_Abertura
            };

            _context.HistoricoCarteira.Add(historicoCarteira);
            await _context.SaveChangesAsync();
        }
        
    }

   public async Task<RetornoDashboardAPI> RetornoDadosDashboard()
    {
        List<HistoricoCarteira> historicoCarteira = await _context.HistoricoCarteira.ToListAsync();
        var datasHistorico = from historico in historicoCarteira
                                    group historico by historico.Data_historico into novoHistorico
                                    orderby novoHistorico.Key
                                    select novoHistorico;
        
        List<HistoricoAcoesRetornoAPI> historicoAcoesRetornoAPI = new();
        foreach (var data in datasHistorico)
        {
            Console.WriteLine(data.Key.ToString());
            //Pegando os dados da tabela Historico Carteira
            var acoesCarteira = await _context.HistoricoCarteira.Where(x => x.Data_historico.Date == data.Key.Date).ToListAsync();

            List<AcoesRetornoAPI> acoesRetornoAPI = new();

            decimal valorCarteira = 0;
            decimal retornoDiario = 0;

            foreach (var i in acoesCarteira)
            {
                //Pegando os dados da tabela acao
                var acaoRetorno = await _context.Acoes.Where(x=> x.Id == i.Acao_id).FirstOrDefaultAsync();
                //Pegando os dados da tabela de carteira_acao
                var carteiraRetorno = await _context.CarteiraAcao.Where(x=> x.Acao_id == i.Acao_id).FirstOrDefaultAsync();
                //Pegando os dados da tabela de Vvlores
                var valoresRetorno = await _context.Valores.Where(x=> x.Acao_id == i.Acao_id && x.Data.Date == i.Data_historico.Date).FirstOrDefaultAsync();
                acoesRetornoAPI.Add(new AcoesRetornoAPI{
                    Acao = acaoRetorno.Nome,
                    Sigla = acaoRetorno.Sigla,
                    ValorCota = valoresRetorno.Valor_Abertura,
                    ValorInvestido = valoresRetorno.Valor_Abertura * carteiraRetorno.Qtd_cotas,
                    RetornoDiario = (valoresRetorno.Valor_Abertura - valoresRetorno.Valor_Fechamento) * carteiraRetorno.Qtd_cotas
                });   
                //Somando o valor da minha carteira no dia.
                valorCarteira = (i.Valor_Compra * carteiraRetorno.Qtd_cotas) + valorCarteira;
                retornoDiario = (i.Valor_Compra - valoresRetorno.Valor_Fechamento) * 3 + retornoDiario;
            }
            
            historicoAcoesRetornoAPI.Add(new HistoricoAcoesRetornoAPI {
                Data = data.Key,
                ValorCarteira = valorCarteira,
                RetornoDiario = retornoDiario,
                Acoes = acoesRetornoAPI
            });
        }
        Console.WriteLine(historicoAcoesRetornoAPI);
        return new RetornoDashboardAPI() {
            HistoricoAcoes = historicoAcoesRetornoAPI
        };
    }

    public async Task<List<RetornoAcoesVendidas>> AcoesVendidas(){

        var result = await _context.Acoes
        .Join(_context.HistoricoCarteira,
                a => a.Id,
                h => h.Acao_id,
                (a, h) => new { a, h })
        .Where(joined => joined.h.Valor_Venda != null && joined.h.Valor_Venda != 0)
        .Select(joined => new RetornoAcoesVendidas
        {
            Nome = joined.a.Nome,
            ValorVenda = joined.h.Valor_Venda,
            ValorCompra = joined.h.Valor_Compra,
        })
        .ToListAsync();

    return result;

    }
}

//Rotina

