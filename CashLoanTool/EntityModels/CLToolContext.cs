using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CashLoanTool.EntityModels
{
    public partial class CLToolContext : DbContext
    {
        public virtual DbSet<Ability> Ability { get; set; }
        public virtual DbSet<AccountType> AccountType { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<CustomerInfo> CustomerInfo { get; set; }
        public virtual DbSet<Division> Division { get; set; }
        public virtual DbSet<Request> Request { get; set; }
        public virtual DbSet<Response> Response { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAbility> UserAbility { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ability>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(50);
            });

            modelBuilder.Entity<AccountType>(entity =>
            {
                entity.HasKey(e => e.Type);

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.Property(e => e.BranchId).ValueGeneratedNever();

                entity.Property(e => e.BranchName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CustomerInfo>(entity =>
            {
                entity.HasKey(e => e.CustomerId);

                entity.Property(e => e.CompanyAddress).HasMaxLength(400);

                entity.Property(e => e.CompanyName).HasMaxLength(200);

                entity.Property(e => e.ContactAddress).HasMaxLength(400);

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.HomeAddress)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.IdentityCard)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.IssueDate).HasColumnType("date");

                entity.Property(e => e.Issuer).HasMaxLength(50);

                entity.Property(e => e.LoanAmount).HasColumnType("numeric(12, 0)");

                entity.Property(e => e.MartialStatus).HasMaxLength(1);

                entity.Property(e => e.Nationality)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Pob)
                    .HasColumnName("POB")
                    .HasMaxLength(50);

                entity.Property(e => e.Position).HasMaxLength(150);

                entity.Property(e => e.Professional).HasMaxLength(150);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.CustomerInfo)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerInfo_Request");
            });

            modelBuilder.Entity<Division>(entity =>
            {
                entity.HasKey(e => e.DivisionName);

                entity.Property(e => e.DivisionName)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Division)
                    .HasForeignKey(d => d.BranchId)
                    .HasConstraintName("FK_Division_Branch");
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasMaxLength(36);

                entity.Property(e => e.LoanNo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.RequestCreateTime).HasColumnType("datetime");

                entity.Property(e => e.RequestSendTime).HasColumnType("datetime");

                entity.Property(e => e.RequestType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Signature).HasMaxLength(300);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.Request)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Request_User");
            });

            modelBuilder.Entity<Response>(entity =>
            {
                entity.Property(e => e.AcctName).HasMaxLength(100);

                entity.Property(e => e.AcctNo).HasMaxLength(20);

                entity.Property(e => e.ReceiveTime).HasColumnType("datetime");

                entity.Property(e => e.ResponseCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.ResponseMessage).HasMaxLength(200);

                entity.Property(e => e.Signature).HasMaxLength(300);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Response)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Response_Request");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Username);

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DivisionName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.DivisionNameNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.DivisionName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Division");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.Type)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_AccountType");
            });

            modelBuilder.Entity<UserAbility>(entity =>
            {
                entity.HasKey(e => new { e.Ability, e.Username });

                entity.Property(e => e.Ability).HasMaxLength(50);

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.Property(e => e.Parameter1).HasMaxLength(100);

                entity.Property(e => e.Parameter2).HasMaxLength(100);

                entity.HasOne(d => d.AbilityNavigation)
                    .WithMany(p => p.UserAbility)
                    .HasForeignKey(d => d.Ability)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAbility_Ability");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.UserAbility)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAbility_User");
            });
        }
    }
}
