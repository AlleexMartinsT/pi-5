# Pastas criadas (RF01):


+ Controllers:
  + Pasta **AcoesController**: Cria as rotas http que serão utilizadas. 
+ Database:
  + **PI5Context**: Pasta em que é feito a conexão no banco, nela está informando que as variáveis "Acoes" e "Valores" correspondem às tabelas no banco.
        Toda vez que chamar o Context.Acoes/Valores em alguma classe, irá chamar a tabela no banco. 
+ Entities:
  + **Acoes**: Cria a classe com os atributos da variável Acoes que corresponde a tabela de acoes no banco, conforme descrito na classe PI5Context.
  + **Valores**: Cria a classe com os atributos da variável Valores que corresponde a tabela de valores no banco, conforme descrito na classe PI5Context.
  + **RetornoAPI**: Classe que diz quais as informações do json iremos pegar e cria uma variável para cada informação.
  + **Carteira**: Cria Classe Carteira que irá guardar somente o ID de cada carteira criada.
  + **CarteiraAcao**: Cria classe para guardar as ações que tenho na carteira e a quantidade de cotas para ela.
  + **HistoricoCarteira**: Cria classe para guardar os dados relevantes das açoes que iremos usar para fazer a análise.
  + **MediaAcoes**: Cria classe para guardar a Média Móvel, o ID e o Desvio Padrão de cada ação em relação ao periodo de análise. ((((Listas do tipo da classe são criadas para armazenar os valores correspondentes das ações.
  + **RetornoDashboardAPI**: Retorno json para o front.
+ Interfaces:
  + **IAcoesService**: Interface que cria a classe AtualizaDados responsável por fazer a operação de atualização e inserção das açoes no banco. 
  + **IIntegracaoService**: Interface que cria a classe GetDados responsável por pegar o retorno da API que está no link parametrizado. CLasse permite inserir o caminho('quote') e o nome da ação que o usuário deseja inserir no banco. (parametrização ocorre direto na clase 'AcoesService').
+ Services:
  + **AcoesService**: Classe que tem o método 'AtualizaDados' que insere dados nas tabelas de Acoes e VAlores no banco. 
  + **IntegracaoService**: Classe de integração que pega as informações do Appsettings(token, url, conexão com o banco) através do IConfiguration. Cria o link que será acessado por meio do método GET
 
# RF02.

+ Para a entrega da RF02, será necessário escolher as melhores ações de um grupo de ações baseado nos dados históricos. 
+ No nosso projeto, escolhemos 5 ações e analisamos os dados delas no período do dia 08-04 ao dia 12-04-2024 (5 dias), tiramos a média móvel do Valor_Fechamento de cada uma das 5 ações no período referido (5 dias da semana x 5 ações).
+ Como o objetivo era começar o investimento no dia 15-04, pegamos o valor_Abertura do dia 15-04 e calculamos o desvio padrão do valor_Abertura do dia 15-04 em relação a média móvel que cada ação teve na semana anterior. 
+ Das 5 ações, escolhemos as 3 que tiveram o maior desvio padrão em relação a ao valor de Abertura do dia 15-04, visto que as ações com maior desvio tem uma tendencia maior a crescer em curto prazo. Portanto, a primeira parte da análise está finalizada, pegamos as 3 melhores ações mais tendenciosas de um grupos de 5.
+ Finalizamos nosso investimento no dia 19-04, com os dados de perda e ganho que tivemos em cada dia da semana na nossa carteira.

**Calculo Média Móvel:** soma valor_Fechamento / qtdDias \
**Calulo Desvio Padrão:** 100*Valor_abertura / mediaMovel -100


# RF03

+ Para a entrega da RF03, pegamos a maior quantidade de ações disponível na api, inserimos as ações no banco com o valor de fechamento e abertura.
+ Dessas ações, pedimos para o motor retornar as 5 que tiveram o maior desvio padrão no valor de abertura da ação no dia 22/04, visto que mapeamos a média móvel de todas as ações no periodo de 15/04 a 19/04 e calculamos o desvio padrão em relação ao valor_Abertura de cada ação do banco.
+ Definimos que quanto maior o desvio padrão, mas tendenciosa é a ação de crescer num período curto/curtíssimo.

  ##PENDENCIAS:
  + Criar rotina para executar o codigo todo dia as 10:30 da manhã - Concluido
  + Retornar os dados necessários para o frontEnd. - Concluido
  + Pegar mais ações da API - Concluido
    

  Decisão das ações: \
![Sem título](https://github.com/Nathaliavicb/PI5/assets/104801628/ffd2e024-e9a4-40a3-951a-32bab6e88995)

Variáveis que irão fazer a trativa da imagem acima: \
acoesCompra, acoesIguais, acoesVender.

 
# RF04

Criamos o método RetornoDadosDashboard para retornar os dados para o FRONTEND. 
Antes de realizar o tutorial abaixo, foi preciso inserir o pequeno código abaixo no nosso Program.cs para tratar os erros de CORS. 

app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition"));

# Para rodar o projeto, siga o tutorial abaixo.

# 🖥️ Instalação

1. Clone este repositório
```bash
git clone https://github.com/matheusmatosr/PI5-front.git
```

Abra o terminal e faça os seguintes códigos:

2. Para instalar as dependências

```bash
yarn install
```

3. Para rodar o projeto:

```bash
yarn run start
```

Acesse o link que aparecerá no terminal para obter acesso ao projeto.

