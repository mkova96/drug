using DrugData.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace DrugData
{
    public class ApplicationDbContext: IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        
        public DbSet<Medication> Drug { get; set; }
        public DbSet<Disease> Disease { get; set; }
        public DbSet<DrugDisease> DrugDisease { get; set; }
        public DbSet<Manufacturer> Manufacturer { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<SideEffect> SideEffect { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Doctor> Doctor { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<DrugSideEffect> DrugSideEffect { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<DrugCart> DrugCart { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Package> Package { get; set; }
        public DbSet<Currency> Currency { get; set; }


    }





}

