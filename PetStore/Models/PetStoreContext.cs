using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PetStore.Models
{
    public partial class PetStoreContext : DbContext
    {
        public static PetStoreContext Ins = new PetStoreContext();
        public PetStoreContext()
        {
            if (Ins == null)
            {
                Ins = this;
            }
        }

        public PetStoreContext(DbContextOptions<PetStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Forum> Forums { get; set; } = null!;
        public virtual DbSet<ForumType> ForumTypes { get; set; } = null!;
        public virtual DbSet<Hashtag> Hashtags { get; set; } = null!;
        public virtual DbSet<Infor> Infors { get; set; } = null!;
        public virtual DbSet<Messenger> Messengers { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<PostHashtag> PostHashtags { get; set; } = null!;
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public virtual DbSet<StateInfor> StateInfors { get; set; } = null!;
        public virtual DbSet<StatusOrder> StatusOrders { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            if (!optionsBuilder.IsConfigured) { optionsBuilder.UseSqlServer(config.GetConnectionString("value")); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.State).HasMaxLength(255);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Account__RoleID__267ABA7A");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.Address1)
                    .HasMaxLength(255)
                    .HasColumnName("Address");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FullNameCustomer).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.Phone).HasMaxLength(50);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).HasMaxLength(255);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.Property(e => e.CommentId).HasColumnName("CommentID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.ForumId).HasColumnName("ForumID");

                entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Comment__Account__4222D4EF");

                entity.HasOne(d => d.Forum)
                    .WithMany(p => p.CommentsNavigation)
                    .HasForeignKey(d => d.ForumId)
                    .HasConstraintName("FK__Comment__ForumID__412EB0B6");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Dislike)
                    .HasColumnName("dislike")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Feedback1).HasColumnName("Feedback");

                entity.Property(e => e.Like)
                    .HasColumnName("like")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Love)
                    .HasColumnName("love")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Feedback__Accoun__44FF419A");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Feedback__Produc__45F365D3");
            });

            modelBuilder.Entity<Forum>(entity =>
            {
                entity.ToTable("Forum");

                entity.Property(e => e.ForumId).HasColumnName("ForumID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Age).HasDefaultValueSql("((0))");

                entity.Property(e => e.Comments).HasColumnName("comments");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Gene)
                    .HasColumnName("GENE")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.Likes).HasColumnName("likes");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.Property(e => e.Views).HasColumnName("views");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Forums)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Forum__AccountID__3E52440B");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Forums)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK__Forum__TypeId__02FC7413");
            });

            modelBuilder.Entity<ForumType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK__ForumTyp__516F03B5533A3D5D");

                entity.ToTable("ForumType");

                entity.Property(e => e.TypeName).HasMaxLength(255);
            });

            modelBuilder.Entity<Hashtag>(entity =>
            {
                entity.HasIndex(e => e.Tag, "UQ__Hashtags__C451641313BF0AD0")
                    .IsUnique();

                entity.Property(e => e.HashtagId).HasColumnName("HashtagID");

                entity.Property(e => e.Tag)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Infor>(entity =>
            {
                entity.ToTable("Infor");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Fullname).HasMaxLength(255);

                entity.Property(e => e.Gender).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(10);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Infors)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Infor__AccountID__4D94879B");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Infors)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK__Infor__StateId__4E88ABD4");
            });

            modelBuilder.Entity<Messenger>(entity =>
            {
                entity.ToTable("Messenger");

                entity.Property(e => e.MessengerId).HasColumnName("MessengerID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.SentTime).HasColumnType("datetime");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Messengers)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Messenger__Accou__48CFD27E");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.PaymentMethodId)
                    .HasColumnName("paymentMethodId")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Order__AccountId__31EC6D26");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK__Order__AddressID__32E0915F");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .HasConstraintName("FK_Order_PaymentMethod");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__Order__StatusID__33D4B598");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Total).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderDeta__Order__36B12243");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__OrderDeta__Produ__37A5467C");
            });

            modelBuilder.Entity<PostHashtag>(entity =>
            {
                entity.HasKey(e => e.PostHashtagsId)
                    .HasName("PK__PostHash__F25F49D92A6A2C70");

                entity.Property(e => e.ForumId).HasColumnName("ForumID");

                entity.Property(e => e.HashtagId).HasColumnName("HashtagID");

                entity.HasOne(d => d.Forum)
                    .WithMany(p => p.PostHashtags)
                    .HasForeignKey(d => d.ForumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PostHasht__Forum__5EBF139D");

                entity.HasOne(d => d.Hashtag)
                    .WithMany(p => p.PostHashtags)
                    .HasForeignKey(d => d.HashtagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PostHasht__Hasht__5FB337D6");
            });
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.MethodId)
                    .HasName("PK__PaymentM__C7B34C894A22A74C");

                entity.ToTable("PaymentMethod");

                entity.Property(e => e.MethodId).HasColumnName("methodId");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createAt");

                entity.Property(e => e.MethodName)
                    .HasMaxLength(50)
                    .HasColumnName("methodName");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .HasColumnName("status");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updateAt");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductName).HasMaxLength(255);

                entity.Property(e => e.Size).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UnitInStock).HasDefaultValueSql("((0))");

                entity.Property(e => e.UnitOrdered).HasDefaultValueSql("((0))");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Product__Categor__2F10007B");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.ImgId)
                    .HasName("PK__ProductI__352F5413C133FC94");

                entity.ToTable("ProductImage");

                entity.Property(e => e.ImgId).HasColumnName("ImgID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.ImgUrl).IsUnicode(false);

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductIm__Produ__5165187F");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName).HasMaxLength(255);
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.CartId)
                    .HasName("PK__Shopping__51BCD7974D03D238");

                entity.ToTable("ShoppingCart");

                entity.Property(e => e.CartId).HasColumnName("CartID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ShoppingCarts)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__ShoppingC__Accou__3A81B327");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ShoppingCarts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ShoppingC__Produ__3B75D760");
            });

            modelBuilder.Entity<StateInfor>(entity =>
            {
                entity.HasKey(e => e.StateId)
                    .HasName("PK__StateInf__C3BA3B3AB8876DD0");

                entity.ToTable("StateInfor");

                entity.Property(e => e.StateName).HasMaxLength(255);
            });

            modelBuilder.Entity<StatusOrder>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__StatusOr__C8EE2043C41213CF");

                entity.ToTable("StatusOrder");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.StatusName).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
