using Microsoft.EntityFrameworkCore;
using SSSSProject.Data;
using Service;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SSSSProjectContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("SSSSProject")));

builder.Services.AddTransient<IMasterService,MasterService >();
builder.Services.AddScoped<IMasterService, MasterService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
