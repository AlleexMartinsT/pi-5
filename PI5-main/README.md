# Pastas criadas:


+ Controllers:
  + Pasta **AcoesController**: Cria as rotas http que serão utilizadas. 
+ Database:
  + **PI5Context**: Pasta em que é feito a conexão no banco, nela está informando que as variáveis "Acoes" e "Valores" correspondem às tabelas no banco.
        Toda vez que chamar o Context.Acoes/Valores em alguma classe, irá chamar a tabela no banco. 
+ Entities:
  + **Acoes**: Cria a classe com os atributos da variável Acoes que corresponde a tabela de acoes no banco, conforme descrito na classe PI5Context.
  + **Valores**: Cria a classe com os atributos da variável Valores que corresponde a tabela de valores no banco, conforme descrito na classe PI5Context.
  + **RetornoAPI**: Classe que diz quais as informações do json iremos pegar e cria uma variável para cada informação. 
+ Interfaces:
  + **IAcoesService**: Interface que cria a classe AtualizaDados responsável por fazer a operação de atualização e inserção das açoes no banco. 
  + **IIntegracaoService**: Interface que cria a classe GetDados responsável por pegar o retorno da API que está no link parametrizado. CLasse permite inserir o caminho('quote') e o nome da ação que o usuário deseja inserir no banco. (parametrização ocorre direto na clase 'AcoesService').
+ Services:
  + **AcoesService**: Classe que tem o método 'AtualizaDados' que insere dados nas tabelas de Acoes e VAlores no banco. 
  + **IntegracaoService**: Classe de integração que pega as informações do Appsettings(token, url, conexão com o banco) através do IConfiguration. Cria o link que será acessado por meio do método GET
