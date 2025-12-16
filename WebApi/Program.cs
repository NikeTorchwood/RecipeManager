using System.Data;
using Application.Services.Implementations.MappingProfiles;
using Application.Validators;
using Dapper;
using DapperRepositories.NpgSql;
using DapperRepositories.NpgSql.Migrations;
using FluentMigrator.Runner;
using FluentValidation;
using FluentValidation.AspNetCore;
using Npgsql;
using WebApi.Extensions;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var configuration = builder.Configuration;

DefaultTypeMap.MatchNamesWithUnderscores = true;
var connectionString = configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(connectionString));

services.AddAutoMapper(typeof(RecipeProfile).Assembly);
services.AddApplicationLogic();

services.AddFluentMigratorCore()
    .ConfigureRunner(x => x
        .AddPostgres()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(InitialMigration).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

services.AddCors(opt =>
{
    opt.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

services.AddFluentValidationAutoValidation();
services.AddValidatorsFromAssemblyContaining<RecipeCreateDtoValidator>();

services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowVueApp");
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

DatabaseManager.EnsureDatabase(connectionString, "Recipes"); // по идее можно так же из конфиги доставать
DatabaseManager.MigrateUp(app);

app.Run();