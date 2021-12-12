using CoreUrlShortner.Data;
using CoreUrlShortner.Extentions;
using CoreUrlShortner.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDeveloperExceptionPage();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // using static System.Net.Mime.MediaTypeNames;
            context.Response.ContentType = Text.Plain;

            await context.Response.WriteAsync("An exception was thrown.");

            var exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
            {
                await context.Response.WriteAsync(" The file was not found.");
            }

            if (exceptionHandlerPathFeature?.Path == "/")
            {
                await context.Response.WriteAsync(" Page: Home.");
            }
        });
    });

    app.UseHsts();
}

app.MapGet("/{code}", async (string code, AppDbContext ctx, HttpContext httpContext, IConfiguration config) => {
    if (!string.IsNullOrWhiteSpace(code))
    {
        var link = await ctx.Links.FirstOrDefaultAsync(x => x.code == code);
        if (link != null)
        {
            var baseUrl = config["appUrl"];
            return Results.Ok(new { shortLink=baseUrl+link.code, longUrl=link.LongUrl});
        }
    }
    return Results.BadRequest("URL is not valid");

});

app.MapPost("/", async (string url, AppDbContext ctx, IConfiguration config) =>
{
    if (url.CheckURLValid() && await url.CheckUrlExitAsync())
    {
        var baseUrl = config["appUrl"];
        var code  = Nanoid.Nanoid.Generate(size: 7);
       var shortUrl = baseUrl + code;

        var link = new Link { code = code, LongUrl = url };

        try
        {
            ctx.Links.Add(link);
            ctx.SaveChanges();
            return Results.Ok(shortUrl);
        } // incase of Nanoid generate same Id (it has low chance bit there is chance)
        catch (Exception ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 2601))
        {
            code = Nanoid.Nanoid.Generate(size: 7);
            link.code = code;
            shortUrl = baseUrl + code;
            ctx.Links.Add(link);
            ctx.SaveChanges();
            return Results.Ok(shortUrl);

        }

    }
    return Results.BadRequest("URL is not valid");

});

app.Run();



