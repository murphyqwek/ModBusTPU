namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int m = 4400000;
            for (int i = m / 2; i < m; i++)
            {
                for(int j = m / 2; j < m; j++)
                {
                    if ((i + j) % m == ((i * (j - 1)) % m) &&
                        (i + j - 1) % m == ((i * j) % m))
                    {
                        Console.WriteLine(i);
                        Console.WriteLine(j);
                    } 
                }
            }
        }
    }
}