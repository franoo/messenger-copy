using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Authentication;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Services
{

    public interface IUserService
    {
        UserDTO GetUser(UserLogin model);
        UserDTO GetById(int id);
    }
    public class UserService: IUserService
    {
        private MyDBContext _context;
        public UserService(
            MyDBContext context)
        {
            _context = context;
        }
        public UserDTO GetUser(UserLogin model)
        {
            User user = _context.Users.SingleOrDefault(x => x.username == model.UserName && x.PasswordHash == model.Password);//_userService.Authenticate(model);
            var userDTO=new UserDTO { Password = user.PasswordHash, Username = user.username, Id=user.id};
            return userDTO;
        }
        public UserDTO GetById(int id)
        {
            UserDTO userDTO = new UserDTO();
            try
            {
                var user = _context.Users.Find(id);
                userDTO = new UserDTO { Id = user.id, Password = user.PasswordHash, Username = user.username };
               
            }
            catch
            {

            }
            return userDTO;
        }

    }
}
