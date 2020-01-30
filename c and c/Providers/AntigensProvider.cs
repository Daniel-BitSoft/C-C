using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Providers
{
    public class AntigensProvider
    {
        Entities dbcontext = new Entities();

        public bool CreateAntigen(Antigen antigen)
        {
            try
            {
                dbcontext.Antigens.Add(antigen);
                dbcontext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // log somewhere
                return false;
            }
        }

        public bool UpdateAntigen(Antigen antigen)
        {
            try
            {
                var antigenToUpdate = dbcontext.Antigens.FirstOrDefault(a => a.AntigenId == antigen.AntigenId);

                antigenToUpdate.AntigenName = antigen.AntigenName;
                antigenToUpdate.UpdatedBy = antigen.UpdatedBy;
                antigenToUpdate.UpdatedDt = DateTime.Now;

                dbcontext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // log somewhere
                return false;
            }
        }



    }
}
