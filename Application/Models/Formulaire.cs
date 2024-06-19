using Application.Common.Mappings;
using Domaine.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Application.Models
{
    public class Formulaire
    {
        public Head Head { get; set; }
        public List<BodyItem> Body { get; set; } = new List<BodyItem>();
        public List<FooterItem> Footer { get; set; } = new List<FooterItem>();
    }


    public class Head : IMapFrom<HeadDTO>
    {
        public string Title { get; set; } = string.Empty;
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InputType
    {
        [EnumMember(Value = "Text")]
        Text,

        [EnumMember(Value = "Image")]
        Image,

        [EnumMember(Value = "File")]
        File,

        [EnumMember(Value = "Date")]
        Date,

        // Add other types as needed
    }


    public class BodyItem : IMapFrom<BodyItemDTO>
    {
        public string Titre { get; set; } = string.Empty;
        public string Type { get; set; }
        public string RespenseText { get; set; } = string.Empty;
        public bool Required { get; set; } = false;
    }

    public class FooterItem
    {
        public string Titre { get; set; } = string.Empty;
        public string LinkNextForm { get; set; } = string.Empty;
    }




}
