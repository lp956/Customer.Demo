using MediatR;

namespace CustomerManage.API.Events
{
    public class CustomerEvent(long customerId, int score) : INotification
    {
        public long Id { get; set; } = customerId;
        public int Score { get; set; } = score;
    }
}
