using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
namespace OutlawHess
{
    /// <summary>
    /// Interaction logic for customerAccount.xaml
    /// </summary>
    public partial class customerAccount : Window
    {
        /// <summary> Object for window that opened this. Used to open that window.</summary>
        MainWindow winSender;
        /// <summary> Customer details</summary>
        dataItem customer;
        /// <summary> Stores whether the app can close when this window does.</summary>
        private bool canQuitApp = true;
        datagridFiller Filler;
        //contents of the data grids
        List<dataItem> products = new List<dataItem>();
        List<dataItem> accounts = new List<dataItem>();
        //Establish connection to database
        OleDbConnection myConn = new OleDbConnection(dbConnection.dbConnect);
        //create data reader
        OleDbDataReader myDR = null;
        public customerAccount( MainWindow window, dataItem dataItem)
        {
            InitializeComponent();
            //set local objects as passed values.
            winSender = window;
            customer = dataItem;

            //Display customer details
            lbcustID.Content = customer?.id?.ToString() ?? "id = null";
            txtCustFName.Text = customer?.fName?.ToString() ?? "fname = null";
            txtCustLname.Text = customer?.lName?.ToString() ?? "lname = null";
            txtDob.Text = customer?.DoB?.ToString() ?? "dob = null";
            txtNatins.Text = customer?.natins?.ToString() ?? "natins = null";
            txtEmail.Text = customer?.email?.ToString() ?? "email = null";
            txtAllowence.Text = customer?.allowence?.ToString() ?? "allowance = null";

            //add columns to account data grid
            Column.AddColumn("account id", "accID", dgCustAccounts);
            Column.AddColumn("customer id", "custID", dgCustAccounts);
            Column.AddColumn("product id", "prodID", dgCustAccounts);
            Column.AddColumn("balance", "balance", dgCustAccounts);
            Column.AddColumn("accrued", "accrued", dgCustAccounts);
            Column.AddColumn("active", "active", dgCustAccounts);
            //add columns to transaction data grid
            Column.AddColumn("Transaction ID", "tranxID", dgCustTransactions);
            Column.AddColumn("account ID", "accID", dgCustTransactions);
            Column.AddColumn("Action", "action", dgCustTransactions);
            Column.AddColumn("Amount", "amnt", dgCustTransactions);
            Column.AddColumn("DateTime", "theEvent", dgCustTransactions);
            //Fill data grids; data base query
            Filler = new datagridFiller(dgCustAccounts);
            Filler.fillDataGrid("select * from account WHERE custID = " + (customer?.id ?? "1=2") + ";");

            //fill product combo box
            fillCombo(CBProduct, products, "SELECT ProdID, theName FROM Product;", "prodName", "id");
            //fill account combo box
            fillCombo(CBAccount, accounts, "SELECT accID FROM account WHERE custID = " + lbcustID.Content + ";", "id", "id");
        }
        /// <summary>
        /// Fills contents of passed combo box with values from database.
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="dataItems"></param>
        /// <param name="query"></param>
        /// <param name="member"></param>
        /// <param name="value"></param>
        private void fillCombo(ComboBox comboBox, List<dataItem> dataItems, string query, string displayValue, string selector)
        {
            //Fill List with passed query's database results
            fillComboBoxList(dataItems, query);
            //Set list as combo box item source
            comboBox.ItemsSource = dataItems;
            //Set property from data item to display 
            comboBox.DisplayMemberPath = displayValue;
            //Selector for record to display
            comboBox.SelectedValuePath = selector;

            //Procedure to query the database a fill list with result 
            void fillComboBoxList(List<dataItem> items, string sql)
            {
                myConn.Open();
                //create query
                OleDbCommand myCmd = myConn.CreateCommand();
                //set passed SQL string to query
                myCmd.CommandText = sql;
                try
                {
                    //run query
                    myDR = myCmd.ExecuteReader();
                    while (myDR.Read())
                    {
                        //null validation arrow function
                        string outOfBounds(int i) => myDR.FieldCount > i ? myDR[i].ToString() : "";
                        //add new data item to items with values read from the database.
                        items.Add(new dataItem
                        {
                            //add properties
                            id = outOfBounds(0),
                            prodName = outOfBounds(1)
                        });
                    }
                }
                catch (Exception e)
                {
                    //display unhelpful obtuse system generation error message
                    MessageBox.Show(e.ToString());
                }
                //close connections (prevents exception errors!)
                myConn?.Close();
                myDR?.Close();
            }
        }
        /// <summary>
        /// Open main window, hide this.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btReturn_Click(object sender, RoutedEventArgs e)
        {
            winSender.Show();
            winSender.fillDataGrids();
            canQuitApp = false;
            Close();
        }
        /// <summary>
        /// Close app is if can quit == true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(canQuitApp) winSender.Close();
        }
        /// <summary>
        /// Show transactions of selected account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgCustAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filler = new datagridFiller(dgCustTransactions);                       
            dataItem acc = (dataItem)dgCustAccounts.SelectedItem;
            Filler.fillDataGrid("select * from tranx WHERE accID = " + (acc?.accID ?? "1=2") + ";");
        }
        /// <summary>
        /// Handle possible exception error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgCustAccounts_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }
        /// <summary>
        /// Update customer record in database.s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUpdateCust_Click(object sender, RoutedEventArgs e)
        {
            //label shows feedback of whether the record was updated successfully or not.
            lbUpdated.Content = Filler.WriteCust(@"update customer set"
                + @" [firstName] = @goGetFName, [LastName] = @goGetLName, "
                + @"[DoB] = @goGetDoB, [natins] = @goGetNat, [email] = @goGetEmail, "
                + @"[allowence] = @goGetAllowence WHERE [custID] = @goGetId;",
                new List<string> { "goGetFName", "goGetLName", "goGetDoB",
                    "goGetNat", "goGetEmail", "goGetAllowence", "goGetId" },
                new List<object> { txtCustFName, txtCustLname, txtDob,
                    txtNatins, txtEmail, txtAllowence, lbcustID }) ? "Success" : "Fail";
        }
        //Delete this customer from database.
        private void btDeleteCust_Click(object sender, RoutedEventArgs e)
        {
            //display message box to confirm user action.
            if(MessageBox.Show("Permantly *DELETE* this record?", "Sure?", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //Delete customer from database
                lbUpdated.Content = Filler.WriteCust(@"DELETE FROM customer WHERE"
                + @"[custID] = @goGetId;",
                new List<string> { "goGetId" },
                new List<object> { lbcustID }) ? "Success" : "Fail";
                //go to main window
                btReturn_Click(sender,e);
            }
            
        }           

        private void btAddIsa_Click(object sender, RoutedEventArgs e)
        {
            //validation
            if (CBProduct.SelectedIndex == -1) return;

            myConn.Open();

            //create query
            OleDbCommand myCmd = myConn.CreateCommand();
            //set passed SQL string to query
            myCmd.CommandText = @"INSERT INTO account (custID, prodID, active) VALUES (@custID, @prodID, true);";
            myCmd.Parameters.AddWithValue("custID"
                        , lbcustID.Content);
            myCmd.Parameters.AddWithValue("prodID"
                        , CBProduct.SelectedValue);
            try
            {
                myCmd.ExecuteReader();
                //close connections (prevents exception errors!)
                myConn.Close();
                Filler = new datagridFiller(dgCustAccounts);
                Filler.fillDataGrid("select * from account WHERE custID = " + (customer?.id ?? "1=2") + ";");
            }
            catch (Exception ex)
            {
                //close connections (prevents exception errors!)
                myConn.Close();
                MessageBox.Show(ex.ToString());
            }
            myConn.Close();
            //fill account combo box
            fillCombo(CBAccount, accounts, "SELECT accID FROM account WHERE custID = " + lbcustID.Content + ";", "id", "id");
        }

        private void btDelIsa_Click(object sender, RoutedEventArgs e)
        {
            myConn.Open();

            //create query
            OleDbCommand myCmd = myConn.CreateCommand();
            //set passed SQL string to query
            myCmd.CommandText = @"DELETE FROM account WHERE custID = @custID AND accID = @accID";
            myCmd.Parameters.AddWithValue("custID"
                        , lbcustID.Content);
            dataItem item = (dataItem)dgCustAccounts.SelectedItem;
            myCmd.Parameters.AddWithValue("accID"
                        ,  item?.accID ?? "1=55");
            try
            {
                myCmd.ExecuteReader();
                //close connections (prevents exception errors!)
                myConn.Close();
                Filler = new datagridFiller(dgCustAccounts);
                Filler.fillDataGrid("select * from account WHERE custID = " + (customer?.id ?? "1=2") + ";");
            }
            catch (Exception ex)
            {
                //close connections (prevents exception errors!)
                myConn.Close();
                MessageBox.Show(ex.ToString());
            }
            myConn.Close();
        }

        private void btTransfer_Click(object sender, RoutedEventArgs e)
        {
            // validation
            if (CBAccount.SelectedIndex == -1 || dgCustAccounts.SelectedIndex == -1) return;
            else if (double.TryParse(txtAmount.Text, out double x) == false) return;
            myConn.Open();

            /*Command can't perform multiple queries, thus multiple commands are required*/
            //create query
            OleDbCommand myCmd = myConn.CreateCommand();
            OleDbCommand myCmd2 = myConn.CreateCommand();
            //set passed SQL string to query
            myCmd.CommandText = @"UPDATE account SET balance = balance - @transAmnt1 WHERE accID = @accIDFrom;";
                //parameter passing
                myCmd.Parameters.AddWithValue("transAmnt1"
                            , txtAmount.Text);
                dataItem item = (dataItem)dgCustAccounts.SelectedItem;
                myCmd.Parameters.AddWithValue("accIDFrom"
                            , item?.accID ?? "1=55");
            //set passed SQL string to query
            myCmd2.CommandText = @"UPDATE account SET balance = balance + @transAmnt2 WHERE accID = @accIDTo;";
                //parameter passing
                myCmd2.Parameters.AddWithValue("transAmnt1"
                            , txtAmount.Text);
                myCmd2.Parameters.AddWithValue("accIDTo"
                            , CBAccount.SelectedValue);
            try
            {
                //execute query
                myCmd.ExecuteNonQuery();
                myCmd2.ExecuteNonQuery();
                //close connections (prevents exception errors!)
                myConn.Close();
                Filler = new datagridFiller(dgCustAccounts);
                Filler.fillDataGrid("select * from account WHERE custID = " + (customer?.id ?? "1=2") + ";");
            }
            catch (Exception ex)
            {
                //close connections (prevents exception errors!)
                myConn.Close();
                MessageBox.Show(ex.ToString());
            }
            myConn.Close();
        }
    }
}


