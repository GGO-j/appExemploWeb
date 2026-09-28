using appExemploWeb.Components;
using appExemploWeb.Configs;
using appExemploWeb.DAO;
using AppWebExemplo.Configs;

var builder = WebApplication.CreateBuilder(args);

// 1. Adiciona serviços do Razor Components / Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Configuração de Injeção de Dependência (DI)
builder.Services.AddScoped<Conexao>();
builder.Services.AddScoped<ProcessoDAO>();

var app = builder.Build();

// 3. Middlewares de Tratamento de Erros e Exceções
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

// 4. Arquivos Estáticos e Segurança
app.UseAntiforgery();
app.MapStaticAssets(); // Em .NET 8, pode usar app.UseStaticFiles(); caso MapStaticAssets não esteja disponível.

// 5. Mapeamento de Roteamento dos Componentes Blazor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();