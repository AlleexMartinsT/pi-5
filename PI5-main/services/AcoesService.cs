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
        public async Task AtualizaDados()
        {
            try{
                // Solicitar todas as ações disponíveis na API
                RetornoAPI retornoDados = await _integracaoService.GetDados("quote", new List<string> { "*" });

                if (retornoDados == null || retornoDados.Results == null){
                    Console.WriteLine("Os dados de retorno da API estão ausentes ou vazios.");
                    return;
                }

                // verificando se cada ação já está no banco de dados
                foreach (var acao in retornoDados.Results){
                    // verificar se o nome da ação está presente e não é nulo
                    if (string.IsNullOrEmpty(acao.LongName)){
                        Console.WriteLine("O nome da ação está ausente ou vazio.");
                        continue;
                    }

                    // Verificar se a lista de histórico de preço está presente e não é nula
                    if (acao.HistoricalDataPrice == null || !acao.HistoricalDataPrice.Any()){
                        Console.WriteLine($"Não há dados de histórico de preço disponíveis para a ação {acao.LongName}.");
                        continue;
                    }

                    var acaoBanco = _context.Acoes.FirstOrDefault(x => x.Nome == acao.LongName);
                    if (acaoBanco == null){
                        // Se a ação não existe no banco, adicioná-la
                        acaoBanco = new Acoes(){
                            Nome = acao.LongName,
                            Logo = acao.Logourl,
                            Sigla = acao.Symbol,
                        };
                        _context.Acoes.Add(acaoBanco);
                        await _context.SaveChangesAsync();
                    }

                    // atualizando os valores históricos da ação no banco de dados
                    foreach (var valoresAPI in acao.HistoricalDataPrice){
                        DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(valoresAPI.Data);
                        // Verificar se já existe um valor histórico com a mesma data
                        var valorExistente = await _context.Valores.FirstOrDefaultAsync(x => x.Acao_id == acaoBanco.Id && x.Data == dateTime.LocalDateTime);
                        if (valorExistente == null){
                            // Se não existir, adicionar o novo valor histórico
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
            catch (Exception ex)
            {
                // Lidar com exceções que possam ocorrer durante o processo de atualização
                Console.WriteLine($"Ocorreu uma exceção durante a atualização dos dados: {ex.Message}");
                // Você pode registrar a exceção em logs ou tomar outras medidas adequadas
            }
        }


        public async Task<decimal> SimularGanhosPerdas(string nomeAcao)
    {
        // Recuperar os valores relevantes da ação do banco de dados
        var acao = await _context.Acoes.FirstOrDefaultAsync(a => a.Nome == nomeAcao);

        if (acao == null)
        {
            throw new ArgumentException($"Ação com nome '{nomeAcao}' não encontrada.");
        }

        // Recuperar os valores associados à ação
        var valores = await _context.Valores
            .Where(v => v.Acao_id == acao.Id)
            .OrderByDescending(v => v.Data)
            .FirstOrDefaultAsync();

        if (valores == null)
        {
            throw new InvalidOperationException($"Não foram encontrados valores para a ação '{nomeAcao}'.");
        }

        var precoCompra = valores.PrecoCompra;
        var precoVenda = valores.PrecoVenda;
        var quantidadeAcoes = valores.QuantidadeAcoes;

        var ganhosPerdas = (precoVenda - precoCompra) * quantidadeAcoes;

        return ganhosPerdas;
    }
        public async Task MelhorAcao() {
            
        }

    }