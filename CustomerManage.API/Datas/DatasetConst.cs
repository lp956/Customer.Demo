using CustomerManage.API.Entities;
using System.Collections.Concurrent;

namespace CustomerManage.API.Datas
{
    public static class DatasetConst
    {
        public readonly static ConcurrentDictionary<long, CustomerRankEntity> Customers = [];

        public readonly static ConcurrentDictionary<long, CustomerRankEntity> CustomerRanks = [];

        public readonly static List<CustomerRankEntity> RankSortList = [];

        private static readonly ReaderWriterLockSlim rwLock = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCustomer"></param>
        public static void SortAddItem(CustomerRankEntity newCustomer)
        {
            int left = 0;
            int right = RankSortList.Count - 1;
            int index = RankSortList.FindIndex(x => x.CustomerId == newCustomer.CustomerId);
            if (index != -1 && RankSortList[index].Score == newCustomer.Score) return;
            rwLock.EnterWriteLock();
            try
            {                   
                if (index != -1) RankSortList.RemoveAt(index);
                while (left <= right)
                {
                    int mid = left + (right - left) / 2;
                    var midCustomer = RankSortList[mid];

                    if (midCustomer.Score > newCustomer.Score) left = mid + 1;                   
                    else if (midCustomer.Score < newCustomer.Score) right = mid - 1;                    
                    else
                    {                        
                        if (midCustomer.CustomerId < newCustomer.CustomerId) left = mid + 1;                        
                        else right = mid - 1;                        
                    }
                }
                newCustomer.Rank = left + 1;
                RankSortList.Insert(left, newCustomer);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally { rwLock.ExitWriteLock(); }
        }
    }
}
