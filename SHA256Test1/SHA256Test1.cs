using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

public class HashDirectory
{

    [STAThreadAttribute]
    public static void Main(String[] args)
    {
        // Initialize a SHA256 hash object.
        SHA256 mySHA256 = SHA256Managed.Create();

        byte[] hash0 = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(""));
        Console.Write("\"\" (null)\t"); PrintByteArray(hash0);

        byte[] hash = mySHA256.ComputeHash(Encoding.ASCII.GetBytes("Hello world!"));
        Console.Write("\"Hello World!\"\t"); PrintByteArray(hash);

        Console.WriteLine();
        foreach (string s in new string[] { "a", "b", "c", "d" })
        {
            byte[] hash2 = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(s));
            Console.Write("\"" + s + "\"\t"); PrintByteArray(hash2);
        }

        Console.WriteLine();
        foreach (string s in new string[] { "a", "b", "c", "d" })
        {
            byte[] hash2 = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(s));
            Console.Write("\"" + s + "\"\t"); PrintByteArray(hash2);
        }

        Console.WriteLine();
        hash = mySHA256.ComputeHash(Encoding.ASCII.GetBytes("Hello world! 0"));
        Console.Write("\"Hello World! 0\"          \t"); PrintByteArray(hash);
        hash = mySHA256.ComputeHash(Encoding.ASCII.GetBytes("Hello world! 1"));
        Console.Write("\"Hello World! 1\"          \t"); PrintByteArray(hash);
        hash = mySHA256.ComputeHash(Encoding.ASCII.GetBytes("Hello world! 2"));
        Console.Write("\"Hello World! 2\"          \t"); PrintByteArray(hash);
        Console.WriteLine("");
        hash = mySHA256.ComputeHash(Encoding.ASCII.GetBytes("Hello world! 10000000000"));
        Console.Write("\"Hello World! 10000000000\"\t"); PrintByteArray(hash);

        Console.WriteLine();
        hash = mySHA256.ComputeHash(Encoding.ASCII.GetBytes("Hello world! 6697050"));
        Console.Write("\"Hello World! 6697050\"     \t"); PrintByteArray(hash);

        long MAXITERATIONS = 10L * 1000 * 1000 * 1000;
        long[] byteCount = new long[256];
        int[] count = new int[] { 0, 0, 0, 0 };
        DateTime dtStart = DateTime.Now;
        for (long i = 0; i < MAXITERATIONS; i++)
        {
            if (i % 1000000 == 0) Console.Write(".");
            byte[] hash2 = mySHA256.ComputeHash(Encoding.ASCII.GetBytes("Hello world! " + i.ToString()));
            foreach (byte b in hash2)
            {
                byteCount[(uint)b]++;
            }
            if (hash2[0] == 0x0)
            {
                if (hash2[1] == 0x0)
                {
                    if (hash2[2] == 0x00)
                    {
                        if (hash2[3] == 0x00)
                        {
                            count[3]++;
                            Console.Write("{0,12} 0x00000000:\t", i); PrintByteArray(hash2);
                        }
                        else
                        {
                            count[2]++;
                            Console.Write("{0,12} 0x000000:\t", i); PrintByteArray(hash2);
                        }
                    }
                    else
                    {
                        count[1]++;
                        //Console.Write("{0,12} 0x0000:\t", i); PrintByteArray(hash2);
                    }
                }
                else
                {
                    count[0]++;
                    //Console.Write("{0,12} 0x00:\t"); PrintByteArray(hash2);
                }
            }
        }

        Console.WriteLine();
        DateTime dtEnd = DateTime.Now;
        TimeSpan tsElapsed = dtEnd - dtStart;
        uint secondsElapsed = (uint)tsElapsed.TotalSeconds;
        double rate = MAXITERATIONS / secondsElapsed;
        double hoursElapsed = secondsElapsed / 60.0 / 60.0;
        Console.WriteLine("seconds:\t" + secondsElapsed.ToString());
        Console.WriteLine("rate:\t" + rate.ToString());
        Console.WriteLine("hours:\t" + hoursElapsed.ToString());

        //for (int i = 0; i < 256; i++)
        //{
        //    Console.WriteLine("byteCount[{0}]:\t{1}", i, byteCount[i]);
        //}

        string[] tag = new string[4] { "0x00", "0x0000", "0x000000", "0x00000000" };
        Console.WriteLine();
        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine("{0,-10}\t{1}", tag[i], count[i]);
        }

        Console.Write("Press Enter to exit...");
        Console.ReadLine();

        foreach (string s in new string[] { "a", "b", "c", "d" })
        {
            string nonce = DateTime.Now.Ticks.ToString();
            string s2 = s + nonce;
            byte[] hash2 = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(s2));
            Console.Write(s2 + ":\t"); PrintByteArray(hash2);
        }

        string directory = "";
        if (args.Length < 1)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult dr = fbd.ShowDialog();
            if (dr == DialogResult.OK)
                directory = fbd.SelectedPath;
            else
            {
                Console.WriteLine("No directory selected.");
                directory = "c:/temp";
            }
        }
        else
            directory = args[0];

        Console.WriteLine("Directory:\t" + directory);
    
        try
        {
            // Create a DirectoryInfo object representing the specified directory.
            DirectoryInfo dir = new DirectoryInfo(directory);
            // Get the FileInfo objects for every file in the directory.
            FileInfo[] files = dir.GetFiles();

            byte[] hashValue;
            // Compute and print the hash values for each file in directory.
            foreach (FileInfo fInfo in files)
            {
                // Create a fileStream for the file.
                FileStream fileStream = fInfo.Open(FileMode.Open);
                // Be sure it's positioned to the beginning of the stream.
                fileStream.Position = 0;
                // Compute the hash of the fileStream.
                hashValue = mySHA256.ComputeHash(fileStream);
                // Write the name of the file to the Console.
                Console.Write(fInfo.Name + ":\t");
                // Write the hash value to the Console.
                PrintByteArray(hashValue);
                // Close the file.
                fileStream.Close();
            }
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Error: The directory specified could not be found.");
        }
        catch (IOException)
        {
            Console.WriteLine("Error: A file in the directory could not be accessed.");
        }

        Console.Write("Press Enter to exit...");
        Console.ReadLine();
    }
    // Print the byte array in a readable format.
    public static void PrintByteArray(byte[] array)
    {
        int i;
        for (i = 0; i < array.Length; i++)
        {
            Console.Write(String.Format("{0:X2}", array[i]));
            if ((i % 4) == 3) Console.Write(" ");
        }
        Console.WriteLine();
    }
}