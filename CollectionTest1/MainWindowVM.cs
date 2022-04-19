using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace CollectionTest1
{
    class MainWindowVM : INotifyPropertyChanged
    {
        private ObservableCollection<Student> students;
        public ObservableCollection<Student> Students
        {
            get
            {
                return students;
            }

        }

        public string Title
        {
            get
            {
                return "This is Title";
            }
        }

        public MainWindowVM()
        {
            students = new ObservableCollection<Student>();
            students.Add(new Student() { Name = "No", IsEnable = false});
            students.Add(new Student() { Name = "Yes", IsEnable = true});
            students.Add(new Student() { Name = "Yes", IsEnable = true});
            students.Add(new Student() { Name = "Yes", IsEnable = true});

        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
