using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{

    public class Design
    {
       public List<string> ProductImages { get; set; }
        public string BackgroundColor { get; set; }
        public string Logo { get; set; }
    }
}
