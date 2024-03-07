using Content.Server.Construction.Components;
using Content.Shared.Construction;
using Content.Shared.Construction.Components;
using JetBrains.Annotations;
using Robust.Server.Containers;

namespace Content.Server.Construction.NodeEntities;

/// <summary>
///     Works for both <see cref="ComputerBoardComponent"/> and <see cref="MachineBoardComponent"/>
///     because duplicating code just for this is really stinky.
/// </summary>
[UsedImplicitly]
[DataDefinition]
public sealed partial class BoardNodeEntity : IGraphNodeEntity
{
    [DataField("container")] public string Container { get; private set; } = string.Empty;

    public string? GetId(EntityUid? uid, EntityUid? userUid, GraphNodeEntityArgs args)
    {
        if (uid == null)
            return null;

        var containerSystem = args.EntityManager.EntitySysManager.GetEntitySystem<ContainerSystem>();

        if (!containerSystem.TryGetContainer(uid.Value, Container, out var container)
            || container.ContainedEntities.Count == 0)
            return null;

        var board = container.ContainedEntities[0];

        // Frontier - adds tabletop variants
        if (args.EntityManager.TryGetComponent(container.Owner, out ConstructionComponent? constructionComponent)
            && constructionComponent.Graph == "GraphComputerTabletop"
            && args.EntityManager.TryGetComponent(board, out ComputerTabletopBoardComponent? tabletopComputer))
        {
            return tabletopComputer.Prototype;
        }

        // There should not be a case where both of these components exist on the same entity...
        if (args.EntityManager.TryGetComponent(board, out MachineBoardComponent? machine))
            return machine.Prototype;

        if(args.EntityManager.TryGetComponent(board, out ComputerBoardComponent? computer))
            return computer.Prototype;

        return null;
    }
}
