using UnityEngine;
using System;

public class CsvOperations : MonoBehaviour
{
    public static string[,] CsvToArray(string data)
    {
        data.Replace("\n", Environment.NewLine );
        string[] rows = data.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);

        string[,] output = new string[rows[0].Split(';').Length, rows.Length];

        for (int y = 0; y < rows.Length; y++)
        {
            string[] column = rows[y].Split(';');
            for (int x = 0; x < column.Length; x++)
            {
                output[x, y] = column[x];
            }
        }

        return output;
    }
}
