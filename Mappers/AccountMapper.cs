using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpEaseApi.Dtos.Account;
using HelpEaseApi.Models;

namespace HelpEaseApi.Mappers
{
    public static class AccountMapper
    {
        public static ReadAccountDto ToReadAccount(this AppUser user)
        {
            return new ReadAccountDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber
            };
        }
    }
}