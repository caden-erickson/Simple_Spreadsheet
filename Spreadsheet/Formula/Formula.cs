// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!

// Change log:
// Last updated: 9/8, updated for non-nullable types


// Full functionality (beyond skeleton code) written by Caden Erickson
// Last modified: 09/16/2022

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private readonly List<string> finalTokens;
        private readonly HashSet<string> finalVariables;
        private readonly string finalFormula;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(string formula) : this(formula, s => s, s => true)
        {
            // Everything necessary to construct a Formula from here is done
            // through the call to the three-parameter constructor above
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(string formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            StringBuilder finalFormulaBuilder = new();
            finalTokens = new();
            finalVariables = new();

            List<string> rawTokens = GetTokens(formula).ToList();

            // Go through every token and add to the tokens list, processing the variables
            for (int i = 0; i < rawTokens.Count; i++)
            {
                string current = rawTokens[i];

                // If it's a variable, it needs to be processed, and also added to the variables list
                if (Regex.IsMatch(current, @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$"))
                {
                    current = ProcessVar(current, normalize, isValid);
                    finalVariables.Add(current);
                }

                if (double.TryParse(current, out double parsed))
                    current = parsed.ToString();

                finalTokens.Add(current);
                finalFormulaBuilder.Append(current);
            }

            // Final syntax check of entire expression
            CheckSyntax(rawTokens);

            // Final repackaging as string since all checks have been passed
            finalFormula = finalFormulaBuilder.ToString();
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<double> valStack = new();
            Stack<string> opStack = new();

            try
            {
                foreach (string token in finalTokens)
                {
                    // If the token is a digit
                    if (double.TryParse(token, out _))
                    {
                        if (opStack.EitherOnTop("*", "/"))
                            valStack.CalcAndPush(double.Parse(token), valStack.Pop(), opStack.Pop());
                        else
                            valStack.Push(double.Parse(token));
                    }

                    // Or, if the token is a variable
                    else if (Regex.IsMatch(token, @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$"))
                    {
                        if (opStack.EitherOnTop("*", "/"))
                            valStack.CalcAndPush(lookup(token), valStack.Pop(), opStack.Pop());
                        else
                            valStack.Push(lookup(token));
                    }

                    // Or, if the token is an operator
                    else if (Regex.IsMatch(token, @"^[+\-*/()]$"))
                    {
                        if (token == "+" || token == "-")
                        {
                            if (opStack.EitherOnTop("+", "-"))
                                valStack.CalcAndPush(valStack.Pop(), valStack.Pop(), opStack.Pop());

                            opStack.Push(token);
                        }
                        else if (token == "*" || token == "/" || token == "(")
                        {
                            opStack.Push(token);
                        }
                        else if (token == ")")
                        {
                            if (opStack.EitherOnTop("+", "-"))
                                valStack.CalcAndPush(valStack.Pop(), valStack.Pop(), opStack.Pop());

                            // The operator stack should have a "(" on top at this point, pop it
                            opStack.Pop();

                            // If after resolving a parentheses block a * or / is on top of the operator stack, it needs to be evaluated
                            if (opStack.EitherOnTop("*", "/"))
                                valStack.CalcAndPush(valStack.Pop(), valStack.Pop(), opStack.Pop());
                        }
                    }
                }

                // Finished processing the sequence- take last action(s)
                if (opStack.Count != 0)
                    valStack.CalcAndPush(valStack.Pop(), valStack.Pop(), opStack.Pop());

                return valStack.Pop();
            }
            catch (Exception e)
            {
                // Dividing by 0 throws an ArgumentException; use its message for the FormulaError
                // Same goes for Lookup not finding a value for a given variable
                return new FormulaError(e.Message);
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<string> GetVariables()
        {
            return finalVariables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return finalFormula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj == null || !obj.GetType().Equals(GetType()))
                return false;

            return finalFormula.Equals(obj.ToString());
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return f1.ToString().Equals(f2.ToString());
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !f1.ToString().Equals(f2.ToString());
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return finalFormula.GetHashCode();
        }


        //============================== Private helper methods for validating expression ==============================//

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(string formula)
        {
            // Patterns for individual tokens
            string lpPattern = @"\(";
            string rpPattern = @"\)";
            string opPattern = @"[\+\-*/]";
            string varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            string spacePattern = @"\s+";

            // Overall pattern
            string pattern = string.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        /// <summary>
        /// Given a variable (already checked for syntax), this function normalizes it with the normalize
        /// passed to the constructor, checks it for syntax again, and then passes it through the validator given
        /// to the constructor before returning it.
        /// </summary>
        /// <param name="var">The variable to be processed</param>
        /// <param name="normalize">The normalization delegate to be used</param>
        /// <param name="isValid">The validation delegate to be used</param>
        /// <returns>The normalized and validated variable</returns>
        /// <exception cref="FormulaFormatException"></exception>
        private static string ProcessVar(string var, Func<string, string> normalize, Func<string, bool> isValid)
        {
            string processed = normalize(var);

            // Now that it's been normalized, check it for correct syntax again
            if (!(Regex.IsMatch(processed, @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$")))
                throw new FormulaFormatException("\"" + var + "\" was normalized to \"" + processed +
                    "\", which is an invalid variable name.\nConsider changing your expression.");
                    
            // Past that, make sure the validator also approves it
            if (!isValid(processed))
                throw new FormulaFormatException("\"" + var + "\" was normalized to \"" + processed +
                    "\", which is an invalid variable name.\nConsider changing your expression.");

            return processed;
        }

        /// <summary>
        /// Given a decomposed (tokenized) expression, checks that it is syntactically correct:<br/>
        /// - Expressions must contain only decimal real numbers, variables, and operators: '(', ')', '+', '-', '*', '/'.<br/>
        /// - Expressions must have at least one token.<br/>
        /// - Pairings of parentheses must be syntactically sound.<br/>
        /// - The number of '('s must match the number of ')'s.<br/>
        /// - Expressions must start with a number, variable, or '('.<br/>
        /// - Expressions must end with a number, variable, or ')'.<br/>
        /// - Any token that immediately follows a '(' or an operator must be either a number, a variable, or a '('.<br/>
        /// - Any token that immediately follows a number, a variable, or a ')' must be either an operator or a ')'.
        /// 
        /// </summary>
        /// <param name="formula">The formula to be checked</param>
        /// <exception cref="FormulaFormatException"></exception>
        private static void CheckSyntax(IEnumerable<string> formula)
        {
            // For tracking the number of opening parentheses (OpPars) and closing parentheses (ClPars)
            int OpPars = 0;
            int ClPars = 0;

            if (!formula.Any())
                throw new FormulaFormatException("Blank entries are invalid. Please enter an expression.");

            for (int i = 0; i < formula.Count(); i++)
            {
                // Check the token for validity based on its position in the expression
                CheckToken(formula, i);

                string s = formula.ElementAt(i);
                if (s == "(")
                    OpPars++;
                if (s == ")")
                    ClPars++;

                if (ClPars > OpPars)
                    throw new FormulaFormatException(
                        "When reading tokens from left to right, at no point should the number of closing parentheses " +
                        "seen so far be greater than the number of opening parentheses seen so far. " +
                        "Check your parentheses and try again.");
            }

            if (OpPars != ClPars)
                throw new FormulaFormatException(
                    "The total number of opening parentheses must equal the total number of closing parentheses.");
        }

        /// <summary>
        /// Given a token, checks that it is an accepted one.<br/>
        /// Accepted tokens: decimal real numbers, variables, and operators: '(', ')', '+', '-', '*', '/'.
        /// Variables are valid if they consist of a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores.<br/>
        /// Also checks that the given token is a valid token for its position in the expression,
        /// and that the token that follows it is a accepted one to follow this particular token.
        /// </summary>
        /// <param name="token">The token to be checked</param>
        /// <exception cref="FormulaFormatException"></exception>
        private static void CheckToken(IEnumerable<string> formula, int position)
        {
            string token = formula.ElementAt(position);

            // operators and '('s
            if (Regex.IsMatch(token, @"^[+\-*/(]$"))
            {
                // If this is the first token, then past the above condition only '('s are valid
                if (position == 0 && token != "(")
                    throw new FormulaFormatException(
                        "Expressions must begin with a number, a variable, or a '('. \"" + token + "\" is invalid here. " +
                        "Try rewriting your expression, or trya different expression.");

                // If this is the last token, it's invalid in this position
                if (position == formula.Count() - 1)
                    throw new FormulaFormatException(
                        "Expressions must end with a number, a variable, or a ')'. \"" + token + "\" is invalid here. " +
                        "Try rewriting your expression, or try a different expression.");

                // Must be followed by a number, a variable or '('
                string next = formula.ElementAt(position + 1);
                if (!IsNumOrVar(next) && next != "(")
                    throw new FormulaFormatException(
                        "'('s and operators must be followed by a number, a variable or a '('. \"" + next + "\" is invalid here. " +
                        "Try rewriting your expression, or try a different expression.");
            }

            // Numbers, variables, and ')'s
            else if (IsNumOrVar(token) || token == ")")
            {
                // If this is the first token, then past the above condition only numbers and variables are valid
                if (position == 0 && !IsNumOrVar(token))
                    throw new FormulaFormatException(
                        "Expressions must begin with a number, a variable, or a '('. \"" + token + "\" is invalid here. " +
                        "Try rewriting your expression, or try a different expression.");

                // If this token isn't last, it must be followed by a ')' or an operator 
                if (position != formula.Count() - 1 && !Regex.IsMatch(formula.ElementAt(position + 1), @"^[+\-*/)]$"))
                    throw new FormulaFormatException(
                        "Numbers, variables, and '('s must be followed by a '(' or an operator. \"" + token + "\" is invalid here. " +
                        "Try rewriting your expression, or try a different expression.");
            }

            // Any other token is invalid
            else
            {
                throw new FormulaFormatException("\"" + token + "\" is an invalid token. Try removing it from your expression.");
            }
        }

        /// <summary>
        /// Checks whether a given token is a number (Num) or a variable (Var)
        /// </summary>
        /// <returns>true if one of the above conditions is met, false otherwise</returns>
        private static bool IsNumOrVar(string token)
        {
            return double.TryParse(token, out _) || Regex.IsMatch(token, @"^[a-zA-Z_](?:[a-zA-Z_]|\d)*$");
        }        
    }

    /// <summary>
    /// This static class contains Extensions to the Stack class, to be used in the 
    /// Evaluate method in the Formula class above.
    /// Declared internal, as no methods other than Evaluate need access to it
    /// </summary>
    internal static class StackExtensions
    {
        /// <summary>
        /// This helper method takes two operands and operator, applies the appropiate calculation,
        /// and pushes the result to the given stack.
        /// </summary>
        /// <param name="stack">The stack to which to push</param>
        /// <param name="left">The left operand</param>
        /// <param name="right">The right operand</param>
        /// <param name="op">The operator to apply</param>
        internal static void CalcAndPush(this Stack<double> stack, double right, double left, string op)
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
            }
        }

        /// <summary>
        /// This helper method checks whether either of two given operands are on top of the given stack.
        /// </summary>
        /// <param name="stack">The stack to check within</param>
        /// <param name="op1">The first operand to check</param>
        /// <param name="op2">The second operand to check</param>
        /// <returns></returns>
        internal static bool EitherOnTop(this Stack<string> stack, string op1, string op2)
        {
            return stack.Any() && (stack.Peek() == op1 || stack.Peek() == op2);
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(string reason) : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}