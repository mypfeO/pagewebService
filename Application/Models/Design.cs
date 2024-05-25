﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{

    public class Design
    {
       public List<IFormFile> ProductImages { get; set; }
        public string BackgroundColor { get; set; }
        public IFormFile Logo { get; set; }
    }
}
