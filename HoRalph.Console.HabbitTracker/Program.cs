﻿using System;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace habit_tracker
{
class Program
{
    public static string connectionString = @"Data Source = habit-Tracker.db";

    static void Main(string[] args)
    {
        string?result="";
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS HABIT (
                                    HabitID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Date TEXT,
                                    Habit TEXT,
                                    Units TEXT,
                                    Quantity INTEGER)";

            tableCmd.ExecuteNonQuery();
            connection.Close();
        }
        bool exitApp = false;
        while(!exitApp)
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("-----------------MAIN MENU-----------------");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine(@"
            Type 0 to Close Application.
            Type 1 to View All Records.
            Type 2 to Insert Record.
            Type 3 to Delete Record.
            Type 4 to Update Record.
            Type 5 to Delete Table.
            ");
            Console.WriteLine("------------------------------------------");
            result = Console.ReadLine();
            while(!int.TryParse(result,out int choice))
            {
                Console.WriteLine("Invalid Input! Please try again.");
                result = Console.ReadLine();
            }
            Console.Clear();
            switch (result)
            {
                case "0":
                return;

                case "1": //view all records
                ViewAll();
                Console.WriteLine("\n\n Press any key to return to the menu.");
                Console.ReadLine();
                break;

                case "2": //insert Record
                InsertRecord();
                break;

                case "3": //Delete Record
                ViewAll();
                Console.WriteLine();
                DeleteRecordID();
                //DeleteRecord();
                break;

                case "4": //Update Record
                ViewAll();
                Console.WriteLine();
                UpdateRecordID();
                //UpdateRecord();
                break;
                
                case "5": //drop table
                DeleteTable();
                break;

                default:
                break;
            }
        }
    }
    static void ViewAll()
    {
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = @"SELECT * FROM HABIT;";
            SqliteDataReader reader = tableCmd.ExecuteReader();
            if (reader.HasRows)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write(reader.GetName(i).PadRight(15));
                    Console.Write("|");
                }
                Console.WriteLine();
                while(reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader.GetString(i).PadRight(15));
                        Console.Write("|");
                    }
                    Console.WriteLine();
                }
            }
            else
                {
                    Console.WriteLine("Empty  table.");
                }
            reader.Close();
            connection.Close();

        }
    }
    static void InsertRecord()
    {
        bool validDate = false;
        string?date="";
        string[] dateFormats = {"MM/dd/yyyy"};
        CultureInfo enUS = new CultureInfo("en-US");
        while (!validDate)
        {
            Console.WriteLine("Enter a date (MM/DD/YYYY). (Enter blank to default to Today' date)");
            date = Console.ReadLine();
            if(DateTime.TryParseExact(date, dateFormats, enUS, DateTimeStyles.None,out DateTime value))
            {
                validDate = true;
            }
            else if (date == null || date == "")
            {
                date = DateTime.Today.ToString("d");
                validDate = true;
            }
            else
            {
                Console.WriteLine("Invalid Date. Please re-enter");
            }
        }
        Console.WriteLine("Enter a habit.");
        string?habit = Console.ReadLine();
        Console.WriteLine("Enter the unit.");
        string?units = Console.ReadLine();
        Console.WriteLine("Enter the quantity.");
        string?quantity = Console.ReadLine();
        while (!int.TryParse(quantity,out int result))
        {
            Console.WriteLine("Invalid Number");
            quantity = Console.ReadLine();
        }
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = @$"INSERT INTO HABIT (Date, Habit, Units, Quantity)
                                    VALUES ('{date}', '{habit}', '{units}', {quantity});";
            tableCmd.ExecuteNonQuery();
            connection.Close();
        }
    }
    static void DeleteTable()
    {
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = @"DELETE FROM HABIT;";
            tableCmd.ExecuteNonQuery();
            connection.Close();
        }
    }
    static void DeleteRecord()
    {
        string dateDelete = null;
        bool validDate = false;
        string[] dateFormats = {"MM/dd/yyyy"};
        CultureInfo enUS = new CultureInfo("en-US");
        while (!validDate)
        {
            Console.WriteLine("Enter a date (MM/DD/YYYY).");
            dateDelete = Console.ReadLine();

            if(DateTime.TryParseExact(dateDelete, dateFormats,enUS, DateTimeStyles.None ,out DateTime value))
            {
                validDate = true;
            }
            else
            {
                Console.WriteLine("Invalid Date. Please re-enter");
            }   
        }       
        Console.WriteLine("Enter Habit. (leave blank if delete all records with specified date)");
        string?habitDelete =Console.ReadLine();
        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            if ((habitDelete == "") || (habitDelete == null))
            {
                tableCmd.CommandText = @$"DELETE FROM HABIT
                                         WHERE date = '{dateDelete}';";
            }
            else
            {
                tableCmd.CommandText = @$"DELETE FROM HABIT
                WHERE date = '{dateDelete}' AND habit = '{habitDelete}';";
            }
            tableCmd.ExecuteNonQuery();
            connection.Close();
        }
    }
        static void DeleteRecordID()
        {
            string IDDelete = null;
            bool validDate = false;
            string[] dateFormats = {"MM/dd/yyyy"};
            CultureInfo enUS = new CultureInfo("en-US");
            Console.WriteLine("Enter Habit ID to Delete. Enter Blank if you want to cancel delete.");
            IDDelete = Console.ReadLine();
            using(var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                if ((IDDelete == "") || (IDDelete == null))
                {
                    Console.WriteLine("Cancel Delete request.");
                }
                else
                {
                    tableCmd.CommandText = @$"DELETE FROM HABIT
                    WHERE habitID = '{IDDelete}';";
                }
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
 static void UpdateRecord()
    {
        string?dateUpdate = null;
        bool validDate = false;   
        string[] dateFormats = {"MM/dd/yyyy"};
        CultureInfo enUS = new CultureInfo("en-US");   
        while (!validDate)
        {
            Console.WriteLine("Enter a date (MM/DD/YYYY).");
            dateUpdate = Console.ReadLine();

            if(DateTime.TryParseExact(dateUpdate, dateFormats, enUS, DateTimeStyles.None,out DateTime value))
            {
                validDate = true;
            }
            else
            {
                Console.WriteLine("Invalid Date. Please re-enter");
            }   
        }     
        Console.WriteLine("Enter Habit. (leave blank if update all records with specified date)");
        string?habitUpdate =Console.ReadLine();
        Console.WriteLine("Enter the new unit. (leave blank if don't update unit)");
        string?unitUpdate = Console.ReadLine();
        Console.WriteLine("Enter the new Quantity. (leave blank if don't update quantity)");
        string?QuantityUpdate = Console.ReadLine();
        string?setOne=null;

        if (((unitUpdate != null)|| (unitUpdate !="")) && ((QuantityUpdate != null)|| (QuantityUpdate !="")))
        {
            setOne = $"SET Units = '{unitUpdate}', Quantity = {QuantityUpdate}";
        }
        else if ((unitUpdate == null)|| (unitUpdate ==""))
        {
            setOne = $"SET Quantity = {QuantityUpdate}";
        }
        else if ((QuantityUpdate == null) || (QuantityUpdate ==""))
        {
            setOne = $"SET UNITS = {unitUpdate}";
        }

        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            if ((habitUpdate == "") || (habitUpdate == null))
            {
                tableCmd.CommandText = @$"UPDATE HABIT
                                         {setOne}
                                         WHERE date = '{dateUpdate}';";
            }
            else
            {
                tableCmd.CommandText = @$"UPDATE HABIT
                                        {setOne}                
                                        WHERE date = '{dateUpdate}' AND habit = '{habitUpdate}';";
            }
            tableCmd.ExecuteNonQuery();
            connection.Close();
        }
    }
    static void UpdateRecordID()
    {
        string?IDUpdate = null;
        bool validDate = false;   
        string[] dateFormats = {"MM/dd/yyyy"};
        CultureInfo enUS = new CultureInfo("en-US");   
  
            Console.WriteLine("Enter a Habit ID to update. Leave blank if you want to cancel update.");
            IDUpdate = Console.ReadLine();

 
        
        Console.WriteLine("Enter the new unit. (leave blank if don't update unit)");
        string?unitUpdate = Console.ReadLine();
        Console.WriteLine("Enter the new Quantity. (leave blank if don't update quantity)");
        string?QuantityUpdate = Console.ReadLine();
        string?setOne=null;

        if (((unitUpdate != null)|| (unitUpdate !="")) && ((QuantityUpdate != null)|| (QuantityUpdate !="")))
        {
            setOne = $"SET Units = '{unitUpdate}', Quantity = {QuantityUpdate}";
        }
        else if ((unitUpdate == null)|| (unitUpdate ==""))
        {
            setOne = $"SET Quantity = {QuantityUpdate}";
        }
        else if ((QuantityUpdate == null) || (QuantityUpdate ==""))
        {
            setOne = $"SET UNITS = {unitUpdate}";
        }

        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            if ((IDUpdate == "") || (IDUpdate == null))
            {
                Console.WriteLine("No records updated");
            }
            else
            {
                tableCmd.CommandText = @$"UPDATE HABIT
                                        {setOne}                
                                        WHERE HabitID = '{IDUpdate}';";
            }
            tableCmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}
}



