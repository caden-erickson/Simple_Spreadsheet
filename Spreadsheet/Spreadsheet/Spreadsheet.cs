// Full functionality (beyond skeleton code) written by Caden Erickson
// Last modified: 09/30/2022

using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SS;

/// <summary>
/// A Spreadsheet object represents the current state of a spreadsheet. It contains an infinite
/// number of validly-name cells.
/// 
/// A string is a valid cell name if and only if it consists of one or more letters (A to Z, case insensitive)
/// followed by one or more digits (0 to 9).
///   
/// Each cell within a Spreadsheet has both contents and a value- they may be different.
/// 
/// Unedited, or empty, cells have "" as both their contents and their value.
/// 
/// Circular dependencies among cells are not allowed.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class Spreadsheet : AbstractSpreadsheet
{
    // ==================================  F I E L D S   &   P R O P E R T I E S  ==================================

    // Stores mappings of the names of all the nonempty (having contents other than "") cells
    // in this Spreadsheet object to their correspondent Cell objects
    [JsonProperty(PropertyName = "cells")]
    private readonly Dictionary<string, Cell> nonemptyCells;

    // Stores all the dependency relation between cells in this Spreadsheet object
    private readonly DependencyGraph dependencies;

    /// <summary>
    /// Tracks whether the current spreadsheet object has been changed since the last
    /// save to a file.
    /// </summary>
    public override bool Changed { get; protected set; }



    // =========================================  C O N S T R U C T O R S  =========================================

    /// <summary>
    /// Default constructor: creates a new Spreadsheet object.<br/>
    /// No extra validity conditions; cell names are normalized to themselves.
    /// Version: default
    /// </summary>
    public Spreadsheet() : base(s => true, s => s, "default")
    {
        nonemptyCells = new();
        dependencies = new();
    }

    /// <summary>
    /// Creates a new spreadsheet object with the given validity and normalization delegates, and given version.
    /// </summary>
    /// <param name="isValid">Method to test variable validity</param>
    /// <param name="normalize">Method to normalize variables</param>
    /// <param name="version">The version of the spreadsheet software creating this spreadsheet object</param>
    public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
        : base(isValid, normalize, version)
    {
        nonemptyCells = new();
        dependencies = new();
    }

    /// <summary>
    /// Creates a new spreadsheet object with the given validity and normalization delegates, and given version.<br/>
    /// Uses the file at the given filepath to build the spreadsheet object.
    /// </summary>
    /// <param name="filepath">The filepath of the file from which to create this spreadsheet object</param>
    /// <param name="isValid">Method to test variable validity</param>
    /// <param name="normalize">Method to normalize variables</param>
    /// <param name="version">The version of the spreadsheet software creating this spreadsheet object</param>
    public Spreadsheet(string filepath, Func<string, bool> isValid, Func<string, string> normalize, string version)
        : base(isValid, normalize, version)
    {
        nonemptyCells = new();
        dependencies = new();

        // Read in file using filepath, build spreadsheet object from created object
        try
        {
            Spreadsheet? newSS = JsonConvert.DeserializeObject<Spreadsheet>(File.ReadAllText(filepath));
            if (newSS == null)
                throw new SpreadsheetReadWriteException("The file at " + filepath + " is blank.");

            // Build in all of the cell objects
            foreach(string cellName in newSS.GetNamesOfAllNonemptyCells())
            {
                SetContentsOfCell(cellName, newSS.nonemptyCells[cellName].ContentsAsString);
            }

            // Versions need to match, throw exception if they don't
            if (version != newSS.Version)
                throw new SpreadsheetReadWriteException("The specified version and the version in the saved file do not match.");
        }
        catch(Exception ex)
        {
            string message;
            if (ex is DirectoryNotFoundException)
                message = "The given directory was not found.";
            else if (ex is FileNotFoundException)
                message = "The specified file was not found";
            else if (ex is SpreadsheetReadWriteException)
                message = ex.Message;
            else
                message = "Error reading spreadsheet from file located at: " + filepath;
            throw new SpreadsheetReadWriteException(message);
        }

        // Even though technically the constructed object itself was modified,
        // loading a spreadsheet from file doesn't count as changing it
        Changed = false;
    }



    // ==============================================  M E T H O D S  ==============================================

    /// <summary>
    /// Saves a JSON representation of the current spreadsheet object to the file
    /// at the given filepath.
    /// </summary>
    /// <param name="filename">The path at which to save the JSON string</param>
    public override void Save(string filename)
    {
        try
        {
            // Serialize this object, write to file
            File.WriteAllText(filename, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        catch(Exception ex)
        {
            string message;
            if (ex is DirectoryNotFoundException)
                message = "The given directory was not found.";
            else
                message = "Error writing spreadsheet to file located at: " + filename;
            throw new SpreadsheetReadWriteException(message);
        }

        Changed = false;
    }

    /// <summary>
    /// Returns the value of the cell identified by the given name.
    /// </summary>
    /// <param name="name">The name of the cell whose value is being gotten</param>
    /// <returns>The value of the named cell</returns>
    /// <exception cref="InvalidNameException">If the given name is an invalid cell name</exception>
    public override object GetCellValue(string name)
    {
        ProcessCellName(ref name);

        // If the cell is nonempty, return its value, if not, return the empty string
        return nonemptyCells.TryGetValue(name, out Cell? cell) ? cell.Value : "";
    }

    /// <summary>
    /// Enumerates the names of every cell in the current
    /// Spreadsheet whose contents are something other than "".
    /// </summary>
    /// <returns>An IEnumerable of the nonempty cells</returns>
    public override IEnumerable<string> GetNamesOfAllNonemptyCells()
    {
        foreach (string cellName in nonemptyCells.Keys)
            yield return cellName;
    }

    /// <summary>
    /// Returns the contents of the cell identified by the given name.
    /// </summary>
    /// <param name="name">The name of the cell whose contents are being gotten</param>
    /// <returns>The contents of the named cell</returns>
    /// <exception cref="InvalidNameException">If the given name is an invalid cell name</exception>
    public override object GetCellContents(string name)
    {
        ProcessCellName(ref name);

        // If the cell is nonempty, return its contents, if not, return the empty string
        return nonemptyCells.TryGetValue(name, out Cell? cell) ? cell.Contents : "";
    }


    /// <summary>
    /// Sets the contents of the cell of the given name, to the given contents.
    /// Returns all cells whose values depend on the named cell.
    /// </summary>
    /// <param name="name">The name of the cell whose contents are being set</param>
    /// <param name="content"></param>
    /// <returns>An IList of the named cell's dependents</returns>
    /// <exception cref="InvalidNameException">If the given name is an invalid cell name</exception>
    public override IList<string> SetContentsOfCell(string name, string content)
    {
        ProcessCellName(ref name);

        // Setting the contents of any cell, regardless of what old and new contents are,
        // is regarded as a change to the spreadsheet
        Changed = true;

        // If content is a double, make cell's contents a double. Same for formulas. 
        // If neither, set content to the content string itself
        if (double.TryParse(content, out double doub))
            return SetCellContents(name, doub);
        else if (content.Length != 0 && content[0] == '=')
            return SetCellContents(name, new Formula(content[1..], Normalize, IsValid));
        else
            return SetCellContents(name, content);
    }

    /// <summary>
    /// Sets the contents of the cell of the given name, to the given number.
    /// Returns all cells whose values depend on the named cell.
    /// </summary>
    /// <param name="name">The name of the cell whose contents are being set</param>
    /// <param name="number">The number to set as the cell's contents</param>
    /// <returns>An IList of the named cell's dependents</returns
    protected override IList<string> SetCellContents(string name, double number)
    {
        // Try to get the cell of the given name out of the Dictionary
        // If the cell wasn't found, make a new one and add it to the Dictionary
        if (!nonemptyCells.TryGetValue(name, out Cell? cell))
        {
            cell = new Cell();
            nonemptyCells.Add(name, cell);
        }

        // Adjust its contents
        cell.Contents = number;

        // Changing the contents of this cell to a number should remove any previous dependees
        dependencies.ReplaceDependees(name, new List<string>());

        // Find all the cells whose values need recalculating, recalculate them, return the required list
        IEnumerable<string> toRecalc = GetCellsToRecalculate(name);
        foreach (string cellName in toRecalc)
            nonemptyCells[cellName].UpdateValue(Lookup);

        return toRecalc.ToList();
    }

    /// <summary>
    /// Sets the contents of the cell of the given name, to the given text.
    /// Returns all cells whose values depend on the named cell.
    /// </summary>
    /// <param name="name">The name of the cell whose contents are being gotten</param>
    /// <param name="text">The text to set as the cell's contents</param>
    /// <returns>An IList of the named cell's dependents</returns>
    protected override IList<string> SetCellContents(string name, string text)
    {
        // Try to get the cell of the given name out of the Dictionary
        // If the cell wasn't found, make a new one and add it to the Dictionary
        if (!nonemptyCells.TryGetValue(name, out Cell? cell))
        {
            cell = new Cell();
            nonemptyCells.Add(name, cell);
        }

        // Adjust its contents
        cell.Contents = text;

        // Changing the contents of this cell to text should remove any previous dependees
        dependencies.ReplaceDependees(name, new List<string>());

        // Find all the cells whose values need recalculating, recalculate them, return the required list
        IEnumerable<string> toRecalc = GetCellsToRecalculate(name);
        foreach (string cellName in toRecalc)
            nonemptyCells[cellName].UpdateValue(Lookup);

        // If setting the contents to empty, it's no longer a nonempty cell
        if (text == "")
            nonemptyCells.Remove(name);

        return toRecalc.ToList();
    }

    /// <summary>
    /// Sets the contents of the cell of the given name, to the given formula.
    /// Returns all cells whose values depend on the named cell.
    /// </summary>
    /// <param name="name">The name of the cell whose contents are being gotten</param>
    /// <param name="formula">The formula to set as the cell's contents</param>
    /// <returns>An IList of the named cell's dependents</returns>
    protected override IList<string> SetCellContents(string name, Formula formula)
    {
        // In case of a CircularException, the old values are needed
        object oldContents = "";
        bool oldState = Changed;
        IEnumerable<string> oldDependees = dependencies.GetDependees(name);

        // If the cell wasn't found, make a new one and add it
        if (!nonemptyCells.TryGetValue(name, out Cell? cell))
        {
            cell = new Cell();
            nonemptyCells.Add(name, cell);
        }
        else
        {
            oldContents = cell.Contents;
        }

        // Adjust its contents
        cell.Contents = formula;

        // Changing the contents of this cell to a formula affects its dependees. Update them
        dependencies.ReplaceDependees(name, formula.GetVariables());

        // There's the possibility that the following call to GetCellsToRecalculate will throw a CircularException.
        // If so, we want the spreadsheet to roll back this call to SetCellContents, since the contents
        // being set create a circular dependency, and we don't want the change to end up in the spreadsheet.
        // 
        // Reset the cell's contents and dependees to their old values, remove it from the Dictionary if contents are empty,
        // and rethrow the CircularException.
        try
        {
            // Find all the cells whose values need recalculating, recalculate them, return the required list
            IEnumerable<string> toRecalc = GetCellsToRecalculate(name);
            foreach (string cellName in toRecalc)
                nonemptyCells[cellName].UpdateValue(Lookup);

            return toRecalc.ToList();
        }
        catch(CircularException e)
        {
            cell.Contents = oldContents;
            dependencies.ReplaceDependees(name, oldDependees);

            if (cell.Contents.ToString() == "")
                nonemptyCells.Remove(name);

            Changed = oldState;

            throw e;
        }
    }

    /// <summary>
    /// Enumerates the names of every cell in this Spreadsheet whose
    /// values depend directly on the value of the cell of the given name.
    /// </summary>
    /// <param name="name">The name of the cell whose dependents are being gotten</param>
    /// <returns>An IEnumerable of the named cell's dependents</returns>
    protected override IEnumerable<string> GetDirectDependents(string name)
    {
        return dependencies.GetDependents(name);
    }


    /// <summary>
    /// Helper method for validating and normalizing the given name as a cell name.
    /// <br/>
    /// Throws an InvalidNameException if the given cell name is invalid.
    /// </summary>
    /// <param name="name">The cell name to be validated and normalized</param>
    /// <exception cref="InvalidNameException">If the given name is an invalid cell name</exception>
    private void ProcessCellName(ref string name)
    {
        // Normalize first- the normalizer may return an invalid cell name
        name = Normalize(name);

        // If the cell name doesn't both pass the baseline cellname test AND
        // the validator delegate test, throw an exception
        if (!(Regex.IsMatch(name, @"^[a-zA-Z]+[0-9]+$") && IsValid(name)))
            throw new InvalidNameException();
    }

    /// <summary>
    /// Method used as a delegate in evaluation of Formulas.<br/>
    /// Returns the value of the cell of the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private double Lookup(string name)
    {
        object value = GetCellValue(name);
        if (value.GetType() == typeof(double))
        {
            return (double)value;
        }
        else
        {
            throw new ArgumentException("Cell " + name + " has invalid contents.");
        }
    }


    // =======================================  N E S T E D   C L A S S E S  =======================================

    /// <summary>
    /// A cell object represents a single cell within a Spreadsheet object.
    /// <br/>
    /// Cells have fields for both their contents and their value, which may
    /// be the same or different depending on their type. For doubles and strings,
    /// the values will be equal to the contents, but for Formulas, the contents
    /// will be the formula (as a string) and the values will be either the double resulting
    /// from evaluating the formula, or a FormulaError object if the formula could
    /// not be properly evaluated.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    private class Cell
    {
        private object cellContents;

        /// <summary>
        /// Creates a blank Cell object, with its fields/properties all
        /// initialized to a blank string.
        /// </summary>
        public Cell()
        {
            cellContents = "";
            ContentsAsString = "";
            Value = "";
        }


        /// <summary>
        /// The contents of this Cell (not necessarily the value)
        /// </summary>
        public object Contents
        {
            // Whenever the contents of a cell are set/updated,
            // the stringform of the Cell's contents and its value should be too
            set
            {
                cellContents = value;
                ContentsAsString = MakeString();
            }

            get
            {
                return cellContents;
            }
        }

        /// <summary>
        /// The contents of this Cell, in string form<br/>
        /// For serialization/deserialization
        /// </summary>
        [JsonProperty(PropertyName = "stringForm")]
        public string ContentsAsString { private set; get; }

        /// <summary>
        /// The value of this Cell (not necessarily the contents)
        /// </summary>
        public object Value { private set; get; }


        /// <summary>
        /// Helper method for determining what a Cell's value should be, 
        /// given its contents. If the contents are a string or double,
        /// the value will likewise be a string or double; if the contents are a Formula,
        /// the value will be either the double or FormulaError returned from evaluating
        /// the Formula, using the given lookup delegate.
        /// </summary>
        /// <param name="contents">The contents to convert to a value</param>
        /// <param name="lookup">Delegate for looking up varaiables when evaluating Formulas</param>
        /// <returns></returns>
        public void UpdateValue(Func<string, double> lookup)
        {
            // Formulas need to be evaluated, but double and string contents
            // will be the values themselves
            if (cellContents.GetType() == typeof(Formula))
                Value = ((Formula)cellContents).Evaluate(lookup);
            else
                Value = cellContents;
        }

        /// <summary>
        /// Helper method for determining what the stringForm of a Cell's
        /// contents should be. Strings and doubles are returned without
        /// changes; Formulas are returned in their string form with a '=' prepended.
        /// </summary>
        /// <param name="contents">The contents to convert to a string form</param>
        /// <returns>The contents as a string</returns>
        private string MakeString()
        {
            // This method would be much simpler if the ToString method
            // of the object class just returned a non-nullable type.
            //
            // Seriously. Why. No one on the internet likes it either.

            if (cellContents.GetType() == typeof(Formula))
                return "=" + cellContents.ToString();
            else if (cellContents.GetType() == typeof(double))
                return ((double)cellContents).ToString();
            else
                return ((string)cellContents).ToString();
        }
    }
}