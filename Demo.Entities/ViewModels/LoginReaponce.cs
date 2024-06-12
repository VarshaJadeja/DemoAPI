using Demo.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Entities.ViewModels
{
    public class LoginReaponce 
    {
        public User User { get; set; }

        public string Token { get; set; }
    }
}
