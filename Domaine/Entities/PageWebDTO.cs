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
        public List<ObjectId> Users { get; set; } = new List<ObjectId>();
    }

}
