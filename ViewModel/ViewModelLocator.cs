/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PoliceSoft.Aquas.Model.Initializer"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using PoliceSoft.Aquas.Model.Initializer.Services;
using PoliceSoft.Aquas.Model.Initializer.Views;

namespace PoliceSoft.Aquas.Model.Initializer.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			////if (ViewModelBase.IsInDesignModeStatic)
			////{
			////    // Create design time view services and models
			////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
			////}
			////else
			////{
			////    // Create run time view services and models
			////    SimpleIoc.Default.Register<IDataService, DataService>();
			////}

			SimpleIoc.Default.Register<IConnectionService>(() => new SqlServerConnectionService());

			SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<ConnectionDialogViewModel>();
			SimpleIoc.Default.Register<ErrorDialogViewModel>();

			SimpleIoc.Default.Register<IConnectionDialog>(() => new ConnectionDialog());
			SimpleIoc.Default.Register<IErrorDialog>(() => new ErrorDialog());
		}

        public MainViewModel Main
        {
            get { return GetInstance<MainViewModel>(); }
        }

		public ConnectionDialogViewModel ConnectionDialog
		{
			get { return GetInstance<ConnectionDialogViewModel>(); }
		}

		public ErrorDialogViewModel ErrorDialog
		{
			get { return GetInstance<ErrorDialogViewModel>(); }
		}

		public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

		private TInstance GetInstance<TInstance>()
		{
			return ServiceLocator.Current.GetInstance<TInstance>();
		}
    }
}