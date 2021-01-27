using CC.Constants;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CC.Providers
{
    public class ArrayProvider
    {
        public string CreateArray(Array array, Dictionary<string, List<Antigen>> antigensGroups)
        {
            try
            {
                var existingArray = App.dbcontext.Arrays.FirstOrDefault(a => a.ArrayName == array.ArrayName);
                if (existingArray == null)
                {
                    array.CreatedBy = App.LoggedInUser.UserId;
                    array.CreatedDt = DateTime.Now;
                    array.ArrayId = Guid.NewGuid().ToString();

                    App.dbcontext.Arrays.Add(array);
                    App.dbcontext.SaveChanges();

                    foreach (var group in antigensGroups)
                    {
                        foreach (var antigen in group.Value)
                        {
                            App.dbcontext.ArrayAntigens.Add(new ArrayAntigen
                            {
                                AntigenId = antigen.AntigenId,
                                ArrayId = array.ArrayId,
                                Group = group.Key,
                                CreatedBy = App.LoggedInUser.UserId,
                                CreatedDt = DateTime.Now
                            });
                        }
                    }
                    App.dbcontext.SaveChanges();
                }
                else
                {
                    return Messages.ArrayAlreadyExists;
                }
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(CreateArray), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                return $"{ Messages.Exception} - log: {logNumber}";
            }

            return string.Empty;
        }

        public string UpdateArray(Array array, List<Antigen> AddedAntigens, List<Antigen> removedAntigens)
        {
            try
            {
                // update Array
                array.UpdatedBy = App.LoggedInUser.UserId;
                array.UpdatedDt = DateTime.Now;

                // record audit
                Audit audit = new Audit
                {
                    RecordId = array.ArrayId,
                    Type = AuditTypes.Array.ToString(),
                    Description = AuditEvents.ArrayUpdated.ToString(),
                    UpdatedBy = App.LoggedInUser.UserId,
                    UpdatedDt = DateTime.Now
                };

                App.dbcontext.Audits.Add(audit);
                App.dbcontext.SaveChanges();

                // delete any antigens removed from array
                if (removedAntigens != null && removedAntigens.Any())
                {
                    var arrayAntigens = App.dbcontext.ArrayAntigens.Where(a => a.ArrayId == array.ArrayId).ToList();
                    var removedAntigenIds = removedAntigens.Select(a => a.AntigenId).ToList();
                    App.dbcontext.ArrayAntigens.RemoveRange(arrayAntigens.Where(a => removedAntigenIds.Contains(a.AntigenId)));

                    App.dbcontext.SaveChanges();
                }

                // Create any new antigens added to array
                if (AddedAntigens != null && AddedAntigens.Any())
                {
                    foreach (var antigen in AddedAntigens)
                    {
                        App.dbcontext.ArrayAntigens.Add(new ArrayAntigen
                        {
                            AntigenId = antigen.AntigenId,
                            ArrayId = array.ArrayId,
                            CreatedBy = App.LoggedInUser.UserId,
                            CreatedDt = DateTime.Now
                        });
                    }
                    App.dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(UpdateArray), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }

            return null;
        }

        public void DeleteArray(Array array)
        {
            try
            {
                // Delete Array
                var ArrayToDelete = App.dbcontext.Arrays.Find(array.ArrayId);
                App.dbcontext.Arrays.Remove(ArrayToDelete);

                // Record Audit
                Audit audit = new Audit
                {
                    RecordId = array.ArrayId,
                    Type = AuditTypes.Array.ToString(),
                    Description = AuditEvents.ArrayDeleted.ToString(),
                    UpdatedBy = App.LoggedInUser.UserId,
                    UpdatedDt = DateTime.Now
                };
                App.dbcontext.Audits.Add(audit);
                App.dbcontext.SaveChanges();

                // Delete arrayAntigens related to the deleted array
                var arrayAntigens = App.dbcontext.ArrayAntigens.Where(a => a.ArrayId == array.ArrayId).ToList();
                App.dbcontext.ArrayAntigens.RemoveRange(arrayAntigens);
                App.dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(DeleteArray), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        public List<CC.Array> GetAllArrays(bool returnOnlyMaster)
        {
            try
            {
                if (returnOnlyMaster)
                    return App.dbcontext.Arrays.Where(a => string.IsNullOrEmpty(a.MasterArrayId)).OrderBy(a=>a.ArrayName).ToList();
                else
                    return App.dbcontext.Arrays.OrderBy(a => a.ArrayName).ToList();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(GetAllArrays), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        public List<CC.ArrayAntigen> GetArrayAntigens(string ArrayId)
        {
            try
            {
                return App.dbcontext.ArrayAntigens.Where(a => a.ArrayId == ArrayId).ToList();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(GetArrayAntigens), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        public Array GetArrayByLIMArrayNumber(string LimArrayNumber)
        {
            try
            {
                var limArray = new SqlParameter("@LIMArrayNumber", LimArrayNumber);
                var array = App.dbcontext.Database.SqlQuery<Array>("GetArrayByLIMNumber @LIMArrayNumber", limArray)?.FirstOrDefault();
                return array;
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(GetArrayAntigens), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }
    }
}
