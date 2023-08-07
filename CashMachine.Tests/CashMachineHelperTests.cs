namespace CashMachine.Tests
{
  public class CashMachineHelperTests
  {
    List<BanknoteSet> banknotes = new List<BanknoteSet>()
    {
      new BanknoteSet(100, 3),
      new BanknoteSet(50, 10),
      new BanknoteSet(20, 7),
      new BanknoteSet(500, 4)
    };

    [Theory]
    [InlineData(400, 5)]
    [InlineData(1190, 6)]
    [InlineData(130, 5)]
    public void CashMachineHelper_GiveAmount_Success(int amount, int count)
    {
      CashMachineHelper cashMachine = new CashMachineHelper(banknotes);

      var result = cashMachine.CollectAmount(amount)!;
      Assert.NotNull(result);
      Assert.Equal(count, result.Sum(b => b.Count));

      var total = result.Sum(b => b.Count * b.Nominal);
      Assert.Equal(amount, total);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(111)]
    [InlineData(3500)]
    public void CashMachineHelper_GiveAmount_Fail(int amount)
    {
      CashMachineHelper cashMachine = new CashMachineHelper(banknotes);

      var result = cashMachine.CollectAmount(amount);
      Assert.Null(result);
    }
  }
}