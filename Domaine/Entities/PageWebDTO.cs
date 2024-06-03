 using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domaine.Entities
{
    public class PageWebDTO
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;  // New field for theme
        public ObjectId Admin { get; set; }
    }

}
