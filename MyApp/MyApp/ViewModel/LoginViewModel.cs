using MyApp.Model;
using MyApp.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyApp.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;
        
        
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        public bool IsViewVisible
        {
            get
            {
                return _isViewVisible;
            }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }
        public ICommand LoginCommand { get; }
       

        public LoginViewModel()
        {
           
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            
        }

        private void ExecuteLoginCommand(object obj)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source = NHATLEDUONG\SQLEXPRESS; Initial Catalog = LoginMyApp; Integrated Security = True");
            try
            {
                if (sqlCon.State == System.Data.ConnectionState.Closed)
                {
                    sqlCon.Open();
                    string querry = "Select *From [User] Where Username = @Username And Password = @Password";
                    using (SqlCommand command = new SqlCommand(querry, sqlCon))
                    {
                        // command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@Username", Username);
                        command.Parameters.AddWithValue("@Password", ConvertToUnsecureString(Password));

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Đăng nhập thành công
                                var mainView = new MainView();
                                mainView.Show();
                                Application.Current.MainWindow.Close();
                            }
                            else
                            {
                                // Đăng nhập thất bại
                                ErrorMessage = "Sai tên đăng nhập hoặc mật khẩu";
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

        private string ConvertToUnsecureString(SecureString secureString)
        {
            if (secureString == null)
                return string.Empty;

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        private bool CanExecuteLoginCommand(object obj)
        {
            bool validData = false;
            if(string.IsNullOrWhiteSpace(Username) || Username.Length < 3 ||
                Password == null || Password.Length < 3)
            {
                validData = false;
            }
            else
            {
                validData = true;
            }    
            return validData;
        }
       
    }
}
