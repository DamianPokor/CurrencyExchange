using CurrencyExchange.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

app.UseRouting();

app.UseAntiforgery();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();

