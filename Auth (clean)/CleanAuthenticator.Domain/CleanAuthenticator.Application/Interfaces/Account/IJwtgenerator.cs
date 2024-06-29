using CleanAuthenticator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.Interfaces.Account
{
    public interface IJwtgenerator
    {
        Task<string> CreateToken(Accounts user);
    }
}
