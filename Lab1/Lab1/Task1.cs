using System.Diagnostics;



public class App
{
    public int x = Convert.ToInt32(Console.ReadLine());

    public int resolution = Convert.ToInt32(Console.ReadLine());
    double[,] result;
    double[,] ar1;
    double[,] ar2;
    public void DGEMM_BLAS(int A, int start, int end)
    {
        if (result == null)
        {
            result = new double[A, A];
            ar1 = CreateArray(A);
            ar2 = CreateArray(A);
        }
        int c = 0;
        for (int k = 0; k < A; k++)
        {
            c = start;
            for (int i = 0; i < A; i++)
            {
                result[k, c] += ar1[k, i] * ar2[i, c];
                if ((i + 1 >= end) && (c + 1 < A))
                {
                    i = -1;
                    c++;
                }
            }
        }
        for (int i = 0; i < A; i++)
        {
            for (int j = 0; j < A; j++)
            {
                Console.Write(Math.Ceiling(result[i, j] * 100) / 100 + "\t");
            }
            Console.WriteLine();
        }
    }
    private double[,] CreateArray(int res)
    {
        Random rd = new Random();
        double[,] a = new double[res, res];
        for (int k = 0; k < res; k++)
        {
            for (int i = 0; i < res; i++)
            {
                a[i,k] = rd.NextDouble() * 10;
            }
        }
        return a;
    }
}

public class ThreadApp
{
    static void Main()
    {
        Stopwatch sw = new Stopwatch();

        sw.Start();

        App app = new App();
        ThreadApp threadApp = new ThreadApp();
        Thread.CurrentThread.Name = "main";
        for (int i = 0; i < app.x - 1; i++)
        {
            Thread th = new Thread(new ParameterizedThreadStart(Work));
            th.Name = $"th{i}";
            th.Start(app);
        }
        app.DGEMM_BLAS(app.resolution, 0, app.resolution / app.x);
        Thread.CurrentThread.Join();
        sw.Stop();
        Console.WriteLine(sw);
    }


    static void Work(object apClass)
    {

        App data = (App)apClass;
        int id = Convert.ToInt32(Thread.CurrentThread.Name.Substring(2));
        if ((data.resolution % data.x != 0) & (id + 1 == data.x))
        {
            data.DGEMM_BLAS(data.resolution, data.resolution / data.x * (id + 1), data.resolution / data.x * (id + 2) + data.resolution % data.x);
        } else
        {
            data.DGEMM_BLAS(data.resolution, data.resolution / data.x * (id + 1), data.resolution / data.x * (id + 2));
        }

    }
}
