namespace EvAlex.AwesomeEfMigrator.Views
{
	public interface IDialog : IWindow
	{
		bool? ShowDialog();

		void Close();
	}
}