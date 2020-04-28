using Core.UI;
using TowerDefense.Game;
using UnityEngine.UI;

namespace TowerDefense.UI
{
	/// <summary>
	/// Simple options menu for setting volumes 
	/// </summary>
	public class OptionsMenu : SimpleMainMenuPage
	{
		public Slider masterSlider;

		public Slider sfxSlider;
		
		public Slider musicSlider;

        public Slider ambiSlider;

		/// <summary>
		/// Event fired when sliders change
		/// </summary>
		public void UpdateVolumes()
		{
			float masterVolume, sfxVolume, musicVolume, ambiVolume;
			GetSliderVolumes(out masterVolume, out sfxVolume, out musicVolume, out ambiVolume);

			if (GameManager.instanceExists)
			{
				GameManager.instance.SetVolumes(masterVolume, sfxVolume, musicVolume, ambiVolume, false);
			}
		}

		/// <summary>
		/// Set initial slider values
		/// </summary>
		public override void Show()
		{
			if (GameManager.instanceExists)
			{
				float master, sfx, music, ambi;
				GameManager.instance.GetVolumes(out master, out sfx, out music, out ambi);

				if (masterSlider != null)
				{
					masterSlider.value = master;
				}
				if (sfxSlider != null)
				{
					sfxSlider.value = sfx;
				}
				if (musicSlider != null)
				{
					musicSlider.value = music;
				}
                if (ambiSlider != null)
                {
                    ambiSlider.value = ambi;
                }
			}

			base.Show();
		}

		/// <summary>
		/// Persist volumes to data store
		/// </summary>
		public override void Hide()
		{
			float masterVolume, sfxVolume, musicVolume, ambiVolume;
			GetSliderVolumes(out masterVolume, out sfxVolume, out musicVolume, out ambiVolume);

			if (GameManager.instanceExists)
			{
				GameManager.instance.SetVolumes(masterVolume, sfxVolume, musicVolume, ambiVolume, true);
			}

			base.Hide();
		}

		/// <summary>
		/// Retrieve values from sliders
		/// </summary>
		void GetSliderVolumes(out float masterVolume, out float sfxVolume, out float musicVolume, out float ambiVolume)
		{
			masterVolume = masterSlider != null ? masterSlider.value : 1;
			sfxVolume = sfxSlider != null ? sfxSlider.value : 1;
			musicVolume = musicSlider != null ? musicSlider.value : 1;
            ambiVolume = ambiSlider != null ? ambiSlider.value : 1;
		}
	}
}