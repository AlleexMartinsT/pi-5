using System;
using PI5.entities;

namespace pi5.Interfaces.Services;

//Interface criada para obrigar o usuario a utilizar o método dela. 
public interface IIntegracaoService{

    //Criando assinatura do método que iremos utilizar para pegar os dados

    //Utilizamos o task para informar que o método é assincrono
    public Task<RetornoAPI> GetDados(string caminho, List<string> acoes);
    public Task<List<string>> TodasAcoes();

    
}