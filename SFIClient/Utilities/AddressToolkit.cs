using SFIClient.SFIServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFIClient.Utilities
{
    public class AddressToolkit
    {
        public static string FormatAsText(Address address)
        { 
            return address.Street + " #" + address.OutdoorNumber + " interior " + address.InteriorNumber +
                ", colonia " + address.Neighborhod + ". CP" + address.PostCode + ", " + address.City + ", " +
                address.Municipality + ", " + address.State;
        }

    }
}
