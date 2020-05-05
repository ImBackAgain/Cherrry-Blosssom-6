using System;
using Core.Utilities;
using UnityEngine;
using UnityEngine.Audio;
using FMOD.Studio;
using FMODUnity;

namespace Core.Data
{
	/// <summary>
	/// Base game manager
	/// </summary>
	public abstract class GameManagerBase<TGameManager, TDataStore> : PersistentSingleton<TGameManager>
		where TDataStore : GameDataStoreBase, new()
		where TGameManager : GameManagerBase<TGameManager, TDataStore>
	{
        Bus masterBus;
        VCA musicVCA, sfxVCA, ambiVCA;
        const string masterBusName = "Bus:/", musicVcaName = "vca:/Music", sfxVcaName = "vca:/SFX", ambiVcaName = "vca:/Ambience";
        const string masterKey = "Enan|master", musicKey = "Enan/music", sfxKey = "Enan:sfx", ambiKey = "I just neeed unique names here";
                        //Hahaha
		/// <summary>
		/// File name of saved game
		/// </summary>
		const string k_SavedGameFile = "save";

		/// <summary>
		/// Reference to audio mixer for volume changing
		/// </summary>
		public AudioMixer gameMixer;

		/// <summary>
		/// Master volume parameter on the mixer
		/// </summary>
		public string masterVolumeParameter;

		/// <summary>
		/// SFX volume parameter on the mixer
		/// </summary>
		public string sfxVolumeParameter;

		/// <summary>
		/// Music volume parameter on the mixer
		/// </summary>
		public string musicVolumeParameter;

		/// <summary>
		/// The serialization implementation for persistence 
		/// </summary>
		protected JsonSaver<TDataStore> m_DataSaver;

		/// <summary>
		/// The object used for persistence
		/// </summary>
		protected TDataStore m_DataStore;

		/// <summary>
		/// Retrieve volumes from data store
		/// </summary>
		public virtual void GetVolumes(out float master, out float sfx, out float music, out float ambi)
		{
			master = m_DataStore.masterVolume;
			sfx = m_DataStore.sfxVolume;
			music = m_DataStore.musicVolume;
            ambi = m_DataStore.ambiVolume;
		}

		/// <summary>
		/// Set and persist game volumes
		/// </summary>
		public virtual void SetVolumes(float master, float sfx, float music, float ambi, bool save)
		{
            // Early out if no mixer set
            //if (gameMixer == null)
            //{
            //	return;
            //}

            // Transform 0-1 into logarithmic -80-0
            //if (masterVolumeParameter != null)
            //{
            //	gameMixer.SetFloat(masterVolumeParameter, LogarithmicDbTransform(Mathf.Clamp01(master)));
            //}
            //if (sfxVolumeParameter != null)
            //{
            //	gameMixer.SetFloat(sfxVolumeParameter, LogarithmicDbTransform(Mathf.Clamp01(sfx)));
            //}
            //if (musicVolumeParameter != null)
            //{
            //	gameMixer.SetFloat(musicVolumeParameter, LogarithmicDbTransform(Mathf.Clamp01(music)));
            //}
            //string s;
            //ambiVCA.getPath(out s);
            //print(s);
            masterBus.setVolume(master);
            sfxVCA.setVolume(sfx);
            musicVCA.setVolume(music);
            ambiVCA.setVolume(ambi);


            print("Settting volume:");
            /*
            print(ambi);
            /*/
            float x; ambiVCA.getVolume(out x); print(x);
            //*/
            if (save)
            {
                print("saving:");
                print(m_DataStore.ambiVolume);
                m_DataSaver.Save(m_DataStore);
                PlayerPrefs.SetFloat(masterKey, master);
                PlayerPrefs.SetFloat(musicKey, music);
                PlayerPrefs.SetFloat(sfxKey, sfx);
                PlayerPrefs.SetFloat(ambiKey, ambi);
            }
		}

		/// <summary>
		/// Load data
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			LoadData();
            //print("Loading data!");
            masterBus = RuntimeManager.GetBus(masterBusName);
            sfxVCA = RuntimeManager.GetVCA(sfxVcaName);
            musicVCA = RuntimeManager.GetVCA(musicVcaName);
            ambiVCA = RuntimeManager.GetVCA(ambiVcaName);
            //SetVolumes(m_DataStore.masterVolume, m_DataStore.sfxVolume, m_DataStore.musicVolume, m_DataStore.ambiVolume, false);
		}

		/// <summary>
		/// Initialize volumes. We cannot change mixer params on awake
		/// </summary>
		protected virtual void Start()
		{
            //float x;
            //musicVCA.getVolume(out x);
            //print(x);

            //masterBus.getVolume(out x);
            //print(x);

            SetVolumes(m_DataStore.masterVolume, m_DataStore.sfxVolume, m_DataStore.musicVolume, m_DataStore.ambiVolume, false);
		}

		/// <summary>
		/// Set up persistence
		/// </summary>
		protected void LoadData()
		{
			// If it is in Unity Editor use the standard JSON (human readable for debugging) otherwise encrypt it for deployed version
#if UNITY_EDITOR
			m_DataSaver = new JsonSaver<TDataStore>(k_SavedGameFile);
#else
			m_DataSaver = new EncryptedJsonSaver<TDataStore>(k_SavedGameFile);
#endif

			try
			{
				if (!m_DataSaver.Load(out m_DataStore))
				{
                    m_DataStore = new TDataStore();
                    print(m_DataStore.ambiVolume);
					SaveData();
				}
                else
                {
                    SaveData();
                }
			}
			catch (Exception)
			{
				Debug.LogError("Failed to load data, resetting");
                m_DataStore = new TDataStore();
                SaveData();
			}
		}

		/// <summary>
		/// Saves the gamme
		/// </summary>
		protected virtual void SaveData()
		{
			m_DataSaver.Save(m_DataStore);
        }

		/// <summary>
		/// Transform volume from linear to logarithmic
		/// </summary>
		protected static float LogarithmicDbTransform(float volume)
		{
			volume = (Mathf.Log(89 * volume + 1) / Mathf.Log(90)) * 80;
			return volume - 80;
		}
	}
}