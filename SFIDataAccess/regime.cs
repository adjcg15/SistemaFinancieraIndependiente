//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SFIDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class regime
    {
        public int id_regime { get; set; }
        public Nullable<System.DateTime> application_end_date { get; set; }
        public System.DateTime application_start_date { get; set; }
        public string credit_condition_identifier { get; set; }
        public string credit_invoice { get; set; }
    
        public virtual credit_conditions credit_conditions { get; set; }
        public virtual credit credit { get; set; }
    }
}
