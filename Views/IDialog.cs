namespace PoliceSoft.Aquas.Model.Initializer.Views
{
	public interface IDialog : IWindow
	{
		bool? ShowDialog();

		void Close();
	}
}