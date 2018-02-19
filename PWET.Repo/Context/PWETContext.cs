
using PWET.Domain;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PWET.Repo.Context
{
    public class PWETContext : DbContext
    {
        public PWETContext() : base("name=ExpenseTrackerContext")
        {

        }

        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<ExpenseGroup> ExpenseGroups { get; set; }
        public virtual DbSet<ExpenseGroupStatus> ExpenseGroupStatusses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ExpenseGroup>()
                .HasMany(e => e.Expenses)
                .WithRequired(e => e.ExpenseGroup).WillCascadeOnDelete();

            modelBuilder.Entity<ExpenseGroupStatus>()
                .HasMany(e => e.ExpenseGroups)
                .WithRequired(e => e.ExpenseGroupStatus)
                .HasForeignKey(e => e.ExpenseGroupStatusId)
                .WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
