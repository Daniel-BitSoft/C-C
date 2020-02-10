using CC.Constants;
using CC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CC.Providers
{
    public class CCProvider
    {
        public string CreateCalibControl(CalibControl calibControl, List<CCAntigens> antigens, string CCType)
        {
            try
            {
                foreach (var antigen in antigens)
                {
                    calibControl.CreatedBy = App.LoggedInUser.UserId;
                    calibControl.CreatedDt = DateTime.Now;
                    calibControl.CCId = Guid.NewGuid().ToString();
                    calibControl.Type = CCType;
                    calibControl.AntigenId = antigen.AntigenId;
                    calibControl.Min = antigen.Min;
                    calibControl.Max = antigen.Max;

                    calibControl.LotNumber = "TODO: NEEDS TO BE CALCULATED";

                    App.dbcontext.CalibControls.Add(calibControl);
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

            return string.Empty;
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


    }
}
