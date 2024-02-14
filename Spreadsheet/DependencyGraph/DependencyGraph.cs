// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
//
//
// Implementation of skeleton code written by Caden Erickson
// Last updated: 09/08/2022
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{ 
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private readonly Dictionary<string, HashSet<string>> dependents;
        private readonly Dictionary<string, HashSet<string>> dependees;
        private int numPairs;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
            numPairs = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                return numPairs;
            }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                return GetDependees(s).Count();
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            // GetDependents will handle s not being in the dependents Dictionary
            return GetDependents(s).Any();
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            // GetDependees will handle s not being in the dependees Dictionary
            return GetDependees(s).Any();
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            // If s isn't in the dependents dictionary, just return a empty HashSet
            if (!dependents.ContainsKey(s))
                return new HashSet<string>();

            return dependents[s];
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            // If s isn't in the dependees dictionary, just return a empty HashSet
            if (!dependees.ContainsKey(s))
                return new HashSet<string>();

            return dependees[s];
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>
        public void AddDependency(string s, string t)
        {
            // Initalize s's dependents list, if not already done
            if (!dependents.ContainsKey(s))
                dependents.Add(s, new HashSet<string>());

            // Initialize t's dependees list, if not already done
            if (!dependees.ContainsKey(t))
                dependees.Add(t, new HashSet<string>());

            // Add (s, t) pair to both lists. Increment numPairs if the adds were successful
            if (dependents[s].Add(t) && dependees[t].Add(s))
                numPairs++;
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            // If s isn't in the dependents dictionary, (s, t) doesn't exist
            if (!dependents.ContainsKey(s)) return;

            // Remove the dependee->dependent relationship
            dependents[s].Remove(t);
            // Then, if s has no more dependents, remove it from the dependents Dictionary completely
            if (dependents[s].Count == 0)
                dependents.Remove(s);

            // Remove the dependent->dependee relationship
            dependees[t].Remove(s);
            // Then, if t has no more dependees, remove it from the dependees Dictionary completely
            if (dependees[t].Count == 0)
                dependees.Remove(t);

            numPairs--;
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            // Copy of dependents list, to avoid concurrent modification errors
            HashSet<string> dependentsCopy = new(GetDependents(s));

            // Remove each pair using RemoveDependency so that both dictionaries are updated correctly
            foreach (string dependent in dependentsCopy)
            {
                RemoveDependency(s, dependent);
            }


            // Add each new dependency using AddDependency so that both dictionaries are updated correctly
            foreach (string newDependent in newDependents)
            {
                AddDependency(s, newDependent);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            // Copy of dependees list, to avoid concurrent modification errors
            HashSet<string> dependeesCopy = new(GetDependees(s));

            // Remove each pair using RemoveDependency so that both dictionaries are updated correctly
            foreach (string dependee in dependeesCopy)
            {
                RemoveDependency(dependee, s);
            }


            // Add each new dependency using AddDependency so that both dictionaries are updated correctly
            foreach (string newDependee in newDependees)
            {
                AddDependency(newDependee, s);
            }
        }
    }
}
