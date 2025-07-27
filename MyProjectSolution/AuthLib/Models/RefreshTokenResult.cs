using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLib.Models
{
    public class RefreshTokenResult
    {
        public string RawToken { get; set; } = string.Empty;
        public RefreshToken StoredToken { get; set; } = new();
    }
}

