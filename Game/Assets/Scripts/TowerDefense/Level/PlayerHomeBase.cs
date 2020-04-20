using System.Collections.Generic;
using ActionGameFramework.Audio;
using Core.Health;
using FMODUnity;
using TowerDefense.Agents;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// A class representing the home base that players must defend
	/// </summary>
	public class PlayerHomeBase : DamageableBehaviour
	{
		/// <summary>
		/// The particle system when an attack is charging
		/// </summary>
		public ParticleSystem chargePfx;

		/// <summary>
		/// Sound to play when charge effect starts
		/// </summary>
		public StudioEventEmitter chargeSound;

        /// <summary>
        /// Sound to play when charge efffect ends and so does everything else
        /// </summary>
        public StudioEventEmitter atttackSound;


        public StudioEventEmitter bgm;
        FMOD.Studio.EventInstance inst;
        FMOD.Studio.PARAMETER_ID healthId;

		/// <summary>
		/// The particle system for an attack
		/// </summary>
		public ParticleSystem attackPfx;

        [FMODUnity.EventRef]
        string BaseDamagedSound;

		/// <summary>
		/// The current Agents within the home base attack zone
		/// </summary>
		protected List<Agent> m_CurrentAgentsInside = new List<Agent>();

		/// <summary>
		/// Subscribes to damaged event
		/// </summary>
		protected virtual void Start()
		{
			configuration.damaged += OnDamaged;
            inst = bgm.EventInstance;
            healthId = Bleh.GetIdFromEvent(bgm.Event, "Health");
		}

		/// <summary>
		/// Unsubscribes to damaged event
		/// </summary>
		protected virtual void OnDestroy()
		{
			configuration.damaged -= OnDamaged;
		}

		/// <summary>
		/// Plays <see cref="attackPfx"/> if assigned
		/// </summary>
		protected virtual void OnDamaged(HealthChangeInfo obj)
		{
			if (attackPfx != null)
			{
				attackPfx.Play();
			}
			if (atttackSound != null)
			{
                //attackSound.PlayRandomClip();
                atttackSound.Play();
			}
            float h = configuration.currentHealth * 10f / configuration.maxHealth;
            inst.setParameterByID(healthId, h);
		}
		
		/// <summary>
		/// Adds triggered Agent to tracked Agents, subscribes to Agent's
		/// removed event and plays pfx
		/// </summary>
		/// <param name="other">Triggered collider</param>
		void OnTriggerEnter(Collider other)
		{
			var homeBaseAttacker = other.GetComponent<HomeBaseAttacker>();
			if (homeBaseAttacker == null)
			{
				return;
			}
			m_CurrentAgentsInside.Add(homeBaseAttacker.agent);
			homeBaseAttacker.agent.removed += OnAgentRemoved;
			if (chargePfx != null)
			{
				chargePfx.Play();
			}
			if (chargeSound != null)
			{
				chargeSound.Play();
			}
		}
		
		/// <summary>
		/// If the entity that has entered the collider
		/// has an <see cref="Agent"/> component on it
		/// </summary>
		void OnTriggerExit(Collider other)
		{
			var homeBaseAttacker = other.GetComponent<HomeBaseAttacker>();
			if (homeBaseAttacker == null)
			{
				return;
			}
			RemoveTarget(homeBaseAttacker.agent);
		}
		
		/// <summary>
		/// Removes Agent from tracked <see cref="m_CurrentAgentsInside"/>
		/// </summary>
		void OnAgentRemoved(DamageableBehaviour targetable)
		{
			targetable.removed -= OnAgentRemoved;
			Agent attackingAgent = targetable as Agent;
			RemoveTarget(attackingAgent);
		}

		/// <summary>
		/// Removes <paramref name="agent"/> from <see cref="m_CurrentAgentsInside"/> and stops pfx 
		/// if there are no more <see cref="Agent"/>s
		/// </summary>
		/// <param name="agent">
		/// The agent to remove
		/// </param>
		void RemoveTarget(Agent agent)
		{
			if (agent == null)
			{
				return;
			}
			m_CurrentAgentsInside.Remove(agent);
			if (m_CurrentAgentsInside.Count == 0 && chargePfx != null)
			{
				chargePfx.Stop();
			}
		}
	}
}