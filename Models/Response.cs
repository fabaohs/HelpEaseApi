using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpEaseApi.Models
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = String.Empty;
        public T? Data { get; set; }
    }
}