using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlogPhone.Models.LogViewer.Database
{
    public class LogContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<LogString> LogStrings { get; set; } = null!;

        public LogContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=62.109.29.6;Port=32968;Database=main;Username=admin;Password=w0HMCVFq1");
        }
    }
}
