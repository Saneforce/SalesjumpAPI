using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy1",
        policy =>
        {
            policy.WithOrigins("http://dev.sanfmcg.com", "http://devapi.sanfmcg.com",
                                "http://*.salesjump.in")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });

});


builder.Services.AddControllers();
//builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "FMCG API",
        Description = "An ASP.NET Core Web API for managing FMCG items",
        //TermsOfService = new Uri("https://example.com/terms"),
                
        //Contact = new OpenApiContact
        //{
        //    Name = "Example Contact",
        //    Url = new Uri("https://example.com/contact")
        //},
        //License = new OpenApiLicense
        //{
        //    Name = "Example License",
        //    Url = new Uri("https://example.com/license")
        //}
    });
});
var app = builder.Build();


app.UseRouting();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });    
}


//app.UseEndpoints(endpoints =>
//{
//    //endpoints.MapHub<EventsHub>("/events");
//    endpoints.MapControllers();
//    //endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
//    //{
//    //    Authorization = new[] { new AllowAnyoneAuthorizationFilter() },
//    //    IsReadOnlyFunc = _ => true
//    //});
//    endpoints.MapFallbackToFile("index.html");
//});


app.MapGet("/swagger/v1/swagger.json", () => "Swagger API V1");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors();

app.Run();
