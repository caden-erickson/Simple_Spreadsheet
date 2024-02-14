// Tests written by Caden Erickson
// Last updated: 09/16/22

using SpreadsheetUtilities;

namespace FormulaTests
{
    /// <summary>
    /// This class contains unit tests for the Formula class
    /// in the SpreadsheetUtilities namespace.
    /// </summary>
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void BasicTest()
        {
            Formula formula = new("C10 + 3.4e20 / A1 * _4 + _y3 + x - (5.200000 - 00010.2)", s => s.ToUpper(), s => true);
            Assert.AreEqual("C10+3.4E+20/A1*_4+_Y3+X-(5.2-10.2)", formula.ToString());
        }

        //=======================================================================//

        /// <summary>
        /// Blank formula should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BlankExpressionTest1()
        {
            Formula formula = new("");
        }

        /// <summary>
        /// Formula of only spaces should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BlankExpressionTest2()
        {
            Formula formula = new("   ");
        }

        //=======================================================================//

        /// <summary>
        /// Formula with unmatched numbers of opening and
        /// closing parentheses should throw an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void UnmatchedParenthesesTest1()
        {
            Formula formula = new("(((5 + 7) - 4)");
        }

        /// <summary>
        /// Formula with unmatched numbers of opening and
        /// closing parentheses should throw an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void UnmatchedParenthesesTest2()
        {
            Formula formula = new("((5 + 7) - 4))");
        }

        /// <summary>
        /// Formula with unmatched numbers of opening and
        /// closing parentheses should throw an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void UnmatchedParenthesesTest3()
        {
            Formula formula = new("((5 + 7)) - 4)");
        }

        /// <summary>
        /// Improperly ordered parentheses should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void WrongOrderParenthesesTest()
        {
            Formula formula = new("(5 + 7)) - (4");
        }

        //=======================================================================//


        /// <summary>
        /// Formula beginning with an operator should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadStartTokenTest1()
        {
            Formula formula = new("+ 5");
        }

        /// <summary>
        /// Formula beginning with a ')' should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadStartTokenTest2()
        {
            Formula formula = new(") + 5");
        }

        /// <summary>
        /// Formula ending with an operator should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadEndTokenTest1()
        {
            Formula formula = new("5 +");
        }

        /// <summary>
        /// Formula ending with a '(' should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadEndTokenTest2()
        {
            Formula formula = new("5 + (");
        }

        //=======================================================================//

        /// <summary>
        /// Operator followed by operator should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest01()
        {
            Formula formula = new("5 + + 5");
        }

        /// <summary>
        /// Operator followed by ')' should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest02()
        {
            Formula formula = new("5 + )");
        }

        /// <summary>
        /// '(' followed by ')' should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest03()
        {
            Formula formula = new("5 + ()");
        }

        /// <summary>
        /// '(' followed by operator should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest04()
        {
            Formula formula = new("5 + ( * 7)");
        }

        /// <summary>
        /// Digit followed by separate digit should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest05()
        {
            Formula formula = new("5 5 + 10");
        }

        /// <summary>
        /// Digit followed by variable should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest06()
        {
            Formula formula = new("5 A1 + 10");
        }

        /// <summary>
        /// Digit followed by '(' should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest07()
        {
            Formula formula = new("5 (3 + 10)");
        }

        /// <summary>
        /// Variable followed by digit should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest08()
        {
            Formula formula = new("A1 5 + 1");
        }

        /// <summary>
        /// Variable followed by variable should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest09()
        {
            Formula formula = new("A1 B3 / 4");
        }

        /// <summary>
        /// Variable followed by '(' should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest10()
        {
            Formula formula = new("A1 (3 + 4)");
        }

        /// <summary>
        /// ')' followed by digit should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest11()
        {
            Formula formula = new("(1 + 2) 2");
        }

        /// <summary>
        /// ')' followed by variable should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest12()
        {
            Formula formula = new("(1 + 2) A1");
        }

        /// <summary>
        /// ')' followed by '(' should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadTokenSequenceTest13()
        {
            Formula formula = new("(1 + 2)(2 - 1)");
        }

        //=======================================================================//

        /// <summary>
        /// Invalid token ($) should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidTokenTest1()
        {
            Formula formula = new("$(1 + 2) * A1 / 4");
        }

        /// <summary>
        /// Invalid token ($) should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidTokenTest2()
        {
            Formula formula = new("(1 + 2) - $ * A1 / 4");
        }

        /// <summary>
        /// Invalid token ($) should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidTokenTest3()
        {
            Formula formula = new("(1 + 2) * A1 / $");
        }

        /// <summary>
        /// Invalid token ($) should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableTest1()
        {
            Formula formula = new("1_a + (1 + 2) * A1 / 4");
        }

        /// <summary>
        /// Invalid token ($) should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableTest2()
        {
            Formula formula = new("(1 + 2) - 1_a * A1 / 4");
        }

        /// <summary>
        /// Invalid token ($) should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableTest3()
        {
            Formula formula = new("(1 + 2) * A1 / 1_a");
        }

        //=======================================================================//

        /// <summary>
        /// Normalizing to an invalid variable syntax should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadNormalizerTest()
        {
            Formula formula = new("(1 + 2) * A1 / 4", s => "1A", s => true);
        }

        /// <summary>
        /// Normalizing to an invalid variable syntax should throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadValidatorTest1()
        {
            Formula formula = new("(1 + 2) * a1 / 4", s => s.ToUpper(), s => s.ElementAt(0).Equals('a'));
        }

        /// <summary>
        /// An impossibly picky validator should break everything
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void BadValidatorTest2()
        {
            Formula formula = new("(1 + 2) * A1 / 4", s => s, s => false);
        }

        //=======================================================================//

        /// <summary>
        /// Explicit divide by 0 should return a FormulaError upon evaluation
        /// </summary>
        [TestMethod]
        public void FormulaErrorTest1()
        {
            Formula formula = new("5 + 3 * 2 / 0 - 4");
            object result = formula.Evaluate(s => 0);
            Assert.IsInstanceOfType(result, typeof(FormulaError));
            Assert.AreEqual("Error: Divide by 0.", ((FormulaError)result).Reason);
        }

        /// <summary>
        /// Implicit divide by 0 should return a FormulaError upon evaluation
        /// </summary>
        [TestMethod]
        public void FormulaErrorTest2()
        {
            Formula formula = new("5 + 3 * 2 / A1 - 4");
            object result = formula.Evaluate(s => 0);
            Assert.IsInstanceOfType(result, typeof(FormulaError));
            Assert.AreEqual("Error: Divide by 0.", ((FormulaError)result).Reason);
        }

        /// <summary>
        /// Lookup throwing an exception should return a FormulaError upon evaluation
        /// </summary>
        [TestMethod]
        public void FormulaErrorTest3()
        {
            Formula formula = new("5 + 3 * 2 / A1 - 4");
            object result = formula.Evaluate(s => throw new ArgumentException("Variable not found."));
            Assert.IsInstanceOfType(result, typeof(FormulaError));
            Assert.AreEqual("Variable not found.", ((FormulaError)result).Reason);
        }

        //=======================================================================//

        /// <summary>
        /// Single-token formula should work
        /// </summary>
        [TestMethod]
        public void SingleTokenTest1()
        {
            Formula formula = new("10");
            Assert.IsTrue(formula.ToString().Equals("10"));
        }

        /// <summary>
        /// Single-token formula should work
        /// </summary>
        [TestMethod]
        public void SingleTokenTest2()
        {
            Formula formula = new("A1");
            Assert.IsTrue(formula.ToString().Equals("A1"));
        }

        //=======================================================================//

        /// <summary>
        /// Scientific notation should be parsed correctly
        /// </summary>
        [TestMethod]
        public void ScientificNotationTest1()
        {
            Formula formula = new("1 + 3.4e20 / 4");
            Assert.AreEqual("1+3.4E+20/4", formula.ToString());
        }

        /// <summary>
        /// Scientific notation should be parsed correctly
        /// </summary>
        [TestMethod]
        public void ScientificNotationTest2()
        {
            Formula formula = new("1 + 3e-10 / 4");
            Assert.AreEqual("1+3E-10/4", formula.ToString());
        }

        /// <summary>
        /// Scientific notation should be parsed correctly
        /// </summary>
        [TestMethod]
        public void ScientificNotationTest3()
        {
            Formula formula = new("1 + 3E20 / 4");
            Assert.AreEqual("1+3E+20/4", formula.ToString());
        }

        //=======================================================================//

        /// <summary>
        /// Expressions written with different spacing should be parsed the same
        /// </summary>
        [TestMethod]
        public void StandardizeTest1()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3)");
            Formula formula2 = new("1+A1+2*(B2/C3)");
            Formula formula3 = new("   1   +   A1   +   2   *   (   B2   /   C3   )   ");
            Assert.IsTrue(formula1.ToString().Equals(formula2.ToString()));
            Assert.IsTrue(formula1.ToString().Equals(formula3.ToString()));
            Assert.IsTrue(formula2.ToString().Equals(formula3.ToString()));
        }

        /// <summary>
        /// Expressions written with different casings should be parsed the same
        /// (with an appropriate normalizer)
        /// </summary>
        [TestMethod]
        public void StandardizeTest2()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3)", s => s.ToUpper(), s => true);
            Formula formula2 = new("1 + a1 + 2 * (b2 / c3)", s => s.ToUpper(), s => true);
            Assert.IsTrue(formula1.ToString().Equals(formula2.ToString()));
        }

        /// <summary>
        /// Expressions written with different number formats should be parsed the same
        /// </summary>
        [TestMethod]
        public void StandardizeTest3()
        {
            Formula formula1 = new("1 + A1 + 2.0000 * (B2 / 05)");
            Formula formula2 = new("1e0 + A1 + 02 * (B2 / 000000005)");
            Formula formula3 = new("1.0 + A1 + 200e-2 * (B2 / 5.000)");
            Assert.IsTrue(formula1.ToString().Equals(formula2.ToString()));
            Assert.IsTrue(formula1.ToString().Equals(formula3.ToString()));
            Assert.IsTrue(formula2.ToString().Equals(formula3.ToString()));
        }

        //=======================================================================//

        /// <summary>
        /// Equivalent expressions should work with the overriden Equals method
        /// </summary>
        [TestMethod]
        public void EqualsTest1()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3)");
            Formula formula2 = new("1+A1+2*(B2/C3)");
            Formula formula3 = new("   1   +   A1   +   2   *   (   B2   /   C3   )   ");
            Assert.IsTrue(formula1.Equals(formula2));
            Assert.IsTrue(formula1.Equals(formula3));
            Assert.IsTrue(formula2.Equals(formula3));
        }

        /// <summary>
        /// Unmatched object types should return false
        /// </summary>
        [TestMethod]
        public void EqualsTest2()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3)");
            Assert.IsFalse(formula1.Equals(null));
        }

        /// <summary>
        /// Unmatched object types should return false
        /// </summary>
        [TestMethod]
        public void EqualsTest3()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3)");
            object formula2 = new();
            Assert.IsFalse(formula1.Equals(formula2));
        }

        /// <summary>
        /// Equivalent expressions should work with the overriden == and != operators
        /// </summary>
        [TestMethod]
        public void EqualsEqualsTest()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3)");
            Formula formula2 = new("1+A1+2*(B2/C3)");
            Formula formula3 = new("   1   +   A1   +   2   *   (   B2   /   C3   )   ");
            Assert.IsTrue(formula1 == formula2);
            Assert.IsTrue(formula1 == formula3);
            Assert.IsTrue(formula2 == formula3);

            Assert.IsFalse(formula1 != formula2);
            Assert.IsFalse(formula1 != formula3);
            Assert.IsFalse(formula2 != formula3);
        }

        /// <summary>
        /// Inequivalent expressions should work with the overriden == operator
        /// </summary>
        [TestMethod]
        public void NotEqualsTest()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3) - 10");
            Formula formula2 = new("1 + A1 + 2 * (B2 / C3) - 20");
            Formula formula3 = new("1 + A1 + 2 * (B2 / C3) - 30");
            
            Assert.IsTrue(formula1 != formula2);
            Assert.IsTrue(formula1 != formula3);
            Assert.IsTrue(formula2 != formula3);

            Assert.IsFalse(formula1 == formula2);
            Assert.IsFalse(formula1 == formula3);
            Assert.IsFalse(formula2 == formula3);
        }

        //=======================================================================//

        /// <summary>
        /// Equivalent expressions should have equivalent HashCodes
        /// </summary>
        [TestMethod]
        public void HashCodeTest1()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3)");
            Formula formula2 = new("1+A1+2*(B2/C3)");
            Formula formula3 = new("   1   +   A1   +   2   *   (   B2   /   C3   )   ");
            Assert.IsTrue(formula1.GetHashCode().Equals(formula2.GetHashCode()));
            Assert.IsTrue(formula2.GetHashCode().Equals(formula3.GetHashCode()));
            Assert.IsTrue(formula1.GetHashCode().Equals(formula2.GetHashCode()));
        }

        /// <summary>
        /// Equivalent expressions should have equivalent HashCodes
        /// </summary>
        [TestMethod]
        public void HashCodeTest2()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3)");
            Formula formula2 = new("1+A1+2*(B2/C3)");
            Formula formula3 = new("   1   +   A1   +   2   *   (   B2   /   C3   )   ");
            Assert.IsTrue(formula1.GetHashCode().Equals(formula2.GetHashCode()));
            Assert.IsTrue(formula1.GetHashCode().Equals(formula3.GetHashCode()));
            Assert.IsTrue(formula2.GetHashCode().Equals(formula3.GetHashCode()));
        }

        /// <summary>
        /// Equivalent expressions should have equivalent HashCodes
        /// </summary>
        [TestMethod]
        public void HashCodeTest3()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3)");
            Formula formula2 = new("1+A1+2*(B2/C3)");
            Formula formula3 = new("   1   +   A1   +   2   *   (   B2   /   C3   )   ");
            Assert.IsTrue(formula1.GetHashCode().Equals(formula2.GetHashCode()));
            Assert.IsTrue(formula1.GetHashCode().Equals(formula3.GetHashCode()));
            Assert.IsTrue(formula2.GetHashCode().Equals(formula3.GetHashCode()));
        }

        /// <summary>
        /// Inequivalent expressions should have inequivalent HashCodes
        /// </summary>
        [TestMethod]
        public void HashCodeTest4()
        {
            Formula formula1 = new("1 + 2");
            Formula formula2 = new("1 + 1 + 1");
            Assert.IsFalse(formula1.GetHashCode().Equals(formula2.GetHashCode()));
        }

        /// <summary>
        /// Inequivalent expressions should have inequivalent HashCodes
        /// </summary>
        [TestMethod]
        public void HashCodeTest5()
        {
            Formula formula1 = new("1 + 2");
            Formula formula2 = new("2 + 1");
            Assert.IsFalse(formula1.GetHashCode().Equals(formula2.GetHashCode()));
        }

        /// <summary>
        /// Inequivalent expressions should have inequivalent HashCodes
        /// </summary>
        [TestMethod]
        public void HashCodeTest6()
        {
            Formula formula1 = new("10 + 10");
            Formula formula2 = new("10 + 10 + 0");
            Assert.IsFalse(formula1.GetHashCode().Equals(formula2.GetHashCode()));
        }

        //=======================================================================//

        /// <summary>
        /// Variables should be enumerated correctly
        /// </summary>
        [TestMethod]
        public void GetVariablesTest1()
        {
            Formula formula1 = new("1 + A1 + 2 * (B2 / C3)");
            IEnumerable<string> vars = formula1.GetVariables();
            Assert.AreEqual("A1", vars.ElementAt(0));
            Assert.AreEqual("B2", vars.ElementAt(1));
            Assert.AreEqual("C3", vars.ElementAt(2));
            Assert.AreEqual(3, vars.Count());
        }

        /// <summary>
        /// Variables should be enumerated correctly
        /// </summary>
        [TestMethod]
        public void GetVariablesTest2()
        {
            Formula formula1 = new("1 + 2 * (8 / 4)");
            IEnumerable<string> vars = formula1.GetVariables();
            Assert.AreEqual(0, vars.Count());
        }

        /// <summary>
        /// Variables should be enumerated correctly
        /// </summary>
        [TestMethod]
        public void GetVariablesTest3()
        {
            Formula formula1 = new("1 + a1 + 2 * (b2 / c3)", s => s.ToUpper(), s => true);
            IEnumerable<string> vars = formula1.GetVariables();
            Assert.AreEqual("A1", vars.ElementAt(0));
            Assert.AreEqual("B2", vars.ElementAt(1));
            Assert.AreEqual("C3", vars.ElementAt(2));
            Assert.AreEqual(3, vars.Count());
        }

        //=======================================================================//

        /// <summary>
        /// Expression evaluation resulting in a negative should work correctly
        /// </summary>
        [TestMethod]
        public void ExpectNegativeTest()
        {
            Formula formula = new("(10 - B5) / 4");
            Assert.AreEqual(-4.0, (double)formula.Evaluate(s => 26), 1e-9);
        }

        /// <summary>
        /// Expression evaluation resulting in zero should work correctly
        /// </summary>
        [TestMethod]
        public void ExpectZeroTest()
        {
            Formula formula = new("(10 - B5) / 4");
            Assert.AreEqual(0.0, (double)formula.Evaluate(s => 10), 1e-9);
        }

        /// <summary>
        /// Expression evaluation resulting in a positive should work correctly
        /// </summary>
        [TestMethod]
        public void ExpectPositiveTest()
        {
            Formula formula = new("(10 + B5) / 4");
            Assert.AreEqual(4.0, (double)formula.Evaluate(s => 6), 1e-9);
        }

        //=======================================================================//

        /// <summary>
        /// Lots of different valid token orderings should evaluate correctly
        /// </summary>
        [TestMethod]
        public void EvaluateTest1()
        {
            Formula formula = new("1 + _B2 - 10 / 2 * 3");
            Assert.AreEqual(-9.0, (double)formula.Evaluate(s => 5), 1e-9);
        }

        /// <summary>
        /// Lots of different valid token orderings should evaluate correctly
        /// </summary>
        [TestMethod]
        public void EvaluateTest2()
        {
            Formula formula = new("(1 - _B2) * 10 / (2 + 3)");
            Assert.AreEqual(-8.0, (double)formula.Evaluate(s => 5), 1e-9);
        }

        /// <summary>
        /// Lots of different valid token orderings should evaluate correctly
        /// </summary>
        [TestMethod]
        public void EvaluateTest3()
        {
            Formula formula = new("1 / ((_B2 * 10) - 2) * 3");
            Assert.AreEqual(0.0625, (double)formula.Evaluate(s => 5), 1e-9);
        }

        /// <summary>
        /// Lots of different valid token orderings should evaluate correctly
        /// </summary>
        [TestMethod]
        public void EvaluateTest4()
        {
            Formula formula = new("(1 - (_B2 * 10)) / 2 * 3");
            Assert.AreEqual(-73.5, (double)formula.Evaluate(s => 5), 1e-9);
        }

        /// <summary>
        /// Lots of different valid token orderings should evaluate correctly
        /// </summary>
        [TestMethod]
        public void EvaluateTest5()
        {
            Formula formula = new("(((1 + _B2) * 10) - 2) / 3");
            Assert.AreEqual(19.333333333333, (double)formula.Evaluate(s => 5), 1e-9);
        }

        /// <summary>
        /// Lots of different valid token orderings should evaluate correctly
        /// </summary>
        [TestMethod]
        public void EvaluateTest6()
        {
            Formula formula = new("1 * (_B2 + (10 / (2 - 3)))");
            Assert.AreEqual(-5.0, (double)formula.Evaluate(s => 5), 1e-9);
        }

        /// <summary>
        /// Lots of different valid token orderings should evaluate correctly
        /// </summary>
        [TestMethod]
        public void EvaluateTest7()
        {
            Formula formula = new("(1 * _B2) / (10 - (2 + 3))");
            Assert.AreEqual(1.0, (double)formula.Evaluate(s => 5), 1e-9);
        }
    }
}