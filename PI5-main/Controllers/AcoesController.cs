using System;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("melhor-acao")]
    public Task MelhorAcao() {
        return _acoesService.MelhorAcao();
    }

}