﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MarketplaceSession.ADO
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MarketplaceSessionEntities : DbContext
    {
        private static MarketplaceSessionEntities _context;

        public MarketplaceSessionEntities()
            : base("name=MarketplaceSessionEntities")
        {
        }

        private static MarketplaceSessionEntities GetContext()
        {
            if (_context == null)
                _context = new MarketplaceSessionEntities();
            return _context;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<FollowProduct> FollowProduct { get; set; }
        public virtual DbSet<LogIn> LogIn { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCart> ProductCart { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<User> User { get; set; }
    }
}
