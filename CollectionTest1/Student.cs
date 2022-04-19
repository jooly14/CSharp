using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace CollectionTest1
{
    class Student : INotifyPropertyChanged
    {
        public Student()
        {
            BtnClick = new RelayCommand(btnclick);
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string name;

        public bool IsEnable
        {
            get
            {
                return isEnable;
            }
            set
            {
                isEnable = value;
                OnPropertyChanged("IsEnable");
            }
        }
        public bool isEnable;

        public ICommand BtnClick { get; set; }
        
        public void btnclick ()
        {
            if (IsEnable)
            {
                IsEnable = false;
                Name = "No";
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
