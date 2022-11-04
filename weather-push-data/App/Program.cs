using Microsoft.Data.SqlClient;
class Program
{
    static void Main(string[] args)
    {
        bool tableCreated = false;
        bool dataInserted = false;
        bool tableSelected = false;
        bool succeeded = false;

        const int retryCount = 10;
        const int retryIntervalSeconds = 5;
        const string containingFolder = "./DB_Files/";

        string connectionString = File.ReadAllText(containingFolder + "connection_string.txt");
        SqlConnection connection = new SqlConnection(connectionString);

        for (int tries = 1; tries <= retryCount; tries++)
        {
            try
            {
                // Sleep only for 2nd try onwards
                Thread.Sleep(tries > 1 ? 1000 * retryIntervalSeconds : 0);
                Console.WriteLine("Attempting try {0}/{1} in {2} seconds.", tries, retryCount, retryIntervalSeconds);

                if (!tableCreated)
                {
                    string createTableQuery = File.ReadAllText(containingFolder + "CREATE_TABLE.sql");
                    SqlCommand sqlCommand = new SqlCommand(createTableQuery, connection);
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    sqlCommand.ExecuteNonQuery();
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    tableCreated = true;
                }

                Console.WriteLine(tableCreated ? "Table Created" : "Table not created.");

                if (!dataInserted)
                {
                    string insertDataQuery = File.ReadAllText(containingFolder + "INSERT_DATA.sql");
                    SqlCommand sqlCommand = new SqlCommand(insertDataQuery, connection);
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    sqlCommand.ExecuteNonQuery();
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    dataInserted = true;
                }
                
                Console.WriteLine(dataInserted ? "Data Inserted" : "Data not inserted.");

                // For debug purposes
                if (!tableSelected)
                {
                    String selectTableQuery = File.ReadAllText(containingFolder + "SELECT_TABLE.sql");
                    SqlCommand sqlCommand = new SqlCommand(selectTableQuery, connection);
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        // Format output so that RID is left aligned with 5 width, Area is left aligned with 40 width, etc.
                        Console.WriteLine("{0, -5}{1, -40}{2, -45}{3, -25}{4, -25}", 
                            reader["RID"].ToString(),
                            reader["AREA"].ToString(),
                            reader["FORECAST"].ToString(),
                            reader["STARTTIME"].ToString(),
                            reader["ENDTIME"].ToString()
                        );
                    }

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    tableSelected = true;
                }

                succeeded = (tableCreated && dataInserted && tableSelected);
                if (succeeded)
                {
                    Console.WriteLine("All queries executed successfully.");
                    break;
                }
            } 
            catch (SqlException exception) 
            {
              Console.WriteLine("SqlException {0}.", exception.Number);
              succeeded = false;  
            }
        }
    }
}