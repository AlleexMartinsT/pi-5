using System;

namespace pi5.Interfaces.Services;

//Interface criada para obrigar o usuario a utilizar o método dela. 
public interface IAcoesService{

    //Criando assinatura do método que iremos utilizar para atualizar os dados
    public Task AtualizaDados();
    public Task MelhorAcao();
    Task<decimal> SimularGanhosPerdas(string nomeAcao);
}