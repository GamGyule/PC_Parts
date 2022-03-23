using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace compuzoneWPF
{
    /// <summary>
    /// AdminForm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AdminForm : Window
    {
        DBManager db = new DBManager();
        MainWindow main = null;
        TextStyle textStyle = new TextStyle();

        public AdminForm()
        {
            InitializeComponent();
        }

        public AdminForm(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void adminTableLoad(List<AdminProduct> data)
        {
            List<AdminProduct> adminProducts = data;

            DataTable dt = new DataTable();

            dt.Columns.Add("번호");
            dt.Columns.Add("이름");
            dt.Columns.Add("정보");
            dt.Columns.Add("가격");
            dt.Columns.Add("회사명");
            dt.Columns.Add("개수");
           

            for (int i = 0; i < adminProducts.Count; i++)
            {
                dt.Rows.Add(adminProducts[i].Index, adminProducts[i].Name, adminProducts[i].Info, String.Format("{0:N0}원", adminProducts[i].Price), adminProducts[i].Img, adminProducts[i].Count);
            }


            productDataGrid.ItemsSource = dt.DefaultView;
            productDataGrid.Columns[2].Width = 700;
            productDataGrid.Columns[3].CellStyle = textStyle.getTextRight();

        }

        private void adminForm_Loaded(object sender, RoutedEventArgs e)
        {
            beginDate.SelectedDate = DateTime.Now.Date.AddMonths(-1);
            endDate.SelectedDate = DateTime.Now.Date;
            adminTableLoad(db.DataLoad());
        }

        private void AdminAdd_Click(object sender, RoutedEventArgs e)
        {
            AdminProduct ap = new AdminProduct(adminName.Text, adminInfo.Text, Int32.Parse(adminPrice.Text), adminImg.Text, Int32.Parse(adminCount.Text), 0);
            db.ProductAdd(ap);


            productDataGrid.ScrollIntoView(productDataGrid.Items[productDataGrid.Items.Count - 1]);
            DataGridRow row = (DataGridRow)productDataGrid.ItemContainerGenerator.ContainerFromIndex(productDataGrid.Items.Count - 1);
            row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            productDataGrid.UpdateLayout();



            adminTextInit();
        }

        private void AdminModify_Click(object sender, RoutedEventArgs e)
        {
            AdminProduct ap = new AdminProduct(adminName.Text, adminInfo.Text, Int32.Parse(adminPrice.Text), adminImg.Text, Int32.Parse(adminCount.Text), 0);
            db.ProductModify(ap,pid);

            adminTextInit();
        }

        int pid;
        private void AdminDel_Click(object sender, RoutedEventArgs e)
        {
            if (pid == 0)
                return;
            db.ProductDel(pid);

            adminTextInit();
        }

        private void adminTextInit()
        {
            adminName.Text = "";
            adminInfo.Text = "";
            adminPrice.Text = "";
            adminCount.Text = "";
            adminImg.Text = "";

            adminTableLoad(db.DataLoad());
        }

       
        private void adminForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.ProductList_Refresh();
        }

        private void productDataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataRowView myRow = (DataRowView)productDataGrid.CurrentCell.Item;
            string tempStr = Regex.Replace(myRow.Row.ItemArray[3].ToString(), @"\D", "");

            adminName.Text = myRow.Row.ItemArray[1].ToString();
            adminInfo.Text = myRow.Row.ItemArray[2].ToString();
            adminPrice.Text = tempStr; // 11,000원 에서 숫자만 가져오기
            adminCount.Text = myRow.Row.ItemArray[5].ToString();
            adminImg.Text = myRow.Row.ItemArray[4].ToString();
            pid = Int32.Parse(myRow.Row.ItemArray[0].ToString());
        }

        private void AdminOrderDataGridLoad()
        {
            List<AdminOrder> adminOrders = db.OrderLoad(beginDate.SelectedDate.ToString().Substring(0,10), endDate.SelectedDate.ToString().Substring(0,10));

            DataTable dt = new DataTable();

            dt.Columns.Add("번호");
            dt.Columns.Add("이름");
            dt.Columns.Add("핸드폰 번호");
            dt.Columns.Add("날짜");
            dt.Columns.Add("상품 번호");
            dt.Columns.Add("개수");
            dt.Columns.Add("가격");

            int sum = 0;
            for (int i = 0; i < adminOrders.Count; i++)
            {
                dt.Rows.Add(adminOrders[i].Index, adminOrders[i].Name, adminOrders[i].Phone, adminOrders[i].Date.ToString().Substring(0, 10), adminOrders[i].Pid, adminOrders[i].Count, adminOrders[i].Price.ToString("#,##0원"));
                sum += adminOrders[i].Price;
            }
            adminTotal.Text = sum.ToString("#,##0원");

            orderDataGrid.ItemsSource = dt.DefaultView;
        }

        private void adminOrderSearch_Click(object sender, RoutedEventArgs e)
        {
            AdminOrderDataGridLoad();
            distinctGrid();
        }

        private void ChartShow()
        {
            distinctGrid();
        }

        private void distinctGrid()
        {
            string[] arr = new string[orderDataGrid.Items.Count];
            for (int i = 0; i < orderDataGrid.Items.Count; i++)
            {
                DataRowView myRow = (DataRowView)orderDataGrid.Items[i];
                string myDate = myRow.Row.ItemArray[3].ToString();

                arr[orderDataGrid.Items.Count-i-1] = myDate;
            }
            
            List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();

            int tempCnt = 0;

            int[] sum = new int[arr.Distinct().ToArray().Count()];
            foreach (string str in arr.Distinct().ToArray()) {
                for (int i = 0; i < orderDataGrid.Items.Count; i++)
                {
                    DataRowView myRow = (DataRowView)orderDataGrid.Items[i];
                    string myDate = myRow.Row.ItemArray[3].ToString();  
                    if (myDate == str)
                    {
                        int tempPrice = Int32.Parse(Regex.Replace(myRow.Row.ItemArray[6].ToString(), @"\D", ""));
                        int tempCount = Int32.Parse(myRow.Row.ItemArray[5].ToString());
                        //sum[tempCnt] += int.Parse(myRow.Row.ItemArray[5].ToString()) * tempPrice;
                        sum[tempCnt] += tempCount * tempPrice;
                    }
                }
                tempCnt++;
            }


            int cnt = 0;
            foreach (string str in arr.Distinct().ToArray())
            {
                KeyValuePair<string, int> keyValuePair = new KeyValuePair<string, int>(str.Substring(2,8),sum[cnt]);
                Console.WriteLine(keyValuePair);
                items.Add(keyValuePair);
                cnt++;
            }

            orderDataGrid.Columns[1].Width = 70;
            orderDataGrid.Columns[2].Width = 150;
            orderDataGrid.Columns[3].Width = 150;
            orderDataGrid.Columns[6].Width = 220;

            orderDataGrid.Columns[6].CellStyle = textStyle.getTextRight();



            ((ColumnSeries)myChart.Series[0]).DataContext = items;
        }

        private void AdminOrderSearch_month_Click(object sender, RoutedEventArgs e)
        {
            beginDate.SelectedDate = DateTime.Now.Date.AddMonths(-1);
            endDate.SelectedDate = DateTime.Now.Date;
            adminTableLoad(db.DataLoad());

            AdminOrderDataGridLoad();
            distinctGrid();
        }

        private void AdminOrderSearch_years_Click(object sender, RoutedEventArgs e)
        {
            beginDate.SelectedDate = DateTime.Now.Date.AddYears(-1);
            endDate.SelectedDate = DateTime.Now.Date;
            adminTableLoad(db.DataLoad());

            AdminOrderDataGridLoad();
            distinctGrid();
        }

        private void adminSearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(adminSearchBox.Text != "")
            {
                List<AdminProduct> adminProducts = db.SearchDataLoad(adminSearchBox.Text);

                DataTable dt = new DataTable();

                dt.Columns.Add("번호");
                dt.Columns.Add("이름");
                dt.Columns.Add("정보");
                dt.Columns.Add("가격");
                dt.Columns.Add("파일명");
                dt.Columns.Add("개수");


                for (int i = 0; i < adminProducts.Count; i++)
                {
                    dt.Rows.Add(adminProducts[i].Index, adminProducts[i].Name, adminProducts[i].Info, String.Format("{0:N0}원", adminProducts[i].Price), adminProducts[i].Img, adminProducts[i].Count);
                }


                productDataGrid.ItemsSource = dt.DefaultView;
                productDataGrid.Columns[2].Width = 700;
                productDataGrid.Columns[3].CellStyle = textStyle.getTextRight();
            }
            else
            {
                adminTableLoad(db.DataLoad());
            }
        }
    }
}
