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
    
    public partial class credit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public credit()
        {
            this.regimes = new HashSet<regime>();
        }
    
        public string invoice { get; set; }
        public System.DateTime approval_date { get; set; }
        public Nullable<System.DateTime> settlement_date { get; set; }
        public System.DateTime withdrawal_date { get; set; }
        public decimal ammount_approved { get; set; }
        public string client_rfc { get; set; }
        public int id_credit_type { get; set; }
        public string credit_application_invoice { get; set; }
    
        public virtual client client { get; set; }
        public virtual credit_applications credit_applications { get; set; }
        public virtual credit_types credit_types { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<regime> regimes { get; set; }
    }
}
