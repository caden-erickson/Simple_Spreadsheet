using FormulaEvaluator;
using System;

namespace FormulaEvaluatorTestor
{
    /// <summary>
    /// This class contains print statements to test the functionality of the FormulaEvaluator.Evaluator class
    /// </summary>
    internal class EvaluatorTestor
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        /// <param name="args">unused</param>
        static void Main(string[] args)
        {
            Console.WriteLine(Evaluator.Evaluate("2 + 6 / 3 - 4", Make8) + " should be 0.");
            Console.WriteLine(Evaluator.Evaluate("8 * (7 - 9) / 4", Make8) + " should be -4.");
            Console.WriteLine(Evaluator.Evaluate("7 + G10 - (2 * 0)", Make8) + " should be 15.");
            Console.WriteLine(Evaluator.Evaluate("6", Make2) + " should be 6.");
            Console.WriteLine(Evaluator.Evaluate("0 - 9", Make5) + " should be -9.");
            Console.WriteLine(Evaluator.Evaluate("7 * U4 - R7", Make5) + " should be 30.");
            Console.WriteLine(Evaluator.Evaluate("N19", Make8) + " should be 8.");
            Console.WriteLine(Evaluator.Evaluate("(6 - 10) / X18 + 10", Make2) + " should be 8.");

            Console.WriteLine(Evaluator.Evaluate("4 + () 4", Make2) + " should be 8.");

            try { Evaluator.Evaluate("6 */ 3", Make2); }
            catch(ArgumentException) {Console.WriteLine("\"6 */ 3\" successfully threw an error."); }

            try { Evaluator.Evaluate("6 ++ 3", Make2); }
            catch (ArgumentException) { Console.WriteLine("\"6 ++ 3\" successfully threw an error."); }

            try { Evaluator.Evaluate("", Make2); }
            catch (ArgumentException) { Console.WriteLine("\"\" successfully threw an error."); }
            
        }

        /// <summary>
        /// Returns a value of 8 for any given variable.
        /// </summary>
        /// <param name="s">unused</param>
        /// <returns></returns>
        public static int Make8(string s)
        {
            return 8;
        }

        /// <summary>
        /// Returns a value of 2 for any given variable.
        /// </summary>
        /// <param name="s">unused</param>
        /// <returns></returns>
        public static int Make2(string s)
        {
            return 2;
        }

        /// <summary>
        /// Returns a value of 5 for any given variable.
        /// </summary>
        /// <param name="s">unused</param>
        /// <returns></returns>
        public static int Make5(string s)
        {
            return 5;
        }

    }
}