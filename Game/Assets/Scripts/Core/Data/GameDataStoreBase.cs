namespace Core.Data
{
	/// <summary>
	/// Base game data store for GameManager to save, containing only data for saving volumes
	/// </summary>
	public abstract class GameDataStoreBase : IDataStore
	{
		/// <summary>
		/// Called just before we save
		/// </summary>
		public abstract void PreSave();

		/// <summary>
		/// Called just after load
		/// </summary>
		public abstract void PostLoad();
	}
}