//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProductWebAPI.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    
    public partial class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public Nullable<System.DateTime> IntroductionDate { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Url { get; set; }
        public Nullable<int> CategoryId { get; set; }
        [JsonIgnore]
        public virtual Category Category { get; set; }
    }
}
