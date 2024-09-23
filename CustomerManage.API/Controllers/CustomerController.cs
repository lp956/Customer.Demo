using CustomerManage.API.Datas;
using CustomerManage.API.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManage.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        /// <summary>
        /// update customer score
        /// </summary>
        /// <param name="customerid"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpPost("{customerid:long}/score/{score:int}")]
        public async Task<string> PostAsync(long customerid, int score)
        {            
            if (score < -1000 || score > 1000) throw new ArgumentException("'score' required decimal number in range of [-1000,1000].");
            var newCustomerRank = new CustomerRankEntity() { CustomerId = customerid, Score = score };
            // Real-world development can use scheduled tasks to periodically process leaderboards and write them to redis .
            DatasetConst.SortAddItem(newCustomerRank);
            // Use ConcurrentDictionary to simulate a database id search .
            DatasetConst.Customers.TryGetValue(customerid, out CustomerRankEntity? oldCustomerRank);
            if (oldCustomerRank != null)
            {
                if (oldCustomerRank.Score == score) return "Current score after update";
                else DatasetConst.Customers.Remove(customerid, out oldCustomerRank);
            }
            DatasetConst.Customers.TryAdd(customerid, newCustomerRank);
            // Update only the changes in the leaderboard
            int index = DatasetConst.RankSortList.FindIndex(x => x.CustomerId == customerid);
            newCustomerRank.Rank = index + 1;
            DatasetConst.Customers.TryUpdate(customerid, newCustomerRank, default!);
            for (int i = index; i < DatasetConst.RankSortList.Count; i++)
            {
                int key = i + 1;
                if (DatasetConst.CustomerRanks.ContainsKey(key))
                {
                    DatasetConst.Customers[DatasetConst.RankSortList[i].CustomerId].Rank = key; 
                    DatasetConst.CustomerRanks[key] = new CustomerRankEntity() { CustomerId = DatasetConst.RankSortList[i].CustomerId, Score = DatasetConst.RankSortList[i].Score, Rank = key };
                }
                else
                {
                    DatasetConst.RankSortList[i].Rank = key;
                    DatasetConst.CustomerRanks.TryAdd(key, DatasetConst.RankSortList[i]);
                }
            }
            // The interface with high concurrency can also import message queues .
            //await this._mediator.Publish(new CustomerEvent(customerid, score));
            await Task.CompletedTask;
            return "Current score after update";
        }

        /// <summary>
        /// Get customers by rank
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet("leaderboard")]
        public async Task<List<CustomerRankEntity>> GetAsync(int start, int end)
        {
            List<CustomerRankEntity> result = [];
            if (start < 0) start = 0;
            if (end > DatasetConst.RankSortList.Count) end = DatasetConst.RankSortList.Count;
            for (int i = start; i <= end; i++)
            {
                DatasetConst.CustomerRanks.TryGetValue(i, out CustomerRankEntity? rankEntity);
                //CustomerRankEntity rankEntity = DatasetConst.RankSortList[i - 1];
                // rankEntity.Rank = i;
                // result.Add(rankEntity);
                if (rankEntity != null) { result.Add(rankEntity); }
            }
            await Task.CompletedTask;
            return result;
        }

        /// <summary>
        /// Get customers by rank
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet("leaderboard/{customerid:long}")]
        public async Task<List<CustomerRankEntity>> GetAsync(long customerid, int high, int low)
        {
            List<CustomerRankEntity> result = [];
            DatasetConst.Customers.TryGetValue(customerid, out CustomerRankEntity? customer);
            // int rank = DatasetConst.RankSortList.FindIndex(x => x.CustomerId == customerid);
            // if (rank != -1)
            if (customer != null)                
            {
                int rank = customer.Rank;
                int start = rank - high;
                if (start < 0) start = 0;
                int end = rank + low;
                // if (end > DatasetConst.RankSortList.Count) end = DatasetConst.RankSortList.Count;
                for (int i = start; i < end; i++)
                {
                    DatasetConst.CustomerRanks.TryGetValue(i, out CustomerRankEntity? rankEntity);
                    if (rankEntity != null) result.Add(rankEntity);
                    // CustomerRankEntity rankEntity = DatasetConst.RankSortList[i];
                    // result.Add(rankEntity);
                }
            }
            await Task.CompletedTask;
            return result;
        }
    }
}
