using MantraBioTimeDLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Net;
using System.Text;     // For Encoding

namespace BioTimeAttendance
{
    public class AttendanceDataSender
    {
        private readonly string _apiEndpoint;

        public AttendanceDataSender(string apiEndpoint)
        {
            _apiEndpoint = apiEndpoint;
        }

        public void SendData(DataTable attendanceLogs)
        {
            var attendanceList = new List<Dictionary<string, string>>();

            foreach (DataRow row in attendanceLogs.Rows)
            {
                var attendanceData = new Dictionary<string, string>
                {
                    { "user_id", row["UserId"].ToString() },
                    { "attendance_timestamp", row["VDateTime"].ToString() },
                    { "attendance_type", row["VerifyType"].ToString() },
                    { "attendance_mode", row["VerifyMode"].ToString() }
                };
                attendanceList.Add(attendanceData);
            }

            var jsonContent = JsonConvert.SerializeObject(attendanceList);

            try
            {
                // Create a request to the API endpoint
                WebRequest request = WebRequest.Create(_apiEndpoint);
                request.Method = "POST";
                request.ContentType = "application/json";

                // Convert JSON data to byte array
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonContent);
                request.ContentLength = byteArray.Length;

                // Write data to the request stream
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                // Get the response from the server
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            string responseText = reader.ReadToEnd();
                            Console.WriteLine("Attendance data migrated successfully.");
                            Console.WriteLine(responseText);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                // Handle the WebException
                if (ex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)ex.Response)
                    {
                        using (var errorStream = errorResponse.GetResponseStream())
                        {
                            using (var errorReader = new StreamReader(errorStream))
                            {
                                string errorText = errorReader.ReadToEnd();
                                Console.WriteLine("Failed to migrate attendance data. Status Code: " + errorResponse.StatusCode);
                                Console.WriteLine("Error: " + errorText);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed to migrate attendance data. Error: " + ex.Message);
                }
            }
        }
    }
}