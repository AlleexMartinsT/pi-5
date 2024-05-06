using System;
using Microsoft.EntityFrameworkCore;
using pi5.entities;
//Configurando a conexao com o banco
namespace pi5.database;

public class PI5Context:DbContext{

    //construtor
    public PI5Context(DbContextOptions<PI5Context> options): base(options){}
    //Toda vez que chamar o Context.Acoes/Valores, irá chamar a tabela no banco. 
    public DbSet<Acoes> Acoes{get;set;}
    public DbSet<Valores> Valores{get;set;}
    public DbSet<CarteiraAcao> CarteiraAcao {get;set;}
    public DbSet<Carteira> Carteira {get;set;}
    public DbSet<HistoricoCarteira> HistoricoCarteira {get;set;}



}