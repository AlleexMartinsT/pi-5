using System;
using Microsoft.AspNetCore.Mvc;
using pi5.entities;
using pi5.Interfaces.Services;

namespace pi5.Controllers;

[ApiController]
[Route("api/[Controller]")]

public class AcoesController:ControllerBase{

    private readonly IAcoesService _acoesService;
    public AcoesController (IAcoesService acoesService){
        _acoesService = acoesService;
    }

    //criando rota de teste
    [HttpGet("teste")]
    public string Teste() {
        return "Ei";

    }
    //Rota para atualizar os dados das ações
    [HttpPost("atualizar-dados")]
    public Task AtualizarDados() {
        return _acoesService.AtualizaDados();
    }

    [HttpGet("historico-fechamento")]
    public Task HistoricoFechamento() {
        return _acoesService.HistoricoFechamento();
    }

    [HttpGet("retorno-dados-dashboard")]
    public async Task<RetornoDashboardAPI> RetornoDadosDashboard(){
        
        return await _acoesService.RetornoDadosDashboard();

    }

    [HttpGet("retorno-acoes-vendidas")]
    public async Task<List<RetornoAcoesVendidas>> AcoesVendidas(){
        
        return await _acoesService.AcoesVendidas();

    }


}