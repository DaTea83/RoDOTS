using System;
using Unity.Entities;

namespace EugeneC.ObsoleteV2 {
    
    [Obsolete]
    public class DemoPathwayAgent : GenericPathwayController<DemoPathwayControllerAuthoring,
        DemoPathwayControllerAuthoring.EPathway>.AgentMovementBase {

        public override DemoPathwayControllerAuthoring.EPathway AgentEnum =>
            DemoPathwayControllerAuthoring.EPathway.Type1;

        private class DemoPathwayAgentBaker : Baker<DemoPathwayAgent> {

            public override void Bake(DemoPathwayAgent authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(e, new AgentMovementIData {
                    PathwayId = (byte)authoring.AgentEnum,
                    Speed = authoring.stats.MoveSpeed
                });
            }

        }

    }

}