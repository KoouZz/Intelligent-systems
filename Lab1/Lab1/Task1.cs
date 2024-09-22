public class App
{
    static void Main()
    {
        App ap = new App();
        ap.x = Convert.ToInt32(Console.ReadLine());
        ap.DGEMM_BLAS(ap.x);
    }
    int x;
    private void DGEMM_BLAS(int A)
    {
        double[,] result = new double[A, A];
        double[,] ar1 = CreateArray(A);
        double[,] ar2 = CreateArray(A);
        int c;
        for (int k = 0; k < A; k++)
        {
            c = 0;
            for (int i = 0; i < A; i++)
            {
                result[k, c] += ar1[k, i] * ar2[i, c];
                if ((i + 1 >= A) && (c + 1 < A))
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
