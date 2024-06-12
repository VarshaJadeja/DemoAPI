using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repositories.Repositories
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);

        Task<LoginReaponce> Login(LoginRequest oginRequest);

        Task<User> Register(RegistrationRequest registrationRequest);
    }
}
