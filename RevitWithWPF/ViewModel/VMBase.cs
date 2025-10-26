using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RevitWithWPF.ViewModel
{
    public class VMBase : INotifyPropertyChanged
    {

        #region PropertyChanged Event Handling
        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged([CallerMemberName] string propertyname = null) 
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyname)); 
        }
        #endregion 

       
    }
}
