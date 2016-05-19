using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace MakeATest
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=10.1.10.148;Initial Catalog=Buzzfeed02; User ID=academy_admin;Password=12345");
            connection.Open();
            int userId = Login(connection);
            int testId = nameYourTest(connection, userId);
            enterQuestionsAndAnswers(connection, testId);
            enterResults(connection, testId);
        }

        static int Login(SqlConnection connection) {
            int userId = 0;
            Console.WriteLine("What is your username?");
            string userName = Console.ReadLine().ToLower().Replace("\'", "~").Replace("\"", "`");
            Console.WriteLine("What is your password?");
            string password = Console.ReadLine().Replace("\'", "~").Replace("\"", "`");

            string insertString = $"INSERT INTO Users (Name, Password) Values('{userName}', '{password}'); Select @@Identity AS ID";
            userId = ReaderFunction(insertString, connection);
            return userId; 
        }

        static int nameYourTest(SqlConnection connection, int userId)
        {
            int testId = 0;
            Console.WriteLine("What would you like to name your test?");
            string testName = Console.ReadLine().Replace("\'", "~").Replace("\"", "`");

            string insertString = $"INSERT INTO Tests (Title, UserId) Values('{testName}', '{userId}'); Select @@Identity AS ID";
            testId = ReaderFunction(insertString, connection);
            return testId;
        }

        static void enterQuestionsAndAnswers(SqlConnection connection, int testId)
        {
            int questionId = 0;
            string input = null;
            while (input != "q")
            {
                Console.WriteLine("Enter a question. Enter 'q' to quit.");
                input = Console.ReadLine().Replace("\'", "~").Replace("\"", "`");
                if (input != "q")
                {
                    string insertString = $"INSERT INTO Questions (TestId, Text, SortOrder) Values('{testId}', '{input}', ''); Select @@Identity AS ID";
                    questionId = ReaderFunction(insertString, connection);
                    bool moreAnswers = true;
                    while (moreAnswers)
                    {
                        Console.WriteLine("Enter your answer");
                        string answer = Console.ReadLine().Replace("\'", "~").Replace("\"", "`");
                        bool keepLooping = true;
                        int intValue = 0;
                        while (keepLooping)
                        {
                            Console.WriteLine("Enter a number value");
                            string stringValue = Console.ReadLine().Replace("\'", "~").Replace("\"", "`");
                            bool isANumber = int.TryParse(stringValue, out intValue);
                            if (isANumber)
                            {
                                keepLooping = false;
                            }
                        }

                        insertString = $"INSERT INTO Answers (QuestionId, Text, Value) Values('{questionId}', '{answer}', '{intValue}'); Select @@Identity AS ID";
                        int answerId = ReaderFunction(insertString, connection);
                        Console.WriteLine("Are there other answers? Y or N");
                        string moreOptions = Console.ReadLine().ToLower();
                        if (moreOptions == "n")
                        {
                            moreAnswers = false;
                        }
                    }
                }
            }
        }

        static void enterResults(SqlConnection connection, int testId)
        {
            bool storingResults = true;
            while (storingResults)
            {
                bool moreLoops = true;

                Console.WriteLine("Name a possible result:");
                string resultTitle = Console.ReadLine().Replace("\'", "~").Replace("\"", "`");
                Console.WriteLine("Describe result:");
                string description = Console.ReadLine().Replace("\'", "~").Replace("\"", "`");
                int intScore = 0;

                while (moreLoops)
                {
                    Console.WriteLine("Enter a score:");
                    string stringScore = Console.ReadLine();
                    bool isANumber = int.TryParse(stringScore, out intScore);
                    if (isANumber)
                    {
                        moreLoops = false;
                    }
                }

                string insertString = $"INSERT INTO Results (TestId, Title, Description, Score) Values('{testId}', '{resultTitle}', '{description}', '{intScore}'); Select @@Identity AS ID";
                testId = ReaderFunction(insertString, connection);

                Console.WriteLine("Do you have more results. Enter 'y'/'n'");
                string moreEndings = Console.ReadLine().ToLower();
                if (moreEndings == "n")
                {
                    storingResults = false;
                }
            }
        }

        static public int ReaderFunction(string insertString, SqlConnection connection)
        {
            int id = 0;
            SqlCommand insertCommand = new SqlCommand(insertString, connection);
            SqlDataReader reader = insertCommand.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                id = Convert.ToInt16(reader["ID"]);
                //Select@@Identity as ID
            }
            reader.Close();
            return id;
        }
    }
}
