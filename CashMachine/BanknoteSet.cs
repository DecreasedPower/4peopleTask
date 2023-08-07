namespace CashMachine
{
  public class BanknoteSet
  {
    public int Nominal { get; private set; }
    public int Count { get; private set; }

    public BanknoteSet(int nominal, int count)
    {
      Nominal = nominal;
      Count = count;
    }
  }
}
