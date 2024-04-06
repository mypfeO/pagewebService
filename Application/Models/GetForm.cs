using Application.Common.Mappings;
using AutoMapper;
using Domaine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class GetForm 
    {
       public string siteWebId  { get; set; }
       public string formId { get; set; }
    }
}
