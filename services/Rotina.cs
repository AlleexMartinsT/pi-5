
using pi5.Interfaces.Services;
namespace pi5.services;

public class Rotina : BackgroundService{

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Rotina (IServiceScopeFactory serviceScopeFactory) {
        _serviceScopeFactory = serviceScopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken){

        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IAcoesService>();

        while (!stoppingToken.IsCancellationRequested){

            TimeSpan rodarRotina = new TimeSpan(10, 10, 00); //Hora que quero que minha rotina rode
            TimeSpan agora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
             //Console.WriteLine($"{rodarRotina} / {agora}");
            if(rodarRotina == agora){
                 await service.AtualizaDados();
                 await service.HistoricoFechamento();

            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}