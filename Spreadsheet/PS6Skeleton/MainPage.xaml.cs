// Skeleton code provided by Daniel Kopta, Travis Martin
//
// Further implementation by Caden Erickson
// Last modified: 10/21/22

using SpreadsheetUtilities;
using SS;
using System.Text.RegularExpressions;

namespace SpreadsheetGUI;

/// <summary>
/// Code-behind for the MainPage of the SpreadsheetGUI project
/// </summary>
public partial class MainPage : ContentPage
{
    // backed by spreadsheet object
    private Spreadsheet backing;
    private string fileLoc;
    private bool savedBefore;


    /// <summary>
    /// Constructor for creating a new spreadsheet
    /// </summary>
	public MainPage()
    {
        InitializeComponent();

        backing = new(s => Regex.IsMatch(s, "^[A-Z][0-9]{1,2}$"), s => s.ToUpper(), "ps6");
        fileLoc = "spreadsheet1";
        savedBefore = false;

        // Register appropriate event handlers
        spreadsheetGrid.SelectionChanged += ReloadEditor;
        spreadsheetGrid.SelectionChanged += DisableButton;

        spreadsheetGrid.SetSelection(0, 0);
    }


    // ==============================================  System event handlers  ==============================================

    // File menu

    /// <summary>
    /// Event handler for when the File>Save As menu item is clicked.
    /// If there is unsaved data, prompts the user if they want to proceed.
    /// If so, clears the spreadsheet and starts with a fresh one.
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private async void NewClicked(object sender, EventArgs e)
    {
        if (backing.Changed)
            if (!await DisplayAlert("Unsaved Data", "Your current spreadsheet has not been saved. Continue anyway?", "Yes", "No"))
                return;

        backing = new(s => Regex.IsMatch(s, "^[A-Z][0-9]{1,2}$"), s => s.ToUpper(), "ps6");

        FileNameDisplay.Text = "spreadsheet1.sprd";
        spreadsheetGrid.Clear();
        DisplayCells(backing.GetNamesOfAllNonemptyCells());
        ReloadEditor();
        savedBefore = true;
        spreadsheetGrid.SetSelection(0, 0);
    }

    /// <summary>
    /// Event handler for when the File>Save As menu item is clicked.
    /// Loads a spreadsheet file selected by the user and displays its contents.
    /// Only .sprd files are allowed to be chosen.
    /// </summary>
    private async void OpenClicked(object sender, EventArgs e)
    {
        // If the current spreadsheet hasn't been saved, alert the user
        // If they choose not to continue, don't continue with the open
        if (backing.Changed)
            if (!await DisplayAlert("Unsaved Data", "Your current spreadsheet has not been saved. Continue anyway?", "Yes", "No"))
                return;

        try
        {
            // This is what restricts the allowed filetypes to only files ending in ".sprd"
            PickOptions ops = new()
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".sprd" } }
                })
            };
            FileResult fileResult = await FilePicker.Default.PickAsync(ops);

            if (fileResult != null)
            {
                // Update filename and filename display
                fileLoc = fileResult.FullPath;
                FileNameDisplay.Text = fileResult.FileName;

                // New backing object
                backing = new(fileLoc, s => Regex.IsMatch(s, "^[A-Z][0-9]{1,2}$"), s => s.ToUpper(), "ps6");

                // Clear spreadsheet, draw nonempty cells, adjust editor, selection and save state
                spreadsheetGrid.Clear();
                DisplayCells(backing.GetNamesOfAllNonemptyCells());
                ReloadEditor();
                savedBefore = true;
                spreadsheetGrid.SetSelection(0, 0);
            }
            else
            {
                await DisplayAlert("No file selected.", "Please select a file", "Continue");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error opening file:", ex.ToString(), "OK");
        }
    }

    /// <summary>
    /// Event handler for when the File>Save As menu item is clicked.
    /// Saves the current spreadsheet.
    /// If the spreadsheet has been saved previously, saves it to its previous location.
    /// Otherwise, prompts the user for a destination to which to save.
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private async void SaveClicked(object sender, EventArgs e)
    {
        try
        {
            // A dialogue has to pop up if the current file hasn't been saved before
            if (!savedBefore)
                SaveToLocation();
            else
                backing.Save(fileLoc);
        }
        catch (Exception ex)
        {
            await DisplayAlert("An error occurred.", ex.Message, "OK");
        }

        // Remove the "*" from the filename display (if one was there)
        // to indicate that there are no unsaved changes
        if (FileNameDisplay.Text[^1] == '*')
            FileNameDisplay.Text = FileNameDisplay.Text[..^1];
    }

    /// <summary>
    /// Event handler for when the File>Save As menu item is clicked.
    /// Saves a copy of the current spreadsheet to a location specified by the user, and with 
    /// the filename specified by the current user.
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private async void SaveAsClicked(object sender, EventArgs e)
    {
        try
        { 
            SaveToLocation();
        }
        catch (Exception ex)
        {
            await DisplayAlert("An error occurred.", ex.Message, "OK");
        }
    }


    // View menu

    /// <summary>
    /// Event handler for when the View>Header Themes>Light menu item is clicked
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private void LightClicked(object sender, EventArgs e)
    {
        Application.Current.UserAppTheme = AppTheme.Light;
    }

    /// <summary>
    /// Event handler for when the View>Header Themes>Dark menu item is clicked
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private void DarkClicked(object sender, EventArgs e)
    {
        Application.Current.UserAppTheme = AppTheme.Dark;
    }

    
    // Help menu

    /// <summary>
    /// Event handler for when the Help>About Program menu item is clicked.
    /// Displays a page with information about the spreadsheet program.
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private void AboutClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new HelpPage());
    }


    // Editor panel

    /// <summary>
    /// Event handler for when an edit of the entry field's text is confirmed.
    /// Handles the confirm button's Clicked event as well as the entry field's Completed event.
    /// Updates the backing spreadsheet object and updates the display accordingly.
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private async void EditConfirmed(object sender, EventArgs e)
    {
        // Get the name of the selected cell, in letter+number form
        spreadsheetGrid.GetSelection(out int col, out int row);
        string cellName = String.Concat((char)(col + 65), row + 1);

        string newContents = EditorContents.Text;

        try
        {
            DisplayCells(backing.SetContentsOfCell(cellName, newContents));
        }
        catch (Exception ex)
        {
            await DisplayAlert("An error occurred.", ex.Message, "OK");
            return;
        }

        // fter confirming a change, display a "*" next the file name to indicate that modifications have been made since the last save
        if (FileNameDisplay.Text[^1] != '*')
            FileNameDisplay.Text += "*";

        // Move the selection one cell down (if the selection isn't at the bottom)
        if (row != 98)
            spreadsheetGrid.SetSelection(col, row + 1);
        ReloadEditor();

        // If the new cell is empty, the confirm button shouldn't be active
        if (EditorContents.Text == "")
            DisableButton();
    }

    /// <summary>
    /// Event handler for when the editor entry field's text changes.
    /// Enables or disables the confirm button based on the field's contents.
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private void EditorTextChanged(object sender, EventArgs e)
    {
        spreadsheetGrid.GetSelection(out int col, out int row);
        spreadsheetGrid.GetValue(col, row, out string oldContents);

        // Disable the button if, during this content entry,
        // the contents of the entry field started as blank and are now blank again
        if (EditorContents.Text == "" && EditorContents.Text == oldContents)
            DisableButton();
        else
            EnableButton();
    }

    /// <summary>
    /// Event handler for when the editor entry field gains focus.
    /// Enables or disables the confirm button based on the field's contents.
    /// </summary>
    /// <param name="sender">unused</param>
    /// <param name="e">unused</param>
    private void EditorFocused(object sender, EventArgs e)
    {
        // Only activate the button if the selected cell is nonempty
        if (EditorContents.Text != "")
            EnableButton();
    }


    // ==============================================  SelectionChanged event handlers  ==============================================


    /// <summary>
    /// Refreshes the contents of the editor label, entry field, and value display.
    /// </summary>
    private void ReloadEditor()
    {
        // Get the name of the selected cell, in letter+number form
        spreadsheetGrid.GetSelection(out int col, out int row);
        string cellName = string.Concat((char)(col + 65), row + 1);

        // Update cell name label
        EditorLabel.Text = cellName;

        // Update entry field contents
        object contents = backing.GetCellContents(cellName);
        if (contents is Formula)
            EditorContents.Text = "= " + contents.ToString();
        else
            EditorContents.Text = contents.ToString();

        // Update value field contents
        spreadsheetGrid.GetValue(col, row, out string value);
        EditorValue.Text = value;
    }


    // ==============================================  Helper methods  ==============================================

    /// <summary>
    /// Draws the values of all the nonempty cells into the spreadsheet
    /// </summary>
    private void DisplayCells(IEnumerable<string> toFill)
    {
        foreach (string cell in toFill)
        {
            // Split the cell name to get the col and row in numerical form
            string[] split = Regex.Split(cell, "(?<=[A-Z])");
            int col = split[0][0] - 65;
            int row = int.Parse(split[1]) - 1;

            object newVal = backing.GetCellValue(cell);

            // Display FormulaErrors properly, and round floating point numbers to 5 decimal places
            if (newVal is FormulaError)
                spreadsheetGrid.SetValue(col, row, "#VALUE!");
            else if (newVal is double @doubleVal)
                spreadsheetGrid.SetValue(col, row, (Math.Round(@doubleVal, 5)).ToString());
            else
                spreadsheetGrid.SetValue(col, row, newVal.ToString());
        }
    }

    /// <summary>
    /// Saves the current spreadsheet, to a location prompted from the user.
    /// </summary>
    private async void SaveToLocation()
    {
        // Get the file location and desired filename from the user
        string filepath = await DisplayPromptAsync("File Location", "Path of folder in which to save this file:");
        if (filepath == null) return;
        string filename = await DisplayPromptAsync("File Name", "Name under which to save this file:", initialValue: ".sprd");
        if (filename == null) return;

        // Add the file extension if they didn't provide it
        if (!filename.EndsWith(".sprd"))
            filename += ".sprd";

        // If a file already exists of that location + name combination, ask about overwriting
        string newFileLoc = filepath + "\\" + filename;
        if (File.Exists(newFileLoc))
            if (!await DisplayAlert("File Overwrite", "The file " + filename + " already exists. Do you want to replace it?", "Yes", "No"))
                return;

        // Update data & displays, save backing object
        fileLoc = filepath + "\\" + filename;
        savedBefore = true;
        FileNameDisplay.Text = filename;


        try
        {
            backing.Save(fileLoc);
        }
        catch (Exception ex)
        {
            await DisplayAlert("An error occurred", ex.ToString(), "OK");
        }
    }

    /// <summary>
    /// Enables (and un-grays out) the checkmark button
    /// </summary>
    private void EnableButton()
    {
        ConfirmButton.TextColor = Colors.White;
        ConfirmButton.IsEnabled = true;
    }

    /// <summary>
    /// Disables (and grays out) the checkmark button
    /// </summary>
    private void DisableButton()
    {
        ConfirmButton.TextColor = new Color(211, 211, 211);
        ConfirmButton.IsEnabled = false;
    }
}
