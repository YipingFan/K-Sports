//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoDelivery.Repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class RegionPostcode
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }
        public int FromPostCode { get; set; }
        public int ToPostCode { get; set; }
        public string Description { get; set; }
    
        public virtual Region Region { get; set; }
    }
}
