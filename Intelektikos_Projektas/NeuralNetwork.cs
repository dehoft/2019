using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Intelektikos_Projektas
{
    class NeuralNetwork
    {
        public int inputNodes, hiddenNodes, outputNodes;
        public double[,] weightsIH, weightsHO, biasH, biasO;
        public Matrix<double> WeightsIH, WeightsHO, BiasH, BiasO;
        public double learningRate;



        public NeuralNetwork(int inputNodes, int hiddenNodes, int outputNodes)
        {
            this.inputNodes = inputNodes;
            this.hiddenNodes = hiddenNodes;
            this.outputNodes = outputNodes;

            this.weightsIH = new double[this.hiddenNodes, this.inputNodes];
            this.weightsHO = new double[this.outputNodes, this.hiddenNodes];

            this.WeightsIH = DenseMatrix.OfArray(weightsIH);
            this.WeightsHO = DenseMatrix.OfArray(weightsHO);


            this.WeightsIH = RandomiseMatrix(WeightsIH);
            this.WeightsHO = RandomiseMatrix(WeightsHO);

            this.biasH = new double[this.hiddenNodes, 1];
            this.biasO = new double[this.outputNodes, 1];

            this.BiasH = DenseMatrix.OfArray(biasH);
            this.BiasO = DenseMatrix.OfArray(biasO);

            this.BiasH = RandomiseMatrix(BiasH);
            this.BiasO = RandomiseMatrix(BiasO);

            this.learningRate = 0.05;

        }

        public void ResetWeights()
        {
            this.WeightsIH = RandomiseMatrix(WeightsIH);
            this.WeightsHO = RandomiseMatrix(WeightsHO);

            this.BiasH = RandomiseMatrix(BiasH);
            this.BiasO = RandomiseMatrix(BiasO);

        }

        public Matrix<double> RandomiseMatrix(Matrix<double> input)
        {
            Random rnd = new Random(154);
            double random;


            for (int i = 0; i < input.RowCount; i++)
            {
                for (int j = 0; j < input.ColumnCount; j++)
                {
                    random = rnd.NextDouble() * 2 - 1;

                    input[i, j] = random;

                }
            }

            return input;
        }


        public Matrix<double> sigmoid(Matrix<double> input)
        {
            Matrix<double> output = DenseMatrix.Build.DenseIdentity(input.RowCount, input.ColumnCount);
            for (int i = 0; i < input.RowCount; i++)
            {
                for (int j = 0; j < input.ColumnCount; j++)
                {
                    output[i, j] = 1 / (1 + Math.Exp(-input[i, j]));
                }
            }

            return output;
        }

        public Matrix<double> DerrivativeOfSigmoid(Matrix<double> input)
        {
            Matrix<double> output = DenseMatrix.Build.DenseIdentity(input.RowCount, input.ColumnCount);
            for (int i = 0; i < input.RowCount; i++)
            {
                for (int j = 0; j < input.ColumnCount; j++)
                {
                    //output[i, j] = (1 / (1 + Math.Exp(-input[i, j]))) * (1 - (1 / (1 + Math.Exp(-input[i, j])))); //actual derrivative of sigmoid
                    output[i, j] = input[i, j] * (1 - input[i, j]);
                }
            }

            return output;
        }

        public Matrix<double> SoftMax(Matrix<double> input)
        {
            Matrix<double> output = DenseMatrix.Build.DenseIdentity(input.RowCount, input.ColumnCount);
            double sumExp = 0;
            for (int i = 0; i < input.RowCount; i++)
            {
                for (int j = 0; j < input.ColumnCount; j++)
                {
                    output[i, j] = Math.Exp(input[i, j]);
                }
            }

            for (int i = 0; i < input.RowCount; i++)
            {
                for (int j = 0; j < input.ColumnCount; j++)
                {
                    sumExp += output[i, j];
                }
            }

            for (int i = 0; i < input.RowCount; i++)
            {
                for (int j = 0; j < input.ColumnCount; j++)
                {
                    output[i, j] = output[i, j] / sumExp;
                }
            }

            return output;
        }

        public Matrix<double> HadamardMultiplication(Matrix<double> x, Matrix<double> y)
        {
            Matrix<double> output = x;

            for (int i = 0; i < output.RowCount; i++)
            {
                for (int j = 0; j < output.ColumnCount; j++)
                {
                    output[i, j] *= y[i, j];
                }
            }

            return output;
        }


        public Matrix<double> FeedForward(Matrix<double> input)
        {
            //Generate the hidden outputs
            Matrix<double> hidden = DenseMatrix.Build.DenseIdentity(WeightsIH.RowCount, input.ColumnCount);

            WeightsIH.Multiply(input, hidden);
            hidden = hidden.Add(BiasH);

            //activation function
            hidden = sigmoid(hidden);

            // Generating output
            Matrix<double> output = DenseMatrix.Build.DenseIdentity(WeightsHO.RowCount, hidden.ColumnCount);

            WeightsHO.Multiply(hidden, output);
            output = output.Add(BiasO);

            // activation function          
            output = sigmoid(output);

            return output;
        }


        public void train(Matrix<double> inputs, Matrix<double> targets)
        {
            Matrix<double> hidden = DenseMatrix.Build.DenseIdentity(this.WeightsIH.RowCount, inputs.ColumnCount);
            this.WeightsIH.Multiply(inputs, hidden);
            hidden = hidden.Add(this.BiasH);

            //Applying activation function
            hidden = sigmoid(hidden);

            // Generating output
            Matrix<double> outputs = DenseMatrix.Build.DenseIdentity(this.WeightsHO.RowCount, hidden.ColumnCount);

            this.WeightsHO.Multiply(hidden, outputs);
            outputs = outputs.Add(BiasO);

            //Applying activation function
            outputs = sigmoid(outputs);

            //Calculate the error
            //ERROR = TARGETS - OUTPUTS
            Matrix<double> OutputErrors = targets.Subtract(outputs);

            Matrix<double> gradients = DerrivativeOfSigmoid(outputs);

            //calculate gradients
            gradients = HadamardMultiplication(gradients, OutputErrors);
            gradients = gradients.Multiply(this.learningRate);

            // Calculate the HO (hidden output) weights deltas
            Matrix<double> hiddenT = hidden.Transpose();
            Matrix<double> weightsHODeltas = DenseMatrix.Build.DenseIdentity(gradients.RowCount, hiddenT.ColumnCount);
            gradients.Multiply(hiddenT, weightsHODeltas);

            // changing the weights of HO
            this.WeightsHO.Add(weightsHODeltas, this.WeightsHO);

            // Adjust the bias by its deltas (which is gradients)
            this.BiasO.Add(gradients, this.BiasO);


            // Calculate the hidden layer errors
            Matrix<double> WHO_T = this.WeightsHO.Transpose();
            Matrix<double> HiddenErrors = DenseMatrix.Build.DenseIdentity(WHO_T.RowCount, OutputErrors.ColumnCount);
            WHO_T.Multiply(OutputErrors, HiddenErrors);

            Matrix<double> hiddenGradients = DerrivativeOfSigmoid(hidden);

            // Calculate hidden gradients
            hiddenGradients = HadamardMultiplication(hiddenGradients, HiddenErrors);
            hiddenGradients = hiddenGradients.Multiply(this.learningRate);


            // Calculate the IH (input hidden) weights deltas
            Matrix<double> inputsT = inputs.Transpose();
            Matrix<double> weightsIHDeltas = DenseMatrix.Build.DenseIdentity(hiddenGradients.RowCount, inputsT.ColumnCount);
            hiddenGradients.Multiply(inputsT, weightsIHDeltas);

            // changing the weights of IH
            this.WeightsIH.Add(weightsIHDeltas, this.WeightsIH);

            // Adjust the bias by its deltas (which is gradients)
            this.BiasH.Add(hiddenGradients, this.BiasH);

        }



    }
}
