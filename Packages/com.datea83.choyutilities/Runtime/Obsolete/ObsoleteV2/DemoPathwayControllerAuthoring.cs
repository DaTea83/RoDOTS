using System;
using Unity.Entities;

namespace EugeneC.ObsoleteV2 {

    [Obsolete]
    public class DemoPathwayControllerAuthoring
        : GenericPathwayController<DemoPathwayControllerAuthoring, DemoPathwayControllerAuthoring.EPathway> {

        public enum EPathway {

            Type1,
            Type2,
            Type3

        }

        private class DemoPathwayControllerBaker : Baker<DemoPathwayControllerAuthoring> {

            public override void Bake(DemoPathwayControllerAuthoring authoring) {
                if (authoring.prefabs.Length == 0 || authoring.pathIds.Length == 0) return;
                var e = GetEntity(TransformUsageFlags.Renderable);

                var spline = authoring.BakePathways();
                AddBlobAsset(ref spline, out _);

                AddComponent(e, new AgentPathwaysIData {
                    Pathways = spline
                });
            }

        }

    }

}