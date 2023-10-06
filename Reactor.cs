// Original by kuilin (2016) in javascript, translated to C# by Bin#4027 (Jakub Gil) And ChatGPT
// PEL LABORATORIES, 2023

using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;

public class Reactor : MonoBehaviour
{
    float gcd_calc(float a, float b)
    {
        if (b == 0) return a;
        return gcd_calc(b, a % b);
    }

    string solve(string x)
    {
        float bigNumber = 1;
        Regex regex = new Regex(@"\s+D\s+");
        var arrayOfNumbers = new HashSet<string>(Regex.Split(x, @"\D+"));
        arrayOfNumbers.Remove("");
        foreach (string i in arrayOfNumbers)
        {
            bigNumber *= Int32.Parse(i);
        }
        string[] reactants = x.Split('?')[0].Split('+');
        string[] products = x.Split('?')[1].Split('+');
        int molecules = reactants.Length + products.Length;
        var regex2 = new Regex(@"\s+|[A-Z][a-z]*");
        var elems = new HashSet<string>(regex2.Matches(Regex.Replace(x, @"\s+|->", "")).OfType<Match>().Select(m => m.Value));
        elems.Remove("");

        List<List<float>> rrefArray = new List<List<float>>();
        foreach (string elem in elems)
        {
            List<float> buildArr = new List<float>();
            foreach (string reactant in reactants)
            {
                int index = reactant.IndexOf(elem);
                if (index == -1) buildArr.Add(0);
                else
                {
                    index += elem.Length;
                    Match numberAfterElement = Regex.Match(reactant.Substring(index), @"^\d+");
                    if (!numberAfterElement.Success)
                    {
                        buildArr.Add(1);
                    }
                    else
                    {
                        buildArr.Add(Int32.Parse(numberAfterElement.Value));
                    }
                }
            }
            foreach (string product in products)
            {
                int index = product.IndexOf(elem);
                if (index == -1) buildArr.Add(0);
                else
                {
                    index += elem.Length;
                    Match numberAfterElement = Regex.Match(product.Substring(index), @"^\d+");
                    if (numberAfterElement == null || !numberAfterElement.Success)
                    {
                        buildArr.Add(-1);
                    }
                    else
                    {
                        buildArr.Add(-1 * Int32.Parse(numberAfterElement.Value));
                    }
                }
            }
            rrefArray.Add(buildArr);
        }

        List<float> workingOnThisRow = null;
        for (int pivot = 0; pivot < Math.Min(reactants.Length + products.Length, elems.Count); pivot++)
        {
            for (int i = pivot; i < rrefArray.Count; i++)
            {
                List<float> row = rrefArray[i];
                if (row[pivot] != 0)
                {
                    int index = rrefArray.IndexOf(row);
                    workingOnThisRow = rrefArray[index];
                    rrefArray.RemoveAt(index);
                    
                }
                string test3 = "[";
                foreach (var y in row) test3 += $" {y},";
                Debug.Log($"{test3}]");
            }

            float multiplyWhat = bigNumber / workingOnThisRow[pivot];
            for (int i = 0; i < workingOnThisRow.Count; i++) workingOnThisRow[i] *= multiplyWhat;

            for (int i = 0; i < rrefArray.Count; i++)
            {
                List<float> row = rrefArray[i];
                if (row[pivot] != 0)
                {
                    multiplyWhat = bigNumber / row[pivot];
                    for (int j = 0; j < row.Count; j++)
                    {
                        row[j] *= multiplyWhat;
                        row[j] -= workingOnThisRow[j];
                        row[j] /= multiplyWhat;
                    }
                    rrefArray[i] = row;
                }
            }
            rrefArray.Insert(pivot, workingOnThisRow);
        }

        string test1 = "";
        string test2 = "";
        foreach (var z in rrefArray)
        {
            foreach (var y in z) test1 += $"{y}, ";
            test2 += $"[{test1}], ";
            test1 = "";
        }
        // Debug.Log(test2);

        if (rrefArray[0][elems.Count] == 0 || rrefArray[0][elems.Count] == 0) return "Nope!";

        bigNumber *= -1;
        List<float> coEffs = new List<float>();
        float gcd = bigNumber;
        for (int i = 0; i < rrefArray.Count; i++)
        {
            float num = rrefArray[i][molecules - 1];
            coEffs.Add(num);
            gcd = gcd_calc(gcd, num);
        }
        coEffs.Add(bigNumber);
        for (int i = 0; i < coEffs.Count; i++) coEffs[i] /= gcd;

        string outt = "";
        for (int i = 0; i < coEffs.Count; i++)
        {
            float coEff = coEffs[i];
            if (coEff != 1) outt += coEff;
            outt += reactants[0];
            reactants = reactants.Skip(1).ToArray();
            if (reactants.Length == 0 && products.Length != 0)
            {
                outt += '?';
                reactants = products;
            }
            else if (i != coEffs.Count - 1) outt += '+';
        }
        Debug.Log($"{reactants}, {products}");
        return outt.Remove(outt.Length - 1, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(solve("Na+H2O?NaOH+H2"));
        Debug.Log(solve("Na+Cl2?NaCl"));
        Debug.Log(solve("Ca+H2O?Ca(OH)2+H2"));
    }
}
