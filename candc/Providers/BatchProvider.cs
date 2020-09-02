using CC.Constants;
using CC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Providers
{
    public class BatchProvider
    {
        public string UpsertBatch(Batch newBatchInfo, List<BatchAntigen> batchAntigens)
        {
            try
            { 
                foreach (var bA in batchAntigens)
                {
                    var existingRecord = App.dbcontext.Batches.FirstOrDefault(a =>
                                            a.BatchName == newBatchInfo.BatchName &&
                                            a.RunDate == newBatchInfo.RunDate &&
                                            a.BlockNumber == newBatchInfo.BlockNumber &&
                                            a.CCType == bA.Type.ToString() &&
                                            a.AntigenId == bA.AntigenId);

                    if (existingRecord != null) // existing batch record with different lot number. so needs to be updated
                    {
                        existingRecord.LotNumber = bA.LotNumber;
                        existingRecord.UpdatedBy = App.LoggedInUser.UserId;
                        existingRecord.UpdatedDt = DateTime.Now;

                        var audit = new Audit
                        {
                            RecordId = existingRecord.BatchId,
                            Type = AuditTypes.Batch.ToString(),
                            Description = AuditEvents.BatchRecordUpdated.ToString(),
                            UpdatedBy = App.LoggedInUser.UserId,
                            UpdatedDt = DateTime.Now
                        };

                        App.dbcontext.Audits.Add(audit);
                    }
                    // new record needs to be create
                    else if (!string.IsNullOrEmpty(bA.LotNumber))
                    {
                        App.dbcontext.Batches.Add(new Batch
                        {
                            BatchId = Guid.NewGuid().ToString(),
                            BatchName = newBatchInfo.BatchName,
                            RunDate = newBatchInfo.RunDate,
                            BlockNumber = newBatchInfo.BlockNumber,
                            AntigenGroup = newBatchInfo.AntigenGroup,
                            LotNumber = bA.LotNumber,
                            AntigenId = bA.AntigenId,
                            CCType = bA.Type.ToString(),
                            CreatedBy = App.LoggedInUser.UserId,
                            CreatedDt = DateTime.Now
                        });
                    }
                }

                App.dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(UpsertBatch), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex },
                    { nameof(newBatchInfo.BatchName), newBatchInfo.BatchName },
                    { nameof(newBatchInfo.RunDate), newBatchInfo.RunDate },
                    { nameof(newBatchInfo.BlockNumber), newBatchInfo.BlockNumber },
                    { nameof(newBatchInfo.AntigenGroup), newBatchInfo.AntigenGroup }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                return $"{ Messages.Exception} - log: {logNumber}";
            }

            return string.Empty;
        }

        public List<Batch> GetBatchRecordsInGroup(string batchName, DateTime runDate, int blockNumber, string antigenGroup)
        {
            try
            {
                var bName = new SqlParameter("@batchName", batchName);
                var bRundate = new SqlParameter("@runDate", runDate.ToShortDateString());
                var blockNum = new SqlParameter("@BlockNumber", blockNumber);
                var group = new SqlParameter("@antigenGroup", antigenGroup);

                return App.dbcontext.Database.SqlQuery<Batch>("GetBatchRecords @batchName, @runDate, @BlockNumber, @antigenGroup", bName, bRundate, blockNum, group)?.ToList();
            }
            catch (Exception ex)
            {

                var logNumber = Logger.Log(nameof(UpsertBatch), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex },
                    { nameof(batchName), batchName },
                    { nameof(runDate), runDate },
                    { nameof(blockNumber), blockNumber },
                    {nameof(antigenGroup), antigenGroup }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }
    }
}
