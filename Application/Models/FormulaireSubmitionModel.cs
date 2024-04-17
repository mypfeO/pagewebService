using Application.Common.Mappings;
using Domaine.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class SubmitFormModel
    {
        public List<FormFieldModel> Body { get; set; }
        public string ExcelFileLink { get; set; }
    }

    public class FormFieldModel
    {
        public string Titre { get; set; }
        public bool ImageLink { get; set; }
        public IFormFile? RespenseFile { get; set; } = null;
        public string? RespenseText { get; set; } = string.Empty;
    }



}
