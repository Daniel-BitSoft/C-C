using CC.Constants;
using CC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CC.Providers
{
    public class AntigensProvider
    {
        public AntigenResponse CreateAntigen(Antigen antigen)
        {
            try
            {
                var existingAntigen = App.dbcontext.Antigens.FirstOrDefault(a => a.AntigenName == antigen.AntigenName);
                if (existingAntigen != null)
                {
                    return new AntigenResponse { ErrorMessage = Messages.AlreadyExists };
                }

                antigen.AntigenId = Guid.NewGuid().ToString();
                antigen.CreatedBy = App.LoggedInUser.UserId;
                antigen.CreatedDt = DateTime.Now;

                App.dbcontext.Antigens.Add(antigen);
                App.dbcontext.SaveChanges();

                return new AntigenResponse { Antigen = antigen };
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(CreateAntigen), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                return new AntigenResponse { ErrorMessage = $"{ Messages.Exception} - log: {logNumber}" };
            }
        }

        public string UpdateAntigen(Antigen antigen)
        {
            try
            {
                var antigenToUpdate = App.dbcontext.Antigens.FirstOrDefault(a => a.AntigenId == antigen.AntigenId);
                if (antigenToUpdate.AntigenName != antigen.AntigenName)
                {
                    var auditRecord = new Audit
                    {
                        RecordId = antigen.AntigenId,
                        Type = AuditTypes.Antigen.ToString(),
                        Description = AuditEvents.AntigenUpdated.ToString(),
                        UpdatedBy = App.LoggedInUser.UserId,
                        UpdatedDt = DateTime.Now
                    };
                    App.dbcontext.Audits.Add(auditRecord);

                    antigenToUpdate.AntigenName = antigen.AntigenName;

                    App.dbcontext.SaveChanges();
                }

                return null;
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(CreateAntigen), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                return $"{ Messages.Exception} - log: {logNumber}";
            }
        }

        public AntigensResponse GetAntigensNotAssigned()
        {
            try
            {
                var antigens = App.dbcontext.Database.SqlQuery<Antigen>("GetAntigensNotAssingedToArray")?.ToList();
                return new AntigensResponse { Antigens = antigens };
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(CreateAntigen), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                return new AntigensResponse { ErrorMessage = $"{ Messages.Exception} - log: {logNumber}" };
            }
        }

        public AntigensResponse GetAntigensAssignedToArray(string arrayId = null)
        {
            try
            {
                var assignedAntigens = App.dbcontext.GetAntigensAssingedToArray(arrayId)?.ToList();
                return new AntigensResponse { Antigens = App.mapper.Map<List<Antigen>>(assignedAntigens) };
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(CreateAntigen), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                return new AntigensResponse { ErrorMessage = $"{ Messages.Exception} - log: {logNumber}" };
            }
        }

        public AntigensResponse GetArrayAntigens(string arrayId)
        {
            try
            {
                var arrayIdParam = new SqlParameter("@arrayId", arrayId);

                var antigens = App.dbcontext.Database.SqlQuery<Antigen>("GetArrayAntigens @arrayId", arrayIdParam)?.ToList();
                return new AntigensResponse { Antigens = antigens };
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(CreateAntigen), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                return new AntigensResponse { ErrorMessage = $"{ Messages.Exception} - log: {logNumber}" };
            }
        }
    }
}
