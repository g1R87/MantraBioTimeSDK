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
    class Program
    {
        // Default device configurations
        static int deviceType = 2; 
        static int deviceId = 1;
        static string deviceIP = "192.168.0.105";
        static int devicePort = 5005;
        static int devicePassword = 1234;
        static string apiEndpoint = "http://127.0.0.1:8000/api/attendance";
        static bool isConnected = false;

        static void Main(string[] args)
        {

            while (true)
            {
                Console.Clear();
                Console.Write("Device Connected Succesfully");
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Import all data");
                Console.WriteLine("2. Import unread data");
                Console.WriteLine("3. Change device information");
                Console.WriteLine("4. Exit");

                string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            ImportAllData();
                            break;
                        case "2":
                            ImportUnreadData();
                            break;
                        case "3":
                            ChangeDeviceInformation();
                            break;
                        case "4":
                            Console.WriteLine("Exiting...");
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please select again.");
                            break;
                    }
          


                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void ImportAllData()
        {
            Console.WriteLine("Connecting Device..");

            if (ConnectDevice())
            {
                Console.WriteLine("Device Connected!");

                // Simulate fetching unread data
                Console.WriteLine("Importing all data...");
                DataTable allData = new DataTable();

                int result = MantraBioTime.ReadAllLogData(deviceType, ref allData);

                if (result == 0)
                {

                    var dataSender = new AttendanceDataSender(apiEndpoint);
                    dataSender.SendData(allData);
                    Console.WriteLine("All data imported successfully.");
                }
                else
                {
                    Console.WriteLine("All data import failed");
                }
            }
            else
            {
                Console.WriteLine("Device Not Connected!");
            }
        }

        static void ImportUnreadData()
        {
            Console.WriteLine("Connecting Device..");

            if (ConnectDevice())
            {
                Console.WriteLine("Device Connected!");
                // Simulate fetching unread data
                Console.WriteLine("Importing unread data...");
                // Here you would add the logic to fetch and display unread data
                DataTable unreadData = new DataTable(); // Replace with actual data retrieval
                Console.WriteLine("Unread data imported successfully.");
            } else
            {
                Console.WriteLine("Device Not Connected!");
            }
        }

        static bool ConnectDevice()
        {
            if (!isConnected) { 
                    int result = MantraBioTime.ConnectTCP(deviceId, deviceIP, devicePort, devicePassword, deviceType);
                if (result == 0) isConnected = true;
                return isConnected;
            }
            return isConnected;
        }

        static void ChangeDeviceInformation()
        {
            Console.WriteLine("Change Device Information:");

            Console.Write("Enter Device Type (current: " + deviceType + "): ");
            string input = Console.ReadLine();
            if (!String.IsNullOrEmpty(input)) deviceType = int.Parse(input);

            Console.Write("Enter Device ID (current: " + deviceId + "): ");
            input = Console.ReadLine();
            if (!String.IsNullOrEmpty(input)) deviceId = int.Parse(input);

            Console.Write("Enter Device IP (current: " + deviceIP + "): ");
            input = Console.ReadLine();
            if (!String.IsNullOrEmpty(input)) deviceIP = input;

            Console.Write("Enter Device Port (current: " + devicePort + "): ");
            input = Console.ReadLine();
            if (!String.IsNullOrEmpty(input)) devicePort = int.Parse(input);

            Console.Write("Enter Device Password (current: " + devicePassword + "): ");
            input = Console.ReadLine();
            if (!String.IsNullOrEmpty(input)) devicePassword = int.Parse(input);

            Console.Write("Enter API Endpoint (current: " + apiEndpoint + "): ");
            input = Console.ReadLine();
            if (!String.IsNullOrEmpty(input)) apiEndpoint = input;

            Console.WriteLine("Device information updated successfully.");
        }
    }
}
