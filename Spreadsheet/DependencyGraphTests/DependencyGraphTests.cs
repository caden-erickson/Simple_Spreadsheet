// Initial tests provided by Daniel Kopta and Travis Martin
//
// Further implementation and tests written by Caden Erickson
// Last updated: 09/08/2022
//

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /// <summary>
    /// This is a test class for DependencyGraphTest and is intended
    /// to contain all DependencyGraphTest Unit Tests
    /// </summary>
    [TestClass()]
    public class DependencyGraphTest
    {
        // ====================================== SIMPLE INITIAL TESTS ======================================

        /// <summary>
        /// Empty graph should contain nothing
        /// </summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new();
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        /// Empty graph should contain nothing
        /// </summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }

        /// <summary>
        /// Non-empty graph contains something
        /// </summary>
        [TestMethod()]
        public void NonEmptySizeTest()
        {
            DependencyGraph t = new();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        // ====================================== INDEXER TESTS ======================================

        /// <summary>
        /// Empty graph should have no dependees in x's list
        /// </summary>
        [TestMethod()]
        public void IndexerEmptyTest1()
        {
            DependencyGraph t = new();
            Assert.AreEqual("0", "" + t["x"]);
        }

        /// <summary>
        /// Newly empty graph should have no dependees in a's or c's list
        /// </summary>
        [TestMethod()]
        public void IndexerEmptyTest2()
        {
            DependencyGraph t = new();
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.RemoveDependency("a", "c");
            t.RemoveDependency("c", "b");
            Assert.AreEqual("0", "" + t["a"]);
            Assert.AreEqual("0", "" + t["c"]);
        }

        /// <summary>
        /// Index should work on a graph with multiple dependence relations 
        /// </summary>
        [TestMethod()]
        public void IndexerSimpleTest()
        {
            DependencyGraph t = new();
            t.AddDependency("x", "y");
            t.AddDependency("x", "z");
            t.AddDependency("r", "x");
            t.AddDependency("q", "x");
            Assert.AreEqual("2", "" + t["x"]);
        }


        // ====================================== HASDEPENDENTS() TESTS ======================================

        /// <summary>
        /// An empty graph should have no dependents of x
        /// </summary>
        [TestMethod()]
        public void HasDependentsEmptyTest1()
        {
            DependencyGraph t = new();
            Assert.IsFalse(t.HasDependents("x"));
        }

        /// <summary>
        /// A newly empty graph should have no dependents of x
        /// </summary>
        [TestMethod()]
        public void HasDependentsEmptyTest2()
        {
            DependencyGraph t = new();
            t.AddDependency("x", "y");
            t.AddDependency("x", "z");
            t.RemoveDependency("x", "y");
            t.RemoveDependency("x", "z");
            Assert.IsFalse(t.HasDependents("x"));
        }

        /// <summary>
        /// HasDependents should work on a graph with dependence relations 
        /// </summary>
        [TestMethod()]
        public void HasDependentsSimpleTest()
        {
            DependencyGraph t = new();
            t.AddDependency("x", "y");
            t.AddDependency("x", "z");
            t.AddDependency("r", "x");
            t.AddDependency("q", "x");
            Assert.IsTrue(t.HasDependents("x"));
        }


        // ====================================== HASDEPENDEES() TESTS ======================================

        /// <summary>
        /// An empty graph should have no dependees of x
        /// </summary>
        [TestMethod()]
        public void HasDependeesEmptyTest1()
        {
            DependencyGraph t = new();
            Assert.IsFalse(t.HasDependees("x"));
        }

        /// <summary>
        /// A newly empty graph should have no dependees of x
        /// </summary>
        [TestMethod()]
        public void HasDependeesEmptyTest2()
        {
            DependencyGraph t = new();
            t.AddDependency("y", "x");
            t.AddDependency("z", "x");
            t.RemoveDependency("y", "x");
            t.RemoveDependency("z", "x");
            Assert.IsFalse(t.HasDependees("x"));
        }

        /// <summary>
        /// HasDependees should work on a graph with dependence relations
        /// </summary>
        [TestMethod()]
        public void HasDependeesSimpleTest()
        {
            DependencyGraph t = new();
            t.AddDependency("x", "y");
            t.AddDependency("x", "z");
            t.AddDependency("r", "x");
            t.AddDependency("q", "x");
            Assert.IsTrue(t.HasDependees("x"));
        }


        // ====================================== ENUMERATION TESTS ======================================

        /// <summary>
        /// Non-empty graph contains something
        /// </summary>
        [TestMethod()]
        public void DependentsEnumeratorTest()
        {
            DependencyGraph t = new();
            t.AddDependency("b", "a");
            t.AddDependency("c", "a");
            t.AddDependency("b", "c");
            t.AddDependency("d", "b");

            IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependents("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependents("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependents("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        /// Non-empty graph contains something
        /// </summary>
        [TestMethod()]
        public void DependeesEnumeratorTest()
        {
            DependencyGraph t = new();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        /// <summary>
        /// Empty graph should contain nothing
        /// </summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }

        /// <summary>
        /// Non-empty graph contains something
        /// </summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }


        // ====================================== ADD TESTS ======================================

        /// <summary>
        /// Adding a pre-existent ordered pair shouldn't fail
        /// </summary>
        [TestMethod()]
        public void AddExistingDependencyTest()
        {
            DependencyGraph t = new();
            t.AddDependency("x", "y");
            Assert.AreEqual("1", "" + t.Size);
            t.AddDependency("x", "y");
            Assert.AreEqual("1", "" + t.Size);
        }

        // ====================================== REMOVE TESTS ======================================

        /// <summary>
        /// Remove on an empty graph shouldn't fail
        /// </summary>
        [TestMethod()]
        public void RemoveDependencyEmptyTest()
        {
            DependencyGraph t = new();
            Assert.AreEqual(0, t.Size);
            t.RemoveDependency("x", "y");
        }


        // ====================================== REPLACE TESTS ======================================

        /// <summary>
        /// Replace on an empty DG shouldn't fail
        /// </summary>
        [TestMethod()]
        public void EmptyReplaceTest1()
        {
            DependencyGraph t = new();
            Assert.AreEqual(t.Size, 0);
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
            Assert.AreEqual(t.Size, 0);
        }

        /// <summary>
        /// Replace on a newly empty DG shouldn't fail
        /// </summary>
        [TestMethod()]
        public void EmptyReplaceTest2()
        {
            DependencyGraph t = new();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(t.Size, 0);
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
            Assert.AreEqual(t.Size, 0);
        }

        /// <summary>
        /// Multiple replacements should correctly change the DG's size
        /// </summary>
        [TestMethod()]
        public void ReplaceDependentsTest()
        {
            DependencyGraph t = new();
            t.AddDependency("x", "a");
            t.AddDependency("x", "b");
            t.AddDependency("x", "c");
            Assert.AreEqual(t.Size, 3);

            t.ReplaceDependents("x", new HashSet<string>() { "d", "e" });
            Assert.AreEqual(t.Size, 2);

            t.ReplaceDependents("x", new HashSet<string>() { "f" });
            Assert.AreEqual(t.Size, 1);

            t.ReplaceDependents("x", new HashSet<string>());
            Assert.AreEqual(t.Size, 0);
            Assert.IsFalse(t.HasDependents("x"));
        }

        /// <summary>
        /// Multiple replacements should correctly change the DG's size
        /// </summary>
        [TestMethod()]
        public void ReplaceDependeesTest()
        {
            DependencyGraph t = new();
            t.AddDependency("a", "x");
            t.AddDependency("b", "x");
            t.AddDependency("c", "x");
            Assert.AreEqual(t.Size, 3);
            Assert.AreEqual(t["x"], 3);

            t.ReplaceDependees("x", new HashSet<string>() { "d", "e" });
            Assert.AreEqual(t.Size, 2);
            Assert.AreEqual(t["x"], 2);

            t.ReplaceDependees("x", new HashSet<string>() { "f" });
            Assert.AreEqual(t.Size, 1);
            Assert.AreEqual(t["x"], 1);

            t.ReplaceDependees("x", new HashSet<string>());
            Assert.AreEqual(t.Size, 0);
            Assert.AreEqual(t["x"], 0);
            Assert.IsFalse(t.HasDependees("x"));
        }


        // ====================================== OTHER TESTS ======================================

        /// <summary>
        /// It should be possible to have more than one DG at a time.
        /// </summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new();
            DependencyGraph t2 = new();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }

        /// <summary>
        /// Using lots of data
        /// </summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

    }
}