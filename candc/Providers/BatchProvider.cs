using CC.Constants;
using CC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Providers
{
    public class BatchProvider
    {
        public string CreateBatch(Batch newBatchInfo, List<BatchAntigen> batchAntigens, List<Batch> existingBatch)
        {
            try
            {

                foreach (var bA in batchAntigens)
                {
                    var existingRecord = existingBatch?.FirstOrDefault(a => a.AntigenId == bA.AntigenId && !string.Equals(a.LotNumber, bA.LotNumber, StringComparison.OrdinalIgnoreCase));
                    if (existingRecord != null) // existing batch record with different lot number. so needs to be updated
                    {
                        existingRecord.LotNumber = bA.LotNumber;
                    }
                    // new record needs to be create
                    else if (!string.IsNullOrEmpty(bA.LotNumber))
                    {
                        App.dbcontext.Batches.Add(new Batch
                        {
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
                var logNumber = Logger.Log(nameof(CreateBatch), new Dictionary<string, object>
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

        public List<Batch> GetBatchRecords(string batchName, DateTime runDate, int blockNumber, string antigenGroup)
        {
            try
            {
                return App.dbcontext.GetBatchRecords(batchName, runDate, blockNumber, antigenGroup)?.ToList();
            }
            catch (Exception ex)
            {

                var logNumber = Logger.Log(nameof(CreateBatch), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex },
                    { nameof(batchName), batchName },
                    { nameof(runDate), runDate },
                    { nameof(blockNumber), blockNumber },
                    {nameof(antigenGroup), antigenGroup }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                return null;
            }
        }
    }
}
