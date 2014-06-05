using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerImporter {


    public static List<Customer> ProcessCSV(TextAsset csv) //This is where i want to be working "Tim"
    {

        string[,] grid = CSVReader.SplitCsvGrid(csv.text);
        //string[] lines = CSVReader.SplitCsvLine(csv.text);
        //Debug.Log(lines.Length);
        //Debug.Log(lines[3]);
        //Debug.Log(grid.GetLength(1));


        int sexIndex = 0;
        int ageIndex = 1;
        int ethnicityIndex = 2;
        int scenarioIndex = 3;
        int npsIndex = 6;
        int timeAvailableIndex = 7;
        int bestZoneIndex = 8;
        int secondBestZoneIndex = 9;
        int upsellIndex = 10;
        int spendIndex = 11;
        List<Customer> generatedCustomers = new List<Customer>(); // HERE it is

        string[] sexType = { "MALE", "FEMALE" };
        string[] ageType = { "TEENS", "ASPIRING", "FAMILY (PREGNANT/BABY)", "MIDDLE AGED (CASUAL)", "MIDDLE AGED (BUSINESS)", "PENSIONER" };
        string[] EthnicityType = { "FAIR", "MID", "DARK" };

        for (int y = 0; y < grid.GetLength(1) - 1; y++)
        {

            //Checks if the line is a customer line or not at all.
            if (grid[sexIndex, y].ToUpper() != "MALE" && grid[sexIndex, y].ToUpper() != "FEMALE")
            {
                if (grid[sexIndex, y].ToUpper().Trim() == "")
                {
                    break;
                }
            }
            else
            {

                string sex = grid[sexIndex, y].ToUpper();

                if (checkList(sex, sexType) == false)
                {
                    throw new System.Exception("Gender in row: " + (y + 1) + " has got a problem. Value: " + sex);
                }



                string age = grid[ageIndex, y];

                if (checkList(age, ageType) == false)
                {
                    throw new System.Exception("Age in row: " + (y + 1) + " has got a problem. Value: " + age);
                }

                string ethnicity = grid[ethnicityIndex, y];

                if (checkList(ethnicity, EthnicityType) == false)
                {
                    throw new System.Exception("Ethnicity in row: " + (y + 1) + " has got a problem. Value: " + ethnicity);
                }


                string scenario = grid[scenarioIndex, y];
                string nps = grid[npsIndex, y];
                string[] npsWords = nps.Split(' ');
                int npsValue = 0;
                try
                {
                    npsValue = int.Parse(npsWords[0]);
                    if (npsValue < 1 || npsValue > 10)
                    {
                        throw new System.Exception("NPS in row: " + (y + 1) + " has got a problem. Value: " + nps);
                    }
                }
                catch (UnityException e)
                {
                    throw new System.Exception("NPS in row: " + (y + 1) + " has got a problem. Value: " + nps);
                }

                string timeAvailable = grid[timeAvailableIndex, y];

                string[] timeWords = timeAvailable.Split(' ');

                float timeMins = 0;

                try
                {
                    timeMins = float.Parse(timeWords[0]);
                }
                catch (UnityException e)
                {
                    throw new System.Exception("Time Avaliable in row: " + (y + 1) + " has got a problem. Value: " + timeAvailable);
                }


                string bestZone = grid[bestZoneIndex, y].ToUpper();
                string secondBestZone = grid[secondBestZoneIndex, y].ToUpper();

                string upsell = grid[upsellIndex, y].ToUpper();


                bool upSellVal = false;


                if (upsell != "YES" && upsell != "NO")
                {
                    throw new System.Exception("Upsell in row: " + (y + 1) + " has got a problem. Value: " + upsell);
                }
                else
                {

                    if (upsell == "YES")
                    {
                        upSellVal = true;
                    }
                    else
                    {
                        upSellVal = false;
                    }
                }

                string spend = grid[spendIndex, y];
                float spendVal = 0;

                try
                {
                    spendVal = float.Parse(spend);
                }
                catch (UnityException e)
                {
                    throw new System.Exception("Spend in row: " + (y + 1) + " has got a problem. Value: " + spend);
                }


                //double spendVal = double.Parse(spend);

                Customer player = new Customer (sex, age, ethnicity, scenario, npsValue, timeMins, bestZone, secondBestZone, upSellVal, spendVal);
                generatedCustomers.Add(player);


                //Customer objects get created here and stored somewhere. (possibleCustomersPool)
            }
        }
        return generatedCustomers;

    }

    static bool checkList(string word, string[] checkWords)
    {
        bool match = false;
        foreach (string check in checkWords)
        {
            if (word.ToUpper() == check.ToUpper())
            {
                match = true;
                break;
            }
        }
        return match;
    }

}
