using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OutlawHess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        datagridFiller filler, fillerProd;
        public MainWindow()
        {
            InitializeComponent();
            //add columns to datagrids
            Column.AddColumn("ID", "id", dgCustomers);
            Column.AddColumn("Title", "title", dgCustomers);
            Column.AddColumn("First Name", "fName", dgCustomers);
            Column.AddColumn("Last Name", "lName", dgCustomers);
            Column.AddColumn("Date of Birth", "DoB", dgCustomers);
            Column.AddColumn("National Insurance No#", "natins", dgCustomers);
            Column.AddColumn("email", "email", dgCustomers);
            Column.AddColumn("allowence", "allowence", dgCustomers);

            Column.AddColumn("ID", "prodID", dgProducts);
            Column.AddColumn("Title", "prodName", dgProducts);
            Column.AddColumn("Status", "status", dgProducts);
            Column.AddColumn("transin", "transin", dgProducts);
            Column.AddColumn("Interest Rate", "intRate", dgProducts);
            Column.AddColumn("Total Invested", "amnt", dgProducts);

            //populate data grids
            filler = new datagridFiller(dgCustomers);            
            fillerProd = new datagridFiller(dgProducts);
            fillDataGrids();

        }
        /// <summary>
        /// Fill data grids
        /// </summary>
        public void fillDataGrids()
        {
            filler.fillDataGrid("select * from customer;");
            fillerProd.fillDataGrid("select * from product;");
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if search box is empty or has default value then show all customers
            if (txtSearchCustomers.Text == "" || txtSearchCustomers.Text == "Search")
            {
                filler?.fillDataGrid("select * from customer;");
                return;
            }
            else
            {
                /* Fill data grid with records where fname or fname match searchbox contents.
                 * Includes parameter passing to prevent SQL injection
                */
                filler?.fillDataGrid(@"select * from customer where"
                + @" [firstName] like @goGetFirstName or "
                + @"[lastName] = @goGetLastName;", 
                new List<string> { "goGetFirstName", "goGetLastName" }, 
                new List<TextBox> { txtSearchCustomers, txtSearchCustomers });
            }           
        }
        /// <summary>
        /// Open customer account details when selection change on customer data grid/table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {      
            //validation
            if((dataItem)dgCustomers.SelectedItem != null)
            {
                //create new object of customerAccount class
                customerAccount customerAccount = new customerAccount(this, (dataItem)dgCustomers.SelectedItem);
                //open customer details
                customerAccount.Show();
                //hide main window (this)
                this.Hide();
            }           
            
        }
        /// <summary>
        /// Filter data grid contents by search box contents on first and last name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearchProducts_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if search box is empty or has default value then show all customers
            if (txtSearchProducts.Text == "" || txtSearchProducts.Text == "Search")
            {
                fillerProd?.fillDataGrid("select * from product;");
                return;
            }
            else
            {
                /* Fill data grid with records where fname or fname match searchbox contents.
                 * Includes parameter passing to prevent SQL injection
                */
                fillerProd?.fillDataGrid(@"select * from product where"
                + @" [theName] like @goGetName;",
                new List<string> { "goGetName" },
                new List<TextBox> { txtSearchProducts });
            }
        }
        /// <summary>
        /// Add new customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            //validation check
            if (validate() == false) return;
            //add customer to data base, if this fails then don't clear text boxes — return/exit method.
            if (!filler.WriteCust(@"INSERT INTO customer ("
                + @" title, [firstName], [LastName], DoB, natins, email, allowence) VALUES ( "
                + @"@goGetTitle, @goGetFName, goGetLName, @goGetDoB, @goGetNat, @goGetEmail, "
                + @" @goGetAllowence);",
                new List<string> { "goGetTitle", "goGetFName", "goGetLName", "goGetDoB",
                    "goGetNat", "goGetEmail", "goGetAllowence", },
                new List<object> { txtCustTitle, txtCustFName, txtCustLName, txtCustDoB,
                    txtCustNat, txtCustEmail, txtCustAllowence})) return;
            //clear text boxes
            txtCustTitle.Text = "";
            txtCustFName.Text = "";
            txtCustLName.Text = "";
            txtCustDoB.Text = "";
            txtCustEmail.Text = "";
            txtCustNat.Text = "";
            txtCustAllowence.Text = "";
            filler?.fillDataGrid("select * from customer;");
        }

        private void dgCustomers_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //prevent exception error when user clicks cell in datagrid
            e.Cancel = true;
        }
        
        /// <summary>
        /// Validates the contents of the customer form
        /// returns false if validation has failed
        /// </summary>
        /// <returns></returns>
        private Boolean validate()
        {
            //Null validation
            Boolean isNull = false;
            if(string.IsNullOrWhiteSpace(txtCustTitle.Text))
            {
                //validation failed
                isNull = true;
                //mark failed text box
                rectCustTitle.Stroke =
                    Brushes.OrangeRed;
            }
            if (string.IsNullOrWhiteSpace(txtCustFName.Text))
            {
                isNull = true;
                rectCustFirstName.Stroke = 
                    Brushes.OrangeRed;
            }
            if (string.IsNullOrWhiteSpace(txtCustLName.Text))
            {
                isNull = true;
                rectCustLastName.Stroke = 
                    Brushes.OrangeRed;
            }
            DateTime dt;
            if (string.IsNullOrWhiteSpace(txtCustDoB.Text) || 
                DateTime.TryParse(txtCustDoB.Text, out dt) != true)
            {
                isNull = true;
                rectCustDoB.Stroke = 
                    Brushes.OrangeRed;
            }
            if (string.IsNullOrWhiteSpace(txtCustNat.Text))
            {
                isNull = true;
                rectCustNat.Stroke =
                    Brushes.OrangeRed;
            }
            //validate null and is valid email address
            if (string.IsNullOrWhiteSpace(txtCustEmail.Text) || isEmail(txtCustEmail.Text) == false)
            {
                isNull = true;
                rectCustEmail.Stroke = 
                    Brushes.OrangeRed;
            }
            //validate null and is double
            if (string.IsNullOrWhiteSpace(txtCustAllowence.Text) || 
                Double.TryParse(txtCustAllowence.Text, out Double x) != true)
            {
                isNull = true;
                rectCustAllowence.Stroke = 
                    Brushes.OrangeRed;
            }
            //return false if validation fail
            if (isNull) return false;
            clearErrorWarning();
            return true;
        }
        /// <summary>
        /// Clear the red border from all of the text boxes
        /// </summary>
        private void clearErrorWarning()
        {
            rectCustTitle.Stroke = null;
            rectCustFirstName.Stroke = null;
            rectCustLastName.Stroke = null;
            rectCustDoB.Stroke = null;
            rectCustNat.Stroke = null;
            rectCustEmail.Stroke = null;
            rectCustAllowence.Stroke = null;
        }

        /// <summary>
        /// Checks if the given parameter contains "@" & "."
        /// returns false if not
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private Boolean isEmail(String address)
        {
            if (address.ToLower().Contains("@") == false
                || address.ToLower().Contains(".") == false)
            {
                //MessageBox.Show("Email is invalid");
                return false;
            }
            return true;
        }

        private void btInterest_Click(object sender, RoutedEventArgs e)
        {
            //Establish connection to database
            OleDbConnection myConn = new OleDbConnection(dbConnection.dbConnect);
            myConn.Open();
            //create data reader
            OleDbDataReader myDR = null;
            OleDbDataReader dataReader = null;
            //create query
            OleDbCommand cmdGetAccount = myConn.CreateCommand();
            //set passed SQL string to query
            cmdGetAccount.CommandText = @"SELECT prodID, accID FROM account;";
           
            try
            {
                /*As an "Operation must be updatable query" exception occurs whenever a sub-query is used in the set clause, 
                 * the code uses a while loop to get the account id and product id of that account into a list
                 * loops through the list (Account) to get intRate for each account
                 * Updates account with accrued interest based on formula: "Accrued Interest = Accrued Interest + (Balance * Account Annual Interest Rate / 365.0)"                 
                 */
                myDR = cmdGetAccount.ExecuteReader();
                // list to store accounts
                List<dataItem> accounts = new List<dataItem>();
                //Read accounts from database to list
                while (myDR.Read()) accounts.Add(new dataItem { prodID = myDR["prodID"].ToString(), accID = myDR["accID"].ToString()});
                //close data reader and connection
                myDR.Close();
                myConn.Close();
                //loop through accounts to add accrued interest
                foreach(var item in accounts)
                {
                    //Complications come when trying to run multiple readers on the same connection, so this creates a new connection
                    OleDbConnection connection = new OleDbConnection(dbConnection.dbConnect);
                    connection.Open();
                    //and commands
                    OleDbCommand cmdGet = connection.CreateCommand();
                    OleDbCommand cmdWrite = connection.CreateCommand();
                    //SQL
                    cmdGet.CommandText = @"SELECT intRate FROM Product WHERE prodID = @prodID;";
                    //parameter passing to prevent sql injection
                    cmdGet.Parameters.AddWithValue("prodID", item.prodID);
                    //get intRate
                    dataReader = cmdGet.ExecuteReader();
                    while (dataReader.Read()) item.intRate = dataReader["intRate"].ToString();
                    //SQL and parameter passing
                    cmdWrite.CommandText = @"UPDATE account SET accrued = accrued + (balance * @intRate / 365.0) WHERE accID = @accID;";
                    cmdWrite.Parameters.AddWithValue("intRate", item.intRate);
                    cmdWrite.Parameters.AddWithValue("accID", item.accID);
                    //Update account
                    cmdWrite.ExecuteNonQuery();

                    connection.Close();
                }
                //set label to tell user their click actually did something
                lbInterestUpdate.Content = "Success";
            }
            //error handling
            catch (Exception ex)
            {
                //close connections (prevents exception errors!)
                myConn.Close();
                //error messages
                MessageBox.Show(ex.ToString());
                lbInterestUpdate.Content = "Fail";
            }
        }
    }
}
