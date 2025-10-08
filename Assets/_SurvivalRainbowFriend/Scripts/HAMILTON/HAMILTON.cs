using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAMILTON
{
    private static bool[,] a;
    private static bool[] Free;
    private static int[] X;

    private static int n = 7;
    private static List<int[]> Results;
  
    public static List<int[]> GetListHamilton(bool[,] _a, int _n)
    {
        Results = new List<int[]>();
        Init(_n);
        Input(_a);
        X[1] = 1; Free[1] = false;
        Try(2);
        return Results;
    }
   
    static void Init(int _n)
    {
        Results.Clear();
        n = _n;
        a = new bool[n, n];
        Free = new bool[n];
        X = new int[n];
        for (int i = 0; i < n; i++)
        {
            Free[i] = true;
            X[i] = 0;
        }
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i, j] = false;
            }
        }
    }
    static void Input(bool[,] _a)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i, j] = _a[i,j];
            }
        }
    }
    static void PrintResult()
    {
        //X[n - 1] = X[1];
        var list = new List<int>(X);
        list.RemoveAt(0);
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = list[i] - 1;
        }   
        Results.Add(list.ToArray());
    }
    static void Try(int i)
    {
        int j;
        for (j = 1; j < n; j++)
        {
            if (Free[j] && a[X[i - 1], j])
            {
                X[i] = j;
                if (i < n - 1)
                {
                    Free[j] = false;
                    Try(i + 1);
                    Free[j] = true;
                }
                else
                {

                    if (a[j, X[1]]) PrintResult();
                }
            }
        }
    }
}
