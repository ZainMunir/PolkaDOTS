﻿using PolkaDOTS.Deployment;
using PolkaDOTS.Multiplay;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

namespace PolkaDOTS.Emulation
{
    /// <summary>
    ///  Sets up and starts emulated player input 
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class EmulationInitSystem : SystemBase
    {
        private EntityQuery connections;
        protected override void OnCreate()
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<NetworkId, NetworkStreamInGame>();
            connections = GetEntityQuery(builder);
        }

        protected override void OnUpdate()
        {
            Emulation emulation = EmulationSingleton.Instance;
            Multiplay.Multiplay multiplay = MultiplaySingleton.Instance;
            if (emulation is null || multiplay is null)
                return;
            // Wait for either clientworld to be connected to the server, or for a guest client to be connected
            if (!(multiplay.IsGuestConnected() || !connections.IsEmpty))
            {
                return;
            }
            Enabled = false;

            emulation.emulationType = ApplicationConfig.EmulationType;
            if (World.Unmanaged.IsSimulatedClient())
            {
                emulation.emulationType = EmulationType.Simulation;
            }
            Debug.Log($"Emulation type is {emulation.emulationType}");
            
            // Multiplay guest emulation only supports input playback
            if (ApplicationConfig.MultiplayStreamingRole == MultiplayStreamingRoles.Guest && (emulation.emulationType & EmulationType.Simulation) == EmulationType.Simulation)
            {
                Debug.Log("Multiplay guest emulation only supports input playback, switching to it.");
                emulation.emulationType ^= EmulationType.Simulation;
                emulation.emulationType |= EmulationType.Playback;
            }
            
            if((emulation.emulationType & EmulationType.Record) == EmulationType.Record)
                emulation.initializeRecording();
            if((emulation.emulationType & EmulationType.Playback) == EmulationType.Playback)
                emulation.initializePlayback();
            if ((emulation.emulationType & EmulationType.Simulation) == EmulationType.Simulation)
                emulation.initializeSimulation();
        }
    }
}