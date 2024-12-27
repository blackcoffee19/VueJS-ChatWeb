using ChatWorkServer.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace ChatWorkServer.Models
{
    public class ChatDbContext : DbContext 
    {

        public ChatDbContext(DbContextOptions<ChatDbContext> op) :base(op) { }
        public DbSet<UsersModel>? Users { get; set; }
        public DbSet<GroupChatModel>? GroupChats { get; set; }
        public DbSet<MemeberGroupModel>? Memebers { get; set; }
        public DbSet<ChatModel>? Chats { get; set; }
        public DbSet<RequirementModel>? Requirements { get; set; }
        public DbSet<ConnectionModel>? Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName!.StartsWith("AspNet"))
                    entityType.SetTableName(tableName.Substring(6));
            }
            builder.Entity<UsersModel>().Property(e => e.UsID).ValueGeneratedOnAdd();
            builder.Entity<GroupChatModel>().Property(e => e.GrId).ValueGeneratedOnAdd();
            builder.Entity<MemeberGroupModel>().Property(e => e.MemId).ValueGeneratedOnAdd();
            builder.Entity<ChatModel>().Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Entity<MemeberGroupModel>()
                .HasOne(e => e.User)
                .WithMany(e => e.GroupsJoined)
                .HasForeignKey(e => e.UserId).HasConstraintName("nnnnnnnnnnnnn").HasPrincipalKey(e =>e.UsID).OnDelete(DeleteBehavior.ClientNoAction)
                .IsRequired();
            builder.Entity<MemeberGroupModel>()
               .HasOne(e => e.GroupChat)
               .WithMany(e => e.MemeberGroup)
               .HasForeignKey(e => e.GroupId).HasConstraintName("mmmmmmmmmmmmm").HasPrincipalKey(e => e.GrId).OnDelete(DeleteBehavior.ClientNoAction)
               .IsRequired();
            builder.Entity<GroupChatModel>().HasOne(e => e.UserCreated).WithMany(e =>e.GroupCreated).HasForeignKey(e => e.UserCreatedId);
            builder.Entity<UsersModel>()
                .HasMany(e => e.ChatsSend)
                .WithOne( e=>e.UserChat).HasForeignKey(e => e.UserId).HasConstraintName("aaaaaaaaaaaa").HasPrincipalKey(e => e.UsID).OnDelete(DeleteBehavior.Cascade).IsRequired();
            builder.Entity<GroupChatModel>()
                .HasMany(e => e.ChatsSend)
                .WithOne(e => e.GroupChat).HasForeignKey(e => e.GroupId).HasConstraintName("bbbbbbbbbbbbbbbb")
                .HasPrincipalKey(e => e.GrId).OnDelete(DeleteBehavior.ClientNoAction).IsRequired();

            ModelBuilderExtension.Seed(builder);
        }
    }
    
}
