using CC.Constants;
using CC.Models;
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

        public AntigenResponse CreateAntigen(Antigen antigen)
        {
            try
            {
                var existingAntigen = dbcontext.Antigens.FirstOrDefault(a => a.AntigenName == antigen.AntigenName);
                if (existingAntigen != null)
                {
                    return new AntigenResponse { ErrorMessage = Messages.AlreadyExists };
                }

                antigen.AntigenId = Guid.NewGuid().ToString();
                antigen.CreatedBy = App.User.UserId;
                antigen.CreatedDt = DateTime.Now;

                dbcontext.Antigens.Add(antigen);
                dbcontext.SaveChanges();

                return new AntigenResponse { Antigen = antigen };
            }
            catch (Exception ex)
            {
                var logCode = Guid.NewGuid().ToString();
                Logger.Log(nameof(CreateAntigen), new Dictionary<string, object>
                {
                    { LogConsts.LogNumber, logCode},
                    { LogConsts.Exception, ex }
                });

                return new AntigenResponse { ErrorMessage = $"{ Messages.Exception} - log: {logCode}" };
            }
        }

        public string UpdateAntigen(Antigen antigen)
        {
            try
            {
                var antigenToUpdate = dbcontext.Antigens.FirstOrDefault(a => a.AntigenId == antigen.AntigenId);
                if (antigenToUpdate.AntigenName != antigen.AntigenName)
                {
                    AntigenAudit auditRecord = new AntigenAudit
                    {
                        AntigenId = antigenToUpdate.AntigenId,
                        PreviousAntigenName = antigenToUpdate.AntigenName,
                        UpdatedBy = App.User.UserId,
                        UpdatedDt = DateTime.Now
                    }; 
                    dbcontext.AntigenAudits.Add(auditRecord);

                    antigenToUpdate.AntigenName = antigen.AntigenName;  

                    dbcontext.SaveChanges();
                }

                return null;
            }
            catch (Exception ex)
            {
                var logCode = Guid.NewGuid().ToString();
                Logger.Log(nameof(CreateAntigen), new Dictionary<string, object>
                {
                    { LogConsts.LogNumber, logCode},
                    { LogConsts.Exception, ex }
                });

                return $"{ Messages.Exception} - log: {logCode}";
            }
        }

        public AntigensResponse GetAntigensNotAssigned()
        {
            try
            {
                var antigens = dbcontext.Database.SqlQuery<Antigen>("GetAntigensNotAssingedToBatch")?.ToList();
                return new AntigensResponse { Antigens = antigens };
            }
            catch (Exception ex)
            {
                var logCode = Guid.NewGuid().ToString();
                Logger.Log(nameof(CreateAntigen), new Dictionary<string, object>
                {
                    { LogConsts.LogNumber, logCode},
                    { LogConsts.Exception, ex }
                });

                return new AntigensResponse { ErrorMessage = $"{ Messages.Exception} - log: {logCode}" };
            }
        }
    }
}
