using DazPackage;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Daz_Package_Manager
{
    internal class GUISettings : INotifyPropertyChanged
    {
        #region Image Icon Setting
        private double imageSize = Properties.Settings.Default.ImageSize;
        public double ImageSize
        {
            get => imageSize;
            set
            {
                Properties.Settings.Default.ImageSize = value;
                Properties.Settings.Default.Save();
                imageSize = Properties.Settings.Default.ImageSize;
                OnPropertyChanged();
            }
        }

        private bool imageVisible = true;
        public bool ImageVisible
        {
            get => imageVisible;
            set
            {
                imageVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Select Generation
        private Generation showingGeneration = Generation.All;
        public Generation ToggleGeneration
        {
            get => showingGeneration;
            set { showingGeneration ^= value; OnPropertyChanged(); }
        }

        public bool ToggleGen0
        {
            get => ToggleGeneration.HasFlag(Generation.Unknown);
            set => ToggleGeneration = Generation.Unknown;
        }

        public bool ToggleGen4
        {
            get => ToggleGeneration.HasFlag(Generation.Gen4);
            set => ToggleGeneration = Generation.Gen4;
        }

        public bool ToggleGen5
        {
            get => ToggleGeneration.HasFlag(Generation.Genesis_1);
            set => ToggleGeneration = Generation.Genesis_1;
        }

        public bool ToggleGen6
        {
            get => ToggleGeneration.HasFlag(Generation.Genesis_2);
            set => ToggleGeneration = Generation.Genesis_2;
        }

        public bool ToggleGen7
        {
            get => ToggleGeneration.HasFlag(Generation.Genesis_3);
            set => ToggleGeneration = Generation.Genesis_3;
        }

        public bool ToggleGen8
        {
            get => ToggleGeneration.HasFlag(Generation.Genesis_8);
            set => ToggleGeneration = Generation.Genesis_8;
        }
        #endregion

        #region Select Gender
        private Gender showingGender = Gender.All;
        public Gender ToggleGender
        {
            get => showingGender;
            set { showingGender ^= value; OnPropertyChanged(); }
        }

        public bool ToggleMale
        {
            get => ToggleGender.HasFlag(Gender.Male);
            set => ToggleGender = Gender.Male;
        }

        public bool ToggleFemale
        {
            get => ToggleGender.HasFlag(Gender.Female);
            set => ToggleGender = Gender.Female;
        }

        public bool ToggleUnknownGender
        {
            get => ToggleGender.HasFlag(Gender.Unknown);
            set => ToggleGender = Gender.Unknown;
        }
        #endregion
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

}
