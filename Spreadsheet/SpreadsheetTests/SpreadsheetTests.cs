// Written by Caden Erickson
// Last modified: 09/30/2022

using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests;

/// <summary>
/// This class contains unit tests to test the functionality of the Spreadsheet class.
/// </summary>
[TestClass]
public class SpreadsheetTests
{
    /// <summary>
    /// Initializing an empty spreadsheet should work
    /// </summary>
    [TestMethod]
    public void BlankSpreadsheet()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        Assert.IsNotNull(s);
    }

    /// <summary>
    /// The normalizer set by the 4-argument constructor should work as expected
    /// </summary>
    [TestMethod]
    public void Normalize()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => s.Length < 4, s => s.ToUpper(), "1");
        Assert.AreEqual("A1", s.Normalize("a1"));
        Assert.AreEqual("AAAA", s.Normalize("aAaA"));
        Assert.AreEqual("1234Y5678", s.Normalize("1234y5678"));
        Assert.AreEqual("AAAA", s.Normalize("AAAA"));
        Assert.AreEqual("12345", s.Normalize("12345"));
    }

    /// <summary>
    /// The validator set by the 4-argument constructor should work as expected
    /// </summary>
    [TestMethod]
    public void Validate()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => s.Length < 4, s => s.ToUpper(), "1");
        Assert.IsTrue(s.IsValid("a1"));
        Assert.IsTrue(s.IsValid("A11"));
        Assert.IsFalse(s.IsValid("aa11"));
        Assert.IsTrue(s.IsValid("a"));
        Assert.IsFalse(s.IsValid("aaaaaa111111"));
    }

    /// <summary>
    /// Getting the contents of an invalidly named cell should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetInvalidNameContents1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.GetCellContents("1A");
    }

    /// <summary>
    /// Getting the contents of an invalidly named cell should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetInvalidNameContents2()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => s.Length < 4, s => s.ToUpper(), "1");
        Assert.AreEqual("1", s.Version);
        s.GetCellContents("AA11");
    }

    /// <summary>
    /// Getting the contents of a cell where the normalizer is invalid should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetInvalidNameContents3()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => true, s => "1A", "1");
        Assert.AreEqual("1", s.Version);
        s.GetCellContents("A1");
    }

    /// <summary>
    /// Getting the contents of a cell where the normalizer does not satisfy the validator should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetInvalidNameContents4()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => char.IsUpper(s[0]), s => s.ToLower(), "1");
        Assert.AreEqual("1", s.Version);
        s.GetCellContents("A1");
    }

    /// <summary>
    /// Getting the contents of a nonempty cell should work
    /// </summary>
    [TestMethod]
    public void GetNonemptyContents()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "2.05");
        string contents = "" + s.GetCellContents("A1");

        Assert.AreEqual("2.05", contents);
    }

    /// <summary>
    /// Getting the contents of an empty cell should work
    /// </summary>
    [TestMethod]
    public void GetEmptyContents()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        string contents = (string)s.GetCellContents("A1");

        Assert.AreEqual("", contents);
    }

    /// <summary>
    /// Getting the contents of a previously nonempty, now empty cell should work
    /// </summary>
    [TestMethod]
    public void GetNewlyEmptyContents()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "2.05");
        s.SetContentsOfCell("A1", "");
        string contents = (string)s.GetCellContents("A1");

        Assert.AreEqual("", contents);
    }

    /// <summary>
    /// Getting the string contents of an cell after multiple changes should still work
    /// </summary>
    [TestMethod]
    public void GetNumberContentsAfterChanges()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "2.05");
        string contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("2.05", contents);

        s.SetContentsOfCell("A1", "6");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("6", contents);

        s.SetContentsOfCell("A1", "5e-10");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("5E-10", contents);
    }

    /// <summary>
    /// Getting the string contents of an cell after multiple changes should still work
    /// </summary>
    [TestMethod]
    public void GetStringContentsAfterChanges()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "first");
        string contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("first", contents);

        s.SetContentsOfCell("A1", "second");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("second", contents);

        s.SetContentsOfCell("A1", "third");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("third", contents);
    }

    /// <summary>
    /// Getting the string contents of an cell after multiple changes should still work
    /// </summary>
    [TestMethod]
    public void GetFormulaContentsAfterChanges()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=1 + 2 + 3");
        string contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("1+2+3", contents);

        s.SetContentsOfCell("A1", "=2 + B2 * 4");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("2+B2*4", contents);

        s.SetContentsOfCell("A1", "=3 - (4e-10) / 7");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("3-(4E-10)/7", contents);
    }

    /// <summary>
    /// Getting the non-empty cells of an empty spreadsheet should return an empty set
    /// </summary>
    [TestMethod]
    public void GetNonemptyCellsBlankSpreadsheet()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        IEnumerable<string> nonempties = s.GetNamesOfAllNonemptyCells();
        Assert.AreEqual(0, nonempties.Count());
    }

    /// <summary>
    /// Getting the non-empty cells of an spreadsheet whose nonempty cells have had their
    /// contents set to "" should return an empty set
    /// </summary>
    [TestMethod]
    public void GetNonemptyCellsEmptiedSpreadsheet()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.SetContentsOfCell("B2", "message");
        s.SetContentsOfCell("C3", "=6 * 8");
        s.SetContentsOfCell("A1", "");
        s.SetContentsOfCell("B2", "");
        s.SetContentsOfCell("C3", "");

        IEnumerable<string> nonempties = s.GetNamesOfAllNonemptyCells();
        Assert.AreEqual(0, nonempties.Count());
    }

    /// <summary>
    /// Getting the non-empty cells, where all their contents are the same type, should work
    /// </summary>
    [TestMethod]
    public void GetNonemptyCellsSingleType1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "2");
        s.SetContentsOfCell("B2", "4");
        s.SetContentsOfCell("C3", "6");
        s.SetContentsOfCell("D4", "8");
        s.SetContentsOfCell("E5", "10");

        IEnumerable<string> nonempties = s.GetNamesOfAllNonemptyCells();
        Assert.IsTrue(nonempties.Contains("A1"));
        Assert.IsTrue(nonempties.Contains("B2"));
        Assert.IsTrue(nonempties.Contains("C3"));
        Assert.IsTrue(nonempties.Contains("D4"));
        Assert.IsTrue(nonempties.Contains("E5"));
        Assert.AreEqual(5, nonempties.Count());
    }

    /// <summary>
    /// Getting the non-empty cells, where all their contents are the same type, should work
    /// </summary>
    [TestMethod]
    public void GetNonemptyCellsSingleType2()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "eyone");
        s.SetContentsOfCell("B2", "beetwo");
        s.SetContentsOfCell("C3", "ceethree");
        s.SetContentsOfCell("D4", "deefour");
        s.SetContentsOfCell("E5", "eefive");

        IEnumerable<string> nonempties = s.GetNamesOfAllNonemptyCells();
        Assert.IsTrue(nonempties.Contains("A1"));
        Assert.IsTrue(nonempties.Contains("B2"));
        Assert.IsTrue(nonempties.Contains("C3"));
        Assert.IsTrue(nonempties.Contains("D4"));
        Assert.IsTrue(nonempties.Contains("E5"));
        Assert.AreEqual(5, nonempties.Count());
    }

    /// <summary>
    /// Getting the non-empty cells, where all their contents are the same type, should work
    /// </summary>
    [TestMethod]
    public void GetNonemptyCellsSingleType3()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=1+2");
        s.SetContentsOfCell("B2", "=2+3");
        s.SetContentsOfCell("C3", "=3+4");
        s.SetContentsOfCell("D4", "=4+5");
        s.SetContentsOfCell("E5", "=5+6");
            
        IEnumerable<string> nonempties = s.GetNamesOfAllNonemptyCells();
        Assert.IsTrue(nonempties.Contains("A1"));
        Assert.IsTrue(nonempties.Contains("B2"));
        Assert.IsTrue(nonempties.Contains("C3"));
        Assert.IsTrue(nonempties.Contains("D4"));
        Assert.IsTrue(nonempties.Contains("E5"));
        Assert.AreEqual(5, nonempties.Count());
    }

    /// <summary>
    /// Getting the non-empty cells, where all their contents are different types, should work
    /// </summary>
    [TestMethod]
    public void GetNonemptyCellsMultipleTypes()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=2+3");
        s.SetContentsOfCell("B2", "beetwo");
        s.SetContentsOfCell("C3", "=4+5");
        s.SetContentsOfCell("D4", "8");
        s.SetContentsOfCell("E5", "eefive");
        s.SetContentsOfCell("F6", "12");
        s.SetContentsOfCell("G7", "14");
        s.SetContentsOfCell("H8", "=9+10");
        s.SetContentsOfCell("I9", "ainine");
        s.SetContentsOfCell("J10", "jayten");
        s.SetContentsOfCell("K11", "22");
        s.SetContentsOfCell("L12", "=13+14");
        s.SetContentsOfCell("M13", "emthirteen");
        s.SetContentsOfCell("N14", "=15+16");
        s.SetContentsOfCell("O15", "30");


        IEnumerable<string> nonempties = s.GetNamesOfAllNonemptyCells();
        Assert.IsTrue(nonempties.Contains("A1"));
        Assert.IsTrue(nonempties.Contains("B2"));
        Assert.IsTrue(nonempties.Contains("C3"));
        Assert.IsTrue(nonempties.Contains("D4"));
        Assert.IsTrue(nonempties.Contains("E5"));
        Assert.IsTrue(nonempties.Contains("F6"));
        Assert.IsTrue(nonempties.Contains("G7"));
        Assert.IsTrue(nonempties.Contains("H8"));
        Assert.IsTrue(nonempties.Contains("I9"));
        Assert.IsTrue(nonempties.Contains("J10"));
        Assert.IsTrue(nonempties.Contains("K11"));
        Assert.IsTrue(nonempties.Contains("L12"));
        Assert.IsTrue(nonempties.Contains("M13"));
        Assert.IsTrue(nonempties.Contains("N14"));
        Assert.IsTrue(nonempties.Contains("O15"));
        Assert.AreEqual(15, nonempties.Count());
    }

    /// <summary>
    /// Setting the contents of an invalidly named cell should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetInvalidNameContents1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("1A", "10");
    }

    /// <summary>
    /// Setting the contents of an invalidly named cell should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetInvalidNameContents2()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A$1", "10");
    }

    /// <summary>
    /// Setting the contents of an invalidly named cell should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetInvalidNameContents3()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("", "10");
    }

    /// <summary>
    /// Setting the contents of an invalidly named cell should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetInvalidNameContents4()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => s.Length < 4, s => s.ToUpper(), "1");
        Assert.AreEqual("1", s.Version);
        s.SetContentsOfCell("AA11", "10");
    }

    /// <summary>
    /// Setting the contents of a cell where the normalizer is invalid should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetInvalidNameContents5()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => true, s => "1A", "1");
        Assert.AreEqual("1", s.Version);
        s.SetContentsOfCell("A1", "10");
    }

    /// <summary>
    /// Setting the contents of a cell where the normalizer does not satisfy the validator should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetInvalidNameContents6()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => char.IsUpper(s[0]), s => s.ToLower(), "1");
        Assert.AreEqual("1", s.Version);
        s.SetContentsOfCell("A1", "10");
    }

    /// <summary>
    /// Setting the contents of a cell with a very long name should work
    /// </summary>
    [TestMethod]
    public void SetVeryLongName()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("kJOCkjeoilwoCIkjokleMBlwooCK1098309498710912898283402089311341", "10");
        Assert.AreEqual(10.0, s.GetCellContents("kJOCkjeoilwoCIkjokleMBlwooCK1098309498710912898283402089311341"));
    }

    /// <summary>
    /// Setting the contents of a cell to a very long string should work
    /// </summary>
    [TestMethod]
    public void SetVeryLongContents()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "_x1JGiU_109ulwk4ENgMO81U7T563L_1000_lwIOIWUUUUU9LIWJi10928466574220_12");
        Assert.AreEqual("_x1JGiU_109ulwk4ENgMO81U7T563L_1000_lwIOIWUUUUU9LIWJi10928466574220_12", s.GetCellContents("A1"));
    }

    /// <summary>
    /// Setting the contents of an empty cell to an empty string should work, and cause the cell to remain empty
    /// </summary>
    [TestMethod]
    public void SetEmptyContents1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "");
        Assert.AreEqual(0, s.GetNamesOfAllNonemptyCells().Count());
    }

    /// <summary>
    /// Setting the contents of a nonempty cell to an empty string should work, and cause the cell to become empty
    /// </summary>
    [TestMethod]
    public void SetEmptyContents2()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "txt");
        Assert.AreEqual(1, s.GetNamesOfAllNonemptyCells().Count());

        s.SetContentsOfCell("A1", "");
        Assert.AreEqual(0, s.GetNamesOfAllNonemptyCells().Count());
    }

    /// <summary>
    /// Changing the contents of a cell from a double to a string should work
    /// </summary>
    [TestMethod]
    public void SetContentsTypeChangeDS()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "2.05");
        string contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("2.05", contents);

        s.SetContentsOfCell("A1", "text");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("text", contents);
    }

    /// <summary>
    /// Changing the contents of a cell from a double to a formula should work
    /// </summary>
    [TestMethod]
    public void SetContentsTypeChangeDF()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "2.05");
        string contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("2.05", contents);

        s.SetContentsOfCell("A1", "=5 + 6 - 2");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("5+6-2", contents);
    }

    /// <summary>
    /// Changing the contents of a cell from a string to a double should work
    /// </summary>
    [TestMethod]
    public void SetContentsTypeChangeSD()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "text");
        string contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("text", contents);

        s.SetContentsOfCell("A1", "2.05");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("2.05", contents);
    }

    /// <summary>
    /// Changing the contents of a cell from a string to a formula should work
    /// </summary>
    [TestMethod]
    public void SetContentsTypeChangeSF()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "text");
        string contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("text", contents);

        s.SetContentsOfCell("A1", "=5 + 6 - 2");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("5+6-2", contents);
    }

    /// <summary>
    /// Changing the contents of a cell from a formula to a double should work
    /// </summary>
    [TestMethod]
    public void SetContentsTypeChangeFD()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=5 + 6 - 2");
        string contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("5+6-2", contents);

        s.SetContentsOfCell("A1", "2.05");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("2.05", contents);
    }

    /// <summary>
    /// Changing the contents of a cell from a formula to a string should work
    /// </summary>
    [TestMethod]
    public void SetContentsTypeChangeFS()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=5 + 6 - 2");
        string contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("5+6-2", contents);

        s.SetContentsOfCell("A1", "2.05");
        contents = "" + s.GetCellContents("A1");
        Assert.AreEqual("2.05", contents);
    }

    /// <summary>
    /// Setting the contents of a cell with no dependents should return an empty list
    /// </summary>
    [TestMethod]
    public void SetContentsNoDependents1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.SetContentsOfCell("B2", "=2");
        s.SetContentsOfCell("C3", "=3");
        s.SetContentsOfCell("D4", "=4");
        s.SetContentsOfCell("E5", "=5");


        IEnumerable<string> toRecalc = s.SetContentsOfCell("A1", "2.05");
        Assert.AreEqual(1, toRecalc.Count());
    }

    /// <summary>
    /// Setting the contents of a cell with no dependents should return an empty list
    /// </summary>
    [TestMethod]
    public void SetContentsMultipleDependents()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "5");
        s.SetContentsOfCell("B2", "=2 * A1");
        s.SetContentsOfCell("C3", "=2 * A1");
        s.SetContentsOfCell("D4", "=2 * B2");
        s.SetContentsOfCell("E5", "=2 * D4");


        List<string> toRecalc = s.SetContentsOfCell("A1", "2.05").ToList();
        string[] expected = { "A1", "B2", "C3", "D4", "E5"};

        Assert.AreEqual(5, toRecalc.Count);
        for (int i = 0; i < toRecalc.Count; i++)
        {
            Assert.IsTrue(toRecalc.Contains(expected[i]));
        }

        Assert.IsTrue(toRecalc.IndexOf("A1") < toRecalc.IndexOf("B2"));
        Assert.IsTrue(toRecalc.IndexOf("A1") < toRecalc.IndexOf("C3"));
        Assert.IsTrue(toRecalc.IndexOf("B2") < toRecalc.IndexOf("D4"));
        Assert.IsTrue(toRecalc.IndexOf("D4") < toRecalc.IndexOf("E5"));
    }

    /// <summary>
    /// Setting the contents of a cell to Formula with a dependency on itself should throw an exception
    /// </summary>
    [TestMethod]
    public void SetCircularDependencyContents1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        Assert.ThrowsException<CircularException>(() => s.SetContentsOfCell("A1", "=5 * 3 + A1 - 7"));
        Assert.AreEqual("", s.GetCellContents("A1"));
        Assert.AreEqual(0, s.GetNamesOfAllNonemptyCells().Count());
    }

    /// <summary>
    /// Setting the contents of a cell to Formula that creates a circular dependency should throw an exception
    /// </summary>
    [TestMethod]
    public void SetCircularDependencyContents2()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "=5 * 3 + B2 - 7");
        s.SetContentsOfCell("B2", "=7 + (C3 - 2) / 4");
        s.SetContentsOfCell("C3", "=6 + D4 + D4");
        s.SetContentsOfCell("D4", "=E5 - (2 + 7 / 3)");
        Assert.ThrowsException<CircularException>(() => s.SetContentsOfCell("E5", "=7 * 8 / (1 + A1)"));
        Assert.AreEqual("", s.GetCellContents("E5"));
        Assert.AreEqual(4, s.GetNamesOfAllNonemptyCells().Count());
    }

    /// <summary>
    /// Dealing with hundreds of nonempty cells should still be doable quickly
    /// </summary>
    [TestMethod, Timeout(3000)]
    public void StressTest1()
    {
        int SIZE = 1000;
        string[] names = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            names[i] = "A" + i;
        }

        AbstractSpreadsheet s = new Spreadsheet();

        for (int i = 0; i < SIZE; i++)
        {
            s.SetContentsOfCell(names[i], "=" + i + " * B1");
        }

        IEnumerable<string> toRecalc = s.SetContentsOfCell("B1", "5");

        Assert.AreEqual(SIZE + 1, toRecalc.Count());

        for (int i = 0; i < SIZE; i++)
        {
            s.SetContentsOfCell(names[i], "=" + i + " * 5");
        }

        toRecalc = s.SetContentsOfCell("B1", "5");

        Assert.AreEqual(1, toRecalc.Count());
    }

    /// <summary>
    /// Saving and loading a spreadsheet with hundreds of nonempty cells
    /// should still be doable quickly
    /// </summary>
    [TestMethod, Timeout(3000)]
    public void StressTest2()
    {
        int SIZE = 1000;
        string[] names = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            names[i] = "A" + i;
        }

        AbstractSpreadsheet s = new Spreadsheet();

        for (int i = 0; i < SIZE; i++)
        {
            s.SetContentsOfCell(names[i], "=" + i + " * B1");
        }

        IEnumerable<string> toRecalc = s.SetContentsOfCell("B1", "5");

        Assert.AreEqual(SIZE + 1, toRecalc.Count());

        s.Save("testFile.json");

        AbstractSpreadsheet s2 = new Spreadsheet("testFile.json", s => true, s => s, "default");

        Assert.AreEqual(SIZE + 1, s2.GetNamesOfAllNonemptyCells().Count());

        for (int i = 0; i < SIZE; i++)
        {
            Assert.AreEqual(i * 5.0, s2.GetCellValue(names[i]));
        }
    }

    /// <summary>
    /// Normal save process should work
    /// </summary>
    [TestMethod]
    public void SaveTest()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=2+3");
        s.SetContentsOfCell("B2", "beetwo");
        s.SetContentsOfCell("C3", "=4+5");
        s.SetContentsOfCell("D4", "8");
        s.SetContentsOfCell("E5", "eefive");
        s.SetContentsOfCell("F6", "12");

        Assert.IsTrue(s.Changed);

        s.Save("testFile.json");

        Assert.IsFalse(s.Changed);
    }

    /// <summary>
    /// Saving a spreadsheet to a nonexistent filepath should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveErrorTest1()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=2+3");
        s.SetContentsOfCell("B2", "beetwo");
        s.SetContentsOfCell("C3", "=4+5");
        s.SetContentsOfCell("D4", "8");
        s.SetContentsOfCell("E5", "eefive");
        s.SetContentsOfCell("F6", "12");

        Assert.IsTrue(s.Changed);

        s.Save("/this/doesnt/exist.txt");
    }

    /// <summary>
    /// Saving a spreadsheet to a blank filepath should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveErrorTest2()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=2+3");
        s.SetContentsOfCell("B2", "beetwo");
        s.SetContentsOfCell("C3", "=4+5");
        s.SetContentsOfCell("D4", "8");
        s.SetContentsOfCell("E5", "eefive");
        s.SetContentsOfCell("F6", "12");

        Assert.IsTrue(s.Changed);

        s.Save("");
    }

    /// <summary>
    /// Normal save process should work
    /// </summary>
    [TestMethod]
    public void SaveAndLoadTest()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=2+3");
        s.SetContentsOfCell("B2", "beetwo");
        s.SetContentsOfCell("C3", "=4+5");
        s.SetContentsOfCell("D4", "8");
        s.SetContentsOfCell("E5", "eefive");
        s.SetContentsOfCell("F6", "12");

        Assert.IsTrue(s.Changed);

        s.Save("testFile.json");

        Assert.IsFalse(s.Changed);

        AbstractSpreadsheet s2 = new Spreadsheet("testFile.json", s => true, s => s, "default");

        Assert.AreEqual("2+3", s2.GetCellContents("A1").ToString());
        Assert.AreEqual("beetwo", s2.GetCellContents("B2"));
        Assert.AreEqual("4+5", s2.GetCellContents("C3").ToString());
        Assert.AreEqual(8.0, s2.GetCellContents("D4"));
        Assert.AreEqual("eefive", s2.GetCellContents("E5"));
        Assert.AreEqual(12.0, s2.GetCellContents("F6"));

        Assert.IsFalse(s2.Changed);

    }

    /// <summary>
    /// Saving a file, then loading from a nonexistent filepath should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveAndLoadErrorTest1()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=2+3");
        s.SetContentsOfCell("B2", "beetwo");
        s.SetContentsOfCell("C3", "=4+5");
        s.SetContentsOfCell("D4", "8");
        s.SetContentsOfCell("E5", "eefive");
        s.SetContentsOfCell("F6", "12");

        Assert.IsTrue(s.Changed);

        s.Save("testFile.json");

        Assert.IsFalse(s.Changed);

        AbstractSpreadsheet s2 = new Spreadsheet("/this/doesnt/exist.txt", s => true, s => s, "default");
    }

    /// <summary>
    /// Saving a file, then loading from a nonexistent file should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveAndLoadErrorTest2()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=2+3");
        s.SetContentsOfCell("B2", "beetwo");
        s.SetContentsOfCell("C3", "=4+5");
        s.SetContentsOfCell("D4", "8");
        s.SetContentsOfCell("E5", "eefive");
        s.SetContentsOfCell("F6", "12");

        Assert.IsTrue(s.Changed);

        s.Save("testFile.json");

        Assert.IsFalse(s.Changed);

        AbstractSpreadsheet s2 = new Spreadsheet("nothingtoseehere.json", s => true, s => s, "default");
    }

    /// <summary>
    /// Saving a file, then loading from a blank filepath should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveAndLoadErrorTest3()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=2+3");
        s.SetContentsOfCell("B2", "beetwo");
        s.SetContentsOfCell("C3", "=4+5");
        s.SetContentsOfCell("D4", "8");
        s.SetContentsOfCell("E5", "eefive");
        s.SetContentsOfCell("F6", "12");

        Assert.IsTrue(s.Changed);

        s.Save("testFile.json");

        Assert.IsFalse(s.Changed);

        AbstractSpreadsheet s2 = new Spreadsheet("", s => true, s => s, "default");
    }

    /// <summary>
    /// Saving a file with one version, then loading it while specifying
    /// a different version should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveAndLoadErrorTest4()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=2+3");
        s.SetContentsOfCell("B2", "beetwo");
        s.SetContentsOfCell("C3", "=4+5");
        s.SetContentsOfCell("D4", "8");
        s.SetContentsOfCell("E5", "eefive");
        s.SetContentsOfCell("F6", "12");

        Assert.IsTrue(s.Changed);

        s.Save("testFile.json");

        Assert.IsFalse(s.Changed);

        AbstractSpreadsheet s2 = new Spreadsheet("testFile.json", s => true, s => s, "notdefault");
    }

    /// <summary>
    /// Building a spreadsheet from an empty file should throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void LoadNullTest()
    {
        File.WriteAllText("testFile.json", "");

        AbstractSpreadsheet s = new Spreadsheet("testFile.json", s => true, s => s, "default");
    }

    /// <summary>
    /// A spreadsheet object built with non-default delegates should behave as expected
    /// </summary>
    [TestMethod]
    public void BuildWithDelegatesTest()
    {
        AbstractSpreadsheet s = new Spreadsheet(s => s[0] != 'z', s => s.ToUpper(), "1");
        Assert.AreEqual("1", s.Version);

        s.SetContentsOfCell("a1", "8");
        s.SetContentsOfCell("b2", "12");
        s.SetContentsOfCell("c3", "=a1+3");
        s.SetContentsOfCell("d4", "=b2+5");
        s.SetContentsOfCell("e5", "=z8+1");

        Assert.IsInstanceOfType(s.GetCellContents("C3"), typeof(Formula));
        Assert.AreEqual("A1+3", s.GetCellContents("C3").ToString());
        Assert.AreEqual(11.0, s.GetCellValue("C3"));

        Assert.IsInstanceOfType(s.GetCellContents("D4"), typeof(Formula));
        Assert.AreEqual("B2+5", s.GetCellContents("D4").ToString());
        Assert.AreEqual(17.0, s.GetCellValue("D4"));

        Assert.IsInstanceOfType(s.GetCellContents("E5"), typeof(Formula));
        Assert.AreEqual("Z8+1", s.GetCellContents("E5").ToString());
        Assert.IsInstanceOfType(s.GetCellValue("E5"), typeof(FormulaError));
    }

    /// <summary>
    /// Getting the value of a cell with contents that either are, or evaluate to, a double
    /// should return a double
    /// </summary>
    [TestMethod]
    public void GetValueDouble()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=2+3");
        Assert.AreEqual(5.0, s.GetCellValue("A1"));
            
        s.SetContentsOfCell("A1", "=5*7");
        Assert.AreEqual(35.0, s.GetCellValue("A1"));

        s.SetContentsOfCell("A1", "7.5");
        s.SetContentsOfCell("B2", "=A1-5");
        Assert.AreEqual(7.5, s.GetCellValue("A1"));
        Assert.AreEqual(2.5, s.GetCellValue("B2"));
    }

    /// <summary>
    /// Getting the value of a cell with contents that are a string
    /// should return a string
    /// </summary>
    [TestMethod]
    public void GetValueString()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "hello");
        Assert.AreEqual("hello", s.GetCellValue("A1"));

        s.SetContentsOfCell("A1", "hiya");
        Assert.AreEqual("hiya", s.GetCellValue("A1"));

        s.SetContentsOfCell("A1", "");
        Assert.AreEqual("", s.GetCellValue("A1"));
    }

    /// <summary>
    /// Getting the value of a cell that depends on a non-numerical cell
    /// should return a FormulaError
    /// </summary>
    [TestMethod]
    public void GetValueError1()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=B2-5");
        s.SetContentsOfCell("B2", "nope");
        Assert.IsInstanceOfType(s.GetCellValue("A1"), typeof(FormulaError));
    }

    /// <summary>
    /// Getting the value of a cell that depends on a non-numerical cell
    /// should return a FormulaError
    /// </summary>
    [TestMethod]
    public void GetValueError2()
    {
        AbstractSpreadsheet s = new Spreadsheet();

        s.SetContentsOfCell("A1", "=B2-5");
        Assert.IsInstanceOfType(s.GetCellValue("A1"), typeof(FormulaError));
    }
}