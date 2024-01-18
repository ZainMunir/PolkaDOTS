﻿using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace PolkaDOTS.Networking
{
    /// <summary>
    /// Initializes Netcode for Entities (NFE) by creating a <see cref="ClientServerTickRate"/> singleton. This runs on all non-deployment
    /// NFE worlds and sets the tick rates to values specified via command line argument.
    /// </summary>
    /// <remarks>
    /// Client tick rates are currently overriden by values provided to the server the client connects to.
    /// </remarks>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial class SetServerClientNetworkSettingsSystem : SystemBase
    {
        protected override void OnCreate()
        {
            // Create and set a singleton component with the tick rate parameters.
            ClientServerTickRate cst = new ClientServerTickRate {
                NetworkTickRate = ApplicationConfig.NetworkTickRate,
                SimulationTickRate = ApplicationConfig.SimulationTickRate,
                MaxSimulationStepsPerFrame = ApplicationConfig.MaxSimulationStepsPerFrame,
                MaxSimulationStepBatchSize = ApplicationConfig.MaxSimulationStepBatchSize,
                SendSnapshotsForCatchUpTicks = false
            };
            cst.ResolveDefaults();
            EntityManager.CreateSingleton<ClientServerTickRate>(cst,"ClientServerTickRateSingleton");
            Enabled = false;
        }

        protected override void OnUpdate()
        { }
    }
    
    /// <summary>
    /// Initializes Netcode for Entities (NFE) by creating a <see cref="ClientTickRate"/> singleton.
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial class SetClientNetworkSettingsSystem : SystemBase
    {
        protected override void OnCreate()
        {
            // Create and set a singleton component with the tick rate parameters.
            ClientTickRate ctr = NetworkTimeSystem.DefaultClientTickRate;
            ctr.MaxPredictAheadTimeMS = (uint) ApplicationConfig.MaxPredictAheadTimeMS.Value;
            //ctr.ResolveDefaults();
            EntityManager.CreateSingleton<ClientTickRate>(ctr,"ClientTickRateSingleton");
            Enabled = false;
        }

        protected override void OnUpdate()
        { }
    }
}