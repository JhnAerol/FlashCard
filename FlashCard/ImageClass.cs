using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCard
{
    public class ImageClass : INotifyPropertyChanged
    {
        string imageFile;
        public string ImageFile
        {
            get
            {
                return imageFile;
            }
            set
            {
                if (imageFile != value)
                {
                    imageFile = value;
                    OnPropertyChanged(nameof(ImageFile));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
