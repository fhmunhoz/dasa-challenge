using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using buscador.Models;

namespace buscador.Data
{
    public class ScraperDbContext : DbContext
    {

        public ScraperDbContext(DbContextOptions options)
        : base(options)
        { }

        public DbSet<Roupas> Roupas { get; set; }
        public DbSet<RoupasTamanho> RoupasTamanho { get; set; }

    }
}