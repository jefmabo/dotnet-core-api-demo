# Demo web api .net Core

O entendimento da utilização do dotnet CLI tem se tornado importante por facilitar a automação de processos dentro das empresas, com base nesta observação vamos criar uma aplicação utilizando-se desta ferramenta.

Este projeto tem como objetivo a criação de uma aplicação web api de demonstração que possa ser executada tanto no VsCode quanto no tradicional Visual Studio, os passos abaixo descrevem um tutorial usando o dotnet CLI para criação desta aplicação.

## 1. Extensões VsCode
Caso opte por utilizar o VsCode recomendo a intalação de algumas extensões, são elas:

- [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp): Esta extensão nos permitirá realizar o debug, usar funcionalidades como 'Go to definition', etc.

- [C# Extensions](https://marketplace.visualstudio.com/items?itemName=jchannon.csharpextensions): Este extensão facilita a criação de classes e interfaces, criação de construtores, etc.

- [Visual Studio IntelliCode](https://marketplace.visualstudio.com/items?itemName=VisualStudioExptTeam.vscodeintellicode): Esta extensão utiliza inteligencia artificial para facilitar o auto completar, tem suporte a diversas linguagens inclusive C#.

- [TabNine](https://marketplace.visualstudio.com/items?itemName=TabNine.tabnine-vscode): Esta é uma extensão auternativa à Visual Studio IntelliCode, o objetivo também é auto completar.


## 2. Criando a solution

O arquivo .sln é o arquivo base que armazena todos os projetos para que o Visual Studio possa carregar corretamente, como temos o objetivo de executar a aplicação tanto no VsCode quanto no Visual Studio, este arquivo se faz extremamente necessário, para criá-lo primeiro vamos criar uma pasta pasta com nome do app. Em seguida vamos executar o comando abaixo sendo que depois do parametro `--name` deve ser informado o nome da sua solution.

```
dotnet new sln --name api-demo
```

## 3. Configurando launch.json
Para esta execução no VsCode é necessário criar o arquivo `launch.json` na pasta `.vscode`. o arquivo deve ficar conforme abaixo:

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/Pets.Api/bin/Debug/netcoreapp3.1/Pets.Api.dll",
            "args": [],
            "cwd": "${workspaceFolder}/Pets.Api/bin/Debug/netcoreapp3.1/",
            "stopAtEntry": false,
            "serverReadyAction": {
                "action": "openExternally",
                "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
            },
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            },
            "sourceFileMap": {
                "/Views": "${workspaceFolder}/Views"
            }
        },
        {
            "name": ".NET Core Attach",
            "type": "coreclr",
            "request": "attach",
            "processId": "${command:pickProcess}"
        }
    ]
}
```

Neste arquivo temos duas chaves importantes a serem tratadas uma é a `program` na qual iremos colocar o caminho da dll que será o ponto de partida para execução do debug.

A segunda chave é `cwd`, nela é necessário que seja referenciado o caminho em que os arquivos serão disponibilizados, pois caso queiramos configurar algo customizado no arquivo `appsettings.json`, para recuperarmos no código utilizando o VsCode o `cwd` deve estar corretamente apontando para pasta de arquivos compilados.

## 4. Criando os projetos Pets.Api

Agora iremos criar os projetos que serão utilizados para execucao da aplicação, nesta demo não usei um padrão arquitetural específico pois o objetivo principal é a utilização da linha de comando para criação de aplicações Net Core.

O comando abaixo irá criar um projeto do tipo webapi:

```
dotnet new webapi --name Pets.Api
```

Os projetos WebApi já vêm com um endpoint padrão, neste ponto já é possível testar a execução da api. 

O projeto criado estará disponibilizado da seguinte forma:

- /Controllers: contém classes com métodos públicos que serão expostos como endpoints HTTP;
- Program.cs: Contém o método principal do ponto de entrada da aplicação. Basicamente a execução inicia-se nesta classe;
- ContosoPets.Api.csproj: Contém a configuração dos metadados para o projeto;
- Startup.cs: Configura os servicos, injeções de dependencia e a pilha das execuções HTTP da aplicação. Nesta classe também é onde são configurados os middlewares da aplicação.

Para configurar o banco de dados via injeção de depêndencia, é necessário instalação da biblioteca `Microsoft.EntityFrameworkCore.InMemory` neste projeto. Para isto iremos executar o comando abaixo:

```
dotnet add Pets.Api/Pets.Api.csproj package Microsoft.EntityFrameworkCore.InMemory
```

Criaremos o controller `ProductsController` que servirá como nosso endpoint `/products`, nele criaremos um método get que retornará uma lista com todos os produtos.

A classe controller ficará como abaixo:

```c#
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly Context _context;

    public ProductsController(Context context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<List<Product>> GetAll() => Ok(_context.Products.ToList());

}
```

A anotação `ApiController` sinalisa que esta classe será um ponto de requisição, já a anotação `Route("[controller"])` identifica que o sufixo de nossa classe `ProductsController` será nosso endpoint, ou seja `/products`.

Nesta classe queria apresentar um recurso da linguagem chamado `expression body`, o método `GetAll()` possui apenas uma instrução, neste caso podemos utilizar este recurso e deixar a criação e retorno em apenas uma linha.

### 4.1. Injeção de dependência
O .net Core trabalha a injeção de dependência por meio da biblioteca `Microsoft.Extensions.DependencyInjection`, por padrão quando criados projetos do tipo WebApi esta dependência já é instalada junto com o pacote.

As injeções são configuradas dentro do arquivo `Startup.cs`, neste exemplo ele se encontra dentro da pasta `/Pets.Api`. Dentro da classe temos o método `ConfigureServices` utilizado especificamente para tratar dependências como banco de dados e injeções, por critério de organização vamos criar uma extensão da interface IServiceCollection que será responsável unicamente por injetar as classes do nosso projeto.

Uma extensão (ou Extension) trata-se de uma expansão de uma classe ou interface já existente, quando criado um método de extensão podemos chamá-lo na classe original como se este fosse nativo, no trecho abaixo temos o código para essa extensão.

```c#
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddLocalServices(this IServiceCollection services) 
    {

        services.AddTransient<IProductService, ProductService>();

        services.AddTransient<IProductRepository, ProductRepository>();

        return services;
    }
}
```

Devemos observar alguns pontos importantes ao criar uma extensão, tanto a classe quanto o método deve ser obrigatóriamente estáticos e a classe ou interface que deseja extender deve ir na assinatura do método com a palavra reservada `this`.

Agora sempre que for criado um novo service ou repository ele deverá ser injetado no método `AddLocalServices` da classe `ServiceCollectionExtension`.

Esta classe acima foi criada no arquivo `Pets.Api/Extension`, esta pasta não vem por padrão, portanto deve ser criada.

Agora iremos utilizar o método `AddLocalServices` na classe `Startup.cs`, para isso basta importar o namespace com `using Pets.Api.Extension;` e o método `Configuration` ficará conforme abaixo:

```c#
public void ConfigureServices(IServiceCollection services)
{
    // Utilizando extensão criada.
    services.AddLocalServices();
    services.AddDbContext<Context>(options => options.UseInMemoryDatabase("Pets"));
    services.AddControllers();
}
```

## 5. Criando projeto Pets.Model

Este é o projeto mais simples da solução, nele teremos as classes que representarão nossas entidades no banco de dados.

Criando projeto Model:
```
dotnet new classlib --name Pets.Model
```

Para a solução que estamos criando, teremos apenas a classe `Product`.

```c#
public class Product
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public decimal Price { get; set; }
}
```

## 6. Criando projeto Pets.Service
Teremos um projeto específico para tratar regras de negócio, em nosso exemplo é o projeto `Pets.Services`, nele teremos uma pasta `/Interface` onde ficarão nossas interfaces de service, neste exemplo teremos apenas a interface `IProductService`. 

Criando projeto Service:
```
dotnet new classlib --name Pets.Service
```

Dentro do projeto teremos as classes como a classe `ProductService` abaixo:

```c#
public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;

    public ProductService(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    public IList<Product> GetAll() => productRepository.GetAll();
    
}
```

A classe será responsável por permitir a comunicação entre os controllers e os repositories.

Para essa comunicação iremos criar uma propriedade do tipo `IProductRepository` e no construtor passamos também um objeto deste mesmo tipo. Você não precisará se preocupar com instâncias destes objetos pois elas serão gerenciadas pelo próprio netCore se configurado conforme item 4.1.

## 7. Criando projeto Pets.Repository
No repository é onde iremos criar as classes que farão comunicação com o banco de dados.

Criando projeto Repository:
```
dotnet new classlib --name Pets.Repository
```

Iremos adicionar neste projeto um banco de dados em memória que será gerenciado pelo ORM EntityFramework, esta ferramenta facilita a manipulação do banco de dados. 

Para que possamos utilizar esta ferramenta, devemos adiciona a dependência por meio do Nuget, que se trata de um gerenciador de pacotes. Neste projeto em específico a dependência a ser adicionada será o `Microsoft.EntityFrameworkCore.InMemory`, portanto executaremos o comando abaixo que adicionará a dependência no projeto Repository.

```
dotnet add Pets.Repository/Pets.Repository.csproj package Microsoft.EntityFrameworkCore.InMemory
```

Neste projeto também iremos criar uma pasta chamada `Configuration` na qual iremos configurar os mapeamentos de nossas entidades com o Entity Framework.

Dentro da pasta `Configuration` teremos basicamente classes como esta abaixo, elas irão implementar a interface `IEntityTypeConfiguration` que recebe um objeto genérico, para nós os objetos que serão mapeados são os models.

```c#
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

        builder.Property(p => p.Price)
                .IsRequired();            
    }
}
```

Outra classe que será utilizada é a `Context`, esta herdará os atributos da classe `DbContext` do EntityFramework, para cada entidade no banco de dados teremos um `DbSet` que a representará e no método `OnModelCreating` iremos informar ao context nossas configurações para o mapeamento das entidades.

Esta classe ficará como abaixo:

```c#
public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ProductConfiguration());
    }
}
```

Nesta solução em específico, criaremos uma pasta chamada `/Data` e dentro dela criaremos a classe `SeedData` na quale faremos uma carga inicial em nosso banco de dados, caso não queira criá-la fica a seu critério.

```c#
public static class SeedData
{
    public static void Initialize(Context context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product
                {
                    Name = "Squeaky Bone",
                    Price = 20.99m,
                },
                new Product
                {
                    Name = "Knotted Rope",
                    Price = 12.99m,
                }
            );

            context.SaveChanges();
        }
    }
}
```

Se optar por criar a classe acima, é necessário fazer uma adequação na classe `Pets.Api/Program.cs`.

```c#
public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        SeedDatabase(host);
        host.Run();
    }

    private static void SeedDatabase(IHost host)
    {
        var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Context>();

        if (context.Database.EnsureCreated())
        {
            try
            {
                SeedData.Initialize(context);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "A database seeding error occurred.");
            }
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

## 8. Referenciando os projetos

Abaixo iremos referenciar os projetos uns aos outros conforme a necessidade.

```
dotnet add Pets.Api/Pets.Api.csproj reference Pets.Model/Pets.Model.csproj

dotnet add Pets.Api/Pets.Api.csproj reference Pets.Service/Pets.Service.csproj

dotnet add Pets.Api/Pets.Api.csproj reference Pets.Repository/Pets.Repository.csproj

dotnet add Pets.Service/Pets.Service.csproj reference Pets.Repository/Pets.Repository.csproj

dotnet add Pets.Service/Pets.Service.csproj reference Pets.Model/Pets.Model.csproj

dotnet add Pets.Repository/Pets.Repository.csproj reference Pets.Model/Pets.Model.csproj
```

## 9. Adicionando projetos à solution
Agora para correta execução no Visual Studio iremos referenciar os projetos criados ao arquivo .sln.

```
dotnet sln add Pets.Api/Pets.Api.csproj 
dotnet sln add Pets.Model/Pets.Model.csproj
dotnet sln add Pets.Service/Pets.Service.csproj 
dotnet sln add Pets.Repository/Pets.Service.csproj  
```

## 10. Executando o projeto

Finalizado o desenvolvimento agora iremos executá-lo, com o VsCode devidamente configurado basta prescionar `F5`.

Via linha de comando executaremos como abaixo:

```
dotnet run --project Pets.Api/Pets.Api.csproj
```