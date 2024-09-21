using CustomerManage.API.Datas;
using CustomerManage.API.Entities;
using MediatR;

namespace CustomerManage.API.Events
{
    public class RankNotificationHandler : INotificationHandler<CustomerEvent>
    {
        //public Task Handle(CustomerEvent notification, CancellationToken cancellationToken)
        //{
        //    long customerid = notification.Id;
        //    int score = notification.Score;
        //    var newCustomerRank = new CustomerRankEntity() { CustomerId = customerid, Score = score };
        //    DatasetConst.Customers.TryGetValue(customerid, out CustomerRankEntity? oldCustomerRank);
        //    if (oldCustomerRank != null)
        //    {
        //        if (oldCustomerRank.Score == score) return Task.FromResult(0);
        //        else DatasetConst.Customers.Remove(customerid, out oldCustomerRank);
        //    }
        //    DatasetConst.Customers.TryAdd(customerid, newCustomerRank);
        //    List<CustomerRankEntity> sortList = [.. DatasetConst.Customers.Values];
        //    sortList.Sort((c1, c2) =>
        //    {
        //        int scoreComparison = c2.Score.CompareTo(c1.Score);
        //        if (scoreComparison == 0) return c1.CustomerId.CompareTo(c2.CustomerId);
        //        return scoreComparison;
        //    });
        //    int rank = sortList.FindIndex(x => x.CustomerId == customerid);
        //    newCustomerRank.Rank = rank + 1;
        //    DatasetConst.Customers.TryUpdate(customerid, newCustomerRank, default!);
        //    for (int i = rank; i < sortList.Count; i++)
        //    {
        //        int key = i + 1;
        //        if (DatasetConst.CustomerRanks.ContainsKey(key))
        //        {
        //            DatasetConst.CustomerRanks[key] = new CustomerRankEntity() { CustomerId = sortList[i].CustomerId, Score = sortList[i].Score, Rank = key };                    
        //        }
        //        else
        //        {
        //            sortList[i].Rank = key;
        //            DatasetConst.CustomerRanks.TryAdd(key, sortList[i]);
        //        }
        //    }
        //    return Task.FromResult(0);
        //}

        public Task Handle(CustomerEvent notification, CancellationToken cancellationToken)
        {
            long customerid = notification.Id;
            int score = notification.Score;
            var newCustomerRank = new CustomerRankEntity() { CustomerId = customerid, Score = score };
            DatasetConst.Customers.TryGetValue(customerid, out CustomerRankEntity? oldCustomerRank);
            if (oldCustomerRank != null)
            {
                if (oldCustomerRank.Score == score) return Task.FromResult(0);
                else DatasetConst.Customers.Remove(customerid, out oldCustomerRank);
            }
            DatasetConst.Customers.TryAdd(customerid, newCustomerRank);

            int index = DatasetConst.RankSortList.FindIndex(x => x.CustomerId == customerid);            
            newCustomerRank.Rank = index + 1;
            DatasetConst.Customers.TryUpdate(customerid, newCustomerRank, default!);
            for (int i = index; i < DatasetConst.RankSortList.Count; i++)
            {
                int key = i + 1;
                if (DatasetConst.CustomerRanks.ContainsKey(key))
                {
                    DatasetConst.CustomerRanks[key] = new CustomerRankEntity() { CustomerId = DatasetConst.RankSortList[i].CustomerId, Score = DatasetConst.RankSortList[i].Score, Rank = key };
                }
                else
                {
                    DatasetConst.RankSortList[i].Rank = key;
                    DatasetConst.CustomerRanks.TryAdd(key, DatasetConst.RankSortList[i]);
                }
            }
            return Task.FromResult(0);
        }
    }
}
