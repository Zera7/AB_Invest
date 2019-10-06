using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AB_invest.Models;
using Microsoft.EntityFrameworkCore;

namespace AB_invest
{
    public class Context : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<UserData> UserData { get; set; }
        public DbSet<CompanyData> CompanyData { get; set; }
        public DbSet<ShareDeal> ShareDeal { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<CurrencyDeal> CurrencyDeal { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<ShareBalance> ShareBalances { get; set; }
        public DbSet<CurrencyBalance> CurrencyBalances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShareBalance>()
                .HasOne(p => p.Account)
                .WithMany(t => t.Shares)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
} 
