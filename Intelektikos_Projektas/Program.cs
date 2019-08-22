using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ConsoleTableExt;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;




namespace Intelektikos_Projektas
{

   
    
    class Program
    {

        static public List<Player> list0fPlayers = new List<Player>();
        static public List<Player> listOfPassed = list0fPlayers.Where(x => x.TARGET_Yrs).ToList();
        static public List<Player> listOfDenied = list0fPlayers.Where(x => !x.TARGET_Yrs).ToList();
        static public double[,] ArrOfPlayers = new double[20,1329];
        static public List<Player> list0fDimensions = new List<Player>();
        static public double AVGACC = 0;

        static public int parts = 10;
        static void Main(string[] args)
        {
            String File = @"..\..\nba_logreg.csv";

            NeuralNetwork nn = new NeuralNetwork(12, 10, 1);

            int folds = 10;

            Read_Data(File);

            for (int i = 0; i < 10; i++)
            {
                GiniCalculation(i, true);
                GiniCalculation(i, false);
            }
            //DimensionReduction();

            StandartScoreNormalization();                     
            
            int TestingCount = list0fPlayers.Count / folds;

            int[] iterations = new int[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };

            //for (int J = 9; J < 10; J++)
            //{
            //    Console.WriteLine();

            //    Console.WriteLine("Test with {0} iterations: ",iterations[J]);
            //    for (int i = 0; i < folds; i++)
            //    {
            //        TrainNeuralNet(nn, TestingCount, i, folds,iterations[J]);
            //        TestCase(nn, TestingCount, i, folds);
            //        nn.ResetWeights();
            //    }
            //    Console.WriteLine("Average accuracy   {0:P}",AVGACC / 10);
            //    AVGACC = 0;
            //}

            for (int i = 0; i < folds; i++)
            {
                TrainNeuralNet(nn, TestingCount, i, folds,200);
                TestCase(nn, TestingCount, i, folds);
                nn.ResetWeights();
            }

        }



        static public void Read_Data(string File)
        {
            string[] lines = System.IO.File.ReadAllLines(File);

            bool value;
            foreach (string line in lines)
            {
                string[] words = line.Split('/');
                list0fPlayers.Add(new Player(words[0], double.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3]), double.Parse(words[4]),
                double.Parse(words[5]), double.Parse(words[6]), double.Parse(words[7]), double.Parse(words[8]), double.Parse(words[9]),
                double.Parse(words[10]), double.Parse(words[11]), double.Parse(words[12]), double.Parse(words[13]), double.Parse(words[14]),
                double.Parse(words[15]), double.Parse(words[16]), double.Parse(words[17]), double.Parse(words[18]), double.Parse(words[19]), value = (words[20] == "1,0")));
                
            }

        }

        public static void TrainNeuralNet(NeuralNetwork nn, int testingCount, int step, int folds, int iterations)
        {
            //Matrix<double> inputs = DenseMatrix.Build.DenseIdentity(15, 1);
            Matrix<double> inputs = DenseMatrix.Build.DenseIdentity(12, 1);

            Matrix<double> targets = DenseMatrix.Build.DenseIdentity(1, 1);
            Random rnd = new Random(1524);
            int index = 0;
            Console.WriteLine("Experiment NR: {0}",step+1);
            for (int k = 0; k < iterations; k++)
            {
                for (int i = 0; i < step * testingCount; i++) // ??
                {
                    index = rnd.Next(0, step * testingCount+1);
                   

                    inputs[0, 0] = ArrOfPlayers[0, index];
                    inputs[1, 0] = ArrOfPlayers[1, index];
                    inputs[2, 0] = ArrOfPlayers[2, index];
                    inputs[3, 0] = ArrOfPlayers[5, index];
                    inputs[4, 0] = ArrOfPlayers[9, index];
                    inputs[5, 0] = ArrOfPlayers[11, index];
                    inputs[6, 0] = ArrOfPlayers[12, index];
                    inputs[7, 0] = ArrOfPlayers[13, index];
                    inputs[8, 0] = ArrOfPlayers[15, index];
                    inputs[9, 0] = ArrOfPlayers[16, index];
                    inputs[10, 0] = ArrOfPlayers[17, index];
                    inputs[11, 0] = ArrOfPlayers[18, index];



                    targets[0, 0] = ArrOfPlayers[19, index];

                    nn.train(inputs, targets);
                }

                //Console.WriteLine("----------------------------------------------------------------------------");
                //index = 0;

                for (int i = (step - 1 == folds ? list0fPlayers.Count : (step + 1) * testingCount) + 1; i < list0fPlayers.Count; i++)  // ??
                {
                    index = rnd.Next(0, (step - 1 == folds ? list0fPlayers.Count : (step + 1) * testingCount) + 2);

                    inputs[0, 0] = ArrOfPlayers[0, index];
                    inputs[1, 0] = ArrOfPlayers[1, index];
                    inputs[2, 0] = ArrOfPlayers[2, index];
                    inputs[3, 0] = ArrOfPlayers[5, index];
                    inputs[4, 0] = ArrOfPlayers[9, index];
                    inputs[5, 0] = ArrOfPlayers[11, index];
                    inputs[6, 0] = ArrOfPlayers[12, index];
                    inputs[7, 0] = ArrOfPlayers[13, index];
                    inputs[8, 0] = ArrOfPlayers[15, index];
                    inputs[9, 0] = ArrOfPlayers[16, index];
                    inputs[10, 0] = ArrOfPlayers[17, index];
                    inputs[11, 0] = ArrOfPlayers[18, index];

                    targets[0, 0] = ArrOfPlayers[19, index];

                    nn.train(inputs, targets);

                }
            }
            


        }
       


        public static void TestCase(NeuralNetwork nn, int testingCount, int step, int folds)
        {
           double PlayersFound = 0;
           double PlayersMissed = 0;
           double BPlayersFound = 0;
           double BPlayersMissed = 0;
           double GoodPlayers = 0;
           double BadPlayers = 0;
                       

            Matrix<double> inputs = DenseMatrix.Build.DenseIdentity(12, 1);
            Matrix<double> target = DenseMatrix.Build.DenseIdentity(12, 1);




            for (int i = step * testingCount; i < (step - 1 == folds ? list0fPlayers.Count : (step + 1) * testingCount); i++)
            {

                inputs[0, 0] = ArrOfPlayers[0, i];
                inputs[1, 0] = ArrOfPlayers[1, i];
                inputs[2, 0] = ArrOfPlayers[2, i];
                inputs[3, 0] = ArrOfPlayers[5, i];
                inputs[4, 0] = ArrOfPlayers[9, i];
                inputs[5, 0] = ArrOfPlayers[11, i];
                inputs[6, 0] = ArrOfPlayers[12, i];
                inputs[7, 0] = ArrOfPlayers[13, i];
                inputs[8, 0] = ArrOfPlayers[15, i];
                inputs[9, 0] = ArrOfPlayers[16, i];
                inputs[10, 0] = ArrOfPlayers[17, i];
                inputs[11, 0] = ArrOfPlayers[18, i];

                target[0, 0] = ArrOfPlayers[19, i];

                if (target[0, 0] == 1)
                {
                    GoodPlayers++;
                }
                else
                    BadPlayers++;


                if (target[0, 0] == 1)
                {
                    bool res = CheckResultP(nn, inputs, target);
                    if (res)
                    {
                        PlayersFound++;
                    }
                    else
                    {
                        PlayersMissed++;
                    }
                }
                else
                {
                    bool resN = CheckResultN(nn, inputs, target);
                    if (resN)
                    {
                        BPlayersFound++;
                    }
                    else
                    {
                        BPlayersMissed++;
                    }
                }

            }
            double TPR = PlayersFound / (GoodPlayers);
            double FPR = BPlayersFound / (BadPlayers);
            double ACC = (PlayersFound + BPlayersFound) / (GoodPlayers + BadPlayers);
            AVGACC += ACC;           
            Console.WriteLine("Good players found: {0}, Good players missed: {1}", PlayersFound, PlayersMissed);

            Console.WriteLine("Bad players found: {0}, Bad players missed: {1}", BPlayersFound, BPlayersMissed);


            Console.WriteLine("Accuracy: {0:P}", ACC);
            Console.WriteLine("TPR: {0:P} " , TPR);
            Console.WriteLine("FPR: {0:P} " , FPR);
            //Console.WriteLine("Good players: {0:P}", GoodPlayers/(GoodPlayers+BadPlayers));

            //Console.WriteLine("Players missed: " + PlayersMissed);

            Console.WriteLine("--------------------");
        }

        public static bool CheckResultP(NeuralNetwork nn, Matrix<double> inputs, Matrix<double> target)
        {
           Matrix<double> output = nn.FeedForward(inputs);
            //Console.WriteLine(output[0,0] + "          " + target[0,0]);
            if (output[0, 0] > 0.5)
            {
                output[0, 0] = 1;
            }          
            
            return output[0, 0] == target[0,0];
        }

        public static bool CheckResultN(NeuralNetwork nn, Matrix<double> inputs, Matrix<double> target)
        {
            Matrix<double> output = nn.FeedForward(inputs);
            //Console.WriteLine(output[0,0] + "          " + target[0,0]);
            if (output[0, 0] < 0.5)
            {
                output[0, 0] = 0;
            }

            return output[0, 0] == target[0, 0];
        }


        public static Matrix<double> ToMatrix (double[,] input)
        {
            Matrix<double> output = DenseMatrix.Build.DenseIdentity(15,1);


            return output;
        }


        public static double[] Averages()
        {            
            double[] output = new double[19];

            output[0] = list0fPlayers.Select(x => x.GP).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[1] = list0fPlayers.Select(x => x.MIN).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[2] = list0fPlayers.Select(x => x.PTS).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[3] = list0fPlayers.Select(x => x.FGM).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[4] = list0fPlayers.Select(x => x.FGA).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[5] = list0fPlayers.Select(x => x.FGP).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[6] = list0fPlayers.Select(x => x.THRPM).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[7] = list0fPlayers.Select(x => x.THRPA).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[8] = list0fPlayers.Select(x => x.THRPP).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[9] = list0fPlayers.Select(x => x.FTM).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[10] = list0fPlayers.Select(x => x.FTA).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[11] = list0fPlayers.Select(x => x.FTP).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[12] = list0fPlayers.Select(x => x.OREB).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[13]= list0fPlayers.Select(x => x.DREB).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[14] = list0fPlayers.Select(x => x.REB).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[15] = list0fPlayers.Select(x => x.AST).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[16] = list0fPlayers.Select(x => x.STL).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[17] = list0fPlayers.Select(x => x.BLCK).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;
            output[18] = list0fPlayers.Select(x => x.TOV).Aggregate(0d, (x, y) => x + y) / list0fPlayers.Count;

            return output;
        }

        public static double[] StandartDeviations()
        {
            double[] averages = Averages();
            double[] output = new double[19];

            output[0] = list0fPlayers.Select(x => x.GP).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[0],2))) / list0fPlayers.Count;
            output[1] = list0fPlayers.Select(x => x.MIN).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[1],2))) / list0fPlayers.Count;
            output[2] = list0fPlayers.Select(x => x.PTS).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[2],2))) / list0fPlayers.Count;
            output[3] = list0fPlayers.Select(x => x.FGM).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[3],2))) / list0fPlayers.Count;
            output[4] = list0fPlayers.Select(x => x.FGA).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[4],2))) / list0fPlayers.Count;
            output[5] = list0fPlayers.Select(x => x.FGP).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[5],2))) / list0fPlayers.Count;
            output[6] = list0fPlayers.Select(x => x.THRPM).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[6],2))) / list0fPlayers.Count;
            output[7] = list0fPlayers.Select(x => x.THRPA).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[7],2))) / list0fPlayers.Count;
            output[8] = list0fPlayers.Select(x => x.THRPP).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[8],2))) / list0fPlayers.Count;
            output[9] = list0fPlayers.Select(x => x.FTM).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[9],2))) / list0fPlayers.Count;
            output[10] = list0fPlayers.Select(x => x.FTA).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[10],2))) / list0fPlayers.Count;
            output[11] = list0fPlayers.Select(x => x.FTP).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[11],2))) / list0fPlayers.Count;
            output[12] = list0fPlayers.Select(x => x.OREB).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[12],2))) / list0fPlayers.Count;
            output[13] = list0fPlayers.Select(x => x.DREB).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[13],2))) / list0fPlayers.Count;
            output[14] = list0fPlayers.Select(x => x.REB).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[14],2))) / list0fPlayers.Count;
            output[15] = list0fPlayers.Select(x => x.AST).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[15],2))) / list0fPlayers.Count;
            output[16] = list0fPlayers.Select(x => x.STL).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[16],2))) / list0fPlayers.Count;
            output[17] = list0fPlayers.Select(x => x.BLCK).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[17],2))) / list0fPlayers.Count;
            output[18] = list0fPlayers.Select(x => x.TOV).Aggregate(0d, (x, y) => x + (Math.Pow(y - averages[18],2))) / list0fPlayers.Count;

            for (int i = 0; i < output.Count(); i++)
            {
                output[i] = Math.Sqrt(output[i]);
            }

            return output;
        }

        public static void StandartScoreNormalization()
        {
            double[] averages = Averages();
            double[] standartDeviations = StandartDeviations();

            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                ArrOfPlayers[0, i] = (list0fPlayers[i].GP - averages[0]) / standartDeviations[0];
                ArrOfPlayers[1, i] = (list0fPlayers[i].MIN - averages[1]) / standartDeviations[1];
                ArrOfPlayers[2, i] = (list0fPlayers[i].PTS - averages[2]) / standartDeviations[2];
                ArrOfPlayers[3, i] = (list0fPlayers[i].FGM - averages[3]) / standartDeviations[3];
                ArrOfPlayers[4, i] = (list0fPlayers[i].FGA - averages[4]) / standartDeviations[4];
                ArrOfPlayers[5, i] = (list0fPlayers[i].FGP - averages[5]) / standartDeviations[5];
                ArrOfPlayers[6, i] = (list0fPlayers[i].THRPM - averages[6]) / standartDeviations[6];
                ArrOfPlayers[7, i] = (list0fPlayers[i].THRPA - averages[7]) / standartDeviations[7];
                ArrOfPlayers[8, i] = (list0fPlayers[i].THRPP - averages[8]) / standartDeviations[8];
                ArrOfPlayers[9, i] = (list0fPlayers[i].FTM - averages[9]) / standartDeviations[9];
                ArrOfPlayers[10, i] = (list0fPlayers[i].FTA - averages[10]) / standartDeviations[10];
                ArrOfPlayers[11, i] = (list0fPlayers[i].FTP - averages[11]) / standartDeviations[11];
                ArrOfPlayers[12, i] = (list0fPlayers[i].OREB - averages[12]) / standartDeviations[12];
                ArrOfPlayers[13, i] = (list0fPlayers[i].DREB - averages[13]) / standartDeviations[13];
                ArrOfPlayers[14, i] = (list0fPlayers[i].REB - averages[14]) / standartDeviations[14];
                ArrOfPlayers[15, i] = (list0fPlayers[i].AST - averages[15]) / standartDeviations[15];
                ArrOfPlayers[16, i] = (list0fPlayers[i].STL - averages[16]) / standartDeviations[16];
                ArrOfPlayers[17, i] = (list0fPlayers[i].BLCK - averages[17]) / standartDeviations[17];
                ArrOfPlayers[18, i] = (list0fPlayers[i].TOV - averages[18]) / standartDeviations[18];
                ArrOfPlayers[19, i] = list0fPlayers[i].TARGET_Yrs ? 1d : 0d;
            }

        }
        static public void GiniCalculation(int block, bool isLearning)
        {
            int start = list0fPlayers.Count / 10 * block;
            int end = list0fPlayers.Count / 10 * (block + 1);
            List<Player> learnTest = new List<Player>();

            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        learnTest.Add(list0fPlayers[i]);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        learnTest.Add(list0fPlayers[i]);
                }
            }
            if (!isLearning)
            {
                Console.WriteLine("Data set\n");
            }
            listOfPassed = learnTest.Where(x => x.TARGET_Yrs).ToList();
            listOfDenied = learnTest.Where(x => !x.TARGET_Yrs).ToList();
            List<Gini> giniList = new List<Gini>();
            double Average = 0;

            int less = 0;
            int more = 0;
            int lessPos = 0;
            int lessNeg = 0;
            int morePos = 0;
            int moreNeg = 0;


            double averageGP = listOfPassed.Select(x => x.GP).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageGP1 = listOfDenied.Select(x => x.GP).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageGP + averageGP1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].GP, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].GP, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("GP", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageMIN = listOfPassed.Select(x => x.MIN).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageMIN1 = listOfDenied.Select(x => x.MIN).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageMIN + averageMIN1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].MIN, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].MIN, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("MIN", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averagePTS = listOfPassed.Select(x => x.PTS).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averagePTS1 = listOfDenied.Select(x => x.PTS).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averagePTS + averagePTS1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].PTS, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].PTS, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("PTS", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageFGM = listOfPassed.Select(x => x.FGM).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageFGM1 = listOfDenied.Select(x => x.FGM).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageFGM + averageFGM1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].FGM, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].FGM, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("FGM", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageFGA = listOfPassed.Select(x => x.FGA).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageFGA1 = listOfDenied.Select(x => x.FGA).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageFGA + averageFGA1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].FGA, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].FGA, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("FGA", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageTHRPM = listOfPassed.Select(x => x.THRPM).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageTHRPM1 = listOfDenied.Select(x => x.THRPM).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageTHRPM + averageTHRPM1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].THRPM, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].THRPM, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("THRPM", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageTHRPA = listOfPassed.Select(x => x.THRPA).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageTHRPA1 = listOfDenied.Select(x => x.THRPA).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageTHRPA + averageTHRPA1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].THRPA, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].THRPA, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("THRPA", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageFTM = listOfPassed.Select(x => x.FTM).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageFTM1 = listOfDenied.Select(x => x.FTM).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageFTM + averageFTM1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].FTM, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].FTM, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("FTM", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageFTA = listOfPassed.Select(x => x.FTA).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageFTA1 = listOfDenied.Select(x => x.FTA).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageFTA + averageFTA1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].FTA, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].FTA, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("FTA", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageOREB = listOfPassed.Select(x => x.OREB).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageOREB1 = listOfDenied.Select(x => x.OREB).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageOREB + averageOREB1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].OREB, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].OREB, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("OREB", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageDREB = listOfPassed.Select(x => x.DREB).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageDREB1 = listOfDenied.Select(x => x.DREB).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageDREB + averageDREB1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].DREB, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].DREB, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("DREB", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageREB = listOfPassed.Select(x => x.REB).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageREB1 = listOfDenied.Select(x => x.REB).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageREB + averageREB1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].REB, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].REB, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("REB", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageAST = listOfPassed.Select(x => x.AST).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageAST1 = listOfDenied.Select(x => x.AST).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageAST + averageAST1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].AST, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].AST, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("AST", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageSTL = listOfPassed.Select(x => x.STL).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageSTL1 = listOfDenied.Select(x => x.STL).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageSTL + averageSTL1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].STL, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].STL, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("STL", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageBLCK = listOfPassed.Select(x => x.BLCK).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageBLCK1 = listOfDenied.Select(x => x.BLCK).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageBLCK + averageBLCK1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].BLCK, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].BLCK, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("BLCK", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));
            less = 0;
            more = 0;
            lessPos = 0;
            lessNeg = 0;
            morePos = 0;
            moreNeg = 0;
            double averageTOV = listOfPassed.Select(x => x.TOV).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
            double averageTOV1 = listOfDenied.Select(x => x.TOV).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
            Average = (averageTOV + averageTOV1) / 2;
            for (int i = 0; i < learnTest.Count; i++)
            {
                if (isLearning)
                {
                    if (i < start || i >= end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].TOV, Average, learnTest[i].TARGET_Yrs);
                }
                else if (!isLearning)
                {
                    if (i >= start && i < end)
                        GiniParameters(ref more, ref less, ref lessPos, ref lessNeg, ref morePos, ref moreNeg, learnTest[i].TOV, Average, learnTest[i].TARGET_Yrs);
                }
            }
            giniList.Add(new Gini("TOV", GiniIndex(less, more, lessPos, lessNeg, morePos, moreNeg)));

            SortGini(giniList);

            //foreach (object gini in giniList)
            //{
            //    Console.WriteLine(gini.ToString());
            //}
            DecisionTree(giniList, learnTest, block, isLearning);
        }
        public static void SortGini(List<Gini> giniList)
        {
            for (int i = 0; i < giniList.Count; i++)
            {
                for (int j = 0; j < giniList.Count; j++)
                {
                    if (giniList[i].GiniIndex <= giniList[j].GiniIndex)
                    {
                        double temp = giniList[i].GiniIndex;
                        giniList[i].GiniIndex = giniList[j].GiniIndex;
                        giniList[j].GiniIndex = temp;

                        string col = giniList[i].Collumn;
                        giniList[i].Collumn = giniList[j].Collumn;
                        giniList[j].Collumn = col;
                    }
                }
            }
        }
        public static void GiniParameters(ref int more, ref int less, ref int lessPos, ref int lessNeg, ref int morePos, ref int moreNeg, double collumn, double average, bool target)
        {

            if (collumn >= average && target)
            {
                more++;
                morePos++;
            }
            else if (collumn >= average && !target)
            {
                more++;
                moreNeg++;
            }
            if (collumn < average && target)
            {
                less++;
                lessPos++;
            }
            else if (collumn < average && !target)
            {
                less++;
                lessNeg++;
            }
        }
        static public double GiniIndex(double lessCount, double moreCount, double lessPos, double lessNeg, double morePos, double moreNeg)
        {
            double result = 0;
            double less = 1 - (Math.Pow(lessPos / lessCount, 2) + Math.Pow(lessNeg / lessCount, 2));
            double more = 1 - (Math.Pow(morePos / moreCount, 2) + Math.Pow(moreNeg / moreCount, 2));

            result = lessCount / list0fPlayers.Count * less + moreCount / list0fPlayers.Count * more;
            return result;
        }
        public static void DecisionTree(List<Gini> giniList, List<Player> list0fPlayers, int block, bool isLearning)
        {
            listOfPassed = list0fPlayers.Where(x => x.TARGET_Yrs).ToList();
            listOfDenied = list0fPlayers.Where(x => !x.TARGET_Yrs).ToList();
            int truePos = 0;
            int trueNeg = 0;
            int falsePos = 0;
            int falseNeg = 0;
            double Average = 0;
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                int trueCount = 0;
                for (int j = 0; j < giniList.Count; j++)
                {
                    double collumn = 0;
                    if (giniList[j].Collumn == "GP")
                    {
                        collumn = list0fPlayers[i].GP;
                        double averageGP = listOfPassed.Select(x => x.GP).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageGP1 = listOfDenied.Select(x => x.GP).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageGP + averageGP1) / 2 - 47;
                    }
                    else if (giniList[j].Collumn == "PTS")
                    {
                        collumn = list0fPlayers[i].PTS;
                        double averagePTS = listOfPassed.Select(x => x.PTS).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averagePTS1 = listOfDenied.Select(x => x.PTS).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averagePTS + averagePTS1) / 2 - 10;
                    }
                    else if (giniList[j].Collumn == "MIN")
                    {
                        collumn = list0fPlayers[i].MIN;
                        double averageMIN = listOfPassed.Select(x => x.MIN).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageMIN1 = listOfDenied.Select(x => x.MIN).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageMIN + averageMIN1) / 2 - 5;
                    }
                    else if (giniList[j].Collumn == "FTM")
                    {
                        collumn = list0fPlayers[i].FTM;
                        double averageFTM = listOfPassed.Select(x => x.FTM).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageFTM1 = listOfDenied.Select(x => x.FTM).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageFTM + averageFTM1) / 2;
                    }
                    else if (giniList[j].Collumn == "OREB")
                    {
                        collumn = list0fPlayers[i].OREB;
                        double averageOREB = listOfPassed.Select(x => x.OREB).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageOREB1 = listOfDenied.Select(x => x.OREB).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageOREB + averageOREB1) / 2 - 2;
                    }
                    else if (giniList[j].Collumn == "FTA")
                    {
                        collumn = list0fPlayers[i].FTA;
                        double averageFTA = listOfPassed.Select(x => x.FTA).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageFTA1 = listOfDenied.Select(x => x.FTA).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageFTA + averageFTA1) / 2 - 8;
                    }
                    else if (giniList[j].Collumn == "FGM")
                    {
                        collumn = list0fPlayers[i].FGM;
                        double averageFGM = listOfPassed.Select(x => x.FGM).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageFGM1 = listOfDenied.Select(x => x.FGM).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageFGM + averageFGM1) / 2 - 6;
                    }
                    else if (giniList[j].Collumn == "FGA")
                    {
                        collumn = list0fPlayers[i].FGA;
                        double averageFGA = listOfPassed.Select(x => x.FGA).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageFGA1 = listOfDenied.Select(x => x.FGA).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageFGA + averageFGA1) / 2;
                    }
                    else if (giniList[j].Collumn == "REB")
                    {
                        collumn = list0fPlayers[i].REB;
                        double averageREB = listOfPassed.Select(x => x.REB).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageREB1 = listOfDenied.Select(x => x.REB).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageREB + averageREB1) / 2 - 1;
                    }
                    else if (giniList[j].Collumn == "DREB")
                    {
                        collumn = list0fPlayers[i].DREB;
                        double averageDREB = listOfPassed.Select(x => x.DREB).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageDREB1 = listOfDenied.Select(x => x.DREB).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageDREB + averageDREB1) / 2;
                    }
                    else if (giniList[j].Collumn == "TOV")
                    {
                        collumn = list0fPlayers[i].TOV;
                        double averageTOV = listOfPassed.Select(x => x.TOV).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageTOV1 = listOfDenied.Select(x => x.TOV).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageTOV + averageTOV1) / 2;
                    }
                    else if (giniList[j].Collumn == "BLCK")
                    {
                        collumn = list0fPlayers[i].BLCK;
                        double averageBLCK = listOfPassed.Select(x => x.BLCK).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageBLCK1 = listOfDenied.Select(x => x.BLCK).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageBLCK + averageBLCK1) / 2;

                    }
                    else if (giniList[j].Collumn == "STL")
                    {
                        collumn = list0fPlayers[i].STL;
                        double averageSTL = listOfPassed.Select(x => x.STL).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageSTL1 = listOfDenied.Select(x => x.STL).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageSTL + averageSTL1) / 2;
                    }
                    else if (giniList[j].Collumn == "AST")
                    {
                        collumn = list0fPlayers[i].AST;
                        double averageAST = listOfPassed.Select(x => x.AST).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                        double averageAST1 = listOfDenied.Select(x => x.AST).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                        Average = (averageAST + averageAST1) / 2;
                    }
                    //else if (giniList[j].Collumn == "THRPM")
                    //{
                    //    collumn = list0fPlayers[i].THRPM;
                    //    double averageTHRPM = listOfPassed.Select(x => x.THRPM).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                    //    double averageTHRPM1 = listOfDenied.Select(x => x.THRPM).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                    //    Average = (averageTHRPM + averageTHRPM1) / 2 - 2;
                    //}
                    //else if (giniList[j].Collumn == "THRPA")
                    //{
                    //    collumn = list0fPlayers[i].THRPA;
                    //    double averageTHRPA = listOfPassed.Select(x => x.THRPA).Aggregate(1d, (x, y) => x + y) / listOfPassed.Count;
                    //    double averageTHRPA1 = listOfDenied.Select(x => x.THRPA).Aggregate(1d, (x, y) => x + y) / listOfDenied.Count;
                    //    Average = (averageTHRPA + averageTHRPA1) / 2 - 1;
                    //}
                    if (giniList[j].GiniIndex == 0 && list0fPlayers[i].TARGET_Yrs)
                    {
                        truePos++;
                        break;
                    }
                    else if (giniList[j].GiniIndex == 0 && !list0fPlayers[i].TARGET_Yrs)
                    {
                        falsePos++;
                        break;
                    }
                    if (collumn >= Average)
                    {
                        trueCount++;
                    }
                }
                int threshold = 4; //max 16
                if (trueCount != 0 && !isLearning)
                {
                    if (trueCount >= threshold && list0fPlayers[i].TARGET_Yrs)
                        truePos++;
                    else if (trueCount >= threshold && !list0fPlayers[i].TARGET_Yrs)
                        trueNeg++;
                    else if (trueCount < threshold && list0fPlayers[i].TARGET_Yrs)
                        falseNeg++;
                    else if (trueCount < threshold && !list0fPlayers[i].TARGET_Yrs)
                        falsePos++;
                }
            }
            if (!isLearning)
            {
                Console.WriteLine();
                Console.WriteLine("Player will last more than 5 years (correct){0}", truePos);
                Console.WriteLine("Player will last more than 5 years (false){0}", trueNeg);
                Console.WriteLine("Player will not last more than 5 years (correct){0}", falsePos);
                Console.WriteLine("Player will not last more than 5 years (false){0}", falseNeg);
                int correct = falsePos + truePos;
                double allTrue = truePos + trueNeg;
                double allFalse = falsePos + falseNeg;
                double all = list0fPlayers.Count;
                double reliability = correct / all;
                Console.WriteLine("True positive rate {0:P}", truePos / allTrue);
                Console.WriteLine("False positive rate {0:P}", falsePos / allFalse);
                Console.WriteLine("Decision tree reliability: {0:P}", reliability);
                Console.WriteLine();
            }
        }
        public static void DimensionReduction()
        {
            int a = 0, b = 0, c = 0, d = 0, e = 0;
            list0fPlayers.Sort((x, y) => x.GP.CompareTo(y.GP));
            Console.WriteLine("GP");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.MIN.CompareTo(y.MIN));
            Console.WriteLine("MIN");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.PTS.CompareTo(y.PTS));
            Console.WriteLine("PTS");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.FGM.CompareTo(y.FGM));
            Console.WriteLine("FGM");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.FGA.CompareTo(y.FGA));
            Console.WriteLine("FGA");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.FGP.CompareTo(y.FGP));
            Console.WriteLine("FGP");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.THRPM.CompareTo(y.THRPM));
            Console.WriteLine("THRPM");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.THRPA.CompareTo(y.THRPA));
            Console.WriteLine("THRPA");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.THRPP.CompareTo(y.THRPP));
            Console.WriteLine("THRPP");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.FTM.CompareTo(y.FTM));
            Console.WriteLine("FTM");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.FTA.CompareTo(y.FTA));
            Console.WriteLine("FTA");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.FTP.CompareTo(y.FTP));
            Console.WriteLine("FTP");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.OREB.CompareTo(y.OREB));
            Console.WriteLine("OREB");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.DREB.CompareTo(y.DREB));
            Console.WriteLine("DREB");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.REB.CompareTo(y.REB));
            Console.WriteLine("REB");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.AST.CompareTo(y.AST));
            Console.WriteLine("AST");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.STL.CompareTo(y.STL));
            Console.WriteLine("STL");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.BLCK.CompareTo(y.BLCK));
            Console.WriteLine("BLCK");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;

            list0fPlayers.Sort((x, y) => x.TOV.CompareTo(y.TOV));
            Console.WriteLine("TOV");
            for (int i = 0; i < list0fPlayers.Count; i++)
            {
                if (i < list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    a++;
                }
                else if (i < (list0fPlayers.Count / 5) * 2 && i > list0fPlayers.Count / 5 && list0fPlayers[i].TARGET_Yrs)
                {
                    b++;
                }
                else if (i < (list0fPlayers.Count / 5) * 3 && i > list0fPlayers.Count / 5 * 2 && list0fPlayers[i].TARGET_Yrs)
                {
                    c++;
                }
                else if (i < (list0fPlayers.Count / 5) * 4 && i > (list0fPlayers.Count / 5) * 3 && list0fPlayers[i].TARGET_Yrs)
                {
                    d++;
                }
                else if (i > (list0fPlayers.Count / 5) * 4 && list0fPlayers[i].TARGET_Yrs)
                    e++;
            }
            Console.WriteLine("a {0} b {1} c {2} d {3} e {4}", a, b, c, d, e);
            a = 0; b = 0; c = 0; d = 0; e = 0;





        }

    }
}
