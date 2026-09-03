using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using 

namespace BCMS_ConsoleApp
{
    internal class Program
    {


        //using nerede 
    //        public static bool Find(int personID, ref string firstName, ref string lastName, ref DateTime? dateOfBirth, ref string phone, ref string address, ref string email, ref string imagePath)
    //        {

     
    //            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
    //            {
    //                string query = "select * from People where ID=@ID";

    //                using (SqlCommand cmd = new SqlCommand(query, connection)
    //)
    //                {
    //                    cmd.Parameters.AddWithValue("@ID", personID);

    //                    try
    //                    {
    //                        connection.Open();
    //                        using (SqlDataReader read = cmd.ExecuteReader())
    //                        {
    //                            if (read.Read())
    //                            {

    //                                personID = Convert.ToInt32(read["ID"]);
    //                                firstName = read["firstName"].ToString();
    //                                lastName = read["lastName"].ToString();
    //                                dateOfBirth = Convert.ToDateTime(read["dateOfBirth"]);
    //                                phone = read["phone"].ToString();

    //                                address = read["address"]?.ToString();
    //                                email = read["Email"]?.ToString();
    //                                imagePath = read["ImagePath"]?.ToString();

    //                                read.Close();

    //                                return true;
    //                            }

    //                        }
    //                    catch (Exception ex)
    //                    {
    //                        //Log message to view logger
    //                        return false;
    //                    }
    //                    finally
    //                    {
    //                        connection.Close();

    //                    }
    //                }
                          
    //                }
                

    //            }

    //            return false;

    //        }



        static void Main(string[] args)
        {

          DataTable dt=personda

        }
    }
}
