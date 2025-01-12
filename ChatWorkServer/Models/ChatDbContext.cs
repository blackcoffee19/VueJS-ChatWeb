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
        public DbSet<RelationshipModel>? Relationships { get; set; }
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
            builder.Entity<RelationshipModel>().Property(e => e.RelaId).ValueGeneratedOnAdd();
            builder.Entity<MemeberGroupModel>()
                .HasOne(e => e.User)
                .WithMany(e => e.GroupsJoined)
                .HasForeignKey(e => e.UserId).HasConstraintName("nnnnn").HasPrincipalKey(e => e.UsID).OnDelete(DeleteBehavior.ClientNoAction)
                .IsRequired();
            builder.Entity<MemeberGroupModel>()
               .HasOne(e => e.GroupChat)
               .WithMany(e => e.MemeberGroup)
               .HasForeignKey(e => e.GroupId).HasConstraintName("mmmmm").HasPrincipalKey(e => e.GrId).OnDelete(DeleteBehavior.ClientNoAction)
               .IsRequired();
            builder.Entity<GroupChatModel>().HasOne(e => e.UserCreatedModel).WithMany(e => e.GroupCreated).HasForeignKey(e => e.UserCreated );
            builder.Entity<UsersModel>()
                .HasMany(e => e.ChatsSend)
                .WithOne(e => e.UserChat).HasForeignKey(e => e.UserId).HasConstraintName("aaaaa").HasPrincipalKey(e => e.UsID).OnDelete(DeleteBehavior.Cascade).IsRequired();
            builder.Entity<RelationshipModel>()
                .HasOne(e => e.User1)  
                .WithMany()            
                .HasForeignKey(e => e.User_1_Id)  
                .HasConstraintName("FK_Relationship_User_1")  
                .HasPrincipalKey(e => e.UsID)   
                .OnDelete(DeleteBehavior.ClientNoAction) 
                .IsRequired();  

            builder.Entity<RelationshipModel>()
                .HasOne(e => e.User2)  // User2 là thuộc tính trong RelationshipModel
                .WithMany()            // Mỗi người dùng có thể có nhiều quan hệ
                .HasForeignKey(e => e.User_2_Id)  // User_2_Id là khóa ngoại
                .HasConstraintName("FK_Relationship_User_2")  // Tên của Constraint (tùy chỉnh)
                .HasPrincipalKey(e => e.UsID)   // UsID là khóa chính của UsersModel
                .OnDelete(DeleteBehavior.ClientNoAction)  // Xử lý khi xóa dữ liệu
                .IsRequired();  // Quan hệ này là bắt buộc
            builder.Entity<GroupChatModel>()
                .HasMany(e => e.ChatsSend)
                .WithOne(e => e.GroupChat).HasForeignKey(e => e.GroupId).HasConstraintName("bbbbbb")
                .HasPrincipalKey(e => e.GrId).OnDelete(DeleteBehavior.ClientNoAction).IsRequired();

            ModelBuilderExtension.Seed(builder);
        }
    }
    
}
