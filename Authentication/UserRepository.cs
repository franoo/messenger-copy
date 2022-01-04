using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Authentication
{
    public interface IUserRepository
    {
        UserDTO GetUser(UserLogin userMode);
    }
    public class UserRepository
    {
    }
}
