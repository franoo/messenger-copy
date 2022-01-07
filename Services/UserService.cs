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
            User user = _context.Users.SingleOrDefault(x => x.Username == model.UserName && x.PasswordHash == model.Password);//_userService.Authenticate(model);
            var userDTO=new UserDTO { Password = user.PasswordHash, Username = user.Username, Id=user.Id};
            return userDTO;
        }
        public UserDTO GetById(int id)
        {
            UserDTO userDTO = new UserDTO();
            try
            {
                var user = _context.Users.Find(id);
                userDTO = new UserDTO { Id = user.Id, Password = user.PasswordHash, Username = user.Username };
               
            }
            catch
            {

            }
            return userDTO;
        }

    }
}
