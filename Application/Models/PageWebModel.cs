using Application.Common.Mappings;
using Domaine.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class PageWebModel : IMapFrom<PageWebDTO>
    {
        public string Name { get; set; } = string.Empty;
        public List<ObjectId> Users { get; set; } = new List<ObjectId>();
    }
}
