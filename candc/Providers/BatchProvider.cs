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
        public string CreateBatch(Batch batchInfo, List<BatchAntigen> batchAntigens)
        {
            try
            {
                var existingBatch = App.dbcontext.GetBatchRecords(batchInfo.BatchName, batchInfo.RunDate, batchInfo.BlockNumber, batchInfo.AntigenGroup);

                if (existingBatch == null || !existingBatch.Any())
                {
                    foreach (var bA in batchAntigens)
                    {
                        App.dbcontext.Batches.Add(new Batch
                        {
                            BatchName = batchInfo.BatchName,
                            RunDate = batchInfo.RunDate,
                            BlockNumber = batchInfo.BlockNumber,
                            AntigenGroup = batchInfo.AntigenGroup,
                            LotNumber = bA.LotNumber,
                            AntigenId = bA.AntigenId,
                            CreatedBy = App.LoggedInUser.UserId,
                            CreatedDt = DateTime.Now
                        });
                    }

                    App.dbcontext.SaveChanges();
                }
                else
                {
                    return Messages.BatchAlreadyExist;
                }
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(CreateBatch), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex },
                    { nameof(batchInfo.BatchName), batchInfo.BatchName },
                    { nameof(batchInfo.RunDate), batchInfo.RunDate },
                    { nameof(batchInfo.BlockNumber), batchInfo.BlockNumber }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                return $"{ Messages.Exception} - log: {logNumber}";
            }

            return string.Empty;
        }
    }
}
