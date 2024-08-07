using Microsoft.EntityFrameworkCore;
using Partify.DataAccess.DbContexts;
using Partify.WebApi.Extensions;
using Partify.WebApi.MapperConfigurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options
	=> options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddMemoryCache();

builder.Services.AddApiServices();

builder.Services.AddServices();

builder.Services.AddValidators();

builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureSwagger();

builder.Services.AddExceptions();

builder.Services.AddJwt(builder.Configuration);

builder.Services.AddProblemDetails();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.AddInjectHelper();

app.AddPathInitializer();

app.UseSwagger();

app.UseSwaggerUI();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();