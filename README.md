# CS 3500 Spreadsheet Project
Created by Caden Erickson  
Software Practice I  
University of Utah  
Fall 2022  

---

## Introduction
I built this program as a cumulative assignment for my CS 3500- Software Practice I class, during the fall semester of 2022.
It is a very simple spreadsheet program, and lacks many of the powerful functions of other spreadsheet programs. It still has basic functionality, however, and can evaluate basic formulas. 
  
This program was my first experience with C#, all my prior college coding experience being in Java. For the GUI, I used Microsoft's (currently) brand new .NET MAUI framework. Since MAUI is so new, there were a lot of stumbling blocks in creating the GUI. Hopefully as MAUI developed further, I will be able to improve on what I've already created in this program.
  
---
  
## Program Layout
The layout is divided into three sections: A menu bar, an editor section, and the spreadsheet grid.  

**Menu Section**  
In the menu section you will see a File tab, a View tab, and a Help tab.  
          
*File*                
Under the File tab, there are options for New, Open, Save, and Save As.  
* New works as you'd expect. It clears the spreadsheet (the program will warn you if you have unsaved data), and creates a new blank one.  
* Open should also be familiar. Note that the program only allows files with the ".sprd" file extension to be opened.  Files are read using Newtonsoft JSON deserialization.  
* Save and Save As may be slightly unfamiliar. Save works as usual if you've previously saved your spreadsheet. If you haven't, or if you've chosen Save As, the program will prompt you for a folder location and a filename. Enter the COMPLETE folder location where you'd like to save your file. For the filanme, all files will be saved as .sprd files, so even if you enter something like "mySheet.xlsx", it will be saves as "mySheet.xlsx.sprd". Files are saved using Newtonsoft JSON serialization.  
  
*View*  
The View tab has options for changing the editor theme. Note that these themes apply to the editor section only, and do not change the menu bar or the spreadsheet grid.  

*Help*  
The Help tab displays a page with information about the program and how to use it. To return to the original spreadsheet interface, click the back arrow in the top left corner.  
  
  
**Editor Section**  
There are four notable elements in the editor section: a label, an entry field, a checkmark button, and a value field.  

The label shows the name of the currently selected grid cell.  
The entry field shows the contents (not necessarily the evaluated value) of the currently selected grid cell. The entry field is the only user-editable element in the editor section.  
The checkmark button is used as an option for confirming an entry typed in the entry field (more later). Note that if the checkmark button is gray, then the contents of the entry box aren't confirmable (usually meaning the contents are empty/blank).  
The value field shows the evaluated value (not necessarily the contents) of the currently selected grid cell.  
  
  
**Grid Section**  
The grid section only contains 26 columns (A-Z) and 99 rows. No more can be added, nor can any be removed.  
Only one cell can be selected at a time.  


## Using The Program
Only one cell can be selected at a time. To edit the contents of a cell, you must select the cell on the grid AND then click inside the entry field in the editor section.  
Entries can fall into one of three categories: text, a number or a formula.  
* For numbers: decimals, negatives, and scientific notation are all allowed  
* For formulas: formulas must follow a very specific format:  
      - They MUST start with '=' to be recognized as a formula.  
      - This program does not support unary negatives (-1, -2, etc). Instead, use 0-1, 0-2, etc.  
      - This program does not support any functions (SUM, AVG, etc).  
      - The only permitted operators are + - * and / along with parentheses.  
      - This program does not support implicit multiplication: 2(5 + 6), 3(B1), etc. You must provide a '*' operator.  
      
The program will tell you if you make any mistakes in entering a formula.  
Refer to other cells by letter+number (A1, N17, etc). No underscores, leading 0s, etc.  
If a reference is erroneous (causes a circular dependency, depends on a string, etc.) the program will alert you, either by displaying an alert or by displaying "#VALUE!" in the selected cell.  
                
To confirm an entry, you may press the *ENTER* key or press the checkmark button. When your edit is confirmed, the cell selection will move one cell downwards.  

## The Extra Mile
I added several small features beyond what was required for the assignment:
1. When a cell is selected, the corresponding row and column labels are highlighted.
2. Light and dark themes for the editor section are available in the View menu.
3. To save a separate copy of an already-saved spreadsheet, a Save As option is included in the File menu.
4. The name of the current spreadsheet file is displayed at the top of the window, along with a '*' if unsaved changes have been made.

These involved some foraging into files and topics not covered in class. To accomplish the above features, I added a new Page (HelpPage.xaml and HelpPage.xaml.cs), edited both Colors.xaml and Styles.xaml, and edited the provided class SpreadsheetGrid.cs. I also explored FilePickerOptions in order restrict the files shown in my Open File dialogue.

## Potential Issues
Please note, several issues still exist in the program:
* When any cell in row 99 is selected, the selection box does not appear
* When any cell in column Z is selected, all the row dividing lines disappear

The above two issues come from code that was provided to me for the assignment, so I didn't worry about fixing them before submitting it.
If you find other issues, please let me know!
  
---
  
## Credits
Skeleton code for assignments was provided by Joe Zachary, Daniel Kopta, and Travis Martin (instructors)

## Version History
10/21/2022 - Version submitted for PS6 assignment
