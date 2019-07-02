/*
 * CS-675 Assignment 6 (Problem 3)
 * Written by: Dylan Hargett
 * 
 * This code implements the bilinear interpolation method to "zoom" an image by a facotr of 4.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bilinearInterpolation
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFileName = "brain_128_128.raw";

            char[] splitChars = { '_', '.' };
            int originalImagelength = Int32.Parse(inputFileName.Split(splitChars)[1]);
            int originalImageWidth = Int32.Parse(inputFileName.Split(splitChars)[2]);
            int scalingFactor = 4;
            byte[] allLines = File.ReadAllBytes(@"..\..\img\" + inputFileName);
            byte[,] multiDimArray;

            int scaledImageLength = scalingFactor * originalImagelength;
            int scaledImageWidth = scalingFactor * originalImageWidth;
            byte[,] scaledMultiDimArray = new byte[scaledImageLength, scaledImageWidth];


            // transform our 1D array into a 2D array
            multiDimArray = TransformToMultiArray(allLines, originalImagelength, originalImageWidth);


            int origI_Counter = 0;
            int origJ_Counter = 0;
            int intVal1J = 0;
            int intVal2J = 0;
            int intVal1I = 0;
            int intVal2I = 0;
            int intScaleCounter = scalingFactor - 1;
            

            // populate our scaled 2D array with the vals from our original 2D array
            for(int i = 0; i < originalImagelength; i++)
            {
                for(int j = 0; j < originalImageWidth; j++)
                {
                    scaledMultiDimArray[i * 4, j * 4] = multiDimArray[i, j];
                }
            }

            
            // interpolate rows where % scalingFactor == 0
            for(int i = 0; i < scaledImageLength; i+=4)
            {
                for(int j = 0; j < scaledImageWidth; j++)
                {
                    if (j % scalingFactor == 0)
                    {
                        intVal1J = (int)multiDimArray[origI_Counter, origJ_Counter];
                        if (origJ_Counter != originalImageWidth - 1)
                            intVal2J = multiDimArray[origI_Counter, origJ_Counter + 1];
                        else
                            intVal2J = intVal1J; // not sure if this is needed
                        origJ_Counter++;
                    }
                    else
                        scaledMultiDimArray[i,j] = InterpolateValue(intVal1J, intVal2J, scalingFactor, j);
                }
                origJ_Counter = 0; // reset j counter for the next time it will be used
                origI_Counter++; // iterate i counter for the next time it will be used
            }

            // fill in the rest of the scaled 2D array
            for (int i = 0; i < scaledImageLength; i++)
            {
                for (int j = 0; j < scaledImageWidth; j++)
                {
                    if(scaledMultiDimArray[i,j].ToString() == "0")
                    {
                        int populatedI_offset = i % scalingFactor;
                        intVal1I = scaledMultiDimArray[i - (populatedI_offset), j]; //(scalingFactor - populatedI_offset)

                        if (i >= (scaledImageLength - scalingFactor))
                            intVal2I = intVal1I;
                        else
                            intVal2I = scaledMultiDimArray[i + (scalingFactor - populatedI_offset), j]; // populatedI_offset

                        // interpolate values that weren't populated in the first go around
                        scaledMultiDimArray[i, j] = InterpolateValue(intVal1I, intVal2I, scalingFactor, j);
                    }
                }
            }

            string newFileName = @"..\..\img\" + inputFileName.Split(splitChars)[0] + "_" + scaledImageLength.ToString() +"_" + scaledImageWidth.ToString() +"." + inputFileName.Split(splitChars)[3];
            WriteFile(newFileName, scaledMultiDimArray, originalImagelength, originalImageWidth, scalingFactor);

        }

        /// <summary>
        /// write the data to the file
        /// </summary>
        /// <param name="newFileName"></param>
        /// <param name="scaledMultiDimArray"></param>
        private static void WriteFile(string newFileName, byte[,] scaledMultiDimArray, int origImageLength, int origImageWidth, int scalingFactor)
        {

            byte[] scaled1DArray = new byte[(origImageLength*scalingFactor)*(origImageWidth*scalingFactor)];

            int count = 0;
            for(int i = 0; i < origImageLength; i++)
            {
                for(int j = 0; j < origImageWidth; j++)
                {
                    scaled1DArray[count] = scaledMultiDimArray[i, j];
                    count++;
                }
            }

            // write our byte data to our output file
            File.WriteAllBytes(newFileName, scaled1DArray);
            
        }

        /// <summary>
        /// Interpolates a value from 2 given numbers
        /// </summary>
        /// <param name="intVal1"></param>
        /// <param name="intVal2"></param>
        /// <param name="scalingFactor"></param>
        /// <param name="j"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static byte InterpolateValue(int intVal1, int intVal2, int scalingFactor, int j)
        {
            int offset = j % scalingFactor;
            //if (j < scalingFactor)
                //offset = scalingFactor - j;
            if (offset == 0)
                offset = scalingFactor;
            return (byte)Convert.ChangeType(((intVal1 * (scalingFactor - offset)) + (intVal2 * (offset))) / 4, typeof(byte));
        }

        /// <summary>
        /// Transforms our 1D array int a 2D array
        /// </summary>
        /// <param name="allLines"></param>
        /// <param name="imageLength"></param>
        /// <param name="ImageWidth"></param>
        /// <returns></returns>
        private static byte[,] TransformToMultiArray(byte[] allLines, int imageLength, int ImageWidth)
        {
            byte[,] _multiDimArray;
            _multiDimArray = new byte[imageLength, ImageWidth];

            for(int i=0; i < imageLength; i++)
            {
                for(int j = 0; j < ImageWidth; j++)
                {
                    _multiDimArray[i, j] = allLines[i * ImageWidth + j];
                }
            }

            return _multiDimArray;
        }
    }
}
