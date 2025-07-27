using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLib.Models
{
    public interface IUserIdentity
    {
        int Id { get; }
        string Username { get; }
        string Role { get; }
    }

}
