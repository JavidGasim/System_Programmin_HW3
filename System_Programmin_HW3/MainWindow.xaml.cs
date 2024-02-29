using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;

namespace System_Programmin_HW3;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public Thread thread1 { get; set; }
    public string path1 { get; set; }
    public string path2 { get; set; }
    public byte[]? Key { get; set; }
    public byte[]? IV { get; set; }
    private bool CheckCancel { get; set; } = false;
    public string SC { get; set; }

    private int maxVal;

    public int MaxVal
    {
        get { return maxVal; }
        set
        {
            maxVal = value;
            OnPropertyChanged();
        }
    }

    private int fileVal;
    public int FileVal
    {
        get { return fileVal; }
        set
        {
            fileVal = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
               => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public MainWindow()
    {
        InitializeComponent();
        MaxVal = 100;

        DataContext = this;
    }

    private void file_btn_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "All Files (*.*)|*.*";

        if (openFileDialog.ShowDialog() == true)
        {
            file_txtbox.Text = openFileDialog.FileName;
            thread1 = new Thread(foo);
            thread1.Start();

            void foo()
            {
                path1 = openFileDialog.FileName;
            }
        }
    }

    private void file2_btn_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog2 = new OpenFileDialog();
        openFileDialog2.Filter = "All Files (*.*)|*.*";

        if (openFileDialog2.ShowDialog() == true)
        {
            file2_txtbox.Text = openFileDialog2.FileName;
            thread1 = new Thread(foo);
            thread1.Start();

            void foo()
            {
                path2 = openFileDialog2.FileName;
            }
        }
    }

    private void start_btn_Click(object sender, RoutedEventArgs e)
    {
        if (encrypt.IsChecked == true)
        {
            thread1 = new Thread(Encryption);
            thread1.Start();
            pb.Value = 0;
        }

        else
        {
            thread1 = new Thread(Decryption);
            thread1.Start();
            pb.Value = 0;
        }

        file_txtbox.Text = "";
        file2_txtbox.Text = "";
    }

    public void Encryption()
    {
        if (!File.Exists(path1))
        {
            MessageBox.Show("From File Not Found");

            return;
        }

        if (!File.Exists(path2))
        {
            MessageBox.Show("To File Not Found");

            return;
        }

        try
        {
            using (FileStream fsInput = new FileStream(path1, FileMode.Open))
            {
                using (FileStream fsOutput = new FileStream(path2, FileMode.Create))
                {
                    using (AesManaged aes = new AesManaged())
                    {
                        aes.GenerateKey();
                        aes.GenerateIV();
                        Key = aes.Key;
                        IV = aes.IV;


                        ICryptoTransform encryptor = aes.CreateEncryptor();
                        using (CryptoStream cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                        {

                            var len = 10;
                            var fileSize = fsInput.Length;
                            MaxVal = (int)fileSize;
                            byte[] buffer = new byte[len];


                            do
                            {
                                if (CheckCancel)
                                {
                                    fileVal = 0;
                                    CheckCancel = false;
                                    fsOutput.Dispose();


                                    File.WriteAllText(SC, string.Empty);
                                    break;
                                }

                                Thread.Sleep(10);
                                len = fsInput.Read(buffer, 0, buffer.Length);
                                cs.Write(buffer, 0, len);

                                Console.WriteLine(fileSize);
                                fileSize -= len;
                                fileVal += len;

                            } while (len != 0);

                            path1 = "";
                            path2 = "";
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void Decryption()
    {
        if (!File.Exists(path1))
        {
            MessageBox.Show("From File Not Found");

            return;
        }

        if (!File.Exists(path2))
        {
            MessageBox.Show("To File Not Found");

            return;
        }

        try
        {
            using (FileStream fsInput = new FileStream(path1, FileMode.Open))
            {
                using (FileStream fsOutput = new FileStream(path2, FileMode.Create))
                {
                    using (AesManaged aes = new AesManaged())
                    {

                        aes.Key = Key;
                        aes.IV = IV;


                        ICryptoTransform decryptor = aes.CreateDecryptor();
                        using (CryptoStream cs = new CryptoStream(fsOutput, decryptor, CryptoStreamMode.Write))
                        {

                            var len = 10;
                            var fileSize = fsInput.Length;
                            MaxVal = (int)fileSize;

                            byte[] buffer = new byte[len];


                            do
                            {
                                if (CheckCancel)
                                {
                                    FileVal = 0;
                                    CheckCancel = false;
                                    fsOutput.Dispose();

                                    File.WriteAllText(SC, string.Empty);
                                    break;
                                }

                                Thread.Sleep(10);
                                len = fsInput.Read(buffer, 0, buffer.Length); // 8
                                cs.Write(buffer, 0, len);

                                Console.WriteLine(fileSize);
                                fileSize -= len;
                                FileVal += len;


                            } while (len != 0);

                            path1 = "";
                            path2 = "";
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }


    private void cancel_btn_Click(object sender, RoutedEventArgs e)
    {
        CheckCancel = true;
    }
}