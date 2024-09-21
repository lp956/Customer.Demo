
using Yitter.IdGenerator;

namespace CustomerManage.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<Program>());

            var epoch = new DateTime(2024, 1, 23, 0, 0, 0, DateTimeKind.Utc);
            //builder.Services.AddIdGen(123, () => new IdGen.IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch)));
            var options = new IdGeneratorOptions() { BaseTime = epoch };
            YitIdHelper.SetIdGenerator(options);

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
        }
    }
}
