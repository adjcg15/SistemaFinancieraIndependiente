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
    
    public partial class personal_references
    {
        public string ine_key { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string last_name { get; set; }
        public string kinship { get; set; }
        public string relationship_years { get; set; }
        public int id_address { get; set; }
        public string client_rfc { get; set; }
        public int id_personal_reference { get; set; }
        public string phone_number { get; set; }
    
        public virtual address address { get; set; }
        public virtual client client { get; set; }
    }
}
