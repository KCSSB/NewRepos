using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace DataBaseInfo
{
    internal class TemporaryDbContextFabric
    {


        public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
        {

            public AppDbContext CreateDbContext(string[] args)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()) // Ищем appsettings.json в текущей папке
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = config.GetConnectionString("DefaultConnection");

                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseNpgsql(connectionString);

                return new AppDbContext(optionsBuilder.Options);
            }

        }
    }
}
