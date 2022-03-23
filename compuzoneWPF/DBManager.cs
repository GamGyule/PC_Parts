using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace compuzoneWPF
{
    class DBManager
    {
        private MySqlConnection conn;
        private string server;
        private string database;
        private string uid;
        private string upw;

        public static string sql = "select name,info,price,pid,count,co from product";
        public static string query = "select name,info,price,pid,count,co from product";
        public static string final_query = "select name,info,price,pid,count,co  from product";

        public DBManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            server = "localhost";
            database = "compuzone";
            uid = "gamgyule";
            upw = "wkddnwls1!";
            string connectionString;
            connectionString = "Server=" + server + ";" + "Database=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + upw + ";CharSet=UTF8mb4";

            conn = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                conn.Open();
                return true;
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 0:
                        MessageBox.Show("서버와 연결이 끊겼습니다.");
                        break;
                    case 1045:
                        MessageBox.Show("아이디 또는 비밀번호가 틀렸습니다.");
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }



        
        public List<Product> ProductShow()
        {
            List<Product> products = new List<Product>();
            if (this.OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(final_query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (int.Parse(reader["count"].ToString()) < 1)
                    {
                        continue;
                    }
                    string name = reader["name"].ToString();
                    string info = reader["info"].ToString();
                    int price = int.Parse(reader["price"].ToString());
                    int pid = int.Parse(reader["pid"].ToString());
                    string co = reader["co"].ToString();

                    products.Add(new Product(name, info, price, pid,co));
                }
                this.CloseConnection();
            }
            return products;
        }

        public void BuyProduct(int pid,int price,int count, string cname, string cphone)
        {

            if (this.OpenConnection())
            {
                string sql = "insert into orders(date,pid,price,count,cname,cphone) values(curdate(),@param2,@param3,@param4,@param5,@param6)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@param2", pid);
                cmd.Parameters.AddWithValue("@param3", price);
                cmd.Parameters.AddWithValue("@param4", count);
                cmd.Parameters.AddWithValue("@param5", cname);
                cmd.Parameters.AddWithValue("@param6", cphone);

                cmd.ExecuteNonQuery();

                string sql2 = "update product set count=count-@param1 where pid=@param2";
                cmd = new MySqlCommand(sql2, conn);

                cmd.Parameters.AddWithValue("@param1", count);
                cmd.Parameters.AddWithValue("@param2", pid);

                cmd.ExecuteNonQuery();

            }
            this.CloseConnection();

        }

        public List<AdminProduct> DataLoad()
        {
            List<AdminProduct> adminProducts = new List<AdminProduct>();
            if (this.OpenConnection())
            {
                string sql = "select * from product";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    adminProducts.Add(new AdminProduct(reader["name"].ToString(), reader["info"].ToString(), Int32.Parse(reader["price"].ToString()), reader["co"].ToString(), Int32.Parse(reader["count"].ToString()), Int32.Parse(reader["pid"].ToString())));
                }

            }
            this.CloseConnection();
            return adminProducts;
        }

        public String searchName(string phone)
        {
            String userName = "";
            if (this.OpenConnection())
            {
                string sql = "select distinct(cname) from orders where cphone = " + phone;

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    userName = reader["cname"].ToString();
                }
            }
            this.CloseConnection();
            return userName;
        }

        public void ProductAdd(AdminProduct adminProduct)
        {
            if (this.OpenConnection())
            {
                string sql = "insert into product(name,info,price,co,count) values(@param1,@param2,@param3,@param4,@param5)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@param1", adminProduct.Name);
                cmd.Parameters.AddWithValue("@param2", adminProduct.Info);
                cmd.Parameters.AddWithValue("@param3", adminProduct.Price);
                cmd.Parameters.AddWithValue("@param4", adminProduct.Img);
                cmd.Parameters.AddWithValue("@param5", adminProduct.Count);

                cmd.ExecuteNonQuery();
            }
            this.CloseConnection();
        }

        public void ProductModify(AdminProduct adminProduct, int pid)
        {
            if (this.OpenConnection())
            {
                string sql = "update product set name=@param1,info=@param2,price=@param3,co=@param4,count=@param5 where pid="+pid;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@param1", adminProduct.Name);
                cmd.Parameters.AddWithValue("@param2", adminProduct.Info);
                cmd.Parameters.AddWithValue("@param3", adminProduct.Price);
                cmd.Parameters.AddWithValue("@param4", adminProduct.Img);
                cmd.Parameters.AddWithValue("@param5", adminProduct.Count);
                cmd.ExecuteNonQuery();
            }
            this.CloseConnection();
        }

        public void ProductDel(int pid)
        {
            if (this.OpenConnection())
            {
                string sql = "delete from product where pid=" + pid;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            this.CloseConnection();
        }

        public List<AdminOrder> OrderLoad(string begin, string end)
        {
            List<AdminOrder> adminOrders = new List<AdminOrder>();

            if (this.OpenConnection())
            {
                string sql = "select * from orders where date between @param1 and @param2 order by date desc;";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@param1", begin);
                cmd.Parameters.AddWithValue("@param2", end);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    adminOrders.Add(new AdminOrder(Int32.Parse(reader["orderid"].ToString()),reader["cname"].ToString(), reader["cphone"].ToString(), reader["date"].ToString(), Int32.Parse(reader["pid"].ToString()), Int32.Parse(reader["count"].ToString()), Int32.Parse(reader["price"].ToString())));
                }

            }
            this.CloseConnection();
            return adminOrders;
        }

        public List<AdminProduct> SearchDataLoad(string filter)
        {
            List<AdminProduct> adminProducts = new List<AdminProduct>();
            if (this.OpenConnection())
            {
                string sql = "select * from product where name like '%"+filter+"%'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    adminProducts.Add(new AdminProduct(reader["name"].ToString(), reader["info"].ToString(), Int32.Parse(reader["price"].ToString()), reader["co"].ToString(), Int32.Parse(reader["count"].ToString()), Int32.Parse(reader["pid"].ToString())));
                }

            }
            this.CloseConnection();
            return adminProducts;
        }
    }
}
