using ChatWorkServer.Common;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChatWorkServer.Models.Data
{
    public class ModelBuilderExtension
    {
        
        public static void Seed(ModelBuilder builder) {

            builder.Entity<UsersModel>().HasData(
              new UsersModel(1,"tuong", "CatTuong", TUtility.GetMD5("123"), true, 1,DateTime.Now, "tuong.png"),
              new UsersModel(2,"hinata", "Hinata Sohara", TUtility.GetMD5("hinata"), true, 1,DateTime.Now, "hinata-1.jpg"),
              new UsersModel(3,"juria", "Juria", TUtility.GetMD5("juria"), false, 1,DateTime.Now,"juria-1.jpg"),
              new UsersModel(4,"maya", "Maya", TUtility.GetMD5("maya"), false,1, DateTime.Now,"maya-1.jpg"),
              new UsersModel(5,"jurin", "Jurin", TUtility.GetMD5("jurin"), false,1, DateTime.Now,"jurin-1.jpg"),
              new UsersModel(6,"harvey", "Harvey", TUtility.GetMD5("harvey"), false,1, DateTime.Now,"harvey-1.jpg"),
              new UsersModel(7,"chisa", "Chisa", TUtility.GetMD5("chisa"), false, 1,DateTime.Now,"chisa-1.jpg"),
              new UsersModel(8,"cocona", "Cocona", TUtility.GetMD5("cocona"), false,1, DateTime.Now,"cocona-1.jpg")
            );
            builder.Entity<GroupChatModel>().HasData(
              new GroupChatModel(1,"RiaHina","XG1",2,DateTime.Now),
              new GroupChatModel(2,"Chinata","XG2",7,DateTime.Now),
              new GroupChatModel(3,"Mavey","XG3",4,DateTime.Now),
              new GroupChatModel(4,"Osaka Sister","XG4",3,DateTime.Now)
            );
            builder.Entity<RelationshipModel>().HasData(
                new RelationshipModel(1, 1, 2, 3, true, 80,1, DateTime.Now),
                new RelationshipModel(2, 1, 2, 7, true, 60,1, DateTime.Now),
                new RelationshipModel(3, 1, 6, 4, true, 40,1, DateTime.Now),
                new RelationshipModel(4, 1, 3, 7, true, 20,1, DateTime.Now)
            );
            builder.Entity<MemeberGroupModel>().HasData(
                new MemeberGroupModel(1,2,1,DateTime.Now),
                new MemeberGroupModel(2,3,1,DateTime.Now),
                new MemeberGroupModel(3,2,2,DateTime.Now),
                new MemeberGroupModel(4,7,2,DateTime.Now),
                new MemeberGroupModel(5,4,3,DateTime.Now),
                new MemeberGroupModel(6,6,3,DateTime.Now),
                new MemeberGroupModel(7, 3, 4, DateTime.Now),
                new MemeberGroupModel(8, 7, 4, DateTime.Now)
            );
            builder.Entity<ChatModel>().HasData(
                new ChatModel(1, "Hi Hi-chan", 3, 1, true),
                new ChatModel(2, "Hi Juria", 2, 1, true),
                new ChatModel(3, "Good morning ;o;", 3, 1, false),
                new ChatModel(4, "Are you free Hina?", 7, 2, false),
                new ChatModel(5, "Hang out Juria? I'll ask Hinata too", 7, 4, false)
                );
        }
    }
}
