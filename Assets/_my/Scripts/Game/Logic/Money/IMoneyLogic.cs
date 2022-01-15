using System;
using System.Numerics;

namespace Game {
    public interface IMoneyLogic : IInitializable {
        BigInteger Income { get; }
        int IncomePercentMultiplier { get; }
        BigInteger Money { get; }
        float PayInterval { get; }

        bool IsEnoughMoney(BigInteger price);
        
        void AddMoney(BigInteger value);
        void SpendMoney(BigInteger value, Action onSuccess = null, Action onFail = null);

        BigInteger ReceivePayFor(int level);
        
        void AddDiscount(int amount);

        BigInteger GetPriceWithDiscount(BigInteger price);
    }
}