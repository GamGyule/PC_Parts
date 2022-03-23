using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using System.Threading;

namespace compuzoneWPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Product> products;
        List<Order> orders = new List<Order>();
        GroupBox[] gb = new GroupBox[5];
        DBManager db = new DBManager();
        public MainWindow()
        {
            InitializeComponent();
            OrderList.DataContext = orders;
            gb[0] = this.cpuRyzenDetailFilter;
            gb[1] = this.cpuIntelDetailFilter;
            gb[2] = this.memoryDetailFilter;
            gb[3] = this.graphicNvidiaDetailFilter;
            gb[4] = this.graphicAmdDetailFilter;


            MainFilter.SelectedIndex = 0;

            ProductList_Refresh();

        }
        private void ProductOrder(object sender, MouseButtonEventArgs e)
        {
            int productIndex = ProductList.SelectedIndex;

            if (products[productIndex].Pid == 1541)
                return;


            if (OrderDistinctCheck(products[productIndex].Pid))
                return;


            orders.Add(new Order(products[productIndex].Name, products[productIndex].Price, 1, products[productIndex].Pid, 0));

            this.OrderList.ItemsSource = orders;
            this.OrderList.Items.Refresh();

            OrderListCheck();
        }

        private void OrderDelete(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is Order)
            {

                Order deleteOrder = (Order)cmd.DataContext;
                orders.Remove(deleteOrder);
                OrderList.Items.Refresh();
            }

            OrderListCheck();
        }

        private bool OrderDistinctCheck(int pid)
        {
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].Pid == pid)
                {
                    MessageBox.Show("이미 주문 된 상품입니다.");
                    return true;
                }
            }
            return false;
        }

        private void CountUp(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is Order)
            {
                Order countUp = (Order)cmd.DataContext;
                countUp.Count++;
                countUp.Result = (countUp.Count * countUp.Price);
                OrderList.Items.Refresh();
                SetResultWon();
            }
        }

        private void CountDown(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is Order)
            {
                Order countDown = (Order)cmd.DataContext;
                if (countDown.Count <= 1)
                    return;
                countDown.Count--;
                countDown.Result = countDown.Count * countDown.Price;
                OrderList.Items.Refresh();
                SetResultWon();
            }
        }
        private void SetResultWon()
        {
            int price = 0;
            for (int i = 0; i < orders.Count; i++)
            {
                price += (orders[i].Price * orders[i].Count);
            }
            ResultWon.Text = String.Format("{0:N0}원", price);
        }

        private void UnHolderPlace()
        {
            if (SearchBox.Text != "검색")
                return;
            SearchBox.Text = "";
            SearchBox.Foreground = Brushes.Black;
        }

        private void OnHolderPlace()
        {
            if (SearchBox.Text != "")
                return;
            SearchBox.Text = "검색";
            SearchBox.Foreground = Brushes.Gray;
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UnHolderPlace();
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            OnHolderPlace();
        }

        Storyboard st = new Storyboard();
        private void OrderListCheck()
        {

            SetResultWon();

            DoubleAnimation anim = new DoubleAnimation();
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            anim.DecelerationRatio = 0.8;
            anim.AccelerationRatio = 0.2;

            Storyboard.SetTarget(anim, hwnd);
            Storyboard.SetTargetProperty(anim, new PropertyPath("(Window.Height)"));

            anim.From = this.Height;
            if (orders.Count > 0)
            {
                anim.To = 750;
            }
            else
            {
                anim.To = 650;
            }

            st.Children.Add(anim);
            st.Begin();
        }
        private void MainFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentComboBox = sender as ComboBox;
            if (currentComboBox != null)
            {
                ComboBoxItem currentItem = currentComboBox.SelectedItem as ComboBoxItem;
                if (currentItem != null)
                {
                    for (int i = 0; i < gb.Length; i++)
                        gb[i].Visibility = Visibility.Hidden;
                    switch (currentItem.Content.ToString())
                    {
                        case "CPU":
                            DBManager.query = DBManager.sql;
                            DBManager.query += " where name like '%라이젠%'";
                            this.cpuRyzen.IsChecked = true;
                            this.cpuFilter.Visibility = Visibility.Visible;

                            this.memoryFilter.Visibility = Visibility.Hidden;
                            this.vgaFilter.Visibility = Visibility.Hidden;



                            this.gb[0].Visibility = Visibility.Visible;
                            this.gb[2].Visibility = Visibility.Hidden;
                            this.gb[3].Visibility = Visibility.Hidden;
                            break;
                        case "메모리":
                            DBManager.query = DBManager.sql;
                            DBManager.query += " where name like '%삼성전자%'";
                            this.memorySamsung.IsChecked = true;
                            this.memoryFilter.Visibility = Visibility.Visible;

                            this.cpuFilter.Visibility = Visibility.Hidden;
                            this.vgaFilter.Visibility = Visibility.Hidden;



                            this.gb[2].Visibility = Visibility.Visible;
                            this.gb[0].Visibility = Visibility.Hidden;
                            this.gb[3].Visibility = Visibility.Hidden;
                            break;
                        case "그래픽카드":
                            DBManager.query = DBManager.sql;
                            DBManager.query += " where name like '%GeForce%'";
                            this.graphicNvidia.IsChecked = true;
                            this.vgaFilter.Visibility = Visibility.Visible;

                            this.cpuFilter.Visibility = Visibility.Hidden;
                            this.memoryFilter.Visibility = Visibility.Hidden;



                            this.gb[3].Visibility = Visibility.Visible;
                            this.gb[0].Visibility = Visibility.Hidden;
                            this.gb[2].Visibility = Visibility.Hidden;

                            break;
                    }
                    DBManager.final_query = DBManager.query;
                    ProductList_Refresh();
                }
            }
        }

        private void CpuRyzen_Checked(object sender, RoutedEventArgs e)
        {
            ShowDetailFilter(0);
            DBManager.query = DBManager.sql;
            DBManager.query += " where name like '%라이젠%'";
            DBManager.final_query = DBManager.query;
            ProductList_Refresh();
        }

        private void CpuIntel_Checked(object sender, RoutedEventArgs e)
        {
            ShowDetailFilter(1);
            DBManager.query = DBManager.sql;
            DBManager.query += " where name like '%INTEL%'";
            DBManager.final_query = DBManager.query;
            ProductList_Refresh();
        }

        private void MemorySamsung_Checked(object sender, RoutedEventArgs e)
        {
            ShowDetailFilter(2);
            DBManager.query = DBManager.sql;
            DBManager.query += " where name like '%삼성전자%'";
            DBManager.final_query = DBManager.query;
            ProductList_Refresh();
        }

        private void MemoryGskill_Checked(object sender, RoutedEventArgs e)
        {
            ShowDetailFilter(2);
            DBManager.query = DBManager.sql;
            DBManager.query += " where name like '%G.Skill%'";
            DBManager.final_query = DBManager.query;
            ProductList_Refresh();
        }

        private void GraphicNvidia_Checked(object sender, RoutedEventArgs e)
        {
            ShowDetailFilter(3);
            DBManager.query = DBManager.sql;
            DBManager.query += " where name like '%GeForce%'";
            DBManager.final_query = DBManager.query;
            ProductList_Refresh();
        }

        private void GraphicAmd_Checked(object sender, RoutedEventArgs e)
        {
            ShowDetailFilter(4);
            DBManager.query = DBManager.sql;
            DBManager.query += " where name like '%Radeon%'";
            DBManager.final_query = DBManager.query;
            ProductList_Refresh();
        }

        public void ProductList_Refresh()
        {
            products = Product.GetProducts();
            this.ProductList.ItemsSource = products;
        }

        private void ShowDetailFilter(int gbIndex)
        {
            for (int i = 0; i < this.gb.Length; i++)
            {
                if (gbIndex == i)
                {
                    gb[i].Visibility = Visibility.Visible;
                }
                else
                {
                    gb[i].Visibility = Visibility.Hidden;
                }
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Searching();
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            Searching();
        }

        private void Searching()
        {
            if (SearchBox.Text == "검색" || SearchBox.Text == "")
            {
                DBManager.final_query = DBManager.query;
            }
            else
            {
                DBManager.final_query = DBManager.query + " and name like '%" + SearchBox.Text + "%'";
            }
            ProductList_Refresh();
        }


        private void AdminBtn_Click(object sender, RoutedEventArgs e)
        {
            AdminForm adform = new AdminForm(this);
            adform.ShowDialog();
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {

            if (phone.Text == "")
            {
                MessageBox.Show("번호를 입력하세요.");
                return;
            }

            DBManager db = new DBManager();
            String name;
            if (custname.Text == "")
            {
                name = db.searchName(phone.Text);
                if (name == "")
                {
                    MessageBox.Show("이름을 입력하세요.");
                    return;
                }
                else
                {
                    custname.Text = name;
                }
            }

            

            if (MessageBox.Show(ResultWon.Text +"구매하시겠습니까?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                for(int i =0; i < orders.Count; i++)
                {
                    db.BuyProduct(orders[i].Pid, orders[i].Price, orders[i].Count, custname.Text, phone.Text);
                }

                MessageBox.Show("구매 완료");
                custname.Text = "";
                phone.Text = "";
                orders.Clear();
                OrderList.Items.Refresh();
                OrderListCheck();
            }
        }

        private void CheckBoxFilterChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string filter = cb.Content.ToString();
            DBManager.final_query += " and name like '%"+filter+"%'";
            ProductList_Refresh();
        }

        private void CheckBoxFilterUnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string filter = cb.Content.ToString();
            DBManager.final_query = DBManager.final_query.Replace(" and name like '%" + filter + "%'", "");
            ProductList_Refresh();
        }

        private void Phone_KeyDown(object sender, KeyEventArgs e)
        {
            Regex reg = new Regex("[^0-9]");
            e.Handled = reg.IsMatch(phone.Text);
        }

        private void hwnd_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            this.Close();
        }

        private void ProductList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProductOrder(sender, e);
        }
    }
}
