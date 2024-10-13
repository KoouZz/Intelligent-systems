using System.Diagnostics;

public class App
{
    public int x;
    public int resolution;
    double[,] result;
    double[,] ar1;
    double[,] ar2;

    public App(int x, int resolution)
    {
        this.x = x;
        this.resolution = resolution;
        result = new double[resolution, resolution];
        ar1 = CreateArray(resolution);
        ar2 = CreateArray(resolution);
    }

    public void DGEMM_BLAS(int start, int end)
    {
        for (int k = 0; k < resolution; k++)
        {
            for (int i = start; i < end; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    result[k, i] += ar1[k, j] * ar2[j, i];
                }
            }
        }
    }

    public void DGEMM_BLASS_PARALLEL()
    {
        ParallelOptions options = new ParallelOptions
        {
            MaxDegreeOfParallelism = x // Устанавливаем максимальное количество параллельных потоков
        };

        Parallel.For(0, resolution, options, i =>
        {
            for (int j = 0; j < resolution; j++)
            {
                for (int k = 0; k < resolution; k++)
                {
                    result[i, j] += ar1[i, k] * ar2[k, j];
                }
            }
        });
    }

    private double[,] CreateArray(int res)
    {
        Random rd = new Random();
        double[,] a = new double[res, res];
        for (int i = 0; i < res; i++)
        {
            for (int j = 0; j < res; j++)
            {
                a[i, j] = rd.NextDouble() * 10;
            }
        }
        return a;
    }

    public void PrintResult()
    {
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                Console.Write(Math.Ceiling(result[i, j] * 100) / 100 + "\t");
            }
            Console.WriteLine();
        }
    }
}

public class ThreadApp
{
    static void Main()
    {

        Stopwatch sw = new Stopwatch();

        Console.WriteLine("Введите способ выполнения задачи: ");
        Console.WriteLine("1 - Библиотека Thread");
        Console.WriteLine("2 - Метод Parallel.For");
        Console.WriteLine("3 - Тест производительности");
        Console.WriteLine("0 - Выход");
        bool correct = true;
        while (correct)
        {
            Console.Write("Способ: ");
            string key = Console.ReadLine();
            Console.WriteLine();
            switch (key)
            {
                case "1":
                    Console.Write("Введите количество потоков: ");
                    int x = Convert.ToInt32(Console.ReadLine());

                    Console.Write("Введите размерность матрицы: ");
                    int resolution = Convert.ToInt32(Console.ReadLine());

                    App app = new App(x, resolution);
                    sw.Start();

                    Thread[] threads = new Thread[x];
                    for (int i = 0; i < x; i++)
                    {
                        int start = i * resolution / x;
                        int end = (i == x - 1) ? resolution : (i + 1) * resolution / x;
                        threads[i] = new Thread(() => app.DGEMM_BLAS(start, end));
                        threads[i].Start();
                    }

                    foreach (Thread thread in threads)
                    {
                        thread.Join();
                    }

                    sw.Stop();
                    correct = false;
                    break;

                case "2":
                    Console.Write("Введите количество потоков: ");
                    int z = Convert.ToInt32(Console.ReadLine());

                    Console.Write("Введите размерность матрицы: ");
                    int resolution2 = Convert.ToInt32(Console.ReadLine());

                    App app1 = new App(z, resolution2);

                    sw.Start();

                    app1.DGEMM_BLASS_PARALLEL();

                    sw.Stop();

                    //app.PrintResult();
                    correct = false;
                    break;

                case "3":
                    int time = 100000;
                    int HowManyThreads = 1;
                    for (int threadsTest = 1; threadsTest <= Environment.ProcessorCount * 2; threadsTest++)
                    {
                        App appTest = new App(threadsTest, 500);

                        sw.Start();

                        appTest.DGEMM_BLASS_PARALLEL();

                        sw.Stop();
                        Console.WriteLine($"Количество потоков: {threadsTest}, Время выполнения: {sw.ElapsedMilliseconds} ms");
                        if (Convert.ToInt32(sw.ElapsedMilliseconds) < time)
                        {
                            time = Convert.ToInt32(sw.ElapsedMilliseconds);
                            HowManyThreads = threadsTest;
                        }
                        sw.Reset();
                    }
                    Console.WriteLine("\n___________Тест производительности показала___________");
                    Console.WriteLine($"Оптимальное число потоков: {HowManyThreads}");
                    Console.WriteLine("\n\nВведите способ выполнения задачи: ");
                    Console.WriteLine("1 - Библиотека Thread");
                    Console.WriteLine("2 - Метод Parallel.For");
                    Console.WriteLine("3 - Тест производительности");
                    Console.WriteLine("0 - Выход");
                    break;

                case "0":
                    correct = false;
                    break;

                default:W
                    Console.WriteLine("Некорректный ввод. Попробуйте еще раз\n\n");
                    Console.WriteLine("Введите способ выполнения задачи: ");
                    Console.WriteLine("1 - Библиотека Thread");
                    Console.WriteLine("2 - Метод Parallel.For");
                    Console.WriteLine("3 - Тест производительности");
                    Console.WriteLine("0 - Выход");
                    break;
            }
        }

        //app.PrintResult();
        Console.WriteLine($"Время выполнения: {sw.ElapsedMilliseconds} ms");
    }
}
