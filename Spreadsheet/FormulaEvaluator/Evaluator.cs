using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// This class contains functionality for evaluating arithmetic expressions, with or without variables.
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(string v);

        /// <summary>
        /// This class parses and evaluates a given arithmetic expression, following conventional PMDAS order of operations.
        /// Variables are allowed, and are parsed using the given Lookup delegate.
        /// </summary>
        /// <param name="exp">The arithmetic expression to be evaulated</param>
        /// <param name="variableEvaluator">A delegate method for looking up the values of variables</param>
        /// <returns>The integer equal to the given expression</returns>
        public static int Evaluate(string exp, Lookup variableEvaluator)
        {
            Stack<int> valStack = new();
            Stack<string> opStack = new();

            // Split given string into individual tokens
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            try // this try block catches any InvalidOperationExceptions from attempting to Pop an empty Stack
            {
                foreach (string substring in substrings)
                {
                    string token = substring.Trim();
                    if (token == "") continue;  // sometimes the regex split above will create whitespace characters. Skip them.

                    // If the token is an integer
                    if (int.TryParse(token, out _))
                    {
                        if (opStack.EitherOnTop("*", "/"))
                        {
                            valStack.CalcAndPush(int.Parse(token), valStack.Pop(), opStack.Pop());
                        }
                        else
                        {
                            valStack.Push(int.Parse(token));
                        }
                    }

                    // Or, if the token is a variable
                    else if (Regex.IsMatch(token, "^[a-zA-Z]+[0-9]+$"))
                    {
                        if (opStack.EitherOnTop("*", "/"))
                        {
                            valStack.CalcAndPush(variableEvaluator(token), valStack.Pop(), opStack.Pop());
                        }
                        else
                        {
                            valStack.Push(variableEvaluator(token));
                        }
                    }

                    // Or, if the token is an operator
                    else if (IsOp(token))
                    {
                        if (token == "+" || token == "-")
                        {
                            if (opStack.EitherOnTop("+", "-"))
                            {
                                valStack.CalcAndPush(valStack.Pop(), valStack.Pop(), opStack.Pop());
                            }
                            opStack.Push(token);
                        }
                        else if (token == "*" || token == "/" || token == "(")
                        {
                            opStack.Push(token);
                        }
                        else if (token == ")")
                        {
                            if (opStack.EitherOnTop("+", "-"))
                            {
                                valStack.CalcAndPush(valStack.Pop(), valStack.Pop(), opStack.Pop());
                            }

                            // The operator stack should have a "(" on top at this point. Throw an error if not. If no error, pop it
                            if (!opStack.EitherOnTop("(", "("))
                            {
                                throw new ArgumentException("Error: Invalid use of parentheses.");
                            }
                            opStack.Pop();

                            // If after resolving a parentheses block a * or / is on top of the operator stack, it needs to be evaluated
                            if (opStack.EitherOnTop("*", "/"))
                            {
                                valStack.CalcAndPush(valStack.Pop(), valStack.Pop(), opStack.Pop());
                            }
                        }
                    }

                    // Otherwise, the token is invalid
                    else
                    {
                        throw new ArgumentException("Error: Invalid token included in expression.");
                    }
                }
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException("Error: Invalid expression.");
            }

            // Finished processing the sequence- take last action(s)
            if (opStack.Count != 0)
            {
                // If the operator stack isn't empty, there should only be a + or - left, with 2 values in the value stack
                if (!opStack.EitherOnTop("+", "-") || valStack.Count != 2)
                {
                    throw new ArgumentException("Error: Invalid expression.");
                }
                valStack.CalcAndPush(valStack.Pop(), valStack.Pop(), opStack.Pop());
            }

            // Once the operator stack is empty, there should only be 1 value left the value stack
            if (valStack.Count != 1)
            {
                throw new ArgumentException("Error: Invalid expression.");
            }

            return valStack.Pop();
        }

        /// <summary>
        /// This helper method checks whether a given string contains only a valid operator.<br></br>
        /// Valid operators: + - * / ( )
        /// </summary>
        /// <param name="token">The string to be checked</param>
        /// <returns>True if the string contains only a valid operator, false otherwise</returns>
        private static bool IsOp(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/"
                || token == "(" || token == ")";
        }

        /// <summary>
        /// This helper method takes two operands and operator, applies the appropiate calculation,
        /// and pushes the result to the given stack.
        /// </summary>
        /// <param name="stack">The stack to which to push</param>
        /// <param name="left">The left operand</param>
        /// <param name="right">The right operand</param>
        /// <param name="op">The operator to apply</param>
        private static void CalcAndPush(this Stack<int> stack, int right, int left, string op)
        {
            switch (op)
            {
                case "+":
                    stack.Push(left + right);
                    break;
                case "-":
                    stack.Push(left - right);
                    break;
                case "*":
                    stack.Push(left * right);
                    break;
                case "/":
                    if (right == 0) throw new ArgumentException("Error: Divide by 0.");
                    stack.Push(left / right);
                    break;
                default:
                    throw new ArgumentException("Error: Unexpected operator.");
            }
        }

        /// <summary>
        /// This helper method checks whether either of two given operands are on top of the given stack.
        /// </summary>
        /// <param name="stack">The stack to check within</param>
        /// <param name="op1">The first operand to check</param>
        /// <param name="op2">The second operand to check</param>
        /// <returns></returns>
        private static bool EitherOnTop(this Stack<string> stack, string op1, string op2)
        {
            return stack.Count > 0 && (stack.Peek() == op1 || stack.Peek() == op2);
        }
    }
}