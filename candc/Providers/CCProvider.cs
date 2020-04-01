using CC.Constants;
using CC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CC.Providers
{
    public class CCProvider
    {
        public List<AntigensAssingedToArray> ArrayAntigens { get; set; }

        public void SetArrayAntigens()
        {
            ArrayAntigens = App.dbcontext.GetAntigensAssingedToArray()?.ToList(); // Database.SqlQuery<ArrayAntigenRelation>("GetAntigensAssingedToArray")?.ToList();
        }

        public void CreateCalibControl(List<CalibControl> calibControls)
        {
            try
            {
                foreach (var cc in calibControls)
                {
                    cc.CreatedBy = App.LoggedInUser.UserId;
                    cc.CreatedDt = DateTime.Now;
                    cc.CCId = Guid.NewGuid().ToString();

                    App.dbcontext.CalibControls.Add(cc);
                }
                App.dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(CreateCalibControl), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        //public string UpdateCalibControl(CalibControl CalibControl, List<Antigen> AddedAntigens, List<Antigen> removedAntigens)
        //{
        //try
        //{
        //    // update CalibControl
        //    CalibControl.UpdatedBy = App.LoggedInUser.UserId;
        //    CalibControl.UpdatedDt = DateTime.Now;

        //    // record audit
        //    Audit audit = new Audit
        //    {
        //        RecordId = CalibControl.CCId,
        //        Type = AuditTypes.CalibControl.ToString(),
        //        Description = AuditEvents.CalibControlUpdated.ToString(),
        //        UpdatedBy = App.LoggedInUser.UserId,
        //        UpdatedDt = DateTime.Now
        //    };

        //    App.dbcontext.Audits.Add(audit);
        //    App.dbcontext.SaveChanges();

        //    // delete any antigens removed from CalibControl
        //    if (removedAntigens != null && removedAntigens.Any())
        //    {
        //        var CalibControlAntigens = App.dbcontext.CalibControlAntigens.Where(a => a.CalibControlId == CalibControl.CalibControlId).ToList();
        //        var removedAntigenIds = removedAntigens.Select(a => a.AntigenId).ToList();
        //        App.dbcontext.CalibControl.RemoveRange(CalibControlAntigens.Where(a => removedAntigenIds.Contains(a.AntigenId);

        //        App.dbcontext.SaveChanges();
        //    }

        //    // Create any new antigens added to CalibControl
        //    if (AddedAntigens != null && AddedAntigens.Any())
        //    {
        //        foreach (var antigen in AddedAntigens)
        //        {
        //            App.dbcontext.CalibControlAntigens.Add(new CalibControlAntigen
        //            {
        //                AntigenId = antigen.AntigenId,
        //                CalibControlId = CalibControl.CalibControlId,
        //                CreatedBy = App.LoggedInUser.UserId,
        //                CreatedDt = DateTime.Now
        //            });
        //        }
        //        App.dbcontext.SaveChanges();
        //    }
        //}
        //catch (Exception ex)
        //{
        //    var logNumber = Logger.Log(nameof(UpdateCalibControl), new Dictionary<string, object>
        //    {
        //        { LogConsts.Exception, ex }
        //    });

        //    ex.Data.Add(nameof(logNumber), logNumber);
        //    throw ex;
        //}

        //return null;
        //}

        //public void DeleteCalibControl(CalibControl CalibControl)
        //{
        //try
        //{
        //    // Delete CalibControl
        //    var CalibControlToDelete = App.dbcontext.CalibControls.Find(CalibControl.CalibControlId);
        //    App.dbcontext.CalibControls.Remove(CalibControlToDelete);

        //    // Record Audit
        //    Audit audit = new Audit
        //    {
        //        RecordId = CalibControl.CCId,
        //        Type = AuditTypes.CalibControl.ToString(),
        //        Description = AuditEvents.CalibControlDeleted.ToString(),
        //        UpdatedBy = App.LoggedInUser.UserId,
        //        UpdatedDt = DateTime.Now
        //    };
        //    App.dbcontext.Audits.Add(audit);
        //    App.dbcontext.SaveChanges();

        //    // Delete CalibControlAntigens related to the deleted CalibControl
        //    var CalibControlAntigens = App.dbcontext.CalibControlAntigens.Where(a => a.CalibControlId == CalibControl.CalibControlId).ToList();
        //    App.dbcontext.CalibControlAntigens.RemoveRange(CalibControlAntigens);
        //    App.dbcontext.SaveChanges();
        //}
        //catch (Exception ex)
        //{
        //    var logNumber = Logger.Log(nameof(DeleteCalibControl), new Dictionary<string, object>
        //    {
        //        { LogConsts.Exception, ex }
        //    });

        //    ex.Data.Add(nameof(logNumber), logNumber);
        //    throw ex;
        //}
        //}

        public List<CC.CalibControl> GetAllCalibControls(string arrayId)
        {
            try
            {
                return App.dbcontext.CalibControls.Where(a => a.ArrayId == arrayId).ToList();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(GetAllCalibControls), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        public List<CC.ActiveCCs> GetExistingCC(string arrayId, string antigenId, string type)
        {
            try
            {
                return App.dbcontext.GetExistingCCs(arrayId, antigenId, type).ToList();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(GetAllCalibControls), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex },
                    { nameof(arrayId), arrayId },
                    { nameof(antigenId), antigenId},
                    { nameof(type), type }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        public void AssignCC(AssignedCC assignedCC)
        {

        }
    }
}
