using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.Contexts;

public class ChatContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        //builder.UseSqlite("Data Source=DB\\chat.db");
        builder.LogTo(Console.WriteLine)
            //.UseLazyLoadingProxies()
            .UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=example;Database=chat_db");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ConfigurateUsers(builder);
        ConfigurateMessages(builder);
    }

    private static void ConfigurateMessages(ModelBuilder builder)
    {
        builder.Entity<MessageEntity>().ToTable("messages");
        builder.Entity<MessageEntity>().Property(x => x.Id).HasColumnName("id");
        builder.Entity<MessageEntity>().Property(x => x.Text).HasColumnName("text");
        builder.Entity<MessageEntity>().Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Entity<MessageEntity>().Property(x => x.SenderId).HasColumnName("from_user_id");
        builder.Entity<MessageEntity>().Property(x => x.RecipientId).HasColumnName("to_user_id");

        builder.Entity<MessageEntity>().HasKey(x => x.Id).HasName("message_pkey");
        builder.Entity<MessageEntity>().Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Entity<MessageEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.SenderId)
            .HasConstraintName("messages_from_user_id_fkey");
        builder.Entity<MessageEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.RecipientId)
            .HasConstraintName("messages_to_user_id_fkey");
    }

    private static void ConfigurateUsers(ModelBuilder builder)
    {
        builder.Entity<UserEntity>().HasKey(x=>x.Id).HasName("user_pkey");
        builder.Entity<UserEntity>().ToTable("users");
        builder.Entity<UserEntity>().Property(x => x.Id).HasColumnName("id");
        builder.Entity<UserEntity>().Property(x => x.Name).HasMaxLength(255).HasColumnName("name");
        builder.Entity<UserEntity>().Property(x => x.LastOnline).HasColumnName("last_online");
        builder.Entity<UserEntity>().Property(x=>x.Id).ValueGeneratedOnAdd();
        builder.Entity<UserEntity>().HasIndex(x=>x.Name).IsUnique();        
    }

}

