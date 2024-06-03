using Application.Common.Mappings;
using Domaine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class UpdatePageWeb : IMapFrom<PageWebDTO>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }  // New field for theme
        public string Admin { get; set; }
    }
}
