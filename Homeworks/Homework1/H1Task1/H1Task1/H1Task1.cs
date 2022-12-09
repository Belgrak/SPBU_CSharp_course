using H1Task1;

var list = new List<int>();
try
{
    list = Console.ReadLine()?.Trim().Split().Select(int.Parse).ToList() ?? new List<int>();
}
catch (FormatException e)
{
    Console.WriteLine(e.Message);
    return;
}

Console.WriteLine("Result: " + string.Join(", ", InsertionSort.Sort(list)));