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
            DatasetConst.SortAddItem(new CustomerRankEntity() { CustomerId = customerid, Score = score });
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
                // DatasetConst.CustomerRanks.TryGetValue(i, out CustomerRankEntity? rankEntity);
                CustomerRankEntity rankEntity = DatasetConst.RankSortList[i - 1];
                rankEntity.Rank = i;
                // if (rankEntity != null) result.Add(rankEntity);
                result.Add(rankEntity);
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
            // DatasetConst.Customers.TryGetValue(customerid, out CustomerRankEntity? customer);
            int index = DatasetConst.RankSortList.FindIndex(x => x.CustomerId == customerid);
            //if(customer!=null)
            if (index != -1)
            {
                // int rank = customer.Rank;
                int start = index - high;
                if (start < 0) start = 0;
                int end = index + low;
                if (end > DatasetConst.RankSortList.Count) end = DatasetConst.RankSortList.Count;
                for (int i = start; i <= end; i++)
                {
                    // DatasetConst.CustomerRanks.TryGetValue(i, out CustomerRankEntity? rankEntity);
                    CustomerRankEntity rankEntity = DatasetConst.RankSortList[i];
                    // if (rankEntity != null) result.Add(rankEntity);
                    result.Add(rankEntity);
                }
            }
            await Task.CompletedTask;
            return result;
        }
    }
}
