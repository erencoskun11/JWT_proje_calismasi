using JWT_project.core.Configuration;
using JWT_project.core.DTOs;
using JWT_project.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWT_project.core.Services
{
    public interface ITokenService
    {
        TokenDto CreateTokenDto(UserApp userApp);

        ClientTokenDto CreateTokenByClient(Client client);
    }
}
