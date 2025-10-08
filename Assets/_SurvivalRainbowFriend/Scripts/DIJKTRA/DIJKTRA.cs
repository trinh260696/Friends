using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMini
{
    public int WIDTH;  
    public float[,] joints;
    public FieldMini()
    {
        
    }
    public FieldMini Clone()
    {
        FieldMini f = new FieldMini();
        f.WIDTH = this.WIDTH;
        
        f.joints = new float[WIDTH, WIDTH];
        for (int i = 0; i < WIDTH; i++)
            for (int j=0; j<WIDTH; j++)
            {
                f.joints[i, j] = this.joints[i, j];
            }
        return f;
    }
}
public class DIJKTRA 
{
        public static bool[] FREE;
        public static int[] TRACE;
        public static int S, F;
        public static float[,] C;
        public static float MAXC = 10000f;
        public static float[] D;
        public static FieldMini field;
       
        public static void InputSecond(int s, int f)
        {

            S = s; F = f;
            D = new float[field.WIDTH];
            TRACE = new int[field.WIDTH];
            for (int i = 0; i < field.WIDTH; i++)
            {
                D[i] = MAXC;
            }
            for (int i = 0; i < field.WIDTH; i++)
            {
                int c = i / field.WIDTH;
                int r = i % field.WIDTH;
                FREE[i] = true;
            }
            for (int i = 0; i < field.WIDTH; i++)
                for (int j = 0; j < field.WIDTH; j++)
                {
                    C[i, j] = field.joints[i, j];
                    if(i>j)
                     C[i, j] = C[j, i];

            }
            D[S] = 0;
        }
        public static void Init(FieldMini f)
        {
            field = f.Clone();
            FREE = new bool[f.WIDTH];
            C = new float[f.WIDTH, f.WIDTH];                    
        }
        public static int[] Output()
        {
            int i, u, v;
            float min;
            while (true)
            {
                u = -1;
                min = MAXC;
                for(i=0; i<field.WIDTH; i++)
                {
                    if(FREE[i] && D[i] < min)
                    {
                        min = D[i];
                        u = i;
                    }
                }
                if ( u == F || u==-1) break;
                FREE[u] = false;
                for (v = 0; v < field.WIDTH; v++)
                {
                    if (FREE[v] && D[v] > D[u]+C[u,v])
                    {
                        D[v] = D[u] + C[u, v];
                        TRACE[v] = u;
                    }
                }              
            }
            if (D[F] == MAXC) { 
                return null; }
            else
            {
                List<int> result = new List<int>();
                int tmp = F;
                string str = "";
                while (F != S)
                {
                    F = TRACE[F];
                    result.Add(F);
                    
                }
              //  result.Remove(S);
                result.Reverse();
                result.Add(tmp);
                for (int k = 0; k < result.Count; k++)
                {
                    str += result[k]+", ";
                }
               // Debug.LogWarning(str);
                // result.Add(F);
                return result.ToArray();

            }

        }
}
