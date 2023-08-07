namespace CashMachine
{
  /// <summary>
  /// Класс, который реализует простейшую логику банкомата по набору заданной суммы из доступного
  /// множества банкнот.
  /// </summary>
  public class CashMachineHelper
  {
    private Stack<int> _banknotes = new();

    /// <param name="banknotes">Банкноты доступные для выдачи на момент текущей операции.</param>
    /// <exception cref="ArgumentNullException">коллекция с банкнотами равна null</exception>
    public CashMachineHelper(IReadOnlyCollection<BanknoteSet> banknotes)
    {
      if (banknotes is null)
      {
        throw new ArgumentNullException(nameof(banknotes));
      }

      foreach (var set in banknotes)
      {
        for (int i = 0; i < set.Count; i++)
        {
          _banknotes.Push(set.Nominal);
        }
      }
    }

    /// <summary>
    /// Из доступного множества банкнот набирает сумму из минимального числа банкнот
    /// </summary>
    /// <param name="amount">Требуемая сумма</param>
    /// <returns>Возвращает набор купюр с помощью которого сумма может быть набрана
    /// или null, если набрать заданную сумму невозможно.
    /// </returns>
    public List<BanknoteSet>? CollectAmount(int amount)
    {
      var banknotesCount = _banknotes.Count;
      var takenBanknotes = new List<int>();
      var matrix = new List<int>[banknotesCount, banknotesCount];

      // x - количество купюр(столбцы)
      for (int x = 0; x < banknotesCount; x++)
      {
        // y - конкретная купюра или любая предыдущая(строки)
        for (int y = 0; y < banknotesCount; y++)
        {
          // для нахождения оптимального варианта стоит начинать с меньших номиналов
          _banknotes = new Stack<int>(_banknotes.OrderByDescending(b => b));

          // берем y + 1 купюр
          for (int i = -1; i < y; i++)
          {
            takenBanknotes.Add(_banknotes.Pop());
          }

          matrix[x, y] = new();

          // ищем все купюры номиналом меньше требуемой суммы
          var possibleVariant = takenBanknotes.Where(b => b <= amount);
          if (possibleVariant.Any())
          {
            // если такие есть, берем максимальную
            var possibleSum = possibleVariant.Max();

            // добавляем в комбинацию
            matrix[x, y].Add(possibleSum);

            // убираем из пула
            takenBanknotes.Remove(possibleSum);

            // пока купюр меньше чем x + 1 - добираем
            while (matrix[x, y].Count < x + 1)
            {
              // ищем все купюры номиналом меньше требуемой суммы из оставшихся
              var localVariant = takenBanknotes.Where(b => b <= amount - matrix[x, y].Sum());
              if (localVariant.Any())
              {
                // если такие есть, берем максимальную
                var localSum = localVariant.Max();

                // добавляем в комбинацию
                matrix[x, y].Add(localSum);

                // убираем из пула
                takenBanknotes.Remove(localSum);
              }
              else
              {
                // если пул закончился, продолжаем
                break;
              }
            }

            // если текущая комбинация равна требуемой сумме - возвращаем результат
            // поскольку мы идем от минимального набора купюр, последующие варианты будут менее оптимальными
            if (matrix[x, y].Sum() == amount)
            {
              return matrix[x, y]
                .GroupBy(b => b)
                .Select(b => new BanknoteSet(nominal: b.Key, count: b.Count()))
                .ToList();
            }
            // в противном случае - возвращаем в пул купюры из несостоявшейся комбинации
            else
            {
              takenBanknotes.AddRange(matrix[x, y]);
            }

            // возвращаем купюры из текущего пула в условный банкомат
            foreach (var banknote in takenBanknotes)
            {
              _banknotes.Push(banknote);
            }

            // не насилуем GC
            matrix[x, y] = null;
            // очищаем пул для дальнейших итераций
            takenBanknotes.Clear();
          }
          else
          {
            // возвращаем купюры из текущего пула в условный банкомат
            foreach (var banknote in takenBanknotes)
            {
              _banknotes.Push(banknote);
            }

            // не насилуем GC
            matrix[x, y] = null;

            // очищаем пул для дальнейших итераций
            takenBanknotes.Clear();
          }
        }
      }

      // из заданных купюр составить комбинацию невозможно
      return null;
    }
  }
}