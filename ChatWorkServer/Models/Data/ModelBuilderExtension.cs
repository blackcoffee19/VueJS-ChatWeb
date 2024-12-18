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
              new UsersModel(1,"tuong", "CatTuong", TUtility.GetMD5("123"), true, DateTime.Now),
              new UsersModel(2,"hinata", "Hinata Sohara", TUtility.GetMD5("hinata"), true, DateTime.Now),
              new UsersModel(3,"juria", "Juria", TUtility.GetMD5("juria"), false, DateTime.Now),
              new UsersModel(4,"maya", "Maya", TUtility.GetMD5("maya"), false, DateTime.Now),
              new UsersModel(5,"jurin", "Jurin", TUtility.GetMD5("jurin"), false, DateTime.Now),
              new UsersModel(6,"harvey", "Harvey", TUtility.GetMD5("harvey"), false, DateTime.Now),
              new UsersModel(7,"chisa", "Chisa", TUtility.GetMD5("chisa"), false, DateTime.Now),
              new UsersModel(8,"cocona", "Cocona", TUtility.GetMD5("cocona"), false, DateTime.Now)
            );
            builder.Entity<GroupChatModel>().HasData(
              new GroupChatModel(1,"RiaHina","XG1",2,DateTime.Now),
              new GroupChatModel(2,"Chinata","XG2",7,DateTime.Now),
              new GroupChatModel(3,"Mavey","XG3",4,DateTime.Now)
            );
            builder.Entity<MemeberGroupModel>().HasData(
                new MemeberGroupModel(1,2,1,DateTime.Now),
                new MemeberGroupModel(2,3,1,DateTime.Now),
                new MemeberGroupModel(3,2,2,DateTime.Now),
                new MemeberGroupModel(4,7,2,DateTime.Now),
                new MemeberGroupModel(5,4,3,DateTime.Now),
                new MemeberGroupModel(6,6,3,DateTime.Now)
            );

        }
    }
}
