//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Etickets.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MovieCinema
    {
        public int Id { get; set; }
        public int IdMovie { get; set; }
        public int IdCinema { get; set; }
    
        public virtual Cinema Cinema { get; set; }
        public virtual movie movie { get; set; }
    }
}
