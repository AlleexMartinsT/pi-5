using System;
using pi5.entities;

namespace pi5.Interfaces.Services;

//Interface criada para obrigar o usuario a utilizar o método dela. 
public interface IAcoesService{

    //Criando assinatura do método que iremos utilizar para atualizar os dados
    public Task AtualizaDados();
    public Task HistoricoFechamento();

    public Task<List<MediaAcoes>> GetMediaMovel(DateTime dataInicio, DateTime dataFim);   

//Retorna o ID, Media movel e desvio das melhores açoes
    public Task<List<MediaAcoes>> MelhorAcao(List<MediaAcoes> mediaAcoes, int qtdAcoesEscolhidas);

    public Task<int> CadastroCarteira(List<MediaAcoes> melhoresAcoes);

    public Task InsercaoHistorico(int carteiraId, DateTime dataInicio, DateTime dataFim);
    // public Task<RetornoDashboardAPI> RetornoDadosDashboard(int carteiraId);
}

  

    



