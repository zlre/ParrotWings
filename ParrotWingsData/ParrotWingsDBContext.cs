namespace ParrotWingsData
{
    using Microsoft.EntityFrameworkCore;

    public class ParrotWingsDBContext : DbContext
    {
        public ParrotWingsDBContext(DbContextOptions<ParrotWingsDBContext> options)
           : base(options)
        {
            //Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UserBalance> UserBalances { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region User

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Id)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .IsRequired()
                .ValueGeneratedNever();

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserBalances)
                .WithOne(wb => wb.User);
   
            #endregion

            #region Transaction

            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.Id)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Id)
                .IsRequired()
                .ValueGeneratedNever();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.CreationDate)
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Status)
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Recipient)
                .WithMany(u => u.RecipientTransactions)
                .HasForeignKey(t => t.RecipientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Sender)
                .WithMany(u => u.SenderTransactions)
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
                        
            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.UserBalances)
                .WithOne(ub => ub.Transaction)
                .HasForeignKey(ub => ub.TransactionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region UserBalance

            modelBuilder.Entity<UserBalance>()
                .HasKey(ub => ub.Id);

            modelBuilder.Entity<UserBalance>()
                .HasIndex(ub => ub.Id)
                .IsUnique();

            modelBuilder.Entity<UserBalance>()
                .Property(ub => ub.Id)
                .IsRequired()
                .ValueGeneratedNever();            
            
            modelBuilder.Entity<UserBalance>()
                .Property(ub => ub.Amount)
                .IsRequired();            
            
            modelBuilder.Entity<UserBalance>()
                .Property(ub => ub.BalanceDate)
                .IsRequired();            

            modelBuilder.Entity<UserBalance>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.UserBalances)
                .HasForeignKey(ub => ub.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);            
            
            modelBuilder.Entity<UserBalance>()
                .HasOne(ub => ub.Transaction)
                .WithMany(t => t.UserBalances)
                .HasForeignKey(ub => ub.TransactionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
