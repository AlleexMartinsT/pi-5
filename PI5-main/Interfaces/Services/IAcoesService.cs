using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pi5.database;
using pi5.entities;
using pi5.Interfaces.Services;
using PI5.entities;

namespace pi5.services
{
    public class AcoesService : IAcoesService
    {
        private readonly PI5Context _context;
        private readonly IIntegracaoService _integracaoService;

        public AcoesService(PI5Context context, IIntegracaoService integracaoService)
        {
            _context = context;
            _integracaoService = integracaoService;
        }

        public async Task<decimal> calculandorGanhos(string nomeAcao)
        {
            // obtendo os valores de fechamento da ação
            var acao = await _context.Acoes
                .Include(a => a.Valores)
                .FirstOrDefaultAsync(a => a.Nome == nomeAcao);

            if (acao == null)
            {
                throw new ArgumentException($"Ação com nome '{nomeAcao}' não encontrada.");
            }

            // calculando a média móvel dos últimos n dias (ex: 5 dias)
            var qtdDias = 5;
            var valoresFechamento = acao.Valores
                .OrderByDescending(v => v.Data)
                .Take(qtdDias)
                .Select(v => v.Valor_Fechamento)
                .ToList();
            var mediaMovel = valoresFechamento.Sum() / qtdDias;

            // calculando os ganhos como a diferença entre o preço de fechamento mais recente e a média móvel
            var precoFechamento = acao.Valores.OrderByDescending(v => v.Data).FirstOrDefault()?.Valor_Fechamento ?? 0;
            var ganhos = precoFechamento - mediaMovel;

            return ganhos;
        }

        public async Task<decimal> calculandorPerdas(string nomeAcao)
        {
            // Obtenha os valores de fechamento da ação
            var acao = await _context.Acoes
                .Include(a => a.Valores)
                .FirstOrDefaultAsync(a => a.Nome == nomeAcao);

            if (acao == null)
            {
                throw new ArgumentException($"Ação com nome '{nomeAcao}' não encontrada.");
            }

            // calculando a média móvel = soma valor_Fechamento / qtdDias
            var qtdDias = 5;
            var valoresFechamento = acao.Valores
                .OrderByDescending(v => v.Data)
                .Take(qtdDias)
                .Select(v => v.Valor_Fechamento)
                .ToList();
            var mediaMovel = valoresFechamento.Sum() / qtdDias;

            // calculando desvio padrão = 100*Valor_abertura / mediaMovel -100
            var precoAbertura = acao.Valores.OrderByDescending(v => v.Data).FirstOrDefault()?.Valor_Abertura ?? 0;
                var desvioPadrao = 100 * precoAbertura / mediaMovel - 100;

                return desvioPadrao;
        }

            public async Task<List<Acoes>> EscolherMelhoresAcoes()
            {
                // calculando a média móvel para o período especificado
                await calculandorMediaMovel();

                // obtém os valores de abertura das ações para o dia 15-04-2024
                DateTime dataReferencia = new DateTime(2024, 4, 15);
                var valoresAbertura = await _context.Valores
                    .Where(v => v.Data.Date == dataReferencia.Date)
                    .ToListAsync();

                // calculando o desvio padrão para cada ação
                var melhoresAcoes = new List<Acoes>();
                foreach (var acao in Acoes)
                {
                    // recuperando a média móvel da ação
                    decimal mediaMovel = _mediasMoveis[acao.Nome];

                    // calculando o desvio padrão em relação ao valor de abertura
                    decimal desvioPadrao = calculandorDesvioPadrao(acao, valoresAbertura, mediaMovel);

                    // atribui os resultados da análise à entidade Acoes
                    acao.MediaMovel = mediaMovel;
                    acao.DesvioPadrao = desvioPadrao;

                    // adiciona a ação à lista de melhores ações
                    melhoresAcoes.Add(acao);
                }

                // Retorna as 5 melhores ações com base no desvio padrão
                return melhoresAcoes.OrderByDescending(a => a.DesvioPadrao).Take(5).ToList();
            }
 


           public async Task AtualizaDados() 
           // public async Task MelhorAcao() -- acho q nao precisa mais dela
        }
}   
