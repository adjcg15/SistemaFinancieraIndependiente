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
    
    public partial class system_accounts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public system_accounts()
        {
            this.credit_applications = new HashSet<credit_applications>();
            this.dictums = new HashSet<dictum>();
        }
    
        public string employee_number { get; set; }
        public string name { get; set; }
        public string last_name { get; set; }
        public string surname { get; set; }
        public int id_employee_rol { get; set; }
        public string password { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<credit_applications> credit_applications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dictum> dictums { get; set; }
        public virtual employee_roles employee_roles { get; set; }
    }
}
