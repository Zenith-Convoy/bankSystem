using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OutlawHess
{
    public class datagridFiller
    {
        DataGrid datagrid;
        public datagridFiller(DataGrid d)
        {
            datagrid = d;            
        }
        /// <summary>
        /// Fills data grid with content from database.
        /// Requires: SQL command as string.
        /// Optional: @bindingName as string for parameter pass.
        /// Optional: Text box containing value for parameter passing.
        /// Optionals are lists so they are unlimited.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="atBinding"></param>
        /// <param name="control"></param>
        public void fillDataGrid(string sql, List<string> atBinding = null, List<TextBox> control = null)
        {
            //Establish connection to database
            OleDbConnection myConn = new OleDbConnection(dbConnection.dbConnect);
            myConn.Open();

            //create query
            OleDbCommand myCmd = myConn.CreateCommand();

            //create data reader
            OleDbDataReader myDR = null;

            //set passed SQL string to query
            myCmd.CommandText = sql;

            /*Perform parameter passing to prevent SQL injection*/
            //validation: don't run if no parameters passed
            if (atBinding != null && control != null)
            {
                //combine lists
                var binder = atBinding.Zip(control, (a, c) => new { binding = a, theControl = c });
                //add parameter for each item in list
                foreach (var bind in binder)
                {
                    myCmd.Parameters.AddWithValue(bind.binding
                    , bind.theControl.Text ?? "null");
                }
            }
            //clear datagrid
            datagrid?.Items?.Clear();

            //try statement to handle errors
            try
            {
                
                //run query
                myDR = myCmd.ExecuteReader();
                //function to convert data reader output to currency format.
                Func<object, string> toCurrency = (object x) => double.Parse(x.ToString()).ToString("c");
                //read result from query, for each record read add record to database
                while (myDR.Read())
                {
                    //add record to data grid
                    //switch case to handle different entities
                    switch (datagrid.Name)
                    {
                        case "dgCustomers":
                            //add data item
                            datagrid?.Items?.Add(new dataItem
                            {
                                //add data item properties
                                id = myDR["custID"].ToString(),
                                title = myDR["title"].ToString(),
                                fName = myDR["firstName"].ToString(),
                                lName = myDR["lastName"].ToString(),
                                //format date
                                DoB = DateTime.Parse(myDR["DoB"].ToString()).ToString("dd/MM/yyyy"),
                                natins = myDR["natins"].ToString(),
                                email = myDR["email"].ToString(),
                                allowence = toCurrency(myDR["allowence"])
                            });
                            break;
                        case "dgCustAccounts":
                            datagrid?.Items?.Add(new dataItem
                            {
                                accID = myDR["accID"].ToString(),
                                custID = myDR[1].ToString(),
                                prodID = myDR[2].ToString(),
                                //format to currency
                                balance = toCurrency(myDR[3]),
                                accrued = toCurrency(myDR[4]),
                                active = myDR[5].ToString(),
                            });
                            break;
                        case "dgCustTransactions":
                            datagrid?.Items?.Add(new dataItem
                            {
                                tranxID = myDR[0].ToString(),
                                accID = myDR[1].ToString(),
                                action = myDR[2].ToString(),
                                amnt = toCurrency(myDR[3]),
                                theEvent = myDR[4].ToString(),
                            });
                            break;
                        case "dgProducts":
                            datagrid?.Items?.Add(new dataItem
                            {
                                prodID = myDR[0].ToString(),
                                prodName = myDR[1].ToString(),
                                status = myDR[2].ToString(),
                                transin = myDR[3].ToString(),
                                intRate = myDR[4].ToString(),
                                //total invested
                                amnt = GetEntityAmntTotal(myDR[0].ToString())
                            });
                            break;
                    }

                }
            }
            catch (Exception e)
            {
                //display unhelpful obtuse system generation error message
                MessageBox.Show(e.ToString());
            }
            //close connections (prevents exception errors!)
            myConn.Close();
            myDR.Close();
        }
        /// <summary>
        /// Sums values in account where product id matches passed value.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Total invested in ISA product</returns>
        private string GetEntityAmntTotal(string id)
        {
            //Establish connection to database
            OleDbConnection myConn = new OleDbConnection(dbConnection.dbConnect);
            myConn.Open();
            //create data reader
            OleDbDataReader myDR = null;
            //create query
            OleDbCommand myCmd = myConn.CreateCommand();
            //set passed SQL string to query
            myCmd.CommandText = @"SELECT Sum(balance) AS Total FROM account WHERE prodID = @id;";
            myCmd.Parameters.AddWithValue("id", id);
            //try statement to handle errors
            try
            {
                //run query
                myDR = myCmd.ExecuteReader();
                //return sum. Return 0 if empty string. Converts to currency format.
                while (myDR.Read()) return Double.Parse(myDR[0]?.ToString() != "" ? myDR[0]?.ToString() : "0").ToString("c");

            }
            catch(Exception e) {
                MessageBox.Show(e.ToString());               
            }
            return "Error";
        }
        /// <summary>
        /// Add customer to database.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="atBinding"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public Boolean WriteCust(string sql, List<string> atBinding = null, List<object> control = null)
        {
            //Establish connection to database
            OleDbConnection myConn = new OleDbConnection(dbConnection.dbConnect);
            myConn.Open();

            //create query
            OleDbCommand myCmd = myConn.CreateCommand();
            //set passed SQL string to query
            myCmd.CommandText = sql;
            /*Perform parameter passing to prevent SQL injection*/
            //validation: don't run if no parameters passed
            if (atBinding != null && control != null)
            {
                //combine lists
                var binder = atBinding.Zip(control, (a, c) => new { binding = a, theControl = c });
                //add parameter for each item in list
                foreach (var bind in binder)
                {
                    //If statement for different property names
                    if (bind.theControl is TextBox)
                    {
                        //create local object
                        TextBox textBox = (TextBox)bind.theControl;
                        //add parameter
                        myCmd.Parameters.AddWithValue(bind.binding
                        , textBox.Text ?? "null");

                    }
                    else if (bind.theControl is Label)
                    {
                        //create local object
                        Label label = (Label)bind.theControl;
                        //add parameter
                        myCmd.Parameters.AddWithValue(bind.binding
                        , label.Content ?? 1);
                    }
                }
            }
            try
            {
                myCmd.ExecuteReader();
                //close connections (prevents exception errors!)
                myConn.Close();
                //success
                return true;
            }
            catch (Exception e)
            {
                //close connections (prevents exception errors!)
                myConn.Close();
                //error message pop up
                MessageBox.Show(e.ToString());
                //failed to add customer
                return false;
            }
            
        }

    }
}
