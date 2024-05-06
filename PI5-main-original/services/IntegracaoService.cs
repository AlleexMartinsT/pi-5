using System;
using System.Net;
using System.Text.Json;
using pi5.database;
using pi5.entities;
using pi5.Interfaces.Services;
using PI5.entities;

namespace pi5.services;

public class IntegracaoService:IIntegracaoService{
        //iconfiguration pega as informações do app settings(TOKEN e URL)
    private readonly IConfiguration _configuration;

    //Construtor da classe 
    public IntegracaoService (IConfiguration configuration){
        _configuration = configuration;
    }


    public async Task<RetornoAPI> GetDados(string caminho, List<string> acoes){

        //Indicando que estamos pegando uma string no arquivo correspondente e jogando para a variável token
        string token = _configuration.GetValue<String>("token");
        //Indicando que estamos pegando uma string no arquivo correspondente e jogando para a variável url_padrao
        string url_padrao = _configuration.GetValue<String>("URL_PADRAO");
        string acoesTexto = "";
        
        // Rodando um for em cima da lista de açoes
        for (int i = 0; i < acoes.Count; i++) 
        {
            if(i == acoes.Count-1)
            {
                // Caso meu iterador seja o último item da lista
                // Concateno apenas o nome da ação
                acoesTexto += acoes[i];
            }
            else
            {
                // Caso não seja
                // Concateno nome da ação + ","
                acoesTexto += acoes[i] + ",";
            }
        }

        //Montando a URL
        url_padrao = url_padrao + caminho + '/' + acoesTexto + "?range=1y&interval=1d&fundamental=true&token=" + token;
        //Objeto responsável por fazer a requisição
        HttpClient httpClient = new();
        //retorno vindo na variável response
        var response = await httpClient.GetAsync(url_padrao);
        //Pegando o conteúdo do retorno da API e lendo como string
        var retorno = await response.Content.ReadAsStringAsync();
        //Pegando os dados do json na API e inserindo em string na classe RetornoAPI
        var dados = JsonSerializer.Deserialize<RetornoAPI>(retorno);
        return dados;
    }

    public async Task<List<string>> TodasAcoes(){
        
        string url_padrao = _configuration.GetValue<String>("URL_PADRAO") + "available";

        HttpClient httpClient = new();
        //retorno vindo na variável response
        var response = await httpClient.GetAsync(url_padrao);
        //Pegando o conteúdo do retorno da API e lendo como string
        var retorno = await response.Content.ReadAsStringAsync();
        //Pegando os dados do json na API e inserindo em string na classe RetornoAPI
        var dados = JsonSerializer.Deserialize<TodasAcoes>(retorno);
        return dados.Stocks;


    }

}