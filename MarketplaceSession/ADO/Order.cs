//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProductDelivery.ADO
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public int CartId { get; set; }
        public System.DateTime OrderDate { get; set; }
    
        public virtual Cart Cart { get; set; }
        public virtual Status Status { get; set; }
    }
}
